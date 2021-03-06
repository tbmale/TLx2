/*
 * Created by SharpDevelop.
 * User: tdragulinescu
 * Date: 05/03/2021
 * Time: 17:36
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Diagnostics;
using System.DirectoryServices.AccountManagement;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;
using Microsoft.Win32;

namespace TLx2
{
	/// <summary>
	/// Description of ScriptExtensions.
	/// </summary>
	public static class ScriptExtensions
	{
		[DllImport("kernel32.dll", SetLastError=true, ExactSpelling=true)]
		static extern bool FreeConsole();
		[DllImport("kernel32.dll")]
		[return: MarshalAs(UnmanagedType.Bool)]
		static extern bool AllocConsole();
		[DllImport("kernel32.dll")]
		static extern bool AttachConsole(int pid);

		public static void allocconsole(){
			if ( !AttachConsole(-1) )
				AllocConsole();
		}
		public static void freeconsole(){
			FreeConsole();
		}
		public static void consolelog(string text){
			Console.OutputEncoding = Encoding.UTF8;
			var bytes=Encoding.UTF8.GetBytes(text);
			Console.OpenStandardOutput().Write(bytes,0,bytes.Length);
		}
		public static void consoleerror(string text){
			Console.OutputEncoding = Encoding.UTF8;
			var bytes=Encoding.UTF8.GetBytes(text);
			Console.OpenStandardError().Write(bytes,0,bytes.Length);
		}
		public static string Arguments(){
			return new ResultValue{error=false,value=Program.arguments}.Json;
		}
		public static string Opts(){
			return new ResultValue{error=false,value=Program.opts}.Json;
		}
		public static bool ismember(string groupname){
			PrincipalContext context = new PrincipalContext(ContextType.Domain);
			UserPrincipal user = UserPrincipal.Current;
			if(user != null){
				return user.IsMemberOf(context, IdentityType.SamAccountName, groupname);
			}
			return false;
		}
		public static string getenv(string varname){
			return Environment.GetEnvironmentVariable(varname);
		}
		public static void setenv(string varname, string varvalue){
			Environment.SetEnvironmentVariable(varname,varvalue);
		}
		public static string readfile(string fpath){
			string data="";
			var res=new ResultValue();
			try{
				data=System.IO.File.ReadAllText(fpath);
			}catch(Exception ex){
				res = new ResultValue(ex.Message);
			}
			if(!res.error)res = new ResultValue{error=false,value=data};
			return res.Json;
		}
		public static string writefile(string fpath, string content){
			try{
				File.WriteAllText(fpath, content);
			}catch(Exception ex){
				return new ResultValue(ex.Message).Json;
			}
			return new ResultValue{error=false,value=""}.Json;
		}
		public static void alert(string message,string title="Message"){
			MessageBox.Show(message,title);
		}
		public static string regread(string key, string valuename=""){
			RegistryKey rkey;
			try{
				rkey=Registry.CurrentUser.OpenSubKey(key);
			}catch(Exception ex){
				return new ResultValue(ex.Message).Json;
			}
			if(rkey==null)
				return new ResultValue("key not found").Json;
			string rvalue="";
			try{
				rvalue=(string)rkey.GetValue(valuename);
			}catch(Exception ex){
				return new ResultValue(ex.Message).Json;
			}
			if(rvalue==null)
				return new ResultValue("value not found").Json;
			return new ResultValue{error=false,value=rvalue}.Json;
		}
		public static string regwrite(string key, string valuename, string value){
			RegistryKey rkey;
			var keystree=key.Split('\\');
			RegistryKey treekey=Registry.CurrentUser;
			foreach(var keyname in keystree){
				try{
					treekey=treekey.CreateSubKey(keyname);
				}catch(Exception){}
			}
			try{
				rkey=Registry.CurrentUser.OpenSubKey(key,true);
			}catch(Exception ex){
				return new ResultValue(ex.Message).Json;
			}
			if(rkey==null)
				return new ResultValue("key not found").Json;
			string rvalue="";
			try{
				rvalue=(string)rkey.GetValue(valuename);
			}catch(Exception ex){
				return new ResultValue(ex.Message).Json;
			}
			try{
				rkey.SetValue(valuename,value);
			}catch(Exception ex){
				return new ResultValue(ex.Message).Json;
			}
			return new ResultValue{error=false,value=value}.Json;
		}
		public static void run(string cmd,string arguments){
			ProcessStartInfo processInfo;
			Process process;

			processInfo = new ProcessStartInfo(cmd,arguments);
			processInfo.UseShellExecute = true;
			process = Process.Start(processInfo);
			
		}
		public static string hostname(){
			return System.Net.Dns.GetHostName();
		}
	}
}
