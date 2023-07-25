-- A Tenant represents a customer of OBRC that has licensed
-- the BottleDrop system for their own use.
CREATE TABLE [dbo].[Tenant] (
    [Id] [int] NOT NULL IDENTITY(1, 1),
    [Name] [nvarchar] (4000) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
    [Code] [nvarchar] (5) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
)

CREATE UNIQUE INDEX [UNIQ_Tenant_Code] ON
    [dbo].[Tenant] ([Code]);

ALTER TABLE [dbo].[Tenant] ADD CONSTRAINT [PK_Tenant] PRIMARY KEY CLUSTERED  ([Id]) ON [PRIMARY]

INSERT INTO [dbo].[Tenant] (Name, Code)
VALUES
    ('Team Schrute', 'TS'),
    ('Team Halpert', 'TH')