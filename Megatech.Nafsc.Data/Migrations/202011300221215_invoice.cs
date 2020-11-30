namespace Megatech.Nafsc.Data.Migrations
{
    using System;
    using System.Collections.Generic;
    using System.Data.Entity.Infrastructure.Annotations;
    using System.Data.Entity.Migrations;
    
    public partial class invoice : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.TruckAssigns", "DriverId", "dbo.Users");
            DropForeignKey("dbo.TruckAssigns", "ShiftId", "dbo.Shifts");
            DropForeignKey("dbo.TruckAssigns", "TruckId", "dbo.Trucks");
            CreateTable(
                "dbo.Invoices",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        FlightId = c.Int(nullable: false),
                        ParentId = c.Int(),
                        ChildId = c.Int(),
                        InvoiceNumber = c.String(),
                        CustomerId = c.Int(nullable: false),
                        CustomerName = c.String(),
                        TaxCode = c.String(),
                        Address = c.String(),
                        Volume = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Weight = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Gallon = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Temperature = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Density = c.Decimal(nullable: false, precision: 18, scale: 2),
                        ProductName = c.String(),
                        Price = c.Decimal(nullable: false, precision: 18, scale: 2),
                        TaxRate = c.Decimal(nullable: false, precision: 18, scale: 2),
                        SubTotal = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Tax = c.Decimal(nullable: false, precision: 18, scale: 2),
                        DateCreated = c.DateTime(nullable: false),
                        DateUpdated = c.DateTime(nullable: false),
                        UserCreatedId = c.Int(),
                        UserUpdatedId = c.Int(),
                        IsDeleted = c.Boolean(nullable: false),
                        DateDeleted = c.DateTime(),
                        UserDeletedId = c.Int(),
                        
                    },
                annotations: new Dictionary<string, object>
                {
                    { "DynamicFilter_Invoice_IsNotDeleted", "EntityFramework.DynamicFilters.DynamicFilterDefinition" },
                })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Invoices", t => t.ChildId)
                .ForeignKey("dbo.Invoices", t => t.ParentId)
                .Index(t => t.ParentId)
                .Index(t => t.ChildId);
            
            CreateTable(
                "dbo.InvoiceItems",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        InvoiceId = c.Int(nullable: false),
                        RealAmount = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Volume = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Weight = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Gallon = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Density = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Temperature = c.Decimal(nullable: false, precision: 18, scale: 2),
                        TruckNo = c.String(),
                        StartNumber = c.Decimal(nullable: false, precision: 18, scale: 2),
                        EndNumber = c.Decimal(nullable: false, precision: 18, scale: 2),
                        DateCreated = c.DateTime(nullable: false),
                        DateUpdated = c.DateTime(nullable: false),
                        UserCreatedId = c.Int(),
                        UserUpdatedId = c.Int(),
                        IsDeleted = c.Boolean(nullable: false),
                        DateDeleted = c.DateTime(),
                        UserDeletedId = c.Int(),
                    },
                annotations: new Dictionary<string, object>
                {
                    { "DynamicFilter_InvoiceItem_IsNotDeleted", "EntityFramework.DynamicFilters.DynamicFilterDefinition" },
                })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Invoices", t => t.InvoiceId, cascadeDelete: true)
                .Index(t => t.InvoiceId);
            
            CreateTable(
                "dbo.Agencies",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Code = c.String(),
                        Name = c.String(),
                        TaxCode = c.String(),
                        Address = c.String(),
                        InvoiceName = c.String(),
                        InvoiceTaxCode = c.String(),
                        InvoiceAddress = c.String(),
                        DateCreated = c.DateTime(nullable: false),
                        DateUpdated = c.DateTime(nullable: false),
                        UserCreatedId = c.Int(),
                        UserUpdatedId = c.Int(),
                        IsDeleted = c.Boolean(nullable: false),
                        DateDeleted = c.DateTime(),
                        UserDeletedId = c.Int(),
                    },
                annotations: new Dictionary<string, object>
                {
                    { "DynamicFilter_Agency_IsNotDeleted", "EntityFramework.DynamicFilters.DynamicFilterDefinition" },
                })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.QCNoHistories",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        StartDate = c.DateTime(nullable: false),
                        QCNo = c.String(),
                        ProductId = c.Int(),
                        AirtportId = c.Int(),
                        FileUrl = c.String(),
                        DateCreated = c.DateTime(nullable: false),
                        DateUpdated = c.DateTime(nullable: false),
                        UserCreatedId = c.Int(),
                        UserUpdatedId = c.Int(),
                        IsDeleted = c.Boolean(nullable: false),
                        DateDeleted = c.DateTime(),
                        UserDeletedId = c.Int(),
                    },
                annotations: new Dictionary<string, object>
                {
                    { "DynamicFilter_QCNoHistory_IsNotDeleted", "EntityFramework.DynamicFilters.DynamicFilterDefinition" },
                })
                .PrimaryKey(t => t.Id);
            
            AddColumn("dbo.Airlines", "FlightCodePattern", c => c.Int(nullable: false));
            AddColumn("dbo.Flights", "ValvePit", c => c.Int(nullable: false));
            AddColumn("dbo.Flights", "OilCompany", c => c.Int(nullable: false));
            AddColumn("dbo.RefuelItems", "Volume", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("dbo.RefuelItems", "Volume15", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("dbo.RefuelItems", "Weight", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("dbo.RefuelItems", "Extract", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("dbo.ProductPrices", "AgencyId", c => c.Int(nullable: false));
            AddColumn("dbo.ProductPrices", "AirportId", c => c.Int());
            CreateIndex("dbo.ProductPrices", "AgencyId");
            AddForeignKey("dbo.ProductPrices", "AgencyId", "dbo.Agencies", "Id", cascadeDelete: true);
            AddForeignKey("dbo.TruckAssigns", "DriverId", "dbo.Users", "Id");
            AddForeignKey("dbo.TruckAssigns", "ShiftId", "dbo.Shifts", "Id");
            AddForeignKey("dbo.TruckAssigns", "TruckId", "dbo.Trucks", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.TruckAssigns", "TruckId", "dbo.Trucks");
            DropForeignKey("dbo.TruckAssigns", "ShiftId", "dbo.Shifts");
            DropForeignKey("dbo.TruckAssigns", "DriverId", "dbo.Users");
            DropForeignKey("dbo.ProductPrices", "AgencyId", "dbo.Agencies");
            DropForeignKey("dbo.Invoices", "ParentId", "dbo.Invoices");
            DropForeignKey("dbo.InvoiceItems", "InvoiceId", "dbo.Invoices");
            DropForeignKey("dbo.Invoices", "ChildInvoice_Id", "dbo.Invoices");
            DropIndex("dbo.ProductPrices", new[] { "AgencyId" });
            DropIndex("dbo.InvoiceItems", new[] { "InvoiceId" });
            DropIndex("dbo.Invoices", new[] { "ChildInvoice_Id" });
            DropIndex("dbo.Invoices", new[] { "ParentId" });
            DropColumn("dbo.ProductPrices", "AirportId");
            DropColumn("dbo.ProductPrices", "AgencyId");
            DropColumn("dbo.RefuelItems", "Extract");
            DropColumn("dbo.RefuelItems", "Weight");
            DropColumn("dbo.RefuelItems", "Volume15");
            DropColumn("dbo.RefuelItems", "Volume");
            DropColumn("dbo.Flights", "OilCompany");
            DropColumn("dbo.Flights", "ValvePit");
            DropColumn("dbo.Airlines", "FlightCodePattern");
            DropTable("dbo.QCNoHistories",
                removedAnnotations: new Dictionary<string, object>
                {
                    { "DynamicFilter_QCNoHistory_IsNotDeleted", "EntityFramework.DynamicFilters.DynamicFilterDefinition" },
                });
            DropTable("dbo.Agencies",
                removedAnnotations: new Dictionary<string, object>
                {
                    { "DynamicFilter_Agency_IsNotDeleted", "EntityFramework.DynamicFilters.DynamicFilterDefinition" },
                });
            DropTable("dbo.InvoiceItems",
                removedAnnotations: new Dictionary<string, object>
                {
                    { "DynamicFilter_InvoiceItem_IsNotDeleted", "EntityFramework.DynamicFilters.DynamicFilterDefinition" },
                });
            DropTable("dbo.Invoices",
                removedAnnotations: new Dictionary<string, object>
                {
                    { "DynamicFilter_Invoice_IsNotDeleted", "EntityFramework.DynamicFilters.DynamicFilterDefinition" },
                });
            AddForeignKey("dbo.TruckAssigns", "TruckId", "dbo.Trucks", "Id", cascadeDelete: true);
            AddForeignKey("dbo.TruckAssigns", "ShiftId", "dbo.Shifts", "Id", cascadeDelete: true);
            AddForeignKey("dbo.TruckAssigns", "DriverId", "dbo.Users", "Id", cascadeDelete: true);
        }
    }
}
