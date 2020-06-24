using MVC_DEMO.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MVC_DEMO.Controller
{
    public class USER // Lớp User
    {
        public string Name;
        public string Pass;
        public string PpName { get; set; }
        public string PpPass { get; set; }
    }
    class LogInController
    {
        public bool LogIn(string Username, string Password)  
        {
            WindowProgrammingSQLEntities db = new WindowProgrammingSQLEntities();   
            var Result = from c in db.Users
                         where (c.Username == Username && c.Pass_word == Password)
                         select new { Name = c.Username, Pass = c.Pass_word}; 
            if(Result.Count() == 1)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public bool Regist(string Username, string Password)
        {
            string NewUserName = Username;
            string NewUserPass = Password;
            WindowProgrammingSQLEntities db = new WindowProgrammingSQLEntities();
            var Result = from c in db.Users
                         where (c.Username == NewUserName)
                         select new { Name = c.Username, Pass = c.Pass_word};
            if (Result.Count() == 1)
            {
                return false;
            }
            else
            {            
                db.Users.Add(new User { Username = NewUserName, Pass_word = NewUserPass});
                db.SaveChanges();                 
            }
            return true;
        }
    }
}
