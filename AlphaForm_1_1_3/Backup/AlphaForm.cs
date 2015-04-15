#region Copyright (c) Jeff Anderson
/*
Copyright (c) 2009 Jeff Anderson of Braid Art Labs (www.groboto.com)

This code is released under the Code Project
Open License 1.02. See cpol.html for
license details. NOTE: cpol.html MUST be included
with any source code distribution of this code.                                                              
*/
#endregion

#region Version 1.1.3

// Version 1.0 Initial CodeProject Release
// Version 1.1 Initial Commercial Release
//  1. Added AlphaFormTransformer.Fade() for fade in/out functionality
//     To use, override OnShown() and OnClosing() and add call to Fade().
//     If fading in, main form Opacity property should be set to 0
//     in advance AND for fading in you must pass 0 as the
//     argument to TransformForm() to set the initial layered form opacity.
//     See the examples "TestFormGrobotoTV" or "ControlsOnTopOfSkin" projects
//     as they both use Fade().
//  2. Reworked OnPaint override in AlphaFormTransformer (see method summary
//     & PowerToys issue)
//  3. Added CanDrag property. If true user can click and drag on
//     background of AFT to drag the composite form (see property summary
//     for more details)
//  4. Added example project "ControlsOnTopOfSkin" showing how to create
//     a form where the controls appear on top of the background image
//     (uses the new CanDrag property). The basic idea is to set the 
//     AFT's background image to one where the alpha channel cuts out
//     the control region, and then set the main form's background to the
//     same image without the cutout region.
//  5. Fixed issues with main form scaling set to DPI and running under
//     different DPI's. Added scaling of main form background image (which
//     becomes the AFT's background at runtime).
//     We also adjust the region fillBorder for the AFM markers if DPI scaling
//     is needed. This is necessary as image rescaling adds or removes to the 
//     semitransparent border width of the alpha channel.
//  6. Reorganized solution layout with all sample projects in "Test Projects" 
//     folder.
// Version 1.1.1 - CodeProject License Release
//  1. Changed primary namespace and refactored. 
// Version 1.1.2 (beta)
//  1. Fixed SetWindowLong PInvoke signature to be 64 bit compatible, but
//     we have not been able to test this component on X64. 
//  2. Impulse class made public as it's useful for dampened motion if the user
//     wants to animate (either the location or opacity) an alpha transformed window.
//     For example see the SlidingPanes project. 
// Version 1.1.3 (beta)
//  1. Implementing a Visible change event on the main form and keeping the layered
//     window Visible propery in sync was a bad idea. In this component we need to manipulate
//     Visible separately for the fade routine so we removed it. 
//  2. Added SetAlphaAndParentFormVisible and SetAlphaAndParentFormOpacity utility routines.
//  3. Re-worked the SlidingPanes example project to better illustrate animating the drawer and bug fix.
#endregion

using System;
using System.Runtime.InteropServices;
using System.ComponentModel;
using System.Text;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Windows.Forms;
using System.Collections;
using System.Diagnostics.CodeAnalysis;


/// <summary>
/// Imported methods and structs
/// </summary>
internal class Win32
{
	[StructLayout(LayoutKind.Sequential)]
	public struct SIZE
	{
		public Int32 cx;
		public Int32 cy;

		public SIZE(Int32 x, Int32 y) 
		{ 
			cx = x; 
			cy = y; 
		}
	}

	[StructLayout(LayoutKind.Sequential, Pack = 1)]
	public struct BLENDFUNCTION
	{
		public byte BlendOp;
		public byte BlendFlags;
		public byte SourceConstantAlpha;
		public byte AlphaFormat;
	}

	[StructLayout(LayoutKind.Sequential)]
	public struct POINT
	{
		public Int32 x;
		public Int32 y;

		public POINT(Int32 x, Int32 y) 
		{ 
			this.x = x; 
			this.y = y; 
		}
	}
	
	[StructLayout(LayoutKind.Sequential, Pack = 1)]
	public struct COLORREF 
	{ 
		public byte R;   
		public byte G;   
		public byte B;   
		public byte A;   
	}   
	
	[StructLayout(LayoutKind.Sequential)]
	public struct RECT
	{
	  public int left;
	  public int top;
	  public int right;
	  public int bottom;
	}

	public const byte AC_SRC_OVER = 0;
	public const Int32 ULW_ALPHA = 2;
	public const byte AC_SRC_ALPHA = 1;

	[DllImport("gdi32.dll", ExactSpelling = true, SetLastError = true)]
	public static extern IntPtr CreateCompatibleDC(IntPtr hDC);

	[DllImport("user32.dll", ExactSpelling = true, SetLastError = true)]
	public static extern IntPtr GetDC(IntPtr hWnd);

	[DllImport("gdi32.dll", ExactSpelling = true)]
	public static extern IntPtr SelectObject(IntPtr hDC, IntPtr hObj);

	[DllImport("user32.dll", ExactSpelling = true)]
	public static extern int ReleaseDC(IntPtr hWnd, IntPtr hDC);

	[DllImport("gdi32.dll", ExactSpelling = true, SetLastError = true)]
	public static extern int DeleteDC(IntPtr hDC);

	[DllImport("gdi32.dll", ExactSpelling = true, SetLastError = true)]
	public static extern int DeleteObject(IntPtr hObj);

	[DllImport("user32.dll", ExactSpelling = true, SetLastError = true)]
	public static extern int UpdateLayeredWindow(IntPtr hwnd, IntPtr hdcDst, ref POINT pptDst, ref SIZE psize, IntPtr hdcSrc, ref POINT pptSrc, Int32 crKey, ref BLENDFUNCTION pblend, Int32 dwFlags);

	[DllImport("gdi32.dll", ExactSpelling = true, SetLastError = true)]
	public static extern IntPtr ExtCreateRegion(IntPtr lpXform, uint nCount, IntPtr rgnData);

	[DllImport("user32", CharSet=CharSet.Ansi, SetLastError=true, ExactSpelling=true)]
  public static extern int SetLayeredWindowAttributes(IntPtr Handle, int crKey, byte bAlpha, int dwFlags);
  
  [DllImport("user32", CharSet=CharSet.Ansi, SetLastError=true, ExactSpelling=true)] 
	public static extern uint GetLayeredWindowAttributes (IntPtr hwnd, out COLORREF pcrKey, out byte pbAlpha, out uint pwdFlags);

  [DllImport("user32.dll")]
  public static extern IntPtr GetDesktopWindow();

