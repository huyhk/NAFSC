namespace Megatech.Nafsc.Data.Migrations
{
    using System;
    using System.Collections.Generic;
    using System.Data.Entity.Infrastructure.Annotations;
    using System.Data.Entity.Migrations;
    
    public partial class qcNo : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Aircraft",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                        Code = c.String(nullable: false),
                        AircraftType = c.String(nullable: false),
                        CustomerId = c.Int(),
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
                    { "DynamicFilter_Aircraft_IsNotDeleted", "EntityFramework.DynamicFilters.DynamicFilterDefinition" },
                })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Airlines", t => t.CustomerId)
                .Index(t => t.CustomerId);
            
            CreateTable(
                "dbo.Airlines",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        FlightCodePattern = c.Int(nullable: false),
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
                    { "DynamicFilter_Airline_IsNotDeleted", "EntityFramework.DynamicFilters.DynamicFilterDefinition" },
                })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Flights",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Code = c.String(nullable: false),
                        DepartureScheduledTime = c.DateTime(),
                        DepartuteTime = c.DateTime(),
                        ArrivalScheduledTime = c.DateTime(),
                        ArrivalTime = c.DateTime(),
                        FlightTime = c.DateTime(),
                        RouteName = c.String(),
                        AirlineId = c.Int(),
                        AircraftId = c.Int(),
                        AircraftCode = c.String(),
                        AircraftType = c.String(),
                        EstimateAmount = c.Decimal(nullable: false, precision: 18, scale: 2),
                        RefuelScheduledTime = c.DateTime(),
                        RefuelScheduledHours = c.DateTime(),
                        RefuelTime = c.DateTime(),
                        ParkingLotId = c.Int(),
                        Parking = c.String(),
                        ValvePit = c.Int(nullable: false),
                        DriverName = c.String(),
                        TechnicalerName = c.String(),
                        Shift = c.String(),
                        ShiftStartTime = c.DateTime(),
                        ShiftEndTime = c.DateTime(),
                        AirportName = c.String(),
                        TruckName = c.String(),
                        Status = c.Int(nullable: false),
                        AirportId = c.Int(),
                        TotalAmount = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Price = c.Decimal(nullable: false, precision: 18, scale: 2),
                        StartTime = c.DateTime(nullable: false),
                        EndTime = c.DateTime(nullable: false),
                        Note = c.String(),
                        CreatedLocation = c.Int(nullable: false),
                        FlightType = c.Int(nullable: false),
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
                    { "DynamicFilter_Flight_IsNotDeleted", "EntityFramework.DynamicFilters.DynamicFilterDefinition" },
                })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Aircraft", t => t.AircraftId)
                .ForeignKey("dbo.Airlines", t => t.AirlineId)
                .ForeignKey("dbo.Airports", t => t.AirportId)
                .ForeignKey("dbo.ParkingLots", t => t.ParkingLotId)
                .Index(t => t.AirlineId)
                .Index(t => t.AircraftId)
                .Index(t => t.ParkingLotId)
                .Index(t => t.AirportId);
            
            CreateTable(
                "dbo.Airports",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Code = c.String(nullable: false),
                        Name = c.String(nullable: false),
                        Address = c.String(),
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
                    { "DynamicFilter_Airport_IsNotDeleted", "EntityFramework.DynamicFilters.DynamicFilterDefinition" },
                })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Users",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        UserName = c.String(),
                        FullName = c.String(nullable: false),
                        Email = c.String(),
                        IsEnabled = c.Boolean(nullable: false),
                        LastLogin = c.DateTime(),
                        AirportId = c.Int(),
                        DisplayName = c.String(),
                        Permission = c.Int(nullable: false),
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
                    { "DynamicFilter_User_IsNotDeleted", "EntityFramework.DynamicFilters.DynamicFilterDefinition" },
                })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Airports", t => t.AirportId)
                .Index(t => t.AirportId);
            
            CreateTable(
                "dbo.ParkingLots",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        AirportId = c.Int(nullable: false),
                        Code = c.String(nullable: false, maxLength: 20),
                        Name = c.String(maxLength: 100),
                        DateCreated = c.DateTime(nullable: false),
                        DateUpdated = c.DateTime(nullable: false),
                        UserCreatedId = c.Int(),
                        UserUpdatedId = c.Int(),
                        IsDeleted = c.Boolean(nullable: false),
                        DateDeleted = c.DateTime(),
                        UserDeletedId = c.Int(),
                        Location_Id = c.Int(),
                    },
                annotations: new Dictionary<string, object>
                {
                    { "DynamicFilter_ParkingLot_IsNotDeleted", "EntityFramework.DynamicFilters.DynamicFilterDefinition" },
                })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Airports", t => t.AirportId, cascadeDelete: true)
                .ForeignKey("dbo.GeoLocations", t => t.Location_Id)
                .Index(t => t.AirportId)
                .Index(t => t.Location_Id);
            
            CreateTable(
                "dbo.GeoLocations",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Longitude = c.Single(nullable: false),
                        Latitude = c.Single(nullable: false),
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
                    { "DynamicFilter_GeoLocation_IsNotDeleted", "EntityFramework.DynamicFilters.DynamicFilterDefinition" },
                })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.RefuelItems",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        TruckId = c.Int(nullable: false),
                        StartTime = c.DateTime(nullable: false),
                        Amount = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Temperature = c.Decimal(nullable: false, precision: 18, scale: 2),
                        StartNumber = c.Decimal(nullable: false, precision: 18, scale: 2),
                        EndNumber = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Completed = c.Boolean(nullable: false),
                        Printed = c.Boolean(nullable: false),
                        EndTime = c.DateTime(),
                        DriverId = c.Int(),
                        OperatorId = c.Int(),
                        DeviceStartTime = c.DateTime(),
                        DeviceEndTime = c.DateTime(),
                        Status = c.Int(nullable: false),
                        ManualTemperature = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Density = c.Decimal(nullable: false, precision: 18, scale: 4),
                        FlightId = c.Int(nullable: false),
                        Price = c.Decimal(nullable: false, precision: 18, scale: 2),
                        QCNo = c.String(),
                        TaxRate = c.Decimal(nullable: false, precision: 18, scale: 2),
                        ApprovalUserId = c.Int(),
                        ApprovalStatus = c.Int(nullable: false),
                        ApprovalNote = c.String(),
                        CreatedLocation = c.Int(nullable: false),
                        RefuelItemType = c.Int(nullable: false),
                        Volume = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Volume15 = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Weight = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Gallon = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Extract = c.Decimal(nullable: false, precision: 18, scale: 2),
                        DateCreated = c.DateTime(nullable: false),
                        DateUpdated = c.DateTime(nullable: false),
                        UserCreatedId = c.Int(),
                        UserUpdatedId = c.Int(),
                        IsDeleted = c.Boolean(nullable: false),
                        DateDeleted = c.DateTime(),
                        UserDeletedId = c.Int(),
                        Refuel_Id = c.Int(),
                    },
                annotations: new Dictionary<string, object>
                {
                    { "DynamicFilter_RefuelItem_IsNotDeleted", "EntityFramework.DynamicFilters.DynamicFilterDefinition" },
                })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Users", t => t.DriverId)
                .ForeignKey("dbo.Flights", t => t.FlightId, cascadeDelete: true)
                .ForeignKey("dbo.Users", t => t.OperatorId)
                .ForeignKey("dbo.Trucks", t => t.TruckId, cascadeDelete: true)
                .ForeignKey("dbo.Refuels", t => t.Refuel_Id)
                .Index(t => t.TruckId)
                .Index(t => t.DriverId)
                .Index(t => t.OperatorId)
                .Index(t => t.FlightId)
                .Index(t => t.Refuel_Id);
            
            CreateTable(
                "dbo.Trucks",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        DeviceId = c.Int(),
                        TabletId = c.String(),
                        Code = c.String(nullable: false),
                        MaxAmount = c.Decimal(nullable: false, precision: 18, scale: 2),
                        CurrentAmount = c.Decimal(nullable: false, precision: 18, scale: 2),
                        AirportId = c.Int(),
                        Unit = c.String(),
                        DeviceIP = c.String(),
                        PrinterIP = c.String(),
                        TabletSerial = c.String(),
                        DeviceSerial = c.String(),
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
                    { "DynamicFilter_Truck_IsNotDeleted", "EntityFramework.DynamicFilters.DynamicFilterDefinition" },
                })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Airports", t => t.AirportId)
                .ForeignKey("dbo.Devices", t => t.DeviceId)
                .Index(t => t.DeviceId)
                .Index(t => t.AirportId);
            
            CreateTable(
                "dbo.Devices",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        SerialNumber = c.String(),
                        Status = c.Int(nullable: false),
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
                    { "DynamicFilter_Device_IsNotDeleted", "EntityFramework.DynamicFilters.DynamicFilterDefinition" },
                })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.ChangeLogs",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        EntityName = c.String(),
                        EntityDisplay = c.String(),
                        PropertyName = c.String(),
                        KeyValues = c.String(),
                        OldValues = c.String(),
                        NewValues = c.String(),
                        DateChanged = c.DateTime(nullable: false),
                        UserUpdatedId = c.Int(),
                        UserUpdatedName = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.ProductPrices",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        CustomerId = c.Int(),
                        ProductId = c.Int(nullable: false),
                        StartDate = c.DateTime(nullable: false),
                        EndDate = c.DateTime(),
                        Price = c.Decimal(nullable: false, precision: 18, scale: 2),
                        OilCompany = c.Int(nullable: false),
                        Unit = c.Int(nullable: false),
                        AgencyId = c.Int(nullable: false),
                        AirportId = c.Int(),
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
                    { "DynamicFilter_ProductPrice_IsNotDeleted", "EntityFramework.DynamicFilters.DynamicFilterDefinition" },
                })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Agencies", t => t.AgencyId, cascadeDelete: true)
                .ForeignKey("dbo.Airlines", t => t.CustomerId)
                .ForeignKey("dbo.Products", t => t.ProductId, cascadeDelete: true)
                .Index(t => t.CustomerId)
                .Index(t => t.ProductId)
                .Index(t => t.AgencyId);
            
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
                "dbo.Products",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Code = c.String(nullable: false),
                        Name = c.String(nullable: false),
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
                    { "DynamicFilter_Product_IsNotDeleted", "EntityFramework.DynamicFilters.DynamicFilterDefinition" },
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
            
            CreateTable(
                "dbo.Refuels",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        FlightId = c.Int(nullable: false),
                        TotalAmount = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Price = c.Decimal(nullable: false, precision: 18, scale: 2),
                        StartTime = c.DateTime(nullable: false),
                        EndTime = c.DateTime(nullable: false),
                        Status = c.Int(nullable: false),
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
                    { "DynamicFilter_Refuel_IsNotDeleted", "EntityFramework.DynamicFilters.DynamicFilterDefinition" },
                })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Flights", t => t.FlightId, cascadeDelete: true)
                .Index(t => t.FlightId);
            
            CreateTable(
                "dbo.Shifts",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        AirportId = c.Int(nullable: false),
                        StartTime = c.DateTime(nullable: false),
                        EndTime = c.DateTime(nullable: false),
                        Code = c.String(),
                        Name = c.String(),
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
                    { "DynamicFilter_Shift_IsNotDeleted", "EntityFramework.DynamicFilters.DynamicFilterDefinition" },
                })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Airports", t => t.AirportId, cascadeDelete: true)
                .Index(t => t.AirportId);
            
            CreateTable(
                "dbo.Tablets",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        SerialNumber = c.String(),
                        CurrentUserId = c.Int(nullable: false),
                        TruckId = c.Int(nullable: false),
                        Status = c.Int(nullable: false),
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
                    { "DynamicFilter_Tablet_IsNotDeleted", "EntityFramework.DynamicFilters.DynamicFilterDefinition" },
                })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.TruckAssigns",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        TruckId = c.Int(nullable: false),
                        DriverId = c.Int(nullable: false),
                        TechnicalerId = c.Int(nullable: false),
                        ShiftId = c.Int(nullable: false),
                        StartDate = c.DateTime(nullable: false),
                        AirportId = c.Int(nullable: false),
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
                    { "DynamicFilter_TruckAssign_IsNotDeleted", "EntityFramework.DynamicFilters.DynamicFilterDefinition" },
                })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Airports", t => t.AirportId, cascadeDelete: true)
                .ForeignKey("dbo.Users", t => t.DriverId)
                .ForeignKey("dbo.Shifts", t => t.ShiftId)
                .ForeignKey("dbo.Users", t => t.TechnicalerId, cascadeDelete: true)
                .ForeignKey("dbo.Trucks", t => t.TruckId)
                .Index(t => t.TruckId)
                .Index(t => t.DriverId)
                .Index(t => t.TechnicalerId)
                .Index(t => t.ShiftId)
                .Index(t => t.AirportId);
            
            CreateTable(
                "dbo.Routes",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        DepartureId = c.Int(nullable: false),
                        ArrivalId = c.Int(nullable: false),
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
                    { "DynamicFilter_Route_IsNotDeleted", "EntityFramework.DynamicFilters.DynamicFilterDefinition" },
                })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Airports", t => t.ArrivalId)
                .ForeignKey("dbo.Airports", t => t.DepartureId)
                .Index(t => t.DepartureId)
                .Index(t => t.ArrivalId);
            
            CreateTable(
                "dbo.UserAirport",
                c => new
                    {
                        UserId = c.Int(nullable: false),
                        AirportId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.UserId, t.AirportId })
                .ForeignKey("dbo.Users", t => t.UserId, cascadeDelete: true)
                .ForeignKey("dbo.Airports", t => t.AirportId, cascadeDelete: true)
                .Index(t => t.UserId)
                .Index(t => t.AirportId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Routes", "DepartureId", "dbo.Airports");
            DropForeignKey("dbo.Routes", "ArrivalId", "dbo.Airports");
            DropForeignKey("dbo.TruckAssigns", "TruckId", "dbo.Trucks");
            DropForeignKey("dbo.TruckAssigns", "TechnicalerId", "dbo.Users");
            DropForeignKey("dbo.TruckAssigns", "ShiftId", "dbo.Shifts");
            DropForeignKey("dbo.TruckAssigns", "DriverId", "dbo.Users");
            DropForeignKey("dbo.TruckAssigns", "AirportId", "dbo.Airports");
            DropForeignKey("dbo.Shifts", "AirportId", "dbo.Airports");
            DropForeignKey("dbo.RefuelItems", "Refuel_Id", "dbo.Refuels");
            DropForeignKey("dbo.Refuels", "FlightId", "dbo.Flights");
            DropForeignKey("dbo.ProductPrices", "ProductId", "dbo.Products");
            DropForeignKey("dbo.ProductPrices", "CustomerId", "dbo.Airlines");
            DropForeignKey("dbo.ProductPrices", "AgencyId", "dbo.Agencies");
            DropForeignKey("dbo.RefuelItems", "TruckId", "dbo.Trucks");
            DropForeignKey("dbo.Trucks", "DeviceId", "dbo.Devices");
            DropForeignKey("dbo.Trucks", "AirportId", "dbo.Airports");
            DropForeignKey("dbo.RefuelItems", "OperatorId", "dbo.Users");
            DropForeignKey("dbo.RefuelItems", "FlightId", "dbo.Flights");
            DropForeignKey("dbo.RefuelItems", "DriverId", "dbo.Users");
            DropForeignKey("dbo.ParkingLots", "Location_Id", "dbo.GeoLocations");
            DropForeignKey("dbo.Flights", "ParkingLotId", "dbo.ParkingLots");
            DropForeignKey("dbo.ParkingLots", "AirportId", "dbo.Airports");
            DropForeignKey("dbo.Flights", "AirportId", "dbo.Airports");
            DropForeignKey("dbo.UserAirport", "AirportId", "dbo.Airports");
            DropForeignKey("dbo.UserAirport", "UserId", "dbo.Users");
            DropForeignKey("dbo.Users", "AirportId", "dbo.Airports");
            DropForeignKey("dbo.Flights", "AirlineId", "dbo.Airlines");
            DropForeignKey("dbo.Flights", "AircraftId", "dbo.Aircraft");
            DropForeignKey("dbo.Aircraft", "CustomerId", "dbo.Airlines");
            DropIndex("dbo.UserAirport", new[] { "AirportId" });
            DropIndex("dbo.UserAirport", new[] { "UserId" });
            DropIndex("dbo.Routes", new[] { "ArrivalId" });
            DropIndex("dbo.Routes", new[] { "DepartureId" });
            DropIndex("dbo.TruckAssigns", new[] { "AirportId" });
            DropIndex("dbo.TruckAssigns", new[] { "ShiftId" });
            DropIndex("dbo.TruckAssigns", new[] { "TechnicalerId" });
            DropIndex("dbo.TruckAssigns", new[] { "DriverId" });
            DropIndex("dbo.TruckAssigns", new[] { "TruckId" });
            DropIndex("dbo.Shifts", new[] { "AirportId" });
            DropIndex("dbo.Refuels", new[] { "FlightId" });
            DropIndex("dbo.ProductPrices", new[] { "AgencyId" });
            DropIndex("dbo.ProductPrices", new[] { "ProductId" });
            DropIndex("dbo.ProductPrices", new[] { "CustomerId" });
            DropIndex("dbo.Trucks", new[] { "AirportId" });
            DropIndex("dbo.Trucks", new[] { "DeviceId" });
            DropIndex("dbo.RefuelItems", new[] { "Refuel_Id" });
            DropIndex("dbo.RefuelItems", new[] { "FlightId" });
            DropIndex("dbo.RefuelItems", new[] { "OperatorId" });
            DropIndex("dbo.RefuelItems", new[] { "DriverId" });
            DropIndex("dbo.RefuelItems", new[] { "TruckId" });
            DropIndex("dbo.ParkingLots", new[] { "Location_Id" });
            DropIndex("dbo.ParkingLots", new[] { "AirportId" });
            DropIndex("dbo.Users", new[] { "AirportId" });
            DropIndex("dbo.Flights", new[] { "AirportId" });
            DropIndex("dbo.Flights", new[] { "ParkingLotId" });
            DropIndex("dbo.Flights", new[] { "AircraftId" });
            DropIndex("dbo.Flights", new[] { "AirlineId" });
            DropIndex("dbo.Aircraft", new[] { "CustomerId" });
            DropTable("dbo.UserAirport");
            DropTable("dbo.Routes",
                removedAnnotations: new Dictionary<string, object>
                {
                    { "DynamicFilter_Route_IsNotDeleted", "EntityFramework.DynamicFilters.DynamicFilterDefinition" },
                });
            DropTable("dbo.TruckAssigns",
                removedAnnotations: new Dictionary<string, object>
                {
                    { "DynamicFilter_TruckAssign_IsNotDeleted", "EntityFramework.DynamicFilters.DynamicFilterDefinition" },
                });
            DropTable("dbo.Tablets",
                removedAnnotations: new Dictionary<string, object>
                {
                    { "DynamicFilter_Tablet_IsNotDeleted", "EntityFramework.DynamicFilters.DynamicFilterDefinition" },
                });
            DropTable("dbo.Shifts",
                removedAnnotations: new Dictionary<string, object>
                {
                    { "DynamicFilter_Shift_IsNotDeleted", "EntityFramework.DynamicFilters.DynamicFilterDefinition" },
                });
            DropTable("dbo.Refuels",
                removedAnnotations: new Dictionary<string, object>
                {
                    { "DynamicFilter_Refuel_IsNotDeleted", "EntityFramework.DynamicFilters.DynamicFilterDefinition" },
                });
            DropTable("dbo.QCNoHistories",
                removedAnnotations: new Dictionary<string, object>
                {
                    { "DynamicFilter_QCNoHistory_IsNotDeleted", "EntityFramework.DynamicFilters.DynamicFilterDefinition" },
                });
            DropTable("dbo.Products",
                removedAnnotations: new Dictionary<string, object>
                {
                    { "DynamicFilter_Product_IsNotDeleted", "EntityFramework.DynamicFilters.DynamicFilterDefinition" },
                });
            DropTable("dbo.Agencies",
                removedAnnotations: new Dictionary<string, object>
                {
                    { "DynamicFilter_Agency_IsNotDeleted", "EntityFramework.DynamicFilters.DynamicFilterDefinition" },
                });
            DropTable("dbo.ProductPrices",
                removedAnnotations: new Dictionary<string, object>
                {
                    { "DynamicFilter_ProductPrice_IsNotDeleted", "EntityFramework.DynamicFilters.DynamicFilterDefinition" },
                });
            DropTable("dbo.ChangeLogs");
            DropTable("dbo.Devices",
                removedAnnotations: new Dictionary<string, object>
                {
                    { "DynamicFilter_Device_IsNotDeleted", "EntityFramework.DynamicFilters.DynamicFilterDefinition" },
                });
            DropTable("dbo.Trucks",
                removedAnnotations: new Dictionary<string, object>
                {
                    { "DynamicFilter_Truck_IsNotDeleted", "EntityFramework.DynamicFilters.DynamicFilterDefinition" },
                });
            DropTable("dbo.RefuelItems",
                removedAnnotations: new Dictionary<string, object>
                {
                    { "DynamicFilter_RefuelItem_IsNotDeleted", "EntityFramework.DynamicFilters.DynamicFilterDefinition" },
                });
            DropTable("dbo.GeoLocations",
                removedAnnotations: new Dictionary<string, object>
                {
                    { "DynamicFilter_GeoLocation_IsNotDeleted", "EntityFramework.DynamicFilters.DynamicFilterDefinition" },
                });
            DropTable("dbo.ParkingLots",
                removedAnnotations: new Dictionary<string, object>
                {
                    { "DynamicFilter_ParkingLot_IsNotDeleted", "EntityFramework.DynamicFilters.DynamicFilterDefinition" },
                });
            DropTable("dbo.Users",
                removedAnnotations: new Dictionary<string, object>
                {
                    { "DynamicFilter_User_IsNotDeleted", "EntityFramework.DynamicFilters.DynamicFilterDefinition" },
                });
            DropTable("dbo.Airports",
                removedAnnotations: new Dictionary<string, object>
                {
                    { "DynamicFilter_Airport_IsNotDeleted", "EntityFramework.DynamicFilters.DynamicFilterDefinition" },
                });
            DropTable("dbo.Flights",
                removedAnnotations: new Dictionary<string, object>
                {
                    { "DynamicFilter_Flight_IsNotDeleted", "EntityFramework.DynamicFilters.DynamicFilterDefinition" },
                });
            DropTable("dbo.Airlines",
                removedAnnotations: new Dictionary<string, object>
                {
                    { "DynamicFilter_Airline_IsNotDeleted", "EntityFramework.DynamicFilters.DynamicFilterDefinition" },
                });
            DropTable("dbo.Aircraft",
                removedAnnotations: new Dictionary<string, object>
                {
                    { "DynamicFilter_Aircraft_IsNotDeleted", "EntityFramework.DynamicFilters.DynamicFilterDefinition" },
                });
        }
    }
}
