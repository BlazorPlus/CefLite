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

    public unsafe partial class CefSslInfo
    {
     
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

    public unsafe partial class CefX509Certificate
    {
       
    }



}
