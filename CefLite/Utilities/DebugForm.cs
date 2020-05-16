using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace CefLite.Utilities
{
    public partial class DebugForm : Form
    {
        public DebugForm()
        {
            InitializeComponent();

            Button btn1 = new Button() { Left = 20, Top = 20, Text = "Error" };
            btn1.Click += delegate
            {
                throw new Exception("button error.");
            };

            this.Controls.Add(btn1);
        }
    }
}
