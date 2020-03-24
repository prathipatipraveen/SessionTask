﻿CREATE TABLE [dbo].[Feature]
(
	[FeatureId] INT IDENTITY(1,1) NOT NULL,
	[FeatureName] VARCHAR(50) NOT NULL
CONSTRAINT [PK_Feature] PRIMARY KEY CLUSTERED   
(  
    [FeatureId] ASC  
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]  
) ON [PRIMARY]  
Go