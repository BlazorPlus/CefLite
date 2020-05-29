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
using Microsoft.CodeAnalysis;
using System.IO;

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

			//dotnetcore don't support System.CodeDom.Compiler

			throw new NotImplementedException("For dotnetcore , must Use the CefWin.CompileCodeHandler.");

			//var comp = Microsoft.CodeAnalysis.CSharp.CSharpCompilation.Create(dllname);
			//comp = comp.AddReferences(
			//	existAsms.Select(v =>
			//	{
			//		try
			//		{
			//			return MetadataReference.CreateFromFile(v.Location);
			//		}
			//		catch
			//		{
			//			return null;
			//		}
			//	}).Where(v => v != null).ToArray()
			//	);
			//var opt = new Microsoft.CodeAnalysis.CSharp.CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary);
			//comp = comp.WithOptions(opt);

			//MemoryStream ms = new MemoryStream();
			//var res = comp.Emit(ms);
			//if (res.Success)
			//	return Assembly.Load(ms.ToArray());
			//throw new Exception("compilation error:\r\n" + string.Join("\r\n", res.Diagnostics.Select(v => v.ToString()).ToArray()));
		}

	}
}
