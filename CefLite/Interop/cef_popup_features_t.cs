using System;
using System.Collections.Generic;
using System.Linq;
using CefLite.Interop;
using System.Threading.Tasks;

using System.Runtime.InteropServices;


namespace CefLite.Interop
{

    [StructLayout(LayoutKind.Sequential, Pack = 0)]
    public struct cef_popup_features_t : IFixedPointer
    {
        int x;
        int xSet;
        int y;
        int ySet;
        int width;
        int widthSet;
        int height;
        int heightSet;

        int menuBarVisible;
        int statusBarVisible;
        int toolBarVisible;
        int scrollbarsVisible;
    }

    public unsafe partial class CefPopupFeatures : FixedPointer<cef_popup_features_t, CefPopupFeatures>
    {
        public cef_popup_features_t* FixedPtr => (cef_popup_features_t*)Ptr;

        public CefPopupFeatures() : base(RecyclePtr)
        {

        }
        static unsafe void RecyclePtr(IntPtr intptr)
        {

        }

        private CefPopupFeatures(IntPtr ptr) : base(ptr)
        {

        }
        static public CefPopupFeatures FromInArg(IntPtr ptr)
        {
            if (ptr == IntPtr.Zero) return null;
            return new CefPopupFeatures(ptr);
        }
        static public CefPopupFeatures FromInArg(cef_popup_features_t* ptr)
        {
            if (ptr == null) return null;
            return new CefPopupFeatures((IntPtr)ptr);
        }
    }


}
