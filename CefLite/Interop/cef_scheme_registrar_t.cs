using System;
using System.Collections.Generic;
using System.Linq;
using CefLite.Interop;
using System.Threading.Tasks;

using System.Runtime.InteropServices;


namespace CefLite.Interop
{
    [StructLayout(LayoutKind.Sequential, Pack = 0)]
    public unsafe struct cef_scheme_registrar_t
    {
        public size_t size;
        public IntPtr del;
        public IntPtr add_custom_scheme;
    }

    [Flags]
    public enum cef_scheme_options_t
    {
        CEF_SCHEME_OPTION_NONE = 0,

        CEF_SCHEME_OPTION_STANDARD = 1 << 0,

        CEF_SCHEME_OPTION_LOCAL = 1 << 1,

        CEF_SCHEME_OPTION_DISPLAY_ISOLATED = 1 << 2,

        CEF_SCHEME_OPTION_SECURE = 1 << 3,

        CEF_SCHEME_OPTION_CORS_ENABLED = 1 << 4,

        CEF_SCHEME_OPTION_CSP_BYPASSING = 1 << 5,

        CEF_SCHEME_OPTION_FETCH_ENABLED = 1 << 6,
    }


}
