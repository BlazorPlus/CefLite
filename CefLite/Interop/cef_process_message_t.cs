using System;
using System.Collections.Generic;
using System.Linq;
using CefLite.Interop;
using System.Threading.Tasks;

using System.Runtime.InteropServices;


namespace CefLite.Interop
{


	[StructLayout(LayoutKind.Sequential, Pack = 0)]
	public unsafe struct cef_process_message_t
	{
		public cef_base_ref_counted_t brc;
		public IntPtr is_valid;
		public IntPtr is_read_only;
		public IntPtr copy;
		public IntPtr get_name;
		public IntPtr get_argument_list;
	}

	public unsafe partial class CefProcessMessage
	{
		public string GetName()
		{
			return CefString.FromUserFree(Marshal.GetDelegateForFunctionPointer<GetObjectHandler>(FixedPtr->get_name)(Ptr))?.ToString();
		}

		public CefListValue GetArgumentList()
		{
			return CefListValue.FromOutVal(Marshal.GetDelegateForFunctionPointer<GetObjectHandler>(FixedPtr->get_argument_list)(Ptr));
		}

		public override string ToString()
		{
			string name = GetName();
			CefListValue list = GetArgumentList();
			uint size = list.GetSize().value.ToUInt32();
			if (size == 0)
				return name;
			string[] strs = new string[size];
			for (uint i = 0; i < size; i++)
			{
				strs[i] = list.GetString(i);
			}
			return name + ":[" + string.Join(",", strs) + "]";
		}

	}

}