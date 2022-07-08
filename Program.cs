/*
 * Created by SharpDevelop.
 * User: tdragulinescu
 * Date: 31/03/2021
 * Time: 14:42
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */

using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;
using Microsoft.CSharp;

namespace TLx4
{
  internal static class Program
  {
    private static int Main(string[] args)
    {
      var thisass = Assembly.GetExecutingAssembly();
      var opts = getopts(args);
      Console.WriteLine("opts count:{0}", opts.Count);
      if (opts.Count == 0 || opts.ContainsKey("h"))
      {
        showHelp();
        return 0;
      }
      string cs;
      using (var s = thisass.GetManifestResourceStream("bundle.cs"))
      using (var r = new StreamReader(s))
        cs = r.ReadToEnd();
      var tempfile = "invoke.html";
      using (var s = thisass.GetManifestResourceStream("invoke.html"))
      using (var r = new StreamReader(s))
      using (var fo = File.Open(tempfile, FileMode.OpenOrCreate, FileAccess.Write))
      {
        var barr = Encoding.UTF8.GetBytes(r.ReadToEnd());
        fo.Write(barr, 0, barr.Length);
      }
      string outname = "out.exe";
      if (opts.ContainsKey("mainpage"))
        outname = Path.GetFileNameWithoutExtension(opts["mainpage"][0]) + ".exe";
      else
      {
        Console.WriteLine("-mainpage option is mandatory and must point to a valid file");
        return 1;
      }
      cs = cs.Replace("index.html", opts["mainpage"][0]);
      string icon = "";
      if (opts.ContainsKey("icon") && File.Exists(opts["icon"][0]))
        icon = opts["icon"][0];
      CSharpCodeProvider codeProvider = new CSharpCodeProvider();
      string[] m_References = new string[] {
        "System.dll",
        "System.Core.dll",
        "System.Data.dll",
        "System.Data.DataSetExtensions.dll",
        "System.Net.Http.dll",
        "System.Net.dll",
        "System.DirectoryServices.AccountManagement.dll",
        "System.Drawing.dll",
        "System.Runtime.Serialization.dll",
        "System.Windows.Forms.dll",
        "System.Xml.dll",
        "System.Xml.Linq.dll"
      };
      CompilerParameters cp = new CompilerParameters
      {
        GenerateExecutable = true,
        OutputAssembly = outname,
        GenerateInMemory = false,
        TreatWarningsAsErrors = false,
        CompilerOptions = "/t:winexe "
      };
      cp.CompilerOptions += opts.ContainsKey("debug") ? "/debug " : "";
      cp.CompilerOptions += opts.ContainsKey("icon") ? ("/win32icon:" + icon + " ") : "";
      cp.CompilerOptions += opts.ContainsKey("x86") ? ("/platform:anycpu32bitpreferred ") : "";
      cp.EmbeddedResources.Add(opts["mainpage"][0]);
      cp.EmbeddedResources.Add(tempfile);
      if (opts.ContainsKey("res"))
        foreach (string res in opts["res"])
          cp.EmbeddedResources.Add(res);
      cp.ReferencedAssemblies.AddRange(m_References);
      var m_CompilerResults = codeProvider.CompileAssemblyFromSource(cp, cs);
      if (!m_CompilerResults.Errors.HasErrors)
      {
        Console.WriteLine("Compilation succesful: {0}", outname);
        File.Delete(tempfile);
        return 0;
      }
      else
      {
        Console.WriteLine("Errors building {0} into {1}", "bundle", m_CompilerResults.PathToAssembly);
        foreach (CompilerError ce in m_CompilerResults.Errors)
        {
          Console.WriteLine("  {0}", ce.ToString());
          Console.WriteLine();
        }
        File.Delete(tempfile);
        return 1;
      }

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
    static void showHelp()
    {
      Console.WriteLine(
        "-h\t\t this help\n" +
        "-mainpage\t html page to embed/render (must contain all javascript code also)\n" +
        "-debug\t\t compile with debug\n" +
        "-icon\t\t add icon to compiled executable" +
        "-res\t\t add file as resource to compiled executable");

    }
  }
}