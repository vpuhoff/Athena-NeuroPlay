using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;
using System.Drawing;

static class Win32
{
	public enum Message : uint
	{
		WM_NCHITTEST			= 132,
		HTTRANSPARENT			= 0xFFFFFFFF,
		HTCLIENT				= 1,
		HTCAPTION				= 2,
		WM_NCMOUSEMOVE			= 160,
		WM_NCLBUTTONDOWN		= 161,
		WM_NCLBUTTONUP			= 162,
		WM_NCLBUTTONDBLCLK		= 163,
		WM_WINDOWPOSCHANGING	= 70,
		WM_ENTERSIZEMOVE		= 561,
		WM_EXITSIZEMOVE			= 562,
		WM_SYSCOMMAND			= 274,
		WM_PAINT				= 15,
		HWND_TOP				= 0,
		SC_MINIMIZE				= 61472,
		SC_RESTORE				= 61728,
		SC_MAXIMIZE				= 61488,
		WM_SIZE					= 5,
		WM_ACTIVATE				= 6,
		WM_SETFOCUS				= 7,
		WM_SETCURSOR			= 32,
		WM_MOUSEMOVE			= 0x0200,
		WM_LBUTTONDOWN			= 0x0201,
		WM_LBUTTONUP			= 0x0202,
		WM_LBUTTONDBLCLK		= 0x0203,
		WM_RBUTTONDOWN			= 0x0204,
		WM_RBUTTONUP			= 0x0205,
		WM_RBUTTONDBLCLK		= 0x0206,
		WM_MBUTTONDOWN			= 0x0207,
		WM_MBUTTONUP			= 0x0208,
		WM_MBUTTONDBLCLK		= 0x0209,
		WM_MOUSEWHEEL			= 0x020A,
		WM_XBUTTONDOWN			= 0x020B,
		WM_XBUTTONUP			= 0x020C,
		WM_XBUTTONDBLCLK		= 0x020D,
		WM_MOUSELEAVE			= 0x02A3,
		WM_WINDOWPOSCHANGED		= 0x0047,
		WM_NCACTIVATE			= 0X0086,
		GWL_WNDPROC				= 0xFFFFFFFC,
		GWL_EXSTYLE				= 0xFFFFFFEC
	};

	[StructLayout(LayoutKind.Sequential)]
	public struct WINDOWPOS
	{
		public IntPtr hwnd;
		public IntPtr hwndInsertAfter;
		public int x;
		public int y;
		public int cx;
		public int cy;
		public WindowPosFlags flags;
	};

	[Flags]
	public enum WindowPosFlags : uint
	{
		NONE				= 0x0000,
		SWP_NOSIZE			= 0x0001,
		SWP_NOMOVE			= 0x0002,
		SWP_NOZORDER		= 0x0004,
		SWP_NOREDRAW		= 0x0008,
		SWP_NOACTIVATE		= 0x0010,
		SWP_FRAMECHANGED	= 0x0020,
		SWP_SHOWWINDOW		= 0x0040,
		SWP_HIDEWINDOW		= 0x0080,
		SWP_NOCOPYBITS		= 0x0100,
		SWP_NOOWNERZORDER	= 0x0200,
		SWP_NOSENDCHANGING	= 0x0400,
		SWP_DEFERERASE		= 0x2000,
		SWP_ASYNCWINDOWPOS	= 0x4000,
		SWP_CUSTOMFLAG		= 0x8000
	};

	public enum WindowStyles
	{
		WS_EX_LAYERED = 0x00080000
	}

	public enum GetWindow_Cmd : uint
	{
		GW_HWNDFIRST = 0,
		GW_HWNDLAST = 1,
		GW_HWNDNEXT = 2,
		GW_HWNDPREV = 3,
		GW_OWNER = 4,
		GW_CHILD = 5,
		GW_ENABLEDPOPUP = 6
	};

