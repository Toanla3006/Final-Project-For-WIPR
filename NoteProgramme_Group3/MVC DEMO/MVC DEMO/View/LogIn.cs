using MVC_DEMO.Controller;
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
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            LoadPage();
        }
        public void LoadPage()
        {
            StartPosition = FormStartPosition.CenterScreen;
            panel6.Hide();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            LogInController LoadInformation = new LogInController();
            if(LoadInformation.LogIn(textBox1.Text, textBox2.Text) == true)
            {
                NotePage NotePage = new NotePage(textBox1.Text);
                this.Hide();
                NotePage.ShowDialog();
                this.Show();
            }
            else
            {
                MessageBox.Show("Username or Password does not correct","Error:",MessageBoxButtons.OK,MessageBoxIcon.Error);
                textBox1.Text = "Username";
                textBox2.Text = "Password";
            }
        }

        private void textBox1_Enter(object sender, EventArgs e)
        {
            if (textBox1.Text == "Username")
            {
                textBox1.Text = "";
                textBox1.ForeColor = Color.Black;
            }
        }

        private void textBox1_Leave(object sender, EventArgs e)
        {
            if (textBox1.Text == "")
            {
                textBox1.Text = "Username";
                textBox1.ForeColor = Color.Gray;
            }
        }

        private void label3_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void textBox2_Enter(object sender, EventArgs e)
        {
            if (textBox2.Text == "Password")
            {
                textBox2.Text = "";
                textBox2.UseSystemPasswordChar = true;
                textBox2.ForeColor = Color.Black;
            }
        }

        private void textBox2_Leave(object sender, EventArgs e)
        {
            if (textBox2.Text == "")
            {
                textBox2.Text = "Password";
                textBox2.UseSystemPasswordChar = false;
                textBox2.ForeColor = Color.Gray;
            }
        }

        int Toggle = 0;
        private void label2_Click(object sender, EventArgs e)
        {
            panel6.Show();
            if (Toggle == 0)
            {
                panel5.Location = new Point(panel5.Location.X + 120, panel5.Location.Y);
                Toggle = 1;
            }
        }

        private void label1_Click(object sender, EventArgs e)
        {
            panel6.Hide();
            if (Toggle == 1)
            {
                panel5.Location = new Point(panel5.Location.X - 120, panel5.Location.Y);
                Toggle = 0;
            }
        }

        private void textBox3_Enter(object sender, EventArgs e)
        {
            if (textBox3.Text == "Username")
            {
                textBox3.Text = "";
                textBox3.ForeColor = Color.Black;
            }
        }

        private void textBox3_Leave(object sender, EventArgs e)
        {
            if (textBox3.Text == "")
            {
                textBox3.Text = "Username";
                textBox3.ForeColor = Color.Gray;
            }
        }

        private void textBox4_Enter(object sender, EventArgs e)
        {
            if (textBox4.Text == "Password")
            {
                textBox4.Text = "";
                textBox4.UseSystemPasswordChar = true;
                textBox4.ForeColor = Color.Black;
            }
        }

        private void textBox4_Leave(object sender, EventArgs e)
        {
            if (textBox4.Text == "")
            {
                textBox4.Text = "Password";
                textBox4.UseSystemPasswordChar = false;
                textBox4.ForeColor = Color.Gray;
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            LogInController LoadInformation = new LogInController();
            if(LoadInformation.Regist(textBox3.Text, textBox4.Text) == true)
            {
                MessageBox.Show("Regist successful !!!", "Announce:", MessageBoxButtons.OK, MessageBoxIcon.Information);
                panel6.Hide();
                textBox3.Text = "Username";
                textBox4.Text = "Password";
                textBox4.UseSystemPasswordChar = false;
            }
            else
            {
                MessageBox.Show("This Username Unavailable", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
