using System;
using System.Collections.Generic;
using System.Linq;
using CefLite.Interop;
using System.Threading.Tasks;

using System.Runtime.InteropServices;


namespace CefLite.Interop
{


    [StructLayout(LayoutKind.Sequential, Pack = 0)]
    public unsafe struct cef_command_line_t
    {
        public cef_base_ref_counted_t brc;
        public IntPtr is_valid;
        public IntPtr is_read_only;
        public IntPtr copy;
        public IntPtr init_from_argv;
        public IntPtr init_from_string;
        public IntPtr reset;
        public IntPtr get_argv;
        public IntPtr get_command_line_string;
        public IntPtr get_program;
        public IntPtr set_program;
        public IntPtr has_switches;
        public IntPtr has_switch;
        public IntPtr get_switch_value;
        public IntPtr get_switches;
        public IntPtr append_switch;
        public IntPtr append_switch_with_value;
        public IntPtr has_arguments;
        public IntPtr get_arguments;
        public IntPtr append_argument;
        public IntPtr prepend_wrapper;
    }

    public unsafe partial class CefCommandLine
    {
        
        public string GetCommandLineString()
        {
            return invoke_get_command_line_string(FixedPtr);
        }
        public void InitFromString(string str)
        {
            invoke_init_from_string(FixedPtr, str);
        }

        static public string invoke_get_command_line_string(cef_command_line_t* self)
        {
            GetObjectHandler func = Marshal.GetDelegateForFunctionPointer<GetObjectHandler>(self->get_command_line_string);
            CefString str = CefString.FromUserFree(func((IntPtr)self));
            return str?.ToString();
        }
        static public void invoke_init_from_string(cef_command_line_t* self, string command_line)
        {
            CefString cefstr = command_line;
            var func = Marshal.GetDelegateForFunctionPointer<EventCallbackHandler2>(self->init_from_string);
            func((IntPtr)self, cefstr);
        }

    }
}
