using System;
using System.Collections.Generic;
using System.Linq;
using CefLite.Interop;
using System.Threading.Tasks;

using System.Runtime.InteropServices;


namespace CefLite.Interop
{

	[StructLayout(LayoutKind.Sequential, Pack = 0)]
	public unsafe struct cef_v8context_t
	{
		public cef_base_ref_counted_t brc;
		public IntPtr get_task_runner;
		public IntPtr is_valid;
		public IntPtr get_browser;
		public IntPtr get_frame;
		public IntPtr get_global;
		public IntPtr enter;
		public IntPtr exit;
		public IntPtr is_same;
		public IntPtr eval;
	}

	public unsafe partial class CefV8Context
	{

		static public CefV8Context GetCurrent()
		{
			cef_v8context_t* ctx = cef_v8context_get_current_context();
			if (ctx == null)
				return null;
			//TODO:find all cases need FromOutVal
			return CefV8Context.FromOutVal(ctx);
		}



		public CefBrowser GetBrowser()
		{
			var gethandler = Marshal.GetDelegateForFunctionPointer<GetObjectHandler>(FixedPtr->get_browser);
			return CefBrowser.FromOutVal(gethandler(Ptr));
		}
		public CefFrame GetFrame()
		{
			var gethandler = Marshal.GetDelegateForFunctionPointer<GetObjectHandler>(FixedPtr->get_frame);
			return CefFrame.FromOutVal(gethandler(Ptr));
		}

		public CefV8Value Eval(string code)
		{
			if (_cache_eval == null)
				_cache_eval = Marshal.GetDelegateForFunctionPointer<delegate_eval>(FixedPtr->eval);
			CefString strcode = code ?? throw new ArgumentNullException(nameof(code));
			cef_v8value_t* result = null;
			cef_v8exception_t* exception = null;
			int r = _cache_eval(FixedPtr, strcode, null, 0, ref result, ref exception);
			CefV8Value res = CefV8Value.FromOutVal(result);
			CefV8Exception err = CefV8Exception.FromOutVal(exception);
			Console.WriteLine("Eval result : " + r + ":" + (IntPtr)result + ":" + (IntPtr)exception);
			if (err != null)
				throw new Exception("JSERROR:" + err.Message);
			if (r == 0)
				throw new Exception("JSFAILED");
			return res;

		}
		delegate_eval _cache_eval;
		delegate int delegate_eval(cef_v8context_t* v8c, cef_string_t* code, cef_string_t* url, int start_line, ref cef_v8value_t* retval, ref cef_v8exception_t* exception);


		/// <summary>
		/// This method shall be called in webkit_initialized
		/// </summary>
		static public int CefRegisterExtension(string name, string code, CefV8Handler handler = null)
		{
			CefString strname = name;
			CefString strcode = code;
			return cef_register_extension(strname, strcode, handler.AsIntPtr());
		}

		[DllImport("libcef.dll")]
		extern static public int cef_register_extension(IntPtr name, IntPtr code, IntPtr handler);

		[DllImport("libcef.dll")]
		extern static public cef_v8context_t* cef_v8context_get_current_context();

		//TODO:DllImport
		//cef_v8context_get_current_context
		//cef_v8context_get_entered_context
		//cef_v8context_in_context

		//cef_v8stack_trace_get_current

		//cef_v8value_create_function
		//cef_v8value_create_array_buffer
		//cef_v8value_create_array
		//cef_v8value_create_object
		//cef_v8value_create_string
		//cef_v8value_create_date
		//cef_v8value_create_double
		//cef_v8value_create_uint
		//cef_v8value_create_int
		//cef_v8value_create_bool
		//cef_v8value_create_null
		//cef_v8value_create_undefined
	}



	[StructLayout(LayoutKind.Sequential, Pack = 0)]
	public unsafe struct cef_v8handler_t
	{
		public cef_base_ref_counted_t brc;
		public IntPtr execute;

		//int(CEF_CALLBACK* execute)(struct _cef_v8handler_t* self,
		//                   const cef_string_t* name,
		//                   struct _cef_v8value_t* object,
		//                   size_t argumentsCount,
		//                   struct _cef_v8value_t* const* arguments,
		//                   struct _cef_v8value_t** retval,
		//                   cef_string_t* exception);
	}

	public unsafe partial class CefV8Handler : ObjectFromNet<cef_v8handler_t, CefV8Handler>
	{
		public cef_v8handler_t* FixedPtr => (cef_v8handler_t*)Ptr;

		public CefV8Handler()
		{
			FixedPtr->execute = holder_execute;
		}

		delegate int delegate_execute(IntPtr self, cef_string_t* name, cef_v8value_t* obj, size_t argcount, cef_v8value_t** args, ref cef_v8value_t* retval, cef_string_t* error);

		static DelegateHolder<delegate_execute> holder_execute = new DelegateHolder<delegate_execute>(
			(IntPtr self, cef_string_t* name, cef_v8value_t* obj, size_t argcount, cef_v8value_t** args, ref cef_v8value_t* retval, cef_string_t* error) =>
		{
			CefV8Handler inst = CefV8Handler.GetInstance(self);
			string strname = cef_string_t.ToString(name);
			int argc = (int)argcount.value;
			CefWin.WriteDebugLine("CefV8Handler.execute:" + strname + "," + argc);

			if (inst.Execute == null)
			{
				if (error != null)
					F.invoke_set(error, "not implement, no Execute");
				return 1;
			}
			try
			{
				List<CefV8Value> arglist = new List<CefV8Value>();
				for (int i = 0; i < argc; i++)
				{
					arglist.Add(CefV8Value.FromInArg(args[i]));
				}
				object result = inst.Execute(inst, strname, arglist.ToArray());
				//TODO: set retval from result
				if (result != null)
					throw (new NotImplementedException("not support Execute result yet."));
			}
			catch (Exception x)
			{
				CefWin.WriteDebugLine(x);
				if (error != null)
					F.invoke_set(error, x.Message);
				return 1;
			}
			return 1;
		});

		public Func<CefV8Handler, string, CefV8Value[], object> Execute { get; set; }

	}



	[StructLayout(LayoutKind.Sequential, Pack = 0)]
	public unsafe struct cef_v8exception_t
	{
		public cef_base_ref_counted_t brc;
		public IntPtr get_message;
		public IntPtr get_source_line;
		public IntPtr get_script_resource_name;
		public IntPtr get_line_number;
		public IntPtr get_start_position;
		public IntPtr get_end_position;
		public IntPtr get_start_column;
		public IntPtr get_end_column;
	}

	public unsafe partial class CefV8Exception
	{
		string _msg;
		public string Message
		{
			get
			{
				if (_msg == null)
				{
					var func = Marshal.GetDelegateForFunctionPointer<GetObjectHandler>(FixedPtr->get_message);
					_msg = CefString.FromUserFree(func(Ptr))?.ToString();
				}
				return _msg;
			}
		}
	}

}