  [DllImport("user32", CharSet=CharSet.Ansi, SetLastError=true, ExactSpelling=true)] 
  public static extern int InvalidateRgn(IntPtr hWnd, IntPtr hRgn, int bErase);
  
  [DllImport("user32", CharSet=CharSet.Ansi, SetLastError=true, ExactSpelling=true)] 
  public static extern int ValidateRgn(IntPtr hWnd, IntPtr hRgn);
  
  [DllImport("user32", CharSet=CharSet.Ansi, SetLastError=true, ExactSpelling=true)] 
  public static extern int ValidateRect(IntPtr hWnd, ref RECT rct);

  
  public static IntPtr SetWindowLong(IntPtr hWnd, int nIndex, IntPtr dwNewLong)
  {
  if (IntPtr.Size == 4)
  {
  return SetWindowLongPtr32(hWnd, nIndex, dwNewLong);
  }
  return SetWindowLongPtr64(hWnd, nIndex, dwNewLong);
  }

  [SuppressMessage("Microsoft.Portability", "CA1901:PInvokeDeclarationsShouldBePortable")]
  [DllImport("user32", EntryPoint = "SetWindowLong")]
  public static extern IntPtr SetWindowLongPtr32(IntPtr hWnd, int nIndex, IntPtr dwNewLong);

  [SuppressMessage("Microsoft.Interoperability", "CA1400:PInvokeEntryPointsShouldExist")]
  [DllImport("user32", EntryPoint = "SetWindowLongPtr")]
  public static extern IntPtr SetWindowLongPtr64(IntPtr hWnd, int nIndex, IntPtr dwNewLong);

  public static IntPtr SetWindowLong(IntPtr hWnd, short nIndex, IntPtr dwNewLong)
  {
    if (IntPtr.Size == 4)
    {
    return SetWindowLongPtr32(hWnd, nIndex, dwNewLong);
    }
    return SetWindowLongPtr64(hWnd, (int)nIndex, dwNewLong);
  }

  public static IntPtr GetWindowLong(IntPtr hWnd, int nIndex)
  {
    if (IntPtr.Size == 4)
    {
      return GetWindowLong32(hWnd, nIndex);
    }
    return GetWindowLongPtr64(hWnd, nIndex);
  }

  [DllImport("user32", EntryPoint = "GetWindowLong")]
  public static extern IntPtr GetWindowLong32(IntPtr hWnd, int nIndex);

  [SuppressMessage("Microsoft.Interoperability", "CA1400:PInvokeEntryPointsShouldExist")]
  [DllImport("user32", EntryPoint = "GetWindowLongPtr")]
  public static extern IntPtr GetWindowLongPtr64(IntPtr hWnd, int nIndex);

}

/// <summary>
/// Defines a line segment. Used by AlphaFormTransformer.UpdateRectListFromAlpha()
/// </summary>
internal struct LineSeg
{
	public int x1;
	public int x2;
	public int y;
	public int dy;
		
	public LineSeg(int x1,int x2,int y,int dy)
	{
		this.x1 = x1;
		this.x2 = x2;
		this.y = y;
		this.dy = dy;
	}
}


/// <summary>
/// A simple pulse function with exponential dampening
/// input [0..1], output [0..1]
/// Function is steep at beginning and flat at end for Dampening > 2
/// and lesser values not really useful
/// The greater the dampening the steeper the initial ramp
/// If you want it flat at the beginning then call 1.0 - Evaluate(1.0-t)
/// </summary>
public class Impulse {

	#region Class Variables
	double m_pScale;
	double m_pNorm = 1.0;
	#endregion

	#region Contructors
	public Impulse(double s) 
	{
		m_pNorm = 1.0;
		m_pScale = s;
	}
	#endregion
	
	#region Properties
	public double Dampening
	{
		get
		{
			return m_pScale;
		}
		set
		{
			m_pScale = value;
		}
	}
	#endregion
	
	#region Methods
	void UpdateScale()
	{
		m_pNorm = 1.0/EvalInternal(1.0);
	}
	
	double EvalInternal(double t) 
	{
		double val;
			
		t = t * m_pScale;
		if (t < 1.0) 
		{
			val = t - (1 - Math.Exp(-t));
		} 
		else 
		{
			double start = Math.Exp(-1.0);
			t -= 1.0;
			double expx = 1 - Math.Exp(-t);
			val = start + (expx * (1.0 - start));
		}

		return val * m_pNorm;
	}
		
	public void Reset()
	{
		m_pNorm = 1.0;
	}
	
	public double Evaluate(double t) 
	{
		if (t >= 1.0) 
			return 1.0;
		if (t <= 0.0) 
			return 0.0;
		if (m_pNorm == 1.0)
			UpdateScale();
		return EvalInternal(t);
	}
		
	#endregion		
		
}			

/// <summary>
/// This window sets the WS_EX_LAYERED extended style with
/// a method to set the layered window bitmap
/// </summary>
internal class LayeredWindowForm : Form
{
	
	#region Constructors
	public LayeredWindowForm()
	{
		FormBorderStyle = FormBorderStyle.None;
	}
	#endregion
	
	private 
		object theobj;
		
	#region Overrides
	/// <summary>
	/// This window can be the active window when you click
	/// to drag it, and it can receive a close event from
	/// the system (e.g., user clicks Alt+F4), therefore we 
	/// instruct the owner to close and cancel the close for
	/// this window.
	/// </summary>
	/// 
	protected void tstlock()
	{
		lock(theobj) {
		 theobj = null;
		}
	
	
	}
  protected override void OnClosing(CancelEventArgs e)
  {
    if (Owner != null)
    {
      e.Cancel = true;
      base.OnClosing(e);
      Owner.Close();
    }
    else
      base.OnClosing(e);
  }
	
	protected override void OnHandleCreated(EventArgs e)
  {
    InitializeStyles();
    base.OnHandleCreated(e);
  }

	protected override CreateParams CreateParams
	{
		get
		{
			CreateParams cParms = base.CreateParams;
      cParms.ExStyle |= 0x00080000; // WS_EX_LAYERED
			return cParms;
		}
	}
	#endregion
	
