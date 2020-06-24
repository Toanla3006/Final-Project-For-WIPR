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
using System.IO;

namespace MVC_DEMO.View
{
    public partial class TrashBinPage : Form
    {
        public int UserChange = 0;
        string Username;
        string NoteOrder;
        List<NOTE> ListOfTrash = new List<NOTE>();
        List<string> Colorname = new List<string>();
        int SetColorTheme = 0;
        public TrashBinPage()
        {
            InitializeComponent();
            LoadPage();
        }

        public void LoadPage()
        {
            this.comboBox1.Hide();
            this.comboBox2.Hide();
            this.numericUpDown1.Hide();
            dataGridView1.Hide();
            StartPosition = FormStartPosition.CenterScreen;
            flowLayoutPanel1.AutoScroll = true;
            System.Drawing.Text.FontCollection fontcoll = new System.Drawing.Text.InstalledFontCollection();    // Lấy giá trị tên các font chữ và cho vào combobox1 

            foreach (FontFamily font in fontcoll.Families)
            {
                this.comboBox1.Items.Add(font.Name);
            }
            foreach (KnownColor color in Enum.GetValues(typeof(KnownColor))) // Lấy giá trị các màu sắc và cho vào combobox2  - KnownColor là thuộc tính có sẵn của class Color trong C# Winform nhằm lấy các giá trị màu sắc đã có sẵn
            {
                comboBox2.Items.Add(color); // Chỉ lấy tên của từng màu sắc 
                Colorname.Add(color.ToString());    // Đưa các từng giá trị của combobox2 vào List Colorname đã được khai báo ở trên để tiện việc thao tác xử lí màu sắc và tránh lỗi tham chiếu. Lỗi sẽ được ghi chú phía bên dưới trong phần NoteBox_Click
            }
            comboBox2.SelectedIndex = 0;    // Đưa giá trị combobox về lại index 0 _ tức là giá trị đầu tiên
            comboBox1.SelectedIndex = 0;    // Đưa giá trị combobox về lại index 0 _ tức là giá trị đầu tiên
            LoadTimeForClock();
            panel2.Hide();
            panel3.Hide();
            panel8.Hide();
            pictureBox8.Hide();
            panel12.Hide();
        }
        public TrashBinPage(string User) : this()
        {
            Username = User;
            GetTrashFromController();
        }

        public void GetTrashFromController()
        {
            TrashBinPageController LoadNote = new TrashBinPageController();
            dataGridView1.DataSource = LoadNote.GetAllTrash(Username);
            for (int i = 0; i < dataGridView1.RowCount; i++)
            {
                NOTE N = new NOTE();
                N.PpOrder = Int32.Parse(dataGridView1[0, i].Value.ToString());
                N.PpHeader = dataGridView1[1, i].Value.ToString();
                N.PpContent = dataGridView1[2, i].Value.ToString();
                N.PpTag = dataGridView1[3, i].Value.ToString();
                N.PpFontFamily = dataGridView1[4, i].Value.ToString();
                N.PpFontSize = Int32.Parse(dataGridView1[5, i].Value.ToString());
                N.PpFontColor = dataGridView1[6, i].Value.ToString();
                if (dataGridView1[7, i].Value != null)
                {
                    N.PpPictureName = dataGridView1[7, i].Value.ToString();
                }
                if (dataGridView1[8, i].Value != null)
                {
                    N.PpDrawPictureName = dataGridView1[8, i].Value.ToString();
                }
                ListOfTrash.Add(N);
            }
            LoadNoteForPage();
        }

