
CREATE TABLE [dbo].[Invoices] (
    [Id] [int] NOT NULL IDENTITY,
    [FlightId] [int] NOT NULL,
    [ParentId] [int],
    [ChildId] [int],
    [InvoiceNumber] [nvarchar](max),
    [CustomerId] [int] NOT NULL,
    [CustomerName] [nvarchar](max),
    [TaxCode] [nvarchar](max),
    [Address] [nvarchar](max),
    [Volume] [decimal](18, 2) NOT NULL,
    [Weight] [decimal](18, 2) NOT NULL,
    [Gallon] [decimal](18, 2) NOT NULL,
    [Temperature] [decimal](18, 2) NOT NULL,
    [Density] [decimal](18, 2) NOT NULL,
    [ProductName] [nvarchar](max),
    [Price] [decimal](18, 2) NOT NULL,
    [TaxRate] [decimal](18, 2) NOT NULL,
    [SubTotal] [decimal](18, 2) NOT NULL,
    [Tax] [decimal](18, 2) NOT NULL,
    [DateCreated] [datetime] NOT NULL,
    [DateUpdated] [datetime] NOT NULL,
    [UserCreatedId] [int],
    [UserUpdatedId] [int],
    [IsDeleted] [bit] NOT NULL,
    [DateDeleted] [datetime],
    [UserDeletedId] [int],
    CONSTRAINT [PK_dbo.Invoices] PRIMARY KEY ([Id])
)
CREATE INDEX [IX_ParentId] ON [dbo].[Invoices]([ParentId])
CREATE INDEX [IX_ChildId] ON [dbo].[Invoices]([ChildId])
CREATE TABLE [dbo].[InvoiceItems] (
    [Id] [int] NOT NULL IDENTITY,
    [InvoiceId] [int] NOT NULL,
    [RealAmount] [decimal](18, 2) NOT NULL,
    [Volume] [decimal](18, 2) NOT NULL,
    [Weight] [decimal](18, 2) NOT NULL,
    [Gallon] [decimal](18, 2) NOT NULL,
    [Density] [decimal](18, 2) NOT NULL,
    [Temperature] [decimal](18, 2) NOT NULL,
    [TruckNo] [nvarchar](max),
    [StartNumber] [decimal](18, 2) NOT NULL,
    [EndNumber] [decimal](18, 2) NOT NULL,
    [DateCreated] [datetime] NOT NULL,
    [DateUpdated] [datetime] NOT NULL,
    [UserCreatedId] [int],
    [UserUpdatedId] [int],
    [IsDeleted] [bit] NOT NULL,
    [DateDeleted] [datetime],
    [UserDeletedId] [int],
    CONSTRAINT [PK_dbo.InvoiceItems] PRIMARY KEY ([Id])
)
CREATE INDEX [IX_InvoiceId] ON [dbo].[InvoiceItems]([InvoiceId])
ALTER TABLE [dbo].[Invoices] ADD CONSTRAINT [FK_dbo.Invoices_dbo.Invoices_ChildId] FOREIGN KEY ([ChildId]) REFERENCES [dbo].[Invoices] ([Id])
ALTER TABLE [dbo].[Invoices] ADD CONSTRAINT [FK_dbo.Invoices_dbo.Invoices_ParentId] FOREIGN KEY ([ParentId]) REFERENCES [dbo].[Invoices] ([Id])
ALTER TABLE [dbo].[InvoiceItems] ADD CONSTRAINT [FK_dbo.InvoiceItems_dbo.Invoices_InvoiceId] FOREIGN KEY ([InvoiceId]) REFERENCES [dbo].[Invoices] ([Id]) ON DELETE CASCADE

ALTER TABLE dbo.RefuelItems ADD
	InvoiceId int NULL
GO
ALTER TABLE dbo.RefuelItems ADD CONSTRAINT
	[FK_dbo.RefuelItems_dbo.Invoices_InvoiceId] FOREIGN KEY
	(
	InvoiceId
	) REFERENCES dbo.Invoices
	(
	Id
	) ON UPDATE  NO ACTION 
	 ON DELETE  NO ACTION 
	
GO