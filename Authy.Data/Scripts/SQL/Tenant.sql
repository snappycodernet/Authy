CREATE SCHEMA [common]
GO

PRINT '-- Tenant'

-- A Tenant represents a customer of OBRC that has licensed
-- the BottleDrop system for their own use.
CREATE TABLE [common].[Tenant] (
    [Id] [int] NOT NULL IDENTITY(1, 1),
    [Name] [nvarchar] (4000) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
    [Code] [nvarchar] (5) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
)

CREATE UNIQUE INDEX [UNIQ_Tenant_Code] ON
    [common].[Tenant] ([Code]);

ALTER TABLE [common].[Tenant] ADD CONSTRAINT [PK_Tenant] PRIMARY KEY CLUSTERED  ([Id]) ON [PRIMARY]

INSERT INTO [common].[Tenant] (Name, Code)
VALUES
    ('Oregon', 'OR'),
    ('San Francisco', 'SF'),
    ('Iowa', 'IA')

-- TenantPreferences will allow Tenants to customize the appearance
-- and behavior of the product for their own users. Examples of
-- preferences that might be used in the future include:
-- - The White Label name and brand colors
-- - Whether or not certain tenant-level features are enabled like Use
--   My Balance, or College Savings Plans

CREATE TABLE [dbo].[TenantPreference]
(
    [TenantId] [int] NOT NULL,
    [Id] [int] NOT NULL IDENTITY(1, 1),
    [Preference] [nvarchar] (4000) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
    [Value] [nvarchar] (4000) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
    CONSTRAINT [PK_TenantPreference]
        PRIMARY KEY CLUSTERED ([TenantId], [Id]),
    CONSTRAINT [FK_TenantPreference_TenantId]
        FOREIGN KEY ([TenantId]) REFERENCES [common].[Tenant] ([Id])
)

INSERT INTO [dbo].[TenantPreference] (TenantId, Preference, Value)
VALUES (1, 'USE_MY_BALANCE', 'Y'), (2, 'USE_MY_BALANCE', 'Y')