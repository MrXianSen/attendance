/*****************************************������Ϣ************************************************/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[T_Department](
	[F_DepID] [int] IDENTITY(1,1) NOT NULL,			--����ID
	[F_DepName] [nvarchar](50) NOT NULL,			--��������
PRIMARY KEY CLUSTERED 
(
	[F_DepID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

/*****************************************Ա����Ϣ************************************************/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[T_EmployeeInfo](
	[F_EmpID] [int] IDENTITY(1,1) NOT NULL,			--Ա��ID
	[F_EmpName] [nvarchar](50) NOT NULL,			--Ա������
	[F_EmpRole] [nvarchar](10) NULL,				--Ա����ɫ����ʽ��,1,2,3,��
	[F_DepID] [int] NULL,							--Ա����������ID
	[F_DepName] [nvarchar](50) NULL,				--Ա��������������
	[F_EmpAccount] [nvarchar](50) NOT NULL,			--Ա����¼�˺�
	[F_EmpPwd] [nvarchar](50) NOT NULL,				--����
	[F_EmpCreateDate] [date] NULL,					--����ʱ��
 CONSTRAINT [PK__T_Employ__1D6EF83B03317E3D] PRIMARY KEY CLUSTERED 
(
	[F_EmpID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
--������--
ALTER TABLE [dbo].[T_EmployeeInfo]  WITH CHECK ADD  CONSTRAINT [FK_EmployeeInfo_Department] FOREIGN KEY([F_DepID])
REFERENCES [dbo].[T_Department] ([F_DepID])
GO

ALTER TABLE [dbo].[T_EmployeeInfo] CHECK CONSTRAINT [FK_EmployeeInfo_Department]
GO

/*****************************************ǩ����Ϣ************************************************/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

SET ANSI_PADDING ON
GO

CREATE TABLE [dbo].[T_CheckingInInfo](
	[F_CIID] [int] IDENTITY(1,1) NOT NULL,			--ÿ��ǩ����ϢID
	[F_EmpID] [int] NOT NULL,						--ǩ��Ա��ID
	[F_EmpName] [nvarchar](50) NOT NULL,			--ǩ��Ա������
	[F_DepID] [int] NOT NULL,						--ǩ��Ա������ID
	[F_DepName] [nvarchar](50) NOT NULL,			--ǩ��Ա����������
	[F_AppendSignInPersonID] [int] NULL,			--��ǩ����ID
	[F_AppendSignInPersonName] [varchar](50) NULL,	--��ǩ��������
	[F_AppendSignInPersonNote] [varchar](256) NULL,	--��ǩ��˵��
	[F_AppendSignOutPersonID] [int] NULL,			--��ǩ����ID
	[F_AppendSignOutPersonName] [varchar](50) NULL,	--��ǩ��������
	[F_AppendSignOutPersonNote] [varchar](500) NULL,--��ǩ��˵��
	[F_CISignInDate] [datetime] NULL,				--ǩ��ʱ��
	[F_CISignOutDate] [datetime] NULL,				--ǩ��ʱ��
	[F_CIRealityWorkDuration] [int] NULL,			--ʵ�ʹ���ʱ��
	[F_CIIsLate] [bit] NULL,						--�Ƿ�ٵ�
	[F_CIIsLeaveEarvly] [bit] NULL,					--�Ƿ�����
	[F_CIIsAbsenteeism] [bit] NULL,					--�Ƿ�ȱϯ
	[F_CIIsNormal] [bit] NULL,						--�Ƿ�����
	[F_CICreateDate] [date] NULL,					--����ʱ��
	[F_CIIsCalculate] [bit] NULL,					--�Ƿ��Ѿ���ͳ��
	[F_Date] [date] NULL,							--��ǩ����ͬ������
	[F_CIIsSignIn] [bit] NULL,						--�Ƿ���Ա���Լ�ǩ��
	[F_CIIsSignOut] [bit] NULL,						--�Ƿ�ΪԱ���Լ�ǩ��
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

/*****************************************ͳ����Ϣ************************************************/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[T_AttendanceStatistics](
	[F_ASID] [int] IDENTITY(1,1) NOT NULL,			--ͳ����ϢID
	[F_EmpID] [int] NOT NULL,						--��ͳ�Ƶ�Ա��ID
	[F_EmpName] [nvarchar](50) NOT NULL,			--��ͳ��Ա������
	[F_DepID] [int] NOT NULL,						--��ͳ��Ա�������Ĳ���ID
	[F_DepName] [nvarchar](50) NOT NULL,			--��ͳ��Ա����������ID
	[F_ASMonth] [int] NULL,							--ͳ�Ƶ��·ݣ��磺201512��ʾ2015��12�£�
	[F_ASStandardDuration] [int] NULL,				--���±�׼����ʱ��
	[F_ASRealityDuration] [int] NULL,				--����ʵ�ʹ���ʱ��
	[F_ASLateNumber] [int] NULL,					--�ٵ�����
	[F_ASLeaveEavlyNumber] [int] NULL,				--���˴���
	[F_ASAbsenteeismNumber] [int] NULL,				--ȱϯ����
	[F_ASNormalNumber] [int] NULL,					--��������
	[F_ASCreateDate] [datetime] NULL,				--����ʱ��
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

/*****************************************ʱ��ͳ��************************************************/
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

/*****************************************��ʼ����************************************************/
INSERT INTO T_WorkDuration(F_WDMonthDuration, F_WDSignInMonth, F_WDDayDurattion, F_NormalTime, F_AbsentSignInTime, F_AbsentSignOutTime, F_LeaveEarlyTime, F_WDCreateDate, F_WDFloatTime)
VALUES(22, '2015-11', 9, '09:00', '10:00', '17:00', '18:00', '2015-11-24', 30);
INSERT INTO T_Department(F_DepName) VALUES('�з���');
INSERT INTO T_EmployeeInfo(F_EmpName, F_EmpAccount, F_EmpPwd, F_EmpRole, F_DepID, F_DepName, F_EmpCreateDate) 
VALUES('admin', 'admin', 'e10adc3949ba59abbe56e057f20f883e', ',4,', 1, '�з���', '2015-11-24');