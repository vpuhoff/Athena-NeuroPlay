using System;
using System.Drawing;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.Diagnostics;

namespace AlphaForms
{
	public partial class AlphaForm
	{
		/// <summary>
		/// Simple function to determine if the user has done a double-click
		/// </summary>
		/// <param name="pos">The position of the mouse</param>
		/// <returns></returns>
		private bool dblClick(Point pos)
		{
			TimeSpan elapsed = DateTime.Now - m_clickTime;
			Size dist = new Size();
			dist.Width = Math.Abs(m_lockedPoint.X - pos.X);
			dist.Height = Math.Abs(m_lockedPoint.Y - pos.Y);

			if (elapsed.Milliseconds <= SystemInformation.DoubleClickTime
				&& dist.Width <= SystemInformation.DoubleClickSize.Width
				&& dist.Height <= SystemInformation.DoubleClickSize.Height)
			{
				return true;
			}

			return false;
		}

		/// <summary>
		/// Override for the main forms WndProc method
		/// This is used to intercept the forms movement so that we can move
		/// blended background form at the same time, as well as check for 
		/// when the form is activated so we can ensure the Z-order of our
		/// forms remains intact.
		/// </summary>
		/// <param name="m">Windows Message</param>
        ///
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
		protected override void WndProc(ref Message m)
		{
			if (this.DesignMode)
			{
				base.WndProc(ref m);
				return;
			}

			Win32.Message msgId = (Win32.Message)m.Msg;
			bool UseBase = true;
			switch (msgId)
			{
				case Win32.Message.WM_LBUTTONUP:
					{
						//Just in case
						if (Win32.GetCapture() != IntPtr.Zero)
							Win32.ReleaseCapture();
					}
					break;

				case Win32.Message.WM_ENTERSIZEMOVE:
					{

					}
					break;

				case Win32.Message.WM_EXITSIZEMOVE:
					{
						//We've stopped dragging the form, so lets make sure that our values are correct
						m_isDown.Left = false;
						m_moving = false;

						if (m_enhanced)
						{
							this.SuspendLayout();
							foreach (Control ctrl in m_hiddenControls)
								ctrl.Visible = true;
							m_hiddenControls.Clear();
							this.ResumeLayout();
							updateLayeredBackground(this.ClientSize.Width, this.ClientSize.Height, m_layeredWnd.LayeredPos, false);
						}
					}
					break;

				case Win32.Message.WM_MOUSEMOVE:
					//It's unlikely that we will get here unless this we really have captured the mouse
					//because the entire thing is transparent, but we check anyway just to make sure
					if (Win32.GetCapture() != IntPtr.Zero && m_moving)
					{
						//In enhanced mode we draw the main window to the layered window and then hide the main
						//window. This is so that we can have perfectly smooth motion when dragging the form, as
						//we cannot gurantee that the forms will ever be moved together otherwise
						if (m_enhanced)
						{
							//Setup the device contexts we are going to use
							IntPtr hdcScreen = Win32.GetWindowDC(m_layeredWnd.Handle);	//Screen DC that the layered window will draw to
							IntPtr windowDC = Win32.GetDC(this.Handle);					//Window DC that we are going to copy
							IntPtr memDC = Win32.CreateCompatibleDC(windowDC);			//Temporary DC that we draw to
							IntPtr BmpMask = Win32.CreateBitmap(this.ClientSize.Width, 
											this.ClientSize.Height, 1, 1, IntPtr.Zero);	//Mask bitmap so that we only draw areas of the form that are visible

							Bitmap backImage = m_useBackgroundEx ? m_backgroundFull : m_background;
							IntPtr BmpBack = backImage.GetHbitmap(Color.FromArgb(0));	//Background Image

							//Create mask
							Win32.SelectObject(memDC, BmpMask);
							uint oldCol = Win32.SetBkColor(windowDC, 0x00FF00FF);
							Win32.BitBlt(memDC, 0, 0, this.ClientSize.Width, this.ClientSize.Height, windowDC, 0, 0, Win32.TernaryRasterOperations.SRCCOPY);
							Win32.SetBkColor(windowDC, oldCol);

							//Blit window to background image using mask
							//We need to use the SPno raster operation with a white brush to combine our window
							//with a black backround before putting it onto the 32-bit background image, otherwise
							//we end up with blending issues (source and destination colours are ANDed together)
							Win32.SelectObject(memDC, BmpBack);
							IntPtr brush = Win32.CreateSolidBrush(0x00FFFFFF);
							Win32.SelectObject(memDC, brush);
							Win32.MaskBlt(memDC, 0, 0, backImage.Width, backImage.Height, windowDC, 0, 0, BmpMask, 0, 0, 0xCFAA0020);
							//Win32.BitBlt(memDC, 0, 0, backImage.Width, backImage.Height, windowDC, m_offX, m_offY, Win32.TernaryRasterOperations.SRCCOPY);

							Point zero = new Point(0, 0);
							Size size = m_layeredWnd.LayeredSize;
							Point pos = m_layeredWnd.LayeredPos;
							Win32.BLENDFUNCTION blend = new Win32.BLENDFUNCTION();
							blend.AlphaFormat = (byte)Win32.BlendOps.AC_SRC_ALPHA;
							blend.BlendFlags = (byte)Win32.BlendFlags.None;
							blend.BlendOp = (byte)Win32.BlendOps.AC_SRC_OVER;
							blend.SourceConstantAlpha = (byte)(this.Opacity * 255);

							Win32.UpdateLayeredWindow(m_layeredWnd.Handle, hdcScreen, ref pos, ref size, memDC, ref zero, 0, ref blend, Win32.BlendFlags.ULW_ALPHA);

							//Clean up
							Win32.ReleaseDC(IntPtr.Zero, hdcScreen);
							Win32.ReleaseDC(this.Handle, windowDC);
							Win32.DeleteDC(memDC);
							Win32.DeleteObject(brush);
							Win32.DeleteObject(BmpMask);
							Win32.DeleteObject(BmpBack);

							//Hide controls that are visible
							this.SuspendLayout();
							foreach (Control ctrl in this.Controls) {
								if (ctrl.Visible) {
									m_hiddenControls.Add(ctrl);
									ctrl.Visible = false;
								}
							}
							this.ResumeLayout();
						}

						//If we do not release the mouse then Windows will not start dragging the form, also
						//it will mess up mouse input to any border on our form and other windows on the desktop
						Win32.ReleaseCapture();
						Win32.SendMessage(this.Handle, (int)Win32.Message.WM_NCLBUTTONDOWN, (int)Win32.Message.HTCAPTION, 0);

					}
					break;

				case Win32.Message.WM_SIZE:
					{
						//The updateLayeredSize function will check the width and height
						//we pass in, so we don't need to worry about updating the window
						//with the same size it already had
						int width = m.LParam.ToInt32() & 0xFFFF;
						int height = m.LParam.ToInt32() >> 16;
						this.updateLayeredSize(width, height);
					}
					break;

				case Win32.Message.WM_WINDOWPOSCHANGING:
					{
						Win32.WINDOWPOS posInfo = (Win32.WINDOWPOS)Marshal.PtrToStructure(m.LParam, typeof(Win32.WINDOWPOS));

						//We will cancel this movement, and send our own messages to position both forms
						//this way we can ensure that both forms are moved together and that
						//the Z order is unchanged.
						Win32.WindowPosFlags move_size = Win32.WindowPosFlags.SWP_NOMOVE | Win32.WindowPosFlags.SWP_NOSIZE;
						if ((posInfo.flags & move_size) != move_size)
						{
							//Check for my own messages, which I do by setting to hwndInsertAfter to our 
							//own window, which from what I can gather only happens when you resize your
							//window, never when you move it
							if (posInfo.hwndInsertAfter != this.Handle)
							{
								IntPtr hwdp = Win32.BeginDeferWindowPos(2);
								if (hwdp != IntPtr.Zero)
									hwdp = Win32.DeferWindowPos(hwdp, m_layeredWnd.Handle, this.Handle, posInfo.x + m_offX, posInfo.y + m_offY, 
											0, 0, (uint)(posInfo.flags | Win32.WindowPosFlags.SWP_NOSIZE | Win32.WindowPosFlags.SWP_NOZORDER));
								if (hwdp != IntPtr.Zero)
									hwdp = Win32.DeferWindowPos(hwdp, this.Handle, this.Handle, posInfo.x, posInfo.y, posInfo.cx, posInfo.cy, 
											(uint)(posInfo.flags | Win32.WindowPosFlags.SWP_NOZORDER));
								if (hwdp != IntPtr.Zero)
									Win32.EndDeferWindowPos(hwdp);

								m_layeredWnd.LayeredPos = new Point(posInfo.x + m_offX, posInfo.y + m_offY);

								//Update the flags so that the form will not move with this message
								posInfo.flags |= Win32.WindowPosFlags.SWP_NOMOVE;
								Marshal.StructureToPtr(posInfo, m.LParam, true);
							}

							if ((posInfo.flags & Win32.WindowPosFlags.SWP_NOSIZE) == 0)
							{
								//Form was also resized
								int diffX = posInfo.cx - this.Size.Width;
								int diffY = posInfo.cy - this.Size.Height;
								if(diffX != 0 || diffY != 0)
									this.updateLayeredSize(this.ClientSize.Width + diffX, this.ClientSize.Height + diffY);
							}

							UseBase = false;
						}
					}
					break;


				case Win32.Message.WM_ACTIVATE:
					{
						//If WParam is Zero then the form is deactivating and we don't need to do anything
						//Otherwise we need to make sure that the background form is positioned just behind
						//the main form.
						if (m.WParam != IntPtr.Zero)
						{
							//Check for any visible windows between the background and main windows
							IntPtr prevWnd = Win32.GetWindow(m_layeredWnd.Handle, Win32.GetWindow_Cmd.GW_HWNDPREV);
							while (prevWnd != IntPtr.Zero)
							{
								//If we find a visiable window, we stop
								if (Win32.IsWindowVisible(prevWnd))
									break;

								prevWnd = Win32.GetWindow(prevWnd, Win32.GetWindow_Cmd.GW_HWNDPREV);
							}

							//If the visible window isn't ours reset the position of the background form
							if (prevWnd != this.Handle)
								Win32.SetWindowPos(m_layeredWnd.Handle, this.Handle, 0, 0, 0, 0, (uint)(Win32.WindowPosFlags.SWP_NOSENDCHANGING | Win32.WindowPosFlags.SWP_NOACTIVATE | Win32.WindowPosFlags.SWP_NOSIZE | Win32.WindowPosFlags.SWP_NOMOVE));
						}
					}
					break;
			}

			if (UseBase)
				base.WndProc(ref m);
		}

