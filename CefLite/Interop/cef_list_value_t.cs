using System;
using System.Collections.Generic;
using System.Linq;
using CefLite.Interop;
using System.Threading.Tasks;

using System.Runtime.InteropServices;


namespace CefLite.Interop
{

    [StructLayout(LayoutKind.Sequential, Pack = 0)]
    public unsafe struct cef_list_value_t : IFixedPointer
    {
        public cef_base_ref_counted_t brc;
        public IntPtr is_valid;
        public IntPtr is_owned;
        public IntPtr is_read_only;
        public IntPtr is_same;
        public IntPtr is_equal;
        public IntPtr copy;
        public IntPtr set_size;
        public IntPtr get_size;
        public IntPtr clear;
        public IntPtr remove;
        public IntPtr get_type;
        public IntPtr get_value;
        public IntPtr get_bool;
        public IntPtr get_int;
        public IntPtr get_double;
        public IntPtr get_string;
        public IntPtr get_binary;
        public IntPtr get_dictionary;
        public IntPtr get_list;
        public IntPtr set_value;
        public IntPtr set_null;
        public IntPtr set_bool;
        public IntPtr set_int;
        public IntPtr set_double;
        public IntPtr set_string;
        public IntPtr set_binary;
        public IntPtr set_dictionary;
        public IntPtr set_list;
    }

    public unsafe class CefListValue : FixedPointer<cef_list_value_t, CefListValue>
    {
        public cef_list_value_t* FixedPtr => (cef_list_value_t*)Ptr;

        public CefListValue() : base(RecyclePtr) { }
        static unsafe void RecyclePtr(IntPtr intptr)
        {

        }

        private CefListValue(IntPtr ptr) : base(ptr) { }
        static public CefListValue FromNative(IntPtr ptr)
        {
            if (ptr == IntPtr.Zero) return null;
            return new CefListValue(ptr);
        }
        static public CefListValue FromNative(cef_list_value_t* ptr)
        {
            if (ptr == null) return null;
            return new CefListValue((IntPtr)ptr);
        }

        public size_t GetSize()
        {
            return (size_t)Marshal.GetDelegateForFunctionPointer<GetObjectHandler>(FixedPtr->get_size)(Ptr);
        }
        public void SetSize(uint size)
        {
            SetSize((size_t)size);
        }
        public void SetSize(size_t size)
        {
            Marshal.GetDelegateForFunctionPointer<EventCallbackHandler2>(FixedPtr->set_size)(Ptr, size);
        }
        public string GetString(uint index)
        {
            return CefString.FromUserFree(Marshal.GetDelegateForFunctionPointer<GetObjectHandler2>(FixedPtr->get_string)(Ptr, (IntPtr)(long)index))?.ToString();
        }
        public void SetString(uint index, string str)
        {
            CefString cefstr = str;
            Marshal.GetDelegateForFunctionPointer<EventCallbackHandler3>(FixedPtr->set_string)(Ptr, (IntPtr)(long)index, cefstr);
        }
    }
}

