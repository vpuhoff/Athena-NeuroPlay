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
			
		}

    private void blogLink_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
    {
      
    }

    private void rdLink_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
    {
      

    }


    private void gbLink_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
    {
      

    }

    private void alphaFormTransformer1_Paint(object sender, PaintEventArgs e)
    {

    }

	}
}