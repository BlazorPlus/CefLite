using System;
using System.Collections.Generic;
using System.Linq;
using CefLite.Interop;
using System.Threading.Tasks;

using System.Runtime.InteropServices;


namespace CefLite.Interop
{


    [StructLayout(LayoutKind.Sequential, Pack = 0)]
    public struct cef_main_args_t : IFixedPointer
    {
        public IntPtr instance;
    }

    public unsafe partial class CefMainArgs : FixedPointer<cef_main_args_t, CefMainArgs>
    {
        public cef_main_args_t* FixedPtr => (cef_main_args_t*)Ptr;

        public CefMainArgs() : base(RecyclePtr)
        {
            cef_main_args_t* ptr = (cef_main_args_t*)Ptr;
            ptr->instance = WindowsInterop.GetModuleHandle(null);
        }

        static unsafe void RecyclePtr(IntPtr intptr)
        {
            cef_main_args_t* ptr = (cef_main_args_t*)intptr;
        }
    }


    [StructLayout(LayoutKind.Sequential, Pack = 0)]
    public struct cef_settings_t : IFixedPointer
    {

        public size_t size;
        public int no_sandbox;
        public cef_string_t browser_subprocess_path;
        public cef_string_t framework_dir_path;
        public cef_string_t main_bundle_path;
        public int multi_threaded_message_loop;
        public int external_message_pump;
        public int windowless_rendering_enabled;
        public int command_line_args_disabled;
        public cef_string_t cache_path;
        public cef_string_t root_cache_path;

        public cef_string_t user_data_path;

        public int persist_session_cookies;
        public int persist_user_preferences;

        public cef_string_t user_agent;

        public cef_string_t product_version;

        public cef_string_t locale;

        public cef_string_t log_file;
        public cef_log_severity_t log_severity;

        public cef_string_t javascript_flags;

        public cef_string_t resources_dir_path;

        public cef_string_t locales_dir_path;

        public int pack_loading_disabled;

        public int remote_debugging_port;

        public int uncaught_exception_stack_size;

        public int ignore_certificate_errors;

        public uint background_color;   //32bit argb

        public cef_string_t accept_language_list;

        public cef_string_t application_client_id_for_file_scanning;

    }

    public unsafe partial class CefSettings : FixedPointer<cef_settings_t, CefSettings>
    {
        public cef_settings_t* FixedPtr => (cef_settings_t*)Ptr;

        public CefSettings() : base(RecyclePtr)
        {
            cef_settings_t* ptr = (cef_settings_t*)Ptr;
            ptr->log_severity = cef_log_severity_t.LOGSEVERITY_WARNING;
            //ptr->multi_threaded_message_loop = 1;//TEST
        }

        static unsafe void RecyclePtr(IntPtr intptr)
        {
            cef_settings_t* ptr = (cef_settings_t*)intptr;

            //TODO: if the libcef.dll is not initialized , this function is not able to call
            //so check the pointer before release it
            if (ptr->resources_dir_path.str != null) ObjectInterop.cef_string_utf16_clear(&ptr->resources_dir_path);
            //TODO:other strings
        }
    }



}