        private void LoadNoteForPage()
        {
            for (int i = 0; i < ListOfTrash.Count; i++)  // Tạo ra các note nằm trong sidebar để người dùng sử dụng
            {
                Label NoteBox = new Label()
                {
                    AutoSize = false,
                    TextAlign = ContentAlignment.MiddleLeft,
                    Dock = DockStyle.None
                };
                var margin = NoteBox.Margin;
                margin.Top = 10;
                NoteBox.Margin = margin;
                if (SetColorTheme == 1)
                {
                    NoteBox.ForeColor = Color.White;
                }
                else
                {
                    NoteBox.ForeColor = Color.Black;
                }
                NoteBox.Size = new System.Drawing.Size(190, 25);
                NoteBox.Text = ListOfTrash[i].PpHeader;
                NoteBox.Tag = ListOfTrash[i].PpOrder.ToString();
                NoteBox.Click += NoteBox_Click;
                flowLayoutPanel1.Controls.Add(NoteBox);
                Panel Ruler = new Panel();
                Ruler.Size = new System.Drawing.Size(190, 2);
                if (SetColorTheme == 1)
                {
                    Ruler.BackColor = Color.White;
                }
                else
                {
                    Ruler.BackColor = Color.Black;
                }
                flowLayoutPanel1.Controls.Add(Ruler);
            }
        }

        private void NoteBox_Click(object sender, EventArgs e)
        {
            this.Cursor = Cursors.WaitCursor;
            this.pictureBox2.BackgroundImage = null;
            this.pictureBox2.CreateGraphics().Clear(Color.White);
            this.pictureBox4.BackgroundImage = null;
            NoteOrder = (sender as Label).Tag.ToString(); // Dữ liệu lấy từ NoteBox.Tag ở phía trên ***
            string Condition = NoteOrder; // Biến này nhằm để tìm kiếm nội dung của note có giá trị Header
            if (Condition == "")    // Nếu là note mới khởi tạo thì khi click lên sẽ có giá trị nội dung như phía dưới
            {
                textBox1.Text = "New Note";
                textBox2.Text = "This is your new Note...";
            }
            else
            {
                int SelectedIndex = 0;  // Biến này dùng để xử lí combobox 2 - combobox màu sắc, để khi click lên note thì combobox sẽ quay về đúng giá trị màu hiện tại mà note đang sử dụng
                for (int i = 0; i < ListOfTrash.Count; i++)
                {
                    if (ListOfTrash[i].PpOrder.ToString() == Condition)
                    {
                        if (ListOfTrash[i].PpDrawPictureName != "")
                        {
                            if (System.IO.File.Exists(ListOfTrash[i].PpDrawPictureName))
                            {
                                this.pictureBox2.BackgroundImage = Image.FromFile(String.Format(ListOfTrash[i].PpDrawPictureName));
                            }
                        }
                        if (ListOfTrash[i].PpPictureName != "")
                        {
                            if (System.IO.File.Exists(ListOfTrash[i].PpPictureName))
                            {
                                this.pictureBox4.BackgroundImage = Image.FromFile(String.Format(ListOfTrash[i].PpPictureName));
                            }
                        }
                        textBox1.Text = ListOfTrash[i].PpHeader;
                        textBox2.Text = ListOfTrash[i].PpContent;    // Nội dung của note
                        textBox2.Font = new Font(ListOfTrash[i].PpFontFamily, ListOfTrash[i].PpFontSize); // Hiển thị font chữ của note
                        textBox2.ForeColor = Color.FromName(ListOfTrash[i].PpFontColor.ToString());  // Hiển thị màu chữ của note
                        comboBox1.SelectedItem = ListOfTrash[i].PpFontFamily; // Chuyển đổi combobox1 - combobox font chữ, khi click lên note thì thay đổi về giá trị font và note đang sử dụng
                        for (int j = 0; j < Colorname.Count; j++) // Chuyển đổi combobox2 - combobox màu sắc,  khi click lên note thì combobox sẽ quay về đúng giá trị màu hiện tại mà note đang sử dụng
                        {
                            // Lỗi: Không thể dùng câu lệnh comboBox2.SelectedItem = ListOfNote[i].PpFontColor; vì xuất hiện lỗi tham chiếu - không hiểu lí do nên phải dùng cách này: Tạo List lưu giá trị lại và thao tác 
                            if (ListOfTrash[i].PpFontColor.ToString() == Colorname[j].ToString())    // Nếu tên màu sắc của note giống tên màu sắc có trong form thì lưu lại index của nó
                            {
                                SelectedIndex = j;
                            }
                        }
                        comboBox2.SelectedIndex = SelectedIndex; // Khi này click lên note thì note sẽ có màu sắc đúng do sự kiện: combobox2.SelectedIndexChange phía dưới, thay đổi màu sắc của textbox3 giống với màu sắc đã được lưu trữ tương ứng với note đó 
                        numericUpDown1.Value = ListOfTrash[i].PpFontSize;    // Lấy size chữ của note
                    }
                }
            }
            panel2.Show();
            panel3.Show();
            panel8.Show();
            this.Cursor = Cursors.Default;
        }

