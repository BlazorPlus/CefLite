using System;
using System.Collections.Generic;
using System.Linq;
using CefLite.Interop;
using System.Threading.Tasks;

using System.Runtime.InteropServices;


namespace CefLite.Interop
{


    public unsafe class LibCefInterop
    {
        //http://opensource.spotify.com/cefbuilds/cef_binary_81.2.24%2Bgc0b313d%2Bchromium-81.0.4044.113_windows32_client.tar.bz2
        static public string CefVersionFor { get; } = "81.2.24+gc0b313d+chromium-81.0.4044.113";
        static public string CefVersionArr { get; } = "81 2 24 2176 81 0 4044 113";

        static public string CefVersionArrFromLibCefDll()
        {
            int[] verarr = new int[8];
            for (int i = 0; i < 8; i++)
            {
                verarr[i] = LibCefInterop.cef_version_info(i);
            }
            return string.Join(" ", verarr.Select(v => v.ToString()).ToArray());
        }

        static public CefApp App { get; set; } = new CefApp();
        static public CefMainArgs MainArgs { get; set; } = new CefMainArgs();
        static public CefSettings Settings { get; set; } = new CefSettings();
        static public IntPtr SendBoxInfo { get; set; } = IntPtr.Zero;



        static public unsafe CefInitState Initialize()
        {
            CefWin.WriteDebugLine("cef_execute_process");
            App.AddRef();//why ? need add_ref for this function?
            int res = cef_execute_process(MainArgs.Ptr, App.Ptr, SendBoxInfo);
            if (res >= 0)
                return CefInitState.SubProcess;

            CefWin.WriteDebugLine("cef_initialize");
            App.AddRef();//why ? need add_ref for this function?
            res = cef_initialize(MainArgs.Ptr, Settings.Ptr, App.Ptr, SendBoxInfo);

            if (res != 0)
                return CefInitState.Initialized;

            return CefInitState.Failed;
        }

        [DllImport("libcef.dll")]
        internal static extern void cef_shutdown();

        [DllImport("libcef.dll")]
        static public extern void cef_do_message_loop_work();


        [DllImport("libcef.dll")]
        internal static extern void cef_run_message_loop();

        [DllImport("libcef.dll")]
        internal static extern void cef_quit_message_loop();


        [DllImport("libcef.dll")]
        static public extern int cef_version_info(int entry);

        [DllImport("libcef.dll")]
        internal static extern int cef_initialize(IntPtr args, IntPtr settings, IntPtr app, IntPtr sandbox_info);


        [DllImport("libcef.dll")]
        internal static extern int cef_execute_process(IntPtr args, IntPtr application, IntPtr sandbox_info);


        [DllImport("libcef.dll")]
        static public extern void cef_enable_highdpi_support();


    }

}