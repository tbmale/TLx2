/*
 * Created by SharpDevelop.
 * User: tdragulinescu
 * Date: 10/02/2021
 * Time: 13:24
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Runtime.InteropServices;
using System.Security;
using System.Text;
using System.Windows.Forms;
using Microsoft.Win32;

namespace TLx2
{
	/// <summary>
	/// Description of MainForm.
	/// </summary>
	public partial class MainForm : Form
	{
		[DllImport("wininet.dll", SetLastError = true)]
		private static extern bool InternetSetOption(IntPtr hInternet, int dwOption, IntPtr lpBuffer, int lpdwBufferLength);
		private const int INTERNET_OPTION_END_BROWSER_SESSION = 42;

		public MainForm()
		{
			//
			// The InitializeComponent() call is required for Windows Forms designer support.
			//
			int BrowserVer, RegVal;

			// get the installed IE version
			using (WebBrowser Wb = new WebBrowser())
				BrowserVer = Wb.Version.Major;

			// set the appropriate IE version
			if (BrowserVer >= 11)
				RegVal = 11001;
			else if (BrowserVer == 10)
				RegVal = 10001;
			else if (BrowserVer == 9)
				RegVal = 9999;
			else if (BrowserVer == 8)
				RegVal = 8888;
			else
				RegVal = 7000;

			// set the actual key
			using (RegistryKey Key = Registry.CurrentUser.CreateSubKey(@"SOFTWARE\Microsoft\Internet Explorer\Main\FeatureControl\FEATURE_BROWSER_EMULATION", RegistryKeyPermissionCheck.ReadWriteSubTree))
				if (Key.GetValue(System.Diagnostics.Process.GetCurrentProcess().ProcessName + ".exe") == null)
					Key.SetValue(System.Diagnostics.Process.GetCurrentProcess().ProcessName + ".exe", RegVal, RegistryValueKind.DWord);
			InternetSetOption(IntPtr.Zero, INTERNET_OPTION_END_BROWSER_SESSION, IntPtr.Zero, 0);
			InitializeComponent();
			Icon = ExtractAssociatedIcon(System.Reflection.Assembly.GetExecutingAssembly().Location);
			webBrowser1.Refresh(WebBrowserRefreshOption.Completely);
			//webBrowser1.Navigating+=NavigateToRessource;
			this.FormClosing += MainFormClosing;
		}
		[SuppressUnmanagedCodeSecurity]
		internal static class SafeNativeMethods
		{
			[DllImport("shell32.dll", EntryPoint = "ExtractAssociatedIcon", CharSet = CharSet.Auto)]
			internal static extern IntPtr ExtractAssociatedIcon(HandleRef hInst, StringBuilder iconPath, ref int index);
		}
		public static Icon  ExtractAssociatedIcon(String filePath)
		{
			int index = 0;

			Uri uri;
			if (filePath == null)
			{
				throw new ArgumentException(String.Format("'{0}' is not valid for '{1}'", "null", "filePath"), "filePath");
			}
			try
			{
				uri = new Uri(filePath);
			}
			catch (UriFormatException)
			{
				filePath = Path.GetFullPath(filePath);
				uri = new Uri(filePath);
			}
			//if (uri.IsUnc)
			//{
			//  throw new ArgumentException(String.Format("'{0}' is not valid for '{1}'", filePath, "filePath"), "filePath");
			//}
			if (uri.IsFile)
			{
				if (!File.Exists(filePath))
				{
					//IntSecurity.DemandReadFileIO(filePath);
					throw new FileNotFoundException(filePath);
				}

				StringBuilder iconPath = new StringBuilder(260);
				iconPath.Append(filePath);

				IntPtr handle = SafeNativeMethods.ExtractAssociatedIcon(new HandleRef(null, IntPtr.Zero), iconPath, ref index);
				if (handle != IntPtr.Zero)
				{
					//IntSecurity.ObjectFromWin32Handle.Demand();
					return Icon.FromHandle(handle);
				}
			}
			return null;
		}
		void MainFormClosing(object sender, FormClosingEventArgs e){
			var res = (bool?)webBrowser1.Document.InvokeScript("triggercloseevent") ?? true;
			e.Cancel = !(bool)res;
		}
		void NavigateToRessource(object sender, WebBrowserNavigatingEventArgs  e){
			MessageBox.Show(e.Url.ToString());
		}
	}
}
