using CefLite;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Reflection;

namespace CefLite
{
	public partial class CefWin
	{

		private static Assembly CompileCode(Assembly[] existAsms, string dllname, string code)
		{
			if (CompileCodeHandler != null)
			{
				var ccasm = CompileCodeHandler(existAsms, dllname, code);
				if (ccasm != null)
					return ccasm;
			}


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

			var resasm = cr.CompiledAssembly;
			return resasm;
		}

	}
}

static internal class _Extensions
{
	static public async Task WriteAsync(this System.IO.Pipes.NamedPipeServerStream self, byte[] data)
	{
		await self.WriteAsync(data, 0, data.Length);
	}
	static public async Task WaitForConnectionAsync(this System.IO.Pipes.NamedPipeServerStream self, CancellationToken token)
	{
		Exception err = null;
		var cts = new CancellationTokenSource();
		Thread t = new Thread(delegate ()
		{
			try
			{
				self.WaitForConnection();
			}
			catch (Exception x)
			{
				err = x;
				cts.Cancel();
			}
		});
		try
		{
			await Task.Delay(-1, cts.Token);
		}
		catch (Exception)
		{

		}
		if (err != null)
			throw new Exception(err.Message, err);
	}
	static public string[] Split(this string self, string spliter)
	{
		if (string.IsNullOrEmpty(spliter)) throw new ArgumentNullException(nameof(spliter));
		int pos = self.IndexOf(spliter);
		if (pos == -1)
			return new string[] { spliter };
		if (pos == 0)
			return Split(self.Substring(spliter.Length), spliter);
		List<string> list = new List<string>();
		list.Add(self.Substring(0, pos));
		void SplitRest(int start)
		{
			pos = self.IndexOf(spliter, start);
			if (pos == -1)
			{
				list.Add(self.Substring(start));
				return;
			}
			if (pos == start)
				list.Add(string.Empty);
			else
				list.Add(self.Substring(start, pos - start));
			SplitRest(pos + spliter.Length);
		}
		SplitRest(pos + spliter.Length);
		return list.ToArray();
	}
}

//namespace System.Windows
//{
//	public partial class Application
//	{
//		public List<Window> Windows { get; } = new List<Window>();
//		public void Run() { throw new NotSupportedException(); }
//		public void Shutdown() { }
//	}

//	public class Window
//	{
//		public void Close() { }
//	}
//}

