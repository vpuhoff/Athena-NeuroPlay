namespace TestForm
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
                this.alphaFormMarker7 = new AlphaForm.AlphaFormMarker();
                this.offButton = new System.Windows.Forms.Button();
                this.alphaFormMarker2 = new AlphaForm.AlphaFormMarker();
                this.listBox1 = new System.Windows.Forms.ListBox();
                this.pictureBox1 = new System.Windows.Forms.PictureBox();
                this.alphaFormTransformer1.SuspendLayout();
                ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
                this.SuspendLayout();
                // 
                // alphaFormTransformer1
                // 
                this.alphaFormTransformer1.BackColor = System.Drawing.Color.Navy;
                this.alphaFormTransformer1.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("alphaFormTransformer1.BackgroundImage")));
                this.alphaFormTransformer1.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
                this.alphaFormTransformer1.Controls.Add(this.pictureBox1);
                this.alphaFormTransformer1.Controls.Add(this.listBox1);
                this.alphaFormTransformer1.Controls.Add(this.alphaFormMarker7);
                this.alphaFormTransformer1.Controls.Add(this.offButton);
                this.alphaFormTransformer1.Controls.Add(this.alphaFormMarker2);
                this.alphaFormTransformer1.Dock = System.Windows.Forms.DockStyle.Fill;
                this.alphaFormTransformer1.DragSleep = ((uint)(30u));
                this.alphaFormTransformer1.Location = new System.Drawing.Point(0, 0);
                this.alphaFormTransformer1.Margin = new System.Windows.Forms.Padding(4);
                this.alphaFormTransformer1.Name = "alphaFormTransformer1";
                this.alphaFormTransformer1.Size = new System.Drawing.Size(472, 528);
                this.alphaFormTransformer1.TabIndex = 0;
                this.alphaFormTransformer1.Paint += new System.Windows.Forms.PaintEventHandler(this.alphaFormTransformer1_Paint);
                // 
                // alphaFormMarker7
                // 
                this.alphaFormMarker7.FillBorder = ((uint)(4u));
                this.alphaFormMarker7.Location = new System.Drawing.Point(228, 217);
                this.alphaFormMarker7.Margin = new System.Windows.Forms.Padding(4);
                this.alphaFormMarker7.Name = "alphaFormMarker7";
                this.alphaFormMarker7.Size = new System.Drawing.Size(21, 21);
                this.alphaFormMarker7.TabIndex = 20;
                // 
                // offButton
                // 
                this.offButton.BackColor = System.Drawing.Color.Transparent;
                this.offButton.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
                this.offButton.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(128)))), ((int)(((byte)(0)))));
                this.offButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
                this.offButton.Font = new System.Drawing.Font("Tahoma", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
                this.offButton.ForeColor = System.Drawing.SystemColors.ButtonFace;
                this.offButton.Location = new System.Drawing.Point(104, 201);
                this.offButton.Margin = new System.Windows.Forms.Padding(4);
                this.offButton.Name = "offButton";
                this.offButton.Size = new System.Drawing.Size(68, 72);
                this.offButton.TabIndex = 4;
                this.offButton.Text = "Off";
                this.offButton.UseVisualStyleBackColor = false;
                this.offButton.Click += new System.EventHandler(this.offButton_Click);
                // 
                // alphaFormMarker2
                // 
                this.alphaFormMarker2.FillBorder = ((uint)(2u));
                this.alphaFormMarker2.Location = new System.Drawing.Point(151, 201);
                this.alphaFormMarker2.Margin = new System.Windows.Forms.Padding(4);
                this.alphaFormMarker2.Name = "alphaFormMarker2";
                this.alphaFormMarker2.Size = new System.Drawing.Size(21, 21);
                this.alphaFormMarker2.TabIndex = 10;
                // 
                // listBox1
                // 
                this.listBox1.FormattingEnabled = true;
                this.listBox1.ItemHeight = 17;
                this.listBox1.Location = new System.Drawing.Point(273, 217);
                this.listBox1.Name = "listBox1";
                this.listBox1.Size = new System.Drawing.Size(123, 55);
                this.listBox1.TabIndex = 21;
                // 
                // pictureBox1
                // 
                this.pictureBox1.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("pictureBox1.BackgroundImage")));
                this.pictureBox1.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
                this.pictureBox1.Location = new System.Drawing.Point(33, 12);
                this.pictureBox1.Name = "pictureBox1";
                this.pictureBox1.Size = new System.Drawing.Size(400, 334);
                this.pictureBox1.TabIndex = 22;
                this.pictureBox1.TabStop = false;
                // 
                // Form1
                // 
                this.AutoScaleDimensions = new System.Drawing.SizeF(120F, 120F);
                this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
                this.BackColor = System.Drawing.Color.White;
                this.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
                this.ClientSize = new System.Drawing.Size(472, 528);
                this.Controls.Add(this.alphaFormTransformer1);
                this.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
                this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
                this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
                this.Margin = new System.Windows.Forms.Padding(4);
                this.Name = "Form1";
                this.Opacity = 0D;
                this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
                this.Text = "Form1";
                this.Load += new System.EventHandler(this.Form1_Load);
                this.alphaFormTransformer1.ResumeLayout(false);
                ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
                this.ResumeLayout(false);

			}

		#endregion

		private AlphaForm.AlphaFormTransformer alphaFormTransformer1;
        private System.Windows.Forms.Button offButton;
        private AlphaForm.AlphaFormMarker alphaFormMarker2;
    private AlphaForm.AlphaFormMarker alphaFormMarker7;
    private System.Windows.Forms.ListBox listBox1;
    private System.Windows.Forms.PictureBox pictureBox1;
		}
	}

