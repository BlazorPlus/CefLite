using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using System.Threading.Tasks;
using System.Diagnostics;
using System.IO;
using CefLite.Interop;
using System.Threading;
using WF = System.Windows.Forms;
using System.Reflection;

namespace CefLite
{
	public class CefWin
	{
		static public string[] ProgramArgs { get; set; } = Environment.GetCommandLineArgs().Skip(1).ToArray();

		static public Process CurrentProcess { get; } = Process.GetCurrentProcess();

		static public string ApplicationName { get; set; } = Process.GetCurrentProcess().ProcessName;

		static public string ApplicationTitle { get; set; } = nameof(ApplicationTitle);

		static public DateTime ApplicationStartTime { get; } = DateTime.Now;
		static public TimeSpan ApplicationElapsed => DateTime.Now - ApplicationStartTime;


		static public IDisposable ApplicationHost { get; set; }

		static public Task ApplicationTask { get; set; }

		/// <summary>
		/// Only use it for application level logic.
		/// </summary>
		static public CancellationTokenSource ApplicationCTS { get; } = new CancellationTokenSource();

		static public System.Drawing.Icon ApplicationIcon { get; set; }

		static public bool PrintDebugInformation { get; set; }
		static public Action<string> PrintDebugHandler { get; set; }

		static public Func<string, string, string> TranslateHandler { get; set; }

		static public string TranslateString(string key, string text) => TranslateHandler?.Invoke(key, text) ?? text;

		static public List<string> SearchLibCefRootFolderList { get; } = new List<string>();
		static public List<string> SearchLibCefSubPathList { get; } = new List<string>();



		/// <summary>
		/// define more arguments for CEF
		/// default include "--disable-gpu-shader-disk-cache"
		/// see https://peter.sh/experiments/chromium-command-line-switches/ for more
		/// </summary>
		static public List<string> CefAdditionArguments { get; } = new List<string>();


		/// <summary>
		/// Store the first form for OpenBrowser() method
		/// If you are developing multiple forms app , maintain it by yourself , or do not use it .
		/// </summary>
		static public ICefWinBrowser MainBrowser { get; set; }

		static public bool IsWindowsEventLoopQuitRequested { get; private set; }


		static public SynchronizationContext MainThreadSynchronizationContext { get; private set; }

#if SUPPORT_WPF
		static System.Windows.Application _wpfapp;
		static public System.Windows.Application WpfApp
		{
		    get
		    {
		        if (_wpfapp == null)
		            _wpfapp = new System.Windows.Application();
		        return _wpfapp;
		    }
		    set
		    {
		        _wpfapp = value;
		    }
		}
#endif

		static bool _winformsExitFired = false;
		static bool _libcefEverLoaded = false;
		static HashSet<string> _optionset = new HashSet<string>();

		static List<object> _foreverObjs = new List<object>();
		static public void AddStaticObject(object obj)
		{
			_foreverObjs.Add(obj ?? new ArgumentNullException(nameof(obj)));
		}

