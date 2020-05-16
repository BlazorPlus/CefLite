using System;
using System.Collections.Generic;
using System.Linq;
using CefLite.Interop;
using System.Threading.Tasks;

using System.Runtime.InteropServices;


namespace CefLite.Interop
{



    [StructLayout(LayoutKind.Sequential, Pack = 0)]
    public unsafe struct cef_sslinfo_t
    {
        public cef_base_ref_counted_t brc;
        public IntPtr get_cert_status;
        public IntPtr get_x509certificate;
    }

    public unsafe class CefSslInfo : ObjectFromCef<cef_sslinfo_t, CefSslInfo>
    {
        private CefSslInfo(IntPtr ptr) : base(ptr) { }
        static public CefSslInfo FromNative(cef_sslinfo_t* ptr)
            => FromNative((IntPtr)ptr, (p2) => new CefSslInfo(p2));
        static public CefSslInfo FromNative(IntPtr ptr)
            => FromNative(ptr, (p2) => new CefSslInfo(p2));
        public cef_sslinfo_t* FixedPtr => (cef_sslinfo_t*)Ptr;

        static public implicit operator CefSslInfo(cef_sslinfo_t* ptr) => FromNative(ptr);
    }

    [StructLayout(LayoutKind.Sequential, Pack = 0)]
    public unsafe struct cef_x509certificate_t
    {
        public cef_base_ref_counted_t brc;
        public IntPtr get_subject;
        public IntPtr get_issuer;
        public IntPtr get_serial_number;
        public IntPtr get_valid_start;
        public IntPtr get_valid_expiry;
        public IntPtr get_derencoded;
        public IntPtr get_pemencoded;
        public IntPtr get_issuer_chain_size;
        public IntPtr get_derencoded_issuer_chain;
        public IntPtr get_pemencoded_issuer_chain;
    }

    public unsafe class CefX509Certificate : ObjectFromCef<cef_x509certificate_t, CefX509Certificate>
    {
        private CefX509Certificate(IntPtr ptr) : base(ptr) { }
        static public CefX509Certificate FromNative(cef_x509certificate_t* ptr)
            => FromNative((IntPtr)ptr, (p2) => new CefX509Certificate(p2));
        static public CefX509Certificate FromNative(IntPtr ptr)
            => FromNative(ptr, (p2) => new CefX509Certificate(p2));
        public cef_x509certificate_t* FixedPtr => (cef_x509certificate_t*)Ptr;

        static public implicit operator CefX509Certificate(cef_x509certificate_t* ptr) => FromNative(ptr);
    }



}
