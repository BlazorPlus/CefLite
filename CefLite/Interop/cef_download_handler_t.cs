using System;
using System.Collections.Generic;
using System.Linq;
using CefLite.Interop;
using System.Threading.Tasks;

using System.Runtime.InteropServices;


namespace CefLite.Interop
{


	[StructLayout(LayoutKind.Sequential, Pack = 0)]
	public unsafe struct cef_download_handler_t
	{
		public cef_base_ref_counted_t brc;
		public IntPtr on_before_download;
		public IntPtr on_download_updated;
	}

	public unsafe partial class CefDownloadHandler : ObjectFromNet<cef_download_handler_t, CefDownloadHandler>
	{
		public cef_download_handler_t* FixedPtr => (cef_download_handler_t*)Ptr;

		public CefClient Client { get; private set; }

		public CefDownloadHandler(CefClient ownerClient)
		{
			Client = ownerClient;
			cef_download_handler_t* self = FixedPtr;
			self->on_before_download = holder_on_before_download;
			self->on_download_updated = holder_on_download_updated;
		}

		delegate void func_on_before_download(IntPtr self, cef_browser_t* browser, cef_download_item_t* item, cef_string_t* suggested_name, cef_before_download_callback_t* callback);
		static DelegateHolder<func_on_before_download> holder_on_before_download = new DelegateHolder<func_on_before_download>(
			(IntPtr self, cef_browser_t* browser, cef_download_item_t* item, cef_string_t* suggested_name, cef_before_download_callback_t* callback) =>
			{
				var inst = GetInstance(self);
				inst.BeforeDownload?.Invoke(inst, CefBrowser.FromInArg(browser), CefDownloadItem.FromInArg(item), cef_string_t.ToString(suggested_name), CefBeforeDownloadCallback.FromInArg(callback));
			});

		public Action<CefDownloadHandler, CefBrowser, CefDownloadItem, string, CefBeforeDownloadCallback> BeforeDownload { get; set; }

		delegate void func_on_download_updated(IntPtr self, cef_browser_t* browser, cef_download_item_t* item, cef_download_item_callback_t* callback);
		static DelegateHolder<func_on_download_updated> holder_on_download_updated = new DelegateHolder<func_on_download_updated>(
			(IntPtr self, cef_browser_t* browser, cef_download_item_t* item, cef_download_item_callback_t* callback) =>
			{
				var inst = GetInstance(self);
				inst.DownloadUpdated?.Invoke(inst, CefBrowser.FromInArg(browser), CefDownloadItem.FromInArg(item), CefDownloadItemCallback.FromInArg(callback));
			});

		public Action<CefDownloadHandler, CefBrowser, CefDownloadItem, CefDownloadItemCallback> DownloadUpdated { get; set; }


	}


	[StructLayout(LayoutKind.Sequential, Pack = 0)]
	public unsafe struct cef_download_item_t
	{
		public cef_base_ref_counted_t brc;
		public IntPtr is_valid;
		public IntPtr is_in_progress;
		public IntPtr is_complete;
		public IntPtr is_canceled;
		public IntPtr get_current_speed;
		public IntPtr get_percent_complete;
		public IntPtr get_total_bytes;
		public IntPtr get_received_bytes;
		public IntPtr get_start_time;
		public IntPtr get_end_time;
		public IntPtr get_full_path;
		public IntPtr get_id;
		public IntPtr get_url;
		public IntPtr get_original_url;
		public IntPtr get_suggested_file_name;
		public IntPtr get_content_disposition;
		public IntPtr get_mime_type;
	}
	public unsafe partial class CefDownloadItem
	{
		public bool IsValid => Marshal.GetDelegateForFunctionPointer<GetInt32Handler>(FixedPtr->is_valid)(Ptr) != 0;
		public bool IsInProgress => Marshal.GetDelegateForFunctionPointer<GetInt32Handler>(FixedPtr->is_in_progress)(Ptr) != 0;
		public bool IsCanceled => Marshal.GetDelegateForFunctionPointer<GetInt32Handler>(FixedPtr->is_canceled)(Ptr) != 0;
		public bool IsComplete => Marshal.GetDelegateForFunctionPointer<GetInt32Handler>(FixedPtr->is_complete)(Ptr) != 0;