        private void LoadTimeForClock()
        {
            string Hour = DateTime.Now.Hour.ToString();
            string Minute = DateTime.Now.Minute.ToString();
            string Second = DateTime.Now.Second.ToString();
            if (DateTime.Now.Hour < 10)
            {
                Hour = "0" + DateTime.Now.Hour;
            }
            if (DateTime.Now.Minute < 10)
            {
                Minute = "0" + DateTime.Now.Minute;
            }
            if (DateTime.Now.Second < 10)
            {
                Second = "0" + DateTime.Now.Second;
            }
            label4.Text = Hour + ":" + Minute + ":" + Second;
            timer1.Start();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            int NewNoteOrder = 0;
            for(int i=0;i<ListOfTrash.Count;i++)
            {
                if(ListOfTrash[i].PpOrder.ToString() == NoteOrder)
                {
                    TrashBinPageController TrashPage = new TrashBinPageController();
                    TrashPage.Restore(Username, textBox1.Text,ListOfTrash[i].PpOrder, NewNoteOrder, textBox2.Text, textBox3.Text, ListOfTrash[i].PpFontFamily, comboBox2.SelectedItem.ToString(), ListOfTrash[i].PpFontSize, ListOfTrash[i].PpPictureName, ListOfTrash[i].PpDrawPictureName);
                    flowLayoutPanel1.Controls.Clear();
                }
            }
            ListOfTrash.Clear();
            GetTrashFromController();
            panel2.Hide();
            panel3.Hide();
            textBox1.Clear();
            textBox2.Clear();
            textBox3.Clear();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            TrashBinPageController TrashPage = new TrashBinPageController();
            TrashPage.DeleteNote(Username, NoteOrder);
            panel2.Hide();
            panel3.Hide();
            flowLayoutPanel1.Controls.Clear();
            ListOfTrash.Clear();
            GetTrashFromController();
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            string Hour = DateTime.Now.Hour.ToString();
            string Minute = DateTime.Now.Minute.ToString();
            string Second = DateTime.Now.Second.ToString();
            if (DateTime.Now.Hour < 10)
            {
                Hour = "0" + DateTime.Now.Hour;
            }
            if (DateTime.Now.Minute < 10)
            {
                Minute = "0" + DateTime.Now.Minute;
            }
            if (DateTime.Now.Second < 10)
            {
                Second = "0" + DateTime.Now.Second;
            }
            label4.Text = Hour + ":" + Minute + ":" + Second;
        }

        private void pictureBox6_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void pictureBox9_Click(object sender, EventArgs e)
        {
            this.Hide();
        }

        private void pictureBox5_Click(object sender, EventArgs e)
        {
            UserChange = 1;
            this.Hide();
        }

        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string TextAbout = "Programme Name: Note Programme \nCreator: Group 3 \nDate Create: 01/04/2020\nLast Update: 26/05/2020";
            MessageBox.Show(TextAbout, "About", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        int Toggle = 0;
        private void fullScreenToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (Toggle == 0)
            {
                WindowState = FormWindowState.Maximized;
                FullScreenSet();
                Toggle = 1;
            }
            else
            {
                WindowState = FormWindowState.Normal;
                NormalScreenSet();
                Toggle = 0;
            }
        }