	#region Methods
	public void SetBits(Bitmap bitmap, byte opacity)
	{
		if (!Bitmap.IsCanonicalPixelFormat(bitmap.PixelFormat) || !Bitmap.IsAlphaPixelFormat(bitmap.PixelFormat))
			throw new ApplicationException("The bitmap must be 32 bits per pixel with an alpha channel.");
			
		IntPtr oldBits = IntPtr.Zero;
		IntPtr screenDC = Win32.GetDC(IntPtr.Zero);
		IntPtr hBitmap = IntPtr.Zero;
		IntPtr memDc = Win32.CreateCompatibleDC(screenDC);
		Win32.POINT topLoc = new Win32.POINT(Left, Top);
		Win32.SIZE bitMapSize = new Win32.SIZE(bitmap.Width, bitmap.Height);
		Win32.BLENDFUNCTION blendFunc = new Win32.BLENDFUNCTION();
		Win32.POINT srcLoc = new Win32.POINT(0, 0);
    blendFunc.BlendOp = Win32.AC_SRC_OVER;
    blendFunc.SourceConstantAlpha = opacity;
    blendFunc.AlphaFormat = Win32.AC_SRC_ALPHA;
    blendFunc.BlendFlags = 0;

    try
    {
			hBitmap = bitmap.GetHbitmap(Color.FromArgb(0));
			oldBits = Win32.SelectObject(memDc, hBitmap);
			Win32.UpdateLayeredWindow(Handle, screenDC, ref topLoc, ref bitMapSize, memDc, ref srcLoc, 0, ref blendFunc, Win32.ULW_ALPHA);
		}
		finally
		{
			if (hBitmap != IntPtr.Zero)
			{
				Win32.SelectObject(memDc, oldBits);
				Win32.DeleteObject(hBitmap);
			}
			Win32.ReleaseDC(IntPtr.Zero, screenDC);
			Win32.DeleteDC(memDc);
		}
	}
	
  private void InitializeStyles()
  {
		SetStyle(ControlStyles.AllPaintingInWmPaint, true);
		SetStyle(ControlStyles.UserPaint, true);
		UpdateStyles();
	}
	#endregion
	
}


namespace AlphaForm
{

  /// <summary>
  /// Fade type
  /// </summary>
  public enum FadeType
  {
    /// <summary>
    /// Fade from 0% to 100% opacity
    /// </summary>
    FadeIn,
    /// <summary>
    /// Fade from 100% to 0% opacity
    /// </summary>
    FadeOut
  };
  
  
	/// <summary>
	/// AlphaFormMarker serves as a design time aid for specifying 
	/// one or more points in the background image that will be used
	/// to build the main form's Region. In the designer, it must
	/// always be added to the AlphaFormTransformer control (not the main form)
  /// </summary>
  /// <remarks>
  /// At runtime all instances of AlphaFormMarker are made invisible.
  /// </remarks>
  [ToolboxBitmap(typeof(AlphaFormMarker),"marker.png")]
	public class AlphaFormMarker : UserControl
	{
		#region Class Variables
		uint m_fillBorder = 4;
		#endregion
		
		#region Constructors
    /// <summary>
    /// Constructor
    /// </summary>
		public AlphaFormMarker()
		{
			Bounds = new Rectangle(Location, new Size(17, 17));
		}
		#endregion
		
		#region Overrides
    /// <summary>
    /// Overriden for internal purposes
    /// </summary>
		protected override void OnHandleCreated(EventArgs e)
		{
			InitializeStyles();
			base.OnHandleCreated(e);
		}

    /// <summary>
    /// Draws the cross-hairs
    /// </summary>
		protected override void OnPaint(PaintEventArgs e)
		{
			e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
			SolidBrush sb = new SolidBrush(Color.FromArgb(255, 255, 0, 0));
			Pen p = new Pen(sb, 1);
			e.Graphics.DrawEllipse(p, new Rectangle(0, 0, ClientSize.Width - 1, ClientSize.Height - 1));
			e.Graphics.DrawLine(p, Bounds.Width / 2, 0, Bounds.Width / 2, Bounds.Height);
			e.Graphics.DrawLine(p, 0, Bounds.Height / 2, Bounds.Width, Bounds.Height / 2);
			p.Dispose();
			sb.Dispose();
		}
		#endregion
		
		#region Properties
    /// <summary>
    /// The marker's FillBorder property specifies how far into the non-transparent pixel 
    /// border the region will be constructed. The composited image will typically have
    /// semitransparent edges. Therefore the region that's built from the marker position needs
    /// to expand some number of pixels into (and *under* from the compositing point of view)
    /// the semitransparent area, otherwise you will see through to the desktop along these borders.
    /// You want the border value to be large enough to cover the thickness of the semitransparent
    /// edge (typically a couple pixels), but not too large which might cause it to extend past
    /// the other side of the image frame.
    /// </summary>
    /// <value>Sets or gets pixel region fill border for this marker</value>
		[Category("Marker Properties"), Description("Fill Border (Pixels)")]
		[DefaultValue(4)]
		public uint FillBorder
		{
			get
			{
				return m_fillBorder;
			}
			set
			{
				m_fillBorder = value;
			}
		}
		#endregion
		
		#region Methods
		void InitializeStyles()
		{
		  SetStyle(ControlStyles.AllPaintingInWmPaint, true);
		  SetStyle(ControlStyles.UserPaint, true);
		  UpdateStyles();
		}
		#endregion
		
	}

	/// <summary>
	/// A single instance of AlphaFormTransformer is added to the form
	/// to be transformed to an alpha channel border window. It hosts all
	/// controls to be displayed on the window, the image for the layered
	/// window that displays the image border, and one or more AlphaFormMarkers
	/// to construct the main form's region. 
	/// </summary> 
  [ToolboxBitmap(typeof(AlphaFormTransformer), "Transformer.png")]
	public class AlphaFormTransformer : Panel
	{
		#region Class Variables
		bool m_drag = false;
		Point m_dragStart;
		LayeredWindowForm m_lwin = null;
		Bitmap m_alphaBitmap = null;
		uint m_dragSleep = 30;
		bool m_fading = false;
		bool m_transFormerDrags = false;
		bool m_powerToysFix = false;
		#endregion
		
		#region Constructors
    /// <summary>
    /// Constructor
    /// </summary>
    public AlphaFormTransformer()
		{
			Size = new Size(250, 250);
		}
		#endregion
		
		#region Overrides
		/// <summary>
    /// Overriden for internal use.
		/// </summary>
		protected override void OnPaint(PaintEventArgs e)
		{
			base.OnPaint(e);
			if (m_fading) return; // here just to avoid the compiler warning about unused var.
		  if (m_powerToysFix && m_alphaBitmap != null && m_lwin != null && System.Environment.OSVersion.Version.Major < 6)
		    m_lwin.SetBits(m_alphaBitmap,(byte)(ParentForm.Opacity*255));
      }
		#endregion
		
		#region Properties

