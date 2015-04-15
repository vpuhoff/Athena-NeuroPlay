namespace TV_Set
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
      this.button3 = new System.Windows.Forms.Button();
      this.alphaFormTransformer1 = new AlphaForm.AlphaFormTransformer();
      this.pictureBox1 = new System.Windows.Forms.PictureBox();
      this.alphaFormMarker2 = new AlphaForm.AlphaFormMarker();
      this.button2 = new System.Windows.Forms.Button();
      this.button1 = new System.Windows.Forms.Button();
      this.alphaFormMarker1 = new AlphaForm.AlphaFormMarker();
      this.alphaFormTransformer1.SuspendLayout();
      ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
      this.SuspendLayout();
      // 
      // button3
      // 
      this.button3.BackColor = System.Drawing.Color.Goldenrod;
      this.button3.DialogResult = System.Windows.Forms.DialogResult.OK;
      this.button3.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(128)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
      this.button3.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(64)))), ((int)(((byte)(0)))));
      this.button3.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
      this.button3.ForeColor = System.Drawing.Color.White;
      this.button3.Location = new System.Drawing.Point(297, 370);
      this.button3.Name = "button3";
      this.button3.Size = new System.Drawing.Size(64, 39);
      this.button3.TabIndex = 3;
      this.button3.Text = "Off";
      this.button3.UseVisualStyleBackColor = false;
      this.button3.Click += new System.EventHandler(this.button3_Click);
      // 
      // alphaFormTransformer1
      // 
      this.alphaFormTransformer1.BackColor = System.Drawing.Color.Transparent;
      this.alphaFormTransformer1.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("alphaFormTransformer1.BackgroundImage")));
      this.alphaFormTransformer1.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
      this.alphaFormTransformer1.CanDrag = true;
      this.alphaFormTransformer1.Controls.Add(this.pictureBox1);
      this.alphaFormTransformer1.Controls.Add(this.alphaFormMarker2);
      this.alphaFormTransformer1.Controls.Add(this.button3);
      this.alphaFormTransformer1.Controls.Add(this.button2);
      this.alphaFormTransformer1.Controls.Add(this.button1);
      this.alphaFormTransformer1.Controls.Add(this.alphaFormMarker1);
      this.alphaFormTransformer1.Dock = System.Windows.Forms.DockStyle.Fill;
      this.alphaFormTransformer1.DragSleep = ((uint)(30u));
      this.alphaFormTransformer1.Location = new System.Drawing.Point(0, 0);
      this.alphaFormTransformer1.Name = "alphaFormTransformer1";
      this.alphaFormTransformer1.Size = new System.Drawing.Size(417, 499);
      this.alphaFormTransformer1.TabIndex = 0;
      // 
      // pictureBox1
      // 
      this.pictureBox1.BackColor = System.Drawing.SystemColors.Control;
      this.pictureBox1.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
      this.pictureBox1.Location = new System.Drawing.Point(24, 12);
      this.pictureBox1.Name = "pictureBox1";
      this.pictureBox1.Size = new System.Drawing.Size(367, 287);
      this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
      this.pictureBox1.TabIndex = 6;
      this.pictureBox1.TabStop = false;
      // 
      // alphaFormMarker2
      // 
      this.alphaFormMarker2.FillBorder = ((uint)(4u));
      this.alphaFormMarker2.Location = new System.Drawing.Point(182, 411);
      this.alphaFormMarker2.Name = "alphaFormMarker2";
      this.alphaFormMarker2.Size = new System.Drawing.Size(17, 17);
      this.alphaFormMarker2.TabIndex = 5;
      // 
      // button2
      // 
      this.button2.BackColor = System.Drawing.Color.Maroon;
      this.button2.FlatAppearance.BorderColor = System.Drawing.Color.Chocolate;
      this.button2.FlatAppearance.BorderSize = 2;
      this.button2.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
      this.button2.ForeColor = System.Drawing.Color.White;
      this.button2.Location = new System.Drawing.Point(192, 379);
      this.button2.Name = "button2";
      this.button2.Size = new System.Drawing.Size(86, 30);
      this.button2.TabIndex = 2;
      this.button2.Text = "Channel 2";
      this.button2.UseVisualStyleBackColor = false;
      this.button2.Click += new System.EventHandler(this.button2_Click);
      // 
      // button1
      // 
      this.button1.BackColor = System.Drawing.Color.Maroon;
      this.button1.FlatAppearance.BorderColor = System.Drawing.Color.Chocolate;
      this.button1.FlatAppearance.BorderSize = 2;
      this.button1.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
      this.button1.ForeColor = System.Drawing.Color.White;
      this.button1.Location = new System.Drawing.Point(88, 379);
      this.button1.Name = "button1";
      this.button1.Size = new System.Drawing.Size(88, 30);
      this.button1.TabIndex = 1;
      this.button1.Text = "Channel 1";
      this.button1.UseVisualStyleBackColor = false;
      this.button1.Click += new System.EventHandler(this.button1_Click);
      // 
      // alphaFormMarker1
      // 
      this.alphaFormMarker1.BackColor = System.Drawing.Color.Transparent;
      this.alphaFormMarker1.FillBorder = ((uint)(4u));
      this.alphaFormMarker1.Location = new System.Drawing.Point(206, 146);
      this.alphaFormMarker1.Name = "alphaFormMarker1";
      this.alphaFormMarker1.Size = new System.Drawing.Size(18, 18);
      this.alphaFormMarker1.TabIndex = 0;
      // 
      // Form1
      // 
      this.AcceptButton = this.button3;
      this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
      this.BackColor = System.Drawing.SystemColors.ActiveBorder;
      this.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("$this.BackgroundImage")));
      this.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
      this.ClientSize = new System.Drawing.Size(417, 499);
      this.Controls.Add(this.alphaFormTransformer1);
      this.DoubleBuffered = true;
      this.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
      this.Name = "Form1";
      this.Text = "Form1";
      this.Load += new System.EventHandler(this.Form1_Load);
      this.alphaFormTransformer1.ResumeLayout(false);
      ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
      this.ResumeLayout(false);

    }

    #endregion

    private AlphaForm.AlphaFormTransformer alphaFormTransformer1;
    private AlphaForm.AlphaFormMarker alphaFormMarker1;
    private System.Windows.Forms.Button button1;
    private System.Windows.Forms.Button button3;
    private System.Windows.Forms.Button button2;
    private AlphaForm.AlphaFormMarker alphaFormMarker2;
    private System.Windows.Forms.PictureBox pictureBox1;
  }
}