        public void FullScreenSet()
        {
            fullScreenToolStripMenuItem.Text = "Normal Screen";
            int width = Screen.GetWorkingArea(this).Width;
            int height = Screen.GetWorkingArea(this).Height;
            panel4.Size = new Size(width - 20, panel4.Height);
            panel5.Location = new Point(panel5.Location.X, height);
            panel5.Size = new Size(width - 20, panel5.Height);
            panel11.Size = new Size(panel11.Width, panel11.Height + 330);
            panel7.Size = new Size(panel7.Width, panel7.Height + 330);
            flowLayoutPanel1.Size = new Size(flowLayoutPanel1.Width, flowLayoutPanel1.Height + 330);
            panel3.Location = new Point(width - panel3.Width - 20, panel3.Location.Y);
            panel2.Size = new Size(panel2.Width + 490, panel2.Height + 330);
            textBox1.Size = new Size(textBox1.Width + 480, textBox1.Height);
            textBox2.Size = new Size(textBox2.Width + 480, textBox2.Height + 310);
            label6.Location = new Point(label6.Location.X, panel2.Height - 30);
            textBox3.Location = new Point(textBox3.Location.X, panel2.Height - 30);
            pictureBox6.Location = new Point(width - 60, pictureBox6.Location.Y);
            pictureBox10.Location = new Point(width - 110, pictureBox10.Location.Y);
            pictureBox5.Location = new Point(width - 160, pictureBox5.Location.Y);
            label4.Location = new Point(width - 70, label4.Location.Y);
            button1.Location = new Point(panel2.Width - 230, panel2.Height - 30);
            button3.Location = new Point(panel2.Width - 115, panel2.Height - 30);
            ToggleFullScreen = 1;
        }

        public void NormalScreenSet()
        {
            fullScreenToolStripMenuItem.Text = "Full Screen";
            panel4.Size = new Size(1020, 38);
            panel5.Location = new Point(panel5.Location.X, 490);
            panel5.Size = new Size(1020, panel5.Height);
            panel11.Size = new Size(panel11.Width, panel11.Height - 330);
            panel7.Size = new Size(panel7.Width, panel7.Height - 330);
            flowLayoutPanel1.Size = new Size(flowLayoutPanel1.Width, flowLayoutPanel1.Height + 330);
            panel3.Location = new Point(1015 - panel3.Width + 15, panel3.Location.Y);
            panel2.Size = new Size(570, panel2.Height - 330);
            textBox1.Size = new Size(570, textBox1.Height);
            textBox2.Size = new Size(570, textBox2.Height - 310);
            label6.Location = new Point(label6.Location.X, panel2.Height - 20);
            textBox3.Location = new Point(textBox3.Location.X, panel2.Height - 20);
            panel8.Location = new Point(407, 460);
            pictureBox6.Location = new Point(1010 - 40, pictureBox6.Location.Y);
            pictureBox10.Location = new Point(1010 - 80, pictureBox10.Location.Y);
            pictureBox5.Location = new Point(1010 - 120, pictureBox5.Location.Y);
            label4.Location = new Point(1020 - 50, label4.Location.Y);
            button1.Location = new Point(355, panel2.Height - 25);
            button3.Location = new Point(465, panel2.Height - 25);
            ToggleFullScreen = 0;
        }


        private void minimizeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            WindowState = FormWindowState.Minimized;
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        OpenFileDialog open;
        private void exportFiletxtToolStripMenuItem_Click(object sender, EventArgs e)
        {
            open = new OpenFileDialog();    // thiết lập mở cửa sổ 
            string Path = ""; // Biến lưu đường dẫn
            FolderBrowserDialog folder = new FolderBrowserDialog(); // mở folder để chọn đường dẫn 
            folder.ShowNewFolderButton = true;
            DialogResult Result = folder.ShowDialog();
            if (Result == DialogResult.OK)
            {
                Path = folder.SelectedPath; // Lưu lại đường dẫn 
                Environment.SpecialFolder root = folder.RootFolder;
            }
            else
            {
                return;
            }
            string FileName = textBox1.Text + ".txt"; // Chuyển header thành tên file
            StreamWriter writer = new StreamWriter(Path + @"\" + FileName); // Lưu file vào đường dẫn 
            writer.WriteLine(textBox2.Text); // Nội dung lưu
            writer.Close(); // Ngắt kết nối
            MessageBox.Show("File has been saved to: " + Path, "Annouce: ", MessageBoxButtons.OK, MessageBoxIcon.Information); // Thông Báo Người Dùng
        }

        private void copyToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Clipboard.SetText(textBox2.Text);
        }

        private void cutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Clipboard.SetText(textBox2.Text);
            textBox2.Text = "";
        }

