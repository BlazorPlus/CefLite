using System;
using System.Collections.Generic;
using System.Linq;
using CefLite.Interop;
using System.Threading.Tasks;

using System.Runtime.InteropServices;


namespace CefLite.Interop
{

    [StructLayout(LayoutKind.Sequential, Pack = 0)]
    public unsafe struct cef_base_ref_counted_t
    {
        public size_t size;

        public IntPtr add_ref;
        public IntPtr release;
        public IntPtr has_one_ref;
        public IntPtr has_at_least_one_ref;
    }

}