    /// <summary>
    /// When moving the form, this control tries to help the desktop and underlying Windows catch up to
    /// redrawing invalidated areas (we're actually moving two Windows although the layered
    /// Window of the pair is double buffered). If it detects we're dragging the Form, it sleeps the main
    /// thread according to this property. A reasonable value like 30 milliseconds makes for less
    /// distracting redrawing of the invalidated parts of the desktop. Under Vista this property is
    /// ignored as the DWM double buffers everything. 
    /// </summary>
    /// <value>Gets or sets the drag sleep delay in milliseconds</value>
    [Category("Alpha Transformer Properties"), Description("Drag Sleep in Milliseconds (Ignored on Vista)")]
    [DefaultValue(30)]
		public uint DragSleep
		{
			get
			{
				return m_dragSleep;
			}
			set
			{
				m_dragSleep = value;
			}
		}

    /// <summary>
    /// Microsoft's "PowerToys" Alt+Tab extension which replaces the default  
    /// Alt+Tab functionality has issues with some video cards and layered windows
    /// windows and also with any window that has an owner. When
    /// the user press Alt+Tab, it shows snapshots for owned windows which
    /// it should not (a design flaw really), and in our case that
    /// includes our composited frame window which hosts the bitmap. Second in our
    /// labs on machines with ATI video cards, it does not
    /// capture the composited window contents correctly, and somehow corrupts the
    /// buffered contents. So if the user Alt+Tabs to the frame window, it
    /// will all of a sudden turn to a semi-transparent white box. As a
    /// work-around to the corruption you can set PowerToysFix to true 
    /// at the cost of unnecessarily refreshing the layered window contents (normally double
    /// buffered by the OS) when this control gets a paint message.
    /// </summary>
    /// <value>Gets or sets the PowerToys fix flag</value>
    [Category("Alpha Transformer Properties"), Description("PowerToys fix for XP")]
    [DefaultValue(false)]
    public bool PowerToysFix
    {
      get
      {
      return m_powerToysFix;
      }
      set
      {
      m_powerToysFix = value;
      }
    }
		
		private Form ParentForm
		{
			get
			{
				return (Form)TopLevelControl;
			}
		}

    /// <summary>
    /// True if mouse drag on this control drags the entire form (meaning the frame window and main
    /// form together). This will typically be used when a background image is set to the main form and
    /// the user wants controls on top of the bitmap. In this case being able to drag by clicking on the 
    /// background is an assumption most users will make.
    /// </summary>
    /// <value>Gets or sets the CanDrag property</value>
    [Category("Alpha Transformer Properties"), Description("If true user can drag form by dragging this control")]
    [DefaultValue(false)]
    public bool CanDrag
    {
      get
      {
        return m_transFormerDrags;
      }
      set
      {
        m_transFormerDrags = value;
      }
    }
		#endregion
		
		#region Methods
    /// <summary>
    /// This sets the Visible property of the form and layered window
    /// </summary>
    /// <param name="v">
    /// Visible value
    /// </param>
		public void SetAlphaAndParentFormVisible(bool v)
		{
		  ParentForm.Visible = v;
		  m_lwin.Visible = v;
		}

    /// <summary>
    /// This sets the Opacity of the form and layered window
    /// </summary>
    /// <param name="v">
    /// Visible value
    /// </param>
		public void SetAlphaAndParentFormOpacity(double v)
		{
		  ParentForm.Opacity = v;
      m_lwin.SetBits(m_alphaBitmap, Math.Min((byte)(v*255),(byte)255));
		}
		
