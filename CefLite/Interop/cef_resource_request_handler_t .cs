using System;
using System.Collections.Generic;
using System.Linq;
using CefLite.Interop;
using System.Threading.Tasks;

using System.Runtime.InteropServices;


namespace CefLite.Interop
{

    [StructLayout(LayoutKind.Sequential, Pack = 0)]
    public unsafe struct cef_resource_request_handler_t
    {
        public cef_base_ref_counted_t brc;
        public IntPtr get_cookie_access_filter;
        public IntPtr on_before_resource_load;
        public IntPtr get_resource_handler;

        public IntPtr on_resource_redirect;
        public IntPtr on_resource_response;
        public IntPtr get_resource_response_filter;
        public IntPtr on_resource_load_complete;
        public IntPtr on_protocol_execution;
    }

    public unsafe partial class CefResourceRequestHandler : ObjectFromNet<cef_resource_request_handler_t, CefResourceRequestHandler>
    {
        public cef_resource_request_handler_t* FixedPtr => (cef_resource_request_handler_t*)Ptr;

        public CefResourceRequestHandler()
        {
            FixedPtr->on_before_resource_load = holder_on_before_resource_load;
        }

        delegate cef_return_value_t func_on_before_resource_load(IntPtr self, cef_browser_t* browser, cef_frame_t* frame, cef_request_t* request, cef_request_callback_t* callback);
        static DelegateHolder<func_on_before_resource_load> holder_on_before_resource_load = new DelegateHolder<func_on_before_resource_load>(
            delegate (IntPtr self, cef_browser_t* browser, cef_frame_t* frame, cef_request_t* request, cef_request_callback_t* callback)
            {
                var inst = GetInstance(self);
                return inst.BeforeResourceLoad?.Invoke(inst, CefBrowser.FromInArg(browser), CefFrame.FromInArg(frame), CefRequest.FromInArg(request), CefRequestCallback.FromInArg(callback))
                    ?? cef_return_value_t.RV_CONTINUE;
            });
        public Func<CefResourceRequestHandler, CefBrowser, CefFrame, CefRequest, CefRequestCallback, cef_return_value_t> BeforeResourceLoad { get; set; }
    }

    [StructLayout(LayoutKind.Sequential, Pack = 0)]
    public unsafe struct cef_cookie_access_filter_t
    {
        public IntPtr can_send_cookie;
        public IntPtr can_save_cookie;
    }
}