		public uint Id => Marshal.GetDelegateForFunctionPointer<GetUInt32Handler>(FixedPtr->get_id)(Ptr);
		public int PercentComplete => Marshal.GetDelegateForFunctionPointer<GetInt32Handler>(FixedPtr->get_percent_complete)(Ptr);
		public long CurrentSpeed => Marshal.GetDelegateForFunctionPointer<GetInt64Handler>(FixedPtr->get_current_speed)(Ptr);
		public long TotalBytes => Marshal.GetDelegateForFunctionPointer<GetInt64Handler>(FixedPtr->get_total_bytes)(Ptr);
		public long ReceivedBytes => Marshal.GetDelegateForFunctionPointer<GetInt64Handler>(FixedPtr->get_received_bytes)(Ptr);

		public string FullPath => CefString.FromUserFree(Marshal.GetDelegateForFunctionPointer<GetObjectHandler>(FixedPtr->get_full_path)(Ptr))?.ToString();
		public string Url => CefString.FromUserFree(Marshal.GetDelegateForFunctionPointer<GetObjectHandler>(FixedPtr->get_url)(Ptr))?.ToString();
		public string OriginalUrl => CefString.FromUserFree(Marshal.GetDelegateForFunctionPointer<GetObjectHandler>(FixedPtr->get_original_url)(Ptr))?.ToString();
		public string SuggestedFileName => CefString.FromUserFree(Marshal.GetDelegateForFunctionPointer<GetObjectHandler>(FixedPtr->get_suggested_file_name)(Ptr))?.ToString();
		public string ContentDisposition => CefString.FromUserFree(Marshal.GetDelegateForFunctionPointer<GetObjectHandler>(FixedPtr->get_content_disposition)(Ptr))?.ToString();
		public string MimeType => CefString.FromUserFree(Marshal.GetDelegateForFunctionPointer<GetObjectHandler>(FixedPtr->get_mime_type)(Ptr))?.ToString();
	}

	[StructLayout(LayoutKind.Sequential, Pack = 0)]
	public unsafe struct cef_before_download_callback_t
	{
		public cef_base_ref_counted_t brc;
		public IntPtr cont;
	}

	public unsafe partial class CefBeforeDownloadCallback
	{
		public void Cont(string download_path = null, bool show_dialog = true)
		{
			var func = Marshal.GetDelegateForFunctionPointer<delegate_cont>(FixedPtr->cont);
			CefString cefpath = download_path ?? "";
			func(Ptr, cefpath, show_dialog ? 1 : 0);
		}
		delegate void delegate_cont(IntPtr callback, IntPtr download_path, int show_dialog);

	}

	[StructLayout(LayoutKind.Sequential, Pack = 0)]
	public unsafe struct cef_download_item_callback_t
	{
		public cef_base_ref_counted_t brc;
		public IntPtr cancel;
		public IntPtr pause;
		public IntPtr resume;
	}

	public unsafe partial class CefDownloadItemCallback
	{
		
		public void Cancel()
		{
			var func = Marshal.GetDelegateForFunctionPointer<EventCallbackHandler>(FixedPtr->cancel);
			func(Ptr);
		}

		public void Pause()
		{
			var func = Marshal.GetDelegateForFunctionPointer<EventCallbackHandler>(FixedPtr->pause);
			func(Ptr);
		}

		public void Resume()
		{
			var func = Marshal.GetDelegateForFunctionPointer<EventCallbackHandler>(FixedPtr->resume);
			func(Ptr);
		}


	}
}
