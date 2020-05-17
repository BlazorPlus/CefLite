using System;
using System.Collections.Generic;
using System.Linq;
using CefLite.Interop;
using System.Threading.Tasks;

using System.Runtime.InteropServices;


namespace CefLite.Interop
{

    [StructLayout(LayoutKind.Sequential, Pack = 0)]
    public unsafe struct cef_browser_host_t
    {
        public cef_base_ref_counted_t brc;
        public IntPtr get_browser;
        public IntPtr close_browser;
        public IntPtr try_close_browser;
        public IntPtr set_focus;
        public IntPtr get_window_handle;
        public IntPtr get_opener_window_handle;
        public IntPtr has_view;
        public IntPtr get_client;
        public IntPtr get_request_context;
        public IntPtr get_zoom_level;
        public IntPtr set_zoom_level;
        public IntPtr run_file_dialog;
        public IntPtr start_download;
        public IntPtr download_image;
        public IntPtr print;
        public IntPtr print_to_pdf;
        public IntPtr find;
        public IntPtr stop_finding;
        public IntPtr show_dev_tools;
        public IntPtr close_dev_tools;
        public IntPtr has_dev_tools;
        public IntPtr get_navigation_entries;
        public IntPtr set_mouse_cursor_change_disabled;
        public IntPtr is_mouse_cursor_change_disabled;
        public IntPtr replace_misspelling;
        public IntPtr add_word_to_dictionary;
        public IntPtr is_window_rendering_disabled;
        public IntPtr was_resized;
        public IntPtr was_hidden;
        public IntPtr notify_screen_info_changed;
        public IntPtr invalidate;
        public IntPtr send_external_begin_frame;
        public IntPtr send_key_event;
        public IntPtr send_mouse_click_event;
        public IntPtr send_mouse_move_event;
        public IntPtr send_mouse_wheel_event;
        public IntPtr send_touch_event;
        public IntPtr send_focus_event;
        public IntPtr send_capture_lost_event;
        public IntPtr notify_move_or_resize_started;
        public IntPtr get_windowless_frame_rate;
        public IntPtr set_windowless_frame_rate;
        public IntPtr ime_set_composition;
        public IntPtr ime_commit_text;
        public IntPtr ime_finish_composing_text;
        public IntPtr ime_cancel_composition;
        public IntPtr drag_target_drag_enter;
        public IntPtr drag_target_drag_over;
        public IntPtr drag_target_drag_leave;
        public IntPtr drag_target_drop;
        public IntPtr drag_source_ended_at;
        public IntPtr drag_source_system_drag_ended;
        public IntPtr get_visible_navigation_entry;
        public IntPtr set_accessibility_state;
        public IntPtr set_auto_resize_enabled;
        public IntPtr get_extension;
        public IntPtr is_background_host;
        public IntPtr set_audio_muted;
        public IntPtr is_audio_muted;

    }

    public unsafe partial class CefBrowserHost
    {
       

        public IntPtr GetWindowHandle()
        {
            return F.invoke_get_window_handle(FixedPtr);
        }

        public void CloseBrowser()
        {
            var func = Marshal.GetDelegateForFunctionPointer<delegate_close_browser>(FixedPtr->close_browser);
            func(FixedPtr, 1);
        }
        delegate void delegate_close_browser(cef_browser_host_t* host, int force_close);

    }

}
