using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Media;

namespace MVC_DEMO.View
{
    public partial class Annoucement : Form
    {
        SoundPlayer player;
        public Annoucement()
        {
            InitializeComponent();
            timer1.Start();
            //player = new SoundPlayer(@"C:\Users\Tin\Desktop\Winform\MVC DEMO\A.wav");
            //player.Play();
            StartPosition = FormStartPosition.Manual;
            int width = Screen.GetWorkingArea(this).Width;
            int height = Screen.GetWorkingArea(this).Height;
            Location = new Point(width - this.Width, height - this.Height);
        }
        public Annoucement(string Header, string Content, string DrawPic,string PicturePath):this()
        {
            label1.Text = Header;
            label2.Text = Content;
            if (System.IO.File.Exists(DrawPic))
            {
                this.pictureBox2.BackgroundImage = Image.FromFile(String.Format(DrawPic));
            }
            if (System.IO.File.Exists(PicturePath))
            {
                this.pictureBox1.BackgroundImage = Image.FromFile(String.Format(PicturePath));
            }
        }

        int i = 0;
        private void timer1_Tick(object sender, EventArgs e)
        {
            if(i==10)
            {
                this.Hide();
                timer1.Stop();
            }
            i++;
        }

        private void label3_Click(object sender, EventArgs e)
        {
            this.Hide();
            //player.Stop();
        }
    }
}
