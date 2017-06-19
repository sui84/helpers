USE [Package]
GO
/****** Object:  Table [dbo].[ippkg]    Script Date: 6/19/2017 1:46:16 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ippkg](
	[FilePath] [nvarchar](200) NULL,
	[LineNumber] [nvarchar](200) NULL,
	[FrameNumber] [nvarchar](200) NULL,
	[BinaryDataStr] [nvarchar](max) NULL,
	[ProtocolsInFrame] [nvarchar](200) NULL,
	[Version] [nvarchar](200) NULL,
	[Source] [nvarchar](200) NULL,
	[SourceHost] [nvarchar](200) NULL,
	[Destination] [nvarchar](200) NULL,
	[DestinationHost] [nvarchar](200) NULL,
	[SourcePort] [nvarchar](200) NULL,
	[DestinationPort] [nvarchar](200) NULL,
	[SequenceNumber] [nvarchar](200) NULL,
	[NextSequenceNumber] [nvarchar](200) NULL,
	[AcknowledgmentNumber] [nvarchar](200) NULL,
	[ACKFrame] [nvarchar](max) NULL,
	[ArrivalTime] [nvarchar](200) NULL,
	[Ethernet] [nvarchar](max) NULL,
	[Type] [nvarchar](200) NULL,
	[SenderMACAddress] [nvarchar](200) NULL,
	[SenderIPAddress] [nvarchar](200) NULL,
	[TargetMACAddress] [nvarchar](200) NULL,
	[TargetIPAddress] [nvarchar](200) NULL,
	[HeaderLength] [nvarchar](200) NULL,
	[MSNetworkLoadBalancing] [nvarchar](max) NULL,
	[DifferentiatedServicesField] [nvarchar](200) NULL,
	[DifferentiatedServicesCodepoint] [nvarchar](200) NULL,
	[ExplicitCongestionNotification] [nvarchar](200) NULL,
	[TotalLength] [nvarchar](200) NULL,
	[Identification] [nvarchar](200) NULL,
	[Flags] [nvarchar](200) NULL,
	[Reservedbit] [nvarchar](200) NULL,
	[NotFragment] [nvarchar](200) NULL,
	[MoreFragments] [nvarchar](200) NULL,
	[FragmentOffset] [nvarchar](200) NULL,
	[TimeToLive] [nvarchar](200) NULL,
	[Protocol] [nvarchar](200) NULL,
	[HeaderChecksum] [nvarchar](200) NULL,
	[HeaderChecksumStatus] [nvarchar](200) NULL,
	[SourceGeoIP] [nvarchar](200) NULL,
	[DestinationGeoIP] [nvarchar](200) NULL,
	[StreamIndex] [nvarchar](200) NULL,
	[TCPSegmentLen] [nvarchar](200) NULL,
	[Timestamps] [nvarchar](max) NULL,
	[EncapsulationType] [nvarchar](200) NULL,
	[TimeShiftForThisPacket] [nvarchar](200) NULL,
	[EpochTime] [nvarchar](200) NULL,
	[TimeDeltaPrevCapFrame] [nvarchar](200) NULL,
	[TimeDeltaPrevDisFrame] [nvarchar](200) NULL,
	[TimeSinceReferOrFirstFrame] [nvarchar](200) NULL,
	[FrameLength] [nvarchar](200) NULL,
	[CaptureLength] [nvarchar](200) NULL,
	[FrameIsMarked] [nvarchar](200) NULL,
	[FrameIsIgnored] [nvarchar](200) NULL,
	[HeaderLength2] [nvarchar](200) NULL,
	[Flags2] [nvarchar](200) NULL,
	[Reserved] [nvarchar](200) NULL,
	[Nonce] [nvarchar](200) NULL,
	[CongestionWindowReduced] [nvarchar](200) NULL,
	[ECNEcho] [nvarchar](200) NULL,
	[Urgent] [nvarchar](200) NULL,
	[Acknowledgment] [nvarchar](200) NULL,
	[Push] [nvarchar](200) NULL,
	[Reset] [nvarchar](200) NULL,
	[Syn] [nvarchar](200) NULL,
	[Fin] [nvarchar](200) NULL,
	[TCPFlags] [nvarchar](200) NULL,
	[WindowSizeValue] [nvarchar](200) NULL,
	[Checksum] [nvarchar](200) NULL,
	[UrgentPointer] [nvarchar](200) NULL,
	[Options] [nvarchar](max) NULL,
	[SEQACKAnalysis] [nvarchar](max) NULL,
	[TabularDataStream] [nvarchar](max) NULL,
	[HypertextTransferProtocol] [nvarchar](max) NULL,
	[BinaryData] [nvarchar](max) NULL
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
/****** Object:  Table [dbo].[otherpkg]    Script Date: 6/19/2017 1:46:16 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[otherpkg](
	[FilePath] [nvarchar](200) NULL,
	[FrameNumber] [nvarchar](50) NULL,
	[Type] [nvarchar](50) NULL,
	[ColumnsOrMethod] [nvarchar](50) NULL,
	[Url] [nvarchar](1000) NULL,
	[Header] [nvarchar](1000) NULL,
	[RowCntOrRequest] [nvarchar](50) NULL,
	[Direction] [nvarchar](50) NULL,
	[Cookie] [nvarchar](max) NULL,
	[Data] [nvarchar](max) NULL
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
