using MVC_DEMO.Controller;
using MVC_DEMO.Model;
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
using System.Drawing.Imaging;
using System.Speech;
using System.Speech.Synthesis;

namespace MVC_DEMO.View
{
    public partial class NotePage : Form
    {
        string NoteOrder;
        string Username;
        string PicturePath = "";
        string Header_Modify_Problem;
        string Content_From_TXT;
        int PictureNotExisted = 0;
        List<NOTE> ListOfNote = new List<NOTE>();
        List<string> Colorname = new List<string>();
        private DrawItem drawnote;
        private Bitmap BitmapNote;
        private DrawItem drawnote1;
        private int SetColorTheme = 0;
        public NotePage()
        {
            InitializeComponent();
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
            BitmapNote = new Bitmap(this.pictureBox2.ClientSize.Width, this.pictureBox2.ClientSize.Height, this.pictureBox2.CreateGraphics());
            drawnote = new DrawItem();
            undoToolStripMenuItem.Enabled = false;
            redoToolStripMenuItem.Enabled = false;
            dataGridView2.Hide();
            dataGridView1.Hide();
            panel12.Hide();
        }
        public NotePage(string User) : this()
        {
            Username = User;
            GetNoteFromController();
        }

        public void GetNoteFromController()
        {
            NotePageController LoadNote = new NotePageController();
            dataGridView1.DataSource = LoadNote.GetAllNote(Username);
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
                ListOfNote.Add(N);
            }
            LoadNoteForPage();
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
        private void LoadNoteForPage()
        {
            for (int i = 0; i < ListOfNote.Count; i++)  // Tạo ra các note nằm trong sidebar để người dùng sử dụng
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
                NoteBox.Text = ListOfNote[i].PpHeader;
                NoteBox.Tag = ListOfNote[i].PpOrder.ToString();
                NoteBox.Click += NoteBox_Click;
                NoteBox.DoubleClick += NoteBox_DoubleClick;
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

        private int UnExpectedSituationVariable = 0;
        private void NoteBox_DoubleClick(object sender, EventArgs e)
        {
            UnExpectedSituationVariable = 1;
        }

        private void NoteBox_Click(object sender, EventArgs e)
        {
            this.Cursor = Cursors.WaitCursor;
            textBox3.Clear();
            A = 0;
            this.pictureBox2.BackgroundImage = null;
            BitmapNote = new Bitmap(this.pictureBox2.ClientSize.Width, this.pictureBox2.ClientSize.Height, this.pictureBox2.CreateGraphics());
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
                for (int i = 0; i < ListOfNote.Count; i++)
                {
                    if (ListOfNote[i].PpOrder.ToString() == Condition)
                    {
                        if (ListOfNote[i].PpDrawPictureName != "")
                        {
                            if (System.IO.File.Exists(ListOfNote[i].PpDrawPictureName))
                            {
                                this.pictureBox2.BackgroundImage = Image.FromFile(String.Format(ListOfNote[i].PpDrawPictureName));
                                BitmapNote = new Bitmap(this.pictureBox2.ClientSize.Width, this.pictureBox2.ClientSize.Height, this.pictureBox2.CreateGraphics());
                                BitmapNote = (Bitmap)pictureBox2.BackgroundImage;
                                drawnote1 = new DrawItem();
                            }
                            else
                            {
                                PictureNotExisted = 1;
                                BitmapNote = new Bitmap(this.pictureBox2.ClientSize.Width, this.pictureBox2.ClientSize.Height, this.pictureBox2.CreateGraphics());
                                drawnote1 = new DrawItem();
                            }
                        }
                        if (ListOfNote[i].PpPictureName != "")
                        {
                            if (System.IO.File.Exists(ListOfNote[i].PpPictureName))
                            {
                                this.pictureBox4.BackgroundImage = Image.FromFile(String.Format(ListOfNote[i].PpPictureName));
                            }
                        }
                        textBox1.Text = ListOfNote[i].PpHeader;
                        textBox2.Text = ListOfNote[i].PpContent;    // Nội dung của note
                        textBox2.Font = new Font(ListOfNote[i].PpFontFamily, ListOfNote[i].PpFontSize); // Hiển thị font chữ của note
                        textBox2.ForeColor = Color.FromName(ListOfNote[i].PpFontColor.ToString());  // Hiển thị màu chữ của note
                        textBox3.Text = ListOfNote[i].PpTag;
                        comboBox1.SelectedItem = ListOfNote[i].PpFontFamily; // Chuyển đổi combobox1 - combobox font chữ, khi click lên note thì thay đổi về giá trị font và note đang sử dụng
                        for (int j = 0; j < Colorname.Count; j++) // Chuyển đổi combobox2 - combobox màu sắc,  khi click lên note thì combobox sẽ quay về đúng giá trị màu hiện tại mà note đang sử dụng
                        {
                            // Lỗi: Không thể dùng câu lệnh comboBox2.SelectedItem = ListOfNote[i].PpFontColor; vì xuất hiện lỗi tham chiếu - không hiểu lí do nên phải dùng cách này: Tạo List lưu giá trị lại và thao tác 
                            if (ListOfNote[i].PpFontColor.ToString() == Colorname[j].ToString())    // Nếu tên màu sắc của note giống tên màu sắc có trong form thì lưu lại index của nó
                            {
                                SelectedIndex = j;
                            }
                        }
                        comboBox2.SelectedIndex = SelectedIndex; // Khi này click lên note thì note sẽ có màu sắc đúng do sự kiện: combobox2.SelectedIndexChange phía dưới, thay đổi màu sắc của textbox3 giống với màu sắc đã được lưu trữ tương ứng với note đó 
                        numericUpDown1.Value = ListOfNote[i].PpFontSize;    // Lấy size chữ của note
                        PicturePath = ListOfNote[i].PpPictureName;
                        if (ListOfNote[i].PpPictureName == "")
                        {
                            button1.Text = "Attach Image";
                        }
                        else
                        {
                            button1.Text = "Modify Image";
                        }
                        NotePageController CheckImportant = new NotePageController();
                        if (CheckImportant.ExistImportant(Username, NoteOrder) == true)
                        {
                            button7.Text = "Non-Important";
                        }
                        else
                        {
                            button7.Text = "Important";
                        }
                    }
                }
            }
            panel2.Show();
            panel3.Show();
            panel8.Show();
            panel9.Hide();
            this.Cursor = Cursors.Default;
        }

