using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.Drawing;
using System.Runtime.InteropServices;

namespace AlphaForms
{
	class LayeredWindow : Form
	{
		private Rectangle m_rect;

		public Point LayeredPos
		{
			get { return m_rect.Location; }
			set { m_rect.Location = value; }
		}

		public Size LayeredSize
		{
			get { return m_rect.Size; }
		}

		public LayeredWindow()
		{
			//We need to set this before the window is created, otherwise we
			//have to reset thw window styles using SetWindowLong because
			//the window will no longer be drawn
			this.ShowInTaskbar = false;

			this.FormBorderStyle = FormBorderStyle.None;
		}

		public void UpdateWindow(Bitmap image, byte opacity)
		{
			UpdateWindow(image, opacity, -1, -1, this.LayeredPos);
		}

		public void UpdateWindow(Bitmap image, byte opacity, int width, int height, Point pos)
		{
			IntPtr hdcWindow = Win32.GetWindowDC(this.Handle);
			IntPtr hDC = Win32.CreateCompatibleDC(hdcWindow);
			IntPtr hBitmap = image.GetHbitmap(Color.FromArgb(0));
			IntPtr hOld = Win32.SelectObject(hDC, hBitmap);
			Size size = new Size(0,0);
			Point zero = new Point(0, 0);

			if (width == -1 || height == -1) {
				//No width and height specified, use the size of the image
				size.Width = image.Width;
				size.Height = image.Height;
			} else {
				//Use whichever size is smallest, so that the image will
				//be clipped if necessary
				size.Width = Math.Min(image.Width, width);
				size.Height = Math.Min(image.Height, height);
			}
			m_rect.Size = size;
			m_rect.Location = pos;

			Win32.BLENDFUNCTION blend = new Win32.BLENDFUNCTION();
			blend.BlendOp = (byte)Win32.BlendOps.AC_SRC_OVER;
			blend.SourceConstantAlpha = opacity;
			blend.AlphaFormat = (byte)Win32.BlendOps.AC_SRC_ALPHA;
			blend.BlendFlags = (byte)Win32.BlendFlags.None;

			Win32.UpdateLayeredWindow(this.Handle, hdcWindow, ref pos, ref size, hDC, ref zero, 0, ref blend, Win32.BlendFlags.ULW_ALPHA);
			
			Win32.SelectObject(hDC, hOld);
			Win32.DeleteObject(hBitmap);
			Win32.DeleteDC(hDC);
			Win32.ReleaseDC(this.Handle, hdcWindow);
		}

		protected override CreateParams CreateParams
		{
			get
			{
				CreateParams cp = base.CreateParams;
				cp.ExStyle |= (int)Win32.WindowStyles.WS_EX_LAYERED;
				return cp;
			}
		}

        private void InitializeComponent()
        {
            this.SuspendLayout();
            // 
            // LayeredWindow
            // 
            this.ClientSize = new System.Drawing.Size(282, 255);
            this.Name = "LayeredWindow";
            this.Load += new System.EventHandler(this.LayeredWindow_Load);
            this.ResumeLayout(false);

        }

        private void LayeredWindow_Load(object sender, EventArgs e)
        {

        }
	}
}
