using System;
using System.Collections.Generic;
using System.Text;

namespace CefLite.Interop
{
    static public class Extensions
    {

        static public IntPtr AsIntPtr<T>(this BasePointer<T> inst)
          where T : struct
        {
            return inst?.Ptr ?? IntPtr.Zero;
        }

        static public IntPtr EnsureIntPtr<T>(this BasePointer<T> inst, string argname)
            where T : struct
        {
            return inst?.Ptr ?? throw new ArgumentNullException(argname);
        }


        static public IntPtr AddRefReturnIntPtr<TS,TC>(this ObjectFromNet<TS,TC> inst)
            where TS : struct
            where TC : ObjectFromNet<TS, TC>
        {
            if (inst == null) return IntPtr.Zero;
            inst.AddRef();
            return inst.Ptr;
        }

    }
}
