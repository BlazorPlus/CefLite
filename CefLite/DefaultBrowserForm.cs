using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using WF = System.Windows.Forms;
using CefLite.Interop;

namespace CefLite
{
    public partial class DefaultBrowserForm : Form, ICefWinBrowser
    {

        BrowserAgent _browserAgent;
        public BrowserAgent Agent => _browserAgent;

        public DefaultBrowserForm(string defaulturl)
        {
            InitializeComponent();

            if (CefWin.ApplicationIcon != null)
                this.Icon = CefWin.ApplicationIcon;

            this.Text = CefWin.ApplicationTitle;

            this.MinimumSize = new Size(360, 360);

            if (Application.OpenForms.Count - (CefWin._splashForm?.Visible == true ? 1 : 0) == 0)
                this.StartPosition = FormStartPosition.CenterScreen;

            this.Disposed += DefaultBrowserForm_Disposed;

            this.Resize += DefaultBrowserForm_Resize;

            _browserAgent = new BrowserAgent();
            _browserAgent.Url = defaulturl;

            if (CefWin._splashForm != null)
            {
                this.Opacity = 0;
                //this.ShowInTaskbar = false;   //TODO:this will affect the Forms collection , handle it later
            }


            CefWin.PostToAppThread(delegate
            {
                BeforeLoad?.Invoke(this, EventArgs.Empty);
                CefWin.WriteDebugLine("BeforeLoad Invoked.");
            });


        }

        static public event EventHandler BeforeLoad;
        static public event EventHandler AfterDisposed;

        protected override void OnLoad(EventArgs e)
        {
            Agent.Client.LoadHandler.LoadEnd += _LoadEnd;
            base.OnLoad(e); // Show(){ OnLoad(){ InitAsChild() } }
            _browserAgent.InitAsChild(this.Handle, this.ClientRectangle);
            _browserAgent.WaitBrowserReady();
            CefWin.Register(this);
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            base.OnFormClosing(e);
            switch(e.CloseReason)
            {
                case CloseReason.ApplicationExitCall:
                case CloseReason.WindowsShutDown:
                    this.Dispose();
                    break;
            }
        }

        /// <summary>
        /// Default value 0.2
        /// </summary>
        static public double OpacityShowStepValue { get; set; } = 0.2;

        void _LoadEnd(CefLoadHandler handler, CefBrowser browser, CefFrame frame, int status)
        {
            this.Agent.Client.LoadHandler.LoadEnd -= _LoadEnd;
            CefWin.WriteDebugLine("LoadEnd:" + browser.Identifier + ":" + frame.Identifier + ":" + status);

            CefWin.CloseSplashScreen();

            if (this.Opacity < 1)
            {
                //this.ShowInTaskbar = true;
                WF.Timer timer = new WF.Timer();
                timer.Interval = 20;
                timer.Tick += delegate
                {
                    this.Opacity += OpacityShowStepValue;
                    //WriteDebugLine("Opacity..." + form.Opacity);
                    if (this.Opacity >= 1)
                    {
                        timer.Dispose();
                    }
                };
                timer.Start();
            }

        }

        private void DefaultBrowserForm_Disposed(object sender, EventArgs e)
        {
            _browserAgent.Dispose();
            CefWin.NotifyFormDisposed();
            AfterDisposed?.Invoke(this, e);
        }

        private void DefaultBrowserForm_Resize(object sender, EventArgs e)
        {
            _browserAgent.OnResize(this.ClientRectangle);
        }



    }
}
