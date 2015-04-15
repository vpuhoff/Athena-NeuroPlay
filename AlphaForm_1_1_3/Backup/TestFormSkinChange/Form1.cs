using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace TestFormSkinChange
{
  public partial class Form1 : Form
  {
    public Form1()
    {
      InitializeComponent();
    }

    private void skinButton2_Click(object sender, EventArgs e)
    {
      Bitmap bmap = TestFormSkinChange.Properties.Resources.skin2;
      alphaFormTransformer1.UpdateSkin(bmap,null,255);
    }

    private void skinButton1_Click(object sender, EventArgs e)
    {
      Bitmap bmap = TestFormSkinChange.Properties.Resources.skin1;
      alphaFormTransformer1.UpdateSkin(bmap,null,255);
    }

    private void Form1_Load(object sender, EventArgs e)
    {
      alphaFormTransformer1.TransformForm(255);
    }

    private void closeButton_Click(object sender, EventArgs e)
    {
      Close();
    }
  }
}