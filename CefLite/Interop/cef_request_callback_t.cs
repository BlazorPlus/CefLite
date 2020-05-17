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

    public unsafe partial class CefRequestCallback
    {
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
