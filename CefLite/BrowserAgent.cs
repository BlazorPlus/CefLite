using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;

using CefLite.Interop;

namespace CefLite
{

    public unsafe class BrowserAgent
    {
        public CefBrowserSettings Settings { get; } = new CefBrowserSettings();
        public CefWindowInfo WindowInfo { get; } = new CefWindowInfo();
        public CefClient Client { get; } = new CefClient();

        public CefBrowser Browser { get; private set; }

        public CefFrame MainFrame { get; private set; }
        public CefBrowserHost BrowserHost { get; private set; }

        IntPtr hostWindowHandle;

        public void WaitBrowserReady()
        {
            while (Browser == null)
            {
                CefWin.DoMessageLoopOnce();
            }
            CefWin.DoMessageLoopOnce();
        }

        public BrowserAgent()
        {
            InitSettingsCommon(Settings);
            InitClientCommon(Client);
        }

        public string Url { get; set; }

        public unsafe void InitAsChild(IntPtr handle, System.Drawing.Rectangle rect)
        {

            cef_window_info_t* pinfo = WindowInfo.FixedPtr;

            pinfo->parent_window = handle;
            pinfo->x = rect.Left;
            pinfo->y = rect.Top;
            pinfo->width = rect.Width;
            pinfo->height = rect.Height;

            CefWin.WriteDebugLine("Creating browser " + CefWin.ApplicationElapsed);

            Browser = CefBrowser.CreateBrowserSync(WindowInfo, Client, Url, Settings);
            MainFrame = Browser.GetMainFrame();
            BrowserHost = Browser.GetHost();
            hostWindowHandle = BrowserHost.GetWindowHandle();

            CefWin.WriteDebugLine("Creating browser OK doc:" + Browser.HasDocument + " id:" + Browser.Identifier + ":" + MainFrame.Identifier + " " + CefWin.ApplicationElapsed);
            CefWin.WriteDebugLine("send msgfrombrowser");
            MainFrame.SendProcessMessage(cef_process_id_t.PID_RENDERER, "msgfrombrowser");

            //do not call here, document is not ready!
            //MainFrame.ExecuteJavaScript("console.log('hello CefLite')");

            //ShowDevTools();
        }

        class DevToolsForm : System.Windows.Forms.Form
        {
            CefClient client = new CefClient();//must keep the reference , unknown bug cause recycled.

            IntPtr hostWindowHandle;

            BrowserAgent _owner;

            public DevToolsForm(BrowserAgent owner)
            {
                _owner = owner;

                this.Text = "DevTools";
                this.Width = 800;
                this.Height = 600;

                if (CefWin.ApplicationIcon != null)
                    this.Icon = CefWin.ApplicationIcon;

                this.MinimumSize = new System.Drawing.Size(360, 360);

               

                this.Load += DevToolsForm_Load;
            }

            private void DevToolsForm_Load(object sender, EventArgs e)
            {

                CefWindowInfo devwin_info = new CefWindowInfo();
                CefBrowserSettings browser_settings = new CefBrowserSettings();

                _owner.InitClientCommon(client);

                client.LifeSpanHandler.AfterCreated += (a, b) =>
                {
                    hostWindowHandle = b.GetHost().GetWindowHandle();
                };

                var rect = this.ClientRectangle;

                cef_window_info_t* pdevwin = devwin_info.FixedPtr;
                pdevwin->parent_window = this.Handle;
                pdevwin->x = rect.Left;
                pdevwin->y = rect.Top;
                pdevwin->width = rect.Width;
                pdevwin->height = rect.Height;

                var func = Marshal.GetDelegateForFunctionPointer<delegate_show_dev_tools>(_owner.BrowserHost.FixedPtr->show_dev_tools);
                func(_owner.BrowserHost.FixedPtr, devwin_info.Ptr, client, browser_settings, IntPtr.Zero);
            }
            delegate void delegate_show_dev_tools(cef_browser_host_t* host, IntPtr window_info, IntPtr client, IntPtr browser_settings, IntPtr pointInspect);

            protected override void OnResize(EventArgs e)
            {
                base.OnResize(e);

                if (hostWindowHandle != IntPtr.Zero)
                {
                    var rect = this.ClientRectangle;

                    WindowsInterop.SetWindowPos(hostWindowHandle, IntPtr.Zero, rect.Left, rect.Top, rect.Width, rect.Height, 0);
                }
            }

            protected override void Dispose(bool disposing)
            {
                base.Dispose(disposing);

                var func = Marshal.GetDelegateForFunctionPointer<EventCallbackHandler>(_owner.BrowserHost.FixedPtr->close_dev_tools);
                func((IntPtr)_owner.BrowserHost.FixedPtr);
            }
        }


        DevToolsForm _devtoolsForm;

        public void ShowDevTools()
        {
            if (_devtoolsForm != null && !_devtoolsForm.IsDisposed)
            {
                _devtoolsForm.Activate();
                return;
            }

            WaitBrowserReady();

            _devtoolsForm = new DevToolsForm(this);
            _devtoolsForm.Show();
        }

        public void OnResize(System.Drawing.Rectangle rect)
        {
            if (BrowserHost == null)
                return;

            WindowsInterop.SetWindowPos(hostWindowHandle, IntPtr.Zero, rect.Left, rect.Top, rect.Width, rect.Height, 0);

        }

        public void Dispose()
        {
            if (_devtoolsForm != null)
            {
                if (!_devtoolsForm.IsDisposed)
                    _devtoolsForm.Dispose();
                _devtoolsForm = null;
            }

            if (BrowserHost == null)
                return;

            BrowserHost.CloseBrowser();

        }


