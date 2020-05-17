using System;
using System.Collections.Generic;
using System.Linq;
using CefLite.Interop;
using System.Threading.Tasks;

using System.Runtime.InteropServices;


namespace CefLite.Interop
{


    [StructLayout(LayoutKind.Sequential, Pack = 0)]
    public unsafe struct cef_browser_t
    {
        public cef_base_ref_counted_t brc;
        public IntPtr get_host;
        public IntPtr can_go_back;
        public IntPtr go_back;
        public IntPtr can_go_forward;
        public IntPtr go_forward;
        public IntPtr is_loading;
        public IntPtr reload;
        public IntPtr reload_ignore_cache;
        public IntPtr stop_load;
        public IntPtr get_identifier;
        public IntPtr is_same;
        public IntPtr is_popup;
        public IntPtr has_document;
        public IntPtr get_main_frame;
        public IntPtr get_focused_frame;
        public IntPtr get_frame_byident;
        public IntPtr get_frame;
        public IntPtr get_frame_count;
        public IntPtr get_frame_identifiers;
        public IntPtr get_frame_names;
    }

    public unsafe partial class CefBrowser
    {
        static public CefBrowser CreateBrowserSync(CefWindowInfo wininfo, CefClient client, string url, CefBrowserSettings browser_settings, CefDictionaryValue extra_info = null, CefRequestContext requestContext = null)
        {
            CefString cefurl = url ?? throw new ArgumentNullException(nameof(url));
            cef_browser_t* pBrowser = ObjectInterop.cef_browser_host_create_browser_sync(wininfo, client, cefurl, browser_settings, extra_info, requestContext);
            if (pBrowser == null)
                throw new Exception("Failed to create browser");
            return FromInArg(pBrowser);
        }

        public bool IsPopup => Marshal.GetDelegateForFunctionPointer<GetInt32Handler>(FixedPtr->is_popup)(Ptr) != 0;
        public bool HasDocument => Marshal.GetDelegateForFunctionPointer<GetInt32Handler>(FixedPtr->has_document)(Ptr) != 0;


        public CefBrowserHost GetHost()
        {
            var gethandler = Marshal.GetDelegateForFunctionPointer<GetObjectHandler>(FixedPtr->get_host);
            IntPtr hostptr = gethandler((IntPtr)Ptr);
            return CefBrowserHost.FromOutVal((cef_browser_host_t*)hostptr);
        }

        public CefFrame GetMainFrame()
        {
            var gethandler = Marshal.GetDelegateForFunctionPointer<GetObjectHandler>(FixedPtr->get_main_frame);
            IntPtr hostptr = gethandler((IntPtr)Ptr);
            return CefFrame.FromOutVal(hostptr);
        }

        long _id = long.MinValue;
        public long Identifier
        {
            get
            {
                if (_id == long.MinValue)
                {
                    var func = Marshal.GetDelegateForFunctionPointer<GetInt64Handler>(FixedPtr->get_identifier);
                    _id = func(Ptr);
                }
                return _id;
            }
        }


        //NOTE: can't use this object wrapper to store shared data
        //because there's many instances or IntPtr for same browser object , use the Identifier instead..
        //internal long RendererAgentId;
        internal void _OnBeforeClose()
        {
            edmap.TryRemove(Identifier, out var value);
        }
        static System.Collections.Concurrent.ConcurrentDictionary<long, CefBrowserExtraData> edmap = new System.Collections.Concurrent.ConcurrentDictionary<long, CefBrowserExtraData>();
        public CefBrowserExtraData GetExtraData(bool create)
        {
            long id = Identifier;
            if(create)
                return edmap.GetOrAdd(id, v => new CefBrowserExtraData());
            if (edmap.TryGetValue(id, out var val))
                return val;
            return null;
        }

    }

    public class CefBrowserExtraData
    {
        
    }
}
