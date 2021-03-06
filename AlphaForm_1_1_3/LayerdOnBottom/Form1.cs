﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

using AlphaForm;

namespace LayerdOnBottom
{
	public partial class Form1 : Form
	{
		public Form1()
		{
		InitializeComponent();
		}
		
		protected override void OnShown(EventArgs e)
    {
			alphaFormTransformer1.Fade(FadeType.FadeIn, false,
			false, 500);
			base.OnShown(e);
    }

    protected override void OnClosing(CancelEventArgs e)
    {
			//alphaFormTransformer1.Fade(FadeType.FadeOut, true,
      //false, 500);

			base.OnClosing(e);
    }

		private void Form1_Load(object sender, EventArgs e)
		{
			alphaFormTransformer1.TransformForm2(0);
		}

		private void Form1_FormClosing(object sender, FormClosingEventArgs e)
		{
			alphaFormTransformer1.Fade(FadeType.FadeOut, true,
       false, 500);
		}
	}
}