    /// <summary>
    /// This methods does the following:
    /// <list type="number">
    /// <item>
    /// <description>Sets the frame window image data from <paramref name="frameBmap"/> and
    /// scales as needed.</description>
    /// </item>
    /// <item>
    /// <description>Constructs the main form's Region from the AlphaFormMarkers in 
    /// this control.</description>
    /// </item>
    /// <item>
    /// <description>Updates the frame window's bitmap.</description>
    /// </item>
    /// <item>
    /// <description>Sets the main form's background image to <paramref name="backBmap"/> with scaling if 
    /// needed.</description>
    /// </item>
    /// </list>
    /// </summary>
    /// <param name="frameBmap">
    /// Bitmap to repace current frame image
    /// </param>
    /// <param name="backBmap">
    /// Bitmap to replace the current background image
    /// </param>
    /// <param name="opacity">
    /// Opacity [0..255] of composite form (both the frame and background)
    /// </param>
    /// <remarks>
    /// <para>
    /// If this routine is used at runtime to change the alpha channel image,
    /// it does nothing to resize the main form, this control, or the  
    /// the AlphaFormMarkers. If the user intends to update the image
    /// with one that has a *different* alpha channel, he'll need to do the
    /// following BEFORE calling this routine:
    /// </para>
    /// <list type="number">
    /// <item>
    /// <description>If the image size has changed, resize the main form (and this control if it's not docked)</description>
    /// </item>
    /// <item>
    /// <description>Reposition the AlphaFormMarker controls (potentially adding new 
    /// ones) as needed so the form Region can be calculated.</description>
    /// </item>
    /// <item>
    /// <description>Reposition any other controls on the form.</description>
    /// </item>
    /// </list>
    /// On the other hand, if we're simply swapping in an image with the same
    /// size and alpha channel, then simply call this routine.
    /// </remarks>
		public void UpdateSkin(Bitmap frameBmap, Bitmap backBmap, byte opacity)
		{
		  // From an internal perspective, frameBmap is the layered window contents
		  // and backBmap becomes (potentially after scaling) the transformer control
		  // BackgroundImage.

      if (frameBmap == null && backBmap == null)
        throw new ApplicationException("Must specify at least one bitmap to UpdateSkin()");
      if (!Bitmap.IsCanonicalPixelFormat(frameBmap.PixelFormat) || !Bitmap.IsAlphaPixelFormat(frameBmap.PixelFormat))
				throw new ApplicationException("The frame bitmap must be 32 bits per pixel with an alpha channel.");			
			
      double
        sizeFactor = 1.0;

			if (frameBmap != null)
      {
				// A prerequisite with this control is that the main form's size
				// is set equal to the background image size. However, if the main form's
				// AutoScaleMode is set to DPI, and the application is run
				// on a system where the font or DPI resolution differs from the design
				// time values, then .NET will scale the form and all its controls. However
				// the background image is not scaled, so we'll catch that
				// condition here and scale the image accordingly. Again, this logic works
				// *assuming* you always set the main form's size equal to the image size 
				// at design time.
				if (frameBmap.Size != ParentForm.Size)
				{
          m_alphaBitmap = new Bitmap(ParentForm.Width, ParentForm.Height);
          Graphics gr = Graphics.FromImage(m_alphaBitmap);
					gr.SmoothingMode = SmoothingMode.None;
					gr.CompositingQuality = CompositingQuality.HighQuality;
					gr.InterpolationMode = InterpolationMode.HighQualityBilinear;

					gr.DrawImage(frameBmap, new Rectangle(0, 0, ParentForm.Width, ParentForm.Height),
						new Rectangle(0, 0, frameBmap.Width, frameBmap.Height), GraphicsUnit.Pixel);
					gr.Dispose();
          sizeFactor = Math.Max((double)ParentForm.Size.Width / frameBmap.Size.Width,
            (double)ParentForm.Size.Height/frameBmap.Size.Height);
				}
			  else
          m_alphaBitmap = frameBmap;

        // Need to adjust the region fill border as scaling up will introduce 
        // semi-transparent pixels along edges
        if (frameBmap.Size != ParentForm.Size && ParentForm.BackgroundImage != null)
        {
          foreach (Control cntrl in Controls)
          {
            if (typeof(AlphaFormMarker).IsInstanceOfType(cntrl))
            {
              AlphaFormMarker marker = (AlphaFormMarker)cntrl;
              marker.FillBorder = (uint)Math.Round((marker.FillBorder+1.5) * sizeFactor);
            }
          }
        }
      }
				
			// Build a array of the alpha values taken from the background image.
			// Some of the transparent pixels in this array will be the target of
			// what is essentially a seed fill operation in UpdateRectListFromAlpha().
			// As some members are set to a non-zero value (i.e., filled), corresponding 
			// rectangles are added to a running arraylist of rectangles that will
			// eventually be used to build the main form's Region.

      BitmapData bData = m_alphaBitmap.LockBits(
        new Rectangle(0, 0, m_alphaBitmap.Width, m_alphaBitmap.Height),
				ImageLockMode.ReadOnly,
        m_alphaBitmap.PixelFormat);

      byte[,] alphaArr = new byte[m_alphaBitmap.Width, m_alphaBitmap.Height];

#if FAST_ALPHA_BUILD

			// Here is an optional way to build the alpha array. It's
			// fast but requires pointer access.
			unsafe
			{
				byte *line = (byte*)bData.Scan0;
				byte *alpha;
		
				for (int j = 0; j < m_alphaBitmap.Height; j++, line += bData.Stride)
				{
					// image data is r->g->b->a in memory
					alpha = line + 3;
					for (int i = 0; i < m_alphaBitmap.Width; i++, alpha += 4)
						alphaArr[i,j] = *alpha;
				}
			}
#else

      // Seems faster albeit somewhat wasteful to marshal to a 
			// managed array than to use Bitmap.GetPixel. If you're
			// OK with unsafe code, then define FAST_ALPHA_BUILD and
			// enable unsafe compilation 

      byte[] mngImgData = new byte[m_alphaBitmap.Height * bData.Stride];
			Marshal.Copy(bData.Scan0, mngImgData, 0, mngImgData.Length);
      for (int j = 0; j < m_alphaBitmap.Height; j++)
			{
				int ai = j * bData.Stride + 3;
        for (int i = 0; i < m_alphaBitmap.Width; i++, ai += 4)
				{
					alphaArr[i, j] = mngImgData[ai];
				}
			}
#endif

      m_alphaBitmap.UnlockBits(bData);

			Rectangle bounds = new Rectangle();
			ArrayList rectList = new ArrayList();

			// The location of each AlphaFormMarker control serves as a seed point location
			// for building the set of rectangles that describe the enclosed transparent region 
			// (within the background image's alpha channel) around that point.

			foreach (Control cntrl in Controls)
			{
				if (typeof(AlphaFormMarker).IsInstanceOfType(cntrl))
				{
					AlphaFormMarker marker = (AlphaFormMarker)cntrl;

					UpdateRectListFromAlpha(
						rectList,
						ref bounds,
						new Point(marker.Location.X + marker.Width / 2, marker.Location.Y + marker.Height / 2),
						alphaArr,
						m_alphaBitmap.Width,
						m_alphaBitmap.Height,
						(int)marker.FillBorder);

					// Hide the marker as we don't want to see it at run time.
					marker.Visible = false;
				}
			}
      
      // Build the main form's region
			ParentForm.Region = RegionFromRectList(rectList, bounds);

			// Set the layered window bitmap
      m_lwin.SetBits(m_alphaBitmap,opacity);

      // Swap in the main form's background image (if any) and layout
      BackgroundImage = backBmap;
      BackgroundImageLayout = ParentForm.BackgroundImageLayout;
      if (backBmap != null)
      {
        ParentForm.BackgroundImage = null;
        // Like above, we rescale if needed...
        if (backBmap.Size != ParentForm.Size)
        {
          Bitmap backScaled = new Bitmap(ParentForm.Width, ParentForm.Height);
          Graphics gr = Graphics.FromImage(backScaled);
          gr.SmoothingMode = SmoothingMode.None;
          gr.CompositingQuality = CompositingQuality.HighQuality;
          gr.InterpolationMode = InterpolationMode.HighQualityBilinear;

          gr.DrawImage(backBmap, new Rectangle(0, 0, ParentForm.Width, ParentForm.Height),
            new Rectangle(0, 0, BackgroundImage.Width, BackgroundImage.Height), GraphicsUnit.Pixel);
          gr.Dispose();
          BackgroundImage = backScaled;
        }
      }

      ParentForm.Opacity = opacity / 255.0;
    }
	
		
		/// <summary>
		/// This methods does the following:
    /// <list type="number">
    /// <item>
		/// <description>Constructs a desktop composited frame window and shows it. This window's
		/// bitmap is built from this control's <c>BackgroundImage</c>.</description>
    /// </item>
    /// <item>
		/// <description>Calls <c>UpdateSkin</c> to updated the frame window's image data
    /// and build the main form's Region.</description>
    /// </item>
    /// <item>
		/// <description>The BackgroundImage is set to the main form's Background image.</description>
    /// </item>
    /// </list>
    /// <para>
    /// This method should only be called *once* from the main form's
    /// Load event. If the user wants to change the image at runtime,
    /// <see cref="AlphaFormTransformer.UpdateSkin" /> should be used.
    /// </para>
		/// </summary>
    /// <param name="opacity">
    /// Opacity of form [0..255]
    /// </param>
		public void TransformForm(byte opacity)
		{
			if (DesignMode)
				return;
				
#if _DEMO
      MessageBox.Show("AlphaForm Transformer Demo Mode.\n\nPlease see www.alpha-forms.com for information","Demo Mode");
#endif

			m_lwin = new LayeredWindowForm();
			
			// Setting the layered window's TopMost to the main
			// form's value keeps the relative Z order the same for 
			// the pair of windows
			m_lwin.TopMost = ParentForm.TopMost;
			
			// We don't want the layered window form to show in the 
			// taskbar now do we :-)
			m_lwin.ShowInTaskbar = false;

			// These will handle dragging for both the layered window
			// and the main form.
			m_lwin.MouseDown += new MouseEventHandler(LayeredFormMouseDown);
			m_lwin.MouseMove += new MouseEventHandler(LayeredFormMouseMove);
			m_lwin.MouseUp += new MouseEventHandler(LayeredFormMouseUp);
			if (m_transFormerDrags)
      {
        MouseDown += new MouseEventHandler(FormMouseDown);
        MouseMove += new MouseEventHandler(FormMouseMove);
        MouseUp += new MouseEventHandler(FormMouseUp);
      }
			ParentForm.Move += new EventHandler(ParentFormMove);
			
			// Form must be shown by specifying an owner so that activation of
			// the layered form activates the main app. It's also needed to keep
			// the Z order of the two windows in sync. Note that it's not 
			// necessary to set the size of the layered window. The call to 
			// SetBits() does this.
      m_lwin.Visible = false;
			m_lwin.Show(ParentForm);
			m_lwin.Location = ParentForm.Location;
			
			ParentForm.Opacity = opacity/255.0;

			if (BackgroundImage != null)
			{
        // Update the layered window bits and form region
        UpdateSkin(
          new Bitmap(BackgroundImage), 
          ParentForm.BackgroundImage != null ? new Bitmap(ParentForm.BackgroundImage) : null,
          opacity);
        
        m_lwin.Visible = true;
        
			  if (BackColor == Color.Transparent)
			  {
			    // user may have it this way in the designer too see the 
			    // main form's background image, but it can't be set to 
			    // this at runtime as the background image will be copied from
			    // the main form to this control. Even in the absence of a background
			    // image, it can't be transparent as it won't composite correctly
			    // with the main form at runtime (quirk with Panel? - irrelevant in any case)
			    BackColor = SystemColors.Control;
		    }
		  }
    }
    

