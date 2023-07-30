PRINT '-- Permissions'

CREATE TABLE [common].[PermissionType]
(
	[Id]   TINYINT IDENTITY,
	[Code] NVARCHAR(50) NOT NULL,
	[Name] NVARCHAR(50) NOT NULL,
	CONSTRAINT [PK_PermissionType]
		PRIMARY KEY ([Id])
);

CREATE UNIQUE INDEX [UNIQ_PermissionType_Code] ON
	[common].[PermissionType] ([Code]);

CREATE TABLE [common].[Permission]
(
	[Id]                    INT IDENTITY,
	[Code]                  NVARCHAR(50)                      NOT NULL,
	[Name]                  NVARCHAR(50)                      NOT NULL,
	[Description]           NVARCHAR(2000),
	[PermissionTypeId]      TINYINT                           NOT NULL,
	[IsCorePermission]      BIT
		CONSTRAINT [DF_Permission_IsCorePermission] DEFAULT 0 NOT NULL, -- TODO: what is this?
	[ReportPath]            NVARCHAR(255),
	[PermissionKey]         NVARCHAR(50)                      NOT NULL,
	[CreatedTimestamp]      DATETIME
		CONSTRAINT [DF_Permission_CreatedTimestamp] DEFAULT GETUTCDATE(),
	[LastModifiedTimestamp] DATETIME
		CONSTRAINT [DF_Permission_LastModifiedTimestamp] DEFAULT GETUTCDATE(),
	CONSTRAINT [PK_Permission]
		PRIMARY KEY ([Id]),
	CONSTRAINT [FK_Permission_PermissionType]
		FOREIGN KEY ([PermissionTypeId]) REFERENCES [common].[PermissionType]
);

CREATE UNIQUE INDEX [UNIQ_Permission_Code] ON
	[common].[Permission] ([Code]);

CREATE TABLE [common].[Role]
(
	[Id]                    INT IDENTITY,
	[Code]                  NVARCHAR(50) NOT NULL,
	[Name]                  NVARCHAR(50),
	[Description]           NVARCHAR(2000),
	[IsCoreRole]            BIT
		CONSTRAINT [DF_Roles_IsCoreRole] DEFAULT 0 NOT NULL,
	[CreatedTimestamp]      DATETIME,
	[LastModifiedTimestamp] DATETIME,
	CONSTRAINT [PK_Role]
		PRIMARY KEY ([Id])
);

INSERT INTO [common].[Role] (Code, Name, Description, IsCoreRole, CreatedTimestamp, LastModifiedTimestamp)
VALUES
    ('USER', 'User', 'Regular user', 0, GETUTCDATE(), GETUTCDATE()),
    ('ADMIN', 'Admin', 'Admin user', 1, GETUTCDATE(), GETUTCDATE())

CREATE UNIQUE INDEX [UNIQ_Role_Code] ON
	[common].[Role] ([Code]);

-- If possible, let's avoid letting any users modify
-- these values from the application. These should only
-- be modified by schema changes to avoid complexity
-- and risk of security holes.
CREATE TABLE [common].[RolePermission]
(
	[Id]                    INT IDENTITY,
	[RoleId]                INT NOT NULL,
	[PermissionId]          INT NOT NULL,
	[CreatedTimestamp]      DATETIME
		CONSTRAINT [DF_RolePermission_CreatedTimestamp] DEFAULT GETUTCDATE(),
	[LastModifiedTimestamp] DATETIME
		CONSTRAINT [DF_RolePermission_LastModifiedTimestamp] DEFAULT GETUTCDATE(),
	CONSTRAINT [PK_RolePermission]
		PRIMARY KEY ([Id]),
	CONSTRAINT [FK_RolePermission_PermissionId]
		FOREIGN KEY ([PermissionId]) REFERENCES [common].[Permission],
	CONSTRAINT [FK_RolePermission_RoleId]
		FOREIGN KEY ([RoleId]) REFERENCES [common].[Role]
);

-- NOTE: It looks like records in this table will be added and deleted
-- when User-specific permissions are changed making the LastModifiedUserId
-- not very useful? So I've commented it out for now.
CREATE TABLE [dbo].[UserPermission]
(
	[TenantId]              INT NOT NULL,
	[Id]                    INT IDENTITY,
	[UserId]                BIGINT NOT NULL,
	[PermissionId]          INT NOT NULL,
-- 	[LastModifiedUserId]    BIGINT,
	[CreatedTimestamp]      DATETIME
		CONSTRAINT [DF_UserPermission_CreatedTimestamp] DEFAULT GETUTCDATE(),
	[LastModifiedTimestamp] DATETIME
		CONSTRAINT [DF_UserPermission_LastModifiedTimestamp] DEFAULT GETUTCDATE(),
	CONSTRAINT [PK_UserPermission]
		PRIMARY KEY CLUSTERED ([TenantId], [Id]),
	CONSTRAINT [FK_UserPermission_TenantId]
		FOREIGN KEY ([TenantId]) REFERENCES [common].[Tenant],
	CONSTRAINT [FK_UserPermission_PermissionId]
		FOREIGN KEY ([PermissionId]) REFERENCES [common].[Permission],
	CONSTRAINT [FK_UserPermission_UserId]
		FOREIGN KEY ([TenantId], [UserId]) REFERENCES [dbo].[User],
-- 	CONSTRAINT [FK_UserPermission_LastModifiedUserId]
-- 		FOREIGN KEY ([TenantId], [LastModifiedUserId]) REFERENCES [dbo].[User],
);

-- TODO: consider simplifying this and just putting a [RoleId] on the [User] table.
-- NOTE: It looks like records in this table will be added and deleted
-- when User-specific permissions are changed making the LastModifiedUserId
-- not very useful? So I've commented it out for now.
CREATE TABLE [dbo].[UserRole]
(
	[TenantId]              INT NOT NULL,
	[Id]                    INT IDENTITY,
	[UserId]                BIGINT NOT NULL,
	[RoleId]                INT NOT NULL,
-- 	[LastModifiedUserId]    BIGINT,
	[CreatedTimestamp]      DATETIME
		CONSTRAINT [DF_UserRole_CreatedTimestamp] DEFAULT GETUTCDATE(),
	[LastModifiedTimestamp] DATETIME
		CONSTRAINT [DF_UserRole_LastModifiedTimestamp] DEFAULT GETUTCDATE(),
	CONSTRAINT [PK_UserRole]
		PRIMARY KEY CLUSTERED ([TenantId], [Id]),
	CONSTRAINT [FK_UserRole_TenantId]
		FOREIGN KEY ([TenantId]) REFERENCES [common].[Tenant],
	CONSTRAINT [FK_UsersRole_RoleId]
		FOREIGN KEY ([RoleId]) REFERENCES [common].[Role],
	CONSTRAINT [FK_UsersRole_UserId]
		FOREIGN KEY ([TenantId], [UserId]) REFERENCES [dbo].[User],
-- 	CONSTRAINT [FK_UserRole_LastModifiedUserId]
-- 		FOREIGN KEY ([TenantId], [LastModifiedUserId]) REFERENCES [dbo].[User],
);

