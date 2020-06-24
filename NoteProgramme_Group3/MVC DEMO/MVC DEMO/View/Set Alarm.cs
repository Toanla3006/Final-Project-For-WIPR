using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MVC_DEMO.View
{
    public partial class Set_Alarm : Form
    {
        string NoteHead;
        string NoteContent;
        string DrawNote;
        string Picture;
        public Set_Alarm()
        {
            InitializeComponent();
            this.StartPosition = FormStartPosition.CenterScreen;
            LoadPage();
        }
        public Set_Alarm(string Header,string Content, string DrawPic, string PicturePath) : this()
        {
            NoteHead = Header;
            NoteContent = Content;
            DrawNote = DrawPic;
            Picture = PicturePath;
        }

        void LoadPage() // Load Giao Diện
        {
            dateTimePicker2.Hide();
            button2.Hide(); // Ẩn Button 2 đi, button 2 chỉ hiện lên khi người dùng thay đổi giá trị của 1 trong 3 numericupdown - cài đặt đếm ngược. Nếu người dùng thay đổi giá trị hẹn giờ thì button2 lại ẩn đi
        }

        private void numericUpDown1_ValueChanged(object sender, EventArgs e)
        {
            button2.Show();
        }

        private void numericUpDown2_ValueChanged(object sender, EventArgs e)
        {
            button2.Show();
        }

        private void numericUpDown3_ValueChanged(object sender, EventArgs e)
        {
            button2.Show();
        }

        private void dateTimePicker1_ValueChanged(object sender, EventArgs e)
        {
            button1.Show();
            button2.Hide();
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            if (numericUpDown3.Value > 0)
            {
                numericUpDown3.Value--;
            }
            else
            {
                numericUpDown3.Value = 59;
                if (numericUpDown2.Value > 0)
                {
                    numericUpDown2.Value--;
                }
                else
                {
                    numericUpDown2.Value = 59;
                    if (numericUpDown1.Value > 0)
                    {
                        numericUpDown1.Value--;
                    }
                    else
                    {
                        numericUpDown1.Value = 23;
                    }
                }
            }
            if (numericUpDown2.Value == 0 && numericUpDown3.Value == 0 && numericUpDown1.Value == 0)
            {
                timer1.Stop();
                Annoucement AnnounceForm = new Annoucement(NoteHead,NoteContent,DrawNote,Picture);
                AnnounceForm.Show();
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            timer2.Start();
            this.Hide();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            timer1.Start();
            this.Hide();
        }

        private void timer2_Tick(object sender, EventArgs e)
        {
            dateTimePicker2.Value = DateTime.Now;
            if (dateTimePicker1.Value.ToString() == dateTimePicker2.Value.ToString())
            {
                timer2.Stop();
                Annoucement AnnounceForm = new Annoucement(NoteHead, NoteContent, DrawNote, Picture);
                AnnounceForm.Show();
            }
        }
    }
}
