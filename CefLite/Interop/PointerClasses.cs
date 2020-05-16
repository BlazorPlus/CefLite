using System;
using System.Collections.Concurrent;
using System.Linq;
using CefLite.Interop;
using System.Threading.Tasks;

using System.Runtime.InteropServices;


namespace CefLite.Interop
{

    public abstract class BasePointer<T>
        where T : struct
    {
        public IntPtr Ptr { get; protected set; }


        static public implicit operator IntPtr(BasePointer<T> obj)
        {
            return obj?.Ptr ?? IntPtr.Zero;
        }
    }


    public interface IFixedPointer { }

    /// <summary>
    /// Allocate simple struct in HGlobal , or from Native
    /// </summary>
    public unsafe abstract class FixedPointer<TS, TC> : BasePointer<TS>
        where TS : struct, IFixedPointer
        where TC : FixedPointer<TS, TC>
    {
        static readonly int T_SIZE = Marshal.SizeOf<TS>();

        public enum StructMemoryType
        {
            HGlobal
                ,
            Native
        }

        public Action<IntPtr> Recycle { get; private set; }

        public StructMemoryType MemoryType { get; private set; }

        protected FixedPointer(Action<IntPtr> recycleHandler)
        {
            MemoryType = StructMemoryType.HGlobal;
            Ptr = Marshal.AllocHGlobal(Marshal.SizeOf(typeof(TS)));
            WindowsInterop.RtlZeroMemory(Ptr, (UIntPtr)T_SIZE);
            Recycle = recycleHandler;
        }

        protected FixedPointer(IntPtr intptr) //for FromNative only
        {
            MemoryType = StructMemoryType.Native;
            Ptr = intptr;
            //Recycle = .. ; //Native data don't need recycle
        }

        ~FixedPointer()
        {
            if (MemoryType == StructMemoryType.HGlobal)
            {
                if (Recycle != null)
                    Recycle(Ptr);
                Marshal.FreeHGlobal(Ptr);
            }
            else
            {
                //do not do anything for Native data
            }
        }

    }

    public abstract unsafe class ObjectFromCef<TS, TC> : BasePointer<TS>
       where TS : struct
        where TC : ObjectFromCef<TS, TC>
    {
        static ConcurrentDictionary<IntPtr, WeakReference<ObjectFromCef<TS, TC>>> s_map = new ConcurrentDictionary<IntPtr, WeakReference<ObjectFromCef<TS, TC>>>();

        protected ObjectFromCef(IntPtr addr, bool addref = true)
        {
            Ptr = addr;
            if (addref)
                AddRef();
            s_map[Ptr] = new WeakReference<ObjectFromCef<TS, TC>>(this, true);
        }

        ~ObjectFromCef()
        {
            if (IsDisposed)
                return;
            s_map.TryRemove(Ptr, out var obj);
            //Console.WriteLine("---- Remove :" + typeof(TC).Name + " ");
            Release();
        }

        public bool IsDisposed { get; private set; }

        public virtual void Dispose()
        {
            if (IsDisposed)
                return;
            IsDisposed = true;
            s_map.TryRemove(Ptr, out var obj);
            //Console.WriteLine("---- Dispose :" + typeof(TC).Name + " ");
            Release();
        }

#if CACHEPTR
        static EventCallbackHandler _typed_add_ref;
#endif
        public void AddRef()
        {
            cef_base_ref_counted_t* pbrc = (cef_base_ref_counted_t*)Ptr;
#if CACHEPTR
            if(_typed_add_ref==null)
                _typed_add_ref = Marshal.GetDelegateForFunctionPointer<EventCallbackHandler>(pbrc->add_ref);

#else
            var _typed_add_ref = Marshal.GetDelegateForFunctionPointer<EventCallbackHandler>(pbrc->add_ref);
#endif
            _typed_add_ref(Ptr);
        }

#if CACHEPTR
        static GetInt32Handler _typed_release;
#endif
        public int Release()
        {
            cef_base_ref_counted_t* pbrc = (cef_base_ref_counted_t*)Ptr;
#if CACHEPTR
            if(_typed_release==null)
                _typed_release=Marshal.GetDelegateForFunctionPointer<GetInt32Handler>(pbrc->release);
#else
            var _typed_release = Marshal.GetDelegateForFunctionPointer<GetInt32Handler>(pbrc->release);
#endif
            return _typed_release(Ptr);
        }

        static public T2 FromNative<T2>(IntPtr ptr, Func<IntPtr, T2> ctor)
            where T2 : ObjectFromCef<TS, TC>
        {
            if (ptr == IntPtr.Zero) return null;
            if (s_map.TryGetValue(ptr, out var wr))
                if (wr.TryGetTarget(out var target))
                    return (T2)target;
            //if (typeof(T2) == typeof(CefBrowser)) CefWin.WriteDebugLine("CefBrowser FromNative : 0x" + ptr.ToString("X"));
            return ctor(ptr);
        }

    }



