using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Diagnostics;
using System.ComponentModel.Design;
using System.ComponentModel;

namespace AlphaForms
{
	public partial class AlphaForm : System.Windows.Forms.Form
	{
		public AlphaForm()
		{
			if (!this.DesignMode)
			{
				m_layeredWnd = new LayeredWindow();
			}

			m_sizeMode = SizeModes.None;
			m_background = null;
			m_backgroundEx = null;
			m_backgroundFull = null;
			m_renderCtrlBG = false;
			m_enhanced = false;
			m_isDown.Left = false;
			m_isDown.Right = false;
			m_isDown.Middle = false;
			m_isDown.XBtn = false;
			m_moving = false;
			m_hiddenControls = new List<Control>();
			m_controlDict = new Dictionary<Control, bool>();
			m_initialised = false;

			//Set drawing styles
			this.SetStyle(ControlStyles.DoubleBuffer, true);
		}

		#region Properties
		public enum SizeModes
		{
			None,
			Stretch,
			Clip
		}

		/// <summary>
		/// Gets or Sets the image to be blended with the desktop
		/// </summary>
		[Category("AlphaForm")]
		public Bitmap BlendedBackground
		{
			get { return m_background; }
			set
			{
				if (m_background != value)
				{
					m_background = value;
					UpdateLayeredBackground();
				}
			}
		}

		/// <summary>
		/// If true, a portion of the background image will be drawn behind
		/// each control on the form. This is to solve problems with some
		/// controls that need to blend with the background, Labels are an 
		/// excellent example.
		/// </summary>
		[Category("AlphaForm")]
		public bool DrawControlBackgrounds
		{
			get { return m_renderCtrlBG; }
			set
			{
				if (m_renderCtrlBG != value)
				{
					m_renderCtrlBG = value;
					UpdateLayeredBackground();
				}
			}
		}

		/// <summary>
		/// If true when the form is dragged the foreground window will be drawn to the
		/// background window and then hidden. This prevents any visual disparity between
		/// the two forms.
		/// </summary>
		[Category("AlphaForm")]
		public bool EnhancedRendering
		{
			get { return m_enhanced; }
			set { m_enhanced = value; }
		}

		/// <summary>
		/// Sets the size mode of the form.
		///   None: The background image will always remain its original size
		///   Stretch: The background will be resized to fit the client area of the main form
		///   Clip: The background image will be clipped to within the client area of the main form
		/// </summary>
		[Category("AlphaForm")]
		public SizeModes SizeMode
		{
			get { return m_sizeMode; }
			set
			{
				m_sizeMode = value;
				UpdateLayeredBackground();
			}
		}

		public void SetOpacity(double Opacity)
		{
			this.Opacity = Opacity;
			if (m_background != null)
			{
				int width = this.ClientSize.Width;
				int height = this.ClientSize.Height;
				if(m_sizeMode == SizeModes.None)
				{
					width = m_background.Width;
					height = m_background.Height;
				}

				byte _opacity = (byte)(this.Opacity * 255);
				if (m_useBackgroundEx)
				{
					m_layeredWnd.UpdateWindow(m_backgroundEx, _opacity, width, height, m_layeredWnd.LayeredPos);
				}
				else
				{
					m_layeredWnd.UpdateWindow(m_background, _opacity, width, height, m_layeredWnd.LayeredPos);
				}
			}
		}

		public void UpdateLayeredBackground()
		{
			updateLayeredBackground(this.ClientSize.Width, this.ClientSize.Height);
		}

		public void DrawControlBackground(Control ctrl, bool drawBack)
		{
			if (m_controlDict.ContainsKey(ctrl))
				m_controlDict[ctrl] = drawBack;
		}
		#endregion