        public void InitSettingsCommon(CefBrowserSettings settings)
        {

        }


        public void InitClientCommon(CefClient client)
        {
            uint lastdownloadid = 0;
            string lastdownloadop = null;

            client.DownloadHandler.BeforeDownload +=
                   (CefDownloadHandler handler, CefBrowser browser, CefDownloadItem item, string suggested_name, CefBeforeDownloadCallback callback) =>
                   {
                       lastdownloadid = item.Id;
                       lastdownloadop = null;

                       CefWin.WriteDebugLine("OnBeforeDownload:" + suggested_name + " " + browser.IsPopup + ":" + browser.HasDocument);
                       CefWin.WriteDebugLine(item.Id + ":" + item.TotalBytes + ":" + item.Url);

                       CefWin.InvokeInAppThread(delegate
                       {
                           var parentForm = System.Windows.Forms.Control.FromChildHandle(browser.GetHost().GetWindowHandle())?.FindForm();

                           using (var dialog = new System.Windows.Forms.SaveFileDialog())
                           {
                               dialog.FileName = suggested_name;
                               var result = dialog.ShowDialog(parentForm);
                               if (result == System.Windows.Forms.DialogResult.OK)
                               {
                                   CefWin.WriteDebugLine("Cont:" + dialog.FileName);
                                   callback.Cont(dialog.FileName, false);
                                   lastdownloadop = "continue";
                               }
                               else
                               {
                                   lastdownloadop = "cancel";
                               }
                           }

                           if (!browser.HasDocument && parentForm != null)
                           {
                               parentForm.Hide();
                           }

                           if (lastdownloadop == "cancel")
                               return;


                           CefWin.PostToAppThread(delegate
                           {
                               DownloadItem.ShowDownloadForm(parentForm?.Visible == true ? parentForm : null);
                           });

                       });

                   };

            client.DownloadHandler.DownloadUpdated +=
                   (CefDownloadHandler handler, CefBrowser browser, CefDownloadItem item, CefDownloadItemCallback callback) =>
                   {
                       CefWin.WriteDebugLine("OnDownloadUpdated:" + " " + browser.IsPopup + ":" + browser.HasDocument);
                       CefWin.WriteDebugLine(item.Id + ":" + item.TotalBytes + ":" + item.Url);
                       CefWin.WriteDebugLine(item.IsInProgress + ":" + item.ReceivedBytes);

                       if (lastdownloadid == item.Id && lastdownloadop == "cancel")
                       {
                           CefWin.WriteDebugLine("Cancel.");
                           callback.Cancel();
                           return;
                       }

                       if (lastdownloadid == item.Id && lastdownloadop == "continue")
                       {
                           lastdownloadop = "download";
                           DownloadItem.Show(item, callback);
                       }
                       else
                       {
                           DownloadItem.PostVersionUpdateEvent();
                       }

                       //callback.Resume();
                       if (!browser.HasDocument && (item.IsCanceled || item.IsComplete))
                       {
                           //Problem , close the browser will stop the download??
                           browser.GetHost().CloseBrowser();
                       }
                   };

            client.LifeSpanHandler.AfterCreated += (a, b) =>
              {
                  CefWin.WriteDebugLine("OnAfterCreated:" + CefWin.ApplicationElapsed);
              };

            client.LifeSpanHandler.BeforePopup +=
                (CefLifeSpanHandler lifeSpanHandler, CefBrowser browser, CefFrame frame, string url, string name, cef_window_open_disposition_t dispostion, int user_gesture, CefPopupFeatures features, CefWindowInfo wininfo, ref CefClient cient, CefBrowserSettings settings, CefDictionaryValue extra_info, ref int no_javascript_access) =>
                {
                    CefWin.WriteDebugLine("OnBeforePopup:" + url + ":" + name);
                    //Show Popup in DefaultBrowserForm
                    CefWin.OpenBrowser(url);
                    return 1;
                };
            client.RequestHandler.OpenUrlFromTab +=
                (CefRequestHandler lifeSpanHandler, CefBrowser browser, CefFrame frame, string url, cef_window_open_disposition_t disposition) =>
                {
                    //This event means shift+click a link.
                    CefWin.WriteDebugLine("OnOpenUrlFromTab:" + url);
                    CefWin.OpenBrowser(url);
                };
#if DEBUG

            client.RequestHandler.BeforeBrowse +=
                (CefRequestHandler lifeSpanHandler, CefBrowser browser, CefFrame frame, CefRequest request, int user_gesture, int is_redirect) =>
                {
                    CefWin.WriteDebugLine("OnBeforeBrowse:" + user_gesture + ":" + is_redirect + " , " + request.IsReadOnly + ":" + request.Url);
                    //No effect , readonly.
                    //request.SetHeaderByName("Browser-Agent-Id", this.Id.ToString(),true);
                    return 0;
                };

            //client.RequestHandler.ResourceRequestHandler.BeforeResourceLoad +=
            //    (CefResourceRequestHandler handler, CefBrowser browser, CefFrame frame, CefRequest request, CefRequestCallback callback) =>
            //    {
            //        //Not able to capture websocket ? 
            //        string url = request.Url;
            //        if (url.StartsWith("devtools://"))
            //            return cef_return_value_t.RV_CONTINUE;
            //        CefWin.WriteDebugLine("OnBeforeResourceLoad:" + request.IsReadOnly + ":" + url);
            //        //request.SetHeaderByName("CefWinBrowserId", browser.Identifier.ToString(), true);
            //        //CefWin.WriteDebugLine("BrowserAgentId:" + request.GetHeaderByName("BrowserAgentId"));
            //        return cef_return_value_t.RV_CONTINUE;
            //    };
#endif

        }


    }
}
