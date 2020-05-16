using System;
using System.Collections.Generic;
using System.Linq;
using CefLite.Interop;
using System.Threading.Tasks;

using System.Runtime.InteropServices;


namespace CefLite.Interop
{

    [StructLayout(LayoutKind.Sequential, Pack = 0)]
    public unsafe struct cef_render_process_handler_t
    {
        public cef_base_ref_counted_t brc;
        public IntPtr on_render_thread_created;
        public IntPtr on_web_kit_initialized;
        public IntPtr on_browser_created;
        public IntPtr on_browser_destroyed;
        public IntPtr get_load_handler;
        public IntPtr on_context_created;            //void(CEF_CALLBACK* on_context_released)(_cef_render_process_handler_t* self,_cef_browser_t* browser,_cef_frame_t* frame,_cef_v8context_t* context);
        public IntPtr on_context_released;
        public IntPtr on_uncaught_exception;
        public IntPtr on_focused_node_changed;
        public IntPtr on_process_message_received;  // Broser process cef_frame_t::send_process_message
    }

    public unsafe class CefRenderProcessHandler : ObjectFromNet<cef_render_process_handler_t, CefRenderProcessHandler>
    {
        public cef_render_process_handler_t* FixedPtr => (cef_render_process_handler_t*)Ptr;

        public CefRenderProcessHandler()
        {
            cef_render_process_handler_t* self = FixedPtr;
            self->on_process_message_received = holder_on_process_message_received;
            self->on_context_created = holder_on_context_created;
            self->on_web_kit_initialized = holder_on_web_kit_initialized;
        }

        static DelegateHolder<EventCallbackHandler4> holder_on_context_created = new DelegateHolder<EventCallbackHandler4>(
          (ptr, browser, frame, v8c) =>
          {
              CefBrowser browserObj = CefBrowser.FromNative(browser);
              CefFrame frameObj = CefFrame.FromNative(frame);
              string url = frameObj.Url;

              long id = browserObj.Identifier;
              CefWin.WriteDebugLine("on_context_created id:" + id + " , " + url);

              //unknown case , url is null
              if (url == null || !url.StartsWith("http"))  //avoid devtools:// etc
                  return;

              CefV8Context ctx = CefV8Context.FromNative(v8c);

              try
              {
                  ctx.Eval("window.CefWinBrowserId=" + id + ";document.cookie='CefWinBrowserId=" + id + ";path =/'");
              }
              catch (Exception x)
              {
                  CefWin.WriteDebugLine(x);
              }

              var self = GetInstance(ptr);
              self.ContextCreated?.Invoke(self, browserObj, CefFrame.FromNative(frame), ctx);
          });
        public Action<CefRenderProcessHandler, CefBrowser, CefFrame, CefV8Context> ContextCreated { get; set; }


        static DelegateHolder<EventCallbackHandler> holder_on_web_kit_initialized = new DelegateHolder<EventCallbackHandler>(
            ptr =>
            {
                var inst = GetInstance(ptr);
                inst.WebKitInitialized?.Invoke(inst);
            });
        public Action<CefRenderProcessHandler> WebKitInitialized { get; set; }



        delegate int delegate_on_process_message_received(IntPtr handler, cef_browser_t* browser, cef_frame_t* frame, cef_process_id_t pid, cef_process_message_t* message);
        static DelegateHolder<delegate_on_process_message_received> holder_on_process_message_received = new DelegateHolder<delegate_on_process_message_received>(
            (IntPtr handler, cef_browser_t* browser, cef_frame_t* frame, cef_process_id_t pid, cef_process_message_t* message) =>
            {
                CefProcessMessage cefmsg = message;
                CefWin.WriteDebugLine("CefRenderProcessHandler:on_process_message_received:" + cefmsg.ToString());
                CefRenderProcessHandler inst = GetInstance(handler); ;
                inst.ProcessMessageReceived?.Invoke(inst, CefBrowser.FromNative(browser), CefFrame.FromNative(frame), pid, cefmsg);
                return 0;
            });
        public Action<CefRenderProcessHandler, CefBrowser, CefFrame, cef_process_id_t, CefProcessMessage> ProcessMessageReceived { get; set; }

    }

}
