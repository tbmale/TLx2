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
using System.Resources;
using System.IO.Compression;
using System.Reflection;
using System.Threading;
using System.Diagnostics;
using System.DirectoryServices.AccountManagement;
using System.Linq;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using System.Net.Http;
using System.ComponentModel;

[assembly: AssemblyTitle("TLx2")]
[assembly: AssemblyDescription("")]
[assembly: AssemblyConfiguration("")]
[assembly: AssemblyCompany("ExtraVeral")]
[assembly: AssemblyProduct("TLx2")]
[assembly: AssemblyCopyright("Copyright 2021")]
[assembly: AssemblyTrademark("")]
[assembly: AssemblyCulture("")]
// This sets the default COM visibility of types in the assembly to invisible.
// If you need to expose a type to COM, use [ComVisible(true)] on that type.
[assembly: ComVisible(false)]
// The assembly version has following format :
//
// Major.Minor.Build.Revision
//
// You can specify all the values or you can use the default the Revision and
// Build Numbers by using the '*' as shown below:
[assembly: AssemblyVersion("0.1.*")]
[assembly: Guid("4b280ace-08b1-47de-9fc8-5964941ac7e6")]

namespace TLx2
{
  /// <summary>
  /// Description of MainForm.
  /// </summary>
  // public partial class MainForm : Form
  public class MainForm : Form
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
    public static Icon ExtractAssociatedIcon(String filePath)
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
    void MainFormClosing(object sender, FormClosingEventArgs e)
    {
      var res = (bool?)webBrowser1.Document.InvokeScript("triggercloseevent") ?? true;
      e.Cancel = !(bool)res;
    }
    void NavigateToRessource(object sender, WebBrowserNavigatingEventArgs e)
    {
      MessageBox.Show(e.Url.ToString());
    }
    // }

    private System.ComponentModel.IContainer components = null;
    public System.Windows.Forms.WebBrowser webBrowser1;

    protected override void Dispose(bool disposing)
    {
      if (disposing)
      {
        if (components != null)
        {
          components.Dispose();
        }
      }
      base.Dispose(disposing);
    }

