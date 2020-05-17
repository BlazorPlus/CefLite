using System;
using System.Collections.Generic;
using System.Linq;
using CefLite.Interop;
using System.Threading.Tasks;

using System.Runtime.InteropServices;


namespace CefLite.Interop
{


    [StructLayout(LayoutKind.Sequential, Pack = 0)]
    public unsafe struct cef_request_context_t
    {
        public cef_base_ref_counted_t brc;
        public IntPtr is_same;
        public IntPtr is_sharing_with;
        public IntPtr is_global;
        public IntPtr get_handler;
        public IntPtr get_cache_path;
        public IntPtr get_default_cookie_manager;
        public IntPtr register_scheme_handler_factory;
        public IntPtr clear_scheme_handler_factories;
        public IntPtr purge_plugin_list_cache;
        public IntPtr has_preference;
        public IntPtr get_preference;
        public IntPtr get_all_preferences;
        public IntPtr can_set_preference;
        public IntPtr set_preference;
        public IntPtr clear_certificate_exceptions;
        public IntPtr clear_http_auth_credentials;
        public IntPtr close_all_connections;
        public IntPtr resolve_host;
        public IntPtr load_extension;
        public IntPtr did_load_extension;
        public IntPtr has_extension;
        public IntPtr get_extensions;
        public IntPtr get_extension;
        public IntPtr get_media_router;
    }

    public unsafe partial class CefRequestContext : ObjectFromNet<cef_request_context_t, CefRequestContext>
    {

        public cef_request_context_t* FixedPtr => (cef_request_context_t*)Ptr;

        public CefRequestContext()
        {
            cef_request_context_t* self = FixedPtr;
        }
    }
}
