CREATE TABLE [dbo].[FeatureUserPermissionXref]
(
	[FeatureUserPermissionXrefId] INT IDENTITY(1,1) NOT NULL,
	[FeatureId] INT NOT NULL,
	[UserId] INT NOT NULL,
	[PermissionId] INT NOT NULL
CONSTRAINT [PK_FeatureUserPermissionXref] PRIMARY KEY CLUSTERED   
(  
    [FeatureUserPermissionXrefId] ASC  
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]  
) ON [PRIMARY]  
GO
ALTER TABLE [dbo].[FeatureUserPermissionXref]  ADD  CONSTRAINT [FK_User_FeatureUserPermissionXref] FOREIGN KEY([UserId])
REFERENCES [dbo].[User] ([UserId])
GO
ALTER TABLE [dbo].[FeatureUserPermissionXref]  ADD  CONSTRAINT [FK_Feature_FeatureUserPermissionXref] FOREIGN KEY([FeatureId])
REFERENCES [dbo].[Feature] ([FeatureId])
GO
ALTER TABLE [dbo].[FeatureUserPermissionXref]  ADD  CONSTRAINT [FK_Permission_FeatureUserPermissionXref]FOREIGN KEY([PermissionId])
REFERENCES [dbo].[Permission] ([PermissionId])
GO