    /// <summary>
    /// This method is required for Windows Forms designer support.
    /// Do not change the method contents inside the source code editor. The Forms designer might
    /// not be able to load this method if it was changed manually.
    /// </summary>
    private void InitializeComponent()
    {
      webBrowser1 = new System.Windows.Forms.WebBrowser();
      this.SuspendLayout();
      // 
      // webBrowser1
      // 
      this.webBrowser1.Dock = System.Windows.Forms.DockStyle.Fill;
      this.webBrowser1.Location = new System.Drawing.Point(0, 0);
      this.webBrowser1.MinimumSize = new System.Drawing.Size(20, 20);
      this.webBrowser1.Name = "webBrowser1";
      this.webBrowser1.AllowWebBrowserDrop = false;
      this.webBrowser1.IsWebBrowserContextMenuEnabled = false;
      //			this.webBrowser1.WebBrowserShortcutsEnabled = false;
      this.webBrowser1.ScriptErrorsSuppressed = true;
      this.webBrowser1.ObjectForScripting = new ScriptManager();
      //this.webBrowser1.Size = new System.Drawing.Size(800, 600);
      this.webBrowser1.TabIndex = 0;
      //webBrowser1.Url=new System.Uri(string.Format("http://localhost:{0}/{1}",Program._port,Program.resource));
      webBrowser1.DocumentText = Program.html;
      // 
      // MainForm
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.ClientSize = new System.Drawing.Size(800, 600);
      this.Controls.Add(this.webBrowser1);
      this.Name = "MainForm";
      this.Text = Path.GetFileNameWithoutExtension(AppDomain.CurrentDomain.FriendlyName);
      //			this.Icon = System.Drawing.Icon.ExtractAssociatedIcon(Path.Combine(AppDomain.CurrentDomain.BaseDirectory,AppDomain.CurrentDomain.FriendlyName));
      //this.Icon = (Icon)Resource1.ORANO_VERTICAL_SEUL_NOIRJAUNE_RVB;
      //			var rm=new ResourceManager("GrapevinePublicFolder.Resource1", typeof(Resource1).Assembly);
      //			this.Icon=(Icon)rm.GetObject("ORANO_VERTICAL_SEUL_NOIRJAUNE_RVB",CultureInfo.InvariantCulture);
      this.ResumeLayout(false);

    }
  }

  /// <summary>
  /// Class with program entry point.
  /// </summary>
  internal static class Program
  {
    internal static string html = "";
    // static string mainguid;
    [DllImport("wininet.dll", SetLastError = true)]
    private static extern long DeleteUrlCacheEntry(string lpszUrlName);
    static public Dictionary<string, List<string>> opts;
    static public string[] arguments;
    /// <summary>
    /// Program entry point.
    /// </summary>
    private static int Main(string[] args)
    {
      //			DeleteUrlCacheEntry(String.Format("index.html"));
      opts = getopts(args);
      arguments = args;
      try
      {
        var thisass = Assembly.GetExecutingAssembly();
        string thispath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
        foreach (string dll in Directory.GetFiles(thispath, "*-ScriptExtensions.dll"))
        {
         
          try { Assembly.LoadFile(dll); }
          catch (Exception ex) { MessageBox.Show(string.Format("Assembly.LoadFile({0}):{1}", dll, ex.Message)); };
        }
        // mainguid = "mainguid:" + thisass.GetCustomAttribute<GuidAttribute>().Value;
        // var mainguidbytes = Encoding.ASCII.GetBytes(mainguid);
        using (var s = thisass.GetManifestResourceStream("index.html"))
        using (var r = new StreamReader(s))
          html = r.ReadToEnd();
        using (var s = thisass.GetManifestResourceStream("invoke.html"))
        using (var r = new StreamReader(s))
          html += r.ReadToEnd();

      }
      catch (Exception ex)
      {
        var st = new StackTrace(ex, true);
        // Get the top stack frame
        var frame = st.GetFrame(0);
        // Get the line number from the stack frame
        var line = frame.GetFileLineNumber();
        Console.WriteLine("line {0}: {1}", line, ex.Message);
      }

      //release the GUI !
      Thread uith = new Thread(new ThreadStart(startUI));
      uith.SetApartmentState(ApartmentState.STA);
      uith.Start();
      return 0;
    }
    public static MainForm form;
    [STAThread]
    static void startUI()
    {
      Application.EnableVisualStyles();
      Application.SetCompatibleTextRenderingDefault(false);
      Application.ApplicationExit += new EventHandler(Application_ApplicationExit);
      form = new MainForm();
      Application.Run(form);
    }
    static void Application_ApplicationExit(object o, EventArgs args)
    {
      //			WebRequest.CreateHttp(new Uri(String.Format("http://localhost:{0}/stop",_port))).GetResponseAsync();
    }
    static Dictionary<string, List<string>> getopts(string[] args)
    {
      Dictionary<string, List<string>> res = new Dictionary<string, List<string>>();
      string ckey = "";
      res.Add(ckey, new List<string>());
      foreach (string arg in args)
      {
        if (arg[0] == '-')
        {
          ckey = arg.Substring(1);
          if (!res.ContainsKey(ckey))
            res[ckey] = new List<string>();
        }
        else
          res[ckey].Add(arg);
      }
      return res;
    }

  }

  /// <summary>
  /// Description of ScriptExtensions.
  /// </summary>
  public static class ScriptExtensions
  {
    static Dictionary<string, IEnumerator<string>> dirlist = new Dictionary<string, IEnumerator<string>>();
    static Dictionary<string, IEnumerator<string>> filelist = new Dictionary<string, IEnumerator<string>>();
    static HttpClient http = new HttpClient();
    static bool _debug = false;
    [DllImport("kernel32.dll", SetLastError = true, ExactSpelling = true)]
    static extern bool FreeConsole();
    [DllImport("kernel32.dll")]
    [return: MarshalAs(UnmanagedType.Bool)]
    static extern bool AllocConsole();
    [DllImport("kernel32.dll")]
    static extern bool AttachConsole(int pid);

    public static void allocconsole()
    {
      if (!AttachConsole(-1))
        AllocConsole();
    }
    public static void freeconsole()
    {
      FreeConsole();
    }
    public static void consolelog(string text)
    {
      Console.OutputEncoding = Encoding.UTF8;
      var bytes = Encoding.UTF8.GetBytes(text);
      Console.OpenStandardOutput().Write(bytes, 0, bytes.Length);
    }
    public static void consoleerror(string text)
    {
      Console.OutputEncoding = Encoding.UTF8;
      var bytes = Encoding.UTF8.GetBytes(text);
      Console.OpenStandardError().Write(bytes, 0, bytes.Length);
    }
    public static string Arguments()
    {
      return new ResultValue { error = false, value = Program.arguments }.Json;
    }

    public static string toggledebug()
    {
      _debug = _debug == false;
      Program.form.webBrowser1.ScriptErrorsSuppressed = _debug;
      return new ResultValue { error = false, value = _debug }.Json;
    }
    public static string debug(bool val)
    {
      _debug = val;
      Program.form.webBrowser1.ScriptErrorsSuppressed = _debug;
      return new ResultValue { error = false, value = _debug }.Json;
    }
    public static void appclose()
    {
      Environment.Exit(1);
    }
    public static void resize(int x, int y)
    {
      Application.OpenForms[0].Size = new Size(x, y);
    }
    public static void title(string text)
    {
      Application.OpenForms[0].Text = text;
      Application.OpenForms[0].Update();
    }
    public static bool ismember(string groupname)
    {
      PrincipalContext context = new PrincipalContext(ContextType.Domain);
      UserPrincipal user = UserPrincipal.Current;
      if (user != null)
      {
        var ret = true;
        try { ret = user.IsMemberOf(context, IdentityType.SamAccountName, groupname); }
        catch (Exception) { ret = false; }
        return ret;
      }
      return false;
    }
    public static string getenv(string varname)
    {
      return Environment.GetEnvironmentVariable(varname);
    }
    public static void setenv(string varname, string varvalue)
    {
      Environment.SetEnvironmentVariable(varname, varvalue);
    }
    public static string getclipboard()
    {
      return Clipboard.GetText();
    }
    public static void setclipboard(string text)
    {
      Clipboard.SetText(text);
    }
    public static string readfile(string fpath)
    {
      string data = "";
      var res = new ResultValue();
      try
      {
        data = System.IO.File.ReadAllText(fpath);
      }
      catch (Exception ex)
      {
        res = new ResultValue(ex.Message);
      }
      if (!res.error) res = new ResultValue { error = false, value = data };
      return res.Json;
    }
    public static string writefile(string fpath, string content)
    {
      try
      {
        File.WriteAllText(fpath, content);
      }
      catch (Exception ex)
      {
        return new ResultValue(ex.Message).Json;
      }
      return new ResultValue { error = false, value = "" }.Json;
    }
    public static void alert(string message)
    {
      MessageBox.Show(message, "Message");
    }
    public static void alertm(string message, string title)
    {
      MessageBox.Show(message, title);
    }
    public static int alertmb(string message, string title, string type)
    {
      MessageBoxButtons dialogtype;
      if (!Enum.TryParse<MessageBoxButtons>(type, out dialogtype)) return -1;
      return (int)MessageBox.Show(message, title, dialogtype);
    }
    public static int alertmbi(string message, string title, string type, string icon)
    {
      MessageBoxButtons dialogtype;
      MessageBoxIcon dialogicon;
      if (!Enum.TryParse<MessageBoxButtons>(type, out dialogtype) || !Enum.TryParse<MessageBoxIcon>(icon, out dialogicon)) return -1;
      return (int)MessageBox.Show(message, title, dialogtype, dialogicon);
    }
    public static string regread(string key, string valuename = "")
    {
      RegistryKey rkey;
      try
      {
        rkey = Registry.CurrentUser.OpenSubKey(key);
      }
      catch (Exception ex)
      {
        return new ResultValue(ex.Message).Json;
      }
      if (rkey == null)
        return new ResultValue("key not found").Json;
      string rvalue = "";
      try
      {
        rvalue = (string)rkey.GetValue(valuename);
      }
      catch (Exception ex)
      {
        return new ResultValue(ex.Message).Json;
      }
      if (rvalue == null)
        return new ResultValue("value not found").Json;
      return new ResultValue { error = false, value = rvalue }.Json;
    }
    public static string regwrite(string key, string valuename, string value)
    {
      RegistryKey rkey;
      var keystree = key.Split('\\');
      RegistryKey treekey = Registry.CurrentUser;
      foreach (var keyname in keystree)
      {
        try
        {
          treekey = treekey.CreateSubKey(keyname);
        }
        catch (Exception) { }
      }
      try
      {
        rkey = Registry.CurrentUser.OpenSubKey(key, true);
      }
      catch (Exception ex)
      {
        return new ResultValue(ex.Message).Json;
      }
      if (rkey == null)
        return new ResultValue("key not found").Json;
      try
      {
        rkey.SetValue(valuename, value);
      }
      catch (Exception ex)
      {
        return new ResultValue(ex.Message).Json;
      }
      return new ResultValue { error = false, value = value }.Json;
    }
    public static void run(string cmd, string arguments)
    {
      ProcessStartInfo processInfo;
      Process process;

      processInfo = new ProcessStartInfo(cmd, arguments);
      processInfo.UseShellExecute = true;
      process = Process.Start(processInfo);

    }
    public static string hostname()
    {
      return System.Net.Dns.GetHostName();
    }
    public static string getfilepaths(string pathname, string filter = "*")
    {
      string[] paths;
      if (File.Exists(pathname))
      {
        paths = Directory.EnumerateFiles(Path.GetDirectoryName(pathname), filter, SearchOption.AllDirectories).ToArray();
      }
      else if (Directory.Exists(pathname))
      {
        // This path is a directory
        paths = Directory.EnumerateFiles(pathname, filter, SearchOption.AllDirectories).ToArray();
      }
      else
      {
        return new ResultValue(string.Format("{0} is not a valid file or directory.", pathname)).Json;
      }
      return new ResultValue { error = false, value = paths }.Json;
    }
    public static string enumeratedirs(string pathname, bool deep, string filter = "*")
    //		public static string enumeratedirs(string pathname,string filter)
    {
      var key = string.Format("{0}|{1}", pathname, filter);
      if (!dirlist.ContainsKey(key))
      {
        if (File.Exists(pathname))
        {
          dirlist.Add(key, Directory.EnumerateDirectories(Path.GetDirectoryName(pathname), filter, deep ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly).GetEnumerator());
        }
        else if (Directory.Exists(pathname))
        {
          dirlist.Add(key, Directory.EnumerateDirectories(pathname, filter, deep ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly).GetEnumerator());
        }
        else
        {
          return new ResultValue(string.Format("{0} is not a valid file or directory.", pathname)).Json;
        }
      }
      if (!dirlist[key].MoveNext())
      {
        return new ResultValue("List exhausted").Json;
      }
      return new ResultValue { error = false, value = dirlist[key].Current }.Json;

    }
    public static string enumeratefiles(string pathname, bool deep, string filter = "*")
    {
      var key = string.Format("{0}|{1}", pathname, filter);
      if (!filelist.ContainsKey(key))
      {
        if (File.Exists(pathname))
        {
          filelist.Add(key, Directory.EnumerateFiles(Path.GetDirectoryName(pathname), filter, deep ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly).GetEnumerator());
        }
        else if (Directory.Exists(pathname))
        {
          filelist.Add(key, Directory.EnumerateFiles(pathname, filter, deep ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly).GetEnumerator());
        }
        else
        {
          return new ResultValue(string.Format("{0} is not a valid file or directory.", pathname)).Json;
        }
      }
      if (!filelist[key].MoveNext())
      {
        return new ResultValue("List exhausted").Json;
      }
      return new ResultValue { error = false, value = filelist[key].Current }.Json;

    }
    //		static string dirpath(string pathname,string filter="*"){
    //			if(dirlist==null)
    //				getdirpaths(pathname,filter);
    //		}
    public static string resetdirsenum(string pathname, string filter = "*")
    {
      var key = string.Format("{0}|{1}", pathname, filter);
      if (!dirlist.ContainsKey(key))
        return new ResultValue("Enumerator not initialized.").Json;
      dirlist.Remove(key);
      return new ResultValue { error = false, value = "" }.Json;
    }
    public static string resetfilesenum(string pathname, string filter = "*")
    {
      var key = string.Format("{0}|{1}", pathname, filter);
      if (!filelist.ContainsKey(key))
        return new ResultValue("Enumerator not initialized.").Json;
      filelist.Remove(key);
      return new ResultValue { error = false, value = "" }.Json;
    }
    public static string getresource(string resname)
    {
      var thisass = Assembly.GetExecutingAssembly();
      var s = thisass.GetManifestResourceStream(resname);
      byte[] bytes;
      if (s != null)
        using (var r = new StreamReader(s))
        using (var ms = new MemoryStream())
        {
          s.CopyTo(ms);
          bytes = ms.ToArray();
          return Convert.ToBase64String(bytes);
        }
      return "";
    }
    public static string gitlab_gettoken(string url, string user, string pass)
    {
      var payload = "{\"grant_type\":\"password\",\"username\":\"" + user + "\",\"password\":\"" + pass + "\"}";
      var content = new StringContent(payload);
      content.Headers.ContentType.MediaType = "application/json";
      var res = http.PostAsync(url, content).Result;
      if (res.IsSuccessStatusCode)
        return res.Content.ReadAsStringAsync().Result;
      return new ResultValue(res.StatusCode.ToString()).Json;
    }
    public static string gitlab_getapi(string url, string token)
    {
      var req = new HttpRequestMessage();
      req.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
      req.Method = HttpMethod.Get;
      req.RequestUri = new Uri(url);
      var res = http.SendAsync(req).Result;
      if (res.IsSuccessStatusCode)
        return res.Content.ReadAsStringAsync().Result;
      return new ResultValue(res.StatusCode.ToString()).Json;
    }
    public static string gitlab_putapi(string url, string token, string payload)
    {
      var req = new HttpRequestMessage();
      req.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
      req.Method = HttpMethod.Put;
      var content = new StringContent(payload);
      content.Headers.ContentType.MediaType = "application/json";
      req.Content = content;
      req.RequestUri = new Uri(url);
      var res = http.SendAsync(req).Result;
      if (res.IsSuccessStatusCode)
        return res.Content.ReadAsStringAsync().Result;
      return new ResultValue(res.StatusCode.ToString()).Json;
    }
    public static string gitlab_getarchive(string url, string token, string sha1)
    {
      return "not implemented";
    }
  }

  [DataContract]
  [KnownType(typeof(string[]))]
  [KnownType(typeof(Dictionary<string, List<string>>))]
  public class ResultValue
  {
    [DataMember]
    public bool error;
    [DataMember]
    public object value;
    public ResultValue()
    {
      error = false;
      value = "";
    }
    public ResultValue(object errmsg)
    {
      error = true;
      value = errmsg;
    }
    public string Json
    {
      get
      {
        var ms = new MemoryStream();
        var json = new DataContractJsonSerializer(typeof(ResultValue));
        json.WriteObject(ms, this);
        return System.Text.Encoding.UTF8.GetString(ms.ToArray());
      }
    }
  }
  /// <summary>
  /// Description of ScriptManager.
  /// </summary>
  [ComVisible(true)]
  public class ScriptManager
  {
    readonly Dictionary<string, MethodInfo> methlist;
    public ScriptManager()
    {
      methlist = new Dictionary<string, MethodInfo>();
      var q = AppDomain.CurrentDomain.GetAssemblies()
        .SelectMany(t => t.GetTypes()).Where(t => t.IsAbstract && t.IsSealed && t.IsClass && t.Namespace == "TLx2" && t.Name == "ScriptExtensions");
      var list = new List<string>();
      q.ToList().ForEach(t => t.GetMethods().Where(m => m.IsStatic).ToList().ForEach(m =>
      {
        if (m.Name == "Start")
          m.Invoke(null, null);
        else
          methlist[m.Name] = m;
      }));

    }
    public string Arguments
    {
      get { return new ResultValue { error = false, value = Program.opts }.Json; }
    }

    public void doinbackground(string methname, string callback, object[] args)
    {
      var worker = new BackgroundWorker();
      worker.DoWork += new DoWorkEventHandler(worker_DoWork);
      worker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(worker_RunWorkerCompleted);
      // worker.ProgressChanged +=new ProgressChangedEventHandler(worker_ProgressChanged);
      worker.RunWorkerAsync(new object[] { methname, callback, args });
    }

    void worker_DoWork(object sender, DoWorkEventArgs ev)
    {
      var objs = (object[])ev.Argument;
      var methname = (string)objs[0];
      var callback = (string)objs[1];
      var args = (object[])objs[2];
      if (!methlist.ContainsKey(methname)) ev.Result = new ResultValue(string.Format("Method <{0}> not found", methname)).Json;
      ev.Result = new object[] { methlist[methname].Invoke(null, args), callback };
    }
    void worker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs ev)
    {
      var res = (object[])ev.Result;
      if (ev.Error != null)
        Program.form.webBrowser1.Document.InvokeScript((string)res[1], new string[] { new ResultValue(ev.Error.Message).Json });
      Program.form.webBrowser1.Document.InvokeScript((string)res[1], new string[] { (string)res[0] });
    }
    public string getmethodslist()
    {
      return new ResultValue { error = false, value = methlist.Keys.ToArray() }.Json;
    }
    public object callmethod(string mname, object[] args)
    {
      if (!methlist.ContainsKey(mname))
        throw new MissingMethodException(mname + " not found");
      object[] allArgs = args;
      if (methlist[mname].GetParameters().Length != args.Length)
      {
        var defaultArgs = methlist[mname].GetParameters().Skip(args.Length)
          .Select(a => a.HasDefaultValue ? a.DefaultValue : null);
        allArgs = args.Concat(defaultArgs).ToArray();
      }
      //			return methlist[mname].Invoke(null,allArgs);
      object res;
      try
      {
        res = methlist[mname].Invoke(null, allArgs);
      }
      catch (Exception ex)
      {
        return new ResultValue(ex.Message);
      }
      return res;
    }
  }
}