		public void TransformForm2(byte opacity)
		{
			if (DesignMode)
				return;
				
#if _DEMO
      MessageBox.Show("AlphaForm Transformer Demo Mode.\n\nPlease see www.alpha-forms.com for information","Demo Mode");
#endif

			m_lwin = new LayeredWindowForm();
	
			// This control becomes transparent
			BackColor = Color.Transparent;
			
			// ParentForm becomes transparent via color key
			ParentForm.TransparencyKey = Color.FromArgb(0,0,1);
			ParentForm.BackColor = ParentForm.TransparencyKey;
			ParentForm.Visible = false; 
			ParentForm.ShowInTaskbar = false;
		
			// Setting the layered window's TopMost to the main
			// form's value keeps the relative Z order the same for 
			// the pair of windows
			m_lwin.TopMost = ParentForm.TopMost;
			
			if (BackgroundImage.Size != ParentForm.Size)
			{
        m_alphaBitmap = new Bitmap(ParentForm.Width, ParentForm.Height);
        Graphics gr = Graphics.FromImage(m_alphaBitmap);
				gr.SmoothingMode = SmoothingMode.None;
				gr.CompositingQuality = CompositingQuality.HighQuality;
				gr.InterpolationMode = InterpolationMode.HighQualityBilinear;

				gr.DrawImage(BackgroundImage, new Rectangle(0, 0, ParentForm.Width, ParentForm.Height),
					new Rectangle(0, 0, BackgroundImage.Width, BackgroundImage.Height), GraphicsUnit.Pixel);
				gr.Dispose();
			}
			else
				m_alphaBitmap = new Bitmap(BackgroundImage);
				
			BackgroundImage = null;
				
			// Set the layered window bitmap
      m_lwin.SetBits(m_alphaBitmap,opacity);
			
			// For each transparent control, copy the layered window background
			// into the control's background
			
			foreach (Control cntrl in Controls)
      {
				if (cntrl.Tag != null && cntrl.Tag.ToString() == "back")
					{
					Bitmap bm = new Bitmap(cntrl.Width,cntrl.Height);
					Graphics cg = Graphics.FromImage(bm);
					cg.DrawImage(m_alphaBitmap, new Rectangle(0,0,cntrl.Width,cntrl.Height),
						new Rectangle(cntrl.Location.X, cntrl.Location.Y, cntrl.Width, cntrl.Height),
						GraphicsUnit.Pixel);
					cntrl.BackgroundImage = bm;
					cg.Dispose();
					}
      }	
      
      m_lwin.MouseDown += new MouseEventHandler(LayeredFormMouseDown);
			m_lwin.MouseMove += new MouseEventHandler(LayeredFormMouseMove);
			m_lwin.MouseUp += new MouseEventHandler(LayeredFormMouseUp);
			ParentForm.Move += new EventHandler(ParentFormMove);
			
			ParentForm.Opacity = opacity/255.0;
			m_lwin.Location = ParentForm.Location;
			m_lwin.Text = ParentForm.Text;
			
			// ParentForm becomes owned by layered window
			ParentForm.Visible = true;
			ParentForm.Owner = m_lwin;
			m_lwin.Visible = true;
			
			
    }

 
		void PushSeg(Stack stack, int x1, int xr, int y, int dy, int height)
		{
			int 
				yn = y + dy;
			if (yn >= 0 && yn < height)
				stack.Push(new LineSeg(x1,xr,y,dy));
		}

		void PopSeg(Stack stack, out int xl, out int xr, out int y, out int dy)
		{
			LineSeg lseg = (LineSeg)(stack.Pop());
			xl = lseg.x1;
			xr = lseg.x2;
			dy = lseg.dy;
			y = lseg.y + dy;
		}

