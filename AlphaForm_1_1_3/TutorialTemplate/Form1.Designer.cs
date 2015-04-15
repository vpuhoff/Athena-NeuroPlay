namespace TestFormBraid2
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
    this.button1 = new System.Windows.Forms.Button();
    this.label2 = new System.Windows.Forms.Label();
    this.label1 = new System.Windows.Forms.Label();
    this.alphaFormTransformer1.SuspendLayout();
    this.SuspendLayout();
    // 
    // alphaFormTransformer1
    // 
    this.alphaFormTransformer1.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("alphaFormTransformer1.BackgroundImage")));
    this.alphaFormTransformer1.Controls.Add(this.button1);
    this.alphaFormTransformer1.Controls.Add(this.label2);
    this.alphaFormTransformer1.Controls.Add(this.label1);
    this.alphaFormTransformer1.Dock = System.Windows.Forms.DockStyle.Fill;
    this.alphaFormTransformer1.DragSleep = ((uint)(30u));
    this.alphaFormTransformer1.Location = new System.Drawing.Point(0, 0);
    this.alphaFormTransformer1.Name = "alphaFormTransformer1";
    this.alphaFormTransformer1.Size = new System.Drawing.Size(950, 605);
    this.alphaFormTransformer1.TabIndex = 0;
    // 
    // button1
    // 
    this.button1.Location = new System.Drawing.Point(600, 497);
    this.button1.Name = "button1";
    this.button1.Size = new System.Drawing.Size(75, 23);
    this.button1.TabIndex = 8;
    this.button1.Text = "Close";
    this.button1.UseVisualStyleBackColor = true;
    this.button1.Click += new System.EventHandler(this.button1_Click);
    // 
    // label2
    // 
    this.label2.Font = new System.Drawing.Font("Tahoma", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
    this.label2.Location = new System.Drawing.Point(461, 387);
    this.label2.Name = "label2";
    this.label2.Size = new System.Drawing.Size(302, 113);
    this.label2.TabIndex = 7;
    this.label2.Text = "For a completed demo project, run the TestFormGroBotoTV example.";
    // 
    // label1
    // 
    this.label1.Font = new System.Drawing.Font("Tahoma", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
    this.label1.Location = new System.Drawing.Point(461, 192);
    this.label1.Name = "label1";
    this.label1.Size = new System.Drawing.Size(350, 165);
    this.label1.TabIndex = 6;
    this.label1.Text = "This is just a template project for the How to Use tutorial. After reading the tu" +
        "torial you can use this project to complete the steps in hooking up the AlphaFor" +
        "mTransformer control.";
    // 
    // Form1
    // 
    this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
    this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
    this.ClientSize = new System.Drawing.Size(950, 605);
    this.Controls.Add(this.alphaFormTransformer1);
    this.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
    this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
    this.Name = "Form1";
    this.Text = "Form1";
    this.Load += new System.EventHandler(this.Form1_Load);
    this.alphaFormTransformer1.ResumeLayout(false);
    this.ResumeLayout(false);

    }

    #endregion

    private AlphaForm.AlphaFormTransformer alphaFormTransformer1;
    private System.Windows.Forms.Label label1;
    private System.Windows.Forms.Label label2;
    private System.Windows.Forms.Button button1;
  }
}

