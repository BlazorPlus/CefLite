using System;
using System.Collections.Generic;
using System.Linq;
using CefLite.Interop;
using System.Threading.Tasks;

using System.Runtime.InteropServices;


namespace CefLite.Interop
{
	public struct cef_key_event_t
	{
		cef_key_event_type_t type;

		uint modifiers;

		int windows_key_code;

		int native_key_code;

		int is_system_key;

		char character;

		char unmodified_character;

		int focus_on_editable_field;
	}

	public enum cef_key_event_type_t
	{
		KEYEVENT_RAWKEYDOWN = 0,
		KEYEVENT_KEYDOWN,
		KEYEVENT_KEYUP,
		KEYEVENT_CHAR
	}

}
