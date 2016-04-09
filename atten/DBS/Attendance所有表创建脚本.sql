/*****************************************部门信息************************************************/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[T_Department](
	[F_DepID] [int] IDENTITY(1,1) NOT NULL,			--部门ID
	[F_DepName] [nvarchar](50) NOT NULL,			--部门名称
PRIMARY KEY CLUSTERED 
(
	[F_DepID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

/*****************************************员工信息************************************************/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[T_EmployeeInfo](
	[F_EmpID] [int] IDENTITY(1,1) NOT NULL,			--员工ID
	[F_EmpName] [nvarchar](50) NOT NULL,			--员工姓名
	[F_EmpRole] [nvarchar](10) NULL,				--员工角色（格式：,1,2,3,）
	[F_DepID] [int] NULL,							--员工所属部门ID
	[F_DepName] [nvarchar](50) NULL,				--员工所属部门名称
	[F_EmpAccount] [nvarchar](50) NOT NULL,			--员工登录账号
	[F_EmpPwd] [nvarchar](50) NOT NULL,				--密码
	[F_EmpCreateDate] [date] NULL,					--创建时间
 CONSTRAINT [PK__T_Employ__1D6EF83B03317E3D] PRIMARY KEY CLUSTERED 
(
	[F_EmpID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
--关联表--
ALTER TABLE [dbo].[T_EmployeeInfo]  WITH CHECK ADD  CONSTRAINT [FK_EmployeeInfo_Department] FOREIGN KEY([F_DepID])
REFERENCES [dbo].[T_Department] ([F_DepID])
GO

ALTER TABLE [dbo].[T_EmployeeInfo] CHECK CONSTRAINT [FK_EmployeeInfo_Department]
GO

/*****************************************签到信息************************************************/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

SET ANSI_PADDING ON
GO

CREATE TABLE [dbo].[T_CheckingInInfo](
	[F_CIID] [int] IDENTITY(1,1) NOT NULL,			--每天签到信息ID
	[F_EmpID] [int] NOT NULL,						--签到员工ID
	[F_EmpName] [nvarchar](50) NOT NULL,			--签到员工姓名
	[F_DepID] [int] NOT NULL,						--签到员工部门ID
	[F_DepName] [nvarchar](50) NOT NULL,			--签到员工部门名称
	[F_AppendSignInPersonID] [int] NULL,			--补签到人ID
	[F_AppendSignInPersonName] [varchar](50) NULL,	--补签到人姓名
	[F_AppendSignInPersonNote] [varchar](256) NULL,	--补签到说明
	[F_AppendSignOutPersonID] [int] NULL,			--补签退人ID
	[F_AppendSignOutPersonName] [varchar](50) NULL,	--补签退人姓名
	[F_AppendSignOutPersonNote] [varchar](500) NULL,--补签退说明
	[F_CISignInDate] [datetime] NULL,				--签到时间
	[F_CISignOutDate] [datetime] NULL,				--签退时间
	[F_CIRealityWorkDuration] [int] NULL,			--实际工作时间
	[F_CIIsLate] [bit] NULL,						--是否迟到
	[F_CIIsLeaveEarvly] [bit] NULL,					--是否早退
	[F_CIIsAbsenteeism] [bit] NULL,					--是否缺席
	[F_CIIsNormal] [bit] NULL,						--是否正常
	[F_CICreateDate] [date] NULL,					--创建时间
	[F_CIIsCalculate] [bit] NULL,					--是否已经被统计
	[F_Date] [date] NULL,							--和签到相同的日期
	[F_CIIsSignIn] [bit] NULL,						--是否是员工自己签到
	[F_CIIsSignOut] [bit] NULL,						--是否为员工自己签退
 CONSTRAINT [PK__T_Checki__0BA27F4C07F6335A] PRIMARY KEY CLUSTERED 
(
	[F_CIID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

SET ANSI_PADDING OFF
GO

ALTER TABLE [dbo].[T_CheckingInInfo]  WITH CHECK ADD  CONSTRAINT [FK_CheckingInInfo_Department] FOREIGN KEY([F_DepID])
REFERENCES [dbo].[T_Department] ([F_DepID])
GO

ALTER TABLE [dbo].[T_CheckingInInfo] CHECK CONSTRAINT [FK_CheckingInInfo_Department]
GO

ALTER TABLE [dbo].[T_CheckingInInfo]  WITH CHECK ADD  CONSTRAINT [FK_CheckingInInfo_EmployeeInfo] FOREIGN KEY([F_EmpID])
REFERENCES [dbo].[T_EmployeeInfo] ([F_EmpID])
GO

ALTER TABLE [dbo].[T_CheckingInInfo] CHECK CONSTRAINT [FK_CheckingInInfo_EmployeeInfo]
GO

/*****************************************统计信息************************************************/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[T_AttendanceStatistics](
	[F_ASID] [int] IDENTITY(1,1) NOT NULL,			--统计信息ID
	[F_EmpID] [int] NOT NULL,						--被统计的员工ID
	[F_EmpName] [nvarchar](50) NOT NULL,			--被统计员工姓名
	[F_DepID] [int] NOT NULL,						--被统计员工所属的部门ID
	[F_DepName] [nvarchar](50) NOT NULL,			--被统计员工所属部门ID
	[F_ASMonth] [int] NULL,							--统计的月份（如：201512表示2015年12月）
	[F_ASStandardDuration] [int] NULL,				--当月标准工作时长
	[F_ASRealityDuration] [int] NULL,				--当月实际工作时长
	[F_ASLateNumber] [int] NULL,					--迟到次数
	[F_ASLeaveEavlyNumber] [int] NULL,				--早退次数
	[F_ASAbsenteeismNumber] [int] NULL,				--缺席次数
	[F_ASNormalNumber] [int] NULL,					--正常次数
	[F_ASCreateDate] [datetime] NULL,				--创建时间
PRIMARY KEY CLUSTERED 
(
	[F_ASID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

ALTER TABLE [dbo].[T_AttendanceStatistics]  WITH CHECK ADD  CONSTRAINT [FK_AttendanceStatistics_Department] FOREIGN KEY([F_DepID])
REFERENCES [dbo].[T_Department] ([F_DepID])
GO

ALTER TABLE [dbo].[T_AttendanceStatistics] CHECK CONSTRAINT [FK_AttendanceStatistics_Department]
GO

ALTER TABLE [dbo].[T_AttendanceStatistics]  WITH CHECK ADD  CONSTRAINT [FK_AttendanceStatistics_EmployeeInfo] FOREIGN KEY([F_EmpID])
REFERENCES [dbo].[T_EmployeeInfo] ([F_EmpID])
GO

ALTER TABLE [dbo].[T_AttendanceStatistics] CHECK CONSTRAINT [FK_AttendanceStatistics_EmployeeInfo]
GO

/*****************************************时间统计************************************************/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

SET ANSI_PADDING ON
GO

CREATE TABLE [dbo].[T_WorkDuration](
	[F_WDID] [int] IDENTITY(1,1) NOT NULL,
	[F_WDMonthDuration] [int] NOT NULL,
	[F_WDSignInMonth] [varchar](50) NOT NULL,
	[F_WDDayDurattion] [int] NULL,
	[F_NormalTime] [nvarchar](10) NULL,
	[F_AbsentSignInTime] [nvarchar](10) NULL,
	[F_AbsentSignOutTime] [nvarchar](10) NULL,
	[F_LeaveEarlyTime] [nvarchar](10) NULL,
	[F_WDCreateDate] [datetime] NULL,
	[F_WDFloatTime] [int] NULL,
 CONSTRAINT [PK__T_WorkDu__E92216AE1367E606] PRIMARY KEY CLUSTERED 
(
	[F_WDID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

SET ANSI_PADDING OFF
GO

/*****************************************初始数据************************************************/
INSERT INTO T_WorkDuration(F_WDMonthDuration, F_WDSignInMonth, F_WDDayDurattion, F_NormalTime, F_AbsentSignInTime, F_AbsentSignOutTime, F_LeaveEarlyTime, F_WDCreateDate, F_WDFloatTime)
VALUES(22, '2015-11', 9, '09:00', '10:00', '17:00', '18:00', '2015-11-24', 30);
INSERT INTO T_Department(F_DepName) VALUES('研发部');
INSERT INTO T_EmployeeInfo(F_EmpName, F_EmpAccount, F_EmpPwd, F_EmpRole, F_DepID, F_DepName, F_EmpCreateDate) 
VALUES('admin', 'admin', 'e10adc3949ba59abbe56e057f20f883e', ',4,', 1, '研发部', '2015-11-24');