		protected override void OnLoad(EventArgs e)
		{
			base.OnLoad(e);

			//Set the transparency key to make the back of the form transparent
			//INTERESTING NOTE: If you use Fuchsia as the transparent colour then
			// it will be not only visually transparent but also transparent to 
			// mouse clicks. If you use any other colour then you will be able to
			// see through it, but you'll still get your mouse events
			this.BackColor = Color.Fuchsia;
			this.TransparencyKey = Color.Fuchsia;
			this.AllowTransparency = true;
			
			//Work out any offset to position the background form, in the event that
			//the borders are still active
			Point screen = new Point(0, 0);
			screen = this.PointToScreen(screen);
			m_offX = screen.X - this.Location.X;
			m_offY = screen.Y - this.Location.Y;

			if (!this.DesignMode)
			{
				//Disable the form so that it cannot receive focus
				//We need to do this so that the form will not get focuse
				// by any means and then be positioned above our main form
				Point formLoc = this.Location;
				formLoc.X += m_offX;
				formLoc.Y += m_offY;
				m_layeredWnd.Text = "AlphaForm";
				m_initialised = true;
				updateLayeredBackground(this.ClientSize.Width, this.ClientSize.Height, formLoc, true);
				m_layeredWnd.Show();
				
				m_layeredWnd.Enabled = false;

				//Subclass the background window so that we can intercept its messages
				m_customLayeredWindowProc = new Win32.Win32WndProc(this.LayeredWindowWndProc);
				m_layeredWindowProc = Win32.SetWindowLong(m_layeredWnd.Handle, (uint)Win32.Message.GWL_WNDPROC, m_customLayeredWindowProc);
			}
		}

		protected override void OnPaintBackground(PaintEventArgs e)
		{
			base.OnPaintBackground(e);

			if (m_background != null)
			{
				//If we are in design mode then we can't see the blended background,
				//so we'll just draw it because we're so friendly and helpful
				if (this.DesignMode)
				{
					e.Graphics.DrawImage(m_background, 0, 0, m_background.Width, m_background.Height);
				}
				else if(!m_moving && m_renderCtrlBG)
				{
					//If desired we render a portion of the background image behind
					//each control on our form, these sections are also cut out from the
					//background image. This resolves any issues when the opacity of the 
					//form is less then 1.0 and controls would blend with the background 
					//image instead of the desktop.
					foreach (KeyValuePair<Control, bool> kvp in m_controlDict)
					{
						Control ctrl = kvp.Key;
						bool drawBack = kvp.Value;
						if (drawBack && ctrl.BackColor == Color.Transparent)
						{
							Rectangle rect = ctrl.ClientRectangle;
							rect.X = ctrl.Left;
							rect.Y = ctrl.Top;

							if(m_useBackgroundEx)
								e.Graphics.DrawImage(m_backgroundFull, rect, rect, GraphicsUnit.Pixel);
							else
								e.Graphics.DrawImage(m_background, rect, rect, GraphicsUnit.Pixel);
						}
					}
				}
			}
		}

		protected override void OnControlAdded(ControlEventArgs e)
		{
			base.OnControlAdded(e);
			if(!m_controlDict.ContainsKey(e.Control))
				m_controlDict.Add(e.Control, true);
		}

		protected override void OnControlRemoved(ControlEventArgs e)
		{
			base.OnControlRemoved(e);
			if (m_controlDict.ContainsKey(e.Control))
				m_controlDict.Remove(e.Control);
		}

		private void updateLayeredBackground(int width, int height, Point pos)
		{
			updateLayeredBackground(width, height, pos, true);
		}

		private void updateLayeredBackground(int width, int height)
		{
			updateLayeredBackground(width, height, m_layeredWnd.LayeredPos, true);
		}

