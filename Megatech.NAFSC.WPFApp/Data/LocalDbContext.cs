using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Megatech.NAFSC.WPFApp.Data
{
    public class LocalDbContext : DbContext
    {
        private static string ConnectionString
        {
            get
            {
                string cnString = @"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=" + appPath + @"\Nafsc.mdf;Integrated Security=True";
                
                return cnString;
            }
        }
        public LocalDbContext() :this(LocalDbContext.ConnectionString)
        {
            
            
        }
        public LocalDbContext(string cnStringOrName) : base(cnStringOrName)
        {
            Database.SetInitializer<LocalDbContext>(null);
        }


        private static LocalDbContext _instance;
        public DbSet<LocalRefuel> Refuels { get; set; }

        public DbSet<LocalUser> Users { get; set; }

        public DbSet<LocalInvoice> Invoices { get; set; }

        public DbSet<LocalAirline> Airlines { get; set; }

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
