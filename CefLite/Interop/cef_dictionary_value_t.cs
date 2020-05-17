using System;
using System.Collections.Concurrent;
using System.Text;

using System.Runtime.InteropServices;

namespace CefLite.Interop
{

    [StructLayout(LayoutKind.Sequential, Pack = 0)]
    public unsafe struct cef_dictionary_value_t : IFixedPointer
    {
        public cef_base_ref_counted_t brc;
        public IntPtr is_valid;
        public IntPtr is_owned;
        public IntPtr is_read_only;
        public IntPtr is_same;
        public IntPtr is_equal;
        public IntPtr copy;
        public IntPtr get_size;
        public IntPtr clear;
        public IntPtr has_key;
        public IntPtr get_keys;
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

    public unsafe partial class CefDictionaryValue : FixedPointer<cef_dictionary_value_t, CefDictionaryValue>
    {
        public cef_dictionary_value_t* FixedPtr => (cef_dictionary_value_t*)Ptr;

        public CefDictionaryValue() : base(RecyclePtr) { }
        static unsafe void RecyclePtr(IntPtr intptr)
        {

        }

        private CefDictionaryValue(IntPtr ptr) : base(ptr) { }
        static public CefDictionaryValue FromInArg(IntPtr ptr)
        {
            if (ptr == IntPtr.Zero) return null;
            return new CefDictionaryValue(ptr);
        }
        static public CefDictionaryValue FromInArg(cef_dictionary_value_t* ptr)
        {
            if (ptr == null) return null;
            return new CefDictionaryValue((IntPtr)ptr);
        }
    }

}
