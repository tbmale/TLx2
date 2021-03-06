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
using System.IO;
using System.IO.Compression;
using System.Net;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace TLx2
{
	/// <summary>
	/// Class with program entry point.
	/// </summary>
	internal static class Program
	{
		internal static string html="";
		static string mainguid;
		[DllImport("wininet.dll", SetLastError = true)]
		private static extern long DeleteUrlCacheEntry(string lpszUrlName);
		static public Dictionary<string,List<string>> opts;
		static public string[] arguments;
		/// <summary>
		/// Program entry point.
		/// </summary>
		private static int Main(string[] args)
		{
//			DeleteUrlCacheEntry(String.Format("index.html"));
			opts=getopts(args);
			arguments=args;
			var thisass=Assembly.GetExecutingAssembly();
			string thispath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
			foreach (string dll in Directory.GetFiles(thispath, "*.dll"))
				try{Assembly.LoadFile(dll);}catch(Exception){};
			mainguid = "mainguid:"+thisass.GetCustomAttribute<GuidAttribute>().Value;
			var mainguidbytes=Encoding.ASCII.GetBytes(mainguid);
			int index=-1;
			byte[] exebytes;
			using(var s=File.Open(thisass.Location,FileMode.Open,FileAccess.Read)){
				var ms=new MemoryStream();
				s.CopyTo(ms);
				exebytes=ms.ToArray();
				var searcher=new BoyerMoore(mainguidbytes);
				var indices=searcher.Search(exebytes);
				foreach(int i in indices){
					index=i;
					break;
				}
			}
			//MessageBox.Show(string.Format("{0}: {1}",mainguid,index));
			if(opts.ContainsKey("add") && File.Exists(opts["add"][0])){
				var fp=File.Open(Path.GetFileNameWithoutExtension(opts["add"][0])+".exe",FileMode.OpenOrCreate);
				fp.Close();
				using(var o=new BinaryWriter(File.Open(Path.GetFileNameWithoutExtension(opts["add"][0])+".exe",FileMode.Open,FileAccess.Write))){
					int exelen=index==-1?exebytes.Length:index;
					o.Write(exebytes,0,exelen);
					o.Write(mainguidbytes);
					var compressStream=new MemoryStream();
					var compressor = new DeflateStream(compressStream, CompressionMode.Compress);
					var s=File.ReadAllBytes(opts["add"][0]);
					var jss=thisass.GetManifestResourceStream( "invoke.js" );
					var ms1= new MemoryStream();
					jss.CopyTo(ms1);
					var js=ms1.ToArray();
					ms1.Close();
					jss.Close();
					byte[] block=new byte[s.Length+js.Length];
					System.Buffer.BlockCopy(js,0,block,0,js.Length);
					System.Buffer.BlockCopy(s,0,block,js.Length,s.Length);
					compressor.Write(block,0,block.Length);
					compressor.Close();
					var cs=compressStream.ToArray();
					o.Write(cs);
				}
				Environment.Exit(0);
			}
			if(index!=-1){
				var fi=new FileInfo(thisass.Location);
				var s=new BinaryReader(File.Open(thisass.Location,FileMode.Open,FileAccess.Read));
				s.BaseStream.Seek(index+mainguid.Length,SeekOrigin.Begin);
				var cs=s.ReadBytes(Convert.ToInt32(fi.Length)-Convert.ToInt32(index)-Convert.ToInt32(mainguidbytes.Length));
				var compressStream=new MemoryStream(cs);
				var decomp=new MemoryStream();
				var decompressor = new DeflateStream(compressStream, CompressionMode.Decompress);
				decompressor.CopyTo(decomp);
				decompressor.Close();
				compressStream.Close();
				s.BaseStream.Close();
				html=Encoding.UTF8.GetString(decomp.ToArray());
				decomp.Close();
			}
			else{
				using ( var s=thisass.GetManifestResourceStream( "index.html" ) )
					using ( var r = new StreamReader( s ) )
						html = r.ReadToEnd();
				using ( var s=thisass.GetManifestResourceStream( "invoke.js" ) )
					using ( var r = new StreamReader( s ) )
						html += r.ReadToEnd();
			}
			
			//release the GUI !
			Thread uith = new Thread(new ThreadStart(startUI));
			uith.SetApartmentState(ApartmentState.STA);
			uith.Start();
			return 0;
		}
		[STAThread]
		static void startUI(){
			Application.EnableVisualStyles();
			Application.SetCompatibleTextRenderingDefault(false);
			Application.ApplicationExit += new EventHandler(Application_ApplicationExit);
			Application.Run(new MainForm());
		}
		static void Application_ApplicationExit(object o, EventArgs args){
//			WebRequest.CreateHttp(new Uri(String.Format("http://localhost:{0}/stop",_port))).GetResponseAsync();
		}
		static Dictionary<string,List<string>> getopts(string[] args){
			Dictionary<string,List<string>> res = new Dictionary<string, List<string>>();
			string ckey = "";
			res.Add(ckey,new List<string>());
			foreach(string arg in args){
				if(arg[0] == '-'){
					ckey = arg.Substring(1);
					if(!res.ContainsKey(ckey))
						res[ckey] = new List<string>();
				}
				else
					res[ckey].Add(arg);
			}
			return res;
		}
		
	}
	public sealed class BoyerMoore
	{
		readonly byte[] needle;
		readonly long[] charTable;
		readonly long[] offsetTable;

		public BoyerMoore(byte[] needle)
		{
			this.needle = needle;
			this.charTable = makeByteTable(needle);
			this.offsetTable = makeOffsetTable(needle);
		}

		public IEnumerable<long> Search(byte[] haystack)
		{
			if (needle.Length == 0)
				yield break;

			for (long i = needle.Length - 1; i < haystack.Length;)
			{
				long j;

				for (j = needle.Length - 1; needle[j] == haystack[i]; --i, --j)
				{
					if (j != 0)
						continue;

					yield return i;
					i += needle.Length - 1;
					break;
				}

				i += Math.Max(offsetTable[needle.Length - 1 - j], charTable[haystack[i]]);
			}
		}

		static long[] makeByteTable(byte[] needle)
		{
			const int ALPHABET_SIZE = 256;
			long[] table = new long[ALPHABET_SIZE];

			for (int i = 0; i < table.Length; ++i)
				table[i] = needle.Length;

			for (int i = 0; i < needle.Length - 1; ++i)
				table[needle[i]] = needle.Length - 1 - i;

			return table;
		}

		static long[] makeOffsetTable(byte[] needle)
		{
			long[] table = new long[needle.Length];
			long lastPrefixPosition = needle.Length;

			for (int i = needle.Length - 1; i >= 0; --i)
			{
				if (isPrefix(needle, i + 1))
					lastPrefixPosition = i + 1;

				table[needle.Length - 1 - i] = lastPrefixPosition - i + needle.Length - 1;
			}

			for (int i = 0; i < needle.Length - 1; ++i)
			{
				int slen = suffixLength(needle, i);
				table[slen] = needle.Length - 1 - i + slen;
			}

			return table;
		}

		static bool isPrefix(byte[] needle, int p)
		{
			for (int i = p, j = 0; i < needle.Length; ++i, ++j)
				if (needle[i] != needle[j])
					return false;

			return true;
		}

		static int suffixLength(byte[] needle, int p)
		{
			int len = 0;

			for (int i = p, j = needle.Length - 1; i >= 0 && needle[i] == needle[j]; --i, --j)
				++len;

			return len;
		}
	}
}
