using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace RegBot2
{
    internal class StartProcedures
    {
        public static DataBase StartDataBaseConnection()
        {
            DataBase dataBase = new DataBase();
            dataBase.Open();
            dataBase.Chek();
            return dataBase;
        }         
    }
}
