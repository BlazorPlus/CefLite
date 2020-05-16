using System;
using System.Collections.Generic;
using System.Linq;
using CefLite.Interop;
using System.Threading.Tasks;

using System.Runtime.InteropServices;


namespace CefLite.Interop
{



    [StructLayout(LayoutKind.Sequential, Pack = 0)]
    public unsafe struct cef_browser_settings_t : IFixedPointer
    {
        public size_t size;

        public int windowless_frame_rate;

        public cef_string_t standard_font_family;
        public cef_string_t fixed_font_family;
        public cef_string_t serif_font_family;
        public cef_string_t sans_serif_font_family;
        public cef_string_t cursive_font_family;
        public cef_string_t fantasy_font_family;
        public int default_font_size;
        public int default_fixed_font_size;
        public int minimum_font_size;
        public int minimum_logical_font_size;

        public cef_string_t default_encoding;

        public cef_state_t remote_fonts;
        public cef_state_t javascript;
        public cef_state_t javascript_close_windows;
        public cef_state_t javascript_access_clipboard;
        public cef_state_t javascript_dom_paste;
        public cef_state_t plugins;
        public cef_state_t universal_access_from_file_urls;
        public cef_state_t file_access_from_file_urls;
        public cef_state_t web_security;
        public cef_state_t image_loading;
        public cef_state_t image_shrink_standalone_to_fit;
        public cef_state_t text_area_resize;
        public cef_state_t tab_to_links;
        public cef_state_t local_storage;
        public cef_state_t databases;
        public cef_state_t application_cache;
        public cef_state_t webgl;

        public uint background_color;

        public cef_string_t accept_language_list;

    }

    public unsafe class CefBrowserSettings : FixedPointer<cef_browser_settings_t, CefBrowserSettings>
    {
        public cef_browser_settings_t* FixedPtr => (cef_browser_settings_t*)Ptr;

        public CefBrowserSettings() : base(RecyclePtr) { }

        static unsafe void RecyclePtr(IntPtr intptr)
        {
            cef_browser_settings_t* ptr = (cef_browser_settings_t*)intptr;
        }

        private CefBrowserSettings(IntPtr ptr) : base(ptr)
        {

        }
        static public CefBrowserSettings FromNative(IntPtr ptr)
        {
            if (ptr == IntPtr.Zero) return null;
            return new CefBrowserSettings(ptr);
        }
        static public CefBrowserSettings FromNative(cef_browser_settings_t* ptr)
        {
            if (ptr == null) return null;
            return new CefBrowserSettings((IntPtr)ptr);
        }
    }
}
