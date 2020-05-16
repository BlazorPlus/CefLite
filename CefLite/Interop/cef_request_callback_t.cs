using System;
using System.Collections.Generic;
using System.Linq;
using CefLite.Interop;
using System.Threading.Tasks;

using System.Runtime.InteropServices;


namespace CefLite.Interop
{
    [StructLayout(LayoutKind.Sequential, Pack = 0)]
    public unsafe struct cef_request_callback_t
    {
        public cef_base_ref_counted_t brc;
        public IntPtr cont;
        public IntPtr cancel;

    }

    public unsafe class CefRequestCallback : ObjectFromCef<cef_request_callback_t, CefRequestCallback>
    {

        private CefRequestCallback(IntPtr ptr) : base(ptr) { }
        static public CefRequestCallback FromNative(cef_request_callback_t* ptr)
            => FromNative((IntPtr)ptr, (p2) => new CefRequestCallback(p2));
        public cef_request_callback_t* FixedPtr => (cef_request_callback_t*)Ptr;

        static public implicit operator CefRequestCallback(cef_request_callback_t* ptr) => FromNative(ptr);


        public void Cont()
        {
            var func = Marshal.GetDelegateForFunctionPointer<EventCallbackHandler>(FixedPtr->cont);
            func(Ptr);
        }

        public void Cancel()
        {
            var func = Marshal.GetDelegateForFunctionPointer<EventCallbackHandler>(FixedPtr->cancel);
            func(Ptr);
        }

    }

}
