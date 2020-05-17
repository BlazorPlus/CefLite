using System;
using System.Collections.Generic;
using System.Linq;
using CefLite.Interop;
using System.Threading.Tasks;

using System.Runtime.InteropServices;


namespace CefLite.Interop
{

    [StructLayout(LayoutKind.Sequential, Pack = 0)]
    public unsafe struct cef_frame_t
    {
        public cef_base_ref_counted_t brc;
        public IntPtr is_valid;
        public IntPtr undo;
        public IntPtr redo;
        public IntPtr cut;
        public IntPtr copy;
        public IntPtr paste;
        public IntPtr del;
        public IntPtr select_all;
        public IntPtr view_source;
        public IntPtr get_source;
        public IntPtr get_text;
        public IntPtr load_request;
        public IntPtr load_url;
        public IntPtr execute_java_script;
        public IntPtr is_main;
        public IntPtr is_focused;
        public IntPtr get_name;
        public IntPtr get_identifier;
        public IntPtr get_parent;
        public IntPtr get_url;
        public IntPtr get_browser;
        public IntPtr get_v8context;
        public IntPtr visit_dom;
        public IntPtr create_urlrequest;
        public IntPtr send_process_message; //on_process_message_received
    }



    public unsafe partial class CefFrame
    {
       
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

        public string Url
        {
            get
            {
                var func = Marshal.GetDelegateForFunctionPointer<GetObjectHandler>(FixedPtr->get_url);
                return CefString.FromUserFree(func(Ptr))?.ToString();
            }
        }

        public void ExecuteJavaScript(string code, string url = null, int start_line = 0)
        {
            var func=Marshal.GetDelegateForFunctionPointer<delegate_execute_java_script>(FixedPtr->execute_java_script);
            CefString cefcode = code ?? throw new ArgumentNullException(nameof(code));
            CefString cefurl=url;
            func(FixedPtr, cefcode, cefurl, start_line);
        }
        delegate void delegate_execute_java_script(cef_frame_t* frame, cef_string_t* code, cef_string_t* url, int line);

        public void SendProcessMessage(cef_process_id_t target_pid, string name)
        {

            CefString cefname = name ?? throw new ArgumentNullException(nameof(name));
            //TODO:issue , cef_process_message_create , FromOutVal failed.. Check why,
            CefProcessMessage cefmsg = CefProcessMessage.FromInArg(cef_process_message_create(cefname));
            SendProcessMessage(target_pid, cefmsg);
        }
        public void SendProcessMessage(cef_process_id_t target_pid, string name, params string[] args)
        {

            CefString cefname = name ?? throw new ArgumentNullException(nameof(name));
            //TODO:issue , cef_process_message_create , FromOutVal failed.. Check why,
            CefProcessMessage cefmsg = CefProcessMessage.FromInArg(cef_process_message_create(cefname));
            if (args != null)
            {
                var list = cefmsg.GetArgumentList();
                list.SetSize(args.Length);
                for (uint i = 0; i < args.Length; i++)
                {
                    list.SetString(i, args[i]);
                }
            }
            SendProcessMessage(target_pid, cefmsg);
        }


        public void SendProcessMessage(cef_process_id_t target_pid, CefProcessMessage cefmsg)
        {
            if (cefmsg == null) throw new ArgumentNullException(nameof(cefmsg));
            if (_cache_send_process_message == null)
                _cache_send_process_message = Marshal.GetDelegateForFunctionPointer<delegate_send_process_message>(FixedPtr->send_process_message);
            _cache_send_process_message(FixedPtr, target_pid, cefmsg.FixedPtr);
        }

        delegate_send_process_message _cache_send_process_message;
        delegate void delegate_send_process_message(cef_frame_t* frame, cef_process_id_t pid, cef_process_message_t* msg);

        [DllImport("libcef.dll")]
        extern static public IntPtr cef_process_message_create(IntPtr name);
    }
}
