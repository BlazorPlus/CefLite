using System;
using System.Collections.Generic;
using System.Linq;
using CefLite.Interop;
using System.Threading.Tasks;

using System.Runtime.InteropServices;


namespace CefLite.Interop
{

    [StructLayout(LayoutKind.Sequential, Pack = 0)]
    public unsafe struct cef_request_handler_t
    {
        public cef_base_ref_counted_t brc;
        public IntPtr on_before_browse;
        public IntPtr on_open_urlfrom_tab;
        public IntPtr get_resource_request_handler;
        public IntPtr get_auth_credentials;
        public IntPtr on_quota_request;
        public IntPtr on_certificate_error;
        public IntPtr on_select_client_certificate;
        public IntPtr on_plugin_crashed;
        public IntPtr on_render_view_ready;
        public IntPtr on_render_process_terminated;
    }

    public unsafe class CefRequestHandler : ObjectFromNet<cef_request_handler_t, CefRequestHandler>
    {
        public cef_request_handler_t* FixedPtr => (cef_request_handler_t*)Ptr;

        public CefClient Client { get; private set; }
        public CefRequestHandler(CefClient ownerClient)
        {
            Client = ownerClient;

            cef_request_handler_t* self = FixedPtr;
            self->get_resource_request_handler = holder_get_resource_request_handler;
            self->on_certificate_error = holder_on_certificate_error;
            self->on_before_browse = holder_on_before_browse;
            self->on_open_urlfrom_tab = holder_on_open_urlfrom_tab;
        }

        public CefResourceRequestHandler ResourceRequestHandler { get; set; } = new CefResourceRequestHandler();

        static DelegateHolder<GetObjectHandler> holder_get_resource_request_handler = new DelegateHolder<GetObjectHandler>(
           app => GetInstance(app).ResourceRequestHandler.AddRefReturnIntPtr());

        static DelegateHolder<delegate_on_certificate_error> holder_on_certificate_error = new DelegateHolder<delegate_on_certificate_error>(
            (cef_request_handler_t* handler, cef_browser_t* browser, int errcode, cef_string_t* url, cef_sslinfo_t* sslinfo, cef_request_callback_t* callback) =>
            {
                string cefurl = cef_string_t.ToString(url);
                switch (errcode)
                {
                    case -200:  //CERT_COMMON_NAME_INVALID
                        break;
                }
                CefWin.WriteDebugLine("on_certificate_error:" + errcode + "," + cefurl);
                string hostname = cefurl.Split("://")[1].Split('/')[0].Split(':')[0];
                if (hostname.StartsWith("127.") && System.Net.IPAddress.TryParse(hostname, out var ip))
                {
                    CefWin.WriteDebugLine("Allow IP : " + hostname);
                    CefRequestCallback cefcallback = CefRequestCallback.FromNative(callback);
                    cefcallback.Cont();
                    return 1;
                }
                CefWin.WriteDebugLine("Don't host : " + hostname);
                return 0;
            });
        delegate int delegate_on_certificate_error(cef_request_handler_t* handler, cef_browser_t* browser, int errcode, cef_string_t* url, cef_sslinfo_t* sslinfo, cef_request_callback_t* callback);


        delegate int func_on_before_browse(IntPtr self, cef_browser_t* browser, cef_frame_t* frame, cef_request_t* request, int user_gesture, int is_redirect);
        static DelegateHolder<func_on_before_browse> holder_on_before_browse = new DelegateHolder<func_on_before_browse>(
            delegate (IntPtr self, cef_browser_t* browser, cef_frame_t* frame, cef_request_t* request, int user_gesture, int is_redirect)
            {
                var inst = GetInstance(self);
                return inst.BeforeBrowse?.Invoke(inst, CefBrowser.FromNative(browser), CefFrame.FromNative(frame), CefRequest.FromNative(request), user_gesture, is_redirect)
                    ?? 0;//TODO: option
            });
        public Func<CefRequestHandler, CefBrowser, CefFrame, CefRequest, int, int, int> BeforeBrowse { get; set; }


        delegate int func_on_open_urlfrom_tab(IntPtr self, cef_browser_t* browser, cef_frame_t* frame, IntPtr targeturl, cef_window_open_disposition_t disposition);
        static DelegateHolder<func_on_open_urlfrom_tab> holder_on_open_urlfrom_tab = new DelegateHolder<func_on_open_urlfrom_tab>(
            delegate (IntPtr self, cef_browser_t* browser, cef_frame_t* frame, IntPtr targeturl, cef_window_open_disposition_t disposition)
            {
                var inst = GetInstance(self);
                inst.OpenUrlFromTab?.Invoke(inst, CefBrowser.FromNative(browser), CefFrame.FromNative(frame), cef_string_t.ToString(targeturl), disposition);

                return 1;//always cancel the default popup
            });

        public Action<CefRequestHandler, CefBrowser, CefFrame, string, cef_window_open_disposition_t> OpenUrlFromTab { get; set; }
    }

}
