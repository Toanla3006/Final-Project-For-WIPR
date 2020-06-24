using MVC_DEMO.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MVC_DEMO.Controller
{
    class NotePageController
    {
        public List<NOTE> GetAllNote(string Username)
        {
            WindowProgrammingSQLEntities db = new WindowProgrammingSQLEntities();
            var Result = (from c in db.List_Of_Notes where c.Username == Username select c)
                .Select(x => new NOTE
                {
                    PpOrder = x.NotesOrder,
                    PpHeader = x.NotesHeader,
                    PpContent = x.NotesContent,
                    PpTag = x.NoteTag,
                    PpFontFamily = x.FontFamily,
                    PpFontSize = (int)x.FontSize,
                    PpFontColor = x.NoteColor,
                    PpPictureName = x.NotePictureName,
                    PpDrawPictureName = x.NoteDrawPictureName
                });
            return Result.ToList();
        }

        public List<NOTE> GetAllTrash(string Username)
        {
            WindowProgrammingSQLEntities db = new WindowProgrammingSQLEntities();
            var Result = (from c in db.List_Of_TrashBin where c.Username == Username select c)
                .Select(x => new NOTE
                {
                    PpOrder = x.TrashOrder,
                    PpHeader = x.TrashHeader,
                    PpContent = x.TrashContent,
                    PpTag = x.TrashTag,
                    PpFontFamily = x.FontFamily,
                    PpFontSize = (int)x.FontSize,
                    PpFontColor = x.TrashColor,
                    PpPictureName = x.TrashPictureName,
                    PpDrawPictureName = x.TrashDrawPictureName
                });
            return Result.ToList();
        }

        public List<NOTE1> GetImportantNotes(string Username)
        {
            WindowProgrammingSQLEntities db = new WindowProgrammingSQLEntities();
            var Result = (from c in db.List_Of_ImportantNote where c.Username == Username select c)
                .Select(x => new NOTE1
                {
                    PpImportantOrder = x.ImportantOrder,
                    PpHeader = x.ImportantHeader,
                    PpContent = x.ImportantContent,
                    PpTag = x.ImportantTag,
                    PpFontFamily = x.FontFamily,
                    PpFontSize = (int)x.FontSize,
                    PpFontColor = x.NoteColor,
                    PpPictureName = x.NotePictureName,
                    PpDrawPictureName = x.NoteDrawPictureName,
                    PpOrder = x.NoteOrder
                });
            return Result.ToList();
        }

        public string AddNote(string Username, string NoteHeader, int NewNoteOrder, string NoteContent, string NoteTag, string FontFamily, string Color, int Size, string PicturePath, string DrawPicName)
        {
            List<NOTE> ListOfNote = new List<NOTE>();
            ListOfNote = GetAllNote(Username);
            int condition_NoteOrder = 0;    // Biến điều kiện để xử lí Thứ tự của Note 
            if (ListOfNote.Count == 0 || ListOfNote[0].PpOrder != 1)   // Xử lí xử lí Thứ tự của Note vì Thứ Tự và Username của note là primary key trong SQL - trùng lập sẽ tạo lỗi
            {
                NewNoteOrder = 1;   // Trường hợp chưa có note nào 
            }
            else
            {
                for (int i = 0; i < ListOfNote.Count - 1; i++)   // Trường hợp có note nhưng có note bị xóa làm thứ tự giữa các note có khoảng cách, ví dụ: đang note số 2 nhảy lên note số 4 thì không dc, phải thêm note số 3 vào giữa 2 và 4 có sẵn 
                {
                    if (ListOfNote[i + 1].PpOrder - ListOfNote[i].PpOrder > 1)
                    {
                        condition_NoteOrder = 1;
                        NewNoteOrder = ListOfNote[i].PpOrder + 1;
                    }
                }
                if (condition_NoteOrder == 0) // trường hợp này duyệt hết nhưng thứ tự các note liền nhau không bị đứt nên thêm vào sau
                {
                    NewNoteOrder = ListOfNote.Count + 1;
                }
            }
            DrawPicName = Username + "_" + NewNoteOrder.ToString() + ".jpg";
            using (WindowProgrammingSQLEntities db = new WindowProgrammingSQLEntities())
            {
                db.List_Of_Notes.Add(new List_Of_Notes() { Username = Username, NotesOrder = NewNoteOrder, NotesContent = NoteContent, NotesHeader = NoteHeader, NoteTag = NoteTag, FontFamily = FontFamily, FontSize = Size, NoteColor = Color, NoteDrawPictureName = DrawPicName, NotePictureName = PicturePath }); // Tạo note mới trên SQL và thêm vào
                db.SaveChanges(); // Save thay đổi trên SQL
            }
            return DrawPicName;
        }

        public void DeleteNote(string Username, string NoteOrder)
        {
            List<NOTE> ListOfTrashBin = new List<NOTE>();
            ListOfTrashBin = GetAllTrash(Username);
            List<NOTE> ListOfNote = new List<NOTE>();
            ListOfNote = GetAllNote(Username);
            int NewTrashOrder = 0;   // Biến thứ tự của trash mới dùng để truyền vào SQL 
            string Trashcolor = "";
            string Trashtag = "";
            string Font_Family = "";
            string PictureName = "";
            string DrawPictureName = "";
            string TrashHeader = "";
            string TrashContent = "";
            int Condition_TrashOrder = 0;
            int Size = 8;
            if (ListOfTrashBin.Count == 0 || ListOfTrashBin[0].PpOrder != 1)   // Xử lí xử lí Thứ tự của trash vì Thứ Tự và Username của note là primary key trong SQL - trùng lập sẽ tạo lỗi
            {
                NewTrashOrder = 1;   // Trường hợp chưa có note nào 
            }
            else
            {
                for (int i = 0; i < ListOfTrashBin.Count - 1; i++)   // Trường hợp có note nhưng có note bị xóa làm thứ tự giữa các note có khoảng cách, ví dụ: đang note số 2 nhảy lên note số 4 thì không dc, phải thêm note số 3 vào giữa 2 và 4 có sẵn 
                {
                    if (ListOfTrashBin[i + 1].PpOrder - ListOfTrashBin[i].PpOrder > 1)
                    {
                        Condition_TrashOrder = 1;
                        NewTrashOrder = ListOfTrashBin[i].PpOrder + 1;
                    }
                }
                if (Condition_TrashOrder == 0) // trường hợp này duyệt hết nhưng thứ tự các note liền nhau không bị đứt nên thêm vào sau
                {
                    NewTrashOrder = ListOfTrashBin.Count + 1;
                }
            }
            for (int i = 0; i < ListOfNote.Count; i++) // Duyệt qua list
            {
                if (ListOfNote[i].PpOrder.ToString() == NoteOrder) // Nếu trong list có note nào mà chủ để của note đó giống với textbox 8 thì lưu lại thứ tự của note đó  để truy vấn linq phía dưới 
                {
                    TrashHeader = ListOfNote[i].PpHeader;
                    TrashContent = ListOfNote[i].PpContent;
                    Trashtag = ListOfNote[i].PpTag;
                    Trashcolor = ListOfNote[i].PpFontColor;
                    Size = Int32.Parse(ListOfNote[i].PpFontSize.ToString());
                    Font_Family = ListOfNote[i].PpFontFamily;
                    PictureName = ListOfNote[i].PpPictureName;
                    DrawPictureName = ListOfNote[i].PpDrawPictureName;
                }
            }
            int NoteOrder1 = Int32.Parse(NoteOrder);
            using (WindowProgrammingSQLEntities db = new WindowProgrammingSQLEntities())
            {
                db.List_Of_TrashBin.Add(new List_Of_TrashBin() { Username = Username, TrashOrder = NewTrashOrder, TrashHeader = TrashHeader, TrashContent = TrashContent, TrashTag = Trashtag, FontFamily = Font_Family, FontSize = Size, TrashColor = Trashcolor, TrashPictureName = PictureName, TrashDrawPictureName = DrawPictureName }); // Thêm note vào bảng TrashBin
                db.List_Of_Notes.Remove(db.List_Of_Notes.Where(p => p.Username == Username && p.NotesOrder == NoteOrder1).SingleOrDefault()); // Câu Linq truy vấn SQL  // db.List_Of_Notes.Remove() xóa đối tượng dc tìm thấy từ Linq ra khỏi SQL và đối tượng đó phải là duy nhất trong SQL
                if(ExistImportant(Username,NoteOrder) == true)
                {
                    db.List_Of_ImportantNote.Remove(db.List_Of_ImportantNote.Where(p => p.Username == Username && p.NoteOrder == NoteOrder1).SingleOrDefault());
                }
                db.SaveChanges();   // Save Database lại 
                ListOfNote.Clear(); // Xóa List Of Note vì lúc này trên database đã mất 1 đối tượng, không xóa sẽ tạo lỗi và flowlayoutpanel sẽ không được khởi tạo lại mà là lên gấp đôi đối tượng
                ListOfTrashBin.Clear();
            }
        }

        public string ModifyNote(string Username, string NoteOrder, string NoteHeader, string NoteContent, string Tag, string Font, string Color, int Size, string PicturePath, ref string PicName)
        {
            List<NOTE> ListOfNote = new List<NOTE>();
            ListOfNote = GetAllNote(Username);
            string PicName0 = "";
            {
                for (int i = 0; i < ListOfNote.Count; i++)
                {
                    if (ListOfNote[i].PpOrder.ToString() == NoteOrder)
                    {
                        int NoteOrder1 = Int32.Parse(NoteOrder);
                        PicName = ListOfNote[i].PpDrawPictureName;
                        PicName0 = Username + NoteOrder;
                        int ID = 0;
                        while (System.IO.File.Exists(PicName0 + ID + ".jpg"))
                        {
                            ID++;
                        }
                        PicName0 = PicName0 + ID + ".jpg";
                        WindowProgrammingSQLEntities db = new WindowProgrammingSQLEntities();
                        /* Bước này dùng để xóa một note có sẵn đi nhưng lưu lại thông tin và vị trí của nó sau đó tạo ra một note mới và thêm vào đúng cái vị trí ấy*/
                        db.List_Of_Notes.Remove(db.List_Of_Notes.Where(p => p.Username == Username && p.NotesOrder == NoteOrder1).SingleOrDefault()); // Xóa cái note đã tồn tại đi
                        db.List_Of_Notes.Add(new List_Of_Notes() { Username = Username, NotesOrder = NoteOrder1, NotesHeader = NoteHeader, NotesContent = NoteContent, NoteTag = Tag, FontFamily = Font, FontSize = Size, NoteColor = Color, NotePictureName = PicturePath, NoteDrawPictureName = PicName0 }); // Thêm note mới vào
                        db.SaveChanges();
                    }
                }
            }
            return PicName0;
        }
        public bool AddImportantNote(string Username, string NoteOrder)
        {
            List<NOTE1> Important = new List<NOTE1>();
            Important = GetImportantNotes(Username);
            List<NOTE> ListOfNote = new List<NOTE>();
            ListOfNote = GetAllNote(Username);
            int NewNoteOrder = 0;   // Biến thứ tự của note mới dùng để truyền vào SQL 
            int condition_NoteOrder = 0;    // Biến điều kiện để xử lí Thứ tự của Note 
            if (Important.Count == 0)   // Xử lí xử lí Thứ tự của Note vì Thứ Tự và Username của note là primary key trong SQL - trùng lập sẽ tạo lỗi
            {
                NewNoteOrder = 1;   // Trường hợp chưa có note nào 
            }
            else
            {
                for (int i = 0; i < Important.Count - 1; i++)   // Trường hợp có note nhưng có note bị xóa làm thứ tự giữa các note có khoảng cách, ví dụ: đang note số 2 nhảy lên note số 4 thì không dc, phải thêm note số 3 vào giữa 2 và 4 có sẵn 
                {
                    if (Important[i + 1].PpOrder - Important[i].PpOrder > 1)
                    {
                        condition_NoteOrder = 1;
                        NewNoteOrder = Important[i].PpOrder + 1;
                    }
                }
                if (condition_NoteOrder == 0) // trường hợp này duyệt hết nhưng thứ tự các note liền nhau không bị đứt nên thêm vào sau
                {
                    NewNoteOrder = Important.Count + 1;
                }
            }
            if (NewNoteOrder == 6)
            {
                return false;
            }
            string Header = "";
            string Content = "";
            string Tag = "";
            string Color = "";
            int Size = 0;
            string Font_Family = "";
            string PictureName = "";
            string DrawPictureName = "";
            for (int i = 0; i < ListOfNote.Count; i++) // Duyệt qua list
            {
                if (ListOfNote[i].PpOrder.ToString() == NoteOrder) // Nếu trong list có note nào mà chủ để của note đó giống với textbox 8 thì lưu lại thứ tự của note đó  để truy vấn linq phía dưới 
                {
                    Header = ListOfNote[i].PpHeader;
                    Content = ListOfNote[i].PpContent;
                    Tag = ListOfNote[i].PpTag;
                    Color = ListOfNote[i].PpFontColor;
                    Size = Int32.Parse(ListOfNote[i].PpFontSize.ToString());
                    Font_Family = ListOfNote[i].PpFontFamily;
                    PictureName = ListOfNote[i].PpPictureName;
                    DrawPictureName = ListOfNote[i].PpDrawPictureName;
                }
            }
            int NoteOrder1 = Int32.Parse(NoteOrder);
            using (WindowProgrammingSQLEntities db = new WindowProgrammingSQLEntities())
            {
                db.List_Of_ImportantNote.Add(new List_Of_ImportantNote() { Username = Username, ImportantOrder = NewNoteOrder, ImportantContent = Content, ImportantHeader = Header, ImportantTag = Tag, FontFamily = Font_Family, FontSize = Size, NoteColor = Color, NoteDrawPictureName = PictureName, NotePictureName = DrawPictureName, NoteOrder = NoteOrder1 }); // Tạo note mới trên SQL và thêm vào
                db.SaveChanges(); // Save thay đổi trên SQL
                ListOfNote.Clear(); // Xóa List 
            }
            return true;
        }

        public bool ExistImportant(string Username, string NoteOrder)
        {
            WindowProgrammingSQLEntities db = new WindowProgrammingSQLEntities();
            int NoteOrder1 = Int32.Parse(NoteOrder);
            var Result = from c in db.List_Of_ImportantNote
                         where (c.Username == Username && c.NoteOrder == NoteOrder1)
                         select c;
            if (Result.Count() == 1)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public void DeleteImportant(string Username, string NoteOrder)
        {
            int NoteOrder1 = Int32.Parse(NoteOrder);
            using (WindowProgrammingSQLEntities db = new WindowProgrammingSQLEntities())
            {
                if (ExistImportant(Username, NoteOrder) == true)
                {
                    db.List_Of_ImportantNote.Remove(db.List_Of_ImportantNote.Where(p => p.Username == Username && p.NoteOrder == NoteOrder1).SingleOrDefault());
                    db.SaveChanges();
                }
            }
        }

        public class NOTE
        {
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
    }
}