		/// <summary>
		/// Used to intercept windows messages for our background form so that we can send events
		/// back to the main form. Because the background form is Disabled (so that it cannot receive focus)
		/// we only actually get one message WM_SETCURSOR, which actually gives us all the information on
		/// what the mouse is doing at the time, so we can use that to fire off events on the main form that
		/// we are missing out on because its background is transparent.
		/// </summary>
		/// <param name="hWnd"></param>
		/// <param name="Msg"></param>
		/// <param name="wParam"></param>
		/// <param name="lParam"></param>
		/// <returns></returns>
		private int LayeredWindowWndProc(IntPtr hWnd, int Msg, int wParam, int lParam)
		{
			Point mousePos = this.PointToClient(System.Windows.Forms.Cursor.Position);
			Win32.Message msgId = (Win32.Message)Msg;
			switch (msgId)
			{
				case Win32.Message.WM_LBUTTONDOWN:
					System.Diagnostics.Debugger.Break();

					break;
				case Win32.Message.WM_SETCURSOR:
					{
						//Set the cursor, we need to do this ourselves because we are not letting this message through
						Win32.SetCursor(Win32.LoadCursor(IntPtr.Zero, Win32.SystemCursor.IDC_NORMAL));

						
						MouseEventArgs e = null;
						delMouseEvent mouseEvent = null;
						delStdEvent stdEvent = null;

						//The low word of the lParam contains the hit test code, which we don't
						//need to know, we only need to know what the mouse is doing
						Win32.Message MouseEvent = (Win32.Message)(lParam >> 16);

						switch (MouseEvent)
						{
							case Win32.Message.WM_MOUSEMOVE:
								{
									if (m_isDown.Left && m_lockedPoint != mousePos)
									{
										//We are using the trick of sending the WM_NCLBUTTONDOWN message to make Windows drag our form
										//around, I'm not entirely certain how Windows works but our main form needs to have been the last
										//window with mouse capture for it to work, even thought it is necessary to ReleaseCapture prior
										//to sending the message
										System.Diagnostics.Debug.WriteLine("Setting Capture");
										Win32.SetCapture(this.Handle);
										m_moving = true;
									}
									else
									{
										e = new MouseEventArgs(System.Windows.Forms.MouseButtons.None, 0, mousePos.X, mousePos.Y, 0);
										mouseEvent = new delMouseEvent(this.OnMouseMove);
									}
								}
								break;

							case Win32.Message.WM_LBUTTONDOWN:
								if (m_lastClickMsg == MouseEvent && !m_isDown.Left && dblClick(mousePos))
								{
									e = new MouseEventArgs(System.Windows.Forms.MouseButtons.Left, 2, mousePos.X, mousePos.Y, 0);
									mouseEvent = new delMouseEvent(this.OnMouseDoubleClick);
									stdEvent = new delStdEvent(this.OnDoubleClick);
									
									m_lastClickMsg = 0;
								}
								else if (!m_isDown.Left)
								{
									e = new MouseEventArgs(System.Windows.Forms.MouseButtons.Left, 1, mousePos.X, mousePos.Y, 0);
									mouseEvent = new delMouseEvent(this.OnMouseDown);
									m_lastClickMsg = MouseEvent;
								}

								m_clickTime = DateTime.Now;
								m_lockedPoint = mousePos;

								m_isDown.Left = true;
								break;

							case Win32.Message.WM_LBUTTONUP:
								{
									e = new MouseEventArgs(System.Windows.Forms.MouseButtons.Left, 1, mousePos.X, mousePos.Y, 0);
									mouseEvent = new delMouseEvent(this.OnMouseClick);
									stdEvent = new delStdEvent(this.OnClick);
									m_isDown.Left = false;
								}
								break;

							case Win32.Message.WM_MBUTTONDOWN:
								if (m_lastClickMsg == MouseEvent && !m_isDown.Middle && dblClick(mousePos))
								{
									e = new MouseEventArgs(System.Windows.Forms.MouseButtons.Middle, 2, mousePos.X, mousePos.Y, 0);
									mouseEvent = new delMouseEvent(this.OnMouseDoubleClick);
									stdEvent = new delStdEvent(this.OnDoubleClick);
									m_lastClickMsg = 0;
								}
								else if (!m_isDown.Middle)
								{
									e = new MouseEventArgs(System.Windows.Forms.MouseButtons.Middle, 1, mousePos.X, mousePos.Y, 0);
									mouseEvent = new delMouseEvent(this.OnMouseDown);

									m_lastClickMsg = MouseEvent;
									m_clickTime = DateTime.Now;
									m_lockedPoint = mousePos;
								}
								m_isDown.Middle = true;
								break;

							case Win32.Message.WM_MBUTTONUP:
								{
									e = new MouseEventArgs(System.Windows.Forms.MouseButtons.Middle, 1, mousePos.X, mousePos.Y, 0);
									mouseEvent = new delMouseEvent(this.OnMouseClick);
									stdEvent = new delStdEvent(this.OnClick);

									m_isDown.Middle = false;
								}
								break;

							case Win32.Message.WM_RBUTTONDOWN:
								if (m_lastClickMsg == MouseEvent && !m_isDown.Right && dblClick(mousePos))
								{
									e = new MouseEventArgs(System.Windows.Forms.MouseButtons.Right, 2, mousePos.X, mousePos.Y, 0);
									mouseEvent = new delMouseEvent(this.OnMouseDoubleClick);
									stdEvent = new delStdEvent(this.OnDoubleClick);

									m_lastClickMsg = 0;
								}
								else if (!m_isDown.Right)
								{
									e = new MouseEventArgs(System.Windows.Forms.MouseButtons.Right, 1, mousePos.X, mousePos.Y, 0);
									mouseEvent = new delMouseEvent(this.OnMouseDown);

									m_lastClickMsg = MouseEvent;
									m_clickTime = DateTime.Now;
									m_lockedPoint = mousePos;
								}
								m_isDown.Right = true;
								break;

							case Win32.Message.WM_RBUTTONUP:
								{
									e = new MouseEventArgs(System.Windows.Forms.MouseButtons.Right, 1, mousePos.X, mousePos.Y, 0);
									mouseEvent = new delMouseEvent(this.OnMouseClick);
									stdEvent = new delStdEvent(this.OnClick);

									m_isDown.Right = false;
								}
								break;

							case Win32.Message.WM_XBUTTONDOWN:
								if (m_lastClickMsg == MouseEvent && !m_isDown.XBtn && dblClick(mousePos))
								{
									e = new MouseEventArgs(System.Windows.Forms.MouseButtons.XButton1, 2, mousePos.X, mousePos.Y, 0);
									mouseEvent = new delMouseEvent(this.OnMouseDoubleClick);
									stdEvent = new delStdEvent(this.OnDoubleClick);

									m_lastClickMsg = 0;
								}
								else if (!m_isDown.XBtn)
								{
									e = new MouseEventArgs(System.Windows.Forms.MouseButtons.XButton1, 1, mousePos.X, mousePos.Y, 0);
									mouseEvent = new delMouseEvent(this.OnMouseDown);

									m_lastClickMsg = MouseEvent;
									m_clickTime = DateTime.Now;
									m_lockedPoint = mousePos;
								}
								m_isDown.XBtn = true;
								break;

							case Win32.Message.WM_XBUTTONUP:
								{
									e = new MouseEventArgs(System.Windows.Forms.MouseButtons.XButton1, 1, mousePos.X, mousePos.Y, 0);
									mouseEvent = new delMouseEvent(this.OnMouseClick);
									stdEvent = new delStdEvent(this.OnClick);

									m_isDown.XBtn = false;
								}
								break;
						}

						//Check if the form is being clicked, but is not active
						bool mouseDown = m_isDown.Left || m_isDown.Middle || m_isDown.Right || m_isDown.XBtn;
						if (mouseDown && Form.ActiveForm == null)
						{
							//We need to give our form focus
							this.Activate();
						}

						if (e != null)
						{
							if (mouseEvent != null)
								this.BeginInvoke(mouseEvent, e);
							if (stdEvent != null)
								this.BeginInvoke(stdEvent, e);
						}

						return 0;
					}

			}
			return Win32.CallWindowProc(m_layeredWindowProc, hWnd, Msg, wParam, lParam);
		}
	}
}