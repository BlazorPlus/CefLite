using System;
using System.Collections.Generic;
using System.Linq;
using CefLite.Interop;
using System.Threading.Tasks;

using System.Runtime.InteropServices;


namespace CefLite.Interop
{

	public unsafe partial class CefBrowserHost : ObjectFromCef<cef_browser_host_t, CefBrowserHost>
	{
		private CefBrowserHost(IntPtr ptr) : base(ptr) { }
		private CefBrowserHost(IntPtr ptr, bool addref) : base(ptr, addref) { }
		static public CefBrowserHost FromInArg(cef_browser_host_t* ptr) => FromInArg((IntPtr)ptr, (p2) => new CefBrowserHost(p2));
		static public CefBrowserHost FromInArg(IntPtr ptr) => FromInArg(ptr, (p2) => new CefBrowserHost(p2));
		static public CefBrowserHost FromOutVal(cef_browser_host_t* ptr) => FromOutVal((IntPtr)ptr, (p2) => new CefBrowserHost(p2, false));
		static public CefBrowserHost FromOutVal(IntPtr ptr) => FromOutVal(ptr, (p2) => new CefBrowserHost(p2, false));
		public cef_browser_host_t* FixedPtr => (cef_browser_host_t*)Ptr;
	}
	public unsafe partial class CefBrowser : ObjectFromCef<cef_browser_t, CefBrowser>
	{
		private CefBrowser(IntPtr ptr) : base(ptr) { }
		private CefBrowser(IntPtr ptr, bool addref) : base(ptr, addref) { }
		static public CefBrowser FromInArg(cef_browser_t* ptr) => FromInArg((IntPtr)ptr, (p2) => new CefBrowser(p2));
		static public CefBrowser FromInArg(IntPtr ptr) => FromInArg(ptr, (p2) => new CefBrowser(p2));
		static public CefBrowser FromOutVal(cef_browser_t* ptr) => FromOutVal((IntPtr)ptr, (p2) => new CefBrowser(p2, false));
		static public CefBrowser FromOutVal(IntPtr ptr) => FromOutVal(ptr, (p2) => new CefBrowser(p2, false));
		public cef_browser_t* FixedPtr => (cef_browser_t*)Ptr;
	}
	public unsafe partial class CefFrame : ObjectFromCef<cef_frame_t, CefFrame>
	{
		private CefFrame(IntPtr ptr) : base(ptr) { }
		private CefFrame(IntPtr ptr, bool addref) : base(ptr, addref) { }
		static public CefFrame FromInArg(cef_frame_t* ptr) => FromInArg((IntPtr)ptr, (p2) => new CefFrame(p2));
		static public CefFrame FromInArg(IntPtr ptr) => FromInArg(ptr, (p2) => new CefFrame(p2));
		static public CefFrame FromOutVal(cef_frame_t* ptr) => FromOutVal((IntPtr)ptr, (p2) => new CefFrame(p2, false));
		static public CefFrame FromOutVal(IntPtr ptr) => FromOutVal(ptr, (p2) => new CefFrame(p2, false));
		public cef_frame_t* FixedPtr => (cef_frame_t*)Ptr;
	}
	public unsafe partial class CefProcessMessage : ObjectFromCef<cef_process_message_t, CefProcessMessage>
	{
		private CefProcessMessage(IntPtr ptr) : base(ptr) { }
		private CefProcessMessage(IntPtr ptr, bool addref) : base(ptr, addref) { }
		static public CefProcessMessage FromInArg(cef_process_message_t* ptr) => FromInArg((IntPtr)ptr, (p2) => new CefProcessMessage(p2));
		static public CefProcessMessage FromInArg(IntPtr ptr) => FromInArg(ptr, (p2) => new CefProcessMessage(p2));
		static public CefProcessMessage FromOutVal(cef_process_message_t* ptr) => FromOutVal((IntPtr)ptr, (p2) => new CefProcessMessage(p2, false));
		static public CefProcessMessage FromOutVal(IntPtr ptr) => FromOutVal(ptr, (p2) => new CefProcessMessage(p2, false));
		public cef_process_message_t* FixedPtr => (cef_process_message_t*)Ptr;
	}
	public unsafe partial class CefCommandLine : ObjectFromCef<cef_command_line_t, CefCommandLine>
	{
		private CefCommandLine(IntPtr ptr) : base(ptr) { }
		private CefCommandLine(IntPtr ptr, bool addref) : base(ptr, addref) { }
		static public CefCommandLine FromInArg(cef_command_line_t* ptr) => FromInArg((IntPtr)ptr, (p2) => new CefCommandLine(p2));
		static public CefCommandLine FromInArg(IntPtr ptr) => FromInArg(ptr, (p2) => new CefCommandLine(p2));
		static public CefCommandLine FromOutVal(cef_command_line_t* ptr) => FromOutVal((IntPtr)ptr, (p2) => new CefCommandLine(p2, false));
		static public CefCommandLine FromOutVal(IntPtr ptr) => FromOutVal(ptr, (p2) => new CefCommandLine(p2, false));
		public cef_command_line_t* FixedPtr => (cef_command_line_t*)Ptr;
	}
	public unsafe partial class CefDownloadItem : ObjectFromCef<cef_download_item_t, CefDownloadItem>
	{
		private CefDownloadItem(IntPtr ptr) : base(ptr) { }
		private CefDownloadItem(IntPtr ptr, bool addref) : base(ptr, addref) { }
		static public CefDownloadItem FromInArg(cef_download_item_t* ptr) => FromInArg((IntPtr)ptr, (p2) => new CefDownloadItem(p2));
		static public CefDownloadItem FromInArg(IntPtr ptr) => FromInArg(ptr, (p2) => new CefDownloadItem(p2));
		static public CefDownloadItem FromOutVal(cef_download_item_t* ptr) => FromOutVal((IntPtr)ptr, (p2) => new CefDownloadItem(p2, false));
		static public CefDownloadItem FromOutVal(IntPtr ptr) => FromOutVal(ptr, (p2) => new CefDownloadItem(p2, false));
		public cef_download_item_t* FixedPtr => (cef_download_item_t*)Ptr;
	}
	public unsafe partial class CefBeforeDownloadCallback : ObjectFromCef<cef_before_download_callback_t, CefBeforeDownloadCallback>
	{
		private CefBeforeDownloadCallback(IntPtr ptr) : base(ptr) { }
		private CefBeforeDownloadCallback(IntPtr ptr, bool addref) : base(ptr, addref) { }
		static public CefBeforeDownloadCallback FromInArg(cef_before_download_callback_t* ptr) => FromInArg((IntPtr)ptr, (p2) => new CefBeforeDownloadCallback(p2));
		static public CefBeforeDownloadCallback FromInArg(IntPtr ptr) => FromInArg(ptr, (p2) => new CefBeforeDownloadCallback(p2));
		static public CefBeforeDownloadCallback FromOutVal(cef_before_download_callback_t* ptr) => FromOutVal((IntPtr)ptr, (p2) => new CefBeforeDownloadCallback(p2, false));
		static public CefBeforeDownloadCallback FromOutVal(IntPtr ptr) => FromOutVal(ptr, (p2) => new CefBeforeDownloadCallback(p2, false));
		public cef_before_download_callback_t* FixedPtr => (cef_before_download_callback_t*)Ptr;
	}
	public unsafe partial class CefDownloadItemCallback : ObjectFromCef<cef_download_item_callback_t, CefDownloadItemCallback>
	{
		private CefDownloadItemCallback(IntPtr ptr) : base(ptr) { }
		private CefDownloadItemCallback(IntPtr ptr, bool addref) : base(ptr, addref) { }
		static public CefDownloadItemCallback FromInArg(cef_download_item_callback_t* ptr) => FromInArg((IntPtr)ptr, (p2) => new CefDownloadItemCallback(p2));
		static public CefDownloadItemCallback FromInArg(IntPtr ptr) => FromInArg(ptr, (p2) => new CefDownloadItemCallback(p2));
		static public CefDownloadItemCallback FromOutVal(cef_download_item_callback_t* ptr) => FromOutVal((IntPtr)ptr, (p2) => new CefDownloadItemCallback(p2, false));
		static public CefDownloadItemCallback FromOutVal(IntPtr ptr) => FromOutVal(ptr, (p2) => new CefDownloadItemCallback(p2, false));
		public cef_download_item_callback_t* FixedPtr => (cef_download_item_callback_t*)Ptr;
	}
	public unsafe partial class CefRequestCallback : ObjectFromCef<cef_request_callback_t, CefRequestCallback>
	{
		private CefRequestCallback(IntPtr ptr) : base(ptr) { }
		private CefRequestCallback(IntPtr ptr, bool addref) : base(ptr, addref) { }
		static public CefRequestCallback FromInArg(cef_request_callback_t* ptr) => FromInArg((IntPtr)ptr, (p2) => new CefRequestCallback(p2));
		static public CefRequestCallback FromInArg(IntPtr ptr) => FromInArg(ptr, (p2) => new CefRequestCallback(p2));
		static public CefRequestCallback FromOutVal(cef_request_callback_t* ptr) => FromOutVal((IntPtr)ptr, (p2) => new CefRequestCallback(p2, false));
		static public CefRequestCallback FromOutVal(IntPtr ptr) => FromOutVal(ptr, (p2) => new CefRequestCallback(p2, false));
		public cef_request_callback_t* FixedPtr => (cef_request_callback_t*)Ptr;
	}
	public unsafe partial class CefRequest : ObjectFromCef<cef_request_t, CefRequest>
	{
		private CefRequest(IntPtr ptr) : base(ptr) { }
		private CefRequest(IntPtr ptr, bool addref) : base(ptr, addref) { }
		static public CefRequest FromInArg(cef_request_t* ptr) => FromInArg((IntPtr)ptr, (p2) => new CefRequest(p2));
		static public CefRequest FromInArg(IntPtr ptr) => FromInArg(ptr, (p2) => new CefRequest(p2));
		static public CefRequest FromOutVal(cef_request_t* ptr) => FromOutVal((IntPtr)ptr, (p2) => new CefRequest(p2, false));
		static public CefRequest FromOutVal(IntPtr ptr) => FromOutVal(ptr, (p2) => new CefRequest(p2, false));
		public cef_request_t* FixedPtr => (cef_request_t*)Ptr;
	}
	public unsafe partial class CefSslinfo : ObjectFromCef<cef_sslinfo_t, CefSslinfo>
	{
		private CefSslinfo(IntPtr ptr) : base(ptr) { }
		private CefSslinfo(IntPtr ptr, bool addref) : base(ptr, addref) { }
		static public CefSslinfo FromInArg(cef_sslinfo_t* ptr) => FromInArg((IntPtr)ptr, (p2) => new CefSslinfo(p2));
		static public CefSslinfo FromInArg(IntPtr ptr) => FromInArg(ptr, (p2) => new CefSslinfo(p2));
		static public CefSslinfo FromOutVal(cef_sslinfo_t* ptr) => FromOutVal((IntPtr)ptr, (p2) => new CefSslinfo(p2, false));
		static public CefSslinfo FromOutVal(IntPtr ptr) => FromOutVal(ptr, (p2) => new CefSslinfo(p2, false));
		public cef_sslinfo_t* FixedPtr => (cef_sslinfo_t*)Ptr;
	}
	public unsafe partial class CefX509Certificate : ObjectFromCef<cef_x509certificate_t, CefX509Certificate>
	{
		private CefX509Certificate(IntPtr ptr) : base(ptr) { }
		private CefX509Certificate(IntPtr ptr, bool addref) : base(ptr, addref) { }
		static public CefX509Certificate FromInArg(cef_x509certificate_t* ptr) => FromInArg((IntPtr)ptr, (p2) => new CefX509Certificate(p2));
		static public CefX509Certificate FromInArg(IntPtr ptr) => FromInArg(ptr, (p2) => new CefX509Certificate(p2));
		static public CefX509Certificate FromOutVal(cef_x509certificate_t* ptr) => FromOutVal((IntPtr)ptr, (p2) => new CefX509Certificate(p2, false));
		static public CefX509Certificate FromOutVal(IntPtr ptr) => FromOutVal(ptr, (p2) => new CefX509Certificate(p2, false));
		public cef_x509certificate_t* FixedPtr => (cef_x509certificate_t*)Ptr;
	}
	public unsafe partial class CefV8Context : ObjectFromCef<cef_v8context_t, CefV8Context>
	{
		private CefV8Context(IntPtr ptr) : base(ptr) { }
		private CefV8Context(IntPtr ptr, bool addref) : base(ptr, addref) { }
		static public CefV8Context FromInArg(cef_v8context_t* ptr) => FromInArg((IntPtr)ptr, (p2) => new CefV8Context(p2));
		static public CefV8Context FromInArg(IntPtr ptr) => FromInArg(ptr, (p2) => new CefV8Context(p2));
		static public CefV8Context FromOutVal(cef_v8context_t* ptr) => FromOutVal((IntPtr)ptr, (p2) => new CefV8Context(p2, false));
		static public CefV8Context FromOutVal(IntPtr ptr) => FromOutVal(ptr, (p2) => new CefV8Context(p2, false));
		public cef_v8context_t* FixedPtr => (cef_v8context_t*)Ptr;
	}
	public unsafe partial class CefV8Exception : ObjectFromCef<cef_v8exception_t, CefV8Exception>
	{
		private CefV8Exception(IntPtr ptr) : base(ptr) { }
		private CefV8Exception(IntPtr ptr, bool addref) : base(ptr, addref) { }
		static public CefV8Exception FromInArg(cef_v8exception_t* ptr) => FromInArg((IntPtr)ptr, (p2) => new CefV8Exception(p2));
		static public CefV8Exception FromInArg(IntPtr ptr) => FromInArg(ptr, (p2) => new CefV8Exception(p2));
		static public CefV8Exception FromOutVal(cef_v8exception_t* ptr) => FromOutVal((IntPtr)ptr, (p2) => new CefV8Exception(p2, false));
		static public CefV8Exception FromOutVal(IntPtr ptr) => FromOutVal(ptr, (p2) => new CefV8Exception(p2, false));
		public cef_v8exception_t* FixedPtr => (cef_v8exception_t*)Ptr;
	}
	public unsafe partial class CefV8Value : ObjectFromCef<cef_v8value_t, CefV8Value>
	{
		private CefV8Value(IntPtr ptr) : base(ptr) { }
		private CefV8Value(IntPtr ptr, bool addref) : base(ptr, addref) { }
		static public CefV8Value FromInArg(cef_v8value_t* ptr) => FromInArg((IntPtr)ptr, (p2) => new CefV8Value(p2));
		static public CefV8Value FromInArg(IntPtr ptr) => FromInArg(ptr, (p2) => new CefV8Value(p2));
		static public CefV8Value FromOutVal(cef_v8value_t* ptr) => FromOutVal((IntPtr)ptr, (p2) => new CefV8Value(p2, false));
		static public CefV8Value FromOutVal(IntPtr ptr) => FromOutVal(ptr, (p2) => new CefV8Value(p2, false));
		public cef_v8value_t* FixedPtr => (cef_v8value_t*)Ptr;
	}

}
