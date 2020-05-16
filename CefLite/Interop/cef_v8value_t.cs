using System;
using System.Collections.Generic;
using System.Linq;
using CefLite.Interop;
using System.Threading.Tasks;

using System.Runtime.InteropServices;


namespace CefLite.Interop
{
    [StructLayout(LayoutKind.Sequential, Pack = 0)]
    public unsafe struct cef_v8value_t
    {
        public cef_base_ref_counted_t brc;
        public IntPtr is_valid;
        public IntPtr is_undefined;
        public IntPtr is_null;
        public IntPtr is_bool;
        public IntPtr is_int;
        public IntPtr is_uint;
        public IntPtr is_double;
        public IntPtr is_date;
        public IntPtr is_string;
        public IntPtr is_object;
        public IntPtr is_array;
        public IntPtr is_array_buffer;
        public IntPtr is_function;
        public IntPtr is_same;
        public IntPtr get_bool_value;
        public IntPtr get_int_value;
        public IntPtr get_uint_value;
        public IntPtr get_double_value;
        public IntPtr get_date_value;
        public IntPtr get_string_value;
        public IntPtr is_user_created;
        public IntPtr has_exception;
        public IntPtr get_exception;
        public IntPtr clear_exception;
        public IntPtr will_rethrow_exceptions;
        public IntPtr set_rethrow_exceptions;
        public IntPtr has_value_bykey;
        public IntPtr has_value_byindex;
        public IntPtr delete_value_bykey;
        public IntPtr delete_value_byindex;
        public IntPtr get_value_bykey;
        public IntPtr get_value_byindex;
        public IntPtr set_value_bykey;
        public IntPtr set_value_byindex;
        public IntPtr set_value_byaccessor;
        public IntPtr get_keys;
        public IntPtr set_user_data;
        public IntPtr get_user_data;
        public IntPtr get_externally_allocated_memory;
        public IntPtr adjust_externally_allocated_memory;
        public IntPtr get_array_length;
        public IntPtr get_array_buffer_release_callback;
        public IntPtr neuter_array_buffer;
        public IntPtr get_function_name;
        public IntPtr get_function_handler;
        public IntPtr execute_function;
        public IntPtr execute_function_with_context;

    }

    public unsafe class CefV8Value : ObjectFromCef<cef_v8value_t, CefV8Value>
    {
        private CefV8Value(IntPtr ptr) : base(ptr) { }
        static public CefV8Value FromNative(cef_v8value_t* ptr)
            => FromNative((IntPtr)ptr, (p2) => new CefV8Value(p2));
        static public CefV8Value FromNative(IntPtr ptr)
           => FromNative(ptr, (p2) => new CefV8Value(p2));
        public cef_v8value_t* FixedPtr => (cef_v8value_t*)Ptr;

        static public implicit operator CefV8Value(cef_v8value_t* ptr) => FromNative(ptr);

        public string GetStringValue()
        {
            var func = Marshal.GetDelegateForFunctionPointer<GetObjectHandler>(FixedPtr->get_string_value);
            return CefString.FromUserFree(func(Ptr))?.ToString();
        }

        public object ToObject()
        {
            //TODO: deserialzie it 
            return null;
        }

    }

}
