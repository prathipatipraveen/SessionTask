CREATE TABLE [dbo].[FeatureRolePermissionXref]
(
	[FeatureRolePermissionXrefId] INT IDENTITY(1,1) NOT NULL,
	[FeatureId] INT NOT NULL,
	[RoleId] INT NOT NULL,
	[PermissionId] INT NOT NULL
CONSTRAINT [PK_FeatureRolePermissionXref] PRIMARY KEY CLUSTERED   
(  
    [FeatureRolePermissionXrefId] ASC  
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]  
) ON [PRIMARY]  
GO
ALTER TABLE [dbo].[FeatureRolePermissionXref]  ADD  CONSTRAINT [FK_Role_FeatureRolePermissionXref] FOREIGN KEY([RoleId])
REFERENCES [dbo].[Role] ([RoleId])
GO
ALTER TABLE [dbo].[FeatureRolePermissionXref]  ADD  CONSTRAINT [FK_Feature_FeatureRolePermissionXref] FOREIGN KEY([FeatureId])
REFERENCES [dbo].[Feature] ([FeatureId])
GO
ALTER TABLE [dbo].[FeatureRolePermissionXref]  ADD  CONSTRAINT [FK_Permission_FeatureRolePermissionXref] FOREIGN KEY([PermissionId])
REFERENCES [dbo].[Permission] ([PermissionId])
GO