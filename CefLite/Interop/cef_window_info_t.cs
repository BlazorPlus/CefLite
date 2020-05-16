using System;
using System.Collections.Generic;
using System.Linq;
using CefLite.Interop;
using System.Threading.Tasks;

using System.Runtime.InteropServices;


namespace CefLite.Interop
{

    [StructLayout(LayoutKind.Sequential, Pack = 0)]
    public struct cef_window_info_t : IFixedPointer
    {
        public uint ex_style;
        public cef_string_t window_name;
        public uint style;
        public int x;
        public int y;
        public int width;
        public int height;
        public IntPtr parent_window;
        public IntPtr menu;
        public int windowless_rendering_enabled;

        public int shared_texture_enabled;

        public int external_begin_frame_enabled;

        public IntPtr window;
    }

    public unsafe class CefWindowInfo : FixedPointer<cef_window_info_t, CefWindowInfo>
    {
        public cef_window_info_t* FixedPtr => (cef_window_info_t*)Ptr;

        [Flags]
        public enum WindowStyle : uint
        {
            WS_TABSTOP = 0x00010000,
            WS_CLIPCHILDREN = 0x02000000,
            WS_CLIPSIBLINGS = 0x04000000,
            WS_VISIBLE = 0x10000000,
            WS_CHILD = 0x40000000,
        }

        public CefWindowInfo() : base(RecyclePtr) {

            cef_window_info_t* info = (cef_window_info_t*)Ptr;
            info->style = (uint)(WindowStyle.WS_TABSTOP
                | WindowStyle.WS_CLIPCHILDREN
                | WindowStyle.WS_CLIPSIBLINGS
                | WindowStyle.WS_VISIBLE
                | WindowStyle.WS_CHILD);
        }

        static unsafe void RecyclePtr(IntPtr intptr)
        {
            cef_window_info_t* ptr = (cef_window_info_t*)intptr;

            ObjectInterop.cef_string_utf16_clear(&ptr->window_name);
            //TODO:other strings
        }

        private CefWindowInfo(IntPtr ptr) : base(ptr)
        {

        }
        static public CefWindowInfo FromNative(IntPtr ptr)
        {
            if (ptr == IntPtr.Zero) return null;
            return new CefWindowInfo(ptr);
        }
        static public CefWindowInfo FromNative(cef_window_info_t* ptr)
        {
            if (ptr == null) return null;
            return new CefWindowInfo((IntPtr)ptr);
        }
    }

}
