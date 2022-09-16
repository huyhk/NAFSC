using System.Data.Entity;

using EntityFramework.DynamicFilters;
using System.Data.Entity.ModelConfiguration.Conventions;

namespace FMS.Data
{
    public class DataContext : DbContext
    {

        private static DataContext _db;

        

        public DataContext() : this("FMSConnection")
        { }
        public DataContext(string cnName) : base(cnName)
        {
            //Database.SetInitializer(new CreateDatabaseIfNotExists<DataContext>());
            Database.SetInitializer<DataContext>(null);
        }

        public static DataContext GetInstance()
        {
            if (_db == null)
                _db = new DataContext();
            return _db;
        }
        public DbSet<ChangeLog> ChangeLogs { get; set; }
        public DbSet<Airport> Airports { get; set; }
        public DbSet<Aircraft> Aircrafts { get; set; }

        public DbSet<Airline> Airlines { get; set; }

        public DbSet<Refuel> Refuels { get; set; }

        public DbSet<RefuelItem> RefuelItems { get; set; }

        public DbSet<Truck> Trucks { get; set; }

        public DbSet<Invoice> Invoices { get; set; }
        public DbSet<InvoiceItem> InvoiceItems { get; set; }

        //public DbSet<TruckAssign> TruckAssigns { get; set; }

        public DbSet<Device> Devices { get; set; }

        public DbSet<Tablet> Tablets { get; set; }

        public DbSet<User> Users { get; set; }
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            //modelBuilder.Entity<Refuel>()    
            //      .HasMany(s => s.Items)
            //      .WithOptional()
            //      .WillCascadeOnDelete(true);


            // modelBuilder.Entity<Flight>()
            //.HasOptional<Airline>(s => s.Airline)
            //.WithMany(a => a.Flights)
            //.WillCascadeOnDelete(false);

            //    modelBuilder.Entity<Flight>()
            //  .HasRequired<Airport>(s => s.Departure)
            //  .WithMany()
            //  .WillCascadeOnDelete(false);

            //    modelBuilder.Entity<Flight>()
            //  .HasRequired<Airport>(s => s.Arrival)
            //  .WithMany()
            //  .WillCascadeOnDelete(false);

            //  modelBuilder.Entity<Flight>()
            //.HasOptional<Refuel>(s => s.Refuel)
            //.WithOptionalPrincipal();

            //    modelBuilder.Entity<Flight>()
            //.HasOptional<ParkingLot>(s => s.ParkingLot)
            //.WithOptionalPrincipal();

            modelBuilder.Filter("IsNotDeleted", (IBaseEntity entity) => entity.IsDeleted, false);
            //modelBuilder.DisableFilterGlobally("IsNotDeleted");

            //modelBuilder.Filter("Branch", (Airport entity) => entity.Branch, BRANCHES.MB);
            //modelBuilder.DisableFilterGlobally("Branch");

            modelBuilder.Entity<Route>()
          .HasRequired<Airport>(s => s.Departure)
          .WithMany()
          .WillCascadeOnDelete(false);

            modelBuilder.Entity<Route>()
         .HasRequired<Airport>(s => s.Arrival)
         .WithMany()
         .WillCascadeOnDelete(false);

            base.OnModelCreating(modelBuilder);

            //modelBuilder.Entity<Shift>()
            //    .HasRequired<Airport>(s => s.Airport)
            //    .WithMany()
            //    .WillCascadeOnDelete(false);

            //modelBuilder.Entity<Truck>()
            //   .HasRequired<Airport>(s => s.AirportObject)
            //   .WithMany()
            //   .WillCascadeOnDelete(false);

            //modelBuilder.Entity<TruckAssign>()
            //    .HasRequired<Shift>(s => s.Shift)
            //    .WithOptional()
            //    .WillCascadeOnDelete(false);

            modelBuilder.Entity<Truck>()
                 .HasOptional<Airport>(s => s.CurrentAirport)
                 .WithMany()
                 .WillCascadeOnDelete(false);

            modelBuilder.Entity<TruckAssign>()
                .HasRequired(t => t.Shift)
                .WithMany()
                .HasForeignKey(t => t.ShiftId)
                .WillCascadeOnDelete(false);
            modelBuilder.Entity<TruckAssign>()
                .HasRequired(t => t.Truck)
                .WithMany()
                .HasForeignKey(t => t.TruckId)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<TruckAssign>()
                .HasRequired(t => t.Driver)
                .WithMany()
                .HasForeignKey(t => t.DriverId)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<RefuelItem>().Property(o => o.Density).HasPrecision(18, 4);

            modelBuilder.Entity<User>()
                .HasMany<Airport>(s => s.Airports)
                .WithMany(c => c.Users)
                .Map(cs =>
                {
                    cs.MapLeftKey("UserId");
                    cs.MapRightKey("AirportId");
                    cs.ToTable("UserAirport");
                });

            modelBuilder.Entity<Invoice>().Property(e => e.Density).HasPrecision(18, 4);

            modelBuilder.Entity<InvoiceItem>().Property(e => e.Density).HasPrecision(18, 4);

            modelBuilder.Entity<Invoice>()
                    .HasOptional(c => c.ChildInvoice)
                    .WithMany()
                    .HasForeignKey(c => c.ChildId);

            

            modelBuilder.Entity<Invoice>()
                  .HasOptional(c => c.ParentInvoice)
                  .WithMany()
                  .HasForeignKey(c => c.ParentId);

        }

        public System.Data.Entity.DbSet<FMS.Data.Flight> Flights { get; set; }

        public System.Data.Entity.DbSet<FMS.Data.ParkingLot> ParkingLots { get; set; }

        public System.Data.Entity.DbSet<FMS.Data.Product> Products { get; set; }

        public System.Data.Entity.DbSet<FMS.Data.Shift> Shifts { get; set; }
        public System.Data.Entity.DbSet<FMS.Data.ProductPrice> ProductPrices { get; set; }

        public System.Data.Entity.DbSet<FMS.Data.TruckAssign> TruckAssigns { get; set; }

        public DbSet<QCNoHistory> QCNoHistory { get; set; }

    }
}
