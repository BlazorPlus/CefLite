using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace CefLite
{
    public partial class DefaultDownloadForm : Form
    {
        public DefaultDownloadForm()
        {
            InitializeComponent();

            if (CefWin.ApplicationIcon != null)
                this.Icon = CefWin.ApplicationIcon;

            this.MinimumSize = new Size(360, 360);
        }
    }
}
