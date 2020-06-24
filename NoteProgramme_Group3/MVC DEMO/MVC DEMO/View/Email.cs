using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;           // Thư viện dùng để sử dụng tính năng gửi Email
using System.Net.Mail;      // Thư viện dùng để sử dụng tính năng gửi Email
using System.Windows.Forms;
using System.IO;
using MVC_DEMO.Controller;

namespace MVC_DEMO.View
{
    public partial class Email : Form
    {
        Attachment Attach1;
        Attachment Attach2;
        string EHeader = "";
        string EContent = "";
        string ENoteDraw = "";
        string ENotePic = "";
        public Email()
        {
            InitializeComponent();
            StartPosition = FormStartPosition.CenterScreen;
        }

        public Email(string Header, string Content, string NoteDraw, string NotePic):this()
        {
            LogInController GetEmail = new LogInController();
            EHeader = Header;
            EContent = Content;
            ENoteDraw = NoteDraw;
            ENotePic = NotePic;
            textBox1.Text = EContent;
            textBox5.Text = EHeader;
            if(System.IO.File.Exists(ENoteDraw))
            {
                pictureBox1.BackgroundImage = Image.FromFile(String.Format(ENoteDraw));
            }
            if(System.IO.File.Exists(ENotePic))
            {
                pictureBox2.BackgroundImage = Image.FromFile(String.Format(ENotePic));
            }                 
        }

        void GuiMail(string from, string to, string subject, string message, Attachment fill1, Attachment fill2)    // Hàm này dùng để gửi email
        {
            if (to == "")
            {
                MessageBox.Show("Please enter receiver account", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            if(from == "")
            {
                MessageBox.Show("Please enter your account", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            MailMessage mess = new MailMessage(from, to, subject, message);
            if(Attach1 != null)
            {
                mess.Attachments.Add(Attach1);
            }
            if(Attach2 != null)
            {
                mess.Attachments.Add(Attach2);
            }
            SmtpClient client = new SmtpClient("smtp.gmail.com", 587);  // Thiết lập kết nối
            client.EnableSsl = true;
            if(textBox2.Text == "" || textBox3.Text == "")
            {
                MessageBox.Show("Please enter username and password of your Email", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            else
            {
                client.Credentials = new NetworkCredential(textBox2.Text, textBox3.Text); // textbox 2 là Username Email, textbox 3 là Password Email
            }
            try
            {
                client.Send(mess);
                MessageBox.Show("The Note is sent to your email", "Annouce: ", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch
            {
                MessageBox.Show("Cannot Connect to your email account", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.Hide();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Attach1 = null;
            try
            {
                FileInfo file = new FileInfo(ENotePic);
                Attach1 = new Attachment(ENotePic);
            }
            catch{ }
            Attach2 = null;
            try
            {
                FileInfo file = new FileInfo(ENoteDraw);
                Attach2 = new Attachment(ENoteDraw);
            }
            catch { }
            GuiMail(textBox2.Text, textBox4.Text, EHeader, EContent,Attach1,Attach2);
        }
    }
}
