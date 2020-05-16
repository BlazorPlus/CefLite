using System;
using System.Collections.Generic;
using System.Linq;
using CefLite.Interop;
using System.Threading.Tasks;

using System.Runtime.InteropServices;


namespace CefLite.Interop
{

    [StructLayout(LayoutKind.Sequential, Pack = 0)]
    public unsafe struct cef_string_t : IFixedPointer
    {
        public char* str;
        public size_t length;
        public IntPtr dtor;

        public override string ToString()
        {
            if (str == null)
                return null;
            int len = (int)length.value;
            if (len == 0)
                return string.Empty;
            return new string(str); //verified , the str has '\0' at the end.
        }

        public bool IsNullOrEmpty()
        {
            return length.value == UIntPtr.Zero;
        }


        static public string ToString(IntPtr ptr)
        {
            return ptr == IntPtr.Zero ? null : ((cef_string_t*)ptr)->ToString();
        }
        static public string ToString(cef_string_t* ptr)
        {
            return ptr == null ? null : ptr->ToString();
        }

    }

    public unsafe class CefString : FixedPointer<cef_string_t, CefString>
    {
        public cef_string_t* FixedPtr => (cef_string_t*)Ptr;

        public CefString(string str) : base(RecyclePtr)
        {
            if (str == null) throw new ArgumentNullException(nameof(str));
            F.invoke_set(FixedPtr, str);
        }
        static unsafe void RecyclePtr(IntPtr intptr)
        {
            cef_string_t* ptr = (cef_string_t*)intptr;
            if (ptr->str != null)
                ObjectInterop.cef_string_utf16_clear(ptr);
        }
        public override string ToString()
        {
            return FixedPtr->ToString();
        }

        bool _isuserfree;

        private CefString(IntPtr ptr) : base(ptr)
        {
            _isuserfree = true;
        }

        ~CefString()
        {
            if (_isuserfree)
            {
                F.cef_string_userfree_utf16_free(FixedPtr);
            }
        }

        static public implicit operator cef_string_t*(CefString str)
        {
            return str == null ? null : str.FixedPtr;
        }
        static public implicit operator CefString(string str)
        {
            return str == null ? null : new CefString(str);
        }
        static public CefString FromUserFree(IntPtr ptr)
        {
            if (ptr == IntPtr.Zero) return null;
            return new CefString(ptr);
        }
    }
}
