CREATE TABLE [dbo].[UserSessionXref]
(
	[UserSessionXrefId] INT IDENTITY(1,1) NOT NULL,
	[UserId] INT NOT NULL,
	[SessionId] INT NOT NULL,
	[IsApproved] BIT NOT NULL
CONSTRAINT [PK_UserSessionXref] PRIMARY KEY CLUSTERED   
(  
    [UserSessionXrefId] ASC  
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]  
) ON [PRIMARY]  
GO
ALTER TABLE [dbo].[UserSessionXref]  ADD  CONSTRAINT [FK_User_UserSessionXref] FOREIGN KEY([UserId])
REFERENCES [dbo].[User] ([UserId])
GO
ALTER TABLE [dbo].[UserSessionXref]  ADD  CONSTRAINT [FK_Session_UserSessionXref] FOREIGN KEY([SessionId])
REFERENCES [dbo].[Session] ([SessionId])
GO