		/// <summary>
		/// Updates a list of rectangles using a seedfill of the alpha channel
		/// array starting from the seedPt. 
    /// <para>
    /// The border value specifies how far into the non-transparent pixel border
    /// the region will be constructed. The composited image will typically have
    /// semitransparent edges (in fact you *want* this for esthetic reasons, and 
    /// it might be a consequence of antialiasing, Photoshop mask feathering, or
    /// whatever tool you use to construct the image). Therefore the region that's
    /// built from the marker position needs to expand some number of pixels into
    /// (and *under* from the compositing point of view) the semitransparent area, 
    /// otherwise you will see through to the desktop along these borders. You want
    /// the border value to be large enough the cover the thickness of the
    /// semitransparent edge (a few pixels), but not too large which might
    /// cause it to extend past the other side of the masked image boundary.
    /// </para>
		/// </summary>
		/// <remarks>
		/// The substantive part of this routine is adapted from Paul Heckbert's
		/// seed fill algorithm ("Graphics Gems", Academic Press, 1990). From
		/// the original C implementation we got rid of a goto and changed the
		/// fixed stack size design to use a Collections.Stack.
		/// The fill value in this case is any non-zero alpha value. 
		/// For each filled pixel location, a rectangle is added to the rectList 
		/// which will be used by the caller to build a region. Finally, a bounding
		/// rectangle is calculated as it's needed for region construction. 
		/// </remarks>
		void UpdateRectListFromAlpha(ArrayList rectList , ref Rectangle bounds, Point seedPt, byte[,] alphaArr, int width, int height, int border)
		{
			Stack
				segStack = new Stack();
			Rectangle	
				addRct = new Rectangle();
			int 
				left,x;
				
			if (rectList.Count == 0)
			{
				bounds.X = Int32.MaxValue;
				bounds.Y = Int32.MaxValue;
				bounds.Width = 0;
				bounds.Height = 0;
			}
				 
			if( alphaArr[seedPt.X,seedPt.Y] != 0 || seedPt.X < 0 || seedPt.X >= width || seedPt.Y < 0 || seedPt.Y >= height)
				return;

			PushSeg(segStack,seedPt.X,seedPt.X,seedPt.Y,1,height);
			PushSeg(segStack,seedPt.X,seedPt.X,seedPt.Y+1,-1,height);
			
			int 
				x1,x2,y,dy;
			
			while (segStack.Count > 0)
			{
				PopSeg(segStack,out x1,out x2,out y,out dy);
				for( x = x1; x >= 0 && alphaArr[x,y] == 0; --x )
				{
					addRct.X = x-border;
					addRct.Y = y-border;
					addRct.Width = 2*border+1;
					addRct.Height = addRct.Width;
					rectList.Add(addRct);
					
					if (addRct.Left < bounds.Left)
						bounds.X = addRct.Left;
					if (addRct.Top < bounds.Top)
						bounds.Y = addRct.Top;
					if (addRct.Width > bounds.Width)
						bounds.Width = addRct.Width;
					if (addRct.Height > bounds.Height)
						bounds.Height = addRct.Height;

					// any non-zero value will do
					alphaArr[x,y] = 1; 
				}
								
				if (x < x1)
				{
					left = x + 1;
					if (left < x1)
						PushSeg(segStack,left,x1-1,y,-dy,height);
					x = x1 + 1;
				}
				else 
				{
					for( ++x; x <= x2 &&  alphaArr[x,y] != 0; ++x ) 
						{;}
					left = x;
					if (x > x2)
						continue;
				}
				
				do 
				{
					for( ; x < width && alphaArr[x,y] == 0; ++x )
					{
						addRct.X = x-border;
						addRct.Y = y-border;
						addRct.Width = 2*border+1;
						addRct.Height = addRct.Width;
						rectList.Add(addRct);
						
						if (addRct.X < bounds.Left)
							bounds.X = addRct.Left;
						if (addRct.Y < bounds.Top)
							bounds.Y = addRct.Top;
						if (addRct.Width > bounds.Width)
							bounds.Width = addRct.Width;
						if (addRct.Height > bounds.Height)
							bounds.Height = addRct.Height;

						// any non-zero value will do
						alphaArr[x,y] = 1;
					}
					
					PushSeg(segStack,left,x-1,y,dy,height);
		          
					if(x > x2+1)
						PushSeg(segStack,x2+1,x-1,y,-dy,height);
						
					for( ++x; x <= x2 && alphaArr[x,y] != 0; ++x ) 
						{;}

					left = x;
				} while(x <= x2);
			}
		}
	
		/// <summary>
		/// The region is constructed from a list of Rectangles using the
		/// Win32 ExtCreateRegion() call. 
		/// </summary>
		/// <remarks>
		/// If you're wondering why we use this unmanged API, it's because
		/// GraphicsPath.AddRectangles() followed by instantiating the Region
		/// from the path is *terribly* slow - more than an order of magnitude
		/// slower than ExtCreateRegion. And Region.Union is slower still 
		///	(and broken to boot). 
		/// </remarks>
		Region RegionFromRectList(ArrayList rectList, Rectangle bounds)
		{
			uint
				dSize = (uint)(32 + rectList.Count * 16);

			IntPtr
				rgnData = Marshal.AllocHGlobal((Int32)dSize);
				
			// We're just filling out the equivalent of a RGNDATA 
			// followed by an array of Win32 RECTs
			int [] mngRgnData = new int[rectList.Count * 4 + 8];
			mngRgnData[0] = 32;											// dwSize
			mngRgnData[1] = 1;											// iType = RDH_RECTANGLES
			mngRgnData[2] = rectList.Count;					// nCount
			mngRgnData[3] = rectList.Count * 16;		// nRgnSize
			mngRgnData[4] = bounds.Left;						// rcBound
			mngRgnData[5] = bounds.Top;							// .
			mngRgnData[6] = bounds.Width;						// .
			mngRgnData[7] = bounds.Height;					// .
			// Array of RECTs
			for (int i = 0; i < rectList.Count; i++)
			{
				Rectangle mngRct = (Rectangle)rectList[i];
				mngRgnData[4*i+8] = mngRct.Left;
				mngRgnData[4*i+9] = mngRct.Top;
				mngRgnData[4*i+10] = mngRct.Right;
				mngRgnData[4*i+11] = mngRct.Bottom;
			}
				
			Marshal.Copy(mngRgnData,0,rgnData,mngRgnData.Length);
      IntPtr nativeRgn = Win32.ExtCreateRegion(IntPtr.Zero, dSize, rgnData);
			Marshal.FreeHGlobal(rgnData);
			return Region.FromHrgn(nativeRgn);
		}
			
		void LayeredFormMouseDown(Object sender, MouseEventArgs e) 
		{
			if (e.Button == MouseButtons.Left)
			{
				m_drag = true;
				m_dragStart = e.Location;
			}
		}
			
