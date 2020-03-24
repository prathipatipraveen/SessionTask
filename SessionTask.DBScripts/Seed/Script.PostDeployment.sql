USE [ResumeCreator]
GO
SET IDENTITY_INSERT [dbo].[Feature] ON 
GO
INSERT [dbo].[Feature] ([FeatureId], [FeatureName]) VALUES (1, N'Event')
GO
INSERT [dbo].[Feature] ([FeatureId], [FeatureName]) VALUES (2, N'Session')
GO
INSERT [dbo].[Feature] ([FeatureId], [FeatureName]) VALUES (3, N'Enroll')
GO
INSERT [dbo].[Feature] ([FeatureId], [FeatureName]) VALUES (4, N'Attendees')
GO
SET IDENTITY_INSERT [dbo].[Feature] OFF
GO
SET IDENTITY_INSERT [dbo].[Permission] ON 
GO
INSERT [dbo].[Permission] ([PermissionId], [Name]) VALUES (1, N'Read')
GO
INSERT [dbo].[Permission] ([PermissionId], [Name]) VALUES (2, N'Create')
GO
INSERT [dbo].[Permission] ([PermissionId], [Name]) VALUES (3, N'Update')
GO
INSERT [dbo].[Permission] ([PermissionId], [Name]) VALUES (4, N'Delete')
GO
INSERT [dbo].[Permission] ([PermissionId], [Name]) VALUES (5, N'Execute')
GO
SET IDENTITY_INSERT [dbo].[Permission] OFF
GO
SET IDENTITY_INSERT [dbo].[Role] ON 
GO
INSERT [dbo].[Role] ([RoleId], [RoleName]) VALUES (1, N'Admin')
GO
INSERT [dbo].[Role] ([RoleId], [RoleName]) VALUES (2, N'Attendee')
GO
INSERT [dbo].[Role] ([RoleId], [RoleName]) VALUES (3, N'Host')
GO
SET IDENTITY_INSERT [dbo].[Role] OFF
GO
SET IDENTITY_INSERT [dbo].[FeatureRolePermissionXref] ON 
GO
INSERT [dbo].[FeatureRolePermissionXref] ([FeatureRolePermissionXrefId], [FeatureId], [RoleId], [PermissionId]) VALUES (1, 3, 2, 2)
GO
INSERT [dbo].[FeatureRolePermissionXref] ([FeatureRolePermissionXrefId], [FeatureId], [RoleId], [PermissionId]) VALUES (2, 4, 3, 1)
GO
INSERT [dbo].[FeatureRolePermissionXref] ([FeatureRolePermissionXrefId], [FeatureId], [RoleId], [PermissionId]) VALUES (3, 1, 1, 2)
GO
INSERT [dbo].[FeatureRolePermissionXref] ([FeatureRolePermissionXrefId], [FeatureId], [RoleId], [PermissionId]) VALUES (4, 1, 1, 3)
GO
INSERT [dbo].[FeatureRolePermissionXref] ([FeatureRolePermissionXrefId], [FeatureId], [RoleId], [PermissionId]) VALUES (5, 2, 1, 2)
GO
INSERT [dbo].[FeatureRolePermissionXref] ([FeatureRolePermissionXrefId], [FeatureId], [RoleId], [PermissionId]) VALUES (6, 2, 1, 3)
GO
INSERT [dbo].[FeatureRolePermissionXref] ([FeatureRolePermissionXrefId], [FeatureId], [RoleId], [PermissionId]) VALUES (7, 4, 1, 1)
GO
INSERT [dbo].[FeatureRolePermissionXref] ([FeatureRolePermissionXrefId], [FeatureId], [RoleId], [PermissionId]) VALUES (8, 4, 1, 3)
GO
INSERT [dbo].[FeatureRolePermissionXref] ([FeatureRolePermissionXrefId], [FeatureId], [RoleId], [PermissionId]) VALUES (9, 1, 1, 1)
GO
INSERT [dbo].[FeatureRolePermissionXref] ([FeatureRolePermissionXrefId], [FeatureId], [RoleId], [PermissionId]) VALUES (10, 1, 2, 1)
GO
INSERT [dbo].[FeatureRolePermissionXref] ([FeatureRolePermissionXrefId], [FeatureId], [RoleId], [PermissionId]) VALUES (11, 1, 3, 1)
GO
INSERT [dbo].[FeatureRolePermissionXref] ([FeatureRolePermissionXrefId], [FeatureId], [RoleId], [PermissionId]) VALUES (12, 2, 1, 1)
GO
INSERT [dbo].[FeatureRolePermissionXref] ([FeatureRolePermissionXrefId], [FeatureId], [RoleId], [PermissionId]) VALUES (13, 2, 2, 1)
GO
INSERT [dbo].[FeatureRolePermissionXref] ([FeatureRolePermissionXrefId], [FeatureId], [RoleId], [PermissionId]) VALUES (14, 2, 3, 1)
GO
SET IDENTITY_INSERT [dbo].[FeatureRolePermissionXref] OFF
GO
SET IDENTITY_INSERT [dbo].[User] ON 
GO
INSERT [dbo].[User] ([UserId], [UserName], [Password], [IsActive]) VALUES (1, N'admin', N'admin', 1)
GO
INSERT [dbo].[User] ([UserId], [UserName], [Password], [IsActive]) VALUES (2, N'attendee', N'attendee', 1)
GO
INSERT [dbo].[User] ([UserId], [UserName], [Password], [IsActive]) VALUES (3, N'Host', N'host', 1)
GO
SET IDENTITY_INSERT [dbo].[User] OFF
GO
SET IDENTITY_INSERT [dbo].[UserRoleXref] ON 
GO
INSERT [dbo].[UserRoleXref] ([UserRoleXrefId], [UserId], [RoleId]) VALUES (2, 1, 1)
GO
INSERT [dbo].[UserRoleXref] ([UserRoleXrefId], [UserId], [RoleId]) VALUES (3, 2, 2)
GO
INSERT [dbo].[UserRoleXref] ([UserRoleXrefId], [UserId], [RoleId]) VALUES (6, 3, 3)
GO
SET IDENTITY_INSERT [dbo].[UserRoleXref] OFF
GO
SET IDENTITY_INSERT [dbo].[UserSessionXref] ON 
GO
INSERT [dbo].[UserSessionXref] ([UserSessionXrefId], [UserId], [SessionId], [IsApproved]) VALUES (1, 1, 1, 1)
GO
INSERT [dbo].[UserSessionXref] ([UserSessionXrefId], [UserId], [SessionId], [IsApproved]) VALUES (3, 2, 1, 1)
GO
SET IDENTITY_INSERT [dbo].[UserSessionXref] OFF
GO
