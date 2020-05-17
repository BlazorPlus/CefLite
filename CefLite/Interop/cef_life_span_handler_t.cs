using System;
using System.Collections.Generic;
using System.Linq;
using CefLite.Interop;
using System.Threading.Tasks;

using System.Runtime.InteropServices;


namespace CefLite.Interop
{

    [StructLayout(LayoutKind.Sequential, Pack = 0)]
    public unsafe struct cef_life_span_handler_t
    {
        public cef_base_ref_counted_t brc;
        public IntPtr on_before_popup;
        public IntPtr on_after_created;
        public IntPtr do_close;
        public IntPtr on_before_close;

    }

    public unsafe partial class CefLifeSpanHandler : ObjectFromNet<cef_life_span_handler_t, CefLifeSpanHandler>
    {
        public cef_life_span_handler_t* FixedPtr => (cef_life_span_handler_t*)Ptr;


        public CefClient Client { get; private set; }
        public CefLifeSpanHandler(CefClient ownerClient)
        {
            Client = ownerClient; 
            cef_life_span_handler_t* self = FixedPtr;
            self->on_after_created = holder_on_after_created;
            self->on_before_close = holder_on_before_close;
            self->on_before_popup = holder_on_before_popup;
        }

        static DelegateHolder<EventCallbackHandler2> holder_on_after_created = new DelegateHolder<EventCallbackHandler2>(
            delegate (IntPtr self, IntPtr browser)
            {
                var inst = GetInstance(self);
                CefWin.WriteDebugLine("on_after_created:" + CefBrowser.FromInArg(browser).Identifier);
                inst.AfterCreated?.Invoke(inst, CefBrowser.FromInArg(browser));
            });

        public Action<CefLifeSpanHandler, CefBrowser> AfterCreated { get; set; }

        static DelegateHolder<EventCallbackHandler2> holder_on_before_close = new DelegateHolder<EventCallbackHandler2>(
            delegate (IntPtr self, IntPtr browser)
            {
                var inst = GetInstance(self);
                var browserObj = CefBrowser.FromInArg(browser);
                browserObj._OnBeforeClose();
                CefWin.WriteDebugLine("on_before_close:" + CefBrowser.FromInArg(browser).Identifier);
                inst.BeforeClose?.Invoke(inst, CefBrowser.FromInArg(browser));
            });

        public Action<CefLifeSpanHandler, CefBrowser> BeforeClose { get; set; }

        delegate int func_onbeforepopup(IntPtr self, cef_browser_t* browser, cef_frame_t* frame, cef_string_t* url, cef_string_t* name, cef_window_open_disposition_t dispostion, int user_gesture, cef_popup_features_t* features, cef_window_info_t* wininfo, ref cef_client_t* client, cef_browser_settings_t* settings, cef_dictionary_value_t* extra_info, ref int no_javascript_access);
        static DelegateHolder<func_onbeforepopup> holder_on_before_popup = new DelegateHolder<func_onbeforepopup>(
            delegate (IntPtr self, cef_browser_t* browser, cef_frame_t* frame, cef_string_t* url, cef_string_t* name, cef_window_open_disposition_t dispostion, int user_gesture, cef_popup_features_t* features, cef_window_info_t* wininfo, ref cef_client_t* client, cef_browser_settings_t* settings, cef_dictionary_value_t* extra_info, ref int no_javascript_access)
            {
                CefWin.WriteDebugLine("holder_on_before_popup:" + cef_string_t.ToString(url) + ":" + no_javascript_access);

                var inst = GetInstance(self); 
                var handler = inst.BeforePopup;
                if (handler == null)
                    return 0;//TODO:option

                CefClient _client = client == null ? null : CefClient.GetInstance((IntPtr)client);
                CefClient _prevclient = _client;
                int res = handler.Invoke(inst, CefBrowser.FromInArg(browser), CefFrame.FromInArg(frame), cef_string_t.ToString(url), cef_string_t.ToString(name), dispostion, user_gesture, CefPopupFeatures.FromInArg(features), CefWindowInfo.FromInArg(wininfo)
                    , ref _client, CefBrowserSettings.FromInArg(settings), CefDictionaryValue.FromInArg(extra_info), ref no_javascript_access);
                //TODO:not checked addref/release rules
                if (_client != _prevclient)
                {
                    if (_client == null)
                    {
                        client = null;
                    }
                    else
                    {
                        _client.AddRef();
                        client = _client.FixedPtr;
                    }
                }
                return res;
            });
        public CallbackOnBeforePopup BeforePopup { get; set; }

    }

}
