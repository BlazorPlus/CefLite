using System;
using System.Collections.Generic;
using System.Linq;
using CefLite.Interop;
using System.Threading.Tasks;

using System.Runtime.InteropServices;


namespace CefLite.Interop
{

	[StructLayout(LayoutKind.Sequential, Pack = 0)]
	public unsafe struct cef_load_handler_t
	{
		public cef_base_ref_counted_t brc;
		public IntPtr on_loading_state_change;
		public IntPtr on_load_start;
		public IntPtr on_load_end;
		public IntPtr on_load_error;
	}

	public unsafe partial class CefLoadHandler : ObjectFromNet<cef_load_handler_t, CefLoadHandler>
	{
		public cef_load_handler_t* FixedPtr => (cef_load_handler_t*)Ptr;

		public CefClient Client { get; private set; }
		public CefLoadHandler(CefClient ownerClient)
		{
			Client = ownerClient;
			FixedPtr->on_load_end = holder_on_load_end;
		}

		delegate void func_on_load_end(IntPtr self, cef_browser_t* browser, cef_frame_t* frame, int httpStatus);
		static DelegateHolder<func_on_load_end> holder_on_load_end = new DelegateHolder<func_on_load_end>(
			(IntPtr self, cef_browser_t* browser, cef_frame_t* frame, int httpStatus) =>
			{
				var inst = GetInstance(self);
				inst.LoadEnd?.Invoke(inst, CefBrowser.FromInArg(browser), CefFrame.FromInArg(frame), httpStatus);
			});

		public Action<CefLoadHandler, CefBrowser, CefFrame, int> LoadEnd { get; set; }
	}

}
