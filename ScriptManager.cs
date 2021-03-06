/*
 * Created by SharpDevelop.
 * User: tdragulinescu
 * Date: 11/02/2021
 * Time: 12:07
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;

namespace TLx2
{
	[DataContract]
	[KnownType(typeof(string[]))]
	[KnownType(typeof(Dictionary<string,List<string>>))]
	public class ResultValue{
		[DataMember]
		public bool error;
		[DataMember]
		public object value;
		public ResultValue(){
			error=false;
			value="";
		}
		public ResultValue(object errmsg){
			error=true;
			value=errmsg;
		}
		public string Json{get{
				var ms = new MemoryStream();
				var json=new DataContractJsonSerializer(typeof(ResultValue));
				json.WriteObject(ms,this);
				return System.Text.Encoding.UTF8.GetString(ms.ToArray());
			}}
	}
	/// <summary>
	/// Description of ScriptManager.
	/// </summary>
	[ComVisible(true)]
	public class ScriptManager
	{
		readonly Dictionary<string,MethodInfo> methlist;
		public ScriptManager()
		{
			methlist=new Dictionary<string, MethodInfo>();
			var q = AppDomain.CurrentDomain.GetAssemblies()
				.SelectMany(t => t.GetTypes()).Where(t =>t.IsAbstract && t.IsSealed && t.IsClass && t.Namespace == "TLx2" && t.Name=="ScriptExtensions");
			var list=new List<string>();
			q.ToList().ForEach(t => t.GetMethods().Where(m => m.IsStatic).ToList().ForEach(m=>methlist[m.Name]=m));

		}
		public string Arguments{
			get{return new ResultValue{error=false,value=Program.opts}.Json;}
		}
		
		public string getmethodslist()
		{
			return new ResultValue{error=false,value=methlist.Keys.ToArray()}.Json;
		}
		public object callmethod(string mname,object[] args){
			if(!methlist.ContainsKey(mname))
				throw new MissingMethodException(mname+" not found");
			object[] allArgs = args;
			if (methlist[mname].GetParameters().Length != args.Length)
			{
				var defaultArgs = methlist[mname].GetParameters().Skip(args.Length)
					.Select(a => a.HasDefaultValue ? a.DefaultValue : null);
				allArgs = args.Concat(defaultArgs).ToArray();
			}
			return methlist[mname].Invoke(null,allArgs);
		}
	}
}
