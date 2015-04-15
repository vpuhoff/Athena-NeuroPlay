using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

using AlphaForm;

namespace TV_Set
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
    alphaFormTransformer1.Fade(FadeType.FadeOut, true,
      false, 500);

    base.OnClosing(e);
    }

    private void Form1_Load(object sender, EventArgs e)
    {
      alphaFormTransformer1.TransformForm(0);
      System.Reflection.Assembly myAssembly = System.Reflection.Assembly.GetExecutingAssembly();
      System.IO.Stream picStream = myAssembly.GetManifestResourceStream("TV_Set.tvpic1.jpg");
      pictureBox1.Image = new Bitmap(picStream);
    }

    private void button3_Click(object sender, EventArgs e)
    {
      Close();
    }

    private void button2_Click(object sender, EventArgs e)
    {
      System.Reflection.Assembly myAssembly = System.Reflection.Assembly.GetExecutingAssembly();
      System.IO.Stream picStream = myAssembly.GetManifestResourceStream("TV_Set.tvpic2.jpg");
      pictureBox1.Image = new Bitmap(picStream);
    }

    private void button1_Click(object sender, EventArgs e)
    {
      System.Reflection.Assembly myAssembly = System.Reflection.Assembly.GetExecutingAssembly();
      System.IO.Stream picStream = myAssembly.GetManifestResourceStream("TV_Set.tvpic1.jpg");
      pictureBox1.Image = new Bitmap(picStream);
    }

    private void button4_Click(object sender, EventArgs e)
    {

      alphaFormTransformer1.SetAlphaAndParentFormVisible(false);

    }
  }
}