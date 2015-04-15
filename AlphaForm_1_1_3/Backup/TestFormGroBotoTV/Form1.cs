using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

using AlphaForm;

namespace TestForm
{
	public partial class Form1 : Form
	{
		public Form1()
		{
	    InitializeComponent();
		}
			
		protected override void OnPaintBackground(PaintEventArgs e) {}
    protected override void OnClosing(CancelEventArgs e)
    {
      // On XP, the speed at which the main form fades is 
      // distractingly slow because of its size (see my note in OnShown below),
      // so we just fade out the layered window frame. 
      // NOTE: If you have the main form with a background image,
      // that matches for AFT's background image (i.e., controls on top),
      // you'll always want to fade in/out both windows.
      
      alphaFormTransformer1.Fade(FadeType.FadeOut, true, 
        System.Environment.OSVersion.Version.Major < 6, 500);
     
      base.OnClosing(e);
    }

    protected override void OnShown(EventArgs e)
    {
      // I'm not real pleased with the speed that XP fades in
      // the main form when the form itself is somewhat large like
      // this one. Apparently when you have a Region, changing
      // the layered window opacity attribute doesn't draw very fast
      // for large regions. So here we only fade in the layered window.
      // NOTE: If you have the main form with a background image,
      // that matches for AFT's background image (sans alpha channel),
      // you'll always want to fade in/out both, windows otherwise 
      // it will look ugly. Look at the ControlsOnTopOfSkin project
      // where we've added calls to Fade().
      
       alphaFormTransformer1.Fade(FadeType.FadeIn, false,
        System.Environment.OSVersion.Version.Major < 6, 400);
      base.OnShown(e);
    }

		private void Form1_Load(object sender, EventArgs e)
		{
			alphaFormTransformer1.TransformForm(0); // must pass 0 if we're fading in (set's layered window's opacity)
		}

		private void offButton_Click(object sender, EventArgs e)
		{
			Close();
		}

		private void surfButton_Click(object sender, EventArgs e)
		{
			if (String.IsNullOrEmpty(surfURL.Text))
				return;
			if (surfURL.Text.Equals("about:blank"))
				return;
			if (!surfURL.Text.StartsWith("http://"))
				surfURL.Text = "http://" + surfURL.Text;
			try
				{
				webBrowser.Navigate(new Uri(surfURL.Text));
				}
			catch (System.UriFormatException)
				{
				return;
				}
		}

    private void blogLink_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
    {
      surfURL.Text = "http://kilroytrout.blogspot.com";
      infoLabel.Text = "KilroyTrout";
      try
      {
        webBrowser.Navigate(new Uri(surfURL.Text));
      }
      catch (System.UriFormatException)
      {
        return;
      }
    }

    private void rdLink_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
    {
      surfURL.Text = "http://www.braid.com/GBPlayerDemo2/GBPlayerDemo2.html";
      infoLabel.Text = "Braid Labs - 3D Desktop Gadgets in the Works";
      try
      {
        webBrowser.Navigate(new Uri(surfURL.Text));
      }
      catch (System.UriFormatException)
      {
        return;
      }

    }


    private void gbLink_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
    {
      surfURL.Text = "http://www.groboto.com";
      infoLabel.Text = "Our Most Extraordinary 3D Software";
      try
      {
        webBrowser.Navigate(new Uri(surfURL.Text));
      }
      catch (System.UriFormatException)
      {
        return;
      }

    }

	}
}