        public class NOTE
        {
            public int Order;
            public string Header;
            public string Content;
            public string Tag;
            public string FontFamily;
            public int FontSize;
            public string FontColor;
            public string PictureName;
            public string DrawPictureName;
            public int PpOrder { get; set; }
            public string PpHeader { get; set; }
            public string PpContent { get; set; }
            public string PpTag { get; set; }
            public string PpFontFamily { get; set; }
            public int PpFontSize { get; set; }
            public string PpFontColor { get; set; }
            public string PpPictureName { get; set; }
            public string PpDrawPictureName { get; set; }
        }

        public class NOTE1 : NOTE
        {
            public int PpImportantOrder { get; set; }
        }
        public class DrawItem
        {
            public int X { get; set; }
            public int Y { get; set; }
            public Color color { get; set; }
            public Pen pen { get; set; }
            public bool IsDraw { get; set; }
            public DrawItem()
            {
                color = Color.Black;
                pen = new Pen(this.color, 2);
                IsDraw = false;
            }
        }

        OpenFileDialog open;
        private void button1_Click(object sender, EventArgs e)
        {
            open = new OpenFileDialog(); // Mở Cửa sổ window của my document, file explorer
            open.Filter = "|*.jpg"; // Filter các file có giá trị .txt
            open.Multiselect = false; // Không cho chọn nhiều file để tránh lỗi
            if (open.ShowDialog() == DialogResult.OK) // Mở thành công thì...
            {
                pictureBox4.BackgroundImage = Image.FromFile(open.FileName);
                PicturePath = open.FileName;
            }
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

        private void pictureBox5_Click(object sender, EventArgs e)
        {
            this.Hide();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            int NewNoteOrder = 0;   // Biến thứ tự của note mới dùng để truyền vào SQL 
            if (textBox1.Text == "" || textBox1.Text == "New Note" || textBox1.Text == "Note from.txt")
            {
                MessageBox.Show("Please enter the note header", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            int Size = Int32.Parse(numericUpDown1.Value.ToString());
            string Font_Family = comboBox1.Text;
            string PicName = "";
            NotePageController AddOn = new NotePageController();
            PicName = AddOn.AddNote(Username, textBox1.Text, NewNoteOrder, textBox2.Text, textBox3.Text, Font_Family, comboBox2.SelectedItem.ToString(), Size, PicturePath, PicName);
            BitmapNote.Save(String.Format(PicName));
            BitmapNote.Dispose();
            ListOfNote.Clear();
            flowLayoutPanel1.Controls.Clear();
            textBox1.Clear();
            textBox2.Clear();
            comboBox1.SelectedIndex = 0;
            comboBox2.SelectedIndex = 0;
            numericUpDown1.Value = 8;
            PicturePath = "";
            BitmapNote = new Bitmap(this.pictureBox2.ClientSize.Width, this.pictureBox2.ClientSize.Height, this.pictureBox2.CreateGraphics());
            this.pictureBox2.CreateGraphics().Clear(Color.White);
            this.pictureBox4.BackgroundImage = null;
            GetNoteFromController();
            panel2.Hide();
            panel3.Hide();
            panel8.Hide();
            panel9.Show();
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            textBox2.Font = new Font(comboBox1.Text, Int32.Parse(numericUpDown1.Value.ToString()));
        }

        private void numericUpDown1_ValueChanged(object sender, EventArgs e)
        {
            textBox2.Font = new Font(comboBox1.Text, Int32.Parse(numericUpDown1.Value.ToString()));
        }

        private void comboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (comboBox2.SelectedItem != null)
            {
                string R = comboBox2.SelectedItem.ToString();
                textBox2.ForeColor = Color.FromName(R);
            }
        }

        private void button5_Click(object sender, EventArgs e)
        {
            string Header = "";
            string Content = "";
            string PicPath = "";
            string DrawPath = "";
            for (int i = 0; i < ListOfNote.Count; i++)
            {
                if (ListOfNote[i].PpOrder.ToString() == NoteOrder)
                {
                    Header = ListOfNote[i].PpHeader;
                    Content = ListOfNote[i].PpContent;
                    DrawPath = ListOfNote[i].PpDrawPictureName;
                    PicPath = ListOfNote[i].PpPictureName;
                }
            }
            Set_Alarm SetAlarm = new Set_Alarm(Header, Content, DrawPath, PicPath);
            this.Hide();
            SetAlarm.ShowDialog();
            this.Show();
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {
            ListOfNote.Clear();
            flowLayoutPanel1.Controls.Clear();
            GetNoteFromController();
            Label NoteBox1 = new Label()
            {
                AutoSize = false,
                TextAlign = ContentAlignment.MiddleLeft,
                Dock = DockStyle.None
            };
            var margin = NoteBox1.Margin;
            margin.Top = 10;
            NoteBox1.Margin = margin;
            if (SetColorTheme == 1)
            {
                NoteBox1.ForeColor = Color.White;
            }
            else
            {
                NoteBox1.ForeColor = Color.Black;
            }
            NoteBox1.Size = new System.Drawing.Size(190, 25);
            NoteBox1.Text = "New Note";
            NoteBox1.Tag = "New Note";
            NoteBox1.Click += NoteBox1_Click;
            flowLayoutPanel1.Controls.Add(NoteBox1);
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
            textBox1.Text = "New Note";
            textBox2.Text = "This is your new Note...";
            BitmapNote = new Bitmap(this.pictureBox2.ClientSize.Width, this.pictureBox2.ClientSize.Height, this.pictureBox2.CreateGraphics());
            this.pictureBox2.CreateGraphics().Clear(Color.White);
            this.pictureBox4.BackgroundImage = null;
            button1.Text = "Attach Image";
            panel2.Show();
            panel3.Show();
            panel8.Hide();
            panel9.Hide();
            PicturePath = "";
            comboBox1.SelectedIndex = 0;
            comboBox2.SelectedIndex = 0;
            numericUpDown1.Value = 8;
        }

        private void NoteBox1_Click(object sender, EventArgs e)
        {
            string NoteHeader = (sender as Label).Tag.ToString();
            Header_Modify_Problem = NoteHeader;
            textBox1.Text = NoteHeader;
            textBox2.Text = "This is your new Note...";
            if (textBox1.Text == "Note from .txt")
            {
                textBox2.Text = Content_From_TXT;
            }
            BitmapNote = new Bitmap(this.pictureBox2.ClientSize.Width, this.pictureBox2.ClientSize.Height, this.pictureBox2.CreateGraphics());
            this.pictureBox2.CreateGraphics().Clear(Color.White);
            this.pictureBox4.BackgroundImage = null;
            panel8.Hide();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            NotePageController DeleteNote = new NotePageController();
            DeleteNote.DeleteNote(Username, NoteOrder);
            flowLayoutPanel1.Controls.Clear(); // Xóa các Panel khỏi flowlayoutpanel giống câu lệnh xóa LIST
            ListOfNote.Clear();
            GetNoteFromController();
            comboBox1.SelectedIndex = 0;
            comboBox2.SelectedIndex = 0;
            numericUpDown1.Value = 8;
            BitmapNote = new Bitmap(this.pictureBox2.ClientSize.Width, this.pictureBox2.ClientSize.Height, this.pictureBox2.CreateGraphics());
            this.pictureBox2.CreateGraphics().Clear(Color.White);
            this.pictureBox4.BackgroundImage = null;
            textBox1.Clear();
            textBox2.Clear();
            panel2.Hide();
            panel3.Hide();
            panel8.Hide();
            panel9.Show();
        }

        private void button6_Click(object sender, EventArgs e)
        {
            int Size = Int32.Parse(numericUpDown1.Value.ToString());
            string Font_Family = comboBox1.Text;
            string COLORVARIABLE = comboBox2.SelectedItem.ToString();
            string PicName = "";
            if (textBox1.Text == "" || textBox1.Text == "New Note")
            {
                MessageBox.Show("Please enter the note header", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            NotePageController ModifyNote = new NotePageController();
            string PicName0 = ModifyNote.ModifyNote(Username, NoteOrder, textBox1.Text, textBox2.Text, textBox3.Text, Font_Family, COLORVARIABLE, Size, PicturePath, ref PicName);
            if (PictureNotExisted == 0)
            {
                SaveImage(PicName0, PicName);
            }
            else
            {
                BitmapNote.Save(String.Format(PicName0));
                BitmapNote.Dispose();
                PictureNotExisted = 0;
            }
            textBox1.Clear();
            textBox2.Clear();
            ListOfNote.Clear();
            flowLayoutPanel1.Controls.Clear();
            GetNoteFromController();
            this.pictureBox2.CreateGraphics().Clear(Color.White);
            this.pictureBox4.BackgroundImage = null;
            panel2.Hide();
            panel3.Hide();
            panel8.Hide();
            panel9.Show();
            comboBox1.SelectedIndex = 0;
            comboBox2.SelectedIndex = 0;
            numericUpDown1.Value = 8;
            MessageBox.Show("Note has been overwrite", "Annouce", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        public void SaveImage(string PicName0, string PicName)
        {
            if (A == 1)
            {
                BitmapNote = (Bitmap)pictureBox2.BackgroundImage;
                BitmapNote.Save(PicName0);
                this.pictureBox2.BackgroundImage.Dispose();
                this.pictureBox2.BackgroundImage = null;
                this.BackgroundImage = null;
                if (File.Exists(PicName))
                {
                    BitmapNote.Dispose();
                    if (UnExpectedSituationVariable != 1)
                    {
                        File.Delete(PicName);
                    }
                    else
                    {
                        UnExpectedSituationVariable = 0;
                    }
                }
            }
        }

        private void pictureBox3_Click(object sender, EventArgs e)
        {
            panel2.Hide();
            panel3.Hide();
            panel8.Hide();
            panel9.Show();
            textBox1.Clear();
            textBox2.Clear();
            flowLayoutPanel1.Controls.Clear();
            List<NOTE1> ImportantNotes = new List<NOTE1>();
            NotePageController Important = new NotePageController();
            dataGridView2.DataSource = Important.GetImportantNotes(Username);
            if (dataGridView2.DataSource != null)
            {
                for (int i = 0; i < dataGridView2.RowCount; i++)
                {
                    NOTE1 N = new NOTE1();
                    N.PpImportantOrder = Int32.Parse(dataGridView2[0, i].Value.ToString());
                    N.PpHeader = dataGridView2[2, i].Value.ToString();
                    N.PpContent = dataGridView2[3, i].Value.ToString();
                    N.PpTag = dataGridView2[4, i].Value.ToString();
                    N.PpFontFamily = dataGridView2[5, i].Value.ToString();
                    N.PpFontSize = Int32.Parse(dataGridView2[6, i].Value.ToString());
                    N.PpFontColor = dataGridView2[7, i].Value.ToString();
                    if (dataGridView2[8, i].Value.ToString() != "")
                    {
                        N.PpPictureName = dataGridView2[8, i].Value.ToString();
                    }
                    if (dataGridView2[9, i].Value.ToString() != "")
                    {
                        N.PpDrawPictureName = dataGridView2[9, i].Value.ToString();
                    }
                    N.PpOrder = Int32.Parse(dataGridView2[1, i].Value.ToString());
                    ImportantNotes.Add(N);
                }
                for (int i = 0; i < ImportantNotes.Count; i++) // Duyệt qua danh sách các note
                {
                    Label NoteBox = new Label()
                    {
                        AutoSize = false,
                        TextAlign = ContentAlignment.MiddleLeft,
                        Dock = DockStyle.None
                    };
                    var margin = NoteBox.Margin;
                    margin.Top = 10;
                    if (SetColorTheme == 1)
                    {
                        NoteBox.ForeColor = Color.White;
                    }
                    else
                    {
                        NoteBox.ForeColor = Color.Black;
                    }
                    NoteBox.Margin = margin;
                    NoteBox.Size = new System.Drawing.Size(190, 25);
                    NoteBox.Text = ImportantNotes[i].PpHeader;
                    NoteBox.Tag = ImportantNotes[i].PpOrder.ToString();
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
                pictureBox8.Show();
            }
        }

        private void button7_Click(object sender, EventArgs e)
        {
            if (button7.Text == "Important")
            {
                NotePageController AddImportant = new NotePageController();
                if (AddImportant.AddImportantNote(Username, NoteOrder) == false)
                {
                    MessageBox.Show("Important list is full !!!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                else
                {
                    button7.Text = "Non-Important";
                }
            }
            else
            {
                NotePageController DeleteImportant = new NotePageController();
                DeleteImportant.DeleteImportant(Username, NoteOrder);
                ListOfNote.Clear();
                flowLayoutPanel1.Controls.Clear();
                GetNoteFromController();
                pictureBox8.Hide();
                panel2.Hide();
                panel3.Hide();
                panel8.Hide();
                panel9.Show();
            }
        }

        private void pictureBox8_Click(object sender, EventArgs e)
        {
            flowLayoutPanel1.Controls.Clear();
            ListOfNote.Clear();
            GetNoteFromController();
            pictureBox8.Hide();
            panel2.Hide();
            panel3.Hide();
            panel8.Hide();
            panel9.Show();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            string DrawPic = "";
            string NotePic = "";
            for (int i = 0; i < ListOfNote.Count; i++)
            {
                if (ListOfNote[i].PpOrder.ToString() == NoteOrder)
                {
                    DrawPic = ListOfNote[i].PpDrawPictureName;
                    NotePic = ListOfNote[i].PpPictureName;
                }
            }
            Email EmailForm = new Email(textBox1.Text, textBox2.Text, DrawPic, NotePic);
            this.Hide();
            EmailForm.ShowDialog();
            this.Show();
        }

        private void pictureBox7_MouseHover(object sender, EventArgs e)
        {
            panel12.Show();
        }

        private int A = 0;
        private void pictureBox2_MouseMove(object sender, MouseEventArgs e)
        {
            A = 1;
            if (drawnote.IsDraw)
            {
                Graphics G = this.pictureBox2.CreateGraphics();
                G.DrawLine(drawnote.pen, drawnote.X, drawnote.Y, e.X, e.Y);
                using (Graphics gr = Graphics.FromImage(BitmapNote))
                {
                    gr.DrawLine(drawnote.pen, drawnote.X, drawnote.Y, e.X, e.Y);
                }
                drawnote.X = e.X;
                drawnote.Y = e.Y;
            }
        }
        private void pictureBox2_MouseUp(object sender, MouseEventArgs e)
        {
            drawnote.IsDraw = false;
        }
        private void pictureBox2_MouseDown(object sender, MouseEventArgs e)
        {
            drawnote.IsDraw = true;
            drawnote.X = e.X;
            drawnote.Y = e.Y;
        }

        private void pictureBox9_Click(object sender, EventArgs e)
        {
            TrashBinPage TrashBin = new TrashBinPage(Username);
            this.Hide();
            TrashBin.ShowDialog();
            this.Show();
            int UserChange = TrashBin.UserChange;
            if (UserChange == 1)
            {
                this.Hide();
            }
            flowLayoutPanel1.Controls.Clear();
            ListOfNote.Clear();
            GetNoteFromController();
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void readFiletxtToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ListOfNote.Clear();
            flowLayoutPanel1.Controls.Clear();
            GetNoteFromController();
            open = new OpenFileDialog(); // Mở Cửa sổ window của my document, file explorer
            open.Filter = "|*.txt"; // Filter các file có giá trị .txt
            open.Multiselect = false; // Không cho chọn nhiều file để tránh lỗi
            if (open.ShowDialog() == DialogResult.OK) // Mở thành công thì...
            {
                StreamReader read = new StreamReader(open.FileName); // Lấy File
                textBox2.Text = read.ReadToEnd(); // Đọc File
                Content_From_TXT = textBox2.Text;
                read.Close(); // Đóng kết nối
            }
            // Tạo ra note mới chứ file txt vừa thêm vào.
            Label NoteBox1 = new Label()
            {
                AutoSize = false,
                TextAlign = ContentAlignment.MiddleLeft,
                Dock = DockStyle.None
            };
            var margin = NoteBox1.Margin;
            margin.Top = 10;
            NoteBox1.Margin = margin;
            NoteBox1.Size = new System.Drawing.Size(190, 25);
            textBox1.Text = "Note from .txt";
            NoteBox1.Text = "Note from .txt";
            NoteBox1.Tag = "Note from .txt";
            NoteBox1.Click += NoteBox1_Click;
            flowLayoutPanel1.Controls.Add(NoteBox1);
            Panel Ruler = new Panel();
            Ruler.Size = new System.Drawing.Size(190, 2);
            Ruler.BackColor = Color.Black;
            flowLayoutPanel1.Controls.Add(Ruler);
            BitmapNote = new Bitmap(this.pictureBox2.ClientSize.Width, this.pictureBox2.ClientSize.Height, this.pictureBox2.CreateGraphics());
            this.pictureBox2.CreateGraphics().Clear(Color.White);
            this.pictureBox4.BackgroundImage = null;
            button1.Text = "Attach Image";
            panel2.Show();
            panel3.Show();
            panel8.Hide();
            panel9.Hide();
            PicturePath = "";
            comboBox1.SelectedIndex = 0;
            comboBox2.SelectedIndex = 0;
            numericUpDown1.Value = 8;
            panel2.Show();
            panel3.Show();
            panel8.Show();
            panel9.Hide();
        }

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
            string FileName = textBox1.Text; // Chuyển header thành tên file
            if(System.IO.File.Exists(Path + @"\" + FileName + ".txt"))
            {
                int ID = 0;
                while (System.IO.File.Exists(Path + @"\" + FileName + ID.ToString() + ".txt"))
                {
                    ID++;
                }
                FileName = textBox1.Text + "_" + ID.ToString() + ".txt";
            }
            else
            {
                FileName = textBox1.Text + ".txt";
            }
            StreamWriter writer = new StreamWriter(Path + @"\" + FileName); // Lưu file vào đường dẫn 
            writer.WriteLine(textBox2.Text); // Nội dung lưu
            writer.Close(); // Ngắt kết nối
            MessageBox.Show("File has been saved to: " + Path, "Annouce: ", MessageBoxButtons.OK, MessageBoxIcon.Information); // Thông Báo Người Dùng
        }

        private void newNoteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ListOfNote.Clear();
            flowLayoutPanel1.Controls.Clear();
            GetNoteFromController();
            Label NoteBox1 = new Label()
            {
                AutoSize = false,
                TextAlign = ContentAlignment.MiddleLeft,
                Dock = DockStyle.None
            };
            var margin = NoteBox1.Margin;
            margin.Top = 10;
            NoteBox1.Margin = margin;
            NoteBox1.Size = new System.Drawing.Size(190, 25);
            NoteBox1.Text = "New Note";
            NoteBox1.Tag = "New Note";
            NoteBox1.Click += NoteBox1_Click;
            flowLayoutPanel1.Controls.Add(NoteBox1);
            Panel Ruler = new Panel();
            Ruler.Size = new System.Drawing.Size(190, 2);
            Ruler.BackColor = Color.Black;
            flowLayoutPanel1.Controls.Add(Ruler);
            textBox1.Text = "New Note";
            textBox2.Text = "This is your new Note...";
            BitmapNote = new Bitmap(this.pictureBox2.ClientSize.Width, this.pictureBox2.ClientSize.Height, this.pictureBox2.CreateGraphics());
            this.pictureBox2.CreateGraphics().Clear(Color.White);
            this.pictureBox4.BackgroundImage = null;
            button1.Text = "Attach Image";
            panel2.Show();
            panel3.Show();
            panel8.Hide();
            panel9.Hide();
            PicturePath = "";
            comboBox1.SelectedIndex = 0;
            comboBox2.SelectedIndex = 0;
            numericUpDown1.Value = 8;
        }

        private void minimizeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Minimized;
        }

        private void pictureBox10_Click(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Minimized;
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
            pictureBox8.Hide();
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
            pictureBox8.Hide();

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

        private void changeFontToolStripMenuItem_Click(object sender, EventArgs e)
        {
            comboBox1.SelectedIndex++;
            textBox2.Font = new Font(comboBox1.Text, Int32.Parse(numericUpDown1.Value.ToString()));
        }

        private void changeColorToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (comboBox2.SelectedItem != null)
            {
                comboBox2.SelectedIndex++;
                string R = comboBox2.SelectedItem.ToString();
                textBox1.ForeColor = Color.FromName(R);
                textBox2.ForeColor = Color.FromName(R);
            }
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

        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string TextAbout = "Programme Name: Note Programme \nCreator: Group 3 \nDate Create: 01/04/2020 \nLast Update: 26/05/2020";
            MessageBox.Show(TextAbout, "About", MessageBoxButtons.OK, MessageBoxIcon.Information);
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

        private void textBox2_TextChanged(object sender, EventArgs e)
        {
            undoToolStripMenuItem.Enabled = true;
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

        int ToggleFullScreen = 0;
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
            panel8.Location = new Point(panel8.Location.X + 480, panel8.Location.Y + 325);
            button2.Location = new Point(button2.Location.X + 480, button2.Location.Y + 325);
            pictureBox6.Location = new Point(width - 60, pictureBox6.Location.Y);
            pictureBox10.Location = new Point(width - 110, pictureBox10.Location.Y);
            pictureBox5.Location = new Point(width - 160, pictureBox5.Location.Y);
            label4.Location = new Point(width - 70, label4.Location.Y);
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
            button2.Location = new Point(500, 370);
            pictureBox6.Location = new Point(1010 - 40, pictureBox6.Location.Y);
            pictureBox10.Location = new Point(1010 - 80, pictureBox10.Location.Y);
            pictureBox5.Location = new Point(1010 - 120, pictureBox5.Location.Y);
            label4.Location = new Point(1020 - 50, label4.Location.Y);
            ToggleFullScreen = 0;
        }

        int ToggleSideBar = 0;
        private void hideSidebarToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (ToggleSideBar == 0 && ToggleFullScreen == 0)
            {
                hideSidebarToolStripMenuItem.Text = "Show Sidebar";
                fullScreenToolStripMenuItem.Enabled = false;
                panel1.Hide();
                panel2.Location = new Point(10, panel2.Location.Y);
                panel2.Size = new Size(790, panel2.Height);
                panel7.Hide();
                panel11.Hide();
                panel10.Hide();
                textBox1.Size = new Size(790, textBox1.Height);
                textBox2.Size = new Size(790, textBox2.Height);
                ToggleSideBar = 1;
            }
            else if (ToggleSideBar == 1 && ToggleFullScreen == 0)
            {
                hideSidebarToolStripMenuItem.Text = "Hide Sidebar";
                fullScreenToolStripMenuItem.Enabled = true;
                panel1.Show();
                panel2.Location = new Point(230, panel2.Location.Y);
                panel2.Size = new Size(570, panel2.Height);
                panel7.Show();
                panel11.Show();
                panel10.Show();
                textBox1.Size = new Size(570, textBox1.Height);
                textBox2.Size = new Size(570, textBox2.Height);
                ToggleSideBar = 0;
            }
            else if (ToggleSideBar == 0 && ToggleFullScreen == 1)
            {
                hideSidebarToolStripMenuItem.Text = "Show Sidebar";
                fullScreenToolStripMenuItem.Enabled = false;
                panel1.Hide();
                panel2.Location = new Point(10, panel2.Location.Y);
                panel2.Size = new Size(1270, panel2.Height);
                panel7.Hide();
                panel11.Hide();
                panel10.Hide();
                textBox1.Size = new Size(1270, textBox1.Height);
                textBox2.Size = new Size(1270, textBox2.Height);
                ToggleSideBar = 1;
            }
            else
            {
                hideSidebarToolStripMenuItem.Text = "Hide Sidebar";
                fullScreenToolStripMenuItem.Enabled = true;
                panel1.Show();
                panel2.Location = new Point(230, panel2.Location.Y);
                panel2.Size = new Size(1050, panel2.Height);
                panel7.Show();
                panel11.Show();
                panel10.Show();
                textBox1.Size = new Size(1050, textBox1.Height);
                textBox2.Size = new Size(1050, textBox2.Height);
                ToggleSideBar = 0;
            }
        }

        private void pictureBox7_Click(object sender, EventArgs e)
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
            for (int i = 0; i < ListOfNote.Count; i++) // Duyệt qua danh sách các note
            {
                if (ListOfNote[i].PpHeader == FindValue || ListOfNote[i].PpTag == FindValue) // Nếu cùng Header hoặc cùng Tag (Chủ Đề) 
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
                    NoteBox.Text = ListOfNote[i].PpHeader;
                    NoteBox.Tag = ListOfNote[i].PpOrder.ToString();
                    NoteBox.Click += NoteBox_Click;
                    NoteBox.DoubleClick += NoteBox_DoubleClick;
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

        private void NotePage_KeyDown(object sender, KeyEventArgs e)
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

        SpeechSynthesizer reader = new SpeechSynthesizer();
        int ToggleSpeaker = 0;
        private void pictureBox11_Click(object sender, EventArgs e)
        {
            if (ToggleSpeaker == 0)
            {
                if (textBox2.Text != "")
                {
                    reader.Dispose();
                    reader = new SpeechSynthesizer();
                    reader.SpeakAsync(textBox2.Text);
                }
                else
                {
                    MessageBox.Show("Can not read","Error",MessageBoxButtons.OK,MessageBoxIcon.Error);
                }
                ToggleSpeaker = 1;
            }
            else
            {
                if(reader != null)
                {
                    reader.Dispose();
                }
                ToggleSpeaker = 0;
            }
        }
    }
    public class DrawItem
    {
        public int X { get; set; }
        public int Y { get; set; }
        public Color color { get; set; }
        public Pen pen { get; set; }
        public bool IsDraw { get; set; }
        public DrawItem()
        {
            color = Color.Black;
            pen = new Pen(this.color, 2);
            IsDraw = false;
        }
    }
}
