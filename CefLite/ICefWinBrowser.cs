using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;

namespace CefLite
{
    public interface ICefWinBrowser
    {
        BrowserAgent Agent { get; }

        Form FindForm();

        event EventHandler Disposed;


    }
}
