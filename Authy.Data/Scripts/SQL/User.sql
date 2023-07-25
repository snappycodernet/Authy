PRINT '-- User'

-- A User represents a person's login on the system and
-- contains the data required for authentication as well
-- as data that is common between AccountHolder and AdminUser
-- * A User is confined to a single Tenant
-- * A human being may have multiple Users across multiple Tenants
-- * The Email field doubles as a unique username on each Tenant.
-- * The same e-mail may be used across multiple Tenants, but only
--   once in a single Tenant. If a person wants to use the "same"
--   e-mail multiple times on a single Tenant they must suffix the
--   e-mail with something like jeff+admin@pndlm.com, jeff+personal@pndlm.com
CREATE TABLE [dbo].[User]
(
	[TenantId]              INT NOT NULL,
	[Id]                    BIGINT IDENTITY,
	[FirstName]             NVARCHAR(100),
	[MiddleName]            NVARCHAR(100),
	[LastName]              NVARCHAR(100),
	[Email]                 NVARCHAR(100),
	[PIN]                   NVARCHAR(50),
	[PasswordHash]          NVARCHAR(200),
	[IsActive]              BIT NOT NULL,
	[Salt]                  NVARCHAR(40)
        CONSTRAINT [DF_User_Salt] DEFAULT NEWID(),
	[LastModifiedUserId]    BIGINT,
	[CreatedTimestamp]      DATETIME
		CONSTRAINT [DF_User_CreatedTimestamp] DEFAULT GETUTCDATE(),
	[LastModifiedTimestamp] DATETIME
		CONSTRAINT [DF_User_LastModifiedTimestamp] DEFAULT GETUTCDATE(),
	CONSTRAINT [PK_User]
		PRIMARY KEY CLUSTERED ([TenantId], [Id]),
	CONSTRAINT [FK_User_TenantId]
		FOREIGN KEY ([TenantId]) REFERENCES [dbo].[Tenant],
);

CREATE UNIQUE INDEX [UNIQ_User_Email]
	ON [dbo].[User] ([TenantId], [Email]);

CREATE TABLE [dbo].[ApiKey]
(
	[TenantId]      INT	NOT NULL,
	[Id]            NVARCHAR(200) NOT NULL,
	[UserAuthId]    NVARCHAR(50) NOT NULL,
	[Environment]   NVARCHAR(50),
	[KeyType]       NVARCHAR(50),
	[CreatedDate]   DATETIME
		CONSTRAINT [DF_ApiKey_CreatedDate] DEFAULT GETUTCDATE() NOT NULL,
	[ExpiryDate]    DATETIME,
	[CancelledDate] DATETIME,
	[Notes]         NVARCHAR(2000),
	[RefIdStr]      NVARCHAR(200),
	[RefId]         INT,
	[Meta]          NVARCHAR(200),
	CONSTRAINT [PK_ApiKey] PRIMARY KEY CLUSTERED ([TenantId], [Id]),
	CONSTRAINT [FK_ApiKey_TenantId]
		FOREIGN KEY ([TenantId]) REFERENCES [dbo].[Tenant],
);