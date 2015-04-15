using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using AlphaForms;

namespace TestApp
{
	public partial class Form1 : AlphaForm
	{
		public Form1()
		{
			InitializeComponent();
		}

		private void Form1_Load(object sender, EventArgs e)
		{
			DrawControlBackground(this.picBoxClose, false);
			UpdateLayeredBackground();

			//For convenience when switching between the on / off images
			m_closeBtnOff = this.picBoxClose.Image;
			m_closeBtnOn = this.picBoxClose.BackgroundImage;
		}

		private void picBoxClose_MouseEnter(object sender, EventArgs e)
		{
			this.picBoxClose.Image = m_closeBtnOn;
			this.picBoxClose.BackgroundImage = m_closeBtnOff;
		}

		private void picBoxClose_MouseLeave(object sender, EventArgs e)
		{
			this.picBoxClose.BackgroundImage = m_closeBtnOn;
			this.picBoxClose.Image = m_closeBtnOff;
		}

		private void picBoxClose_Click(object sender, EventArgs e)
		{
			this.Close();
		}

		private Image m_closeBtnOff;
		private Image m_closeBtnOn;

		private void button1_Click(object sender, EventArgs e)
		{
			UpdateLayeredBackground();
            AlphaForm_Extended1 frm = new AlphaForm_Extended1();
            frm.ShowDialog();
            this.Close();
		}
	}
}
