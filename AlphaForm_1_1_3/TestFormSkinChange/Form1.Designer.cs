namespace TestFormSkinChange
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
      this.skinButton2 = new System.Windows.Forms.Button();
      this.skinButton1 = new System.Windows.Forms.Button();
      this.alphaFormMarker1 = new AlphaForm.AlphaFormMarker();
      this.closeButton = new System.Windows.Forms.Button();
      this.alphaFormTransformer1.SuspendLayout();
      this.SuspendLayout();
      // 
      // alphaFormTransformer1
      // 
      this.alphaFormTransformer1.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("alphaFormTransformer1.BackgroundImage")));
      this.alphaFormTransformer1.Controls.Add(this.closeButton);
      this.alphaFormTransformer1.Controls.Add(this.skinButton2);
      this.alphaFormTransformer1.Controls.Add(this.skinButton1);
      this.alphaFormTransformer1.Controls.Add(this.alphaFormMarker1);
      this.alphaFormTransformer1.Dock = System.Windows.Forms.DockStyle.Fill;
      this.alphaFormTransformer1.DragSleep = ((uint)(30u));
      this.alphaFormTransformer1.Location = new System.Drawing.Point(0, 0);
      this.alphaFormTransformer1.Name = "alphaFormTransformer1";
      this.alphaFormTransformer1.Size = new System.Drawing.Size(400, 371);
      this.alphaFormTransformer1.TabIndex = 0;
      // 
      // skinButton2
      // 
      this.skinButton2.Location = new System.Drawing.Point(217, 158);
      this.skinButton2.Name = "skinButton2";
      this.skinButton2.Size = new System.Drawing.Size(75, 23);
      this.skinButton2.TabIndex = 2;
      this.skinButton2.Text = "Skin 2";
      this.skinButton2.UseVisualStyleBackColor = true;
      this.skinButton2.Click += new System.EventHandler(this.skinButton2_Click);
      // 
      // skinButton1
      // 
      this.skinButton1.Location = new System.Drawing.Point(111, 158);
      this.skinButton1.Name = "skinButton1";
      this.skinButton1.Size = new System.Drawing.Size(75, 23);
      this.skinButton1.TabIndex = 1;
      this.skinButton1.Text = "Skin 1";
      this.skinButton1.UseVisualStyleBackColor = true;
      this.skinButton1.Click += new System.EventHandler(this.skinButton1_Click);
      // 
      // alphaFormMarker1
      // 
      this.alphaFormMarker1.FillBorder = ((uint)(4u));
      this.alphaFormMarker1.Location = new System.Drawing.Point(199, 251);
      this.alphaFormMarker1.Name = "alphaFormMarker1";
      this.alphaFormMarker1.Size = new System.Drawing.Size(15, 16);
      this.alphaFormMarker1.TabIndex = 0;
      // 
      // closeButton
      // 
      this.closeButton.Location = new System.Drawing.Point(164, 273);
      this.closeButton.Name = "closeButton";
      this.closeButton.Size = new System.Drawing.Size(75, 23);
      this.closeButton.TabIndex = 3;
      this.closeButton.Text = "Close";
      this.closeButton.UseVisualStyleBackColor = true;
      this.closeButton.Click += new System.EventHandler(this.closeButton_Click);
      // 
      // Form1
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
      this.ClientSize = new System.Drawing.Size(400, 371);
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
    private System.Windows.Forms.Button skinButton2;
    private System.Windows.Forms.Button skinButton1;
    private AlphaForm.AlphaFormMarker alphaFormMarker1;
    private System.Windows.Forms.Button closeButton;
  }
}

