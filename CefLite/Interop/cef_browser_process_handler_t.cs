using System;
using System.Collections.Generic;
using System.Linq;
using CefLite.Interop;
using System.Threading.Tasks;

using System.Runtime.InteropServices;


namespace CefLite.Interop
{

    [StructLayout(LayoutKind.Sequential, Pack = 0)]
    public unsafe struct cef_browser_process_handler_t
    {
        static public EventCallbackHandler s_on_context_initialized;

        public cef_base_ref_counted_t brc;
        public IntPtr on_context_initialized;
        public IntPtr on_before_child_process_launch;
        public IntPtr on_render_process_thread_created;
        public IntPtr get_print_handler;                  //  cef_print_handler_t
        public IntPtr on_schedule_message_pump_work;
    }

    public unsafe class CefBrowserProcessHandler : ObjectFromNet<cef_browser_process_handler_t, CefBrowserProcessHandler>
    {
        public cef_browser_process_handler_t* FixedPtr => (cef_browser_process_handler_t*)Ptr;

        public CefBrowserProcessHandler()
        {
            cef_browser_process_handler_t* self = FixedPtr;
            self->on_context_initialized = holder_on_context_initialized;
            self->on_schedule_message_pump_work = holder_on_schedule_message_pump_work;
            self->on_before_child_process_launch = holder_on_before_child_process_launch;
        }

        static DelegateHolder<EventCallbackHandler> holder_on_context_initialized = new DelegateHolder<EventCallbackHandler>(
            (ptr) =>
            {
                CefWin.WriteDebugLine("Debug:CefBrowserProcessHandler:on_context_initialized");
                var inst = GetInstance(ptr);
                inst.ContextInitialized?.Invoke(inst);
            });

        public Action<CefBrowserProcessHandler> ContextInitialized;

        static DelegateHolder<EventCallbackHandler2> holder_on_before_child_process_launch = new DelegateHolder<EventCallbackHandler2>(
            (ptr, pcmdline) =>
            {
                //CefWin.WriteDebugLine("Debug:CefBrowserProcessHandler:on_before_child_process_launch");
                //CefWin.WriteDebugLine(CefCommandLine.FromNative(pcmdline).GetCommandLineString());
                var inst = GetInstance(ptr);
                inst.BeforeChildProcessLaunch?.Invoke(inst, CefCommandLine.FromNative(pcmdline));
            });
        public Action<CefBrowserProcessHandler, CefCommandLine> BeforeChildProcessLaunch { get; set; }

        delegate void delegate_schedule_message_pump_work(IntPtr self, long delay_ms);
        static DelegateHolder<delegate_schedule_message_pump_work> holder_on_schedule_message_pump_work = new DelegateHolder<delegate_schedule_message_pump_work>(
            (ptr, delay_ms) =>
            {
                ////??? never call ?  need external_message_pump ?
                //CefWin.WriteDebugLine("ScheduleMessagePumpWork:" + delay_ms);
                var inst = GetInstance(ptr);
                inst.ScheduleMessagePumpWork?.Invoke(inst, delay_ms);
            });
        public Action<CefBrowserProcessHandler, long> ScheduleMessagePumpWork { get; set; }
    }


    [StructLayout(LayoutKind.Sequential, Pack = 0)]
    public unsafe struct cef_print_handler_t
    {
        public cef_base_ref_counted_t brc;
        public IntPtr on_print_start;
        public IntPtr on_print_settings;
        public IntPtr on_print_dialog;
        public IntPtr on_print_job;
        public IntPtr on_print_reset;
        public IntPtr get_pdf_paper_size;
    }


}