        private void selectAllToolStripMenuItem_Click(object sender, EventArgs e)
        {
            textBox2.SelectAll();
            textBox2.Focus();
        }

        private void pasteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (Clipboard.ContainsText(TextDataFormat.Text))
            {
                textBox2.Text = Clipboard.GetText(TextDataFormat.Text);
            }
        }

        string Oldtext = "";
        private void undoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Oldtext = textBox2.Text;
            textBox2.Undo();
            undoToolStripMenuItem.Enabled = false;
            redoToolStripMenuItem.Enabled = true;
        }

        private void redoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            textBox2.Text = Oldtext;
            undoToolStripMenuItem.Enabled = true;
            redoToolStripMenuItem.Enabled = false;
        }

        private void darkToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SetColorTheme = 1;
            this.BackColor = Color.Black;
            panel7.BackColor = Color.White;
            panel11.BackColor = Color.White;
            label4.BackColor = Color.White;
            pictureBox2.BackColor = Color.White;
            pictureBox4.BackColor = Color.White;
            label6.ForeColor = Color.White;
            flowLayoutPanel1.Controls.Clear();
            LoadNoteForPage();
        }

        private void lightToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SetColorTheme = 0;
            this.BackColor = Color.White;
            panel7.BackColor = Color.Black;
            panel11.BackColor = Color.Black;
            pictureBox2.BackColor = Color.White;
            pictureBox4.BackColor = Color.White;
            label6.ForeColor = Color.Black;
            flowLayoutPanel1.Controls.Clear();
            LoadNoteForPage();
        }

        private void pictureBox10_Click(object sender, EventArgs e)
        {
            WindowState = FormWindowState.Minimized;
        }

        int ToggleSideBar = 0;
        int ToggleFullScreen = 0;
        private void hideSideBarToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SetSideBar();
        }

        public void SetSideBar()
        {
            if (ToggleSideBar == 0 && ToggleFullScreen == 0)
            {
                hideSideBarToolStripMenuItem.Text = "Show Sidebar";
                fullScreenToolStripMenuItem.Enabled = false;
                panel1.Hide();
                panel2.Location = new Point(10, panel2.Location.Y);
                panel2.Size = new Size(790, panel2.Height);
                panel7.Hide();
                panel11.Hide();
                panel10.Hide();
                textBox1.Size = new Size(790, textBox1.Height);
                textBox2.Size = new Size(790, textBox2.Height);
                button1.Location = new Point(575, button1.Location.Y);
                button3.Location = new Point(685, button3.Location.Y);
                ToggleSideBar = 1;
            }
            else if (ToggleSideBar == 1 && ToggleFullScreen == 0)
            {
                hideSideBarToolStripMenuItem.Text = "Hide Sidebar";
                fullScreenToolStripMenuItem.Enabled = true;
                panel1.Show();
                panel2.Location = new Point(230, panel2.Location.Y);
                panel2.Size = new Size(570, panel2.Height);
                panel7.Show();
                panel11.Show();
                panel10.Show();
                textBox1.Size = new Size(570, textBox1.Height);
                textBox2.Size = new Size(570, textBox2.Height);
                button1.Location = new Point(355, button1.Location.Y);
                button3.Location = new Point(465, button3.Location.Y);
                ToggleSideBar = 0;
            }
            else if (ToggleSideBar == 0 && ToggleFullScreen == 1)
            {
                hideSideBarToolStripMenuItem.Text = "Show Sidebar";
                fullScreenToolStripMenuItem.Enabled = false;
                panel1.Hide();
                panel2.Location = new Point(10, panel2.Location.Y);
                panel2.Size = new Size(1270, panel2.Height);
                panel7.Hide();
                panel11.Hide();
                panel10.Hide();
                textBox1.Size = new Size(1270, textBox1.Height);
                textBox2.Size = new Size(1270, textBox2.Height);
                button1.Location = new Point(panel2.Width - 220, panel2.Height - 30);
                button3.Location = new Point(panel2.Width - 105, panel2.Height - 30);
                ToggleSideBar = 1;
            }
            else
            {
                hideSideBarToolStripMenuItem.Text = "Hide Sidebar";
                fullScreenToolStripMenuItem.Enabled = true;
                panel1.Show();
                panel2.Location = new Point(230, panel2.Location.Y);
                panel2.Size = new Size(1050, panel2.Height);
                panel7.Show();
                panel11.Show();
                panel10.Show();
                textBox1.Size = new Size(1050, textBox1.Height);
                textBox2.Size = new Size(1050, textBox2.Height);
                button1.Location = new Point(1000, button1.Location.Y);
                button3.Location = new Point(1100, button3.Location.Y);
                button1.Location = new Point(panel2.Width - 220, panel2.Height - 30);
                button3.Location = new Point(panel2.Width - 105, panel2.Height - 30);
                ToggleSideBar = 0;
            }
        }

        private void pictureBox7_MouseHover(object sender, EventArgs e)
        {
            panel12.Show();
        }

        private void label9_Click(object sender, EventArgs e)
        {
            panel12.Hide();
        }

        private void button8_Click(object sender, EventArgs e)
        {
            int ConditionLoop = 0; // Xử lí vòng lặp
            string FindValue = textBox4.Text; // lấy giá trị từ textbox4 - textbox tìm kiếm để lưu trữ và thao tác trên biến.
            if (textBox4.Text == "") // Nếu người dùng chưa nhập giá trị thì báo lỗi và quay lại
            {
                MessageBox.Show("Please enter value for searching", "Warning: ", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            flowLayoutPanel1.Controls.Clear(); // Xóa vùng hiển thị các note đi để chuẩn bị hiển thị lại CHỈ các note đang tìm kiếm
            for (int i = 0; i < ListOfTrash.Count; i++) // Duyệt qua danh sách các note
            {
                if (ListOfTrash[i].PpHeader == FindValue || ListOfTrash[i].PpTag == FindValue) // Nếu cùng Header hoặc cùng Tag (Chủ Đề) 
                {
                    Label NoteBox = new Label()
                    {
                        AutoSize = false,
                        TextAlign = ContentAlignment.MiddleLeft,
                        Dock = DockStyle.None
                    };
                    if (SetColorTheme == 1)
                    {
                        NoteBox.ForeColor = Color.White;
                    }
                    else
                    {
                        NoteBox.ForeColor = Color.Black;
                    }
                    var margin = NoteBox.Margin;
                    margin.Top = 10;
                    NoteBox.Margin = margin;
                    NoteBox.Size = new System.Drawing.Size(190, 25);
                    NoteBox.Text = ListOfTrash[i].PpHeader;
                    NoteBox.Tag = ListOfTrash[i].PpOrder.ToString();
                    NoteBox.Click += NoteBox_Click;
                    flowLayoutPanel1.Controls.Add(NoteBox);
                    Panel Ruler = new Panel();
                    Ruler.Size = new System.Drawing.Size(190, 2);
                    if (SetColorTheme == 1)
                    {
                        Ruler.BackColor = Color.White;
                    }
                    else
                    {
                        Ruler.BackColor = Color.Black;
                    }
                    flowLayoutPanel1.Controls.Add(Ruler);
                    ConditionLoop = 1; // Nếu không có bước này thì sau khi chạy xong chương trình sẽ chạy thêm câu lệnh Message Box ở dưới, điều này chỉ xảy ra nếu như duyệt hết list mà không có nội dung tương tự đang tìm kiếm
                    textBox4.Clear();
                    pictureBox8.Show();
                }
            }
            if (ConditionLoop == 0)
                MessageBox.Show("Your looking note does not exist", "Warning: ", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            panel12.Hide();
        }

        private void pictureBox8_Click(object sender, EventArgs e)
        {
            flowLayoutPanel1.Controls.Clear();
            ListOfTrash.Clear();
            GetTrashFromController();
            pictureBox8.Hide();
            panel2.Hide();
            panel3.Hide();
            panel8.Hide();
        }

        private void TrashBinPage_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.F11)
            {
                if (Toggle == 0)
                {
                    WindowState = FormWindowState.Maximized;
                    FullScreenSet();
                    Toggle = 1;
                }
                else
                {
                    WindowState = FormWindowState.Normal;
                    NormalScreenSet();
                    Toggle = 0;
                }
            }
        }
    }
}
