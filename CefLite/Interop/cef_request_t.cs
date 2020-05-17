using System;
using System.Collections.Generic;
using System.Linq;
using CefLite.Interop;
using System.Threading.Tasks;

using System.Runtime.InteropServices;


namespace CefLite.Interop
{

    [StructLayout(LayoutKind.Sequential, Pack = 0)]
    public unsafe struct cef_request_t
    {
        public cef_base_ref_counted_t brc;
        public IntPtr is_read_only;
        public IntPtr get_url;
        public IntPtr set_url;
        public IntPtr get_method;
        public IntPtr set_method;
        public IntPtr set_referrer;
        public IntPtr get_referrer_url;
        public IntPtr get_referrer_policy;
        public IntPtr get_post_data;
        public IntPtr set_post_data;
        public IntPtr get_header_map;
        public IntPtr set_header_map;
        public IntPtr get_header_by_name;
        public IntPtr set_header_by_name;
        public IntPtr set_all_values;   // set 
        public IntPtr get_flags;
        public IntPtr set_flags;
        public IntPtr get_first_party_for_cookies;
        public IntPtr set_first_party_for_cookies;
        public IntPtr get_resource_type;
        public IntPtr get_transition_type;
        public IntPtr get_identifier;

    }

    public unsafe partial class CefRequest
    {
        public void SetHeaderByName(string name, string value, bool overWrite)
        {
            var func = Marshal.GetDelegateForFunctionPointer<delegate_set_header_by_name>(FixedPtr->set_header_by_name);
            CefString cefname = name;
            CefString cefvalue = value;
            func(FixedPtr, cefname.FixedPtr, cefvalue.FixedPtr, overWrite ? 1 : 0);
        }
        delegate void delegate_set_header_by_name(cef_request_t* req, cef_string_t* name, cef_string_t* value, int overwrite);

        public string GetHeaderByName(string name)
        {
            var func = Marshal.GetDelegateForFunctionPointer<delegate_get_header_by_name>(FixedPtr->get_header_by_name);
            CefString cefname = name;
            return CefString.FromUserFree(func(FixedPtr, cefname.FixedPtr))?.ToString();
        }
        delegate IntPtr delegate_get_header_by_name(cef_request_t* req, cef_string_t* name);


        public bool IsReadOnly => Marshal.GetDelegateForFunctionPointer<GetInt32Handler>(FixedPtr->is_read_only)(Ptr) != 0;

        public string Url
        {
            get
            {
                return CefString.FromUserFree(Marshal.GetDelegateForFunctionPointer<GetObjectHandler>(FixedPtr->get_url)(Ptr))?.ToString();
            }
            set
            {
                var func=Marshal.GetDelegateForFunctionPointer<EventCallbackHandler2>(FixedPtr->set_url);
                CefString cefstr = value;
                func(Ptr, cefstr.AsIntPtr());
            }
        }

    }

}