		static CefWin()
		{

			string exefile = CurrentProcess.MainModule.FileName;

			ApplicationIcon = System.Drawing.Icon.ExtractAssociatedIcon(exefile);

			SearchLibCefRootFolderList.Add(Path.GetDirectoryName(exefile));
			SearchLibCefRootFolderList.Add(Environment.CurrentDirectory);

			// * * single-process is not recommanded by chromium team , If you get any strange issue , try removing this option: 
			// * * see https://peter.sh/experiments/chromium-command-line-switches/ for more 
			// * * ERROR msg in https://github.com/chromium/chromium/blob/bd61bd83c9ebcfdb162a447e4a01637d259ea358/chrome/browser/net/system_network_context_manager.cc
			CefAdditionArguments.Add("--single-process");    //use single process , memory usage is 170MB , otherwise 340MB
			CefAdditionArguments.Add("--winhttp-proxy-resolver");
			CefAdditionArguments.Add("--no-proxy-server");  //https://code.google.com/archive/p/chromiumembedded/issues/81

			CefAdditionArguments.Add("--disable-gpu-shader-disk-cache");
			CefAdditionArguments.Add("--allow-running-insecure-content");

			CefAdditionArguments.Add("--enable-speech-input");
			CefAdditionArguments.Add("--enable-media-stream");//TODO: ??? must work with --enable-usermedia-screen-capturing
			CefAdditionArguments.Add("--enable-usermedia-screen-capturing");

			CefAdditionArguments.Add("--enable-aggressive-domstorage-flushing");    //save data more quickly

			// more tested :
			//  --proxy-server=http://127.0.0.1:8080


			new WF.Panel().Handle.ToString();   //Activate  WindowsFormsSynchronizationContext
			MainThreadSynchronizationContext = SynchronizationContext.Current;


			Console.CancelKeyPress += delegate
			{
				//CTRL+C
				WriteDebugLine("Console.CancelKeyPress");
				ApplicationCTS.Cancel();
			};
			WF.Application.ApplicationExit += delegate
			{
				_winformsExitFired = true;
				WriteDebugLine("Warning:do not call Application.Exit()");
				ApplicationCTS.Cancel();
			};



			LibCefInterop.App.RenderProcessHandler.WebKitInitialized += delegate
			{
				CefV8Handler handler = new CefV8Handler();
				AddStaticObject(handler);

				ConcurrentDictionary<string, Assembly> dllmap = new ConcurrentDictionary<string, Assembly>();

				Assembly[] existAsms = null;

				object HandleExecute(CefV8Handler self, string name, CefV8Value[] args)
				{
					if (SettingsTrustedWebsiteHosts == null)
						throw new Exception("TrustedWebsiteHosts is not configured");

					//TODO: v8c get current , get browser
					CefV8Context ctx = CefV8Context.GetCurrent();
					if (ctx == null)
						throw new Exception("no current v8context");
					string url = ctx.GetFrame().Url;
					Uri uri = new Uri(url);
					if (!SettingsTrustedWebsiteHosts.Contains(uri.Host))
					{
						if (!SettingsTrustedWebsiteHosts.Contains(uri.Host + ":" + uri.Port))
						{
							throw (new Exception(uri.Host + ":" + uri.Port + " is not trusted in TrustedWebsiteHosts"));
						}
					}

					if (existAsms == null)
						existAsms = AppDomain.CurrentDomain.GetAssemblies();

					if (name == "_compileAssembly")
					{
						string dllname = args[0].GetStringValue();
						string code = args[1].GetStringValue();

						WriteDebugLine("_compileAssembly:" + dllname + ":" + code.Length);

						if (!SettingAllowCompileCSharpCode)
							throw new Exception("AllowCompileCSharpCode is not setted to true");

						if (dllname.IndexOfAny(System.IO.Path.GetInvalidFileNameChars()) != -1)
							throw new Exception("Invalid dllname.");

						//System.CodeDom.Compiler.CodeCompiler cc= System.CodeDom.Compiler.CodeCompiler
						var cdp = System.CodeDom.Compiler.CodeDomProvider.CreateProvider("cs");
						var cp = new System.CodeDom.Compiler.CompilerParameters();
						foreach (Assembly asm in existAsms)
							cp.ReferencedAssemblies.Add(asm.Location);
						cp.GenerateExecutable = false;
						cp.GenerateInMemory = true;
						cp.IncludeDebugInformation = true;
						cp.OutputAssembly = dllname;

						var cr = cdp.CompileAssemblyFromSource(cp, code);
						foreach (System.CodeDom.Compiler.CompilerError err in cr.Errors)
						{
							string msg = err.ToString();
							if (msg.Contains("System.Runtime.CompilerServices.ExtensionAttribute"))
							{
								//ignore
							}
							else
							{
								WriteDebugLine(err.ToString());
							}
							if (err.IsWarning)
								continue;
							foreach (var asm in existAsms)
							{
								Console.WriteLine(asm.FullName);
							}
							throw new Exception(err.ToString());
						}

						WriteDebugLine("new assembly compiled : " + cr.CompiledAssembly.FullName);

						dllmap[dllname] = cr.CompiledAssembly;
					}
					else if (name == "_installAssembly")
					{
						string dllname = args[0].GetStringValue();
						string base64 = args[1].GetStringValue();

						WriteDebugLine("_installAssembly:" + dllname + ":" + base64.Length);

						if (SettingTrustedPublicKeyTokens == null)
							throw new Exception("CefWin.PublicKeyTokenList is not configured");

						byte[] data = Convert.FromBase64String(base64);
						Assembly asm = AppDomain.CurrentDomain.Load(data);
						byte[] tokendata = asm.GetName().GetPublicKeyToken();
						if (tokendata == null)
							throw new Exception("the dll " + dllname + " must signed for a PublicKeyToken and register it into CefWin.PublicKeyTokenList ");
						string token = BitConverter.ToString(tokendata).Replace("-", "").ToLower();
						if (!SettingTrustedPublicKeyTokens.Contains(token))
							throw new Exception("Invalid dll , " + token + " is not in CefWin.PublicKeyTokenList");

						WriteDebugLine("new assembly installed : " + asm.FullName);

						dllmap[dllname] = asm;
					}
					else if (name == "_executeAssembly")
					{
						string dllname = args[0].GetStringValue();
						string clsname = args[1].GetStringValue();
						string method = args[2].GetStringValue();

						WriteDebugLine("_executeAssembly:" + dllname + ":" + clsname + ":" + method);

						object[] methodargs = (object[])args[3].ToObject() ?? Array.Empty<object>();
						Assembly asm;
						if (!dllmap.TryGetValue(dllname, out asm))
							throw new Exception(dllname + " not exists");
						Type type = asm.GetType(clsname);

						WriteDebugLine("Type : " + type.AssemblyQualifiedName);

						var members = type.GetMember(method);
						if (members.Length == 0)
							throw new Exception("Method " + method + " not exists in " + type.AssemblyQualifiedName);
						if (members.Length != 1)
							throw new Exception("Too many member " + method + " in " + type.AssemblyQualifiedName);

						var minfo = members[0] as MethodInfo;
						if (minfo == null)
							throw new Exception("Member " + method + " is not a method in " + type.AssemblyQualifiedName);
						if (!minfo.IsStatic)
							throw new Exception("Method " + method + " is not static in " + type.AssemblyQualifiedName);
						if (!minfo.IsPublic)
							throw new Exception("Method " + method + " is not public in " + type.AssemblyQualifiedName);


						object result = null;

						InvokeInAppThread(delegate
						{
							result = minfo.Invoke(null, new object[] { methodargs });
						});

					}
					else
					{
						throw new Exception("Invalid func " + name);
					}
					return null;
				}
				handler.Execute = HandleExecute;

				//TODO: shall use async return Promise ... now show dialog will block the JS executing, 
				//TODO: Support multi-process : 

				int r = CefV8Context.CefRegisterExtension("cefwin_extention", @"
var CefWin={};
(function(){
    var dllmap={};
    CefWin.compileAssembly=function(dllname,code)
    {
        var dllitem=dllmap[dllname];
        if(dllitem!=null&&dllitem.code==code)
        {
            console.log(dllname+' already compiled');
            return;
        }
        console.log(dllname,code.length);
        dllitem={code:code};
        native function _compileAssembly();
        _compileAssembly(dllname, code);
        dllmap[dllname]=dllitem;
    }
    CefWin.installAssembly=function(dllname, base64)
    {
        var dllitem=dllmap[dllname];
        if(dllitem!=null&&dllitem.base64==base64)
        {
            console.log(dllname+' already installed');
            return;
        }
        console.log(dllname,base64.length);
        dllitem={base64:base64};
        native function _installAssembly();
        _installAssembly(dllname, base64);
        dllmap[dllname]=dllitem;
    }
    CefWin.executeAssembly=async function _ExecuteAssembly(dllname, clsname, method, args)
    {
        var dllitem=dllmap[dllname];
        if(!dllitem)throw new Error(dllname+' not installed');
        native function _executeAssembly();
        console.log(dllname,clsname,method);
        _executeAssembly(dllname,clsname,method,args);
        return 'OK'+new Date().getTime();
    }
})();
", handler);
				WriteDebugLine("CefRegisterExtension:" + r);
			};


			//TODO: implement the Notification API
			//LibCefInterop.App.RenderProcessHandler.WebKitInitialized += delegate
			//  {
			//      int r = CefV8Context.CefRegisterExtension("CefWin1", "function cefwin(){alert(123);}");
			//      WriteDebugLine("CefRegisterExtension:" + r);
			//  };
			//LibCefInterop.App.RenderProcessHandler.ContextCreated += (rph, browser, frame, v8c) =>
			//  {
			//      WriteDebugLine("OnContextCreated id:" + browser.Identifier);
			//      WriteDebugLine("send msgfromrenderer");
			//      frame.SendProcessMessage(cef_process_id_t.PID_BROWSER, "msgfromrenderer");
			//      try
			//      {
			//          v8c.Eval("console.log('hello...')");
			//          WriteDebugLine("Eval DONE");
			//      }
			//      catch (Exception x)
			//      {
			//          WriteDebugLine(x);
			//      }
			//  };

		}


		static internal void WriteDebugLine(object msg)
		{
			if (!PrintDebugInformation)
				return;
			string line = "CefWin #" + CurrentProcess.Id + " : " + msg;
			if (PrintDebugHandler != null)
				PrintDebugHandler(line);
			else
				Console.WriteLine(line);
		}

		static public string MakeRandomLocalHostUrl()
		{
			return MakeFixedLocalHostUrl(Guid.NewGuid().ToString());
		}
		static public string MakeFixedLocalHostUrl(string seedstr = null)
		{
			if (seedstr == null)
			{
				seedstr = CurrentProcess.ProcessName;
			}
			seedstr += ":" + System.Security.Principal.WindowsIdentity.GetCurrent().Name;
			WriteDebugLine("MakeFixedLocalHostUrl:" + seedstr);
			int seed = 0;
			foreach (char c in seedstr)
				seed = seed * 4567 + c;
			WriteDebugLine("Random seed:" + seed);
			Random r = new Random(seed);
			return "http://127." + r.Next(1, 253) + "." + r.Next(1, 253) + "." + r.Next(1, 253) + ":" + r.Next(20001, 40000);
		}

		static public bool DetectIsMainProcess()
		{
			string cmdargs = string.Join(" ", ProgramArgs);
			bool isSubProc = cmdargs.Contains("--type=");
			return !isSubProc;
		}


		static internal WF.Form _splashForm;
		static public void ShowSplashScreen(string photoFile)
		{
			if (!DetectIsMainProcess())
				return;
			string selectedFullpath = null;
			SearchFile(photoFile, ref selectedFullpath);
			if (selectedFullpath == null) return;
			_splashForm = SplashForm.Show(selectedFullpath);
			if (_splashForm == null)
				return;
			WF.Application.DoEvents();
			WF.Application.DoEvents();
		}
		static public void ShowSplashScreen(System.Drawing.Image img)
		{
			if (!DetectIsMainProcess())
				return;
			_splashForm = SplashForm.Show(img ?? throw new ArgumentNullException(nameof(img)));
			if (_splashForm == null)
				return;
			WF.Application.DoEvents();
			WF.Application.DoEvents();
		}
		static public void CloseSplashScreen()
		{
			if (_splashForm != null)
			{
				_splashForm.Close();
				_splashForm = null;
			}
		}

		/// <summary>
		/// Tool method : search libcef.dll , and load it , and initialize it , the dll and resources files shall be in same folder
		/// </summary>
		/// <returns>Initialized : CEF ready , SubProcess : cef sub process executed , program shall quit. Failed : something wrong and CEF is not ready</returns>
		static public CefInitState SearchAndInitialize()
		{
			WriteDebugLine("StartTime:" + ApplicationStartTime.ToString("HH:mm:ss.fff"));

			string selectedFullpath = null;
			IEnumerable<string> folders = SearchFile("libcef.dll", ref selectedFullpath);
			if (selectedFullpath == null)
			{
				if (PrintDebugInformation)
				{
					WriteDebugLine("Unable to find 'libcef.dll' , Searched folders : ");
					foreach (string eachFolder in folders.Distinct())
						WriteDebugLine("\t" + eachFolder);
				}
				CloseSplashScreen();
				return CefInitState.Failed;
			}
			if (!InternalLoadLibCefDll(selectedFullpath))
			{
				if (PrintDebugInformation)
				{
					int errcode = System.Runtime.InteropServices.Marshal.GetLastWin32Error();
					WriteDebugLine("Unable to load 'libcef.dll' via LoadLibraryEx : err:0x" + errcode.ToString("X").PadLeft(8, '0'));
				}
				CloseSplashScreen();
				return CefInitState.Failed;
			}

			return InternalCefInitialize(Path.GetDirectoryName(selectedFullpath));
		}

		private static IEnumerable<string> SearchFile(string filetosearch, ref string selectedFullpath)
		{
			string selectedFolder = null;
			IEnumerable<string> folders = SearchLibCefRootFolderList;
			if (SearchLibCefSubPathList.Count != 0)
			{
				folders = folders.SelectMany(folder =>
					SearchLibCefSubPathList.Union(new string[] { "" }).Select(part => Path.GetFullPath(Path.Combine(folder, part)))
				);
			}
			foreach (string eachFolder in folders.Distinct())
			{
				string cefFullpath = Path.Combine(eachFolder, filetosearch);
				if (!File.Exists(cefFullpath))
					continue;
				selectedFolder = eachFolder;
				selectedFullpath = cefFullpath;
				break;
			}

			return folders;
		}




		#region ActivateExistingApp

		/// <summary>
		/// Tool method : If another app is running , return true , program shall exit.
		/// </summary>
		/// <returns></returns>
		static public bool ActivateExistingApp(string mutexname = null)
		{
			if (!DetectIsMainProcess())
				return false;
			if (string.IsNullOrWhiteSpace(mutexname))
				mutexname = "mutex_" + CurrentProcess.MainModule.FileName;
			mutexname += ":" + System.Security.Principal.WindowsIdentity.GetCurrent().Name;
			mutexname = mutexname.Replace(":", "_").Replace("\\", "").Replace("/", "");
			string npname = "nps_" + mutexname;
			bool isNew;
			_app_mutex = new Mutex(true, mutexname, out isNew);
			if (!isNew)
			{
				ActivateAnotherInstance(npname);
				return true;//yes another app is running
			}
			else
			{
				Task.Run(async delegate
				{
					await StartServerActivatorAsync(npname);
				});
				ApplicationCTS.Token.Register(delegate
				{
					if (!_app_mutex_disposed)
					{
						_app_mutex_disposed = true;
						_app_mutex.Dispose();
						WriteDebugLine("_app_mutex disposed 1.");
					}
				});
				return false;
			}
		}

		static Mutex _app_mutex;
		static bool _app_mutex_disposed;

		static void ActivateAnotherInstance(string npname)
		{
			using (var nps = new System.IO.Pipes.NamedPipeClientStream(".", npname
					, System.IO.Pipes.PipeDirection.InOut, System.IO.Pipes.PipeOptions.Asynchronous | System.IO.Pipes.PipeOptions.WriteThrough
					, System.Security.Principal.TokenImpersonationLevel.Anonymous))
			{
				MemoryStream ms = new MemoryStream();
				try
				{
					nps.Connect(2000);

					byte[] buffer = new byte[1024];
					while (nps.IsConnected)
					{
						int rc;
						try
						{
							rc = nps.Read(buffer, 0, buffer.Length);
						}
						catch (IOException)
						{
							break;
						}
						if (rc == 0)
						{
							Thread.Sleep(10);
							continue;
						}
						WriteDebugLine("read data size " + rc);
						if (rc != 0) ms.Write(buffer, 0, rc);
					}

					nps.Close();
				}
				catch (Exception x)
				{
					WriteDebugLine(x);
				}
				string msg = System.Text.Encoding.ASCII.GetString(ms.ToArray());
				WriteDebugLine("ActivateAnotherInstance " + msg);
			}
		}

		static async Task StartServerActivatorAsync(string npname)
		{
			await Task.Delay(1);
			while (!ApplicationCTS.IsCancellationRequested)
			{
				bool everConnected = false;
				try
				{
					using (var nps = new System.IO.Pipes.NamedPipeServerStream(npname, System.IO.Pipes.PipeDirection.InOut
						, 1, System.IO.Pipes.PipeTransmissionMode.Byte
						, System.IO.Pipes.PipeOptions.Asynchronous | System.IO.Pipes.PipeOptions.WriteThrough, 65536, 65536))
					{
						await nps.WaitForConnectionAsync(ApplicationCTS.Token);
						everConnected = true;
						byte[] msg = System.Text.Encoding.ASCII.GetBytes(CurrentProcess.Id.ToString());
						await nps.WriteAsync(msg);
						await nps.FlushAsync();
						//nps.Disconnect(); //do not disconnect
						nps.Close();
					}
				}
				catch (Exception x)
				{
					if (ApplicationCTS.IsCancellationRequested)
						return;
					WriteDebugLine(x);
					if (!everConnected)
					{
						await Task.Delay(1000);
						continue;
					}
				}
				PostToAppThread(delegate
				{
					WriteDebugLine("ServerActivator");

					var mf = MainBrowser?.FindForm();
					if (mf?.Visible == true)
					{
						ActivateForm(mf);
						return;
					}
					var forms = WF.Application.OpenForms;
					for (var i = 0; i < forms.Count; i++)
					{
						var form = forms[i];
						if (!form.Visible)
							continue;

						//WindowsInterop.SetForegroundWindow(form.Handle);
						ActivateForm(form);
						break;
					}
				});
			}
		}

		#endregion


		#region Open Find Register Browser

		static public DefaultBrowserForm OpenBrowser(string url)
		{
			if (url == null) throw new ArgumentNullException(nameof(url));

			DefaultBrowserForm form = null;

			InvokeInAppThread(delegate
			{
				form = new DefaultBrowserForm(url);
				if (MainBrowser == null) MainBrowser = form;
				WriteDebugLine("OpenBrowser Navigating to " + url);
			});

			PostToAppThread(delegate
			{
				WriteDebugLine("OpenBrowser Show Start");
				form.Show();
				WriteDebugLine("OpenBrowser Show End");
			});

			return form;
		}
		static public ICefWinBrowser FindBrowser(long id)
		{
			ICefWinBrowser val;
			if (_browsers.TryGetValue(id, out val))
				return val;
			return null;
		}

		static ConcurrentDictionary<long, ICefWinBrowser> _browsers = new ConcurrentDictionary<long, ICefWinBrowser>();

		static public void Register(ICefWinBrowser browser)
		{
			if (browser == null) throw new ArgumentNullException(nameof(browser));
			long id = browser.Agent.Browser.Identifier;
			if (_browsers.ContainsKey(id))
			{
				WriteDebugLine("Warning,call Register again, id:" + id);
				return;
			}
			_browsers[id] = browser;
			browser.Disposed += delegate (object sender, EventArgs e)
			{
				long rid = ((ICefWinBrowser)sender).Agent.Browser.Identifier;
				_browsers.TryRemove(rid, out var value);
			};
		}

		#endregion


		#region RunApplication

		/// <summary>
		/// Tool method : Start the windows message loop , and monitor status , close all when any of them stopped.
		/// </summary>
		static public void RunApplication(Action initHandler = null)
		{
			WriteDebugLine("RunApplication : " + ApplicationElapsed);

			bool mainLoopExited = false;

			var aspnetcoretask = ApplicationTask ?? Task.Delay(-1, ApplicationCTS.Token);

			ApplicationCTS.Token.Register(delegate
			{
				if (!aspnetcoretask.IsCompleted && !mainLoopExited && !IsWindowsEventLoopQuitRequested)
				{
					WriteDebugLine("ApplicationCTS Requested. Waiting ApplicationTask shutdown..");
				}
			});

			aspnetcoretask.ContinueWith(delegate
			{
				ApplicationHost?.Dispose();

				if (!mainLoopExited && !IsWindowsEventLoopQuitRequested)
				{
					WriteDebugLine("ApplicationTask Exited. Now shutdown CefWin");
					QuitWindowsEventLoop();
				}
			});

			////            START Windows Event Loop

			WriteDebugLine("Start WindowsEventLoop()");

			InternalDoEventLoop(initHandler);

			mainLoopExited = true;

			////            RELEASE and Exiting

			if (!aspnetcoretask.IsCompleted)
			{
				WriteDebugLine("CefWin Exited. Now stop ApplicationCTS.");
				ApplicationCTS.CancelAfter(1);
			}

			ApplicationHost?.Dispose();

			//Thread.Sleep(1000);//Debug wait..

			if (!aspnetcoretask.IsCompleted && !_winformsExitFired)
			{
				WriteDebugLine("Exiting....Wait for ApplicationTask");
				int loopcount = 0;
				DateTime dts = DateTime.Now;
				while (!aspnetcoretask.IsCompleted)
				{
					DoMessageLoopOnce();
					loopcount++;
					Thread.Sleep(10);
					if (DateTime.Now - dts > TimeSpan.FromSeconds(1.5))
					{
						WriteDebugLine("timeout..");
						break;
					}
				}

				if (!aspnetcoretask.IsCompleted)
				{
					WriteDebugLine(".....");
					//CurrentProcess.Kill();
				}
				else
				{
					WriteDebugLine("ApplicationTask Exited....final loops : " + loopcount);
				}
			}
			else if (!aspnetcoretask.IsCompleted)
			{
				WriteDebugLine("Will not wait ApplicationTask after Application.Exit called.");
			}

			for (int i = 0; i < 5; i++)
			{
				DoMessageLoopOnce();
			}

			CefShutdown();

			WriteDebugLine("RunApplication END");

		}

		#endregion


		#region Message Loop

		static bool _cefmessagelooprunning = false;
		static void InvokeCefMessageLoopOnce()
		{
			if (_cefmessagelooprunning)
				return;
			_cefmessagelooprunning = true;
			LibCefInterop.cef_do_message_loop_work();
			_cefmessagelooprunning = false;
		}
		static public void DoMessageLoopOnce()
		{
			if (_libcefEverLoaded)
				InvokeCefMessageLoopOnce();
			WF.Application.DoEvents();
		}

		private enum EventLoopMode
		{
			CodeLoop
				,
			LibCef
				,
			WinForms
#if SUPPORT_WPF
				,
			WPF
#endif
		}
		static private EventLoopMode LoopMode { get; set; } = EventLoopMode.CodeLoop;    //No plan to make publish

		static public void InternalDoEventLoop(Action initHandler)
		{
			if (initHandler != null)
				PostToAppThread(initHandler);

			long tickcount = 0;
			WF.Timer globaltimer = new WF.Timer();
			globaltimer.Interval = 50;
			globaltimer.Tick += delegate
			{
				//for Form.ShowDialog(..)
				DoMessageLoopOnce();

				tickcount++;
				if (tickcount % 50 == 0 && WF.Application.OpenForms.Count == 0
#if SUPPORT_WPF
                && (_wpfapp == null || _wpfapp.Windows.Count == 0)
#endif
				)
				{
					QuitWindowsEventLoop();
					WriteDebugLine("QuitWindowsEventLoop for no Forms/Window");
				}
			};
			globaltimer.Start();

			switch (LoopMode)
			{
				case EventLoopMode.LibCef:
					WriteDebugLine("cef_run_message_loop() start.");
					LibCefInterop.cef_run_message_loop();
					WriteDebugLine("cef_run_message_loop() end.");
					break;
#if SUPPORT_WPF
				case EventLoopMode.WPF:
					WpfApp.Run();
					break;
#endif
				case EventLoopMode.WinForms:
					WF.Application.Idle += delegate
					{
						InvokeCefMessageLoopOnce();
					};
					WF.Application.Run();
					break;
				case EventLoopMode.CodeLoop:
				default:
					while (!IsWindowsEventLoopQuitRequested)
					{
						InvokeCefMessageLoopOnce();
						WF.Application.DoEvents();
						System.Threading.Thread.Sleep(1);
					}
					break;
			}

			globaltimer.Stop();

			WriteDebugLine("Existing event loop start");

			if (_app_mutex != null)
			{
				if (!_app_mutex_disposed)
				{
					_app_mutex_disposed = true;
					_app_mutex.Dispose();
					WriteDebugLine("_app_mutex disposed 2.");
				}
			}

			foreach (WF.Form form in new System.Collections.ArrayList(WF.Application.OpenForms))
			{
				WriteDebugLine("Close Form [" + form.Text + "]");
				//form.Close();
				form.Dispose();
			}
#if SUPPORT_WPF
            if (_wpfapp != null)
            {
                WriteDebugLine("Check wpf windows..");
                foreach (System.Windows.Window win in _wpfapp.Windows)
                {
                    win.Close();
                }
            }
#endif

			WriteDebugLine("Existing event loop end");
		}

		static public void QuitWindowsEventLoop()
		{
			if (IsWindowsEventLoopQuitRequested)
				return;
			MainThreadSynchronizationContext.Send(delegate
			{
				if (IsWindowsEventLoopQuitRequested)
					return;
				IsWindowsEventLoopQuitRequested = true;
#if SUPPORT_WPF
				if (LoopMode == EventLoopMode.WPF) _wpfapp?.Shutdown();
#endif
				if (LoopMode == EventLoopMode.LibCef) LibCefInterop.cef_quit_message_loop();
				if (LoopMode == EventLoopMode.WinForms) WF.Application.Exit();  //This is not a good solution, hang
			}, null);
		}

		static public void SetTimeout(int ms, Action handler)
		{
			if (handler == null) throw new ArgumentNullException(nameof(handler));
			if (ms <= 0)
			{
				PostToAppThread(handler);
				return;
			}
			Timer timer = null;
			timer = new Timer(delegate
			{
				timer.Dispose();
				PostToAppThread(handler);
			}, null, ms, Timeout.Infinite);
		}
		static public void PostToAppThread(Action handler)
		{
			if (handler == null) throw new ArgumentNullException(nameof(handler));
			MainThreadSynchronizationContext.Post(delegate
			{
				handler();
			}, null);
		}
		static public void InvokeInAppThread(Action handler)
		{
			if (handler == null) throw new ArgumentNullException(nameof(handler));
			MainThreadSynchronizationContext.Send(delegate
			{
				handler();
			}, null);
		}


		static public async Task InvokeInAppThreadAsync(Action handler)
		{
			if (handler == null) throw new ArgumentNullException(nameof(handler));
			Func<Task<object>> func = delegate
			{
				handler();
				return Task.FromResult<object>(null);
			};
			await _InternalInvokeAsync<object>(func);
		}
		static public async Task InvokeInAppThreadAsync(Func<Task> handler)
		{
			if (handler == null) throw new ArgumentNullException(nameof(handler));
			Func<Task<object>> func = async delegate
			{
				await handler();
				return null;
			};
			await _InternalInvokeAsync<object>(func);
		}

		static public async Task<TResult> InvokeInAppThreadAsync<TResult>(Func<Task<TResult>> handler)
		{
			return await _InternalInvokeAsync(handler ?? throw new ArgumentNullException(nameof(handler)));
		}

		static async Task<T> _InternalInvokeAsync<T>(Func<Task<T>> handler)
		{
			//TODO:not tested yet
			T result = default(T);
			Exception error = null;
			var cts = new CancellationTokenSource();
			PostToAppThread(delegate
			{
				try
				{
					Task<T> task = handler();
					task.ContinueWith(delegate
					{
						if (task.Exception != null)
							error = task.Exception;
						else
							result = task.Result;
						cts.Cancel();
					});
				}
				catch (Exception x)
				{
					error = x;
					cts.Cancel();
				}
			});
			await Task.Delay(-1, cts.Token);
			if (error != null)
				throw new Exception(error.Message, error);
			return result;
		}


		#endregion



		static public void ActivateForm(WF.Form form)
		{
			var oldstate = form.WindowState;
			form.WindowState = WF.FormWindowState.Minimized;
			form.Show();
			form.WindowState = oldstate == WF.FormWindowState.Minimized ? WF.FormWindowState.Normal : oldstate;
		}

		static public void CenterForm(WF.Form form)
		{
			var area = System.Windows.Forms.Screen.FromControl(form).WorkingArea;
			form.Left = (area.Width - form.Width) / 2;
			form.Top = (area.Height - form.Height) / 2;
		}

		static public void NotifyFormDisposed()
		{
			if (IsWindowsEventLoopQuitRequested)
				return;
			int fc = WF.Application.OpenForms.Count;

			int wc =
#if SUPPORT_WPF
				_wpfapp?.Windows.Count ?? 
#endif
				0;
			WriteDebugLine("NotifyFormDisposed : " + fc + "," + wc);
			if (fc == 0 && wc == 0)
			{
				QuitWindowsEventLoop();
				WriteDebugLine("QuitWindowsEventLoop");
			}
		}


		#region Cef Load/Unload

		static public bool InternalLoadLibCefDll(string fullpath)
		{
			uint LOAD_WITH_ALTERED_SEARCH_PATH = 0x00000008;
			IntPtr hr = WindowsInterop.LoadLibraryEx(fullpath, IntPtr.Zero, LOAD_WITH_ALTERED_SEARCH_PATH);
			if (hr == IntPtr.Zero)
				return false;

			WriteDebugLine("libcef.dll Load OK ");


			if (PrintDebugInformation)
			{
				string verstr = LibCefInterop.CefVersionArrFromLibCefDll();
				WriteDebugLine("cef_version_info : " + verstr);
				if (verstr != LibCefInterop.CefVersionArr)
				{
					WriteDebugLine(" * WARNING * , version not match for " + LibCefInterop.CefVersionArr);
				}
			}

			_libcefEverLoaded = true;
			_ApplyOptionsAfterLibCefLoaded();
			return true;
		}

		static public unsafe CefInitState InternalCefInitialize(string resourceFolder)
		{
			cef_settings_t* settings = LibCefInterop.Settings.FixedPtr;

			//TODO: better solution for dirs..

			if (settings->resources_dir_path.IsNullOrEmpty())
				F.invoke_set(&settings->resources_dir_path, resourceFolder);


			string folder;
			try
			{
				folder = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
				folder = System.IO.Path.Combine(folder, ApplicationName);
				if (!System.IO.Directory.Exists(folder))
					System.IO.Directory.CreateDirectory(folder);
			}
			catch (Exception x)
			{
				WriteDebugLine("Error:Failed to create local app data folder.");
				WriteDebugLine(x);
				folder = System.IO.Path.GetTempPath();
			}


			Console.WriteLine("CEF Data Folder : " + folder);

			if (settings->log_severity != cef_log_severity_t.LOGSEVERITY_DISABLE)
			{
				F.invoke_set(&settings->log_file, System.IO.Path.Combine(folder, "ceflog.txt"));
			}


			bool continueUseCacheFolder = true;
			if (SettingAutoClearCacheInStoragePath && DetectIsMainProcess())
			{
				try
				{
					void DeleteFolder(string path)
					{
						if (!Directory.Exists(path))
							return;
						foreach (string file in Directory.GetFiles(path))
							File.Delete(file);
						foreach (string subdir in Directory.GetDirectories(path))
							DeleteFolder(subdir);
						Directory.Delete(path);
					}
					DeleteFolder(Path.Combine(folder, "Cache"));
				}
				catch (Exception x)
				{
					continueUseCacheFolder = false;
					WriteDebugLine(x);
				}
			}

			if (SettingAutoSetCacheStoragePath && continueUseCacheFolder)
			{
				if (settings->cache_path.IsNullOrEmpty())
				{
					F.invoke_set(&settings->cache_path, folder);
					F.invoke_set(&settings->root_cache_path, folder);//force the root_cache_path use the same value
				}
			}

			if (SettingAutoSetUserDataStoragePath)
			{
				if (settings->user_data_path.IsNullOrEmpty())
					F.invoke_set(&settings->user_data_path, folder);
			}

			var state = LibCefInterop.Initialize();

			WriteDebugLine("libcef Initialize : " + state);

			if (state != CefInitState.Initialized)
				CloseSplashScreen();

			return state;
		}



		static internal volatile bool _CefShutdownCalled = false;
		static public void CefShutdown()
		{
#if !FXBELOW46
			bool tsngc = GC.TryStartNoGCRegion(8 * 1024 * 1024);  //there's something wrong , so stop GC while shutdown the CEF
			if (!tsngc)
				WriteDebugLine("Warning , TryStartNoGCRegion failed.");
#endif
			_CefShutdownCalled = true;
			try
			{
				WriteDebugLine("cef shutdown .. " + ApplicationElapsed);
				LibCefInterop.cef_shutdown();
				WriteDebugLine("cef shutdown OK " + ApplicationElapsed);
			}
			catch (AccessViolationException x)// not able to catch ..
			{
				WriteDebugLine(x.ToString());
			}
			finally
			{
				_CefShutdownCalled = false;

#if !FXBELOW46
				if (tsngc) GC.EndNoGCRegion();
#endif
			}

#if DEBUG
#if !FXBELOW46
			GC.Collect(GC.MaxGeneration, GCCollectionMode.Forced, true, false);
#endif
#endif


		}

		#endregion



		//Set Options:
		static public void SetEnableHighDPISupport()
		{
			_optionset.Add("SetEnableHighDPISupport");
			if (!_libcefEverLoaded)
				return;
			LibCefInterop.cef_enable_highdpi_support();
		}
		static void _ApplyOptionsAfterLibCefLoaded()
		{
			foreach (string option in _optionset)
			{
				switch (option)
				{
					case "SetEnableHighDPISupport":
						LibCefInterop.cef_enable_highdpi_support();
						break;
				}
			}
		}

		/// <summary>
		/// set AppData\Local\AppExeName , if set to false , the cookies/localStorage will not persisted.
		/// </summary>
		static public bool SettingAutoSetUserDataStoragePath { get; set; } = true;


		static public bool SettingAutoSetCacheStoragePath { get; set; } = true;

		/// <summary>
		/// before cef Initiaize , try to delete all cache files
		/// </summary>
		static public bool SettingAutoClearCacheInStoragePath { get; set; } = true;


		/// <summary>
		/// Allow the website send dll/code to run
		/// </summary>
		static public string[] SettingsTrustedWebsiteHosts { get; set; }

		/// <summary>
		/// allow webpage install signed assembly and execute it 
		/// 
		/// </summary>
		static public string[] SettingTrustedPublicKeyTokens { get; set; }

		/// <summary>
		/// Allow the browser compile the csharp code and run.
		/// It's not as safe as the dll with PublicKeyToken
		/// </summary>
		static public bool SettingAllowCompileCSharpCode { get; set; }
	}

}
