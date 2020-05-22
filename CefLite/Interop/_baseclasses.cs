using System;
using System.Collections.Generic;
using System.Linq;
using CefLite.Interop;
using System.Threading.Tasks;

using System.Runtime.InteropServices;

namespace CefLite.Interop
{
    //must use these delegates , because Marshal.GetFunctionPointerForDelegate , GetDelegateForFunctionPointer dont support delegate with generic types
    public delegate int GetInt32Handler(IntPtr self);
    public delegate uint GetUInt32Handler(IntPtr self);
    public delegate int GetInt64Handler(IntPtr self);
    public delegate float GetSingleHandler(IntPtr self);
    public delegate double GetDoubleHandler(IntPtr self);
    public delegate IntPtr GetObjectHandler(IntPtr self);
    public delegate IntPtr GetObjectHandler2(IntPtr self,IntPtr index);

    public delegate void SetInt32Handler(IntPtr self, int val);
    public delegate void SetSingleHandler(IntPtr self,float val);
    public delegate void SetDoubleHandler(IntPtr self,double val);
    public delegate void EventCallbackHandler(IntPtr self);
    public delegate void EventCallbackHandler2(IntPtr self, IntPtr p2);
    public delegate void EventCallbackHandler3(IntPtr self, IntPtr p2, IntPtr p3);
    public delegate void EventCallbackHandler4(IntPtr self, IntPtr p2, IntPtr p3, IntPtr p4);

    //special delegates with ref/out
    public delegate int CallbackOnBeforePopup(CefLifeSpanHandler handler, CefBrowser browser, CefFrame frame, string url, string name, cef_window_open_disposition_t dispostion, int user_gesture, CefPopupFeatures features, CefWindowInfo wininfo, ref CefClient cient, CefBrowserSettings settings, CefDictionaryValue extra_info, ref int no_javascript_access);



    public struct size_t
    {
        public UIntPtr value;

        static public implicit operator size_t(int size)
        {
            size_t v;
            v.value = (UIntPtr)size;
            return v;
        }
        static public implicit operator size_t(uint size)
        {
            size_t v;
            v.value = (UIntPtr)size;
            return v;
        }
        static public implicit operator size_t(IntPtr size)
        {
            size_t v;
            v.value = new UIntPtr((ulong)size.ToInt64());
            return v;
        }

        static public implicit operator IntPtr(size_t size)
        {
            return (IntPtr)(long)size.value.ToUInt64();
        }

    }


    public enum cef_value_type_t
    {
        VTYPE_INVALID = 0,
        VTYPE_NULL,
        VTYPE_BOOL,
        VTYPE_INT,
        VTYPE_DOUBLE,
        VTYPE_STRING,
        VTYPE_BINARY,
        VTYPE_DICTIONARY,
        VTYPE_LIST,
    }

    public enum cef_return_value_t
    {
        RV_CANCEL = 0,

        RV_CONTINUE,

        RV_CONTINUE_ASYNC,
    }

    public enum cef_process_id_t
    {

        PID_BROWSER,

        PID_RENDERER,
    }


    public enum cef_state_t
    {
        /// <summary>
        /// Use the default state for the setting.
        /// </summary>
        STATE_DEFAULT = 0,

        /// <summary>
        /// Enable or allow the setting.
        /// </summary>
        STATE_ENABLED,

        /// <summary>
        /// Disable or disallow the setting.
        /// </summary>
        STATE_DISABLED,
    }

    public enum cef_log_severity_t
    {
        /// <summary>
        /// Default logging (currently INFO logging).
        /// </summary>
        LOGSEVERITY_DEFAULT,

        /// <summary>
        /// Verbose logging.
        /// </summary>
        LOGSEVERITY_VERBOSE,

        /// <summary>
        /// DEBUG logging.
        /// </summary>
        LOGSEVERITY_DEBUG = LOGSEVERITY_VERBOSE,

        /// <summary>
        /// INFO logging.
        /// </summary>
        LOGSEVERITY_INFO,

        /// <summary>
        /// WARNING logging.
        /// </summary>
        LOGSEVERITY_WARNING,

        /// <summary>
        /// ERROR logging.
        /// </summary>
        LOGSEVERITY_ERROR,

        /// <summary>
        /// FATAL logging.
        /// </summary>
        LOGSEVERITY_FATAL,

        /// <summary>
        /// Disable logging to file for all messages, and to stderr for messages with
        /// severity less than FATAL.
        /// </summary>
        LOGSEVERITY_DISABLE = 99

    }

    public enum cef_window_open_disposition_t
    {
        WOD_UNKNOWN,
        WOD_CURRENT_TAB,
        WOD_SINGLETON_TAB,
        WOD_NEW_FOREGROUND_TAB,
        WOD_NEW_BACKGROUND_TAB,
        WOD_NEW_POPUP,
        WOD_NEW_WINDOW,
        WOD_SAVE_TO_DISK,
        WOD_OFF_THE_RECORD,
        WOD_IGNORE_ACTION
    }
}
