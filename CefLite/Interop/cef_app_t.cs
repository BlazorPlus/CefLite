using System;
using System.Collections.Generic;
using System.Linq;
using CefLite.Interop;
using System.Threading.Tasks;

using System.Runtime.InteropServices;

namespace CefLite.Interop
{

    [StructLayout(LayoutKind.Sequential, Pack = 0)]
    public unsafe struct cef_app_t
    {
        public cef_base_ref_counted_t brc;
        public IntPtr on_before_command_line_processing;
        public IntPtr on_register_custom_schemes;
        public IntPtr get_resource_bundle_handler;
        public IntPtr get_browser_process_handler;
        public IntPtr get_render_process_handler;
    }

    public unsafe partial class CefApp : ObjectFromNet<cef_app_t, CefApp>
    {

        public cef_app_t* FixedPtr => (cef_app_t*)Ptr;

        public CefBrowserProcessHandler BrowserProcessHandler { get; set; } = new CefBrowserProcessHandler();
        public CefRenderProcessHandler RenderProcessHandler { get; set; } = new CefRenderProcessHandler();
        public CefResourceBundleHandler ResourceBundleHandler { get; set; } = new CefResourceBundleHandler();

        public CefApp()
        {
            cef_app_t* self = FixedPtr;
            self->get_browser_process_handler = holder_get_browser_process_handler;
            self->get_render_process_handler = holder_get_render_prcess_handler;
            self->get_resource_bundle_handler = holder_get_resource_bundle_handler;
            self->on_before_command_line_processing = holder_on_before_command_line_processing;
            self->on_register_custom_schemes = holder_on_register_custom_schemes;
        }

        static DelegateHolder<GetObjectHandler> holder_get_browser_process_handler = new DelegateHolder<GetObjectHandler>(
            app => GetInstance(app).BrowserProcessHandler.AddRefReturnIntPtr());

        static DelegateHolder<GetObjectHandler> holder_get_render_prcess_handler = new DelegateHolder<GetObjectHandler>(
            app => GetInstance(app).RenderProcessHandler.AddRefReturnIntPtr());

        static DelegateHolder<GetObjectHandler> holder_get_resource_bundle_handler = new DelegateHolder<GetObjectHandler>(
            app => GetInstance(app).ResourceBundleHandler.AddRefReturnIntPtr());

        static DelegateHolder<EventCallbackHandler3> holder_on_before_command_line_processing = new DelegateHolder<EventCallbackHandler3>(
            delegate (IntPtr app, IntPtr pstrtype, IntPtr pcmdline)
            {
                string strtype = cef_string_t.ToString(pstrtype);
                var cmdline = CefCommandLine.FromInArg(pcmdline);
                var inst = GetInstance(app);

                //when specify --proxy-server=http://...:.. , remove --no-proxy-server
                string[] arr = CefWin.CefAdditionArguments.Where(v => v != null).ToArray();
                if (arr.Any(v => v.StartsWith("--proxy-server=")))
                {
                    arr = arr.Where(v => v != "--no-proxy-server").ToArray();
                }

                string str = cmdline.GetCommandLineString() + " " + string.Join(" ", arr);
                cmdline.InitFromString(str);

                CefWin.WriteDebugLine("OnBeforeCommandLineProcessing " + strtype + " : " + str);

                inst.BeforeCommandLineProcessing?.Invoke(inst, strtype, cmdline);

            });

        public Action<CefApp, string, CefCommandLine> BeforeCommandLineProcessing { get; set; }

        delegate int delegate_add_custom_scheme(cef_scheme_registrar_t* host, cef_string_t* scheme_name, cef_scheme_options_t options);
        static DelegateHolder<EventCallbackHandler2> holder_on_register_custom_schemes = new DelegateHolder<EventCallbackHandler2>(
            delegate (IntPtr app, IntPtr registrar)
            {
                cef_scheme_registrar_t* reg = (cef_scheme_registrar_t*)registrar;

                var func = Marshal.GetDelegateForFunctionPointer<delegate_add_custom_scheme>(reg->add_custom_scheme);
                CefString strcefwin = "CEFWIN";
                int r1 = func(reg, strcefwin.FixedPtr, cef_scheme_options_t.CEF_SCHEME_OPTION_NONE);
                CefWin.WriteDebugLine($"add_custom_scheme : CEFWIN:{r1}");

                //reg.del is 0 , so don't need to free it ?
            });


    }



    [StructLayout(LayoutKind.Sequential, Pack = 0)]
    public unsafe struct cef_resource_bundle_handler_t
    {
        public cef_base_ref_counted_t brc;
        public IntPtr get_localized_string;
        public IntPtr get_data_resource;
        public IntPtr get_data_resource_for_scale;
    }

    public unsafe partial class CefResourceBundleHandler : ObjectFromNet<cef_resource_bundle_handler_t, CefResourceBundleHandler>
    {
        public cef_resource_bundle_handler_t* FixedPtr => (cef_resource_bundle_handler_t*)Ptr;

        public CefResourceBundleHandler()
        {
            cef_resource_bundle_handler_t* self = FixedPtr;
        }
    }




}
