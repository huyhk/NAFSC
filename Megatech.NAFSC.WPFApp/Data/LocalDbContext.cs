using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Megatech.NAFSC.WPFApp.Data
{
    public class LocalDbContext : DbContext
    {
        public LocalDbContext() : base("FMSConnection")
        { }
        public LocalDbContext(string cnStringOrName) : base(cnStringOrName)
        { }


        private static LocalDbContext _instance;
        public DbSet<LocalRefuel> Refuels { get; set; }

        private static string appPath = Environment.CurrentDirectory;
        internal static LocalDbContext GetInstance()
        {
            if (_instance == null)
            {
                string cnString = @"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=" + appPath + @"\Nafsc.mdf;Integrated Security=True";
                _instance = new LocalDbContext(cnString);
                
            }
            return _instance;
        }
    }
}