	public enum SystemCursor : int
	{
		IDC_APPSTARTING	= 32650, //Standard arrow and small hourglass
		IDC_NORMAL		= 32512, //Standard arrow
		IDC_CROSS		= 32515, //Crosshair
		IDC_HAND		= 32649, //Hand
		IDC_HELP		= 32651, //Arrow and question mark
		IDC_IBEAM		= 32513, //I-beam
		IDC_NO			= 32648, //Slashed circle
		IDC_SIZEALL		= 32646, //Four-pointed arrow pointing north, south, east, and west
		IDC_SIZENESW	= 32643, //Double-pointed arrow pointing northeast and southwest
		IDC_SIZENS		= 32645, //Double-pointed arrow pointing north and south
		IDC_SIZENWSE	= 32642, //Double-pointed arrow pointing northwest and southeast
		IDC_SIZEWE		= 32644, //Double-pointed arrow pointing west and east
		IDC_UP			= 32516, //Vertical arrow
		IDC_WAIT		= 32514  //Hourglass
	}

	[StructLayout(LayoutKind.Sequential, Pack = 1)]
	public struct BLENDFUNCTION
	{
		public byte BlendOp;
		public byte BlendFlags;
		public byte SourceConstantAlpha;
		public byte AlphaFormat;
	}

	public enum BlendOps : byte
	{
		AC_SRC_OVER				= 0x00,
		AC_SRC_ALPHA			= 0x01,
		AC_SRC_NO_PREMULT_ALPHA	= 0x01,
		AC_SRC_NO_ALPHA			= 0x02,
		AC_DST_NO_PREMULT_ALPHA	= 0x10,
		AC_DST_NO_ALPHA			= 0x20
	}

	public enum BlendFlags : uint
	{
		None			= 0x00,
		ULW_COLORKEY	= 0x01,
		ULW_ALPHA		= 0x02,
		ULW_OPAQUE		= 0x04
	}

	public enum TernaryRasterOperations : uint
	{
		SRCCOPY = 0x00CC0020,
		SRCPAINT = 0x00EE0086,
		SRCAND = 0x008800C6,
		SRCINVERT = 0x00660046,
		SRCERASE = 0x00440328,
		NOTSRCCOPY = 0x00330008,
		NOTSRCERASE = 0x001100A6,
		MERGECOPY = 0x00C000CA,
		MERGEPAINT = 0x00BB0226,
		PATCOPY = 0x00F00021,
		PATPAINT = 0x00FB0A09,
		PATINVERT = 0x005A0049,
		DSTINVERT = 0x00550009,
		BLACKNESS = 0x00000042,
		WHITENESS = 0x00FF0062
	}

	public delegate int Win32WndProc(IntPtr hWnd, int Msg, int wParam, int lParam);

	[DllImportAttribute("user32.dll")]
	public static extern int SendMessage(IntPtr hWnd, int Msg, int wParam, int lParam);

	[DllImportAttribute("user32.dll")]
	public static extern bool ReleaseCapture();

	[DllImportAttribute("user32.dll")]
	public static extern void SetCapture(IntPtr hWnd);

	[DllImportAttribute("user32.dll")]
	public static extern IntPtr GetCapture();

