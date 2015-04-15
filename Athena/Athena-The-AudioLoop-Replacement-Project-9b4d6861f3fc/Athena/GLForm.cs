using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using AlphaForms;

namespace Athena
{
    public partial class GLForm : AlphaForms.AlphaForm 
    {
        public GLForm()
        {
            InitializeComponent();
        }

        private void GLForm_Load(object sender, EventArgs e)
        {
            UpdateLayeredBackground();
        }

        private void alphaFormTransformer1_Paint(object sender, PaintEventArgs e)
        {

        }

        //protected override void OnPaintBackground(PaintEventArgs e) { }
        //protected override void OnClosing(CancelEventArgs e)
        //{
        //    // On XP, the speed at which the main form fades is 
        //    // distractingly slow because of its size (see my note in OnShown below),
        //    // so we just fade out the layered window frame. 
        //    // NOTE: If you have the main form with a background image,
        //    // that matches for AFT's background image (i.e., controls on top),
        //    // you'll always want to fade in/out both windows.

        //    alphaFormTransformer1.Fade(FadeType.FadeOut, true,
        //      System.Environment.OSVersion.Version.Major < 6, 500);

        //    base.OnClosing(e);
        //}

        //protected override void OnShown(EventArgs e)
        //{
        //    // I'm not real pleased with the speed that XP fades in
        //    // the main form when the form itself is somewhat large like
        //    // this one. Apparently when you have a Region, changing
        //    // the layered window opacity attribute doesn't draw very fast
        //    // for large regions. So here we only fade in the layered window.
        //    // NOTE: If you have the main form with a background image,
        //    // that matches for AFT's background image (sans alpha channel),
        //    // you'll always want to fade in/out both, windows otherwise 
        //    // it will look ugly. Look at the ControlsOnTopOfSkin project
        //    // where we've added calls to Fade().

        //    alphaFormTransformer1.Fade(FadeType.FadeIn, false,
        //     System.Environment.OSVersion.Version.Major < 6, 400);
        //    base.OnShown(e);
        //}
    }
}
