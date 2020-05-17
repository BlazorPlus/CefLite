using System;
using System.Collections.Generic;
using System.Linq;
using CefLite.Interop;
using System.Threading.Tasks;

using System.Runtime.InteropServices;


namespace CefLite.Interop
{


    [StructLayout(LayoutKind.Sequential, Pack = 0)]
    public unsafe struct cef_client_t
    {
        public cef_base_ref_counted_t brc;
        public IntPtr get_context_menu_handler;
        public IntPtr get_dialog_handler;
        public IntPtr get_display_handler;
        public IntPtr get_download_handler;
        public IntPtr get_drag_handler;
        public IntPtr get_find_handler;
        public IntPtr get_focus_handler;
        public IntPtr get_jsdialog_handler;
        public IntPtr get_keyboard_handler;
        public IntPtr get_life_span_handler;
        public IntPtr get_load_handler;
        public IntPtr get_render_handler;
        public IntPtr get_request_handler;
        public IntPtr on_process_message_received;  //   // Render process cef_frame_t::send_process_message
    }

    public unsafe partial class CefClient : ObjectFromNet<cef_client_t, CefClient>
    {
        public cef_client_t* FixedPtr => (cef_client_t*)Ptr;

        public CefClient()
        {
            RequestHandler = new CefRequestHandler(this);
            LifeSpanHandler = new CefLifeSpanHandler(this);
            DownloadHandler = new CefDownloadHandler(this);
            LoadHandler = new CefLoadHandler(this);

            cef_client_t* self = FixedPtr;
            self->get_request_handler = holder_get_request_handler;
            self->get_life_span_handler = holder_get_life_span_handler;
            self->get_download_handler = holder_get_download_handler;
            self->get_load_handler = holder_get_load_handler;
            self->on_process_message_received = holder_on_process_message_received;
        }

        static DelegateHolder<delegate_on_process_message_received> holder_on_process_message_received = new DelegateHolder<delegate_on_process_message_received>(
            (cef_client_t* client, cef_browser_t* browser, cef_frame_t* frame, cef_process_id_t pid, cef_process_message_t* message) =>
            {
                CefProcessMessage cefmsg = CefProcessMessage.FromInArg(message);
                CefWin.WriteDebugLine("CefClient:on_process_message_received:" + cefmsg.ToString());
                return 0;
            });
        delegate int delegate_on_process_message_received(cef_client_t* client, cef_browser_t* browser, cef_frame_t* frame, cef_process_id_t pid, cef_process_message_t* message);


        public CefRequestHandler RequestHandler { get; set; } 
        public CefLifeSpanHandler LifeSpanHandler { get; set; } 
        public CefDownloadHandler DownloadHandler { get; set; } 
        public CefLoadHandler LoadHandler { get; set; } 

        static DelegateHolder<GetObjectHandler> holder_get_request_handler = new DelegateHolder<GetObjectHandler>(
           app => GetInstance(app).RequestHandler.AddRefReturnIntPtr());

        static DelegateHolder<GetObjectHandler> holder_get_life_span_handler = new DelegateHolder<GetObjectHandler>(
           app => GetInstance(app).LifeSpanHandler.AddRefReturnIntPtr());

        static DelegateHolder<GetObjectHandler> holder_get_download_handler = new DelegateHolder<GetObjectHandler>(
          app => GetInstance(app).DownloadHandler.AddRefReturnIntPtr());

        static DelegateHolder<GetObjectHandler> holder_get_load_handler = new DelegateHolder<GetObjectHandler>(
            app => GetInstance(app).LoadHandler.AddRefReturnIntPtr());


        //Addition options :
        //public bool OptionCloseForFirstDownload { get; set; } = true;
    }


    [StructLayout(LayoutKind.Sequential, Pack = 0)]
    public unsafe struct cef_context_menu_handler_t
    {
        public cef_base_ref_counted_t brc;
        public IntPtr on_before_context_menu;
        public IntPtr run_context_menu;
        public IntPtr on_context_menu_command;
        public IntPtr on_context_menu_dismissed;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 0)]
    public unsafe struct cef_dialog_handler_t
    {

        public cef_base_ref_counted_t brc;
        public IntPtr on_file_dialog;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 0)]
    public unsafe struct cef_display_handler_t
    {
        public cef_base_ref_counted_t brc;
        public IntPtr on_address_change;
        public IntPtr on_title_change;
        public IntPtr on_favicon_urlchange;
        public IntPtr on_fullscreen_mode_change;
        public IntPtr on_tooltip;
        public IntPtr on_status_message;
        public IntPtr on_console_message;
        public IntPtr on_auto_resize;
        public IntPtr on_loading_progress_change;
    }


    [StructLayout(LayoutKind.Sequential, Pack = 0)]
    public unsafe struct cef_drag_handler_t
    {
        public cef_base_ref_counted_t brc;
        public IntPtr on_drag_enter;
        public IntPtr on_draggable_regions_changed;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 0)]
    public unsafe struct cef_find_handler_t
    {
        public cef_base_ref_counted_t brc;
        public IntPtr on_find_result;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 0)]
    public unsafe struct cef_focus_handler_t
    {
        public cef_base_ref_counted_t brc;
        public IntPtr on_take_focus;
        public IntPtr on_set_focus;
        public IntPtr on_got_focus;
    }



    [StructLayout(LayoutKind.Sequential, Pack = 0)]
    public unsafe struct cef_jsdialog_handler_t
    {
        public cef_base_ref_counted_t brc;
        public IntPtr on_jsdialog;
        public IntPtr on_before_unload_dialog;
        public IntPtr on_reset_dialog_state;
        public IntPtr on_dialog_closed;
    }



    [StructLayout(LayoutKind.Sequential, Pack = 0)]
    public unsafe struct cef_keyboard_handler_t
    {
        public cef_base_ref_counted_t brc;
        public IntPtr on_pre_key_event;
        public IntPtr on_key_event;
    }




    [StructLayout(LayoutKind.Sequential, Pack = 0)]
    public unsafe struct cef_render_handler_t
    {
        public cef_base_ref_counted_t brc;
        public IntPtr get_accessibility_handler;
        public IntPtr get_root_screen_rect;
        public IntPtr get_view_rect;
        public IntPtr get_screen_point;
        public IntPtr get_screen_info;
        public IntPtr on_popup_show;
        public IntPtr on_popup_size;
        public IntPtr on_paint;
        public IntPtr on_accelerated_paint;
        public IntPtr on_cursor_change;
        public IntPtr start_dragging;
        public IntPtr update_drag_cursor;
        public IntPtr on_scroll_offset_changed;
        public IntPtr on_ime_composition_range_changed;
        public IntPtr on_text_selection_changed;
        public IntPtr on_virtual_keyboard_requested;
    }












}
