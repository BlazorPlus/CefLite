using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace CefLite
{
    public partial class SplashForm : Form
    {
        public SplashForm()
        {
            InitializeComponent();

            Cursor = Cursors.WaitCursor;

            this.StartPosition = FormStartPosition.CenterScreen;
            this.TopMost = true;
            this.ShowInTaskbar = false;
            this.FormBorderStyle = FormBorderStyle.None;
        }

        static public SplashForm Show(string photofile)
        {
            Image img = null;
            try
            {
                var data = System.IO.File.ReadAllBytes(photofile);
                img = Image.FromStream(new System.IO.MemoryStream(data));
            }
            catch (Exception x)
            {
                CefWin.WriteDebugLine(x);
                CefWin.WriteDebugLine("Failed to load " + photofile);
                return null;
            }
            return Show(img);
        }

        static public SplashForm Show(Image img)
        {
            SplashForm form = new SplashForm();
            form.Width = img.Width;
            form.Height = img.Height;

            PictureBox pb = new PictureBox();
            pb.Image = img;
            pb.Dock = DockStyle.Fill;
            form.Controls.Add(pb);

            form.Show();

            return form;
        }
    }
}