	[DllImport("user32.dll")]
	[return: MarshalAs(UnmanagedType.Bool)]
	public static extern bool SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter, int X, int Y, int cx, int cy, uint uFlags);

	[DllImport("user32.dll", SetLastError = true)]
	public static extern IntPtr SetParent(IntPtr hWndChild, IntPtr hWndNewParent);

	[DllImport("user32")]
	public static extern IntPtr SetWindowLong(IntPtr hWnd, int nIndex, long flags);

	[DllImport("user32")]
	public static extern IntPtr SetWindowLong(IntPtr hWnd, uint nIndex, Win32WndProc newProc);
	
	[DllImport("user32")]
	public static extern int CallWindowProc(IntPtr lpPrevWndFunc, IntPtr hWnd, int Msg, int wParam, int lParam);

	[DllImport("user32.dll")]
	public static extern bool LockWindowUpdate(IntPtr hWndLock);

	[DllImport("user32.dll")]
	public static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

	[DllImport("user32.dll", SetLastError = true)]
	public static extern long GetWindowLong(IntPtr hWnd, uint nIndex);

	[DllImport("user32.dll")]
	public static extern IntPtr BeginDeferWindowPos(int nNumWindows);

	[DllImport("user32.dll", SetLastError = true)]
	public static extern IntPtr DeferWindowPos(IntPtr hWinPosInfo, IntPtr hWnd,
		IntPtr hWndInsertAfter, int x, int y, int cx, int cy, uint uFlags);

	[DllImport("user32.dll")]
	public static extern bool EndDeferWindowPos(IntPtr hWinPosInfo);

	[DllImport("user32.dll", SetLastError = true)]
	public static extern IntPtr GetWindow(IntPtr hWnd, GetWindow_Cmd uCmd);

	[DllImport("user32.dll")]
	public static extern bool EnableWindow(IntPtr hWnd, bool bEnable);

	[DllImport("user32.dll")]
	public static extern IntPtr SetFocus(IntPtr hWnd);

	[DllImport("user32.dll")]
	public static extern IntPtr SetCursor(IntPtr hcur);

	[DllImport("user32.dll")]
	public static extern IntPtr LoadCursor(IntPtr hInstcance, SystemCursor hcur);

	[DllImport("user32.dll")]
	public static extern IntPtr GetDC(IntPtr hWnd);

	[DllImport("gdi32.dll")]
	public static extern IntPtr CreateCompatibleDC(IntPtr hDC);

	[DllImport("gdi32.dll")]
	public static extern IntPtr SelectObject(IntPtr hDC, IntPtr hObject);

	[DllImport("gdi32.dll")]
	public static extern bool DeleteObject(IntPtr hObject);

	[DllImport("gdi32.dll")]
	public static extern bool DeleteDC(IntPtr hdc);

	[DllImport("user32.dll")]
	public static extern int ReleaseDC(IntPtr hWnd, IntPtr hDC);

	[DllImport("user32.dll", CharSet = CharSet.Auto)]
	public static extern int GetWindowTextLength(IntPtr hWnd);
	
	[DllImport("user32.dll", CharSet = CharSet.Auto)]
	public static extern int GetWindowText(IntPtr hWnd, StringBuilder lpString, int nMaxCount);

	[DllImport("user32.dll")]
	public static extern bool IsWindowVisible(IntPtr hWnd);

	[DllImport("user32.dll", ExactSpelling=true, SetLastError = true)]
	public static extern bool UpdateLayeredWindow(IntPtr hwnd, IntPtr hdcDst, ref Point pptDst, 
		ref Size psize, IntPtr hdcSrc, ref Point pprSrc, Int32 crKey, ref BLENDFUNCTION pblend, BlendFlags dwFlags);

	[DllImport("user32.dll")]
	public static extern IntPtr GetWindowDC(IntPtr hWnd);

	[DllImport("gdi32.dll", CallingConvention=CallingConvention.Winapi)]
	[return: MarshalAs(UnmanagedType.Bool)]
	public static extern bool BitBlt(IntPtr hdc, int nXDest, int nYDest, int nWidth, int nHeight, IntPtr hdcSrc, int nXSrc, int nYSrc, TernaryRasterOperations dwRop);

	[DllImport("gdi32.dll")]
	public static extern IntPtr CreateCompatibleBitmap(IntPtr hdc, int nWidth, int nHeight);

	[DllImport("gdi32.dll")]
	public static extern IntPtr CreateBitmap(int nWidth, int nHeight, uint cPlanes, uint cBitsPerPel, IntPtr lpvBits);
	
	[DllImport("gdi32.dll")]
	public static extern uint SetBkColor(IntPtr hdc, uint crColor);

	[DllImport("gdi32.dll")]
	public static extern IntPtr CreateSolidBrush(uint crColor);

	[DllImport("gdi32.dll")]
	public static extern uint GetPixel(IntPtr hdc, int nXPos, int nYPos);

	[DllImport("gdi32.dll")]
	public static extern bool MaskBlt(IntPtr hdcDest, int nXDest, int nYDest, int nWidth,
	   int nHeight, IntPtr hdcSrc, int nXSrc, int nYSrc, IntPtr hbmMask, int xMask,
	   int yMask, uint dwRop);
}