		private void updateLayeredBackground(int width, int height, Point pos, bool Render)
		{
			m_useBackgroundEx = false;
			if (this.DesignMode || m_background == null || !m_initialised)
				return;

			switch (m_sizeMode)
			{
				case SizeModes.Stretch:
					m_useBackgroundEx = true;
					break;

				case SizeModes.Clip:
					//Do nothing, use the width and height provided to
					//clip the background image
					break;

				case SizeModes.None:
					//Always use the width and height of the image,
					//regardless of the size of the window
					width = m_background.Width;
					height = m_background.Height;
					break;
			}

			//Create the extended image with the approproate size
			if ((m_renderCtrlBG || m_useBackgroundEx) && Render)
			{
				if (m_backgroundEx != null)
				{
					m_backgroundEx.Dispose();
					m_backgroundEx = null;
				}
				if (m_backgroundFull != null)
				{
					m_backgroundFull.Dispose();
					m_backgroundFull = null;
				}

				if (m_sizeMode == SizeModes.Clip)
					m_backgroundEx = new Bitmap(m_background);
				else
					m_backgroundEx = new Bitmap(m_background, width, height);

				m_backgroundFull = new Bitmap(m_backgroundEx);
			}

			//Cut out portions of the alpha background that will be drawn by 
			//the main form
			if (m_renderCtrlBG)
			{
				if (Render)
				{
					Graphics g = Graphics.FromImage(m_backgroundEx);
					foreach (KeyValuePair<Control, bool> kvp in m_controlDict)
					{
						Control ctrl = kvp.Key;
						bool drawBack = kvp.Value;
						if (drawBack && ctrl.BackColor == Color.Transparent)
						{
							Rectangle rect = ctrl.ClientRectangle;
							rect.X = ctrl.Left;
							rect.Y = ctrl.Top;
							g.FillRectangle(Brushes.Fuchsia, rect);
						}
					}
					g.Dispose();
					m_backgroundEx.MakeTransparent(Color.Fuchsia);
				}
				m_useBackgroundEx = true;
			}

			byte _opacity = (byte)(this.Opacity * 255);
			if (m_useBackgroundEx)
			{
				m_layeredWnd.UpdateWindow(m_backgroundEx, _opacity, width, height, pos);
			}
			else
			{
				m_layeredWnd.UpdateWindow(m_background, _opacity, width, height, pos);
			}
		}

		private void updateLayeredSize(int width, int height)
		{
			updateLayeredSize(width, height, false);
		}

		private void updateLayeredSize(int width, int height, bool forceUpdate)
		{
			//The size hasn't changed, no need to do anything
			if (!m_initialised)
				return;

			if (!forceUpdate && (width == m_layeredWnd.LayeredSize.Width && height == m_layeredWnd.LayeredSize.Height))
				return;

			switch (m_sizeMode)
			{
				case SizeModes.None:
					break;

				case SizeModes.Stretch:
					{
						updateLayeredBackground(width, height);
						this.Invalidate(false);
					}
					break;

				case SizeModes.Clip:
					{
						//No need to modify any images, just set the new size
						byte _opacity = (byte)(this.Opacity * 255);
						if (m_useBackgroundEx)
						{
							m_layeredWnd.UpdateWindow(m_backgroundEx, _opacity, width, height, m_layeredWnd.LayeredPos);
						}
						else
						{
							m_layeredWnd.UpdateWindow(m_background, _opacity, width, height, m_layeredWnd.LayeredPos);
						}
					}
					break;
			}
		}

		private Bitmap m_background;
		private Bitmap m_backgroundEx;
		private Bitmap m_backgroundFull;
		private bool m_useBackgroundEx;
		private LayeredWindow m_layeredWnd;
		private int m_offX;
		private int m_offY;
		private bool m_renderCtrlBG;
		private bool m_enhanced;
		private SizeModes m_sizeMode;
		private List<Control> m_hiddenControls;
		private Dictionary<Control, bool> m_controlDict;
		private bool m_moving;
		private bool m_initialised;

		private Win32.Win32WndProc m_customLayeredWindowProc;
		private IntPtr m_layeredWindowProc;

		//Mouse
		private Point m_lockedPoint = new Point();
		private DateTime m_clickTime = DateTime.Now;
		private Win32.Message m_lastClickMsg = 0;
		private HeldButtons m_isDown;

		//Event Delegates
		private delegate void delMouseEvent(MouseEventArgs e);
		private delegate void delStdEvent(EventArgs e);

		struct HeldButtons
		{
			public bool Left;
			public bool Middle;
			public bool Right;
			public bool XBtn;
		};
	}
}
