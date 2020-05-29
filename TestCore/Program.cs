using System;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;

using CefLite;

namespace TestCore
{
	public class Program
	{

		[STAThread]
		static public void Main(string[] args)
		{
			CefWin.PrintDebugInformation = true;

			CefWin.ApplicationTitle = "TestCoreApp"; //as the Default Title

			CefWin.SearchLibCefSubPathList.Add("chromium");         // search ./chromium/ for libcef.dll
			CefWin.SearchLibCefSubPathList.Add(@"bin\Debug\netcoreapp3.1\chromium");
			CefInitState initState = CefWin.SearchAndInitialize();

			if (initState != CefInitState.Initialized)
			{
				if (initState == CefInitState.Failed)
				{
					System.Windows.Forms.MessageBox.Show("Failed to start application\r\nCheck the github page about how to deploy the libcef.dll", "Error"
					   , System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error);
				}
				return;
			}

			using IHost host = CreateHostBuilder(args).Build();
			try
			{
				host.Start();
			}
			catch (Exception x)
			{
				Console.WriteLine(x);
				System.Windows.Forms.MessageBox.Show("Failed to start service. Please try again. \r\n" + x.Message, "Error"
					 , System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error);
				CefWin.CefShutdown();
				return;
			}

			CefWin.ApplicationHost = host;
			CefWin.ApplicationTask = host.WaitForShutdownAsync(CefWin.ApplicationCTS.Token);

			ShowMainForm();

			CefWin.RunApplication();

		}

		static void ShowMainForm()
		{
			string startUrl = aspnetcoreUrls.Split(';')[0];
			DefaultBrowserForm form = CefWin.OpenBrowser(startUrl);
			form.Width = 1120;
			form.Height = 777;
			form.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			//CefWin.CenterForm(form);
			//form.WindowState = System.Windows.Forms.FormWindowState.Maximized;
		}

		static string aspnetcoreUrls = "http://127.12.34.56:7890";
		//static string aspnetcoreUrls = "http://127.12.34.56:7890;https://127.12.34.56:7891";
		//static string aspnetcoreUrls = "https://127.12.34.56:7891";       //Force to SSL , not so useful , just a test
		//static string aspnetcoreUrls = CefWin.MakeFixedLocalHostUrl();    //make fixed url by user name , so each user can open 1 instance
		//static string aspnetcoreUrls = CefWin.MakeRandomLocalHostUrl();   //random url allow multiple instance of this app , but cookie/localStorage will lost when open app again.


		static public IHostBuilder CreateHostBuilder(string[] args)
		{
			var builder = Host.CreateDefaultBuilder(args);

			builder.ConfigureWebHostDefaults(webBuilder =>
			{
				Console.WriteLine("aspnetcoreUrls : " + aspnetcoreUrls);
				webBuilder.UseUrls(aspnetcoreUrls);
				webBuilder.UseStartup<Startup>();
			});

			return builder;
		}


	}
}