		void LayeredFormMouseMove(Object sender, MouseEventArgs e) 
		{
			if (m_drag)
			{	
				int dx =  e.X - m_dragStart.X;
				int dy =  e.Y - m_dragStart.Y;

        // The thing about this trick of changing the window position 
        // from the control's mouse move, is it generates additional
        // moves even though the cursor isn't moving (it's a .DOT thing
        // as I've used this approach in straight Win32 and this doesn't happen)
        if (dx == 0 && dy == 0)
          return;
          
        ParentForm.Location = new Point(ParentForm.Location.X + dx, ParentForm.Location.Y + dy);
			}
		}
			
		void LayeredFormMouseUp(Object sender, MouseEventArgs e) 
		{
			m_drag = false;
		}

    void FormMouseDown(Object sender, MouseEventArgs e)
    {
      if (e.Button == MouseButtons.Left)
      {
        m_drag = true;
        m_dragStart = e.Location;
      }
    }

    void FormMouseMove(Object sender, MouseEventArgs e)
    {
      if (m_drag)
      {
        int dx = e.X - m_dragStart.X;
        int dy = e.Y - m_dragStart.Y;

        // The thing about this trick of changing the window position 
        // from the control's mouse move, is it generates additional
        // moves even though the cursor isn't moving (it's a .DOT thing
        // as I've used this approach in straight Win32 and this doesn't happen)
        if (dx == 0 && dy == 0) 
          return;
          
        ParentForm.Location = new Point(ParentForm.Location.X + dx, ParentForm.Location.Y + dy);
        
      }
    }

    void FormMouseUp(Object sender, MouseEventArgs e)
    {
      m_drag = false;
    }
		
		void ParentFormMove(Object sender, EventArgs e)
		{
			m_lwin.Location = ParentForm.Location;
			// On XP or earlier, we wait a specified number of milliseconds
			// to allow the OS to update invalidated areas revealed by dragging.
			// The trade off here is slightly less responsiveness while dragging
			// for reduced repainting unsightliness (not our windows, but other
			// windows behind). On Vista, it's unnecessary as everything is double-buffered.
			if (m_drag && System.Environment.OSVersion.Version.Major < 6)
				System.Threading.Thread.Sleep((int)m_dragSleep);
		}

    /// <summary>
    /// Fades the composite form (i.e., the frame window and the main form) 
    /// in or out
    /// </summary>
    /// <param name="duration">
    /// Fade duration milliseconds
    /// </param>
    /// <param name="ft">
    /// Fade in or out
    /// </param>
    /// <param name="frameWinOnly">
    /// True if fade the frame window only
    /// </param>
    /// <param name="startSteep">
    /// <para>
    /// True if fading starts with steep ramp and ends flat 
    /// </para>
    /// <para>
    /// False if fading starts flat and ends steep
    /// </para>
    /// </param>
    /// <remarks>
    /// NOTE: If fading out and <paramref name="frameWinOnly"/> is true, the main form's <c>Visible</c>
    /// property will be false on exit.
    /// </remarks>
    public void Fade(FadeType ft, bool startSteep, bool frameWinOnly, int duration)
		{
			if (m_alphaBitmap != null && m_lwin != null)
			{
        m_fading = true;
        if (ft == FadeType.FadeIn && !frameWinOnly)
			  {
          SetFormOpacity(ParentForm, 0);
          ParentForm.Update();
			  }

        if (ft == FadeType.FadeOut && frameWinOnly)
			  {
			    // We get faster background updating by setting visibility
			    // as opposed to opacity
			    ParentForm.Visible = false;
          ParentForm.Update();
        }
			 
        Impulse imp = new Impulse(8.0);
        int op = 0;
        int uop = 0;
        // 30 fps is a reasonable median starting value (it will
        // be adjusted, on-the-fly, up or down to give 
        // the greatest frame rate for the requested duration)
        int frames = Math.Max((30 * duration)/1000,1);
        int inc = Math.Max(255/frames,1);
        int frameDur = duration/frames;

        while (uop != 255)
        {
          if (startSteep)
            uop = (int)Math.Round(imp.Evaluate(op / 255.0) * 255.0);
          else
            uop = 255 - (int)Math.Round(imp.Evaluate(1.0 - op / 255.0) * 255.0);
            
          if (uop > 255)
            uop = 255;
            
          DateTime sf = DateTime.Now;

          // If fading in, we want the layered window to lead
          // If fading out, want the main form to lead (to reduce revealing of 
          // the stair-stepped region boundary). 
          for (int i = 0; i < 2; i++)
            {
            if (((ft == FadeType.FadeIn ? 0 : 1) ^ i) == 0)
              m_lwin.SetBits(m_alphaBitmap, (byte)(ft == FadeType.FadeIn ? uop : 255 - uop));
            else if (!frameWinOnly)
              SetFormOpacity(ParentForm, (byte)(ft == FadeType.FadeIn ? uop : 255 - uop));
            }
              
          TimeSpan ts = DateTime.Now - sf;
          int wait = frameDur - ts.Milliseconds;
          
          // We either wait or speed up to maintain requested duration
          if (wait > 0)
          {
            System.Threading.Thread.Sleep(wait);
            if (wait <= frameDur/2 && inc > 1)
            {
              inc = Math.Max(1,inc/2);
              frameDur /= 2;
            }
          }
          else if (Math.Abs(wait) >= frameDur)
          {
            inc *= 2;
            frameDur *= 2;
          }
          
          op += inc;
        }
      
      // Keep the Opacity property in sync as we bypass it above.
      ParentForm.Opacity = ft == FadeType.FadeIn ? 1.0 : 0.0;
   
			m_fading = false;
			}
		
		}

	/// <summary>
	/// Sets a forms opacity by subclassing it to a layered window
	/// and then calling SetLayeredWindowAttributes. This works around
	/// flickering if you try to modify Form.Opacity directly (which does
  /// the same thing but it does something in addition that can cause
  /// flicker)
	/// </summary>
		static void SetFormOpacity(Form form, byte opacity)
		{
		
			int exStyle = (int)Win32.GetWindowLong(form.Handle, -20);
			byte bNewAlpha = opacity;
			int newExStyle = exStyle;

			if (bNewAlpha != 255)
				newExStyle = newExStyle | 0x00080000;

			if (newExStyle != exStyle || (newExStyle & 0x00080000) != 0)
			{
				if (newExStyle != exStyle)
					Win32.SetWindowLong(form.Handle, -20, new IntPtr(newExStyle)); 

				if ((newExStyle & 0x00080000) != 0)
					Win32.SetLayeredWindowAttributes(form.Handle, 0, bNewAlpha, 2);
			}
			// Since we're making a bunch of PInvokes on the form's native handle,
			// make sure it sticks around
			GC.KeepAlive(form);
		}
		#endregion
		
	}
			
}
