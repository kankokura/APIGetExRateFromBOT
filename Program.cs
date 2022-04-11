using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DbManager;

namespace EXRate_API_BOT
{
    class Program
    {
        static void Main(string[] args)
        {
            OracleDB.AddDbSetting("MC", new DbSetting { SID = "DCIOS02", Schema = "MC", Password = "MC" });

            var obj = new Class.GetCurrency();
            obj.GetAPI();
        }
    }
}
