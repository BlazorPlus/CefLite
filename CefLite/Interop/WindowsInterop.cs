using System;
using System.Collections.Generic;
using System.Linq;
using CefLite.Interop;
using System.Threading.Tasks;

using System.Runtime.InteropServices;


namespace CefLite.Interop
{

    public class WindowsInterop
    {
        [DllImport("kernel32.dll", SetLastError = true)]
        static public extern IntPtr LoadLibraryEx(string fullpath, IntPtr reserved, uint flags);

        [DllImport("kernel32.dll")]
        static public extern IntPtr GetModuleHandle(string module);

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        static public extern bool SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter, int X, int Y, int cx, int cy, uint uFlags);

        [DllImport("User32.dll")]
        static public extern Int32 SetForegroundWindow(IntPtr hWnd);

        [DllImport("kernel32.dll")]
        static public extern void RtlZeroMemory(IntPtr dst, UIntPtr length);
    }
}