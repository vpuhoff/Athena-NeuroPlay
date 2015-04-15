namespace LayerdOnBottom
{
	partial class Form1
	{
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		/// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
		protected override void Dispose(bool disposing)
		{
		if (disposing && (components != null))
		{
		components.Dispose();
		}
		base.Dispose(disposing);
		}

		#region Windows Form Designer generated code

		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
		System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
		this.alphaFormTransformer1 = new AlphaForm.AlphaFormTransformer();
		this.treeView1 = new System.Windows.Forms.TreeView();
		this.tabControl1 = new System.Windows.Forms.TabControl();
		this.tabPage1 = new System.Windows.Forms.TabPage();
		this.tabPage2 = new System.Windows.Forms.TabPage();
		this.monthCalendar1 = new System.Windows.Forms.MonthCalendar();
		this.button1 = new System.Windows.Forms.Button();
		this.alphaFormTransformer1.SuspendLayout();
		this.tabControl1.SuspendLayout();
		this.SuspendLayout();
		// 
		// alphaFormTransformer1
		// 
		this.alphaFormTransformer1.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("alphaFormTransformer1.BackgroundImage")));
		this.alphaFormTransformer1.Controls.Add(this.treeView1);
		this.alphaFormTransformer1.Controls.Add(this.tabControl1);
		this.alphaFormTransformer1.Controls.Add(this.monthCalendar1);
		this.alphaFormTransformer1.Controls.Add(this.button1);
		this.alphaFormTransformer1.Dock = System.Windows.Forms.DockStyle.Fill;
		this.alphaFormTransformer1.DragSleep = ((uint)(30u));
		this.alphaFormTransformer1.Location = new System.Drawing.Point(0, 0);
		this.alphaFormTransformer1.Name = "alphaFormTransformer1";
		this.alphaFormTransformer1.Size = new System.Drawing.Size(488, 479);
		this.alphaFormTransformer1.TabIndex = 0;
		// 
		// treeView1
		// 
		this.treeView1.Location = new System.Drawing.Point(282, 123);
		this.treeView1.Name = "treeView1";
		this.treeView1.Size = new System.Drawing.Size(148, 246);
		this.treeView1.TabIndex = 4;
		// 
		// tabControl1
		// 
		this.tabControl1.Controls.Add(this.tabPage1);
		this.tabControl1.Controls.Add(this.tabPage2);
		this.tabControl1.Location = new System.Drawing.Point(69, 308);
		this.tabControl1.Name = "tabControl1";
		this.tabControl1.SelectedIndex = 0;
		this.tabControl1.Size = new System.Drawing.Size(200, 100);
		this.tabControl1.TabIndex = 3;
		// 
		// tabPage1
		// 
		this.tabPage1.Location = new System.Drawing.Point(4, 22);
		this.tabPage1.Name = "tabPage1";
		this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
		this.tabPage1.Size = new System.Drawing.Size(192, 74);
		this.tabPage1.TabIndex = 0;
		this.tabPage1.Text = "tabPage1";
		this.tabPage1.UseVisualStyleBackColor = true;
		// 
		// tabPage2
		// 
		this.tabPage2.Location = new System.Drawing.Point(4, 22);
		this.tabPage2.Name = "tabPage2";
		this.tabPage2.Padding = new System.Windows.Forms.Padding(3);
		this.tabPage2.Size = new System.Drawing.Size(192, 74);
		this.tabPage2.TabIndex = 1;
		this.tabPage2.Text = "tabPage2";
		this.tabPage2.UseVisualStyleBackColor = true;
		// 
		// monthCalendar1
		// 
		this.monthCalendar1.Location = new System.Drawing.Point(69, 123);
		this.monthCalendar1.Name = "monthCalendar1";
		this.monthCalendar1.TabIndex = 1;
		// 
		// button1
		// 
		this.button1.BackColor = System.Drawing.Color.Transparent;
		this.button1.Location = new System.Drawing.Point(69, 75);
		this.button1.Name = "button1";
		this.button1.Size = new System.Drawing.Size(75, 23);
		this.button1.TabIndex = 0;
		this.button1.Tag = "back";
		this.button1.Text = "button1";
		this.button1.UseVisualStyleBackColor = false;
		// 
		// Form1
		// 
		this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
		this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		this.ClientSize = new System.Drawing.Size(488, 479);
		this.Controls.Add(this.alphaFormTransformer1);
		this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
		this.Name = "Form1";
		this.Text = "Form1";
		this.Load += new System.EventHandler(this.Form1_Load);
		this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Form1_FormClosing);
		this.alphaFormTransformer1.ResumeLayout(false);
		this.tabControl1.ResumeLayout(false);
		this.ResumeLayout(false);

		}

		#endregion

		private AlphaForm.AlphaFormTransformer alphaFormTransformer1;
		private System.Windows.Forms.MonthCalendar monthCalendar1;
		private System.Windows.Forms.Button button1;
		private System.Windows.Forms.TabControl tabControl1;
		private System.Windows.Forms.TabPage tabPage1;
		private System.Windows.Forms.TabPage tabPage2;
		private System.Windows.Forms.TreeView treeView1;
	}
}

