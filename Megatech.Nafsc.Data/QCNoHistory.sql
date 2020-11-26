
CREATE TABLE [dbo].[QCNoHistories] (
    [Id] [int] NOT NULL IDENTITY,
    [StartDate] [datetime] NOT NULL,
    [QCNo] [nvarchar](max),
    [ProductId] [int],
    [AirtportId] [int],
    [DateCreated] [datetime] NOT NULL,
    [DateUpdated] [datetime] NOT NULL,
    [UserCreatedId] [int],
    [UserUpdatedId] [int],
    [IsDeleted] [bit] NOT NULL,
    [DateDeleted] [datetime],
    [UserDeletedId] [int],
    CONSTRAINT [PK_dbo.QCNoHistories] PRIMARY KEY ([Id])
)
