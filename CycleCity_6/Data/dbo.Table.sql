CREATE TABLE [dbo].[Table] (
    [Id]         NVARCHAR(50)              NOT NULL,
    [StartTime]  DATETIME         NOT NULL,
    [EndTime]    DATETIME         NOT NULL,
    [Tour]       [sys].[geometry] NOT NULL,
    [StartPoint] [sys].[geometry] NOT NULL,
    [EndPoint]   [sys].[geometry] NOT NULL,
    PRIMARY KEY CLUSTERED ([Id] ASC)
);