    public class DelegateHolder<T>
        where T : Delegate
    {
        static ConcurrentStack<object> holders = new ConcurrentStack<object>();

        static public void AddStatic(T method)
        {
            holders.Push(new DelegateHolder<T>(method ?? throw new ArgumentNullException(nameof(method))));
        }

        public DelegateHolder(T method)
        {
            DelegateInstance = method;
            FunctionPointer = Marshal.GetFunctionPointerForDelegate<T>(method);
        }

        public T DelegateInstance { get; }
        public IntPtr FunctionPointer { get; }

        static public implicit operator IntPtr(DelegateHolder<T> func)
        {
            return func.FunctionPointer;
        }
    }

    public unsafe abstract class ObjectFromNet<TS, TC> : BasePointer<TS>, IDisposable
        where TS : struct
        where TC : ObjectFromNet<TS, TC>
    {
        static readonly int T_SIZE = Marshal.SizeOf<TS>();
        static readonly string T_NAME = typeof(TS).Name;

        [System.Diagnostics.Conditional("DEBUG")]
        static void WriteDebugMsg(string msg)
        {
            //if (T_NAME == "cef_client_t") 
            //CefWin.WriteDebugLine(msg);   //TODO: cef_client_t may still using after shutdown() , check why
        }

        static ConcurrentDictionary<IntPtr, WeakReference<ObjectFromNet<TS, TC>>> s_map = new ConcurrentDictionary<IntPtr, WeakReference<ObjectFromNet<TS, TC>>>();

        static public TC GetInstance(IntPtr Ptr)
        {
            ObjectFromNet<TS, TC> inst;
            if (s_map.TryGetValue(Ptr, out var wr))
            {
                if (!s_map[Ptr].TryGetTarget(out inst))
                    throw new Exception(T_NAME + " recycled? 0x" + Ptr.ToString("X"));
            }
            else
            {
                throw new Exception(T_NAME + " recycled? 0x" + Ptr.ToString("X"));
            }
            return (TC)inst;
        }

        static DelegateHolder<EventCallbackHandler> holder_add_ref = new DelegateHolder<EventCallbackHandler>(ptr =>
        {
            GetInstance(ptr).AddRef();
        });
        static DelegateHolder<GetInt32Handler> holder_release = new DelegateHolder<GetInt32Handler>(ptr =>
        {
            return GetInstance(ptr).Release();
        });
        static DelegateHolder<GetInt32Handler> holder_has_one_ref = new DelegateHolder<GetInt32Handler>(ptr =>
        {
            return GetInstance(ptr)._refcount == 1 ? 1 : 0;
        });

        static DelegateHolder<GetInt32Handler> holder_has_at_least_one_ref = new DelegateHolder<GetInt32Handler>(ptr =>
        {
            return GetInstance(ptr)._refcount >= 1 ? 1 : 0;
        });

        volatile int _refcount = 1;
#if DEBUG
        bool _isremoved = false;
#endif

        public ObjectFromNet()
        {
            Ptr = Marshal.AllocHGlobal(T_SIZE);
            WindowsInterop.RtlZeroMemory(Ptr, (UIntPtr)T_SIZE);

            WriteDebugMsg("ObjectFromNet New: 0x" + Ptr.ToString("X"));
            s_map[Ptr] = new WeakReference<ObjectFromNet<TS, TC>>(this, true);

            cef_base_ref_counted_t* pbrc = (cef_base_ref_counted_t*)Ptr;
            pbrc->size = T_SIZE;
            pbrc->add_ref = holder_add_ref;
            pbrc->release = holder_release;
            pbrc->has_one_ref = holder_has_one_ref;
            pbrc->has_at_least_one_ref = holder_has_at_least_one_ref;
        }

        ~ObjectFromNet()
        {
            if (_refcount == 1)//&& !CefWin._CefShutdownCalled
            {
#if DEBUG
                _isremoved = true;
#endif

                Marshal.FreeHGlobal(Ptr);
                s_map.TryRemove(Ptr, out var obj);
                WriteDebugMsg("ObjectFromNet Remove: " + T_NAME + " 0x" + Ptr.ToString("X"));
                return;
            }
            WriteDebugMsg("ObjectFromNet ReRegisterForFinalize: " + T_NAME + " : " + _refcount + " 0x" + Ptr.ToString("X"));
            GC.ReRegisterForFinalize(this);
        }

        public void AddRef()
        {
            WriteDebugMsg("AddRef: " + _refcount + " 0x" + Ptr.ToString("X"));
#if DEBUG
            if (_isremoved)
            {
                Console.WriteLine("Warning : object has been collected? " + T_NAME);
            }
#endif
            System.Diagnostics.Debug.Assert(_refcount != 0);
            System.Threading.Interlocked.Increment(ref _refcount);
        }
        public int Release()
        {
            WriteDebugMsg("Release: " + _refcount + " 0x" + Ptr.ToString("X"));
            System.Threading.Interlocked.Decrement(ref _refcount);
            System.Diagnostics.Debug.Assert(_refcount != 0);
            return 0; //shall not down to 0,  //return _refcount == 0 ? 1 : 0;
        }


        public bool IsDisposed { get; private set; }

        public virtual void Dispose()
        {
            if (IsDisposed)
                return;
            IsDisposed = true;
            Release();
        }

    }

}

