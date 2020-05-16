using System;
using System.Collections.Generic;
using System.Linq;
using CefLite.Interop;
using System.Threading.Tasks;

using System.Runtime.InteropServices;

namespace CefLite.Interop
{
    internal class F : ObjectInterop
    {
        static public void Trace(string msg)
        {
            CefWin.WriteDebugLine(" * * * " + msg);
        }
        static public void TraceNone(string msg) { }
    }

    public unsafe class ObjectInterop
    {

        [DllImport("libcef.dll")]
        static public extern int cef_string_utf16_set(char* src, size_t src_len, cef_string_t* output, int copy);


        [DllImport("libcef.dll")]
        static public extern void cef_string_utf16_clear(cef_string_t* str);

        [DllImport("libcef.dll")]
        static public extern void cef_string_userfree_utf16_free(cef_string_t* str);



        ///// <summary>
        ///// cef_window_info_t* windowInfo, cef_client_t* client, cef_string_t* url, cef_browser_settings_t* browser_settings, cef_dictionary_value_t* extra_info, cef_request_context_t* request_context
        ///// </summary>
        //[DllImport("libcef.dll")]
        //static public extern int cef_browser_host_create_browser(IntPtr windowInfo, IntPtr client, IntPtr url, IntPtr settings, IntPtr extra_info, IntPtr request_context);

        /// <summary>
        /// cef_window_info_t* windowInfo, cef_client_t* client, cef_string_t* url, cef_browser_settings_t* browser_settings, cef_dictionary_value_t* extra_info, cef_request_context_t* request_context
        /// </summary>
        [DllImport("libcef.dll")]
        static public extern cef_browser_t* cef_browser_host_create_browser_sync(IntPtr windowInfo, IntPtr client, IntPtr url, IntPtr settings, IntPtr extra_info, IntPtr request_context);




        static public IntPtr invoke_get_window_handle(cef_browser_host_t* host)
        {
            var func = Marshal.GetDelegateForFunctionPointer<GetObjectHandler>(host->get_window_handle);
            return func((IntPtr)host);
        }


        static public unsafe void invoke_set(cef_string_t* ptr, string str)
        {
            fixed (char* pstr = str)
            {
                cef_string_utf16_set(pstr, str.Length, (cef_string_t*)ptr, 1);
            }
        }
    }
}
