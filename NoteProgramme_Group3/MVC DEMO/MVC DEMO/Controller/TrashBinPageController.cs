using MVC_DEMO.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MVC_DEMO.Controller
{
    class TrashBinPageController
    {
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

        public void DeleteNote(string Username, string NoteOrder)
        {
            List<NOTE> ListOfTrashBin = new List<NOTE>();
            ListOfTrashBin = GetAllTrash(Username);
            int NoteOrder1 = Int32.Parse(NoteOrder);
            using (WindowProgrammingSQLEntities db = new WindowProgrammingSQLEntities())
            {
                db.List_Of_TrashBin.Remove(db.List_Of_TrashBin.Where(p => p.Username == Username && p.TrashOrder == NoteOrder1).SingleOrDefault()); // Câu Linq truy vấn SQL  // db.List_Of_Notes.Remove() xóa đối tượng dc tìm thấy từ Linq ra khỏi SQL và đối tượng đó phải là duy nhất trong SQL
                db.SaveChanges();   // Save Database lại             
                ListOfTrashBin.Clear();
            }
        }

        public string Restore(string Username, string NoteHeader,int NoteOrder1, int NewNoteOrder, string NoteContent, string NoteTag, string FontFamily, string Color, int Size, string PicturePath, string DrawPicName)
        {
            List<NOTE> ListOfNote = new List<NOTE>();
            ListOfNote = GetAllNote(Username);
            List<NOTE> ListOfTrashBin = new List<NOTE>();
            ListOfTrashBin = GetAllTrash(Username);
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
            using (WindowProgrammingSQLEntities db = new WindowProgrammingSQLEntities())
            {
                db.List_Of_Notes.Add(new List_Of_Notes() { Username = Username, NotesOrder = NewNoteOrder, NotesContent = NoteContent, NotesHeader = NoteHeader, NoteTag = NoteTag, FontFamily = FontFamily, FontSize = Size, NoteColor = Color, NoteDrawPictureName = DrawPicName, NotePictureName = PicturePath }); // Tạo note mới trên SQL và thêm vào
                db.List_Of_TrashBin.Remove(db.List_Of_TrashBin.Where(p => p.Username == Username && p.TrashOrder == NoteOrder1).SingleOrDefault());
                db.SaveChanges(); // Save thay đổi trên SQL
            }
            return DrawPicName;
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
}
