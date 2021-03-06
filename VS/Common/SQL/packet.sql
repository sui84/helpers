USE [packet]
GO
/****** Object:  StoredProcedure [dbo].[ClearDB]    Script Date: 2017/7/9 21:26:06 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[ClearDB]
as 
begin 
	truncate table HttpPacket
	truncate table UdpPacket
	truncate table TcpPacket
	truncate table EthernetPacket
	truncate table ARPPacket
	truncate table IPPacket
	delete from  Packet
	DBCC CHECKIDENT ('Packet', RESEED, 0);
end 
GO
/****** Object:  Table [dbo].[ARPPacket]    Script Date: 2017/7/9 21:26:06 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ARPPacket](
	[PacketId] [bigint] NOT NULL,
	[HardwareAddressType] [nvarchar](50) NULL,
	[ProtocolAddressType] [nvarchar](50) NULL,
	[HardwareAddressLength] [int] NULL,
	[ProtocolAddressLength] [int] NULL,
	[Operation] [nvarchar](50) NULL,
	[SenderProtocolAddress] [nvarchar](50) NULL,
	[TargetProtocolAddress] [nvarchar](50) NULL,
	[SenderHardwareAddress] [nvarchar](50) NULL,
	[TargetHardwareAddress] [nvarchar](50) NULL,
	[Color] [nvarchar](50) NULL,
 CONSTRAINT [PK_ARPPacket] PRIMARY KEY CLUSTERED 
(
	[PacketId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[EthernetPacket]    Script Date: 2017/7/9 21:26:06 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[EthernetPacket](
	[PacketId] [bigint] NOT NULL,
	[DestinationHwAddress] [nvarchar](50) NULL,
	[SourceHwAddress] [nvarchar](50) NULL,
	[Type] [int] NULL,
 CONSTRAINT [PK_EthernetPackets] PRIMARY KEY CLUSTERED 
(
	[PacketId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[HttpPacket]    Script Date: 2017/7/9 21:26:06 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[HttpPacket](
	[PacketId] [bigint] NOT NULL,
	[Direction] [nvarchar](50) NOT NULL,
	[Method] [nvarchar](50) NULL,
	[URL] [nvarchar](500) NULL,
	[Version] [nvarchar](50) NULL,
	[UserAgent] [nvarchar](2000) NULL,
	[ProxyAuthorization] [nvarchar](50) NULL,
	[AcceptLanguage] [nvarchar](50) NULL,
	[Connection] [nvarchar](50) NULL,
	[Host] [nvarchar](50) NULL,
	[ContentEncoding] [nvarchar](50) NULL,
	[ContentLength] [int] NULL,
	[CacheControl] [nvarchar](50) NULL,
	[Cookie] [nvarchar](max) NULL,
	[Status] [nvarchar](50) NULL,
	[Server] [nvarchar](50) NULL,
	[Date] [nvarchar](50) NULL,
	[ContentType] [nvarchar](50) NULL,
	[LastModified] [nvarchar](50) NULL,
	[Body] [nvarchar](max) NULL,
 CONSTRAINT [PK_HttpPacket] PRIMARY KEY CLUSTERED 
(
	[PacketId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
/****** Object:  Table [dbo].[ICMPPacket]    Script Date: 2017/7/9 21:26:06 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[ICMPPacket](
	[PacketId] [bigint] NOT NULL,
	[TypeCode] [int] NULL,
	[Checksum] [int] NULL,
	[ID] [int] NULL,
	[Sequence] [int] NULL,
	[Data] [varbinary](max) NULL,
 CONSTRAINT [PK_ICMPPacket] PRIMARY KEY CLUSTERED 
(
	[PacketId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[Ieee80211RadioPacket]    Script Date: 2017/7/9 21:26:06 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Ieee80211RadioPacket](
	[PacketId] [bigint] NOT NULL,
	[Version] [int] NULL,
	[Length] [int] NULL,
	[Present] [int] NULL,
	[RadioTapFields] [nvarchar](max) NULL,
 CONSTRAINT [PK_Ieee80211RadioPackets] PRIMARY KEY CLUSTERED 
(
	[PacketId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
/****** Object:  Table [dbo].[IGMPPacket]    Script Date: 2017/7/9 21:26:06 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[IGMPPacket](
	[PacketId] [bigint] NOT NULL,
	[Type] [int] NULL,
	[MaxResponseTime] [int] NULL,
	[Checksum] [int] NULL,
	[GroupAddress] [nvarchar](50) NULL,
	[Color] [nvarchar](50) NULL,
 CONSTRAINT [PK_IGMPPackets] PRIMARY KEY CLUSTERED 
(
	[PacketId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[IPPacket]    Script Date: 2017/7/9 21:26:06 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[IPPacket](
	[PacketId] [bigint] NOT NULL,
	[TimeToLive] [int] NULL,
	[SourceAddress] [nvarchar](50) NULL,
	[DestinationAddress] [nvarchar](50) NULL,
	[HeaderLength] [int] NULL,
	[HopLimit] [int] NULL,
	[NextHeader] [nvarchar](50) NULL,
	[PayLoadLengh] [int] NULL,
	[Protocol] [nvarchar](50) NULL,
	[TotalLength] [int] NULL,
	[Version] [nvarchar](10) NULL,
	[Checksum] [int] NULL,
	[ValidChecksum] [bit] NULL,
	[Color] [nvarchar](50) NULL,
	[DifferentiatedServices] [int] NULL,
	[FragmentFlags] [int] NULL,
	[FragmentOffset] [int] NULL,
	[Id] [int] NULL,
 CONSTRAINT [PK_IPPackets] PRIMARY KEY CLUSTERED 
(
	[PacketId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[LinuxSLLType]    Script Date: 2017/7/9 21:26:06 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[LinuxSLLType](
	[PacketId] [bigint] NOT NULL,
	[Type] [int] NULL,
	[LinkLayerAddressType] [int] NULL,
	[LinkLayerAddressLength] [int] NULL,
	[LinkLayerAddress] [int] NULL,
	[EthernetProtocolType] [int] NULL,
 CONSTRAINT [PK_LinuxSLLTypes] PRIMARY KEY CLUSTERED 
(
	[PacketId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[Packet]    Script Date: 2017/7/9 21:26:06 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[Packet](
	[PacketId] [bigint] IDENTITY(1,1) NOT NULL,
	[FilePath] [nvarchar](200) NULL,
	[FrameNumber] [int] NULL,
	[Timeval] [datetime] NULL,
	[HeaderData] [nvarchar](max) NULL,
	[BodyData] [nvarchar](max) NULL,
	[BytesLength] [int] NULL,
	[PacketType] [nvarchar](50) NULL,
	[Bytes] [varbinary](max) NULL,
	[BytesOffset] [int] NULL,
	[NeedsCopy] [bit] NULL,
	[Color] [nvarchar](50) NULL,
	[Header] [varbinary](max) NULL,
	[HeaderLength] [int] NULL,
	[PacketType2] [nvarchar](50) NULL,
	[Bytes2] [varbinary](max) NULL,
	[BytesOffset2] [int] NULL,
	[NeedsCopy2] [bit] NULL,
	[Color2] [nvarchar](50) NULL,
	[Header2] [varbinary](max) NULL,
	[HeaderLength2] [int] NULL,
	[PacketType3] [nvarchar](50) NULL,
	[Bytes3] [varbinary](max) NULL,
	[BytesOffset3] [int] NULL,
	[NeedsCopy3] [bit] NULL,
	[Color3] [nvarchar](50) NULL,
	[Header3] [varbinary](max) NULL,
	[HeaderLength3] [int] NULL,
	[PacketType4] [nvarchar](50) NULL,
	[Bytes4] [varbinary](max) NULL,
	[BytesOffset4] [int] NULL,
	[NeedsCopy4] [bit] NULL,
	[Color4] [nvarchar](50) NULL,
	[Header4] [varbinary](max) NULL,
	[HeaderLength4] [int] NULL,
	[PacketType5] [nvarchar](50) NULL,
	[Bytes5] [varbinary](max) NULL,
	[BytesOffset5] [int] NULL,
	[NeedsCopy5] [bit] NULL,
	[Color5] [nvarchar](50) NULL,
	[Header5] [varbinary](max) NULL,
	[HeaderLength5] [int] NULL,
	[CreatedDate] [datetime] NULL,
 CONSTRAINT [PK_Packets] PRIMARY KEY CLUSTERED 
(
	[PacketId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[PPPoEPacket]    Script Date: 2017/7/9 21:26:06 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[PPPoEPacket](
	[PacketId] [bigint] NOT NULL,
	[VersionType] [int] NULL,
	[Version] [int] NULL,
	[Type] [int] NULL,
	[Code] [int] NULL,
	[SessionId] [int] NULL,
	[Length] [int] NULL,
 CONSTRAINT [PK_PPPoEPacket] PRIMARY KEY CLUSTERED 
(
	[PacketId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[PPPPacket]    Script Date: 2017/7/9 21:26:06 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[PPPPacket](
	[PacketId] [bigint] NOT NULL,
	[Protocol] [int] NULL,
	[Color] [nvarchar](50) NULL,
 CONSTRAINT [PK_PPPPacket] PRIMARY KEY CLUSTERED 
(
	[PacketId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[TcpPacket]    Script Date: 2017/7/9 21:26:06 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[TcpPacket](
	[PacketId] [bigint] NOT NULL,
	[SourcePort] [int] NULL,
	[DestinationPort] [int] NULL,
	[SequenceNumber] [bigint] NULL,
	[AcknowledgmentNumber] [bigint] NULL,
	[DataOffset] [int] NULL,
	[WindowSize] [int] NULL,
	[Checksum] [int] NULL,
	[ValidChecksum] [int] NULL,
	[ValidTCPChecksum] [int] NULL,
	[AllFlags] [int] NULL,
	[Urg] [bit] NULL,
	[Ack] [bit] NULL,
	[Psh] [bit] NULL,
	[Rst] [bit] NULL,
	[Syn] [bit] NULL,
	[Fin] [bit] NULL,
	[ECN] [bit] NULL,
	[CWR] [bit] NULL,
	[Color] [nvarchar](50) NULL,
	[UrgentPointer] [int] NULL,
	[Options] [varbinary](max) NULL,
 CONSTRAINT [PK_TcpPackets] PRIMARY KEY CLUSTERED 
(
	[PacketId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[UdpPacket]    Script Date: 2017/7/9 21:26:06 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[UdpPacket](
	[PacketId] [bigint] NOT NULL,
	[SourcePort] [int] NULL,
	[DestinationPort] [int] NULL,
	[Length] [int] NULL,
	[Checksum] [int] NULL,
	[ValidChecksum] [bit] NULL,
	[ValidUDPChecksum] [bit] NULL,
	[Color] [nvarchar](50) NULL,
 CONSTRAINT [PK_UdpPackets] PRIMARY KEY CLUSTERED 
(
	[PacketId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
ALTER TABLE [dbo].[Packet] ADD  CONSTRAINT [DF_Packet_CreatedDate]  DEFAULT (getdate()) FOR [CreatedDate]
GO
ALTER TABLE [dbo].[ARPPacket]  WITH CHECK ADD  CONSTRAINT [FK_ARPPacket_Packet] FOREIGN KEY([PacketId])
REFERENCES [dbo].[Packet] ([PacketId])
GO
ALTER TABLE [dbo].[ARPPacket] CHECK CONSTRAINT [FK_ARPPacket_Packet]
GO
ALTER TABLE [dbo].[EthernetPacket]  WITH CHECK ADD  CONSTRAINT [FK_EthernetPackets_Packets] FOREIGN KEY([PacketId])
REFERENCES [dbo].[Packet] ([PacketId])
GO
ALTER TABLE [dbo].[EthernetPacket] CHECK CONSTRAINT [FK_EthernetPackets_Packets]
GO
ALTER TABLE [dbo].[HttpPacket]  WITH CHECK ADD  CONSTRAINT [FK_HttpPacket_Packet] FOREIGN KEY([PacketId])
REFERENCES [dbo].[Packet] ([PacketId])
GO
ALTER TABLE [dbo].[HttpPacket] CHECK CONSTRAINT [FK_HttpPacket_Packet]
GO
ALTER TABLE [dbo].[ICMPPacket]  WITH CHECK ADD  CONSTRAINT [FK_ICMPPacket_Packet] FOREIGN KEY([PacketId])
REFERENCES [dbo].[Packet] ([PacketId])
GO
ALTER TABLE [dbo].[ICMPPacket] CHECK CONSTRAINT [FK_ICMPPacket_Packet]
GO
ALTER TABLE [dbo].[Ieee80211RadioPacket]  WITH CHECK ADD  CONSTRAINT [FK_Ieee80211RadioPacket_Packet] FOREIGN KEY([PacketId])
REFERENCES [dbo].[Packet] ([PacketId])
GO
ALTER TABLE [dbo].[Ieee80211RadioPacket] CHECK CONSTRAINT [FK_Ieee80211RadioPacket_Packet]
GO
ALTER TABLE [dbo].[IGMPPacket]  WITH CHECK ADD  CONSTRAINT [FK_IGMPPacket_Packet] FOREIGN KEY([PacketId])
REFERENCES [dbo].[Packet] ([PacketId])
GO
ALTER TABLE [dbo].[IGMPPacket] CHECK CONSTRAINT [FK_IGMPPacket_Packet]
GO
ALTER TABLE [dbo].[IPPacket]  WITH CHECK ADD  CONSTRAINT [FK_IPPackets_Packets] FOREIGN KEY([PacketId])
REFERENCES [dbo].[Packet] ([PacketId])
GO
ALTER TABLE [dbo].[IPPacket] CHECK CONSTRAINT [FK_IPPackets_Packets]
GO
ALTER TABLE [dbo].[LinuxSLLType]  WITH CHECK ADD  CONSTRAINT [FK_LinuxSLLType_Packet] FOREIGN KEY([PacketId])
REFERENCES [dbo].[Packet] ([PacketId])
GO
ALTER TABLE [dbo].[LinuxSLLType] CHECK CONSTRAINT [FK_LinuxSLLType_Packet]
GO
ALTER TABLE [dbo].[PPPoEPacket]  WITH CHECK ADD  CONSTRAINT [FK_PPPoEPacket_Packet] FOREIGN KEY([PacketId])
REFERENCES [dbo].[Packet] ([PacketId])
GO
ALTER TABLE [dbo].[PPPoEPacket] CHECK CONSTRAINT [FK_PPPoEPacket_Packet]
GO
ALTER TABLE [dbo].[PPPPacket]  WITH CHECK ADD  CONSTRAINT [FK_PPPPacket_Packet] FOREIGN KEY([PacketId])
REFERENCES [dbo].[Packet] ([PacketId])
GO
ALTER TABLE [dbo].[PPPPacket] CHECK CONSTRAINT [FK_PPPPacket_Packet]
GO
ALTER TABLE [dbo].[TcpPacket]  WITH CHECK ADD  CONSTRAINT [FK_TcpPackets_Packets] FOREIGN KEY([PacketId])
REFERENCES [dbo].[Packet] ([PacketId])
GO
ALTER TABLE [dbo].[TcpPacket] CHECK CONSTRAINT [FK_TcpPackets_Packets]
GO
ALTER TABLE [dbo].[UdpPacket]  WITH CHECK ADD  CONSTRAINT [FK_UdpPackets_Packets] FOREIGN KEY([PacketId])
REFERENCES [dbo].[Packet] ([PacketId])
GO
ALTER TABLE [dbo].[UdpPacket] CHECK CONSTRAINT [FK_UdpPackets_Packets]
GO
