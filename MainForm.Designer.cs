/*
 * Created by SharpDevelop.
 * User: tdragulinescu
 * Date: 10/02/2021
 * Time: 13:24
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.IO;
using System.Resources;
namespace TLx2
{
	partial class MainForm
	{
		/// <summary>
		/// Designer variable used to keep track of non-visual components.
		/// </summary>
		private System.ComponentModel.IContainer components = null;
		private System.Windows.Forms.WebBrowser webBrowser1;
		
		/// <summary>
		/// Disposes resources used by the form.
		/// </summary>
		/// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
		protected override void Dispose(bool disposing)
		{
			if (disposing) {
				if (components != null) {
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
			this.webBrowser1 = new System.Windows.Forms.WebBrowser();
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
			this.webBrowser1.ObjectForScripting= new ScriptManager();
			//this.webBrowser1.Size = new System.Drawing.Size(800, 600);
			this.webBrowser1.TabIndex = 0;
			//webBrowser1.Url=new System.Uri(string.Format("http://localhost:{0}/{1}",Program._port,Program.resource));
			webBrowser1.DocumentText=Program.html;
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
}
