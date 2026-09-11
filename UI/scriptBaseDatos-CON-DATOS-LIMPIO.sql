USE [master]
GO
/****** Object:  Database [GestionUsuarios]    Script Date: 7/7/2026 11:12:24 PM ******/
CREATE DATABASE [GestionUsuarios]
GO
IF (1 = FULLTEXTSERVICEPROPERTY('IsFullTextInstalled'))
begin
EXEC [GestionUsuarios].[dbo].[sp_fulltext_database] @action = 'enable'
end
GO
ALTER DATABASE [GestionUsuarios] SET ANSI_NULL_DEFAULT OFF 
GO
ALTER DATABASE [GestionUsuarios] SET ANSI_NULLS OFF 
GO
ALTER DATABASE [GestionUsuarios] SET ANSI_PADDING OFF 
GO
ALTER DATABASE [GestionUsuarios] SET ANSI_WARNINGS OFF 
GO
ALTER DATABASE [GestionUsuarios] SET ARITHABORT OFF 
GO
ALTER DATABASE [GestionUsuarios] SET AUTO_CLOSE ON 
GO
ALTER DATABASE [GestionUsuarios] SET AUTO_SHRINK OFF 
GO
ALTER DATABASE [GestionUsuarios] SET AUTO_UPDATE_STATISTICS ON 
GO
ALTER DATABASE [GestionUsuarios] SET CURSOR_CLOSE_ON_COMMIT OFF 
GO
ALTER DATABASE [GestionUsuarios] SET CURSOR_DEFAULT  GLOBAL 
GO
ALTER DATABASE [GestionUsuarios] SET CONCAT_NULL_YIELDS_NULL OFF 
GO
ALTER DATABASE [GestionUsuarios] SET NUMERIC_ROUNDABORT OFF 
GO
ALTER DATABASE [GestionUsuarios] SET QUOTED_IDENTIFIER OFF 
GO
ALTER DATABASE [GestionUsuarios] SET RECURSIVE_TRIGGERS OFF 
GO
ALTER DATABASE [GestionUsuarios] SET  DISABLE_BROKER 
GO
ALTER DATABASE [GestionUsuarios] SET AUTO_UPDATE_STATISTICS_ASYNC OFF 
GO
ALTER DATABASE [GestionUsuarios] SET DATE_CORRELATION_OPTIMIZATION OFF 
GO
ALTER DATABASE [GestionUsuarios] SET TRUSTWORTHY OFF 
GO
ALTER DATABASE [GestionUsuarios] SET ALLOW_SNAPSHOT_ISOLATION OFF 
GO
ALTER DATABASE [GestionUsuarios] SET PARAMETERIZATION SIMPLE 
GO
ALTER DATABASE [GestionUsuarios] SET READ_COMMITTED_SNAPSHOT OFF 
GO
ALTER DATABASE [GestionUsuarios] SET HONOR_BROKER_PRIORITY OFF 
GO
ALTER DATABASE [GestionUsuarios] SET RECOVERY SIMPLE 
GO
ALTER DATABASE [GestionUsuarios] SET  MULTI_USER 
GO
ALTER DATABASE [GestionUsuarios] SET PAGE_VERIFY CHECKSUM  
GO
ALTER DATABASE [GestionUsuarios] SET DB_CHAINING OFF 
GO
ALTER DATABASE [GestionUsuarios] SET FILESTREAM( NON_TRANSACTED_ACCESS = OFF ) 
GO
ALTER DATABASE [GestionUsuarios] SET TARGET_RECOVERY_TIME = 60 SECONDS 
GO
ALTER DATABASE [GestionUsuarios] SET DELAYED_DURABILITY = DISABLED 
GO
ALTER DATABASE [GestionUsuarios] SET ACCELERATED_DATABASE_RECOVERY = OFF  
GO
EXEC sys.sp_db_vardecimal_storage_format N'GestionUsuarios', N'ON'
GO
ALTER DATABASE [GestionUsuarios] SET QUERY_STORE = ON
GO
ALTER DATABASE [GestionUsuarios] SET QUERY_STORE (OPERATION_MODE = READ_WRITE, CLEANUP_POLICY = (STALE_QUERY_THRESHOLD_DAYS = 30), DATA_FLUSH_INTERVAL_SECONDS = 900, INTERVAL_LENGTH_MINUTES = 60, MAX_STORAGE_SIZE_MB = 1000, QUERY_CAPTURE_MODE = AUTO, SIZE_BASED_CLEANUP_MODE = AUTO, MAX_PLANS_PER_QUERY = 200, WAIT_STATS_CAPTURE_MODE = ON)
GO
USE [GestionUsuarios]
GO
/****** Object:  Table [dbo].[BitacoraEventos]    Script Date: 7/7/2026 11:12:24 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[BitacoraEventos](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Username] [nvarchar](50) NOT NULL,
	[Fecha] [datetime] NOT NULL,
	[Modulo] [nvarchar](50) NOT NULL,
	[Descripcion] [nvarchar](max) NOT NULL,
	[Criticidad] [int] NOT NULL,
 CONSTRAINT [PK_BitacoraEventos] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[DigitoVerificador_83KI]    Script Date: 7/7/2026 11:12:24 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[DigitoVerificador_83KI](
	[NombreTabla] [varchar](128) NOT NULL,
	[DVV] [varchar](64) NOT NULL,
	[FechaActualizacion] [datetime] NOT NULL,
 CONSTRAINT [PK_DigitoVerificador_83KI] PRIMARY KEY CLUSTERED 
(
	[NombreTabla] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[FamiliaFamilia]    Script Date: 7/7/2026 11:12:25 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[FamiliaFamilia](
	[CodigoFamiliaPadre] [int] NOT NULL,
	[CodigoFamiliaHija] [int] NOT NULL,
	[DVH] [varchar](64) NOT NULL,
 CONSTRAINT [PK_FamiliaFamilia] PRIMARY KEY CLUSTERED 
(
	[CodigoFamiliaPadre] ASC,
	[CodigoFamiliaHija] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[FamiliaPatente]    Script Date: 7/7/2026 11:12:25 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[FamiliaPatente](
	[CodigoFamilia] [int] NOT NULL,
	[CodigoPatente] [int] NOT NULL,
	[DVH] [varchar](64) NOT NULL,
 CONSTRAINT [PK_FamiliaPatente] PRIMARY KEY CLUSTERED 
(
	[CodigoFamilia] ASC,
	[CodigoPatente] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Familias]    Script Date: 7/7/2026 11:12:25 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Familias](
	[CodigoFamilia] [int] IDENTITY(1,1) NOT NULL,
	[Nombre] [nvarchar](100) NOT NULL,
	[DVH] [varchar](64) NOT NULL,
 CONSTRAINT [PK_Familias] PRIMARY KEY CLUSTERED 
(
	[CodigoFamilia] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Patentes]    Script Date: 7/7/2026 11:12:25 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Patentes](
	[CodigoPatente] [int] IDENTITY(1,1) NOT NULL,
	[Nombre] [nvarchar](100) NOT NULL,
	[DVH] [varchar](64) NOT NULL,
 CONSTRAINT [PK_Patentes] PRIMARY KEY CLUSTERED 
(
	[CodigoPatente] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Roles]    Script Date: 7/7/2026 11:12:25 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Roles](
	[CodigoRol] [int] IDENTITY(1,1) NOT NULL,
	[Nombre] [nvarchar](50) NOT NULL,
	[DVH] [varchar](64) NOT NULL,
 CONSTRAINT [PK__Roles__F0D13057AB23237F] PRIMARY KEY CLUSTERED 
(
	[CodigoRol] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[RolFamilia]    Script Date: 7/7/2026 11:12:25 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[RolFamilia](
	[CodigoRol] [int] NOT NULL,
	[CodigoFamilia] [int] NOT NULL,
	[DVH] [varchar](64) NOT NULL,
 CONSTRAINT [PK_RolFamilia] PRIMARY KEY CLUSTERED 
(
	[CodigoRol] ASC,
	[CodigoFamilia] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[RolPatente]    Script Date: 7/7/2026 11:12:25 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[RolPatente](
	[CodigoRol] [int] NOT NULL,
	[CodigoPatente] [int] NOT NULL,
	[DVH] [varchar](64) NOT NULL,
 CONSTRAINT [PK_RolPatente] PRIMARY KEY CLUSTERED 
(
	[CodigoRol] ASC,
	[CodigoPatente] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Usuarios]    Script Date: 7/7/2026 11:12:25 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Usuarios](
	[DNI] [varchar](50) NOT NULL,
	[Nombre] [nvarchar](80) NOT NULL,
	[Apellido] [nvarchar](80) NOT NULL,
	[Email] [nchar](200) NOT NULL,
	[Bloqueado] [bit] NOT NULL,
	[Contrasena] [nvarchar](max) NOT NULL,
	[Username] [nvarchar](50) NOT NULL,
	[Activo] [bit] NOT NULL,
	[CodigoRol] [int] NOT NULL,
	[IntentosRealizados] [int] NOT NULL,
	[FechaUltimoIntento] [datetime] NULL,
	[IdiomaId] [nvarchar](10) NOT NULL,
	[DVH] [varchar](64) NOT NULL,
 CONSTRAINT [PK_Usuarios] PRIMARY KEY CLUSTERED 
(
	[DNI] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
SET IDENTITY_INSERT [dbo].[BitacoraEventos] ON 

INSERT [dbo].[BitacoraEventos] ([Id], [Username], [Fecha], [Modulo], [Descripcion], [Criticidad]) VALUES (2, N'46948668Maximo', CAST(N'2026-04-26T04:50:00.507' AS DateTime), N'Usuarios', N'Login exitoso: 46948668Maximo', 3)
INSERT [dbo].[BitacoraEventos] ([Id], [Username], [Fecha], [Modulo], [Descripcion], [Criticidad]) VALUES (4, N'46948668Maximo', CAST(N'2026-04-26T04:56:59.467' AS DateTime), N'Usuarios', N'Login exitoso: 46948668Maximo', 3)
INSERT [dbo].[BitacoraEventos] ([Id], [Username], [Fecha], [Modulo], [Descripcion], [Criticidad]) VALUES (5, N'46948668Maximo', CAST(N'2026-04-26T05:02:44.997' AS DateTime), N'Usuarios', N'Login exitoso: 46948668Maximo', 3)
INSERT [dbo].[BitacoraEventos] ([Id], [Username], [Fecha], [Modulo], [Descripcion], [Criticidad]) VALUES (6, N'46948668Maximo', CAST(N'2026-04-26T05:03:28.797' AS DateTime), N'Usuarios', N'Login exitoso: 46948668Maximo', 3)
INSERT [dbo].[BitacoraEventos] ([Id], [Username], [Fecha], [Modulo], [Descripcion], [Criticidad]) VALUES (7, N'46905013Lucas', CAST(N'2026-04-26T05:05:14.367' AS DateTime), N'Usuarios', N'Usuario bloqueado: 46905013Lucas', 1)
INSERT [dbo].[BitacoraEventos] ([Id], [Username], [Fecha], [Modulo], [Descripcion], [Criticidad]) VALUES (8, N'46948668Maximo', CAST(N'2026-04-26T05:07:57.643' AS DateTime), N'Usuarios', N'Login exitoso: 46948668Maximo', 3)
INSERT [dbo].[BitacoraEventos] ([Id], [Username], [Fecha], [Modulo], [Descripcion], [Criticidad]) VALUES (9, N'46948668Maximo', CAST(N'2026-04-26T05:08:30.957' AS DateTime), N'Usuarios', N'Login exitoso: 46948668Maximo', 3)
INSERT [dbo].[BitacoraEventos] ([Id], [Username], [Fecha], [Modulo], [Descripcion], [Criticidad]) VALUES (10, N'46948668Maximo', CAST(N'2026-04-26T05:09:43.570' AS DateTime), N'Usuarios', N'Login exitoso: 46948668Maximo', 3)
INSERT [dbo].[BitacoraEventos] ([Id], [Username], [Fecha], [Modulo], [Descripcion], [Criticidad]) VALUES (11, N'46948668Maximo', CAST(N'2026-04-26T05:11:14.707' AS DateTime), N'Usuarios', N'Login exitoso: 46948668Maximo', 3)
INSERT [dbo].[BitacoraEventos] ([Id], [Username], [Fecha], [Modulo], [Descripcion], [Criticidad]) VALUES (12, N'46948668Maximo', CAST(N'2026-04-26T05:19:39.323' AS DateTime), N'Usuarios', N'Login exitoso: 46948668Maximo', 3)
INSERT [dbo].[BitacoraEventos] ([Id], [Username], [Fecha], [Modulo], [Descripcion], [Criticidad]) VALUES (13, N'23456789as', CAST(N'2026-04-26T05:19:53.123' AS DateTime), N'Usuarios', N'Usuario desbloqueado: 23456789as (Rol: Admin)', 1)
INSERT [dbo].[BitacoraEventos] ([Id], [Username], [Fecha], [Modulo], [Descripcion], [Criticidad]) VALUES (14, N'23456789as', CAST(N'2026-04-26T05:19:56.800' AS DateTime), N'Usuarios', N'Usuario desbloqueado: 23456789as (Rol: Admin)', 1)
INSERT [dbo].[BitacoraEventos] ([Id], [Username], [Fecha], [Modulo], [Descripcion], [Criticidad]) VALUES (15, N'23456789as', CAST(N'2026-04-26T05:20:17.877' AS DateTime), N'Usuarios', N'Usuario desbloqueado: 23456789as (Rol: Admin)', 1)
INSERT [dbo].[BitacoraEventos] ([Id], [Username], [Fecha], [Modulo], [Descripcion], [Criticidad]) VALUES (16, N'23456789as', CAST(N'2026-04-26T05:20:22.193' AS DateTime), N'Usuarios', N'Usuario desbloqueado: 23456789as (Rol: Admin)', 1)
INSERT [dbo].[BitacoraEventos] ([Id], [Username], [Fecha], [Modulo], [Descripcion], [Criticidad]) VALUES (17, N'46948668Maximo', CAST(N'2026-04-26T05:23:40.357' AS DateTime), N'Usuarios', N'Login exitoso: 46948668Maximo', 3)
INSERT [dbo].[BitacoraEventos] ([Id], [Username], [Fecha], [Modulo], [Descripcion], [Criticidad]) VALUES (18, N'46905013Lucas', CAST(N'2026-04-26T05:23:43.210' AS DateTime), N'Usuarios', N'Usuario desbloqueado: 46905013Lucas (Rol: Admin)', 1)
INSERT [dbo].[BitacoraEventos] ([Id], [Username], [Fecha], [Modulo], [Descripcion], [Criticidad]) VALUES (19, N'46905013Lucas', CAST(N'2026-04-26T05:25:54.773' AS DateTime), N'Usuarios', N'Login exitoso: 46905013Lucas', 3)
INSERT [dbo].[BitacoraEventos] ([Id], [Username], [Fecha], [Modulo], [Descripcion], [Criticidad]) VALUES (20, N'46948668Maximo', CAST(N'2026-04-27T00:47:27.250' AS DateTime), N'Usuarios', N'Login exitoso: 46948668Maximo', 3)
INSERT [dbo].[BitacoraEventos] ([Id], [Username], [Fecha], [Modulo], [Descripcion], [Criticidad]) VALUES (21, N'46948668Maximo', CAST(N'2026-04-27T00:47:59.690' AS DateTime), N'Usuarios', N'Contraseña modificada: 46948668Maximo', 2)
INSERT [dbo].[BitacoraEventos] ([Id], [Username], [Fecha], [Modulo], [Descripcion], [Criticidad]) VALUES (22, N'46948668Maximo', CAST(N'2026-04-27T00:48:25.420' AS DateTime), N'Usuarios', N'Logout exitoso: 46948668Maximo', 3)
INSERT [dbo].[BitacoraEventos] ([Id], [Username], [Fecha], [Modulo], [Descripcion], [Criticidad]) VALUES (23, N'46948668Maximo', CAST(N'2026-04-27T00:48:33.207' AS DateTime), N'Usuarios', N'Login exitoso: 46948668Maximo', 3)
INSERT [dbo].[BitacoraEventos] ([Id], [Username], [Fecha], [Modulo], [Descripcion], [Criticidad]) VALUES (24, N'46948668Maximo', CAST(N'2026-04-27T00:48:41.963' AS DateTime), N'Usuarios', N'Logout exitoso: 46948668Maximo', 3)
INSERT [dbo].[BitacoraEventos] ([Id], [Username], [Fecha], [Modulo], [Descripcion], [Criticidad]) VALUES (25, N'46948668Maximo', CAST(N'2026-04-27T02:40:37.517' AS DateTime), N'Usuarios', N'Login exitoso: 46948668Maximo', 3)
INSERT [dbo].[BitacoraEventos] ([Id], [Username], [Fecha], [Modulo], [Descripcion], [Criticidad]) VALUES (26, N'46948668Maximo', CAST(N'2026-04-27T02:40:47.463' AS DateTime), N'Usuarios', N'Logout exitoso: 46948668Maximo', 3)
INSERT [dbo].[BitacoraEventos] ([Id], [Username], [Fecha], [Modulo], [Descripcion], [Criticidad]) VALUES (27, N'46948668Maximo', CAST(N'2026-04-27T02:41:42.557' AS DateTime), N'Usuarios', N'Login exitoso: 46948668Maximo', 3)
INSERT [dbo].[BitacoraEventos] ([Id], [Username], [Fecha], [Modulo], [Descripcion], [Criticidad]) VALUES (28, N'46948668Maximo', CAST(N'2026-04-27T02:45:41.037' AS DateTime), N'Usuarios', N'Login exitoso: 46948668Maximo', 3)
INSERT [dbo].[BitacoraEventos] ([Id], [Username], [Fecha], [Modulo], [Descripcion], [Criticidad]) VALUES (29, N'46948668Maximo', CAST(N'2026-04-27T02:46:23.847' AS DateTime), N'Usuarios', N'Usuario modificado: DNI 23456789. Email: houstonrockets@gmail.com. Rol: RolSimple. Actor: 46948668Maximo', 2)
INSERT [dbo].[BitacoraEventos] ([Id], [Username], [Fecha], [Modulo], [Descripcion], [Criticidad]) VALUES (30, N'46948668Maximo', CAST(N'2026-04-29T03:16:56.817' AS DateTime), N'Usuarios', N'Login exitoso: 46948668Maximo', 3)
INSERT [dbo].[BitacoraEventos] ([Id], [Username], [Fecha], [Modulo], [Descripcion], [Criticidad]) VALUES (31, N'46948668Maximo', CAST(N'2026-04-29T03:17:11.903' AS DateTime), N'Usuarios', N'Usuario deshabilitado: DNI 23456789. Actor: 46948668Maximo', 2)
INSERT [dbo].[BitacoraEventos] ([Id], [Username], [Fecha], [Modulo], [Descripcion], [Criticidad]) VALUES (32, N'46948668Maximo', CAST(N'2026-04-29T03:17:16.877' AS DateTime), N'Usuarios', N'Usuario habilitado: DNI 23456789. Actor: 46948668Maximo', 2)
INSERT [dbo].[BitacoraEventos] ([Id], [Username], [Fecha], [Modulo], [Descripcion], [Criticidad]) VALUES (33, N'46948668Maximo', CAST(N'2026-04-29T03:17:22.523' AS DateTime), N'Usuarios', N'Usuario deshabilitado: DNI 23456789. Actor: 46948668Maximo', 2)
INSERT [dbo].[BitacoraEventos] ([Id], [Username], [Fecha], [Modulo], [Descripcion], [Criticidad]) VALUES (34, N'46948668Maximo', CAST(N'2026-04-29T03:17:31.713' AS DateTime), N'Usuarios', N'Logout exitoso: 46948668Maximo', 3)
INSERT [dbo].[BitacoraEventos] ([Id], [Username], [Fecha], [Modulo], [Descripcion], [Criticidad]) VALUES (35, N'46948668Maximo', CAST(N'2026-04-29T03:17:38.437' AS DateTime), N'Usuarios', N'Login exitoso: 46948668Maximo', 3)
INSERT [dbo].[BitacoraEventos] ([Id], [Username], [Fecha], [Modulo], [Descripcion], [Criticidad]) VALUES (36, N'46948668Maximo', CAST(N'2026-04-29T03:17:47.417' AS DateTime), N'Usuarios', N'Logout exitoso: 46948668Maximo', 3)
INSERT [dbo].[BitacoraEventos] ([Id], [Username], [Fecha], [Modulo], [Descripcion], [Criticidad]) VALUES (37, N'46948668Maximo', CAST(N'2026-04-29T03:40:22.270' AS DateTime), N'Usuarios', N'Login exitoso: 46948668Maximo', 3)
INSERT [dbo].[BitacoraEventos] ([Id], [Username], [Fecha], [Modulo], [Descripcion], [Criticidad]) VALUES (38, N'46948668Maximo', CAST(N'2026-04-29T03:40:29.503' AS DateTime), N'Usuarios', N'Usuario habilitado: DNI 23456789. Actor: 46948668Maximo', 2)
INSERT [dbo].[BitacoraEventos] ([Id], [Username], [Fecha], [Modulo], [Descripcion], [Criticidad]) VALUES (39, N'46948668Maximo', CAST(N'2026-04-29T03:40:33.810' AS DateTime), N'Usuarios', N'Usuario deshabilitado: DNI 23456789. Actor: 46948668Maximo', 2)
INSERT [dbo].[BitacoraEventos] ([Id], [Username], [Fecha], [Modulo], [Descripcion], [Criticidad]) VALUES (40, N'46948668Maximo', CAST(N'2026-04-29T03:40:39.517' AS DateTime), N'Usuarios', N'Logout exitoso: 46948668Maximo', 3)
INSERT [dbo].[BitacoraEventos] ([Id], [Username], [Fecha], [Modulo], [Descripcion], [Criticidad]) VALUES (41, N'46948668Maximo', CAST(N'2026-04-29T03:47:54.163' AS DateTime), N'Usuarios', N'Login exitoso: 46948668Maximo', 3)
INSERT [dbo].[BitacoraEventos] ([Id], [Username], [Fecha], [Modulo], [Descripcion], [Criticidad]) VALUES (42, N'46948668Maximo', CAST(N'2026-04-29T04:02:32.567' AS DateTime), N'Usuarios', N'Login exitoso: 46948668Maximo', 3)
INSERT [dbo].[BitacoraEventos] ([Id], [Username], [Fecha], [Modulo], [Descripcion], [Criticidad]) VALUES (43, N'46948668Maximo', CAST(N'2026-04-29T04:02:37.753' AS DateTime), N'Usuarios', N'Usuario habilitado: DNI 23456789. Actor: 46948668Maximo', 2)
INSERT [dbo].[BitacoraEventos] ([Id], [Username], [Fecha], [Modulo], [Descripcion], [Criticidad]) VALUES (44, N'46948668Maximo', CAST(N'2026-04-29T04:02:39.937' AS DateTime), N'Usuarios', N'Usuario deshabilitado: DNI 23456789. Actor: 46948668Maximo', 2)
INSERT [dbo].[BitacoraEventos] ([Id], [Username], [Fecha], [Modulo], [Descripcion], [Criticidad]) VALUES (45, N'46948668Maximo', CAST(N'2026-04-29T04:02:51.127' AS DateTime), N'Usuarios', N'Logout exitoso: 46948668Maximo', 3)
INSERT [dbo].[BitacoraEventos] ([Id], [Username], [Fecha], [Modulo], [Descripcion], [Criticidad]) VALUES (46, N'46948668Maximo', CAST(N'2026-04-29T04:12:36.127' AS DateTime), N'Usuarios', N'Login exitoso: 46948668Maximo', 3)
INSERT [dbo].[BitacoraEventos] ([Id], [Username], [Fecha], [Modulo], [Descripcion], [Criticidad]) VALUES (47, N'46948668Maximo', CAST(N'2026-04-29T04:13:16.183' AS DateTime), N'Usuarios', N'Logout exitoso: 46948668Maximo', 3)
INSERT [dbo].[BitacoraEventos] ([Id], [Username], [Fecha], [Modulo], [Descripcion], [Criticidad]) VALUES (48, N'46905013Lucas', CAST(N'2026-04-29T04:13:33.307' AS DateTime), N'Usuarios', N'Login exitoso: 46905013Lucas', 3)
INSERT [dbo].[BitacoraEventos] ([Id], [Username], [Fecha], [Modulo], [Descripcion], [Criticidad]) VALUES (49, N'46905013Lucas', CAST(N'2026-04-29T04:13:41.757' AS DateTime), N'Usuarios', N'Logout exitoso: 46905013Lucas', 3)
INSERT [dbo].[BitacoraEventos] ([Id], [Username], [Fecha], [Modulo], [Descripcion], [Criticidad]) VALUES (50, N'46905013Lucas', CAST(N'2026-04-29T04:13:49.277' AS DateTime), N'Usuarios', N'Usuario bloqueado: 46905013Lucas', 1)
INSERT [dbo].[BitacoraEventos] ([Id], [Username], [Fecha], [Modulo], [Descripcion], [Criticidad]) VALUES (51, N'46948668Maximo', CAST(N'2026-04-29T04:14:01.817' AS DateTime), N'Usuarios', N'Login exitoso: 46948668Maximo', 3)
INSERT [dbo].[BitacoraEventos] ([Id], [Username], [Fecha], [Modulo], [Descripcion], [Criticidad]) VALUES (52, N'46905013Lucas', CAST(N'2026-04-29T04:14:31.660' AS DateTime), N'Usuarios', N'Usuario desbloqueado: 46905013Lucas (Rol: Admin)', 1)
INSERT [dbo].[BitacoraEventos] ([Id], [Username], [Fecha], [Modulo], [Descripcion], [Criticidad]) VALUES (53, N'46948668Maximo', CAST(N'2026-04-29T04:14:37.470' AS DateTime), N'Usuarios', N'Logout exitoso: 46948668Maximo', 3)
INSERT [dbo].[BitacoraEventos] ([Id], [Username], [Fecha], [Modulo], [Descripcion], [Criticidad]) VALUES (54, N'46948668Maximo', CAST(N'2026-04-29T04:17:42.167' AS DateTime), N'Usuarios', N'Login exitoso: 46948668Maximo', 3)
INSERT [dbo].[BitacoraEventos] ([Id], [Username], [Fecha], [Modulo], [Descripcion], [Criticidad]) VALUES (55, N'46948668Maximo', CAST(N'2026-04-29T04:18:00.653' AS DateTime), N'Usuarios', N'Logout exitoso: 46948668Maximo', 3)
INSERT [dbo].[BitacoraEventos] ([Id], [Username], [Fecha], [Modulo], [Descripcion], [Criticidad]) VALUES (56, N'46948668Maximo', CAST(N'2026-04-29T18:07:28.567' AS DateTime), N'Usuarios', N'Login exitoso: 46948668Maximo', 3)
INSERT [dbo].[BitacoraEventos] ([Id], [Username], [Fecha], [Modulo], [Descripcion], [Criticidad]) VALUES (57, N'46948668Maximo', CAST(N'2026-04-29T18:07:51.943' AS DateTime), N'Usuarios', N'Usuario habilitado: DNI 23456789. Actor: 46948668Maximo', 2)
INSERT [dbo].[BitacoraEventos] ([Id], [Username], [Fecha], [Modulo], [Descripcion], [Criticidad]) VALUES (58, N'46948668Maximo', CAST(N'2026-04-29T18:07:56.593' AS DateTime), N'Usuarios', N'Usuario deshabilitado: DNI 23456789. Actor: 46948668Maximo', 2)
INSERT [dbo].[BitacoraEventos] ([Id], [Username], [Fecha], [Modulo], [Descripcion], [Criticidad]) VALUES (59, N'46948668Maximo', CAST(N'2026-04-29T18:08:08.930' AS DateTime), N'Usuarios', N'Logout exitoso: 46948668Maximo', 3)
INSERT [dbo].[BitacoraEventos] ([Id], [Username], [Fecha], [Modulo], [Descripcion], [Criticidad]) VALUES (60, N'46948668Maximo', CAST(N'2026-04-29T18:49:32.660' AS DateTime), N'Usuarios', N'Login exitoso: 46948668Maximo', 3)
INSERT [dbo].[BitacoraEventos] ([Id], [Username], [Fecha], [Modulo], [Descripcion], [Criticidad]) VALUES (61, N'46948668Maximo', CAST(N'2026-04-29T18:49:40.060' AS DateTime), N'Usuarios', N'Usuario habilitado: DNI 23456789. Actor: 46948668Maximo', 2)
INSERT [dbo].[BitacoraEventos] ([Id], [Username], [Fecha], [Modulo], [Descripcion], [Criticidad]) VALUES (62, N'46948668Maximo', CAST(N'2026-04-29T18:49:43.650' AS DateTime), N'Usuarios', N'Usuario deshabilitado: DNI 23456789. Actor: 46948668Maximo', 2)
INSERT [dbo].[BitacoraEventos] ([Id], [Username], [Fecha], [Modulo], [Descripcion], [Criticidad]) VALUES (63, N'46948668Maximo', CAST(N'2026-04-29T18:49:57.733' AS DateTime), N'Usuarios', N'Usuario modificado: DNI 23456789. Email: houstonrockets@gmail.com. Rol: Admin. Actor: 46948668Maximo', 2)
INSERT [dbo].[BitacoraEventos] ([Id], [Username], [Fecha], [Modulo], [Descripcion], [Criticidad]) VALUES (64, N'46948668Maximo', CAST(N'2026-04-29T18:50:07.237' AS DateTime), N'Usuarios', N'Usuario modificado: DNI 23456789. Email: houstonrockets@gmail.com. Rol: RolSimple. Actor: 46948668Maximo', 2)
INSERT [dbo].[BitacoraEventos] ([Id], [Username], [Fecha], [Modulo], [Descripcion], [Criticidad]) VALUES (65, N'46948668Maximo', CAST(N'2026-04-29T19:03:52.967' AS DateTime), N'Usuarios', N'Login exitoso: 46948668Maximo', 3)
INSERT [dbo].[BitacoraEventos] ([Id], [Username], [Fecha], [Modulo], [Descripcion], [Criticidad]) VALUES (66, N'46948668Maximo', CAST(N'2026-04-29T19:04:01.183' AS DateTime), N'Usuarios', N'Logout exitoso: 46948668Maximo', 3)
INSERT [dbo].[BitacoraEventos] ([Id], [Username], [Fecha], [Modulo], [Descripcion], [Criticidad]) VALUES (67, N'46948668Maximo', CAST(N'2026-04-29T19:04:19.213' AS DateTime), N'Usuarios', N'Login exitoso: 46948668Maximo', 3)
INSERT [dbo].[BitacoraEventos] ([Id], [Username], [Fecha], [Modulo], [Descripcion], [Criticidad]) VALUES (68, N'46948668Maximo', CAST(N'2026-04-29T19:04:21.300' AS DateTime), N'Usuarios', N'Logout exitoso: 46948668Maximo', 3)
INSERT [dbo].[BitacoraEventos] ([Id], [Username], [Fecha], [Modulo], [Descripcion], [Criticidad]) VALUES (69, N'46948668Maximo', CAST(N'2026-05-02T13:38:09.040' AS DateTime), N'Usuarios', N'Login exitoso: 46948668Maximo', 3)
INSERT [dbo].[BitacoraEventos] ([Id], [Username], [Fecha], [Modulo], [Descripcion], [Criticidad]) VALUES (70, N'46948668Maximo', CAST(N'2026-05-02T13:38:16.073' AS DateTime), N'Usuarios', N'Usuario habilitado: DNI 23456789. Actor: 46948668Maximo', 2)
INSERT [dbo].[BitacoraEventos] ([Id], [Username], [Fecha], [Modulo], [Descripcion], [Criticidad]) VALUES (71, N'46948668Maximo', CAST(N'2026-05-02T13:38:19.330' AS DateTime), N'Usuarios', N'Usuario deshabilitado: DNI 23456789. Actor: 46948668Maximo', 2)
INSERT [dbo].[BitacoraEventos] ([Id], [Username], [Fecha], [Modulo], [Descripcion], [Criticidad]) VALUES (72, N'46905013Lucas', CAST(N'2026-05-02T13:59:39.807' AS DateTime), N'Usuarios', N'Usuario bloqueado: 46905013Lucas', 1)
INSERT [dbo].[BitacoraEventos] ([Id], [Username], [Fecha], [Modulo], [Descripcion], [Criticidad]) VALUES (73, N'46948668Maximo', CAST(N'2026-05-02T14:00:07.137' AS DateTime), N'Usuarios', N'Login exitoso: 46948668Maximo', 3)
INSERT [dbo].[BitacoraEventos] ([Id], [Username], [Fecha], [Modulo], [Descripcion], [Criticidad]) VALUES (74, N'46905013Lucas', CAST(N'2026-05-02T14:00:12.570' AS DateTime), N'Usuarios', N'Usuario desbloqueado: 46905013Lucas (Rol: Admin)', 1)
INSERT [dbo].[BitacoraEventos] ([Id], [Username], [Fecha], [Modulo], [Descripcion], [Criticidad]) VALUES (75, N'46948668Maximo', CAST(N'2026-05-02T14:00:25.370' AS DateTime), N'Usuarios', N'Logout exitoso: 46948668Maximo', 3)
INSERT [dbo].[BitacoraEventos] ([Id], [Username], [Fecha], [Modulo], [Descripcion], [Criticidad]) VALUES (76, N'46905013Lucas', CAST(N'2026-05-02T14:00:45.410' AS DateTime), N'Usuarios', N'Login exitoso: 46905013Lucas', 3)
INSERT [dbo].[BitacoraEventos] ([Id], [Username], [Fecha], [Modulo], [Descripcion], [Criticidad]) VALUES (77, N'46905013Lucas', CAST(N'2026-05-02T14:00:48.140' AS DateTime), N'Usuarios', N'Logout exitoso: 46905013Lucas', 3)
INSERT [dbo].[BitacoraEventos] ([Id], [Username], [Fecha], [Modulo], [Descripcion], [Criticidad]) VALUES (78, N'46948668Maximo', CAST(N'2026-05-04T13:52:57.467' AS DateTime), N'Usuarios', N'Login exitoso: 46948668Maximo', 3)
INSERT [dbo].[BitacoraEventos] ([Id], [Username], [Fecha], [Modulo], [Descripcion], [Criticidad]) VALUES (79, N'46948668Maximo', CAST(N'2026-05-04T13:53:45.680' AS DateTime), N'Usuarios', N'Usuario habilitado: DNI 23456789. Actor: 46948668Maximo', 2)
INSERT [dbo].[BitacoraEventos] ([Id], [Username], [Fecha], [Modulo], [Descripcion], [Criticidad]) VALUES (80, N'46948668Maximo', CAST(N'2026-05-04T13:54:43.313' AS DateTime), N'Usuarios', N'Usuario deshabilitado: DNI 23456789. Actor: 46948668Maximo', 2)
INSERT [dbo].[BitacoraEventos] ([Id], [Username], [Fecha], [Modulo], [Descripcion], [Criticidad]) VALUES (81, N'46948668Maximo', CAST(N'2026-05-04T13:54:56.197' AS DateTime), N'Usuarios', N'Usuario modificado: DNI 46948668. Email: MaximoKirichuk@gmail.com. Rol: Admin. Actor: 46948668Maximo', 2)
INSERT [dbo].[BitacoraEventos] ([Id], [Username], [Fecha], [Modulo], [Descripcion], [Criticidad]) VALUES (82, N'46948668Maximo', CAST(N'2026-05-04T13:55:12.010' AS DateTime), N'Usuarios', N'Usuario modificado: DNI 46948668. Email: MaximoKirichuk2005@gmail.com. Rol: Admin. Actor: 46948668Maximo', 2)
INSERT [dbo].[BitacoraEventos] ([Id], [Username], [Fecha], [Modulo], [Descripcion], [Criticidad]) VALUES (83, N'46948668Maximo', CAST(N'2026-05-04T13:55:20.450' AS DateTime), N'Usuarios', N'Usuario modificado: DNI 46948668. Email: MaximoKirichuk@gmail.com. Rol: Admin. Actor: 46948668Maximo', 2)
INSERT [dbo].[BitacoraEventos] ([Id], [Username], [Fecha], [Modulo], [Descripcion], [Criticidad]) VALUES (84, N'46948668Maximo', CAST(N'2026-05-04T13:57:11.910' AS DateTime), N'Usuarios', N'Logout exitoso: 46948668Maximo', 3)
INSERT [dbo].[BitacoraEventos] ([Id], [Username], [Fecha], [Modulo], [Descripcion], [Criticidad]) VALUES (85, N'46948668Maximo', CAST(N'2026-05-05T20:48:56.303' AS DateTime), N'Usuarios', N'Login exitoso: 46948668Maximo', 3)
INSERT [dbo].[BitacoraEventos] ([Id], [Username], [Fecha], [Modulo], [Descripcion], [Criticidad]) VALUES (86, N'46905013Lucas', CAST(N'2026-05-17T16:10:45.903' AS DateTime), N'Usuarios', N'Login exitoso: 46905013Lucas', 3)
INSERT [dbo].[BitacoraEventos] ([Id], [Username], [Fecha], [Modulo], [Descripcion], [Criticidad]) VALUES (87, N'46905013Lucas', CAST(N'2026-05-17T17:17:45.423' AS DateTime), N'Usuarios', N'Logout exitoso: 46905013Lucas', 3)
INSERT [dbo].[BitacoraEventos] ([Id], [Username], [Fecha], [Modulo], [Descripcion], [Criticidad]) VALUES (88, N'46905013Lucas', CAST(N'2026-05-18T11:39:25.390' AS DateTime), N'Usuarios', N'Login exitoso: 46905013Lucas', 3)
INSERT [dbo].[BitacoraEventos] ([Id], [Username], [Fecha], [Modulo], [Descripcion], [Criticidad]) VALUES (89, N'46905013Lucas', CAST(N'2026-05-18T11:39:46.870' AS DateTime), N'Usuarios', N'Usuario modificado: DNI 46948668. Email: MaximoKirichuk@gmail.com. Rol: RolSimple. Actor: 46905013Lucas', 2)
INSERT [dbo].[BitacoraEventos] ([Id], [Username], [Fecha], [Modulo], [Descripcion], [Criticidad]) VALUES (90, N'46905013Lucas', CAST(N'2026-05-18T13:09:59.157' AS DateTime), N'Usuarios', N'Login exitoso: 46905013Lucas', 3)
INSERT [dbo].[BitacoraEventos] ([Id], [Username], [Fecha], [Modulo], [Descripcion], [Criticidad]) VALUES (91, N'46905013Lucas', CAST(N'2026-05-18T13:10:02.867' AS DateTime), N'Usuarios', N'Logout exitoso: 46905013Lucas', 3)
INSERT [dbo].[BitacoraEventos] ([Id], [Username], [Fecha], [Modulo], [Descripcion], [Criticidad]) VALUES (92, N'46905013Lucas', CAST(N'2026-05-18T13:46:18.510' AS DateTime), N'Usuarios', N'Login exitoso: 46905013Lucas', 3)
INSERT [dbo].[BitacoraEventos] ([Id], [Username], [Fecha], [Modulo], [Descripcion], [Criticidad]) VALUES (93, N'47006530Benjamin', CAST(N'2026-05-18T13:47:21.540' AS DateTime), N'Usuarios', N'Nuevo usuario creado: 47006530Benjamin (Rol: RolSimple)', 1)
INSERT [dbo].[BitacoraEventos] ([Id], [Username], [Fecha], [Modulo], [Descripcion], [Criticidad]) VALUES (94, N'46905013Lucas', CAST(N'2026-05-18T13:47:39.403' AS DateTime), N'Usuarios', N'Logout exitoso: 46905013Lucas', 3)
INSERT [dbo].[BitacoraEventos] ([Id], [Username], [Fecha], [Modulo], [Descripcion], [Criticidad]) VALUES (95, N'47006530Benjamin', CAST(N'2026-05-18T13:49:45.620' AS DateTime), N'Usuarios', N'Login exitoso: 47006530Benjamin', 3)
INSERT [dbo].[BitacoraEventos] ([Id], [Username], [Fecha], [Modulo], [Descripcion], [Criticidad]) VALUES (96, N'47006530Benjamin', CAST(N'2026-05-18T13:49:59.170' AS DateTime), N'Usuarios', N'Logout exitoso: 47006530Benjamin', 3)
INSERT [dbo].[BitacoraEventos] ([Id], [Username], [Fecha], [Modulo], [Descripcion], [Criticidad]) VALUES (97, N'46905013Lucas', CAST(N'2026-05-18T14:13:49.447' AS DateTime), N'Usuarios', N'Login exitoso: 46905013Lucas', 3)
INSERT [dbo].[BitacoraEventos] ([Id], [Username], [Fecha], [Modulo], [Descripcion], [Criticidad]) VALUES (98, N'46905013Lucas', CAST(N'2026-05-18T14:14:03.163' AS DateTime), N'Usuarios', N'Logout exitoso: 46905013Lucas', 3)
INSERT [dbo].[BitacoraEventos] ([Id], [Username], [Fecha], [Modulo], [Descripcion], [Criticidad]) VALUES (99, N'46905013Lucas', CAST(N'2026-05-18T19:07:57.580' AS DateTime), N'Usuarios', N'Login exitoso: 46905013Lucas', 3)
INSERT [dbo].[BitacoraEventos] ([Id], [Username], [Fecha], [Modulo], [Descripcion], [Criticidad]) VALUES (100, N'46905013Lucas', CAST(N'2026-05-18T19:10:33.670' AS DateTime), N'Usuarios', N'Login exitoso: 46905013Lucas', 3)
INSERT [dbo].[BitacoraEventos] ([Id], [Username], [Fecha], [Modulo], [Descripcion], [Criticidad]) VALUES (101, N'46905013Lucas', CAST(N'2026-05-18T19:14:20.390' AS DateTime), N'Usuarios', N'Logout exitoso: 46905013Lucas', 3)
GO
INSERT [dbo].[BitacoraEventos] ([Id], [Username], [Fecha], [Modulo], [Descripcion], [Criticidad]) VALUES (102, N'46905013Lucas', CAST(N'2026-05-18T19:14:27.490' AS DateTime), N'Usuarios', N'Login exitoso: 46905013Lucas', 3)
INSERT [dbo].[BitacoraEventos] ([Id], [Username], [Fecha], [Modulo], [Descripcion], [Criticidad]) VALUES (103, N'46905013Lucas', CAST(N'2026-05-18T19:14:52.573' AS DateTime), N'Usuarios', N'Logout exitoso: 46905013Lucas', 3)
INSERT [dbo].[BitacoraEventos] ([Id], [Username], [Fecha], [Modulo], [Descripcion], [Criticidad]) VALUES (104, N'46905013Lucas', CAST(N'2026-05-18T20:04:45.570' AS DateTime), N'Usuarios', N'Login exitoso: 46905013Lucas', 3)
INSERT [dbo].[BitacoraEventos] ([Id], [Username], [Fecha], [Modulo], [Descripcion], [Criticidad]) VALUES (105, N'46905013Lucas', CAST(N'2026-05-18T20:17:11.567' AS DateTime), N'Usuarios', N'Login exitoso: 46905013Lucas', 3)
INSERT [dbo].[BitacoraEventos] ([Id], [Username], [Fecha], [Modulo], [Descripcion], [Criticidad]) VALUES (106, N'46905013Lucas', CAST(N'2026-05-19T19:02:26.363' AS DateTime), N'Usuarios', N'Login exitoso: 46905013Lucas', 3)
INSERT [dbo].[BitacoraEventos] ([Id], [Username], [Fecha], [Modulo], [Descripcion], [Criticidad]) VALUES (107, N'46905013Lucas', CAST(N'2026-05-19T20:20:08.137' AS DateTime), N'Usuarios', N'Login exitoso: 46905013Lucas', 3)
INSERT [dbo].[BitacoraEventos] ([Id], [Username], [Fecha], [Modulo], [Descripcion], [Criticidad]) VALUES (108, N'46905013Lucas', CAST(N'2026-05-19T20:20:14.847' AS DateTime), N'Usuarios', N'Logout exitoso: 46905013Lucas', 3)
INSERT [dbo].[BitacoraEventos] ([Id], [Username], [Fecha], [Modulo], [Descripcion], [Criticidad]) VALUES (109, N'47006530Benjamin', CAST(N'2026-05-19T20:20:57.157' AS DateTime), N'Usuarios', N'Usuario bloqueado por intentos fallidos: 47006530Benjamin', 1)
INSERT [dbo].[BitacoraEventos] ([Id], [Username], [Fecha], [Modulo], [Descripcion], [Criticidad]) VALUES (110, N'46905013Lucas', CAST(N'2026-05-19T20:21:29.650' AS DateTime), N'Usuarios', N'Login exitoso: 46905013Lucas', 3)
INSERT [dbo].[BitacoraEventos] ([Id], [Username], [Fecha], [Modulo], [Descripcion], [Criticidad]) VALUES (111, N'47006530Benjamin', CAST(N'2026-05-19T20:21:40.470' AS DateTime), N'Usuarios', N'Usuario desbloqueado: 47006530Benjamin (Rol: RolSimple)', 1)
INSERT [dbo].[BitacoraEventos] ([Id], [Username], [Fecha], [Modulo], [Descripcion], [Criticidad]) VALUES (112, N'46905013Lucas', CAST(N'2026-05-19T20:21:54.747' AS DateTime), N'Usuarios', N'Logout exitoso: 46905013Lucas', 3)
INSERT [dbo].[BitacoraEventos] ([Id], [Username], [Fecha], [Modulo], [Descripcion], [Criticidad]) VALUES (113, N'47006530Benjamin', CAST(N'2026-05-19T20:22:43.807' AS DateTime), N'Usuarios', N'Usuario bloqueado por intentos fallidos: 47006530Benjamin', 1)
INSERT [dbo].[BitacoraEventos] ([Id], [Username], [Fecha], [Modulo], [Descripcion], [Criticidad]) VALUES (114, N'46905013Lucas', CAST(N'2026-05-19T20:22:58.430' AS DateTime), N'Usuarios', N'Login exitoso: 46905013Lucas', 3)
INSERT [dbo].[BitacoraEventos] ([Id], [Username], [Fecha], [Modulo], [Descripcion], [Criticidad]) VALUES (115, N'46905013Lucas', CAST(N'2026-05-19T20:27:43.080' AS DateTime), N'Usuarios', N'Login exitoso: 46905013Lucas', 3)
INSERT [dbo].[BitacoraEventos] ([Id], [Username], [Fecha], [Modulo], [Descripcion], [Criticidad]) VALUES (116, N'47006530Benjamin', CAST(N'2026-05-19T20:28:02.687' AS DateTime), N'Usuarios', N'Usuario desbloqueado: 47006530Benjamin (Rol: RolSimple)', 1)
INSERT [dbo].[BitacoraEventos] ([Id], [Username], [Fecha], [Modulo], [Descripcion], [Criticidad]) VALUES (117, N'46905013Lucas', CAST(N'2026-05-19T20:28:11.527' AS DateTime), N'Usuarios', N'Logout exitoso: 46905013Lucas', 3)
INSERT [dbo].[BitacoraEventos] ([Id], [Username], [Fecha], [Modulo], [Descripcion], [Criticidad]) VALUES (118, N'47006530Benjamin', CAST(N'2026-05-19T20:28:25.273' AS DateTime), N'Usuarios', N'Login exitoso: 47006530Benjamin', 3)
INSERT [dbo].[BitacoraEventos] ([Id], [Username], [Fecha], [Modulo], [Descripcion], [Criticidad]) VALUES (119, N'47006530Benjamin', CAST(N'2026-05-19T20:28:30.977' AS DateTime), N'Usuarios', N'Logout exitoso: 47006530Benjamin', 3)
INSERT [dbo].[BitacoraEventos] ([Id], [Username], [Fecha], [Modulo], [Descripcion], [Criticidad]) VALUES (120, N'46905013Lucas', CAST(N'2026-05-19T20:31:34.470' AS DateTime), N'Usuarios', N'Login exitoso: 46905013Lucas', 3)
INSERT [dbo].[BitacoraEventos] ([Id], [Username], [Fecha], [Modulo], [Descripcion], [Criticidad]) VALUES (121, N'46905013Lucas', CAST(N'2026-05-19T20:32:12.213' AS DateTime), N'Usuarios', N'Contraseña modificada: 46905013Lucas', 1)
INSERT [dbo].[BitacoraEventos] ([Id], [Username], [Fecha], [Modulo], [Descripcion], [Criticidad]) VALUES (122, N'46905013Lucas', CAST(N'2026-05-19T20:33:34.380' AS DateTime), N'Usuarios', N'Usuario habilitado: DNI 23456789. Actor: 46905013Lucas', 1)
INSERT [dbo].[BitacoraEventos] ([Id], [Username], [Fecha], [Modulo], [Descripcion], [Criticidad]) VALUES (123, N'46905013Lucas', CAST(N'2026-05-19T20:33:44.893' AS DateTime), N'Usuarios', N'Usuario modificado: DNI 23456789. Email: houstonrockets@gmail.com. Rol: Admin. Actor: 46905013Lucas', 1)
INSERT [dbo].[BitacoraEventos] ([Id], [Username], [Fecha], [Modulo], [Descripcion], [Criticidad]) VALUES (124, N'46905013Lucas', CAST(N'2026-05-19T20:34:08.870' AS DateTime), N'Usuarios', N'Logout exitoso: 46905013Lucas', 3)
INSERT [dbo].[BitacoraEventos] ([Id], [Username], [Fecha], [Modulo], [Descripcion], [Criticidad]) VALUES (125, N'23456789as', CAST(N'2026-05-19T20:34:20.947' AS DateTime), N'Usuarios', N'Usuario bloqueado por intentos fallidos: 23456789as', 1)
INSERT [dbo].[BitacoraEventos] ([Id], [Username], [Fecha], [Modulo], [Descripcion], [Criticidad]) VALUES (126, N'46905013Lucas', CAST(N'2026-05-19T20:34:28.517' AS DateTime), N'Usuarios', N'Login exitoso: 46905013Lucas', 3)
INSERT [dbo].[BitacoraEventos] ([Id], [Username], [Fecha], [Modulo], [Descripcion], [Criticidad]) VALUES (127, N'23456789as', CAST(N'2026-05-19T20:34:39.283' AS DateTime), N'Usuarios', N'Usuario desbloqueado: 23456789as (Rol: Admin)', 1)
INSERT [dbo].[BitacoraEventos] ([Id], [Username], [Fecha], [Modulo], [Descripcion], [Criticidad]) VALUES (128, N'46905013Lucas', CAST(N'2026-05-19T20:34:57.313' AS DateTime), N'Usuarios', N'Logout exitoso: 46905013Lucas', 3)
INSERT [dbo].[BitacoraEventos] ([Id], [Username], [Fecha], [Modulo], [Descripcion], [Criticidad]) VALUES (129, N'23456789as', CAST(N'2026-05-19T20:35:22.753' AS DateTime), N'Usuarios', N'Login exitoso: 23456789as', 3)
INSERT [dbo].[BitacoraEventos] ([Id], [Username], [Fecha], [Modulo], [Descripcion], [Criticidad]) VALUES (130, N'23456789as', CAST(N'2026-05-19T20:38:29.500' AS DateTime), N'Usuarios', N'Usuario modificado: DNI 23456789. Email: AS@gmail.com. Rol: Admin. Actor: 23456789as', 1)
INSERT [dbo].[BitacoraEventos] ([Id], [Username], [Fecha], [Modulo], [Descripcion], [Criticidad]) VALUES (131, N'46948668Maximo', CAST(N'2026-05-19T21:15:43.503' AS DateTime), N'Usuarios', N'Usuario bloqueado por intentos fallidos: 46948668Maximo', 1)
INSERT [dbo].[BitacoraEventos] ([Id], [Username], [Fecha], [Modulo], [Descripcion], [Criticidad]) VALUES (132, N'46905013Lucas', CAST(N'2026-05-19T21:15:58.637' AS DateTime), N'Usuarios', N'Login exitoso: 46905013Lucas', 3)
INSERT [dbo].[BitacoraEventos] ([Id], [Username], [Fecha], [Modulo], [Descripcion], [Criticidad]) VALUES (133, N'46948668Maximo', CAST(N'2026-05-19T21:16:14.977' AS DateTime), N'Usuarios', N'Usuario desbloqueado: 46948668Maximo (Rol: RolSimple)', 1)
INSERT [dbo].[BitacoraEventos] ([Id], [Username], [Fecha], [Modulo], [Descripcion], [Criticidad]) VALUES (134, N'46905013Lucas', CAST(N'2026-05-19T21:16:27.490' AS DateTime), N'Usuarios', N'Logout exitoso: 46905013Lucas', 3)
INSERT [dbo].[BitacoraEventos] ([Id], [Username], [Fecha], [Modulo], [Descripcion], [Criticidad]) VALUES (135, N'46948668Maximo', CAST(N'2026-05-19T21:16:51.797' AS DateTime), N'Usuarios', N'Login exitoso: 46948668Maximo', 3)
INSERT [dbo].[BitacoraEventos] ([Id], [Username], [Fecha], [Modulo], [Descripcion], [Criticidad]) VALUES (136, N'46948668Maximo', CAST(N'2026-05-19T21:17:00.507' AS DateTime), N'Usuarios', N'Logout exitoso: 46948668Maximo', 3)
INSERT [dbo].[BitacoraEventos] ([Id], [Username], [Fecha], [Modulo], [Descripcion], [Criticidad]) VALUES (137, N'46905013Lucas', CAST(N'2026-05-19T21:17:15.423' AS DateTime), N'Usuarios', N'Login exitoso: 46905013Lucas', 3)
INSERT [dbo].[BitacoraEventos] ([Id], [Username], [Fecha], [Modulo], [Descripcion], [Criticidad]) VALUES (138, N'46905013Lucas', CAST(N'2026-05-19T21:17:59.760' AS DateTime), N'Usuarios', N'Usuario modificado: DNI 46948668. Email: MaximoKirichuk@gmail.com. Rol: Admin. Actor: 46905013Lucas', 1)
INSERT [dbo].[BitacoraEventos] ([Id], [Username], [Fecha], [Modulo], [Descripcion], [Criticidad]) VALUES (139, N'46905013Lucas', CAST(N'2026-05-19T21:18:14.443' AS DateTime), N'Usuarios', N'Usuario deshabilitado: DNI 23456789. Actor: 46905013Lucas', 1)
INSERT [dbo].[BitacoraEventos] ([Id], [Username], [Fecha], [Modulo], [Descripcion], [Criticidad]) VALUES (140, N'46905013Lucas', CAST(N'2026-05-19T21:18:37.367' AS DateTime), N'Usuarios', N'Usuario habilitado: DNI 23456789. Actor: 46905013Lucas', 1)
INSERT [dbo].[BitacoraEventos] ([Id], [Username], [Fecha], [Modulo], [Descripcion], [Criticidad]) VALUES (141, N'46905013Lucas', CAST(N'2026-05-19T21:22:44.307' AS DateTime), N'Usuarios', N'Login exitoso: 46905013Lucas', 3)
INSERT [dbo].[BitacoraEventos] ([Id], [Username], [Fecha], [Modulo], [Descripcion], [Criticidad]) VALUES (142, N'46905013Lucas', CAST(N'2026-05-19T21:23:08.847' AS DateTime), N'Usuarios', N'Logout exitoso: 46905013Lucas', 3)
INSERT [dbo].[BitacoraEventos] ([Id], [Username], [Fecha], [Modulo], [Descripcion], [Criticidad]) VALUES (143, N'46948668Maximo', CAST(N'2026-05-20T01:38:10.420' AS DateTime), N'Usuarios', N'Login exitoso: 46948668Maximo', 3)
INSERT [dbo].[BitacoraEventos] ([Id], [Username], [Fecha], [Modulo], [Descripcion], [Criticidad]) VALUES (144, N'46948668Maximo', CAST(N'2026-05-20T01:41:43.603' AS DateTime), N'Usuarios', N'Logout exitoso: 46948668Maximo', 3)
INSERT [dbo].[BitacoraEventos] ([Id], [Username], [Fecha], [Modulo], [Descripcion], [Criticidad]) VALUES (145, N'46948668Maximo', CAST(N'2026-05-20T01:51:09.110' AS DateTime), N'Usuarios', N'Login exitoso: 46948668Maximo', 3)
INSERT [dbo].[BitacoraEventos] ([Id], [Username], [Fecha], [Modulo], [Descripcion], [Criticidad]) VALUES (146, N'46948668Maximo', CAST(N'2026-05-20T02:29:54.247' AS DateTime), N'Usuarios', N'Login exitoso: 46948668Maximo', 3)
INSERT [dbo].[BitacoraEventos] ([Id], [Username], [Fecha], [Modulo], [Descripcion], [Criticidad]) VALUES (147, N'46948668Maximo', CAST(N'2026-05-20T02:39:18.083' AS DateTime), N'Usuarios', N'Logout exitoso: 46948668Maximo', 3)
INSERT [dbo].[BitacoraEventos] ([Id], [Username], [Fecha], [Modulo], [Descripcion], [Criticidad]) VALUES (148, N'46948668Maximo', CAST(N'2026-05-20T03:12:13.603' AS DateTime), N'Usuarios', N'Login exitoso: 46948668Maximo', 3)
INSERT [dbo].[BitacoraEventos] ([Id], [Username], [Fecha], [Modulo], [Descripcion], [Criticidad]) VALUES (149, N'46948668Maximo', CAST(N'2026-05-20T03:12:32.917' AS DateTime), N'Usuarios', N'Logout exitoso: 46948668Maximo', 3)
INSERT [dbo].[BitacoraEventos] ([Id], [Username], [Fecha], [Modulo], [Descripcion], [Criticidad]) VALUES (150, N'46948668Maximo', CAST(N'2026-05-20T03:22:32.723' AS DateTime), N'Usuarios', N'Login exitoso: 46948668Maximo', 3)
INSERT [dbo].[BitacoraEventos] ([Id], [Username], [Fecha], [Modulo], [Descripcion], [Criticidad]) VALUES (151, N'46948668Maximo', CAST(N'2026-05-20T03:23:04.920' AS DateTime), N'Usuarios', N'Logout exitoso: 46948668Maximo', 3)
INSERT [dbo].[BitacoraEventos] ([Id], [Username], [Fecha], [Modulo], [Descripcion], [Criticidad]) VALUES (152, N'46948668Maximo', CAST(N'2026-05-29T19:29:11.757' AS DateTime), N'Usuarios', N'Login exitoso: 46948668Maximo', 3)
INSERT [dbo].[BitacoraEventos] ([Id], [Username], [Fecha], [Modulo], [Descripcion], [Criticidad]) VALUES (153, N'46948668Maximo', CAST(N'2026-05-29T19:31:04.050' AS DateTime), N'Usuarios', N'Login exitoso: 46948668Maximo', 3)
INSERT [dbo].[BitacoraEventos] ([Id], [Username], [Fecha], [Modulo], [Descripcion], [Criticidad]) VALUES (154, N'46948668Maximo', CAST(N'2026-05-29T19:42:29.523' AS DateTime), N'Usuarios', N'Login exitoso: 46948668Maximo', 3)
INSERT [dbo].[BitacoraEventos] ([Id], [Username], [Fecha], [Modulo], [Descripcion], [Criticidad]) VALUES (155, N'46948668Maximo', CAST(N'2026-05-29T19:43:02.133' AS DateTime), N'Usuarios', N'Logout exitoso: 46948668Maximo', 3)
INSERT [dbo].[BitacoraEventos] ([Id], [Username], [Fecha], [Modulo], [Descripcion], [Criticidad]) VALUES (156, N'46948668Maximo', CAST(N'2026-05-29T19:43:08.853' AS DateTime), N'Usuarios', N'Login exitoso: 46948668Maximo', 3)
INSERT [dbo].[BitacoraEventos] ([Id], [Username], [Fecha], [Modulo], [Descripcion], [Criticidad]) VALUES (157, N'46948668Maximo', CAST(N'2026-05-29T19:43:21.990' AS DateTime), N'Usuarios', N'Logout exitoso: 46948668Maximo', 3)
INSERT [dbo].[BitacoraEventos] ([Id], [Username], [Fecha], [Modulo], [Descripcion], [Criticidad]) VALUES (158, N'46948668Maximo', CAST(N'2026-05-29T19:46:25.867' AS DateTime), N'Usuarios', N'Login exitoso: 46948668Maximo', 3)
INSERT [dbo].[BitacoraEventos] ([Id], [Username], [Fecha], [Modulo], [Descripcion], [Criticidad]) VALUES (159, N'124231241fas', CAST(N'2026-05-29T19:48:09.463' AS DateTime), N'Usuarios', N'Nuevo usuario creado: 124231241fas (Rol: Admin)', 1)
INSERT [dbo].[BitacoraEventos] ([Id], [Username], [Fecha], [Modulo], [Descripcion], [Criticidad]) VALUES (160, N'46948668Maximo', CAST(N'2026-05-29T19:48:17.660' AS DateTime), N'Usuarios', N'Usuario deshabilitado: DNI 124231241. Actor: 46948668Maximo', 1)
INSERT [dbo].[BitacoraEventos] ([Id], [Username], [Fecha], [Modulo], [Descripcion], [Criticidad]) VALUES (161, N'46948668Maximo', CAST(N'2026-05-29T22:50:46.207' AS DateTime), N'Usuarios', N'Login exitoso: 46948668Maximo', 3)
INSERT [dbo].[BitacoraEventos] ([Id], [Username], [Fecha], [Modulo], [Descripcion], [Criticidad]) VALUES (162, N'46948668Maximo', CAST(N'2026-05-29T23:12:55.943' AS DateTime), N'Usuarios', N'Login exitoso: 46948668Maximo', 3)
INSERT [dbo].[BitacoraEventos] ([Id], [Username], [Fecha], [Modulo], [Descripcion], [Criticidad]) VALUES (163, N'46948668Maximo', CAST(N'2026-05-30T00:35:05.427' AS DateTime), N'Usuarios', N'Login exitoso: 46948668Maximo', 3)
INSERT [dbo].[BitacoraEventos] ([Id], [Username], [Fecha], [Modulo], [Descripcion], [Criticidad]) VALUES (164, N'46948668Maximo', CAST(N'2026-05-30T00:43:11.237' AS DateTime), N'Usuarios', N'Logout exitoso: 46948668Maximo', 3)
INSERT [dbo].[BitacoraEventos] ([Id], [Username], [Fecha], [Modulo], [Descripcion], [Criticidad]) VALUES (165, N'46948668Maximo', CAST(N'2026-05-30T00:44:55.933' AS DateTime), N'Usuarios', N'Login exitoso: 46948668Maximo', 3)
INSERT [dbo].[BitacoraEventos] ([Id], [Username], [Fecha], [Modulo], [Descripcion], [Criticidad]) VALUES (166, N'46948668Maximo', CAST(N'2026-05-30T00:48:58.810' AS DateTime), N'Usuarios', N'Login exitoso: 46948668Maximo', 3)
INSERT [dbo].[BitacoraEventos] ([Id], [Username], [Fecha], [Modulo], [Descripcion], [Criticidad]) VALUES (167, N'46948668Maximo', CAST(N'2026-05-30T00:53:52.017' AS DateTime), N'Usuarios', N'Logout exitoso: 46948668Maximo', 3)
INSERT [dbo].[BitacoraEventos] ([Id], [Username], [Fecha], [Modulo], [Descripcion], [Criticidad]) VALUES (168, N'46948668Maximo', CAST(N'2026-05-30T19:57:56.200' AS DateTime), N'Usuarios', N'Login exitoso: 46948668Maximo', 3)
INSERT [dbo].[BitacoraEventos] ([Id], [Username], [Fecha], [Modulo], [Descripcion], [Criticidad]) VALUES (169, N'46948668Maximo', CAST(N'2026-05-30T20:05:40.050' AS DateTime), N'Usuarios', N'Logout exitoso: 46948668Maximo', 3)
INSERT [dbo].[BitacoraEventos] ([Id], [Username], [Fecha], [Modulo], [Descripcion], [Criticidad]) VALUES (170, N'46948668Maximo', CAST(N'2026-05-31T13:40:06.523' AS DateTime), N'Usuarios', N'Login exitoso: 46948668Maximo', 3)
INSERT [dbo].[BitacoraEventos] ([Id], [Username], [Fecha], [Modulo], [Descripcion], [Criticidad]) VALUES (171, N'46948668Maximo', CAST(N'2026-05-31T13:57:05.853' AS DateTime), N'Usuarios', N'Login exitoso: 46948668Maximo', 3)
INSERT [dbo].[BitacoraEventos] ([Id], [Username], [Fecha], [Modulo], [Descripcion], [Criticidad]) VALUES (172, N'46948668Maximo', CAST(N'2026-05-31T14:04:38.123' AS DateTime), N'Usuarios', N'Logout exitoso: 46948668Maximo', 3)
INSERT [dbo].[BitacoraEventos] ([Id], [Username], [Fecha], [Modulo], [Descripcion], [Criticidad]) VALUES (173, N'46948668Maximo', CAST(N'2026-05-31T14:12:49.487' AS DateTime), N'Usuarios', N'Login exitoso: 46948668Maximo', 3)
INSERT [dbo].[BitacoraEventos] ([Id], [Username], [Fecha], [Modulo], [Descripcion], [Criticidad]) VALUES (174, N'46948668Maximo', CAST(N'2026-05-31T14:13:14.677' AS DateTime), N'Usuarios', N'Logout exitoso: 46948668Maximo', 3)
INSERT [dbo].[BitacoraEventos] ([Id], [Username], [Fecha], [Modulo], [Descripcion], [Criticidad]) VALUES (175, N'46948668Maximo', CAST(N'2026-05-31T14:21:15.107' AS DateTime), N'Usuarios', N'Login exitoso: 46948668Maximo', 3)
INSERT [dbo].[BitacoraEventos] ([Id], [Username], [Fecha], [Modulo], [Descripcion], [Criticidad]) VALUES (176, N'46948668Maximo', CAST(N'2026-05-31T14:21:38.133' AS DateTime), N'Usuarios', N'Logout exitoso: 46948668Maximo', 3)
INSERT [dbo].[BitacoraEventos] ([Id], [Username], [Fecha], [Modulo], [Descripcion], [Criticidad]) VALUES (177, N'46948668Maximo', CAST(N'2026-05-31T14:21:55.137' AS DateTime), N'Usuarios', N'Login exitoso: 46948668Maximo', 3)
INSERT [dbo].[BitacoraEventos] ([Id], [Username], [Fecha], [Modulo], [Descripcion], [Criticidad]) VALUES (178, N'46948668Maximo', CAST(N'2026-05-31T14:26:34.193' AS DateTime), N'Usuarios', N'Logout exitoso: 46948668Maximo', 3)
INSERT [dbo].[BitacoraEventos] ([Id], [Username], [Fecha], [Modulo], [Descripcion], [Criticidad]) VALUES (179, N'46948668Maximo', CAST(N'2026-05-31T14:26:42.160' AS DateTime), N'Usuarios', N'Login exitoso: 46948668Maximo', 3)
INSERT [dbo].[BitacoraEventos] ([Id], [Username], [Fecha], [Modulo], [Descripcion], [Criticidad]) VALUES (180, N'46948668Maximo', CAST(N'2026-05-31T14:26:47.903' AS DateTime), N'Usuarios', N'Logout exitoso: 46948668Maximo', 3)
INSERT [dbo].[BitacoraEventos] ([Id], [Username], [Fecha], [Modulo], [Descripcion], [Criticidad]) VALUES (181, N'46948668Maximo', CAST(N'2026-05-31T14:30:38.820' AS DateTime), N'Usuarios', N'Login exitoso: 46948668Maximo', 3)
INSERT [dbo].[BitacoraEventos] ([Id], [Username], [Fecha], [Modulo], [Descripcion], [Criticidad]) VALUES (182, N'46948668Maximo', CAST(N'2026-05-31T14:35:15.363' AS DateTime), N'Usuarios', N'Logout exitoso: 46948668Maximo', 3)
INSERT [dbo].[BitacoraEventos] ([Id], [Username], [Fecha], [Modulo], [Descripcion], [Criticidad]) VALUES (183, N'46948668Maximo', CAST(N'2026-05-31T14:37:28.723' AS DateTime), N'Usuarios', N'Login exitoso: 46948668Maximo', 3)
INSERT [dbo].[BitacoraEventos] ([Id], [Username], [Fecha], [Modulo], [Descripcion], [Criticidad]) VALUES (184, N'46948668Maximo', CAST(N'2026-05-31T14:37:53.210' AS DateTime), N'Usuarios', N'Logout exitoso: 46948668Maximo', 3)
INSERT [dbo].[BitacoraEventos] ([Id], [Username], [Fecha], [Modulo], [Descripcion], [Criticidad]) VALUES (185, N'46948668Maximo', CAST(N'2026-05-31T14:39:11.670' AS DateTime), N'Usuarios', N'Login exitoso: 46948668Maximo', 3)
INSERT [dbo].[BitacoraEventos] ([Id], [Username], [Fecha], [Modulo], [Descripcion], [Criticidad]) VALUES (186, N'46948668Maximo', CAST(N'2026-05-31T14:42:48.507' AS DateTime), N'Usuarios', N'Login exitoso: 46948668Maximo', 3)
INSERT [dbo].[BitacoraEventos] ([Id], [Username], [Fecha], [Modulo], [Descripcion], [Criticidad]) VALUES (187, N'46948668Maximo', CAST(N'2026-05-31T14:43:11.413' AS DateTime), N'Usuarios', N'Logout exitoso: 46948668Maximo', 3)
INSERT [dbo].[BitacoraEventos] ([Id], [Username], [Fecha], [Modulo], [Descripcion], [Criticidad]) VALUES (188, N'46905013Lucas', CAST(N'2026-05-31T14:43:42.067' AS DateTime), N'Usuarios', N'Login exitoso: 46905013Lucas', 3)
INSERT [dbo].[BitacoraEventos] ([Id], [Username], [Fecha], [Modulo], [Descripcion], [Criticidad]) VALUES (189, N'46905013Lucas', CAST(N'2026-05-31T14:44:05.553' AS DateTime), N'Usuarios', N'Logout exitoso: 46905013Lucas', 3)
INSERT [dbo].[BitacoraEventos] ([Id], [Username], [Fecha], [Modulo], [Descripcion], [Criticidad]) VALUES (190, N'46948668Maximo', CAST(N'2026-05-31T14:44:13.057' AS DateTime), N'Usuarios', N'Login exitoso: 46948668Maximo', 3)
INSERT [dbo].[BitacoraEventos] ([Id], [Username], [Fecha], [Modulo], [Descripcion], [Criticidad]) VALUES (191, N'46948668Maximo', CAST(N'2026-05-31T15:13:24.760' AS DateTime), N'Usuarios', N'Login exitoso: 46948668Maximo', 3)
INSERT [dbo].[BitacoraEventos] ([Id], [Username], [Fecha], [Modulo], [Descripcion], [Criticidad]) VALUES (192, N'46948668Maximo', CAST(N'2026-05-31T15:13:45.543' AS DateTime), N'Usuarios', N'Usuario modificado: DNI 46905013. Email: lucas@gmail.com. Rol: RolSimple. Actor: 46948668Maximo', 1)
INSERT [dbo].[BitacoraEventos] ([Id], [Username], [Fecha], [Modulo], [Descripcion], [Criticidad]) VALUES (193, N'46948668Maximo', CAST(N'2026-05-31T16:07:13.953' AS DateTime), N'Usuarios', N'Login exitoso: 46948668Maximo', 3)
INSERT [dbo].[BitacoraEventos] ([Id], [Username], [Fecha], [Modulo], [Descripcion], [Criticidad]) VALUES (194, N'46948668Maximo', CAST(N'2026-05-31T16:17:53.687' AS DateTime), N'Usuarios', N'Login exitoso: 46948668Maximo', 3)
INSERT [dbo].[BitacoraEventos] ([Id], [Username], [Fecha], [Modulo], [Descripcion], [Criticidad]) VALUES (195, N'46948668Maximo', CAST(N'2026-05-31T16:22:48.050' AS DateTime), N'Usuarios', N'Usuario modificado: DNI 46905013. Email: lucas@gmail.com. Rol: RolAdminSinAuditoria. Actor: 46948668Maximo', 1)
INSERT [dbo].[BitacoraEventos] ([Id], [Username], [Fecha], [Modulo], [Descripcion], [Criticidad]) VALUES (196, N'46948668Maximo', CAST(N'2026-05-31T16:22:51.583' AS DateTime), N'Usuarios', N'Logout exitoso: 46948668Maximo', 3)
INSERT [dbo].[BitacoraEventos] ([Id], [Username], [Fecha], [Modulo], [Descripcion], [Criticidad]) VALUES (197, N'46905013Lucas', CAST(N'2026-05-31T16:23:13.320' AS DateTime), N'Usuarios', N'Login exitoso: 46905013Lucas', 3)
INSERT [dbo].[BitacoraEventos] ([Id], [Username], [Fecha], [Modulo], [Descripcion], [Criticidad]) VALUES (198, N'46905013Lucas', CAST(N'2026-05-31T16:23:24.017' AS DateTime), N'Usuarios', N'Logout exitoso: 46905013Lucas', 3)
INSERT [dbo].[BitacoraEventos] ([Id], [Username], [Fecha], [Modulo], [Descripcion], [Criticidad]) VALUES (199, N'46948668Maximo', CAST(N'2026-05-31T16:24:09.163' AS DateTime), N'Usuarios', N'Login exitoso: 46948668Maximo', 3)
INSERT [dbo].[BitacoraEventos] ([Id], [Username], [Fecha], [Modulo], [Descripcion], [Criticidad]) VALUES (200, N'46948668Maximo', CAST(N'2026-05-31T16:25:12.413' AS DateTime), N'Usuarios', N'Logout exitoso: 46948668Maximo', 3)
INSERT [dbo].[BitacoraEventos] ([Id], [Username], [Fecha], [Modulo], [Descripcion], [Criticidad]) VALUES (201, N'46905013Lucas', CAST(N'2026-05-31T16:25:25.743' AS DateTime), N'Usuarios', N'Login exitoso: 46905013Lucas', 3)
GO
INSERT [dbo].[BitacoraEventos] ([Id], [Username], [Fecha], [Modulo], [Descripcion], [Criticidad]) VALUES (202, N'46905013Lucas', CAST(N'2026-05-31T16:25:30.013' AS DateTime), N'Usuarios', N'Logout exitoso: 46905013Lucas', 3)
INSERT [dbo].[BitacoraEventos] ([Id], [Username], [Fecha], [Modulo], [Descripcion], [Criticidad]) VALUES (203, N'46948668Maximo', CAST(N'2026-05-31T16:25:53.960' AS DateTime), N'Usuarios', N'Login exitoso: 46948668Maximo', 3)
INSERT [dbo].[BitacoraEventos] ([Id], [Username], [Fecha], [Modulo], [Descripcion], [Criticidad]) VALUES (204, N'46948668Maximo', CAST(N'2026-05-31T16:27:53.110' AS DateTime), N'Usuarios', N'Logout exitoso: 46948668Maximo', 3)
INSERT [dbo].[BitacoraEventos] ([Id], [Username], [Fecha], [Modulo], [Descripcion], [Criticidad]) VALUES (205, N'46905013Lucas', CAST(N'2026-05-31T16:28:03.563' AS DateTime), N'Usuarios', N'Login exitoso: 46905013Lucas', 3)
INSERT [dbo].[BitacoraEventos] ([Id], [Username], [Fecha], [Modulo], [Descripcion], [Criticidad]) VALUES (206, N'46905013Lucas', CAST(N'2026-05-31T16:28:36.363' AS DateTime), N'Usuarios', N'Logout exitoso: 46905013Lucas', 3)
INSERT [dbo].[BitacoraEventos] ([Id], [Username], [Fecha], [Modulo], [Descripcion], [Criticidad]) VALUES (207, N'46948668Maximo', CAST(N'2026-05-31T17:40:36.033' AS DateTime), N'Usuarios', N'Login exitoso: 46948668Maximo', 3)
INSERT [dbo].[BitacoraEventos] ([Id], [Username], [Fecha], [Modulo], [Descripcion], [Criticidad]) VALUES (208, N'46948668Maximo', CAST(N'2026-05-31T17:44:24.130' AS DateTime), N'Usuarios', N'Logout exitoso: 46948668Maximo', 3)
INSERT [dbo].[BitacoraEventos] ([Id], [Username], [Fecha], [Modulo], [Descripcion], [Criticidad]) VALUES (209, N'46905013Lucas', CAST(N'2026-05-31T17:44:34.800' AS DateTime), N'Usuarios', N'Login exitoso: 46905013Lucas', 3)
INSERT [dbo].[BitacoraEventos] ([Id], [Username], [Fecha], [Modulo], [Descripcion], [Criticidad]) VALUES (210, N'46905013Lucas', CAST(N'2026-05-31T17:44:49.743' AS DateTime), N'Usuarios', N'Logout exitoso: 46905013Lucas', 3)
INSERT [dbo].[BitacoraEventos] ([Id], [Username], [Fecha], [Modulo], [Descripcion], [Criticidad]) VALUES (211, N'46948668Maximo', CAST(N'2026-05-31T17:45:01.183' AS DateTime), N'Usuarios', N'Login exitoso: 46948668Maximo', 3)
INSERT [dbo].[BitacoraEventos] ([Id], [Username], [Fecha], [Modulo], [Descripcion], [Criticidad]) VALUES (212, N'46948668Maximo', CAST(N'2026-05-31T17:45:58.030' AS DateTime), N'Usuarios', N'Logout exitoso: 46948668Maximo', 3)
INSERT [dbo].[BitacoraEventos] ([Id], [Username], [Fecha], [Modulo], [Descripcion], [Criticidad]) VALUES (213, N'46905013Lucas', CAST(N'2026-05-31T17:46:13.477' AS DateTime), N'Usuarios', N'Login exitoso: 46905013Lucas', 3)
INSERT [dbo].[BitacoraEventos] ([Id], [Username], [Fecha], [Modulo], [Descripcion], [Criticidad]) VALUES (214, N'46905013Lucas', CAST(N'2026-05-31T17:46:49.167' AS DateTime), N'Usuarios', N'Logout exitoso: 46905013Lucas', 3)
INSERT [dbo].[BitacoraEventos] ([Id], [Username], [Fecha], [Modulo], [Descripcion], [Criticidad]) VALUES (215, N'47006530Benjamin', CAST(N'2026-05-31T17:46:59.513' AS DateTime), N'Usuarios', N'Login exitoso: 47006530Benjamin', 3)
INSERT [dbo].[BitacoraEventos] ([Id], [Username], [Fecha], [Modulo], [Descripcion], [Criticidad]) VALUES (216, N'47006530Benjamin', CAST(N'2026-05-31T17:47:24.340' AS DateTime), N'Usuarios', N'Logout exitoso: 47006530Benjamin', 3)
INSERT [dbo].[BitacoraEventos] ([Id], [Username], [Fecha], [Modulo], [Descripcion], [Criticidad]) VALUES (217, N'46948668Maximo', CAST(N'2026-05-31T17:47:42.680' AS DateTime), N'Usuarios', N'Login exitoso: 46948668Maximo', 3)
INSERT [dbo].[BitacoraEventos] ([Id], [Username], [Fecha], [Modulo], [Descripcion], [Criticidad]) VALUES (218, N'46948668Maximo', CAST(N'2026-05-31T17:59:53.663' AS DateTime), N'Usuarios', N'Logout exitoso: 46948668Maximo', 3)
INSERT [dbo].[BitacoraEventos] ([Id], [Username], [Fecha], [Modulo], [Descripcion], [Criticidad]) VALUES (219, N'46948668Maximo', CAST(N'2026-05-31T18:06:54.793' AS DateTime), N'Usuarios', N'Login exitoso: 46948668Maximo', 3)
INSERT [dbo].[BitacoraEventos] ([Id], [Username], [Fecha], [Modulo], [Descripcion], [Criticidad]) VALUES (220, N'46948668Maximo', CAST(N'2026-05-31T18:19:58.827' AS DateTime), N'Usuarios', N'Logout exitoso: 46948668Maximo', 3)
INSERT [dbo].[BitacoraEventos] ([Id], [Username], [Fecha], [Modulo], [Descripcion], [Criticidad]) VALUES (221, N'46948668Maximo', CAST(N'2026-05-31T18:25:06.420' AS DateTime), N'Usuarios', N'Login exitoso: 46948668Maximo', 3)
INSERT [dbo].[BitacoraEventos] ([Id], [Username], [Fecha], [Modulo], [Descripcion], [Criticidad]) VALUES (222, N'46948668Maximo', CAST(N'2026-05-31T18:26:02.547' AS DateTime), N'Usuarios', N'Logout exitoso: 46948668Maximo', 3)
INSERT [dbo].[BitacoraEventos] ([Id], [Username], [Fecha], [Modulo], [Descripcion], [Criticidad]) VALUES (223, N'46948668Maximo', CAST(N'2026-05-31T18:27:42.557' AS DateTime), N'Usuarios', N'Login exitoso: 46948668Maximo', 3)
INSERT [dbo].[BitacoraEventos] ([Id], [Username], [Fecha], [Modulo], [Descripcion], [Criticidad]) VALUES (224, N'46948668Maximo', CAST(N'2026-05-31T18:29:59.360' AS DateTime), N'Usuarios', N'Logout exitoso: 46948668Maximo', 3)
INSERT [dbo].[BitacoraEventos] ([Id], [Username], [Fecha], [Modulo], [Descripcion], [Criticidad]) VALUES (225, N'46948668Maximo', CAST(N'2026-05-31T18:33:55.490' AS DateTime), N'Usuarios', N'Login exitoso: 46948668Maximo', 3)
INSERT [dbo].[BitacoraEventos] ([Id], [Username], [Fecha], [Modulo], [Descripcion], [Criticidad]) VALUES (226, N'46948668Maximo', CAST(N'2026-05-31T18:35:40.393' AS DateTime), N'Usuarios', N'Logout exitoso: 46948668Maximo', 3)
INSERT [dbo].[BitacoraEventos] ([Id], [Username], [Fecha], [Modulo], [Descripcion], [Criticidad]) VALUES (227, N'46948668Maximo', CAST(N'2026-05-31T18:35:49.433' AS DateTime), N'Usuarios', N'Login exitoso: 46948668Maximo', 3)
INSERT [dbo].[BitacoraEventos] ([Id], [Username], [Fecha], [Modulo], [Descripcion], [Criticidad]) VALUES (228, N'46948668Maximo', CAST(N'2026-05-31T18:37:49.937' AS DateTime), N'Usuarios', N'Logout exitoso: 46948668Maximo', 3)
INSERT [dbo].[BitacoraEventos] ([Id], [Username], [Fecha], [Modulo], [Descripcion], [Criticidad]) VALUES (229, N'46948668Maximo', CAST(N'2026-05-31T18:37:56.263' AS DateTime), N'Usuarios', N'Login exitoso: 46948668Maximo', 3)
INSERT [dbo].[BitacoraEventos] ([Id], [Username], [Fecha], [Modulo], [Descripcion], [Criticidad]) VALUES (230, N'46948668Maximo', CAST(N'2026-05-31T18:43:24.287' AS DateTime), N'Usuarios', N'Usuario modificado: DNI 46905013. Email: lucas@gmail.com. Rol: SuperAdmin. Actor: 46948668Maximo', 1)
INSERT [dbo].[BitacoraEventos] ([Id], [Username], [Fecha], [Modulo], [Descripcion], [Criticidad]) VALUES (231, N'46948668Maximo', CAST(N'2026-05-31T18:43:32.063' AS DateTime), N'Usuarios', N'Logout exitoso: 46948668Maximo', 3)
INSERT [dbo].[BitacoraEventos] ([Id], [Username], [Fecha], [Modulo], [Descripcion], [Criticidad]) VALUES (232, N'46905013Lucas', CAST(N'2026-05-31T18:43:45.923' AS DateTime), N'Usuarios', N'Login exitoso: 46905013Lucas', 3)
INSERT [dbo].[BitacoraEventos] ([Id], [Username], [Fecha], [Modulo], [Descripcion], [Criticidad]) VALUES (233, N'46905013Lucas', CAST(N'2026-05-31T18:43:53.923' AS DateTime), N'Usuarios', N'Logout exitoso: 46905013Lucas', 3)
INSERT [dbo].[BitacoraEventos] ([Id], [Username], [Fecha], [Modulo], [Descripcion], [Criticidad]) VALUES (234, N'46948668Maximo', CAST(N'2026-05-31T18:44:18.843' AS DateTime), N'Usuarios', N'Login exitoso: 46948668Maximo', 3)
INSERT [dbo].[BitacoraEventos] ([Id], [Username], [Fecha], [Modulo], [Descripcion], [Criticidad]) VALUES (235, N'46948668Maximo', CAST(N'2026-05-31T18:47:39.710' AS DateTime), N'Usuarios', N'Logout exitoso: 46948668Maximo', 3)
INSERT [dbo].[BitacoraEventos] ([Id], [Username], [Fecha], [Modulo], [Descripcion], [Criticidad]) VALUES (236, N'46905013Lucas', CAST(N'2026-05-31T18:47:50.133' AS DateTime), N'Usuarios', N'Login exitoso: 46905013Lucas', 3)
INSERT [dbo].[BitacoraEventos] ([Id], [Username], [Fecha], [Modulo], [Descripcion], [Criticidad]) VALUES (237, N'46905013Lucas', CAST(N'2026-05-31T18:48:01.207' AS DateTime), N'Usuarios', N'Usuario modificado: DNI 46948668. Email: MaximoKirichuk@gmail.com. Rol: RolAdminSinAuditoria. Actor: 46905013Lucas', 1)
INSERT [dbo].[BitacoraEventos] ([Id], [Username], [Fecha], [Modulo], [Descripcion], [Criticidad]) VALUES (238, N'46905013Lucas', CAST(N'2026-05-31T18:48:05.967' AS DateTime), N'Usuarios', N'Logout exitoso: 46905013Lucas', 3)
INSERT [dbo].[BitacoraEventos] ([Id], [Username], [Fecha], [Modulo], [Descripcion], [Criticidad]) VALUES (239, N'46948668Maximo', CAST(N'2026-05-31T18:48:23.167' AS DateTime), N'Usuarios', N'Login exitoso: 46948668Maximo', 3)
INSERT [dbo].[BitacoraEventos] ([Id], [Username], [Fecha], [Modulo], [Descripcion], [Criticidad]) VALUES (240, N'46948668Maximo', CAST(N'2026-05-31T18:48:45.720' AS DateTime), N'Usuarios', N'Usuario modificado: DNI 46905013. Email: lucas@gmail.com. Rol: Admin. Actor: 46948668Maximo', 1)
INSERT [dbo].[BitacoraEventos] ([Id], [Username], [Fecha], [Modulo], [Descripcion], [Criticidad]) VALUES (241, N'46948668Maximo', CAST(N'2026-05-31T18:48:55.210' AS DateTime), N'Usuarios', N'Usuario modificado: DNI 46905013. Email: lucas@gmail.com. Rol: SuperAdmin. Actor: 46948668Maximo', 1)
INSERT [dbo].[BitacoraEventos] ([Id], [Username], [Fecha], [Modulo], [Descripcion], [Criticidad]) VALUES (242, N'46948668Maximo', CAST(N'2026-05-31T18:49:02.760' AS DateTime), N'Usuarios', N'Logout exitoso: 46948668Maximo', 3)
INSERT [dbo].[BitacoraEventos] ([Id], [Username], [Fecha], [Modulo], [Descripcion], [Criticidad]) VALUES (243, N'46948668Maximo', CAST(N'2026-05-31T19:48:32.947' AS DateTime), N'Usuarios', N'Login exitoso: 46948668Maximo', 3)
INSERT [dbo].[BitacoraEventos] ([Id], [Username], [Fecha], [Modulo], [Descripcion], [Criticidad]) VALUES (244, N'46948668Maximo', CAST(N'2026-05-31T19:48:59.013' AS DateTime), N'Usuarios', N'Logout exitoso: 46948668Maximo', 3)
INSERT [dbo].[BitacoraEventos] ([Id], [Username], [Fecha], [Modulo], [Descripcion], [Criticidad]) VALUES (245, N'46948668Maximo', CAST(N'2026-05-31T20:37:07.797' AS DateTime), N'Usuarios', N'Login exitoso: 46948668Maximo', 3)
INSERT [dbo].[BitacoraEventos] ([Id], [Username], [Fecha], [Modulo], [Descripcion], [Criticidad]) VALUES (246, N'46948668Maximo', CAST(N'2026-05-31T20:37:14.347' AS DateTime), N'Usuarios', N'Logout exitoso: 46948668Maximo', 3)
INSERT [dbo].[BitacoraEventos] ([Id], [Username], [Fecha], [Modulo], [Descripcion], [Criticidad]) VALUES (247, N'46948668Maximo', CAST(N'2026-05-31T20:46:07.030' AS DateTime), N'Usuarios', N'Login exitoso: 46948668Maximo', 3)
INSERT [dbo].[BitacoraEventos] ([Id], [Username], [Fecha], [Modulo], [Descripcion], [Criticidad]) VALUES (248, N'46948668Maximo', CAST(N'2026-05-31T20:46:30.873' AS DateTime), N'Usuarios', N'Logout exitoso: 46948668Maximo', 3)
INSERT [dbo].[BitacoraEventos] ([Id], [Username], [Fecha], [Modulo], [Descripcion], [Criticidad]) VALUES (249, N'46948668Maximo', CAST(N'2026-05-31T20:47:41.920' AS DateTime), N'Usuarios', N'Login exitoso: 46948668Maximo', 3)
INSERT [dbo].[BitacoraEventos] ([Id], [Username], [Fecha], [Modulo], [Descripcion], [Criticidad]) VALUES (250, N'46948668Maximo', CAST(N'2026-05-31T20:57:08.893' AS DateTime), N'Usuarios', N'Login exitoso: 46948668Maximo', 3)
INSERT [dbo].[BitacoraEventos] ([Id], [Username], [Fecha], [Modulo], [Descripcion], [Criticidad]) VALUES (251, N'46948668Maximo', CAST(N'2026-05-31T20:57:25.800' AS DateTime), N'Usuarios', N'Logout exitoso: 46948668Maximo', 3)
INSERT [dbo].[BitacoraEventos] ([Id], [Username], [Fecha], [Modulo], [Descripcion], [Criticidad]) VALUES (252, N'46948668Maximo', CAST(N'2026-05-31T20:58:49.527' AS DateTime), N'Usuarios', N'Login exitoso: 46948668Maximo', 3)
INSERT [dbo].[BitacoraEventos] ([Id], [Username], [Fecha], [Modulo], [Descripcion], [Criticidad]) VALUES (253, N'46948668Maximo', CAST(N'2026-05-31T20:58:58.903' AS DateTime), N'Usuarios', N'Logout exitoso: 46948668Maximo', 3)
INSERT [dbo].[BitacoraEventos] ([Id], [Username], [Fecha], [Modulo], [Descripcion], [Criticidad]) VALUES (254, N'46948668Maximo', CAST(N'2026-05-31T21:09:31.147' AS DateTime), N'Usuarios', N'Login exitoso: 46948668Maximo', 3)
INSERT [dbo].[BitacoraEventos] ([Id], [Username], [Fecha], [Modulo], [Descripcion], [Criticidad]) VALUES (255, N'46948668Maximo', CAST(N'2026-05-31T21:16:09.167' AS DateTime), N'Usuarios', N'Login exitoso: 46948668Maximo', 3)
INSERT [dbo].[BitacoraEventos] ([Id], [Username], [Fecha], [Modulo], [Descripcion], [Criticidad]) VALUES (256, N'46948668Maximo', CAST(N'2026-06-01T11:29:15.917' AS DateTime), N'Usuarios', N'Login exitoso: 46948668Maximo', 3)
INSERT [dbo].[BitacoraEventos] ([Id], [Username], [Fecha], [Modulo], [Descripcion], [Criticidad]) VALUES (257, N'46948668Maximo', CAST(N'2026-06-01T11:29:33.987' AS DateTime), N'Usuarios', N'Logout exitoso: 46948668Maximo', 3)
INSERT [dbo].[BitacoraEventos] ([Id], [Username], [Fecha], [Modulo], [Descripcion], [Criticidad]) VALUES (258, N'46948668Maximo', CAST(N'2026-06-01T11:29:47.020' AS DateTime), N'Usuarios', N'Login exitoso: 46948668Maximo', 3)
INSERT [dbo].[BitacoraEventos] ([Id], [Username], [Fecha], [Modulo], [Descripcion], [Criticidad]) VALUES (259, N'46948668Maximo', CAST(N'2026-06-01T11:30:04.150' AS DateTime), N'Usuarios', N'Logout exitoso: 46948668Maximo', 3)
INSERT [dbo].[BitacoraEventos] ([Id], [Username], [Fecha], [Modulo], [Descripcion], [Criticidad]) VALUES (260, N'46948668Maximo', CAST(N'2026-06-01T13:48:58.373' AS DateTime), N'Usuarios', N'Login exitoso: 46948668Maximo', 3)
INSERT [dbo].[BitacoraEventos] ([Id], [Username], [Fecha], [Modulo], [Descripcion], [Criticidad]) VALUES (261, N'46948668Maximo', CAST(N'2026-06-01T13:52:07.510' AS DateTime), N'Usuarios', N'Logout exitoso: 46948668Maximo', 3)
INSERT [dbo].[BitacoraEventos] ([Id], [Username], [Fecha], [Modulo], [Descripcion], [Criticidad]) VALUES (262, N'46948668Maximo', CAST(N'2026-06-01T13:54:07.123' AS DateTime), N'Usuarios', N'Login exitoso: 46948668Maximo', 3)
INSERT [dbo].[BitacoraEventos] ([Id], [Username], [Fecha], [Modulo], [Descripcion], [Criticidad]) VALUES (263, N'46948668Maximo', CAST(N'2026-06-01T13:55:37.127' AS DateTime), N'Usuarios', N'Logout exitoso: 46948668Maximo', 3)
INSERT [dbo].[BitacoraEventos] ([Id], [Username], [Fecha], [Modulo], [Descripcion], [Criticidad]) VALUES (264, N'46905013Lucas', CAST(N'2026-06-01T13:57:57.947' AS DateTime), N'Usuarios', N'Login exitoso: 46905013Lucas', 3)
INSERT [dbo].[BitacoraEventos] ([Id], [Username], [Fecha], [Modulo], [Descripcion], [Criticidad]) VALUES (265, N'46948668Maximo', CAST(N'2026-06-02T23:02:11.567' AS DateTime), N'Usuarios', N'Login exitoso: 46948668Maximo', 3)
INSERT [dbo].[BitacoraEventos] ([Id], [Username], [Fecha], [Modulo], [Descripcion], [Criticidad]) VALUES (266, N'46948668Maximo', CAST(N'2026-06-02T23:02:36.587' AS DateTime), N'Usuarios', N'Logout exitoso: 46948668Maximo', 3)
INSERT [dbo].[BitacoraEventos] ([Id], [Username], [Fecha], [Modulo], [Descripcion], [Criticidad]) VALUES (267, N'46905013Lucas', CAST(N'2026-06-02T23:03:01.313' AS DateTime), N'Usuarios', N'Login exitoso: 46905013Lucas', 3)
INSERT [dbo].[BitacoraEventos] ([Id], [Username], [Fecha], [Modulo], [Descripcion], [Criticidad]) VALUES (268, N'46905013Lucas', CAST(N'2026-06-02T23:03:59.200' AS DateTime), N'Usuarios', N'Logout exitoso: 46905013Lucas', 3)
INSERT [dbo].[BitacoraEventos] ([Id], [Username], [Fecha], [Modulo], [Descripcion], [Criticidad]) VALUES (269, N'46948668Maximo', CAST(N'2026-06-11T02:11:10.813' AS DateTime), N'Usuarios', N'Login exitoso: 46948668Maximo', 3)
INSERT [dbo].[BitacoraEventos] ([Id], [Username], [Fecha], [Modulo], [Descripcion], [Criticidad]) VALUES (270, N'46948668Maximo', CAST(N'2026-06-11T02:11:36.050' AS DateTime), N'Usuarios', N'Logout exitoso: 46948668Maximo', 3)
INSERT [dbo].[BitacoraEventos] ([Id], [Username], [Fecha], [Modulo], [Descripcion], [Criticidad]) VALUES (271, N'46905013Lucas', CAST(N'2026-06-11T02:14:38.757' AS DateTime), N'Usuarios', N'Login exitoso: 46905013Lucas', 3)
INSERT [dbo].[BitacoraEventos] ([Id], [Username], [Fecha], [Modulo], [Descripcion], [Criticidad]) VALUES (272, N'46905013Lucas', CAST(N'2026-06-11T02:27:01.283' AS DateTime), N'Usuarios', N'Login exitoso: 46905013Lucas', 3)
INSERT [dbo].[BitacoraEventos] ([Id], [Username], [Fecha], [Modulo], [Descripcion], [Criticidad]) VALUES (273, N'46905013Lucas', CAST(N'2026-06-11T02:28:32.483' AS DateTime), N'Usuarios', N'Logout exitoso: 46905013Lucas', 3)
INSERT [dbo].[BitacoraEventos] ([Id], [Username], [Fecha], [Modulo], [Descripcion], [Criticidad]) VALUES (274, N'46905013Lucas', CAST(N'2026-06-16T01:14:55.770' AS DateTime), N'Usuarios', N'Login exitoso: 46905013Lucas', 3)
INSERT [dbo].[BitacoraEventos] ([Id], [Username], [Fecha], [Modulo], [Descripcion], [Criticidad]) VALUES (275, N'46905013Lucas', CAST(N'2026-06-16T01:20:29.370' AS DateTime), N'Usuarios', N'Logout exitoso: 46905013Lucas', 3)
INSERT [dbo].[BitacoraEventos] ([Id], [Username], [Fecha], [Modulo], [Descripcion], [Criticidad]) VALUES (276, N'46905013Lucas', CAST(N'2026-06-16T01:35:15.813' AS DateTime), N'Usuarios', N'Login exitoso: 46905013Lucas', 3)
INSERT [dbo].[BitacoraEventos] ([Id], [Username], [Fecha], [Modulo], [Descripcion], [Criticidad]) VALUES (277, N'46905013Lucas', CAST(N'2026-06-16T01:36:45.277' AS DateTime), N'Usuarios', N'Usuario modificado: DNI 46948668. Email: MaximoKirichuk@gmail.com. Rol: SuperAdmin. Actor: 46905013Lucas', 1)
INSERT [dbo].[BitacoraEventos] ([Id], [Username], [Fecha], [Modulo], [Descripcion], [Criticidad]) VALUES (278, N'46905013Lucas', CAST(N'2026-06-16T01:38:49.587' AS DateTime), N'Usuarios', N'Logout exitoso: 46905013Lucas', 3)
INSERT [dbo].[BitacoraEventos] ([Id], [Username], [Fecha], [Modulo], [Descripcion], [Criticidad]) VALUES (279, N'46948668Maximo', CAST(N'2026-06-16T01:39:18.380' AS DateTime), N'Usuarios', N'Login exitoso: 46948668Maximo', 3)
INSERT [dbo].[BitacoraEventos] ([Id], [Username], [Fecha], [Modulo], [Descripcion], [Criticidad]) VALUES (280, N'46948668Maximo', CAST(N'2026-06-16T01:47:39.420' AS DateTime), N'Usuarios', N'Logout exitoso: 46948668Maximo', 3)
INSERT [dbo].[BitacoraEventos] ([Id], [Username], [Fecha], [Modulo], [Descripcion], [Criticidad]) VALUES (281, N'46948668Maximo', CAST(N'2026-06-16T01:55:19.730' AS DateTime), N'Usuarios', N'Login exitoso: 46948668Maximo', 3)
INSERT [dbo].[BitacoraEventos] ([Id], [Username], [Fecha], [Modulo], [Descripcion], [Criticidad]) VALUES (282, N'46948668Maximo', CAST(N'2026-06-16T01:56:15.153' AS DateTime), N'Usuarios', N'Logout exitoso: 46948668Maximo', 3)
INSERT [dbo].[BitacoraEventos] ([Id], [Username], [Fecha], [Modulo], [Descripcion], [Criticidad]) VALUES (283, N'46948668Maximo', CAST(N'2026-06-16T02:35:35.180' AS DateTime), N'Usuarios', N'Login exitoso: 46948668Maximo', 3)
INSERT [dbo].[BitacoraEventos] ([Id], [Username], [Fecha], [Modulo], [Descripcion], [Criticidad]) VALUES (284, N'46948668Maximo', CAST(N'2026-06-16T02:36:45.280' AS DateTime), N'Usuarios', N'Logout exitoso: 46948668Maximo', 3)
INSERT [dbo].[BitacoraEventos] ([Id], [Username], [Fecha], [Modulo], [Descripcion], [Criticidad]) VALUES (285, N'46948668Maximo', CAST(N'2026-06-20T18:46:48.567' AS DateTime), N'Usuarios', N'Login exitoso: 46948668Maximo', 3)
INSERT [dbo].[BitacoraEventos] ([Id], [Username], [Fecha], [Modulo], [Descripcion], [Criticidad]) VALUES (286, N'46948668Maximo', CAST(N'2026-06-20T18:52:03.633' AS DateTime), N'Usuarios', N'Logout exitoso: 46948668Maximo', 3)
INSERT [dbo].[BitacoraEventos] ([Id], [Username], [Fecha], [Modulo], [Descripcion], [Criticidad]) VALUES (287, N'46948668Maximo', CAST(N'2026-06-20T18:57:50.967' AS DateTime), N'Usuarios', N'Login exitoso: 46948668Maximo', 3)
INSERT [dbo].[BitacoraEventos] ([Id], [Username], [Fecha], [Modulo], [Descripcion], [Criticidad]) VALUES (288, N'46948668Maximo', CAST(N'2026-06-20T19:03:14.263' AS DateTime), N'Usuarios', N'Logout exitoso: 46948668Maximo', 3)
INSERT [dbo].[BitacoraEventos] ([Id], [Username], [Fecha], [Modulo], [Descripcion], [Criticidad]) VALUES (289, N'46948668Maximo', CAST(N'2026-06-20T19:08:23.233' AS DateTime), N'Usuarios', N'Login exitoso: 46948668Maximo', 3)
INSERT [dbo].[BitacoraEventos] ([Id], [Username], [Fecha], [Modulo], [Descripcion], [Criticidad]) VALUES (290, N'46948668Maximo', CAST(N'2026-06-20T19:23:05.677' AS DateTime), N'Usuarios', N'Login exitoso: 46948668Maximo', 3)
INSERT [dbo].[BitacoraEventos] ([Id], [Username], [Fecha], [Modulo], [Descripcion], [Criticidad]) VALUES (291, N'46948668Maximo', CAST(N'2026-06-20T20:03:09.520' AS DateTime), N'Usuarios', N'Login exitoso: 46948668Maximo', 3)
INSERT [dbo].[BitacoraEventos] ([Id], [Username], [Fecha], [Modulo], [Descripcion], [Criticidad]) VALUES (292, N'46948668Maximo', CAST(N'2026-06-20T20:05:54.843' AS DateTime), N'Usuarios', N'Logout exitoso: 46948668Maximo', 3)
INSERT [dbo].[BitacoraEventos] ([Id], [Username], [Fecha], [Modulo], [Descripcion], [Criticidad]) VALUES (293, N'46948668Maximo', CAST(N'2026-06-20T20:06:39.157' AS DateTime), N'Usuarios', N'Login exitoso: 46948668Maximo', 3)
INSERT [dbo].[BitacoraEventos] ([Id], [Username], [Fecha], [Modulo], [Descripcion], [Criticidad]) VALUES (294, N'46948668Maximo', CAST(N'2026-06-20T20:07:07.817' AS DateTime), N'Admin', N'Nueva familia creada: Nuevo', 3)
INSERT [dbo].[BitacoraEventos] ([Id], [Username], [Fecha], [Modulo], [Descripcion], [Criticidad]) VALUES (295, N'46948668Maximo', CAST(N'2026-06-20T20:11:30.253' AS DateTime), N'Usuarios', N'Logout exitoso: 46948668Maximo', 3)
INSERT [dbo].[BitacoraEventos] ([Id], [Username], [Fecha], [Modulo], [Descripcion], [Criticidad]) VALUES (296, N'46948668Maximo', CAST(N'2026-06-20T20:23:24.760' AS DateTime), N'Usuarios', N'Login exitoso: 46948668Maximo', 3)
INSERT [dbo].[BitacoraEventos] ([Id], [Username], [Fecha], [Modulo], [Descripcion], [Criticidad]) VALUES (297, N'46948668Maximo', CAST(N'2026-06-20T20:30:32.670' AS DateTime), N'Usuarios', N'Login exitoso: 46948668Maximo', 3)
INSERT [dbo].[BitacoraEventos] ([Id], [Username], [Fecha], [Modulo], [Descripcion], [Criticidad]) VALUES (298, N'46948668Maximo', CAST(N'2026-06-20T20:33:58.923' AS DateTime), N'Admin', N'Nueva familia creada: CrearROL PRUBEA', 3)
INSERT [dbo].[BitacoraEventos] ([Id], [Username], [Fecha], [Modulo], [Descripcion], [Criticidad]) VALUES (299, N'46948668Maximo', CAST(N'2026-06-20T20:34:29.077' AS DateTime), N'Usuarios', N'Logout exitoso: 46948668Maximo', 3)
INSERT [dbo].[BitacoraEventos] ([Id], [Username], [Fecha], [Modulo], [Descripcion], [Criticidad]) VALUES (300, N'46948668Maximo', CAST(N'2026-06-20T20:35:56.663' AS DateTime), N'Usuarios', N'Login exitoso: 46948668Maximo', 3)
INSERT [dbo].[BitacoraEventos] ([Id], [Username], [Fecha], [Modulo], [Descripcion], [Criticidad]) VALUES (301, N'46948668Maximo', CAST(N'2026-06-20T20:40:26.693' AS DateTime), N'Usuarios', N'Logout exitoso: 46948668Maximo', 3)
GO
INSERT [dbo].[BitacoraEventos] ([Id], [Username], [Fecha], [Modulo], [Descripcion], [Criticidad]) VALUES (302, N'46948668Maximo', CAST(N'2026-06-20T20:41:29.023' AS DateTime), N'Usuarios', N'Login exitoso: 46948668Maximo', 3)
INSERT [dbo].[BitacoraEventos] ([Id], [Username], [Fecha], [Modulo], [Descripcion], [Criticidad]) VALUES (303, N'46948668Maximo', CAST(N'2026-06-20T20:48:29.673' AS DateTime), N'Usuarios', N'Login exitoso: 46948668Maximo', 3)
INSERT [dbo].[BitacoraEventos] ([Id], [Username], [Fecha], [Modulo], [Descripcion], [Criticidad]) VALUES (304, N'46948668Maximo', CAST(N'2026-06-20T20:55:24.547' AS DateTime), N'Usuarios', N'Logout exitoso: 46948668Maximo', 3)
INSERT [dbo].[BitacoraEventos] ([Id], [Username], [Fecha], [Modulo], [Descripcion], [Criticidad]) VALUES (305, N'46948668Maximo', CAST(N'2026-06-20T21:13:01.950' AS DateTime), N'Usuarios', N'Login exitoso: 46948668Maximo', 3)
INSERT [dbo].[BitacoraEventos] ([Id], [Username], [Fecha], [Modulo], [Descripcion], [Criticidad]) VALUES (306, N'46948668Maximo', CAST(N'2026-06-20T23:37:32.717' AS DateTime), N'Usuarios', N'Login exitoso: 46948668Maximo', 3)
INSERT [dbo].[BitacoraEventos] ([Id], [Username], [Fecha], [Modulo], [Descripcion], [Criticidad]) VALUES (307, N'46948668Maximo', CAST(N'2026-06-20T23:38:14.823' AS DateTime), N'Usuarios', N'Logout exitoso: 46948668Maximo', 3)
INSERT [dbo].[BitacoraEventos] ([Id], [Username], [Fecha], [Modulo], [Descripcion], [Criticidad]) VALUES (308, N'46948668Maximo', CAST(N'2026-06-21T00:35:18.640' AS DateTime), N'Usuarios', N'Login exitoso: 46948668Maximo', 3)
INSERT [dbo].[BitacoraEventos] ([Id], [Username], [Fecha], [Modulo], [Descripcion], [Criticidad]) VALUES (309, N'46948668Maximo', CAST(N'2026-06-21T00:38:07.083' AS DateTime), N'Admin', N'Nueva familia creada: Hijo', 3)
INSERT [dbo].[BitacoraEventos] ([Id], [Username], [Fecha], [Modulo], [Descripcion], [Criticidad]) VALUES (310, N'46948668Maximo', CAST(N'2026-06-21T00:38:21.087' AS DateTime), N'Admin', N'Nueva familia creada: Padre', 3)
INSERT [dbo].[BitacoraEventos] ([Id], [Username], [Fecha], [Modulo], [Descripcion], [Criticidad]) VALUES (311, N'46948668Maximo', CAST(N'2026-06-21T00:38:47.940' AS DateTime), N'Admin', N'Nueva familia creada: Abuelo', 3)
INSERT [dbo].[BitacoraEventos] ([Id], [Username], [Fecha], [Modulo], [Descripcion], [Criticidad]) VALUES (312, N'46948668Maximo', CAST(N'2026-06-21T00:40:39.677' AS DateTime), N'Usuarios', N'Logout exitoso: 46948668Maximo', 3)
INSERT [dbo].[BitacoraEventos] ([Id], [Username], [Fecha], [Modulo], [Descripcion], [Criticidad]) VALUES (313, N'46948668Maximo', CAST(N'2026-06-21T03:40:40.173' AS DateTime), N'Usuarios', N'Login exitoso: 46948668Maximo', 3)
INSERT [dbo].[BitacoraEventos] ([Id], [Username], [Fecha], [Modulo], [Descripcion], [Criticidad]) VALUES (314, N'46948668Maximo', CAST(N'2026-06-21T03:45:43.080' AS DateTime), N'Usuarios', N'Logout exitoso: 46948668Maximo', 3)
INSERT [dbo].[BitacoraEventos] ([Id], [Username], [Fecha], [Modulo], [Descripcion], [Criticidad]) VALUES (315, N'46948668Maximo', CAST(N'2026-06-21T04:27:45.983' AS DateTime), N'Usuarios', N'Login exitoso: 46948668Maximo', 3)
INSERT [dbo].[BitacoraEventos] ([Id], [Username], [Fecha], [Modulo], [Descripcion], [Criticidad]) VALUES (316, N'46948668Maximo', CAST(N'2026-06-21T04:28:36.860' AS DateTime), N'Usuarios', N'Logout exitoso: 46948668Maximo', 3)
INSERT [dbo].[BitacoraEventos] ([Id], [Username], [Fecha], [Modulo], [Descripcion], [Criticidad]) VALUES (317, N'46948668Maximo', CAST(N'2026-06-21T20:41:49.933' AS DateTime), N'Usuarios', N'Login exitoso: 46948668Maximo', 3)
INSERT [dbo].[BitacoraEventos] ([Id], [Username], [Fecha], [Modulo], [Descripcion], [Criticidad]) VALUES (318, N'46948668Maximo', CAST(N'2026-06-21T20:42:15.523' AS DateTime), N'Admin', N'Nuevo rol creado: Prueba', 3)
INSERT [dbo].[BitacoraEventos] ([Id], [Username], [Fecha], [Modulo], [Descripcion], [Criticidad]) VALUES (319, N'46948668Maximo', CAST(N'2026-06-21T20:47:57.910' AS DateTime), N'Usuarios', N'Logout exitoso: 46948668Maximo', 3)
INSERT [dbo].[BitacoraEventos] ([Id], [Username], [Fecha], [Modulo], [Descripcion], [Criticidad]) VALUES (320, N'46948668Maximo', CAST(N'2026-06-21T20:48:30.740' AS DateTime), N'Usuarios', N'Login exitoso: 46948668Maximo', 3)
INSERT [dbo].[BitacoraEventos] ([Id], [Username], [Fecha], [Modulo], [Descripcion], [Criticidad]) VALUES (321, N'46948668Maximo', CAST(N'2026-06-21T20:55:45.363' AS DateTime), N'Admin', N'Nuevo rol creado: Prueba', 3)
INSERT [dbo].[BitacoraEventos] ([Id], [Username], [Fecha], [Modulo], [Descripcion], [Criticidad]) VALUES (322, N'46948668Maximo', CAST(N'2026-06-21T20:58:37.277' AS DateTime), N'Admin', N'Nuevo rol creado: AdminPRUEBA', 3)
INSERT [dbo].[BitacoraEventos] ([Id], [Username], [Fecha], [Modulo], [Descripcion], [Criticidad]) VALUES (323, N'46948668Maximo', CAST(N'2026-06-21T21:00:49.413' AS DateTime), N'Usuarios', N'Logout exitoso: 46948668Maximo', 3)
INSERT [dbo].[BitacoraEventos] ([Id], [Username], [Fecha], [Modulo], [Descripcion], [Criticidad]) VALUES (324, N'46948668Maximo', CAST(N'2026-06-21T21:13:28.570' AS DateTime), N'Usuarios', N'Login exitoso: 46948668Maximo', 3)
INSERT [dbo].[BitacoraEventos] ([Id], [Username], [Fecha], [Modulo], [Descripcion], [Criticidad]) VALUES (325, N'46948668Maximo', CAST(N'2026-06-21T21:17:17.267' AS DateTime), N'Usuarios', N'Logout exitoso: 46948668Maximo', 3)
INSERT [dbo].[BitacoraEventos] ([Id], [Username], [Fecha], [Modulo], [Descripcion], [Criticidad]) VALUES (326, N'46948668Maximo', CAST(N'2026-06-21T21:21:28.133' AS DateTime), N'Usuarios', N'Login exitoso: 46948668Maximo', 3)
INSERT [dbo].[BitacoraEventos] ([Id], [Username], [Fecha], [Modulo], [Descripcion], [Criticidad]) VALUES (327, N'46948668Maximo', CAST(N'2026-06-21T21:24:02.663' AS DateTime), N'Usuarios', N'Logout exitoso: 46948668Maximo', 3)
INSERT [dbo].[BitacoraEventos] ([Id], [Username], [Fecha], [Modulo], [Descripcion], [Criticidad]) VALUES (328, N'46948668Maximo', CAST(N'2026-06-22T02:42:13.687' AS DateTime), N'Usuarios', N'Login exitoso: 46948668Maximo', 3)
INSERT [dbo].[BitacoraEventos] ([Id], [Username], [Fecha], [Modulo], [Descripcion], [Criticidad]) VALUES (329, N'46948668Maximo', CAST(N'2026-06-22T02:42:34.743' AS DateTime), N'Usuarios', N'Logout exitoso: 46948668Maximo', 3)
INSERT [dbo].[BitacoraEventos] ([Id], [Username], [Fecha], [Modulo], [Descripcion], [Criticidad]) VALUES (330, N'46948668Maximo', CAST(N'2026-06-22T02:49:33.153' AS DateTime), N'Usuarios', N'Login exitoso: 46948668Maximo', 3)
INSERT [dbo].[BitacoraEventos] ([Id], [Username], [Fecha], [Modulo], [Descripcion], [Criticidad]) VALUES (331, N'46948668Maximo', CAST(N'2026-06-22T02:50:36.450' AS DateTime), N'Usuarios', N'Logout exitoso: 46948668Maximo', 3)
INSERT [dbo].[BitacoraEventos] ([Id], [Username], [Fecha], [Modulo], [Descripcion], [Criticidad]) VALUES (332, N'46948668Maximo', CAST(N'2026-06-22T02:50:54.073' AS DateTime), N'Usuarios', N'Login exitoso: 46948668Maximo', 3)
INSERT [dbo].[BitacoraEventos] ([Id], [Username], [Fecha], [Modulo], [Descripcion], [Criticidad]) VALUES (333, N'46948668Maximo', CAST(N'2026-06-22T02:51:08.093' AS DateTime), N'Usuarios', N'Logout exitoso: 46948668Maximo', 3)
INSERT [dbo].[BitacoraEventos] ([Id], [Username], [Fecha], [Modulo], [Descripcion], [Criticidad]) VALUES (334, N'46948668Maximo', CAST(N'2026-06-22T02:51:34.843' AS DateTime), N'Usuarios', N'Login exitoso: 46948668Maximo', 3)
INSERT [dbo].[BitacoraEventos] ([Id], [Username], [Fecha], [Modulo], [Descripcion], [Criticidad]) VALUES (335, N'46948668Maximo', CAST(N'2026-06-22T02:53:01.797' AS DateTime), N'Usuarios', N'Login exitoso: 46948668Maximo', 3)
INSERT [dbo].[BitacoraEventos] ([Id], [Username], [Fecha], [Modulo], [Descripcion], [Criticidad]) VALUES (336, N'46948668Maximo', CAST(N'2026-06-22T02:53:32.523' AS DateTime), N'Usuarios', N'Logout exitoso: 46948668Maximo', 3)
INSERT [dbo].[BitacoraEventos] ([Id], [Username], [Fecha], [Modulo], [Descripcion], [Criticidad]) VALUES (337, N'46905013Lucas', CAST(N'2026-06-22T03:26:45.710' AS DateTime), N'Usuarios', N'Login exitoso: 46905013Lucas', 3)
INSERT [dbo].[BitacoraEventos] ([Id], [Username], [Fecha], [Modulo], [Descripcion], [Criticidad]) VALUES (338, N'46905013Lucas', CAST(N'2026-06-22T03:28:07.633' AS DateTime), N'Usuarios', N'Logout exitoso: 46905013Lucas', 3)
INSERT [dbo].[BitacoraEventos] ([Id], [Username], [Fecha], [Modulo], [Descripcion], [Criticidad]) VALUES (339, N'46905013Lucas', CAST(N'2026-06-22T03:29:41.040' AS DateTime), N'Usuarios', N'Login exitoso: 46905013Lucas', 3)
INSERT [dbo].[BitacoraEventos] ([Id], [Username], [Fecha], [Modulo], [Descripcion], [Criticidad]) VALUES (340, N'46905013Lucas', CAST(N'2026-06-22T03:29:45.397' AS DateTime), N'Usuarios', N'Logout exitoso: 46905013Lucas', 3)
INSERT [dbo].[BitacoraEventos] ([Id], [Username], [Fecha], [Modulo], [Descripcion], [Criticidad]) VALUES (341, N'46905013Lucas', CAST(N'2026-06-22T03:30:15.523' AS DateTime), N'Usuarios', N'Login exitoso: 46905013Lucas', 3)
INSERT [dbo].[BitacoraEventos] ([Id], [Username], [Fecha], [Modulo], [Descripcion], [Criticidad]) VALUES (342, N'46905013Lucas', CAST(N'2026-06-22T03:32:28.913' AS DateTime), N'Usuarios', N'Login exitoso: 46905013Lucas', 3)
INSERT [dbo].[BitacoraEventos] ([Id], [Username], [Fecha], [Modulo], [Descripcion], [Criticidad]) VALUES (343, N'46905013Lucas', CAST(N'2026-06-22T03:32:43.830' AS DateTime), N'Usuarios', N'Logout exitoso: 46905013Lucas', 3)
INSERT [dbo].[BitacoraEventos] ([Id], [Username], [Fecha], [Modulo], [Descripcion], [Criticidad]) VALUES (344, N'46905013Lucas', CAST(N'2026-06-22T03:59:53.353' AS DateTime), N'Usuarios', N'Login exitoso: 46905013Lucas', 3)
INSERT [dbo].[BitacoraEventos] ([Id], [Username], [Fecha], [Modulo], [Descripcion], [Criticidad]) VALUES (345, N'46948668Maximo', CAST(N'2026-06-22T04:30:25.950' AS DateTime), N'Usuarios', N'Login exitoso: 46948668Maximo', 3)
INSERT [dbo].[BitacoraEventos] ([Id], [Username], [Fecha], [Modulo], [Descripcion], [Criticidad]) VALUES (346, N'46948668Maximo', CAST(N'2026-06-22T04:30:38.183' AS DateTime), N'Usuarios', N'Logout exitoso: 46948668Maximo', 3)
INSERT [dbo].[BitacoraEventos] ([Id], [Username], [Fecha], [Modulo], [Descripcion], [Criticidad]) VALUES (347, N'46948668Maximo', CAST(N'2026-06-22T04:31:31.787' AS DateTime), N'Usuarios', N'Login exitoso: 46948668Maximo', 3)
INSERT [dbo].[BitacoraEventos] ([Id], [Username], [Fecha], [Modulo], [Descripcion], [Criticidad]) VALUES (348, N'46948668Maximo', CAST(N'2026-06-22T04:31:58.377' AS DateTime), N'Usuarios', N'Logout exitoso: 46948668Maximo', 3)
INSERT [dbo].[BitacoraEventos] ([Id], [Username], [Fecha], [Modulo], [Descripcion], [Criticidad]) VALUES (349, N'46948668Maximo', CAST(N'2026-06-22T04:36:28.160' AS DateTime), N'Usuarios', N'Login exitoso: 46948668Maximo', 3)
INSERT [dbo].[BitacoraEventos] ([Id], [Username], [Fecha], [Modulo], [Descripcion], [Criticidad]) VALUES (350, N'46948668Maximo', CAST(N'2026-06-22T04:42:38.783' AS DateTime), N'Usuarios', N'Logout exitoso: 46948668Maximo', 3)
INSERT [dbo].[BitacoraEventos] ([Id], [Username], [Fecha], [Modulo], [Descripcion], [Criticidad]) VALUES (351, N'46948668Maximo', CAST(N'2026-06-22T16:25:04.233' AS DateTime), N'Usuarios', N'Login exitoso: 46948668Maximo', 3)
INSERT [dbo].[BitacoraEventos] ([Id], [Username], [Fecha], [Modulo], [Descripcion], [Criticidad]) VALUES (352, N'46948668Maximo', CAST(N'2026-06-22T16:26:21.873' AS DateTime), N'Admin', N'Familia modificada: Administrador (Actor: 46948668Maximo)', 3)
INSERT [dbo].[BitacoraEventos] ([Id], [Username], [Fecha], [Modulo], [Descripcion], [Criticidad]) VALUES (353, N'46948668Maximo', CAST(N'2026-06-22T16:27:20.003' AS DateTime), N'Admin', N'Familia modificada: Administrador (Actor: 46948668Maximo)', 3)
INSERT [dbo].[BitacoraEventos] ([Id], [Username], [Fecha], [Modulo], [Descripcion], [Criticidad]) VALUES (354, N'46948668Maximo', CAST(N'2026-06-22T16:29:11.663' AS DateTime), N'Admin', N'Rol eliminado: AdminPRUEBA (Codigo: 10) (Actor: 46948668Maximo)', 3)
INSERT [dbo].[BitacoraEventos] ([Id], [Username], [Fecha], [Modulo], [Descripcion], [Criticidad]) VALUES (355, N'46948668Maximo', CAST(N'2026-06-22T16:30:16.033' AS DateTime), N'Usuarios', N'Logout exitoso: 46948668Maximo', 3)
INSERT [dbo].[BitacoraEventos] ([Id], [Username], [Fecha], [Modulo], [Descripcion], [Criticidad]) VALUES (356, N'46948668Maximo', CAST(N'2026-06-22T16:37:05.247' AS DateTime), N'Usuarios', N'Login exitoso: 46948668Maximo', 3)
INSERT [dbo].[BitacoraEventos] ([Id], [Username], [Fecha], [Modulo], [Descripcion], [Criticidad]) VALUES (357, N'46948668Maximo', CAST(N'2026-06-22T16:37:46.870' AS DateTime), N'Admin', N'Familia eliminada: Abuelo (Codigo: 17) (Actor: 46948668Maximo)', 3)
INSERT [dbo].[BitacoraEventos] ([Id], [Username], [Fecha], [Modulo], [Descripcion], [Criticidad]) VALUES (358, N'46948668Maximo', CAST(N'2026-06-22T19:54:10.390' AS DateTime), N'Usuarios', N'Login exitoso: 46948668Maximo', 3)
INSERT [dbo].[BitacoraEventos] ([Id], [Username], [Fecha], [Modulo], [Descripcion], [Criticidad]) VALUES (359, N'46948668Maximo', CAST(N'2026-06-22T19:54:15.120' AS DateTime), N'Usuarios', N'Logout exitoso: 46948668Maximo', 3)
INSERT [dbo].[BitacoraEventos] ([Id], [Username], [Fecha], [Modulo], [Descripcion], [Criticidad]) VALUES (360, N'46948668Maximo', CAST(N'2026-06-22T19:57:43.667' AS DateTime), N'Usuarios', N'Login exitoso: 46948668Maximo', 3)
INSERT [dbo].[BitacoraEventos] ([Id], [Username], [Fecha], [Modulo], [Descripcion], [Criticidad]) VALUES (361, N'46948668Maximo', CAST(N'2026-06-22T19:58:20.190' AS DateTime), N'Admin', N'Contraseña modificada: 46948668Maximo (Actor: 46948668Maximo)', 1)
INSERT [dbo].[BitacoraEventos] ([Id], [Username], [Fecha], [Modulo], [Descripcion], [Criticidad]) VALUES (362, N'46948668Maximo', CAST(N'2026-06-22T20:03:12.120' AS DateTime), N'Usuarios', N'Logout exitoso: 46948668Maximo', 3)
INSERT [dbo].[BitacoraEventos] ([Id], [Username], [Fecha], [Modulo], [Descripcion], [Criticidad]) VALUES (363, N'46948668Maximo', CAST(N'2026-06-22T20:13:54.237' AS DateTime), N'Usuarios', N'Intento fallido de login: 46948668Maximo', 1)
INSERT [dbo].[BitacoraEventos] ([Id], [Username], [Fecha], [Modulo], [Descripcion], [Criticidad]) VALUES (364, N'46948668Maximo', CAST(N'2026-06-22T20:13:57.133' AS DateTime), N'Usuarios', N'Intento fallido de login: 46948668Maximo', 1)
INSERT [dbo].[BitacoraEventos] ([Id], [Username], [Fecha], [Modulo], [Descripcion], [Criticidad]) VALUES (365, N'46948668Maximo', CAST(N'2026-06-22T20:14:00.857' AS DateTime), N'Usuarios', N'Intento fallido de login: 46948668Maximo', 1)
INSERT [dbo].[BitacoraEventos] ([Id], [Username], [Fecha], [Modulo], [Descripcion], [Criticidad]) VALUES (366, N'46948668Maximo', CAST(N'2026-06-22T20:14:00.857' AS DateTime), N'Usuarios', N'Usuario bloqueado por intentos fallidos: 46948668Maximo', 1)
INSERT [dbo].[BitacoraEventos] ([Id], [Username], [Fecha], [Modulo], [Descripcion], [Criticidad]) VALUES (367, N'46905013Lucas', CAST(N'2026-06-22T20:14:19.507' AS DateTime), N'Usuarios', N'Login exitoso: 46905013Lucas', 3)
INSERT [dbo].[BitacoraEventos] ([Id], [Username], [Fecha], [Modulo], [Descripcion], [Criticidad]) VALUES (368, N'46948668Maximo', CAST(N'2026-06-22T20:14:31.750' AS DateTime), N'Admin', N'Usuario desbloqueado: 46948668Maximo (Rol: SuperAdmin) (Actor: 46905013Lucas)', 1)
INSERT [dbo].[BitacoraEventos] ([Id], [Username], [Fecha], [Modulo], [Descripcion], [Criticidad]) VALUES (369, N'46905013Lucas', CAST(N'2026-06-22T20:15:16.873' AS DateTime), N'Usuarios', N'Logout exitoso: 46905013Lucas', 3)
INSERT [dbo].[BitacoraEventos] ([Id], [Username], [Fecha], [Modulo], [Descripcion], [Criticidad]) VALUES (370, N'46948668Maximo', CAST(N'2026-06-22T20:15:25.267' AS DateTime), N'Usuarios', N'Login exitoso: 46948668Maximo', 3)
INSERT [dbo].[BitacoraEventos] ([Id], [Username], [Fecha], [Modulo], [Descripcion], [Criticidad]) VALUES (371, N'46948668Maximo', CAST(N'2026-06-22T20:15:28.050' AS DateTime), N'Usuarios', N'Logout exitoso: 46948668Maximo', 3)
INSERT [dbo].[BitacoraEventos] ([Id], [Username], [Fecha], [Modulo], [Descripcion], [Criticidad]) VALUES (372, N'46948668Maximo', CAST(N'2026-06-22T20:15:35.477' AS DateTime), N'Usuarios', N'Login exitoso: 46948668Maximo', 3)
INSERT [dbo].[BitacoraEventos] ([Id], [Username], [Fecha], [Modulo], [Descripcion], [Criticidad]) VALUES (373, N'46948668Maximo', CAST(N'2026-06-22T20:15:49.463' AS DateTime), N'Usuarios', N'Logout exitoso: 46948668Maximo', 3)
INSERT [dbo].[BitacoraEventos] ([Id], [Username], [Fecha], [Modulo], [Descripcion], [Criticidad]) VALUES (374, N'46948668Maximo', CAST(N'2026-06-22T20:15:58.953' AS DateTime), N'Usuarios', N'Login exitoso: 46948668Maximo', 3)
INSERT [dbo].[BitacoraEventos] ([Id], [Username], [Fecha], [Modulo], [Descripcion], [Criticidad]) VALUES (375, N'46948668Maximo', CAST(N'2026-06-22T20:16:36.770' AS DateTime), N'Usuarios', N'Contraseña modificada: 46948668Maximo (Actor: 46948668Maximo)', 1)
INSERT [dbo].[BitacoraEventos] ([Id], [Username], [Fecha], [Modulo], [Descripcion], [Criticidad]) VALUES (376, N'46948668Maximo', CAST(N'2026-06-22T20:17:55.493' AS DateTime), N'Admin', N'Bitácora exportada a PDF: 91 eventos', 3)
INSERT [dbo].[BitacoraEventos] ([Id], [Username], [Fecha], [Modulo], [Descripcion], [Criticidad]) VALUES (377, N'46948668Maximo', CAST(N'2026-06-23T17:55:16.127' AS DateTime), N'Usuarios', N'Intento fallido de login: 46948668Maximo', 1)
INSERT [dbo].[BitacoraEventos] ([Id], [Username], [Fecha], [Modulo], [Descripcion], [Criticidad]) VALUES (378, N'46948668Maximo', CAST(N'2026-06-23T17:55:20.867' AS DateTime), N'Usuarios', N'Login exitoso: 46948668Maximo', 3)
INSERT [dbo].[BitacoraEventos] ([Id], [Username], [Fecha], [Modulo], [Descripcion], [Criticidad]) VALUES (379, N'46948668Maximo', CAST(N'2026-06-23T17:56:15.233' AS DateTime), N'Admin', N'Nueva familia creada: Abuelo (Actor: 46948668Maximo)', 3)
INSERT [dbo].[BitacoraEventos] ([Id], [Username], [Fecha], [Modulo], [Descripcion], [Criticidad]) VALUES (380, N'46948668Maximo', CAST(N'2026-06-23T17:56:28.910' AS DateTime), N'Admin', N'Familia modificada: Abuelo (Actor: 46948668Maximo)', 3)
INSERT [dbo].[BitacoraEventos] ([Id], [Username], [Fecha], [Modulo], [Descripcion], [Criticidad]) VALUES (381, N'46948668Maximo', CAST(N'2026-06-23T17:57:22.593' AS DateTime), N'Admin', N'Familia eliminada: Abuelo (Codigo: 18) (Actor: 46948668Maximo)', 3)
INSERT [dbo].[BitacoraEventos] ([Id], [Username], [Fecha], [Modulo], [Descripcion], [Criticidad]) VALUES (382, N'46948668Maximo', CAST(N'2026-06-23T17:57:41.553' AS DateTime), N'Admin', N'Familia eliminada: CrearROL PRUBEA (Codigo: 14) (Actor: 46948668Maximo)', 3)
INSERT [dbo].[BitacoraEventos] ([Id], [Username], [Fecha], [Modulo], [Descripcion], [Criticidad]) VALUES (383, N'46948668Maximo', CAST(N'2026-06-23T17:59:40.237' AS DateTime), N'Admin', N'Nuevo rol creado: Rol prueba (Actor: 46948668Maximo)', 3)
INSERT [dbo].[BitacoraEventos] ([Id], [Username], [Fecha], [Modulo], [Descripcion], [Criticidad]) VALUES (384, N'46948668Maximo', CAST(N'2026-06-23T18:00:29.793' AS DateTime), N'Admin', N'Rol modificado: Rol prueba (Actor: 46948668Maximo)', 3)
INSERT [dbo].[BitacoraEventos] ([Id], [Username], [Fecha], [Modulo], [Descripcion], [Criticidad]) VALUES (385, N'46948668Maximo', CAST(N'2026-06-23T18:01:06.510' AS DateTime), N'Admin', N'Rol modificado: Rol prueba (Actor: 46948668Maximo)', 3)
INSERT [dbo].[BitacoraEventos] ([Id], [Username], [Fecha], [Modulo], [Descripcion], [Criticidad]) VALUES (386, N'46948668Maximo', CAST(N'2026-06-23T18:01:38.353' AS DateTime), N'Admin', N'Rol modificado: Rol prueba (Actor: 46948668Maximo)', 3)
INSERT [dbo].[BitacoraEventos] ([Id], [Username], [Fecha], [Modulo], [Descripcion], [Criticidad]) VALUES (387, N'46948668Maximo', CAST(N'2026-06-23T18:02:07.317' AS DateTime), N'Admin', N'Rol eliminado: Rol prueba (Codigo: 11) (Actor: 46948668Maximo)', 3)
INSERT [dbo].[BitacoraEventos] ([Id], [Username], [Fecha], [Modulo], [Descripcion], [Criticidad]) VALUES (388, N'46948668Maximo', CAST(N'2026-06-23T18:03:15.277' AS DateTime), N'Admin', N'Nueva familia creada: abuelo prueba (Actor: 46948668Maximo)', 3)
INSERT [dbo].[BitacoraEventos] ([Id], [Username], [Fecha], [Modulo], [Descripcion], [Criticidad]) VALUES (389, N'46948668Maximo', CAST(N'2026-06-23T18:03:27.040' AS DateTime), N'Admin', N'Familia modificada: abuelo prueba (Actor: 46948668Maximo)', 3)
INSERT [dbo].[BitacoraEventos] ([Id], [Username], [Fecha], [Modulo], [Descripcion], [Criticidad]) VALUES (390, N'46948668Maximo', CAST(N'2026-06-23T18:04:52.410' AS DateTime), N'Usuarios', N'Login exitoso: 46948668Maximo', 3)
INSERT [dbo].[BitacoraEventos] ([Id], [Username], [Fecha], [Modulo], [Descripcion], [Criticidad]) VALUES (391, N'46948668Maximo', CAST(N'2026-06-23T18:12:01.630' AS DateTime), N'Usuarios', N'Login exitoso: 46948668Maximo', 3)
INSERT [dbo].[BitacoraEventos] ([Id], [Username], [Fecha], [Modulo], [Descripcion], [Criticidad]) VALUES (392, N'46948668Maximo', CAST(N'2026-06-23T18:13:27.080' AS DateTime), N'Usuarios', N'Logout exitoso: 46948668Maximo', 3)
INSERT [dbo].[BitacoraEventos] ([Id], [Username], [Fecha], [Modulo], [Descripcion], [Criticidad]) VALUES (393, N'46948668Maximo', CAST(N'2026-06-23T19:24:55.080' AS DateTime), N'Usuarios', N'Login exitoso: 46948668Maximo', 3)
INSERT [dbo].[BitacoraEventos] ([Id], [Username], [Fecha], [Modulo], [Descripcion], [Criticidad]) VALUES (394, N'46948668Maximo', CAST(N'2026-06-23T19:25:49.113' AS DateTime), N'Usuarios', N'Logout exitoso: 46948668Maximo', 3)
INSERT [dbo].[BitacoraEventos] ([Id], [Username], [Fecha], [Modulo], [Descripcion], [Criticidad]) VALUES (395, N'46948668Maximo', CAST(N'2026-06-24T02:21:26.653' AS DateTime), N'Usuarios', N'Intento fallido de login: 46948668Maximo', 1)
INSERT [dbo].[BitacoraEventos] ([Id], [Username], [Fecha], [Modulo], [Descripcion], [Criticidad]) VALUES (396, N'46948668Maximo', CAST(N'2026-06-24T02:21:30.907' AS DateTime), N'Usuarios', N'Login exitoso: 46948668Maximo', 3)
INSERT [dbo].[BitacoraEventos] ([Id], [Username], [Fecha], [Modulo], [Descripcion], [Criticidad]) VALUES (397, N'46948668Maximo', CAST(N'2026-06-24T02:25:50.580' AS DateTime), N'Usuarios', N'Logout exitoso: 46948668Maximo', 3)
INSERT [dbo].[BitacoraEventos] ([Id], [Username], [Fecha], [Modulo], [Descripcion], [Criticidad]) VALUES (398, N'46948668Maximo', CAST(N'2026-06-24T02:27:04.303' AS DateTime), N'Usuarios', N'Login exitoso: 46948668Maximo', 3)
INSERT [dbo].[BitacoraEventos] ([Id], [Username], [Fecha], [Modulo], [Descripcion], [Criticidad]) VALUES (399, N'46948668Maximo', CAST(N'2026-06-24T02:31:48.860' AS DateTime), N'Admin', N'Nuevo rol creado: PruebaAdmin (Actor: 46948668Maximo)', 3)
INSERT [dbo].[BitacoraEventos] ([Id], [Username], [Fecha], [Modulo], [Descripcion], [Criticidad]) VALUES (400, N'46905013Lucas', CAST(N'2026-06-24T02:32:27.070' AS DateTime), N'Admin', N'Usuario modificado: DNI 46905013. Email: lucas@gmail.com. Rol: PruebaAdmin. Actor: 46948668Maximo', 1)
INSERT [dbo].[BitacoraEventos] ([Id], [Username], [Fecha], [Modulo], [Descripcion], [Criticidad]) VALUES (401, N'46948668Maximo', CAST(N'2026-06-24T02:32:32.523' AS DateTime), N'Usuarios', N'Logout exitoso: 46948668Maximo', 3)
GO
INSERT [dbo].[BitacoraEventos] ([Id], [Username], [Fecha], [Modulo], [Descripcion], [Criticidad]) VALUES (402, N'46905013Lucas', CAST(N'2026-06-24T02:33:00.463' AS DateTime), N'Usuarios', N'Login exitoso: 46905013Lucas', 3)
INSERT [dbo].[BitacoraEventos] ([Id], [Username], [Fecha], [Modulo], [Descripcion], [Criticidad]) VALUES (403, N'46905013Lucas', CAST(N'2026-06-24T02:33:09.400' AS DateTime), N'Admin', N'Bitácora exportada a PDF: 110 eventos', 3)
INSERT [dbo].[BitacoraEventos] ([Id], [Username], [Fecha], [Modulo], [Descripcion], [Criticidad]) VALUES (404, N'46905013Lucas', CAST(N'2026-06-24T02:33:42.677' AS DateTime), N'Usuarios', N'Logout exitoso: 46905013Lucas', 3)
INSERT [dbo].[BitacoraEventos] ([Id], [Username], [Fecha], [Modulo], [Descripcion], [Criticidad]) VALUES (405, N'46948668Maximo', CAST(N'2026-06-24T02:33:53.377' AS DateTime), N'Usuarios', N'Login exitoso: 46948668Maximo', 3)
INSERT [dbo].[BitacoraEventos] ([Id], [Username], [Fecha], [Modulo], [Descripcion], [Criticidad]) VALUES (406, N'46948668Maximo', CAST(N'2026-06-24T02:34:25.180' AS DateTime), N'Admin', N'Rol modificado: PruebaAdmin (Actor: 46948668Maximo)', 3)
INSERT [dbo].[BitacoraEventos] ([Id], [Username], [Fecha], [Modulo], [Descripcion], [Criticidad]) VALUES (407, N'46948668Maximo', CAST(N'2026-06-24T02:34:36.540' AS DateTime), N'Admin', N'Rol modificado: PruebaAdmin (Actor: 46948668Maximo)', 3)
INSERT [dbo].[BitacoraEventos] ([Id], [Username], [Fecha], [Modulo], [Descripcion], [Criticidad]) VALUES (408, N'46948668Maximo', CAST(N'2026-06-24T02:34:39.967' AS DateTime), N'Usuarios', N'Logout exitoso: 46948668Maximo', 3)
INSERT [dbo].[BitacoraEventos] ([Id], [Username], [Fecha], [Modulo], [Descripcion], [Criticidad]) VALUES (409, N'46905013Lucas', CAST(N'2026-06-24T02:34:45.213' AS DateTime), N'Usuarios', N'Login exitoso: 46905013Lucas', 3)
INSERT [dbo].[BitacoraEventos] ([Id], [Username], [Fecha], [Modulo], [Descripcion], [Criticidad]) VALUES (410, N'46905013Lucas', CAST(N'2026-06-24T02:35:19.120' AS DateTime), N'Usuarios', N'Logout exitoso: 46905013Lucas', 3)
INSERT [dbo].[BitacoraEventos] ([Id], [Username], [Fecha], [Modulo], [Descripcion], [Criticidad]) VALUES (411, N'46948668Maximo', CAST(N'2026-06-24T02:35:27.060' AS DateTime), N'Usuarios', N'Login exitoso: 46948668Maximo', 3)
INSERT [dbo].[BitacoraEventos] ([Id], [Username], [Fecha], [Modulo], [Descripcion], [Criticidad]) VALUES (412, N'46948668Maximo', CAST(N'2026-06-24T02:35:44.987' AS DateTime), N'Usuarios', N'Logout exitoso: 46948668Maximo', 3)
INSERT [dbo].[BitacoraEventos] ([Id], [Username], [Fecha], [Modulo], [Descripcion], [Criticidad]) VALUES (413, N'46905013Lucas', CAST(N'2026-06-24T02:39:31.363' AS DateTime), N'Usuarios', N'Login exitoso: 46905013Lucas', 3)
INSERT [dbo].[BitacoraEventos] ([Id], [Username], [Fecha], [Modulo], [Descripcion], [Criticidad]) VALUES (414, N'46905013Lucas', CAST(N'2026-06-24T02:39:43.357' AS DateTime), N'Usuarios', N'Logout exitoso: 46905013Lucas', 3)
INSERT [dbo].[BitacoraEventos] ([Id], [Username], [Fecha], [Modulo], [Descripcion], [Criticidad]) VALUES (415, N'46948668Maximo', CAST(N'2026-06-24T02:40:04.863' AS DateTime), N'Usuarios', N'Login exitoso: 46948668Maximo', 3)
INSERT [dbo].[BitacoraEventos] ([Id], [Username], [Fecha], [Modulo], [Descripcion], [Criticidad]) VALUES (416, N'46948668Maximo', CAST(N'2026-06-24T02:40:19.147' AS DateTime), N'Admin', N'Familia modificada: AdministrarBitacora (Actor: 46948668Maximo)', 3)
INSERT [dbo].[BitacoraEventos] ([Id], [Username], [Fecha], [Modulo], [Descripcion], [Criticidad]) VALUES (417, N'46948668Maximo', CAST(N'2026-06-24T02:40:42.923' AS DateTime), N'Admin', N'Familia modificada: Auditoria (Actor: 46948668Maximo)', 3)
INSERT [dbo].[BitacoraEventos] ([Id], [Username], [Fecha], [Modulo], [Descripcion], [Criticidad]) VALUES (418, N'46948668Maximo', CAST(N'2026-06-24T02:41:20.603' AS DateTime), N'Usuarios', N'Logout exitoso: 46948668Maximo', 3)
INSERT [dbo].[BitacoraEventos] ([Id], [Username], [Fecha], [Modulo], [Descripcion], [Criticidad]) VALUES (419, N'46905013Lucas', CAST(N'2026-06-24T02:41:38.917' AS DateTime), N'Usuarios', N'Login exitoso: 46905013Lucas', 3)
INSERT [dbo].[BitacoraEventos] ([Id], [Username], [Fecha], [Modulo], [Descripcion], [Criticidad]) VALUES (420, N'46905013Lucas', CAST(N'2026-06-24T02:41:47.037' AS DateTime), N'Usuarios', N'Logout exitoso: 46905013Lucas', 3)
INSERT [dbo].[BitacoraEventos] ([Id], [Username], [Fecha], [Modulo], [Descripcion], [Criticidad]) VALUES (421, N'46905013Lucas', CAST(N'2026-06-24T02:43:44.647' AS DateTime), N'Usuarios', N'Login exitoso: 46905013Lucas', 3)
INSERT [dbo].[BitacoraEventos] ([Id], [Username], [Fecha], [Modulo], [Descripcion], [Criticidad]) VALUES (422, N'46905013Lucas', CAST(N'2026-06-24T02:43:56.560' AS DateTime), N'Usuarios', N'Logout exitoso: 46905013Lucas', 3)
INSERT [dbo].[BitacoraEventos] ([Id], [Username], [Fecha], [Modulo], [Descripcion], [Criticidad]) VALUES (423, N'46948668Maximo', CAST(N'2026-06-24T02:44:09.097' AS DateTime), N'Usuarios', N'Login exitoso: 46948668Maximo', 3)
INSERT [dbo].[BitacoraEventos] ([Id], [Username], [Fecha], [Modulo], [Descripcion], [Criticidad]) VALUES (424, N'46948668Maximo', CAST(N'2026-06-24T02:44:24.457' AS DateTime), N'Admin', N'Familia modificada: AdministrarBitacora (Actor: 46948668Maximo)', 3)
INSERT [dbo].[BitacoraEventos] ([Id], [Username], [Fecha], [Modulo], [Descripcion], [Criticidad]) VALUES (425, N'46948668Maximo', CAST(N'2026-06-24T02:44:33.460' AS DateTime), N'Admin', N'Familia modificada: AdministrarBitacora (Actor: 46948668Maximo)', 3)
INSERT [dbo].[BitacoraEventos] ([Id], [Username], [Fecha], [Modulo], [Descripcion], [Criticidad]) VALUES (426, N'46948668Maximo', CAST(N'2026-06-24T02:44:50.113' AS DateTime), N'Admin', N'Familia modificada: Auditoria (Actor: 46948668Maximo)', 3)
INSERT [dbo].[BitacoraEventos] ([Id], [Username], [Fecha], [Modulo], [Descripcion], [Criticidad]) VALUES (427, N'46948668Maximo', CAST(N'2026-06-24T02:45:02.580' AS DateTime), N'Admin', N'Familia modificada: Auditoria (Actor: 46948668Maximo)', 3)
INSERT [dbo].[BitacoraEventos] ([Id], [Username], [Fecha], [Modulo], [Descripcion], [Criticidad]) VALUES (428, N'46948668Maximo', CAST(N'2026-06-24T02:45:07.897' AS DateTime), N'Usuarios', N'Logout exitoso: 46948668Maximo', 3)
INSERT [dbo].[BitacoraEventos] ([Id], [Username], [Fecha], [Modulo], [Descripcion], [Criticidad]) VALUES (429, N'46905013Lucas', CAST(N'2026-06-24T02:45:14.210' AS DateTime), N'Usuarios', N'Login exitoso: 46905013Lucas', 3)
INSERT [dbo].[BitacoraEventos] ([Id], [Username], [Fecha], [Modulo], [Descripcion], [Criticidad]) VALUES (430, N'46905013Lucas', CAST(N'2026-06-24T02:45:46.550' AS DateTime), N'Usuarios', N'Logout exitoso: 46905013Lucas', 3)
INSERT [dbo].[BitacoraEventos] ([Id], [Username], [Fecha], [Modulo], [Descripcion], [Criticidad]) VALUES (431, N'46948668Maximo', CAST(N'2026-06-24T02:45:53.587' AS DateTime), N'Usuarios', N'Login exitoso: 46948668Maximo', 3)
INSERT [dbo].[BitacoraEventos] ([Id], [Username], [Fecha], [Modulo], [Descripcion], [Criticidad]) VALUES (432, N'12345672Nahuel', CAST(N'2026-06-24T02:46:34.060' AS DateTime), N'Admin', N'Nuevo usuario creado: 12345672Nahuel (Rol: RolSimple) (Actor: 46948668Maximo)', 1)
INSERT [dbo].[BitacoraEventos] ([Id], [Username], [Fecha], [Modulo], [Descripcion], [Criticidad]) VALUES (433, N'46948668Maximo', CAST(N'2026-06-24T02:51:53.243' AS DateTime), N'Usuarios', N'Logout exitoso: 46948668Maximo', 3)
INSERT [dbo].[BitacoraEventos] ([Id], [Username], [Fecha], [Modulo], [Descripcion], [Criticidad]) VALUES (434, N'12345672Nahuel', CAST(N'2026-06-24T02:52:49.773' AS DateTime), N'Usuarios', N'Login exitoso: 12345672Nahuel', 3)
INSERT [dbo].[BitacoraEventos] ([Id], [Username], [Fecha], [Modulo], [Descripcion], [Criticidad]) VALUES (435, N'12345672Nahuel', CAST(N'2026-06-24T02:52:54.210' AS DateTime), N'Usuarios', N'Logout exitoso: 12345672Nahuel', 3)
INSERT [dbo].[BitacoraEventos] ([Id], [Username], [Fecha], [Modulo], [Descripcion], [Criticidad]) VALUES (436, N'12345672Nahuel', CAST(N'2026-06-24T02:52:58.830' AS DateTime), N'Usuarios', N'Login exitoso: 12345672Nahuel', 3)
INSERT [dbo].[BitacoraEventos] ([Id], [Username], [Fecha], [Modulo], [Descripcion], [Criticidad]) VALUES (437, N'12345672Nahuel', CAST(N'2026-06-24T02:53:12.193' AS DateTime), N'Usuarios', N'Contraseña modificada: 12345672Nahuel (Actor: 12345672Nahuel)', 1)
INSERT [dbo].[BitacoraEventos] ([Id], [Username], [Fecha], [Modulo], [Descripcion], [Criticidad]) VALUES (438, N'12345672Nahuel', CAST(N'2026-06-24T02:53:26.910' AS DateTime), N'Usuarios', N'Logout exitoso: 12345672Nahuel', 3)
INSERT [dbo].[BitacoraEventos] ([Id], [Username], [Fecha], [Modulo], [Descripcion], [Criticidad]) VALUES (439, N'12345672Nahuel', CAST(N'2026-06-24T03:17:38.257' AS DateTime), N'Usuarios', N'Login exitoso: 12345672Nahuel', 3)
INSERT [dbo].[BitacoraEventos] ([Id], [Username], [Fecha], [Modulo], [Descripcion], [Criticidad]) VALUES (440, N'12345672Nahuel', CAST(N'2026-06-24T03:17:51.523' AS DateTime), N'Usuarios', N'Logout exitoso: 12345672Nahuel', 3)
INSERT [dbo].[BitacoraEventos] ([Id], [Username], [Fecha], [Modulo], [Descripcion], [Criticidad]) VALUES (441, N'46948668Maximo', CAST(N'2026-06-24T03:18:09.960' AS DateTime), N'Usuarios', N'Intento fallido de login: 46948668Maximo', 1)
INSERT [dbo].[BitacoraEventos] ([Id], [Username], [Fecha], [Modulo], [Descripcion], [Criticidad]) VALUES (442, N'46948668Maximo', CAST(N'2026-06-24T03:18:15.363' AS DateTime), N'Usuarios', N'Login exitoso: 46948668Maximo', 3)
INSERT [dbo].[BitacoraEventos] ([Id], [Username], [Fecha], [Modulo], [Descripcion], [Criticidad]) VALUES (443, N'46948668Maximo', CAST(N'2026-06-24T03:23:47.103' AS DateTime), N'Usuarios', N'Logout exitoso: 46948668Maximo', 3)
INSERT [dbo].[BitacoraEventos] ([Id], [Username], [Fecha], [Modulo], [Descripcion], [Criticidad]) VALUES (444, N'46948668Maximo', CAST(N'2026-06-24T03:36:27.567' AS DateTime), N'Usuarios', N'Login exitoso: 46948668Maximo', 3)
INSERT [dbo].[BitacoraEventos] ([Id], [Username], [Fecha], [Modulo], [Descripcion], [Criticidad]) VALUES (445, N'46948668Maximo', CAST(N'2026-06-24T03:37:23.767' AS DateTime), N'Admin', N'Familia modificada: Usuario (Actor: 46948668Maximo)', 3)
INSERT [dbo].[BitacoraEventos] ([Id], [Username], [Fecha], [Modulo], [Descripcion], [Criticidad]) VALUES (446, N'46948668Maximo', CAST(N'2026-06-24T03:37:29.287' AS DateTime), N'Usuarios', N'Logout exitoso: 46948668Maximo', 3)
INSERT [dbo].[BitacoraEventos] ([Id], [Username], [Fecha], [Modulo], [Descripcion], [Criticidad]) VALUES (447, N'46948668Maximo', CAST(N'2026-06-24T03:37:45.973' AS DateTime), N'Usuarios', N'Login exitoso: 46948668Maximo', 3)
INSERT [dbo].[BitacoraEventos] ([Id], [Username], [Fecha], [Modulo], [Descripcion], [Criticidad]) VALUES (448, N'46948668Maximo', CAST(N'2026-06-24T03:38:26.827' AS DateTime), N'Usuarios', N'Logout exitoso: 46948668Maximo', 3)
INSERT [dbo].[BitacoraEventos] ([Id], [Username], [Fecha], [Modulo], [Descripcion], [Criticidad]) VALUES (452, N'46948668Maximo', CAST(N'2026-07-06T23:47:52.090' AS DateTime), N'Usuarios', N'Login exitoso: 46948668Maximo', 3)
INSERT [dbo].[BitacoraEventos] ([Id], [Username], [Fecha], [Modulo], [Descripcion], [Criticidad]) VALUES (453, N'46948668Maximo', CAST(N'2026-07-06T23:49:29.500' AS DateTime), N'Usuarios', N'Login exitoso: 46948668Maximo', 3)
INSERT [dbo].[BitacoraEventos] ([Id], [Username], [Fecha], [Modulo], [Descripcion], [Criticidad]) VALUES (454, N'46948668Maximo', CAST(N'2026-07-06T23:52:26.177' AS DateTime), N'Usuarios', N'Login exitoso: 46948668Maximo', 3)
INSERT [dbo].[BitacoraEventos] ([Id], [Username], [Fecha], [Modulo], [Descripcion], [Criticidad]) VALUES (455, N'46948668Maximo', CAST(N'2026-07-07T01:37:52.413' AS DateTime), N'Admin', N'Integridad de datos comprometida', 1)
INSERT [dbo].[BitacoraEventos] ([Id], [Username], [Fecha], [Modulo], [Descripcion], [Criticidad]) VALUES (456, N'46905013Lucas', CAST(N'2026-07-07T01:38:29.187' AS DateTime), N'Admin', N'Integridad de datos comprometida', 1)
INSERT [dbo].[BitacoraEventos] ([Id], [Username], [Fecha], [Modulo], [Descripcion], [Criticidad]) VALUES (457, N'46905013Lucas', CAST(N'2026-07-07T01:38:33.757' AS DateTime), N'Admin', N'Integridad de datos comprometida', 1)
INSERT [dbo].[BitacoraEventos] ([Id], [Username], [Fecha], [Modulo], [Descripcion], [Criticidad]) VALUES (458, N'46905013Lucas', CAST(N'2026-07-07T01:46:36.033' AS DateTime), N'Admin', N'Integridad de datos comprometida', 1)
INSERT [dbo].[BitacoraEventos] ([Id], [Username], [Fecha], [Modulo], [Descripcion], [Criticidad]) VALUES (460, N'46948668Maximo', CAST(N'2026-07-07T02:01:31.630' AS DateTime), N'Admin', N'Integridad de datos comprometida', 1)
INSERT [dbo].[BitacoraEventos] ([Id], [Username], [Fecha], [Modulo], [Descripcion], [Criticidad]) VALUES (461, N'46948668Maximo', CAST(N'2026-07-07T02:01:33.790' AS DateTime), N'Admin', N'Integridad de datos comprometida', 1)
INSERT [dbo].[BitacoraEventos] ([Id], [Username], [Fecha], [Modulo], [Descripcion], [Criticidad]) VALUES (462, N'46905013Lucas', CAST(N'2026-07-07T02:02:00.563' AS DateTime), N'Admin', N'Integridad de datos comprometida', 1)
INSERT [dbo].[BitacoraEventos] ([Id], [Username], [Fecha], [Modulo], [Descripcion], [Criticidad]) VALUES (464, N'46948668Maximo', CAST(N'2026-07-07T02:06:55.080' AS DateTime), N'Admin', N'Integridad de datos comprometida', 1)
INSERT [dbo].[BitacoraEventos] ([Id], [Username], [Fecha], [Modulo], [Descripcion], [Criticidad]) VALUES (465, N'46948668Maximo', CAST(N'2026-07-07T02:09:04.640' AS DateTime), N'Admin', N'Integridad de datos comprometida', 1)
INSERT [dbo].[BitacoraEventos] ([Id], [Username], [Fecha], [Modulo], [Descripcion], [Criticidad]) VALUES (466, N'46948668Maximo', CAST(N'2026-07-07T02:09:16.700' AS DateTime), N'Admin', N'Integridad de datos comprometida', 1)
INSERT [dbo].[BitacoraEventos] ([Id], [Username], [Fecha], [Modulo], [Descripcion], [Criticidad]) VALUES (467, N'46948668Maximo', CAST(N'2026-07-07T02:10:34.107' AS DateTime), N'Admin', N'Integridad de datos comprometida', 1)
INSERT [dbo].[BitacoraEventos] ([Id], [Username], [Fecha], [Modulo], [Descripcion], [Criticidad]) VALUES (468, N'46948668Maximo', CAST(N'2026-07-07T02:12:07.940' AS DateTime), N'Admin', N'Integridad de datos comprometida', 1)
INSERT [dbo].[BitacoraEventos] ([Id], [Username], [Fecha], [Modulo], [Descripcion], [Criticidad]) VALUES (470, N'46948668Maximo', CAST(N'2026-07-07T02:25:57.383' AS DateTime), N'Admin', N'Integridad de datos comprometida', 1)
INSERT [dbo].[BitacoraEventos] ([Id], [Username], [Fecha], [Modulo], [Descripcion], [Criticidad]) VALUES (471, N'46948668Maximo', CAST(N'2026-07-07T02:26:54.957' AS DateTime), N'Admin', N'Integridad de datos comprometida', 1)
INSERT [dbo].[BitacoraEventos] ([Id], [Username], [Fecha], [Modulo], [Descripcion], [Criticidad]) VALUES (472, N'46948668Maximo', CAST(N'2026-07-07T02:30:13.760' AS DateTime), N'Usuarios', N'Login exitoso: 46948668Maximo', 3)
INSERT [dbo].[BitacoraEventos] ([Id], [Username], [Fecha], [Modulo], [Descripcion], [Criticidad]) VALUES (473, N'46948668Maximo', CAST(N'2026-07-07T02:34:55.220' AS DateTime), N'Usuarios', N'Logout exitoso: 46948668Maximo', 3)
INSERT [dbo].[BitacoraEventos] ([Id], [Username], [Fecha], [Modulo], [Descripcion], [Criticidad]) VALUES (474, N'46948668Maximo', CAST(N'2026-07-07T02:55:05.717' AS DateTime), N'Usuarios', N'Login exitoso: 46948668Maximo', 3)
INSERT [dbo].[BitacoraEventos] ([Id], [Username], [Fecha], [Modulo], [Descripcion], [Criticidad]) VALUES (475, N'46948668Maximo', CAST(N'2026-07-07T02:56:47.500' AS DateTime), N'Usuarios', N'Logout exitoso: 46948668Maximo', 3)
INSERT [dbo].[BitacoraEventos] ([Id], [Username], [Fecha], [Modulo], [Descripcion], [Criticidad]) VALUES (476, N'46948668Maximo', CAST(N'2026-07-07T03:03:30.910' AS DateTime), N'Usuarios', N'Login exitoso: 46948668Maximo', 3)
INSERT [dbo].[BitacoraEventos] ([Id], [Username], [Fecha], [Modulo], [Descripcion], [Criticidad]) VALUES (477, N'46948668Maximo', CAST(N'2026-07-07T03:16:51.120' AS DateTime), N'Usuarios', N'Login exitoso: 46948668Maximo', 3)
INSERT [dbo].[BitacoraEventos] ([Id], [Username], [Fecha], [Modulo], [Descripcion], [Criticidad]) VALUES (478, N'46948668Maximo', CAST(N'2026-07-07T03:17:10.810' AS DateTime), N'Usuarios', N'Logout exitoso: 46948668Maximo', 3)
INSERT [dbo].[BitacoraEventos] ([Id], [Username], [Fecha], [Modulo], [Descripcion], [Criticidad]) VALUES (479, N'46948668Maximo', CAST(N'2026-07-07T03:17:17.127' AS DateTime), N'Usuarios', N'Login exitoso: 46948668Maximo', 3)
INSERT [dbo].[BitacoraEventos] ([Id], [Username], [Fecha], [Modulo], [Descripcion], [Criticidad]) VALUES (480, N'46948668Maximo', CAST(N'2026-07-07T03:17:18.930' AS DateTime), N'Usuarios', N'Logout exitoso: 46948668Maximo', 3)
INSERT [dbo].[BitacoraEventos] ([Id], [Username], [Fecha], [Modulo], [Descripcion], [Criticidad]) VALUES (481, N'46948668Maximo', CAST(N'2026-07-07T03:17:24.253' AS DateTime), N'Usuarios', N'Login exitoso: 46948668Maximo', 3)
INSERT [dbo].[BitacoraEventos] ([Id], [Username], [Fecha], [Modulo], [Descripcion], [Criticidad]) VALUES (482, N'46948668Maximo', CAST(N'2026-07-07T03:17:58.107' AS DateTime), N'Admin', N'Recálculo de hashes ejecutado: 46948668Maximo', 1)
INSERT [dbo].[BitacoraEventos] ([Id], [Username], [Fecha], [Modulo], [Descripcion], [Criticidad]) VALUES (483, N'46948668Maximo', CAST(N'2026-07-07T03:22:33.463' AS DateTime), N'Usuarios', N'Logout exitoso: 46948668Maximo', 3)
INSERT [dbo].[BitacoraEventos] ([Id], [Username], [Fecha], [Modulo], [Descripcion], [Criticidad]) VALUES (484, N'46948668Maximo', CAST(N'2026-07-07T03:26:09.893' AS DateTime), N'Usuarios', N'Login exitoso: 46948668Maximo', 3)
INSERT [dbo].[BitacoraEventos] ([Id], [Username], [Fecha], [Modulo], [Descripcion], [Criticidad]) VALUES (485, N'46948668Maximo', CAST(N'2026-07-07T03:26:13.003' AS DateTime), N'Usuarios', N'Logout exitoso: 46948668Maximo', 3)
INSERT [dbo].[BitacoraEventos] ([Id], [Username], [Fecha], [Modulo], [Descripcion], [Criticidad]) VALUES (486, N'46948668Maximo', CAST(N'2026-07-07T03:27:08.690' AS DateTime), N'Usuarios', N'Login exitoso: 46948668Maximo', 3)
INSERT [dbo].[BitacoraEventos] ([Id], [Username], [Fecha], [Modulo], [Descripcion], [Criticidad]) VALUES (487, N'46948668Maximo', CAST(N'2026-07-07T03:27:14.050' AS DateTime), N'Usuarios', N'Logout exitoso: 46948668Maximo', 3)
INSERT [dbo].[BitacoraEventos] ([Id], [Username], [Fecha], [Modulo], [Descripcion], [Criticidad]) VALUES (488, N'46948668Maximo', CAST(N'2026-07-07T03:27:19.187' AS DateTime), N'Usuarios', N'Login exitoso: 46948668Maximo', 3)
INSERT [dbo].[BitacoraEventos] ([Id], [Username], [Fecha], [Modulo], [Descripcion], [Criticidad]) VALUES (489, N'46948668Maximo', CAST(N'2026-07-07T03:27:22.810' AS DateTime), N'Usuarios', N'Logout exitoso: 46948668Maximo', 3)
INSERT [dbo].[BitacoraEventos] ([Id], [Username], [Fecha], [Modulo], [Descripcion], [Criticidad]) VALUES (490, N'46948668Maximo', CAST(N'2026-07-07T03:30:39.833' AS DateTime), N'Usuarios', N'Login exitoso: 46948668Maximo', 3)
INSERT [dbo].[BitacoraEventos] ([Id], [Username], [Fecha], [Modulo], [Descripcion], [Criticidad]) VALUES (491, N'46948668Maximo', CAST(N'2026-07-07T03:33:42.263' AS DateTime), N'Usuarios', N'Logout exitoso: 46948668Maximo', 3)
INSERT [dbo].[BitacoraEventos] ([Id], [Username], [Fecha], [Modulo], [Descripcion], [Criticidad]) VALUES (492, N'46948668Maximo', CAST(N'2026-07-07T03:42:56.080' AS DateTime), N'Usuarios', N'Login exitoso: 46948668Maximo', 3)
INSERT [dbo].[BitacoraEventos] ([Id], [Username], [Fecha], [Modulo], [Descripcion], [Criticidad]) VALUES (493, N'46948668Maximo', CAST(N'2026-07-07T03:42:58.767' AS DateTime), N'Usuarios', N'Logout exitoso: 46948668Maximo', 3)
INSERT [dbo].[BitacoraEventos] ([Id], [Username], [Fecha], [Modulo], [Descripcion], [Criticidad]) VALUES (494, N'46948668Maximo', CAST(N'2026-07-07T03:43:17.647' AS DateTime), N'Usuarios', N'Login exitoso: 46948668Maximo', 3)
INSERT [dbo].[BitacoraEventos] ([Id], [Username], [Fecha], [Modulo], [Descripcion], [Criticidad]) VALUES (495, N'46948668Maximo', CAST(N'2026-07-07T03:43:20.553' AS DateTime), N'Usuarios', N'Logout exitoso: 46948668Maximo', 3)
INSERT [dbo].[BitacoraEventos] ([Id], [Username], [Fecha], [Modulo], [Descripcion], [Criticidad]) VALUES (496, N'46948668Maximo', CAST(N'2026-07-07T03:43:56.053' AS DateTime), N'Usuarios', N'Login exitoso: 46948668Maximo', 3)
INSERT [dbo].[BitacoraEventos] ([Id], [Username], [Fecha], [Modulo], [Descripcion], [Criticidad]) VALUES (497, N'46948668Maximo', CAST(N'2026-07-07T03:43:59.083' AS DateTime), N'Usuarios', N'Logout exitoso: 46948668Maximo', 3)
INSERT [dbo].[BitacoraEventos] ([Id], [Username], [Fecha], [Modulo], [Descripcion], [Criticidad]) VALUES (498, N'46948668Maximo', CAST(N'2026-07-07T03:44:05.243' AS DateTime), N'Usuarios', N'Login exitoso: 46948668Maximo', 3)
INSERT [dbo].[BitacoraEventos] ([Id], [Username], [Fecha], [Modulo], [Descripcion], [Criticidad]) VALUES (499, N'46948668Maximo', CAST(N'2026-07-07T03:44:08.953' AS DateTime), N'Usuarios', N'Logout exitoso: 46948668Maximo', 3)
INSERT [dbo].[BitacoraEventos] ([Id], [Username], [Fecha], [Modulo], [Descripcion], [Criticidad]) VALUES (500, N'46948668Maximo', CAST(N'2026-07-07T03:45:13.163' AS DateTime), N'Usuarios', N'Login exitoso: 46948668Maximo', 3)
INSERT [dbo].[BitacoraEventos] ([Id], [Username], [Fecha], [Modulo], [Descripcion], [Criticidad]) VALUES (501, N'46948668Maximo', CAST(N'2026-07-07T03:45:18.943' AS DateTime), N'Usuarios', N'Logout exitoso: 46948668Maximo', 3)
INSERT [dbo].[BitacoraEventos] ([Id], [Username], [Fecha], [Modulo], [Descripcion], [Criticidad]) VALUES (502, N'46948668Maximo', CAST(N'2026-07-07T03:50:01.623' AS DateTime), N'Usuarios', N'Login exitoso: 46948668Maximo', 3)
INSERT [dbo].[BitacoraEventos] ([Id], [Username], [Fecha], [Modulo], [Descripcion], [Criticidad]) VALUES (503, N'46948668Maximo', CAST(N'2026-07-07T03:50:06.920' AS DateTime), N'Usuarios', N'Logout exitoso: 46948668Maximo', 3)
INSERT [dbo].[BitacoraEventos] ([Id], [Username], [Fecha], [Modulo], [Descripcion], [Criticidad]) VALUES (504, N'46948668Maximo', CAST(N'2026-07-07T03:57:39.363' AS DateTime), N'Usuarios', N'Login exitoso: 46948668Maximo', 3)
INSERT [dbo].[BitacoraEventos] ([Id], [Username], [Fecha], [Modulo], [Descripcion], [Criticidad]) VALUES (505, N'46948668Maximo', CAST(N'2026-07-07T03:59:24.860' AS DateTime), N'Usuarios', N'Logout exitoso: 46948668Maximo', 3)
INSERT [dbo].[BitacoraEventos] ([Id], [Username], [Fecha], [Modulo], [Descripcion], [Criticidad]) VALUES (506, N'46948668Maximo', CAST(N'2026-07-07T04:00:29.957' AS DateTime), N'Usuarios', N'Login exitoso: 46948668Maximo', 3)
INSERT [dbo].[BitacoraEventos] ([Id], [Username], [Fecha], [Modulo], [Descripcion], [Criticidad]) VALUES (507, N'46948668Maximo', CAST(N'2026-07-07T04:01:52.083' AS DateTime), N'Usuarios', N'Logout exitoso: 46948668Maximo', 3)
GO
INSERT [dbo].[BitacoraEventos] ([Id], [Username], [Fecha], [Modulo], [Descripcion], [Criticidad]) VALUES (508, N'46948668Maximo', CAST(N'2026-07-07T04:05:17.747' AS DateTime), N'Usuarios', N'Login exitoso: 46948668Maximo', 3)
INSERT [dbo].[BitacoraEventos] ([Id], [Username], [Fecha], [Modulo], [Descripcion], [Criticidad]) VALUES (509, N'46948668Maximo', CAST(N'2026-07-07T04:06:54.837' AS DateTime), N'Usuarios', N'Logout exitoso: 46948668Maximo', 3)
INSERT [dbo].[BitacoraEventos] ([Id], [Username], [Fecha], [Modulo], [Descripcion], [Criticidad]) VALUES (510, N'46948668Maximo', CAST(N'2026-07-07T04:11:58.513' AS DateTime), N'Usuarios', N'Login exitoso: 46948668Maximo', 3)
INSERT [dbo].[BitacoraEventos] ([Id], [Username], [Fecha], [Modulo], [Descripcion], [Criticidad]) VALUES (511, N'46948668Maximo', CAST(N'2026-07-07T04:12:02.263' AS DateTime), N'Usuarios', N'Logout exitoso: 46948668Maximo', 3)
INSERT [dbo].[BitacoraEventos] ([Id], [Username], [Fecha], [Modulo], [Descripcion], [Criticidad]) VALUES (512, N'46948668Maximo', CAST(N'2026-07-07T04:24:44.153' AS DateTime), N'Usuarios', N'Login exitoso: 46948668Maximo', 3)
INSERT [dbo].[BitacoraEventos] ([Id], [Username], [Fecha], [Modulo], [Descripcion], [Criticidad]) VALUES (513, N'46948668Maximo', CAST(N'2026-07-07T04:24:53.400' AS DateTime), N'Usuarios', N'Logout exitoso: 46948668Maximo', 3)
INSERT [dbo].[BitacoraEventos] ([Id], [Username], [Fecha], [Modulo], [Descripcion], [Criticidad]) VALUES (514, N'46948668Maximo', CAST(N'2026-07-07T04:25:05.800' AS DateTime), N'Usuarios', N'Login exitoso: 46948668Maximo', 3)
INSERT [dbo].[BitacoraEventos] ([Id], [Username], [Fecha], [Modulo], [Descripcion], [Criticidad]) VALUES (515, N'46948668Maximo', CAST(N'2026-07-07T04:25:09.377' AS DateTime), N'Usuarios', N'Logout exitoso: 46948668Maximo', 3)
INSERT [dbo].[BitacoraEventos] ([Id], [Username], [Fecha], [Modulo], [Descripcion], [Criticidad]) VALUES (516, N'46948668Maximo', CAST(N'2026-07-07T04:25:45.510' AS DateTime), N'Usuarios', N'Login exitoso: 46948668Maximo', 3)
INSERT [dbo].[BitacoraEventos] ([Id], [Username], [Fecha], [Modulo], [Descripcion], [Criticidad]) VALUES (517, N'46948668Maximo', CAST(N'2026-07-07T04:25:51.133' AS DateTime), N'Usuarios', N'Logout exitoso: 46948668Maximo', 3)
INSERT [dbo].[BitacoraEventos] ([Id], [Username], [Fecha], [Modulo], [Descripcion], [Criticidad]) VALUES (518, N'46948668Maximo', CAST(N'2026-07-07T04:29:07.980' AS DateTime), N'Usuarios', N'Login exitoso: 46948668Maximo', 3)
INSERT [dbo].[BitacoraEventos] ([Id], [Username], [Fecha], [Modulo], [Descripcion], [Criticidad]) VALUES (519, N'46948668Maximo', CAST(N'2026-07-07T04:29:30.230' AS DateTime), N'Usuarios', N'Logout exitoso: 46948668Maximo', 3)
INSERT [dbo].[BitacoraEventos] ([Id], [Username], [Fecha], [Modulo], [Descripcion], [Criticidad]) VALUES (520, N'46948668Maximo', CAST(N'2026-07-07T04:29:56.207' AS DateTime), N'Usuarios', N'Login exitoso: 46948668Maximo', 3)
INSERT [dbo].[BitacoraEventos] ([Id], [Username], [Fecha], [Modulo], [Descripcion], [Criticidad]) VALUES (521, N'46948668Maximo', CAST(N'2026-07-07T04:30:26.177' AS DateTime), N'Usuarios', N'Logout exitoso: 46948668Maximo', 3)
INSERT [dbo].[BitacoraEventos] ([Id], [Username], [Fecha], [Modulo], [Descripcion], [Criticidad]) VALUES (522, N'46948668Maximo', CAST(N'2026-07-07T04:32:39.490' AS DateTime), N'Usuarios', N'Login exitoso: 46948668Maximo', 3)
INSERT [dbo].[BitacoraEventos] ([Id], [Username], [Fecha], [Modulo], [Descripcion], [Criticidad]) VALUES (523, N'46948668Maximo', CAST(N'2026-07-07T04:32:46.533' AS DateTime), N'Usuarios', N'Logout exitoso: 46948668Maximo', 3)
INSERT [dbo].[BitacoraEventos] ([Id], [Username], [Fecha], [Modulo], [Descripcion], [Criticidad]) VALUES (524, N'46948668Maximo', CAST(N'2026-07-07T04:33:07.820' AS DateTime), N'Usuarios', N'Login exitoso: 46948668Maximo', 3)
INSERT [dbo].[BitacoraEventos] ([Id], [Username], [Fecha], [Modulo], [Descripcion], [Criticidad]) VALUES (525, N'46948668Maximo', CAST(N'2026-07-07T04:33:46.063' AS DateTime), N'Usuarios', N'Logout exitoso: 46948668Maximo', 3)
INSERT [dbo].[BitacoraEventos] ([Id], [Username], [Fecha], [Modulo], [Descripcion], [Criticidad]) VALUES (528, N'46948668Maximo', CAST(N'2026-07-07T04:36:28.523' AS DateTime), N'Usuarios', N'Login exitoso: 46948668Maximo', 3)
INSERT [dbo].[BitacoraEventos] ([Id], [Username], [Fecha], [Modulo], [Descripcion], [Criticidad]) VALUES (529, N'46948668Maximo', CAST(N'2026-07-07T04:37:23.617' AS DateTime), N'Usuarios', N'Logout exitoso: 46948668Maximo', 3)
INSERT [dbo].[BitacoraEventos] ([Id], [Username], [Fecha], [Modulo], [Descripcion], [Criticidad]) VALUES (530, N'46948668Maximo', CAST(N'2026-07-07T04:44:35.037' AS DateTime), N'Usuarios', N'Login exitoso: 46948668Maximo', 3)
INSERT [dbo].[BitacoraEventos] ([Id], [Username], [Fecha], [Modulo], [Descripcion], [Criticidad]) VALUES (531, N'46948668Maximo', CAST(N'2026-07-07T04:44:41.513' AS DateTime), N'Usuarios', N'Logout exitoso: 46948668Maximo', 3)
INSERT [dbo].[BitacoraEventos] ([Id], [Username], [Fecha], [Modulo], [Descripcion], [Criticidad]) VALUES (532, N'46948668Maximo', CAST(N'2026-07-07T04:44:45.677' AS DateTime), N'Usuarios', N'Login exitoso: 46948668Maximo', 3)
INSERT [dbo].[BitacoraEventos] ([Id], [Username], [Fecha], [Modulo], [Descripcion], [Criticidad]) VALUES (533, N'46948668Maximo', CAST(N'2026-07-07T04:44:47.060' AS DateTime), N'Usuarios', N'Logout exitoso: 46948668Maximo', 3)
INSERT [dbo].[BitacoraEventos] ([Id], [Username], [Fecha], [Modulo], [Descripcion], [Criticidad]) VALUES (534, N'46948668Maximo', CAST(N'2026-07-07T04:45:24.070' AS DateTime), N'Usuarios', N'Login exitoso: 46948668Maximo', 3)
INSERT [dbo].[BitacoraEventos] ([Id], [Username], [Fecha], [Modulo], [Descripcion], [Criticidad]) VALUES (535, N'46948668Maximo', CAST(N'2026-07-07T04:45:51.167' AS DateTime), N'Admin', N'Recálculo de hashes ejecutado: 46948668Maximo', 1)
INSERT [dbo].[BitacoraEventos] ([Id], [Username], [Fecha], [Modulo], [Descripcion], [Criticidad]) VALUES (536, N'46948668Maximo', CAST(N'2026-07-07T04:45:55.317' AS DateTime), N'Usuarios', N'Logout exitoso: 46948668Maximo', 3)
INSERT [dbo].[BitacoraEventos] ([Id], [Username], [Fecha], [Modulo], [Descripcion], [Criticidad]) VALUES (537, N'46948668Maximo', CAST(N'2026-07-07T04:46:10.810' AS DateTime), N'Usuarios', N'Login exitoso: 46948668Maximo', 3)
INSERT [dbo].[BitacoraEventos] ([Id], [Username], [Fecha], [Modulo], [Descripcion], [Criticidad]) VALUES (538, N'46948668Maximo', CAST(N'2026-07-07T04:46:14.867' AS DateTime), N'Usuarios', N'Logout exitoso: 46948668Maximo', 3)
INSERT [dbo].[BitacoraEventos] ([Id], [Username], [Fecha], [Modulo], [Descripcion], [Criticidad]) VALUES (539, N'46948668Maximo', CAST(N'2026-07-07T04:46:18.257' AS DateTime), N'Usuarios', N'Intento fallido de login: 46948668Maximo', 1)
INSERT [dbo].[BitacoraEventos] ([Id], [Username], [Fecha], [Modulo], [Descripcion], [Criticidad]) VALUES (540, N'46948668Maximo', CAST(N'2026-07-07T04:49:49.300' AS DateTime), N'Usuarios', N'Login exitoso: 46948668Maximo', 3)
INSERT [dbo].[BitacoraEventos] ([Id], [Username], [Fecha], [Modulo], [Descripcion], [Criticidad]) VALUES (541, N'46948668Maximo', CAST(N'2026-07-07T04:49:50.353' AS DateTime), N'Admin', N'Recálculo de hashes ejecutado: 46948668Maximo', 1)
INSERT [dbo].[BitacoraEventos] ([Id], [Username], [Fecha], [Modulo], [Descripcion], [Criticidad]) VALUES (542, N'46948668Maximo', CAST(N'2026-07-07T04:49:57.610' AS DateTime), N'Usuarios', N'Logout exitoso: 46948668Maximo', 3)
INSERT [dbo].[BitacoraEventos] ([Id], [Username], [Fecha], [Modulo], [Descripcion], [Criticidad]) VALUES (543, N'46948668Maximo', CAST(N'2026-07-07T04:50:05.953' AS DateTime), N'Usuarios', N'Login exitoso: 46948668Maximo', 3)
INSERT [dbo].[BitacoraEventos] ([Id], [Username], [Fecha], [Modulo], [Descripcion], [Criticidad]) VALUES (544, N'46948668Maximo', CAST(N'2026-07-07T04:55:51.297' AS DateTime), N'Admin', N'Restauración de base de datos ejecutada: 46948668Maximo — Ruta: D:\GestionUsuarios_backup_20260707_015009.bak', 1)
INSERT [dbo].[BitacoraEventos] ([Id], [Username], [Fecha], [Modulo], [Descripcion], [Criticidad]) VALUES (545, N'46948668Maximo', CAST(N'2026-07-07T04:56:20.063' AS DateTime), N'Usuarios', N'Login exitoso: 46948668Maximo', 3)
INSERT [dbo].[BitacoraEventos] ([Id], [Username], [Fecha], [Modulo], [Descripcion], [Criticidad]) VALUES (546, N'46948668Maximo', CAST(N'2026-07-07T04:57:25.640' AS DateTime), N'Usuarios', N'Logout exitoso: 46948668Maximo', 3)
INSERT [dbo].[BitacoraEventos] ([Id], [Username], [Fecha], [Modulo], [Descripcion], [Criticidad]) VALUES (547, N'46948668Maximo', CAST(N'2026-07-07T04:58:47.173' AS DateTime), N'Usuarios', N'Login exitoso: 46948668Maximo', 3)
INSERT [dbo].[BitacoraEventos] ([Id], [Username], [Fecha], [Modulo], [Descripcion], [Criticidad]) VALUES (548, N'46948668Maximo', CAST(N'2026-07-07T05:27:58.290' AS DateTime), N'Admin', N'Restauración de base de datos ejecutada: 46948668Maximo — Ruta: D:\GestionUsuarios_backup_ultima.bak', 1)
INSERT [dbo].[BitacoraEventos] ([Id], [Username], [Fecha], [Modulo], [Descripcion], [Criticidad]) VALUES (549, N'46948668Maximo', CAST(N'2026-07-07T05:28:05.870' AS DateTime), N'Usuarios', N'Login exitoso: 46948668Maximo', 3)
INSERT [dbo].[BitacoraEventos] ([Id], [Username], [Fecha], [Modulo], [Descripcion], [Criticidad]) VALUES (550, N'46948668Maximo', CAST(N'2026-07-07T05:28:12.947' AS DateTime), N'Admin', N'Recálculo de hashes ejecutado: 46948668Maximo', 1)
INSERT [dbo].[BitacoraEventos] ([Id], [Username], [Fecha], [Modulo], [Descripcion], [Criticidad]) VALUES (551, N'46948668Maximo', CAST(N'2026-07-07T05:28:13.913' AS DateTime), N'Usuarios', N'Logout exitoso: 46948668Maximo', 3)
INSERT [dbo].[BitacoraEventos] ([Id], [Username], [Fecha], [Modulo], [Descripcion], [Criticidad]) VALUES (552, N'46948668Maximo', CAST(N'2026-07-07T05:28:47.713' AS DateTime), N'Usuarios', N'Login exitoso: 46948668Maximo', 3)
INSERT [dbo].[BitacoraEventos] ([Id], [Username], [Fecha], [Modulo], [Descripcion], [Criticidad]) VALUES (553, N'46948668Maximo', CAST(N'2026-07-07T05:28:50.427' AS DateTime), N'Usuarios', N'Logout exitoso: 46948668Maximo', 3)
INSERT [dbo].[BitacoraEventos] ([Id], [Username], [Fecha], [Modulo], [Descripcion], [Criticidad]) VALUES (554, N'47006530Benjamin', CAST(N'2026-07-07T05:50:12.003' AS DateTime), N'Admin', N'Integridad de datos comprometida', 1)
INSERT [dbo].[BitacoraEventos] ([Id], [Username], [Fecha], [Modulo], [Descripcion], [Criticidad]) VALUES (555, N'47006530Benjamin', CAST(N'2026-07-07T05:50:16.210' AS DateTime), N'Admin', N'Integridad de datos comprometida', 1)
INSERT [dbo].[BitacoraEventos] ([Id], [Username], [Fecha], [Modulo], [Descripcion], [Criticidad]) VALUES (556, N'47006530Benjamin', CAST(N'2026-07-07T05:50:20.650' AS DateTime), N'Admin', N'Integridad de datos comprometida', 1)
INSERT [dbo].[BitacoraEventos] ([Id], [Username], [Fecha], [Modulo], [Descripcion], [Criticidad]) VALUES (557, N'47006530Benjamin', CAST(N'2026-07-07T05:50:48.780' AS DateTime), N'Admin', N'Integridad de datos comprometida', 1)
INSERT [dbo].[BitacoraEventos] ([Id], [Username], [Fecha], [Modulo], [Descripcion], [Criticidad]) VALUES (558, N'47006530Benjamin', CAST(N'2026-07-07T05:50:53.387' AS DateTime), N'Admin', N'Integridad de datos comprometida', 1)
INSERT [dbo].[BitacoraEventos] ([Id], [Username], [Fecha], [Modulo], [Descripcion], [Criticidad]) VALUES (560, N'47006530Benjamin', CAST(N'2026-07-07T05:51:00.100' AS DateTime), N'Admin', N'Integridad de datos comprometida', 1)
INSERT [dbo].[BitacoraEventos] ([Id], [Username], [Fecha], [Modulo], [Descripcion], [Criticidad]) VALUES (561, N'46948668Maximo', CAST(N'2026-07-07T23:02:44.283' AS DateTime), N'Usuarios', N'Login exitoso: 46948668Maximo', 3)
INSERT [dbo].[BitacoraEventos] ([Id], [Username], [Fecha], [Modulo], [Descripcion], [Criticidad]) VALUES (562, N'46948668Maximo', CAST(N'2026-07-07T23:02:50.727' AS DateTime), N'Admin', N'Recalculo de hashes ejecutado: 46948668Maximo', 1)
INSERT [dbo].[BitacoraEventos] ([Id], [Username], [Fecha], [Modulo], [Descripcion], [Criticidad]) VALUES (563, N'46948668Maximo', CAST(N'2026-07-07T23:02:55.630' AS DateTime), N'Usuarios', N'Logout exitoso: 46948668Maximo', 3)
INSERT [dbo].[BitacoraEventos] ([Id], [Username], [Fecha], [Modulo], [Descripcion], [Criticidad]) VALUES (564, N'46948668Maximo', CAST(N'2026-07-08T00:49:39.683' AS DateTime), N'Usuarios', N'Login exitoso: 46948668Maximo', 3)
INSERT [dbo].[BitacoraEventos] ([Id], [Username], [Fecha], [Modulo], [Descripcion], [Criticidad]) VALUES (565, N'46948668Maximo', CAST(N'2026-07-08T00:51:44.297' AS DateTime), N'Admin', N'Restauracion de base de datos ejecutada: 46948668Maximo — Ruta: D:\GestionUsuarios_backup_20260707_215002.bak', 1)
INSERT [dbo].[BitacoraEventos] ([Id], [Username], [Fecha], [Modulo], [Descripcion], [Criticidad]) VALUES (566, N'46948668Maximo', CAST(N'2026-07-08T00:52:30.767' AS DateTime), N'Usuarios', N'Login exitoso: 46948668Maximo', 3)
SET IDENTITY_INSERT [dbo].[BitacoraEventos] OFF
GO
INSERT [dbo].[DigitoVerificador_83KI] ([NombreTabla], [DVV], [FechaActualizacion]) VALUES (N'FamiliaFamilia', N'bab16e3f51626195d348479720445001c9c50f819ea5804e0a37c9ffad3a801f', CAST(N'2026-07-07T20:02:50.723' AS DateTime))
INSERT [dbo].[DigitoVerificador_83KI] ([NombreTabla], [DVV], [FechaActualizacion]) VALUES (N'FamiliaPatente', N'773f267b9ff2c337b7e1df1fbb1d7d34a4a046753dceebbf9e44ed14646242b7', CAST(N'2026-07-07T20:02:50.720' AS DateTime))
INSERT [dbo].[DigitoVerificador_83KI] ([NombreTabla], [DVV], [FechaActualizacion]) VALUES (N'Familias', N'0393d20d6abeaac2d08ee00257bbc99a38e9904ff64cd51d6842387fc0794858', CAST(N'2026-07-07T20:02:50.690' AS DateTime))
INSERT [dbo].[DigitoVerificador_83KI] ([NombreTabla], [DVV], [FechaActualizacion]) VALUES (N'Patentes', N'af43c3bac4cfc2070f11cc1778432d9d3cfed6e17c014fd951f85c5af64c6533', CAST(N'2026-07-07T20:02:50.697' AS DateTime))
INSERT [dbo].[DigitoVerificador_83KI] ([NombreTabla], [DVV], [FechaActualizacion]) VALUES (N'Roles', N'befd6c81ff9538fdb685237af658337576a9a88f0c84470c98a8f50ed1e94fa6', CAST(N'2026-07-07T20:02:50.687' AS DateTime))
INSERT [dbo].[DigitoVerificador_83KI] ([NombreTabla], [DVV], [FechaActualizacion]) VALUES (N'RolFamilia', N'5885a3e77918009a70bbca18457571b5ce2cefbfd66cdc731e65b9e6ee5a347f', CAST(N'2026-07-07T20:02:50.710' AS DateTime))
INSERT [dbo].[DigitoVerificador_83KI] ([NombreTabla], [DVV], [FechaActualizacion]) VALUES (N'RolPatente', N'1345f08958353d8d97068d0361e8ee2dd10b6c6af4c5bf7d4e9e137dd8cf8c43', CAST(N'2026-07-07T20:02:50.707' AS DateTime))
INSERT [dbo].[DigitoVerificador_83KI] ([NombreTabla], [DVV], [FechaActualizacion]) VALUES (N'Usuarios', N'dc62492b91f5dd637021ee52c539bf9ddf3571eb26f214d79e8579bf96c34dde', CAST(N'2026-07-07T21:52:30.760' AS DateTime))
GO
INSERT [dbo].[FamiliaFamilia] ([CodigoFamiliaPadre], [CodigoFamiliaHija], [DVH]) VALUES (1, 7, N'549bfe4f1093183397a50682e70849cfeeeab4277cc0a71a4e9b4992bfb577f3')
INSERT [dbo].[FamiliaFamilia] ([CodigoFamiliaPadre], [CodigoFamiliaHija], [DVH]) VALUES (5, 4, N'641a59117c9eda2fa6935dad31f9f6f4f5e92eec97b17667979b188f45a7462f')
INSERT [dbo].[FamiliaFamilia] ([CodigoFamiliaPadre], [CodigoFamiliaHija], [DVH]) VALUES (8, 9, N'7a5891873be362cca4c8458e967974667d3e445a450c669806b01eb62801183e')
INSERT [dbo].[FamiliaFamilia] ([CodigoFamiliaPadre], [CodigoFamiliaHija], [DVH]) VALUES (10, 2, N'302e40ed8c8743f55fabdf2a59897d614d16a91b02f9d343b1faed8a22daee39')
INSERT [dbo].[FamiliaFamilia] ([CodigoFamiliaPadre], [CodigoFamiliaHija], [DVH]) VALUES (16, 15, N'e944ab168c416b62c53b859e67c8123f1c9c64243bdd6d82a27da4f22ec7add2')
INSERT [dbo].[FamiliaFamilia] ([CodigoFamiliaPadre], [CodigoFamiliaHija], [DVH]) VALUES (19, 16, N'627d06ec9cb0e30cd6fa62db881e17ddc29595e21118a04b6d17b41458e47f88')
GO
INSERT [dbo].[FamiliaPatente] ([CodigoFamilia], [CodigoPatente], [DVH]) VALUES (1, 1, N'7183009a547deee4e1388f54aa2744d7cdad6d1791f559023426fae5d8509add')
INSERT [dbo].[FamiliaPatente] ([CodigoFamilia], [CodigoPatente], [DVH]) VALUES (1, 2, N'8466f8b8350b74aeafba0dbfbcf81347f6ddacdc27c6f7ab0cd951ebfab673f6')
INSERT [dbo].[FamiliaPatente] ([CodigoFamilia], [CodigoPatente], [DVH]) VALUES (1, 3, N'705d5ec35a2a8e6427f8e5a69a4d86d72a56a188b3428e061dffcd8cce68e80e')
INSERT [dbo].[FamiliaPatente] ([CodigoFamilia], [CodigoPatente], [DVH]) VALUES (1, 4, N'0b2a69bfa8ed25353cc883c828a175152fc825b2510c97b0274b02daada7ccdf')
INSERT [dbo].[FamiliaPatente] ([CodigoFamilia], [CodigoPatente], [DVH]) VALUES (1, 5, N'fd63a18ceb3a63335392392992f92148bcbc6243388d6727db535d07474bc77d')
INSERT [dbo].[FamiliaPatente] ([CodigoFamilia], [CodigoPatente], [DVH]) VALUES (1, 14, N'70737d9dbeaf7819027b297218d2eb496a2c66ae8c31803245093513b2957786')
INSERT [dbo].[FamiliaPatente] ([CodigoFamilia], [CodigoPatente], [DVH]) VALUES (1, 15, N'61d9569469895e2bf8251f828da41deb9f4649d1da01e0f3ef8c77bf13a9cf47')
INSERT [dbo].[FamiliaPatente] ([CodigoFamilia], [CodigoPatente], [DVH]) VALUES (2, 6, N'6985cfdc20992f921f00d4a23fc48494c5d823b7ce7b62af337d5d4b2cb5d223')
INSERT [dbo].[FamiliaPatente] ([CodigoFamilia], [CodigoPatente], [DVH]) VALUES (2, 31, N'9c803c7871fa3d5f26e82ad59daf5e7bccea67107e21445f663d1be32a6ecf19')
INSERT [dbo].[FamiliaPatente] ([CodigoFamilia], [CodigoPatente], [DVH]) VALUES (2, 32, N'0d76bebf0fd95e0438c12b3e7271d768fde8b36b87f0dc57f1f077d6d9acb9c8')
INSERT [dbo].[FamiliaPatente] ([CodigoFamilia], [CodigoPatente], [DVH]) VALUES (2, 33, N'7c39fb2ee8a4dda4d9ed5d2e7670b91b9b75abb5ee71a3a4c98f03447f7bdc30')
INSERT [dbo].[FamiliaPatente] ([CodigoFamilia], [CodigoPatente], [DVH]) VALUES (4, 9, N'f2e60f07a095d2972658947164f4d29cf5687d31b71569e4bc8e5d3d5fbf2c02')
INSERT [dbo].[FamiliaPatente] ([CodigoFamilia], [CodigoPatente], [DVH]) VALUES (5, 7, N'a7f93066db876f8311961e61d6473ba0aaab4c457f03132eba80507968c1b61f')
INSERT [dbo].[FamiliaPatente] ([CodigoFamilia], [CodigoPatente], [DVH]) VALUES (5, 8, N'7d8fdf38dd7fb6c7443b4c6048933c8259e9aaf14a7acc36608a0a08d9030e7a')
INSERT [dbo].[FamiliaPatente] ([CodigoFamilia], [CodigoPatente], [DVH]) VALUES (5, 10, N'84fd3550fb84f7a3b3f0d284dffdbfa180cda22894645bf9d3c6239adc3bc01c')
INSERT [dbo].[FamiliaPatente] ([CodigoFamilia], [CodigoPatente], [DVH]) VALUES (5, 37, N'6786f86ed4a7c3dad1091bfe9d7a2fdf64f9c8edd42ae51b38c4640f79226255')
INSERT [dbo].[FamiliaPatente] ([CodigoFamilia], [CodigoPatente], [DVH]) VALUES (7, 35, N'3c120f49d6daf04e3cdda4875a1da5c4d3529179b2ef6b956ed03655cd96b5f5')
INSERT [dbo].[FamiliaPatente] ([CodigoFamilia], [CodigoPatente], [DVH]) VALUES (8, 24, N'd3ea81e7a8f059e6ab760add45c0a3ded9b3c17118a2b2b91e60caca62cd0893')
INSERT [dbo].[FamiliaPatente] ([CodigoFamilia], [CodigoPatente], [DVH]) VALUES (8, 25, N'2ff122e591d4f9a85ef1e6dec954bf001f944756d5e1c6f6341d0fb710600b78')
INSERT [dbo].[FamiliaPatente] ([CodigoFamilia], [CodigoPatente], [DVH]) VALUES (8, 26, N'29bef49b1128a86664be3a977877323fdc80d09191cd0a52fd45c690e874de2d')
INSERT [dbo].[FamiliaPatente] ([CodigoFamilia], [CodigoPatente], [DVH]) VALUES (8, 27, N'0d76694ec44ab21793e71399cba3b270109a807e21efe768a4dc7fdb3d1e6381')
INSERT [dbo].[FamiliaPatente] ([CodigoFamilia], [CodigoPatente], [DVH]) VALUES (8, 28, N'd990e3b24361d762301090816da3a4338ac06cd964e2a9565c817e233efbe2bb')
INSERT [dbo].[FamiliaPatente] ([CodigoFamilia], [CodigoPatente], [DVH]) VALUES (8, 29, N'c6d3caa5c3b3f24086285a9441b7f485d2827a704dcc1592fc142a6b0dac34c0')
INSERT [dbo].[FamiliaPatente] ([CodigoFamilia], [CodigoPatente], [DVH]) VALUES (8, 30, N'cfdea1591b15652fa1a33a612cae60e020f0f2f5e7b59d0c288449f48220604b')
INSERT [dbo].[FamiliaPatente] ([CodigoFamilia], [CodigoPatente], [DVH]) VALUES (8, 36, N'7201d2c5b0e48914df3a7e83513097ae6d807722b9b2ed97e8971bbf6475c5ec')
INSERT [dbo].[FamiliaPatente] ([CodigoFamilia], [CodigoPatente], [DVH]) VALUES (9, 16, N'e67f6c21308823b7a6d8a692b854eaa7360cb9f6724dbdf94cce881a1d17b73c')
INSERT [dbo].[FamiliaPatente] ([CodigoFamilia], [CodigoPatente], [DVH]) VALUES (9, 17, N'4aa3e071801538e4e9d4aa77f824f73423a445b06d41239b132b0076a2b5f663')
INSERT [dbo].[FamiliaPatente] ([CodigoFamilia], [CodigoPatente], [DVH]) VALUES (9, 18, N'cb4c603f241ce5c9be23c9f20bcb4bd5b37de307b0d47a42f390316af21462ec')
INSERT [dbo].[FamiliaPatente] ([CodigoFamilia], [CodigoPatente], [DVH]) VALUES (9, 19, N'31fccabac5db289191554849104e8c8e0fb33e3d1705847e78cd7f6914f25022')
INSERT [dbo].[FamiliaPatente] ([CodigoFamilia], [CodigoPatente], [DVH]) VALUES (9, 20, N'e5e12a9ddbb37eb6b2c11447dec401336776fa62533979e648eb4f79acf288e6')
INSERT [dbo].[FamiliaPatente] ([CodigoFamilia], [CodigoPatente], [DVH]) VALUES (9, 21, N'89bc9e49079fc34ab5090db4273e1351c3f7ab7711ad3936c31908da01d9403a')
INSERT [dbo].[FamiliaPatente] ([CodigoFamilia], [CodigoPatente], [DVH]) VALUES (9, 22, N'66f6454256aecc46210373ebddd60652909c303764da3ccff827dac2506fa2dd')
INSERT [dbo].[FamiliaPatente] ([CodigoFamilia], [CodigoPatente], [DVH]) VALUES (9, 23, N'f37391f026659afb8afb26f753898270a203e5df35c1d678802e429ec2430970')
INSERT [dbo].[FamiliaPatente] ([CodigoFamilia], [CodigoPatente], [DVH]) VALUES (10, 34, N'814e6fedba5776f5cdc53c2f9137feb4c77fa920c17c422fc734bf26c39298e1')
INSERT [dbo].[FamiliaPatente] ([CodigoFamilia], [CodigoPatente], [DVH]) VALUES (15, 27, N'1226859c0ba6daca326c019d738620237ae46ce22d6e53c982c541c7fe1e556e')
INSERT [dbo].[FamiliaPatente] ([CodigoFamilia], [CodigoPatente], [DVH]) VALUES (16, 20, N'0baa07cbaf5842c956c45df5aee547dca889f39d917a63e5a4aa30284c8c3c0e')
INSERT [dbo].[FamiliaPatente] ([CodigoFamilia], [CodigoPatente], [DVH]) VALUES (19, 15, N'd4ca907fee76e92995da0c15d6314f78b4c665892212493ee939da13e0288676')
GO
SET IDENTITY_INSERT [dbo].[Familias] ON 

INSERT [dbo].[Familias] ([CodigoFamilia], [Nombre], [DVH]) VALUES (1, N'Administrador usuarios', N'7199939b029f493680eda649ecdbefa0f4d8c20592537e41c34b160d94b4fcc8')
INSERT [dbo].[Familias] ([CodigoFamilia], [Nombre], [DVH]) VALUES (2, N'Auditoria', N'190672d0aec50f1f0000e1f8bbd5843da43e6198e93fbad84a300ef8d8b59dd1')
INSERT [dbo].[Familias] ([CodigoFamilia], [Nombre], [DVH]) VALUES (4, N'Simple', N'6b86a6283531614bf1d7870e50a3bcf296840396eedc3ff21b5114c4f94d3665')
INSERT [dbo].[Familias] ([CodigoFamilia], [Nombre], [DVH]) VALUES (5, N'Usuario', N'08ea2caf69fc03d801cac5960b84e4195d1de6941ef0a65ac58156ae41a2ac88')
INSERT [dbo].[Familias] ([CodigoFamilia], [Nombre], [DVH]) VALUES (7, N'Administrador', N'98bc0ecf80c0f7525d008625f20a013c5bb6da312426c980229a93a8f8039a8d')
INSERT [dbo].[Familias] ([CodigoFamilia], [Nombre], [DVH]) VALUES (8, N'AdministrarRoles', N'66c6ee3fedb616f64df4e42311ac8ae1c63d9edb458c6c7e5e8eb22183ab3039')
INSERT [dbo].[Familias] ([CodigoFamilia], [Nombre], [DVH]) VALUES (9, N'AdministrarFamilias', N'7f45922470a2c10168acc858aec48c818181b31d3525aacab1d6ab4ccdf55e3b')
INSERT [dbo].[Familias] ([CodigoFamilia], [Nombre], [DVH]) VALUES (10, N'AdministrarBitacora', N'8e573e13dde0eb194257f0c747ec8634f35361948b735de7f9d6fdee227bd534')
INSERT [dbo].[Familias] ([CodigoFamilia], [Nombre], [DVH]) VALUES (15, N'Hijo', N'7e05c3e5fed95fbb9ad2220ac701cc72d27bdcf6735436aa7b30144fe973b884')
INSERT [dbo].[Familias] ([CodigoFamilia], [Nombre], [DVH]) VALUES (16, N'Padre', N'a34344d1c87383844d27872df9b91c1cc604c62b752163a8d6c0118275f1c427')
INSERT [dbo].[Familias] ([CodigoFamilia], [Nombre], [DVH]) VALUES (19, N'abue prueba', N'ac30c4c826162934ed8654bc5517323fdb85343d9bf3b12cc48d3d21ac3b3c08')
SET IDENTITY_INSERT [dbo].[Familias] OFF
GO
SET IDENTITY_INSERT [dbo].[Patentes] ON 

INSERT [dbo].[Patentes] ([CodigoPatente], [Nombre], [DVH]) VALUES (1, N'Crear usuario', N'98178bb1cd4ff706ba30edd632cc9601eb59b8076d8b0606a6cc43136a339a51')
INSERT [dbo].[Patentes] ([CodigoPatente], [Nombre], [DVH]) VALUES (2, N'Modificar usuario', N'001286a32ca7f1f4a3190e1e6c1e6cf08f0dc6a875c5c004d6e85b07bf121759')
INSERT [dbo].[Patentes] ([CodigoPatente], [Nombre], [DVH]) VALUES (3, N'Habilitar usuario', N'3355b10aff631aed14bc4e07ded558022400f77c25b2e4821c8e76d25f529ecb')
INSERT [dbo].[Patentes] ([CodigoPatente], [Nombre], [DVH]) VALUES (4, N'Deshabilitar usuario', N'bc09253874dbc8279bf6e6ea0f837a2fcb7ca1a6a753af077489db5b0ba8a01b')
INSERT [dbo].[Patentes] ([CodigoPatente], [Nombre], [DVH]) VALUES (5, N'Desbloquear usuario', N'eca24c30f34d9c57986b21bda2b14b7740901aacf8c8c1ab60d568c702495e54')
INSERT [dbo].[Patentes] ([CodigoPatente], [Nombre], [DVH]) VALUES (6, N'Ver bitacora de eventos', N'b1758738ad9a7abf029e5a04ce55aea9258016c218e07b31219f3a1eee63d83e')
INSERT [dbo].[Patentes] ([CodigoPatente], [Nombre], [DVH]) VALUES (7, N'Cerrar sesion', N'4f496c0d9c6ebc384243755f2055b103df4997a484ff75b5161dffbba796f7c4')
INSERT [dbo].[Patentes] ([CodigoPatente], [Nombre], [DVH]) VALUES (8, N'ReLogin', N'd93dbfa4e04e72beab00b71315a8559fc353614ddbaf68f1f42595ef270c75b4')
INSERT [dbo].[Patentes] ([CodigoPatente], [Nombre], [DVH]) VALUES (9, N'Ayuda', N'e816342ecf57262b2104d24f7a5c16ab9b7381abef3d97e4744ae7c6abe04f38')
INSERT [dbo].[Patentes] ([CodigoPatente], [Nombre], [DVH]) VALUES (10, N'Cambiar Contraseña', N'0b8c584239a57e68fcd01e8e9a0ea1947ba32be9ed70926b46ce9851a57aad77')
INSERT [dbo].[Patentes] ([CodigoPatente], [Nombre], [DVH]) VALUES (14, N'Gestion de usuarios', N'f56e8755a285740993d3bf3965795bf12ca89b07a17eef278fba46ca8a1f8873')
INSERT [dbo].[Patentes] ([CodigoPatente], [Nombre], [DVH]) VALUES (15, N'Ver usuarios', N'd6d5d1b580db491a6acf82c3fb4d256f59101ed5226c802ee27c4a1e2125dca8')
INSERT [dbo].[Patentes] ([CodigoPatente], [Nombre], [DVH]) VALUES (16, N'Gestion de familias', N'1dae0956e5d9859728fe8013b0eb8a3c392073e954d898a1cfd337589285b66d')
INSERT [dbo].[Patentes] ([CodigoPatente], [Nombre], [DVH]) VALUES (17, N'Ver familias', N'3d46f6f4e3b5a343ac501a7f1df3ab0f052b780b49706b1ad4875d081364e5be')
INSERT [dbo].[Patentes] ([CodigoPatente], [Nombre], [DVH]) VALUES (18, N'Crear familia', N'68d27c53938da21ee9b5c99fb3b5000ec01aa7c1ffcfca4e61d757f2a372d2a7')
INSERT [dbo].[Patentes] ([CodigoPatente], [Nombre], [DVH]) VALUES (19, N'Eliminar familia', N'ddfe8d06e359ee3b4af8103379c6a4a50cfea6f2928aacc6c779614b9d373fb3')
INSERT [dbo].[Patentes] ([CodigoPatente], [Nombre], [DVH]) VALUES (20, N'Agregar patente a familia', N'6a135037eef6ac4c3ea8da69165e46154b62b615490230cca1f82524fcbc8ef5')
INSERT [dbo].[Patentes] ([CodigoPatente], [Nombre], [DVH]) VALUES (21, N'Quitar patente de familia', N'875db51724a21b3d21beb1f908e43a9f0362860644cb1a9753ba710154993a6c')
INSERT [dbo].[Patentes] ([CodigoPatente], [Nombre], [DVH]) VALUES (22, N'Agregar subfamilia', N'21851f8679a9c75acc5b27351893e153fba7f1c68b88fb49499fc3037fa474b7')
INSERT [dbo].[Patentes] ([CodigoPatente], [Nombre], [DVH]) VALUES (23, N'Quitar subfamilia', N'bf02190defc50bb6ad193fbf1dbb04a81d023bb069fbc43b0db7662d4bd1fb5a')
INSERT [dbo].[Patentes] ([CodigoPatente], [Nombre], [DVH]) VALUES (24, N'Gestion de roles', N'4a29bead2f4abd82d703d04e50159c645d602044e834810b670de89e7e513c99')
INSERT [dbo].[Patentes] ([CodigoPatente], [Nombre], [DVH]) VALUES (25, N'Ver roles', N'0cc308af79d2a2ed794fd1c7b69b3f81c80588041e4cfc71831ac89af6aa6029')
INSERT [dbo].[Patentes] ([CodigoPatente], [Nombre], [DVH]) VALUES (26, N'Ver permisos efectivos de rol', N'b1099fbfd6f4e08c2531f548c81d215c3b4f08ea53205221a5517730e5ac4af2')
INSERT [dbo].[Patentes] ([CodigoPatente], [Nombre], [DVH]) VALUES (27, N'Agregar familia a rol', N'32a2447ba5dd287daa3b244e0130317e5905ad611ef027180f79838c05e54596')
INSERT [dbo].[Patentes] ([CodigoPatente], [Nombre], [DVH]) VALUES (28, N'Quitar familia de rol', N'696b5095c897b7b26c0f72ee4227d93bcf2a42346f39a6afee2832f6848798a2')
INSERT [dbo].[Patentes] ([CodigoPatente], [Nombre], [DVH]) VALUES (29, N'Asignar patente a rol', N'70e95b88b31dacf7bb679110cad8296bde5953e4dc84b7df2577a9e0443ae55c')
INSERT [dbo].[Patentes] ([CodigoPatente], [Nombre], [DVH]) VALUES (30, N'Quitar patente de rol', N'10dc9a4793d699bc73ebd73fe8be33c1ba313f7f9f78101f497ba7d9bfe52b29')
INSERT [dbo].[Patentes] ([CodigoPatente], [Nombre], [DVH]) VALUES (31, N'Consultar bitacora de eventos', N'e0a22a429b7deae30d3021d24a332820faf81eb1eb69785add0d4c8228584320')
INSERT [dbo].[Patentes] ([CodigoPatente], [Nombre], [DVH]) VALUES (32, N'Filtrar bitacora de eventos', N'c88bfe08bdfb7c90172f3643034bd7955e0d06fb0b2018462a05257422078f49')
INSERT [dbo].[Patentes] ([CodigoPatente], [Nombre], [DVH]) VALUES (33, N'Limpiar filtros de bitacora', N'9e41dd9d60548fcef6168e6dd3b479c4e4d7be01573caa6b7cbade1c12fde898')
INSERT [dbo].[Patentes] ([CodigoPatente], [Nombre], [DVH]) VALUES (34, N'Exportar bitacora a PDF', N'16e74a4036fd0c20cf7fa125a39f2961c4b35327c3a63d0242b46d2db2930176')
INSERT [dbo].[Patentes] ([CodigoPatente], [Nombre], [DVH]) VALUES (35, N'GestionAdmin', N'e873447dead71fe04e7d4e18223d1b631756e0742d4d31f1319c33411c2f9c2f')
INSERT [dbo].[Patentes] ([CodigoPatente], [Nombre], [DVH]) VALUES (36, N'EliminarRol', N'0a54804137485b8f83e89e4f8f228486a7fbf42beeae9f934170530f568ba4e7')
INSERT [dbo].[Patentes] ([CodigoPatente], [Nombre], [DVH]) VALUES (37, N'Cambiar Idioma', N'a171accd8b6ea3ff8ef4e970ab40cda0078e18d70363575f800743d25650d66f')
INSERT [dbo].[Patentes] ([CodigoPatente], [Nombre], [DVH]) VALUES (38, N'EjecutarBackup', N'f7ca0d296cd3a44490af198fedae968e8af0b2f9fcc46716b5760f54dd8f5b30')
INSERT [dbo].[Patentes] ([CodigoPatente], [Nombre], [DVH]) VALUES (39, N'EjecutarRestore', N'99765682d30f78e31d2f39ac19f26d4da45876782faac78bcd9daede83aa4010')
INSERT [dbo].[Patentes] ([CodigoPatente], [Nombre], [DVH]) VALUES (40, N'RecalcularHashes', N'55f4f57af9007897c3851de51ca8696c61744a4461ce6482a86376c52f628813')
SET IDENTITY_INSERT [dbo].[Patentes] OFF
GO
SET IDENTITY_INSERT [dbo].[Roles] ON 

INSERT [dbo].[Roles] ([CodigoRol], [Nombre], [DVH]) VALUES (1, N'Admin', N'b1fc372b2b8e30f8c8ce3d861f3d3123cbc216167f29ad580c3ef0a2981488cd')
INSERT [dbo].[Roles] ([CodigoRol], [Nombre], [DVH]) VALUES (2, N'RolSimple', N'bf83688350fff83d3aaf8f31ec21ef581034cb83e068cc8f6d169e96d8b04a03')
INSERT [dbo].[Roles] ([CodigoRol], [Nombre], [DVH]) VALUES (3, N'RolAdminSinAuditoria', N'9c3abce1b30296f148d191f4cf7a32e60215604833ff67523a3e3273aaea6ad4')
INSERT [dbo].[Roles] ([CodigoRol], [Nombre], [DVH]) VALUES (4, N'SuperAdmin', N'b07537540ff0e84763567b81c54c800e460e61d72be87a49d59cd88effb6e525')
INSERT [dbo].[Roles] ([CodigoRol], [Nombre], [DVH]) VALUES (12, N'PruebaAdmin', N'b61f21048ef6375350cab9cf1eddea775440a0f7bc7f8350c3a8cf599b557646')
SET IDENTITY_INSERT [dbo].[Roles] OFF
GO
INSERT [dbo].[RolFamilia] ([CodigoRol], [CodigoFamilia], [DVH]) VALUES (2, 5, N'03ddb99028eb7f8c417a4c0d0c2af1e97e75d872f73a81cb4b20d194b418a518')
INSERT [dbo].[RolFamilia] ([CodigoRol], [CodigoFamilia], [DVH]) VALUES (3, 1, N'a9288e59aa963e2177bf60f4a8ed2cae67a9e0d70c285011f17907dbaad47c85')
INSERT [dbo].[RolFamilia] ([CodigoRol], [CodigoFamilia], [DVH]) VALUES (3, 5, N'4c45462aecbc25b2698f904eeee4282b021aaab1d1cb410ee8b4b834ca8f93ca')
INSERT [dbo].[RolFamilia] ([CodigoRol], [CodigoFamilia], [DVH]) VALUES (4, 1, N'9610aa9dface0816bbf1fc9d38e401aed274a6ad7f1073223496c504b8a303ce')
INSERT [dbo].[RolFamilia] ([CodigoRol], [CodigoFamilia], [DVH]) VALUES (4, 5, N'5e9b92425a0b53de447ed38edcdc09da248003a41861009c018e74cd5c9badc6')
INSERT [dbo].[RolFamilia] ([CodigoRol], [CodigoFamilia], [DVH]) VALUES (4, 7, N'0481f06f493ea1104cc0eb7bbe62f4ffb9a571396cbacea258f2e28b58a409d1')
INSERT [dbo].[RolFamilia] ([CodigoRol], [CodigoFamilia], [DVH]) VALUES (4, 8, N'64c7b6fc0284fb593eb595039a729b87c671290343c2c1febd82a33174ef46f5')
INSERT [dbo].[RolFamilia] ([CodigoRol], [CodigoFamilia], [DVH]) VALUES (4, 10, N'bf668ce9f9bedb034680d803043abebd849b1c968847b14e1913862b7f03c9f8')
INSERT [dbo].[RolFamilia] ([CodigoRol], [CodigoFamilia], [DVH]) VALUES (12, 2, N'c8da1b47b9625fd59499d843bbb575be44aba070d69eac6205f35170540bef44')
INSERT [dbo].[RolFamilia] ([CodigoRol], [CodigoFamilia], [DVH]) VALUES (12, 5, N'68897aba2650cd2bce4f1d53ee75a5abe1542f6ad85cb2b7c69da5d92cec885f')
INSERT [dbo].[RolFamilia] ([CodigoRol], [CodigoFamilia], [DVH]) VALUES (12, 7, N'3ba32c9421dd8aba7236471894241a64e12a813d850141b6d6d8bf94ebea66bf')
GO
INSERT [dbo].[RolPatente] ([CodigoRol], [CodigoPatente], [DVH]) VALUES (1, 1, N'cc3793c04f34e113510979abb2673398b57775190af8edc45728456913c430e0')
INSERT [dbo].[RolPatente] ([CodigoRol], [CodigoPatente], [DVH]) VALUES (1, 2, N'fb46410db6c53827512925b844dfdce6c352157612d1c04064db507dbf1de158')
INSERT [dbo].[RolPatente] ([CodigoRol], [CodigoPatente], [DVH]) VALUES (1, 3, N'b33bb3d5836852c23e52e08aea2ee8506fcb854b325d6c8b7bd6efb27147a4c2')
INSERT [dbo].[RolPatente] ([CodigoRol], [CodigoPatente], [DVH]) VALUES (1, 4, N'f7a59aea87ac42f8fb2161e42dc2dcc9583a1f0f8a38c7ab0a7170f231e194a7')
INSERT [dbo].[RolPatente] ([CodigoRol], [CodigoPatente], [DVH]) VALUES (1, 5, N'fac0a4c3ab7cd8fa07e856afcb2bd3facde724e5f3d94b72c8360906e4949a1f')
INSERT [dbo].[RolPatente] ([CodigoRol], [CodigoPatente], [DVH]) VALUES (1, 6, N'ff10be3fcea367f8c698d111e6aacf41bd55a75899be6b75a6f5857222d51fac')
INSERT [dbo].[RolPatente] ([CodigoRol], [CodigoPatente], [DVH]) VALUES (1, 7, N'fe4aeef375b329684b8442a3b73507be91ff69f273b673d66d1dc8b8b27ab436')
INSERT [dbo].[RolPatente] ([CodigoRol], [CodigoPatente], [DVH]) VALUES (1, 8, N'92ae7f03923d5f84527c0a9f188ef86ee9e2c621d539536fe841a06b40cd7e17')
INSERT [dbo].[RolPatente] ([CodigoRol], [CodigoPatente], [DVH]) VALUES (1, 9, N'e1966da5760a9a49e9efff4c1c0c6e518929ae82ea4405966c7273b826d74ef0')
INSERT [dbo].[RolPatente] ([CodigoRol], [CodigoPatente], [DVH]) VALUES (1, 10, N'6d5c14099857b3087424c8c779e5495bdddae0dce416226a99d49a906543e6b9')
INSERT [dbo].[RolPatente] ([CodigoRol], [CodigoPatente], [DVH]) VALUES (1, 14, N'5dfe2802b73785e751a95c1ed698a3c81b6fe423c5ea242bd24981870ef8d914')
INSERT [dbo].[RolPatente] ([CodigoRol], [CodigoPatente], [DVH]) VALUES (1, 15, N'2563a9c6b55af385a80ccb9315e212fe1b66938876671166a652e53be835e7b7')
INSERT [dbo].[RolPatente] ([CodigoRol], [CodigoPatente], [DVH]) VALUES (1, 16, N'e8392a7d18c9f0ec6bb6c858b40e8cb25f443164d08e3d7d14fa2150597bc7a3')
INSERT [dbo].[RolPatente] ([CodigoRol], [CodigoPatente], [DVH]) VALUES (1, 17, N'040d022c7b97f357707a66bf07c497ded402b9ba35d56663f4b549d2edc920c6')
INSERT [dbo].[RolPatente] ([CodigoRol], [CodigoPatente], [DVH]) VALUES (1, 18, N'7a2f9cd8af4d2eff3f034c66ff0646d6f7afe757432a29876caefff35d78e366')
INSERT [dbo].[RolPatente] ([CodigoRol], [CodigoPatente], [DVH]) VALUES (1, 19, N'401cda189f740e91625b947c1e49a90779de8c7a87647593563db388e64c7129')
INSERT [dbo].[RolPatente] ([CodigoRol], [CodigoPatente], [DVH]) VALUES (1, 20, N'09cf073ec3a43889a632c5f807718da05d78305fe23d30e4ae413c71486ddbc2')
INSERT [dbo].[RolPatente] ([CodigoRol], [CodigoPatente], [DVH]) VALUES (1, 21, N'f52bf93f95e893c4e1fdde27d5f1414961084734b29161dc38bc18a43f651f9e')
INSERT [dbo].[RolPatente] ([CodigoRol], [CodigoPatente], [DVH]) VALUES (1, 22, N'cb61d29e9bf7bbb7f74714fe169470d7a7dfae5a57d19decbe72e6414864d2d9')
INSERT [dbo].[RolPatente] ([CodigoRol], [CodigoPatente], [DVH]) VALUES (1, 23, N'3465ea84e3f7d2198389ffb19f44cd41f3dfdb5c2bc3eee0acdced07eb0e9fca')
INSERT [dbo].[RolPatente] ([CodigoRol], [CodigoPatente], [DVH]) VALUES (1, 24, N'c7e93d01f0a88fdfe171941a4d761f0158e7a6e73cc67efa2c47551ffbf15aaa')
INSERT [dbo].[RolPatente] ([CodigoRol], [CodigoPatente], [DVH]) VALUES (1, 25, N'1807866806df494dbd0ce35aac2b4fcf6a5bcb84534d4b3d82499b610dc1876b')
INSERT [dbo].[RolPatente] ([CodigoRol], [CodigoPatente], [DVH]) VALUES (1, 26, N'8d8ce9f3a7c6006b99236e22c03132bd87ad7580e5626f07b47d2ed80812ca8e')
INSERT [dbo].[RolPatente] ([CodigoRol], [CodigoPatente], [DVH]) VALUES (1, 27, N'95869330a7d0bac4416c5f2611c4209c32c3ccfe0b587e70b5081f3db7762d59')
INSERT [dbo].[RolPatente] ([CodigoRol], [CodigoPatente], [DVH]) VALUES (1, 28, N'd0af3bbbfd02609244ced2cbc7abc15bfd7cb6138f9d07a59c8cf653891a2155')
INSERT [dbo].[RolPatente] ([CodigoRol], [CodigoPatente], [DVH]) VALUES (1, 29, N'898f1a13d1d68cbe04fef57ead3aca0849c707d72bf299cfa74ead69c15af027')
INSERT [dbo].[RolPatente] ([CodigoRol], [CodigoPatente], [DVH]) VALUES (1, 30, N'992f9eb0c3c7f79d678e7351147f7cc85d08f9b5ff4efad700893478fae9b50e')
INSERT [dbo].[RolPatente] ([CodigoRol], [CodigoPatente], [DVH]) VALUES (1, 31, N'5e1f1e77a2ee557719ba20c676a9d41b27ba6285dce541bbc280c5ef81e3696e')
INSERT [dbo].[RolPatente] ([CodigoRol], [CodigoPatente], [DVH]) VALUES (1, 32, N'2e0c12eff6f6fe687c7e210934b4adcf59391913d9c8afe23e82dd58b46a035e')
INSERT [dbo].[RolPatente] ([CodigoRol], [CodigoPatente], [DVH]) VALUES (1, 33, N'aaeb20b8b6cc40e7d92e4d01d1d6618697c4f5f356df2fdecf70bd2d1ffdd4c9')
INSERT [dbo].[RolPatente] ([CodigoRol], [CodigoPatente], [DVH]) VALUES (1, 34, N'59e2db0d465ec033e7c0dbe31a20d83c32504e9f4863f4e7f5667d53f83c8757')
INSERT [dbo].[RolPatente] ([CodigoRol], [CodigoPatente], [DVH]) VALUES (1, 35, N'00b61b8ef931c4ab6446f5eed2a9a26836763387120631ccdb2a02b15627f217')
INSERT [dbo].[RolPatente] ([CodigoRol], [CodigoPatente], [DVH]) VALUES (1, 36, N'9dd27fbd89fc3d97180e9c524f547c24d172d7a1635e791486136472a06b1d1a')
INSERT [dbo].[RolPatente] ([CodigoRol], [CodigoPatente], [DVH]) VALUES (1, 38, N'c38d3a53a0244e44764f9ee98962c22de196d8bd7d8723a6e09f9b112fb69f97')
INSERT [dbo].[RolPatente] ([CodigoRol], [CodigoPatente], [DVH]) VALUES (1, 39, N'f33d48f3a91783741f4e973cff9d946755b56b472fc1977f1010fa67a1da3cab')
INSERT [dbo].[RolPatente] ([CodigoRol], [CodigoPatente], [DVH]) VALUES (1, 40, N'3a0ab4dbf7ba109bef0fad444a58f4758ca43eda313d91af62e96824891e29ab')
INSERT [dbo].[RolPatente] ([CodigoRol], [CodigoPatente], [DVH]) VALUES (4, 30, N'8fe458e7880382464168ef622b9b647b214405cab6a37dbefeeddf57eb0a1fdc')
INSERT [dbo].[RolPatente] ([CodigoRol], [CodigoPatente], [DVH]) VALUES (4, 38, N'0302a3d3ee1b1f41866ccebc5dc3e305858d0e81f25fcc36c261ba56071fc422')
INSERT [dbo].[RolPatente] ([CodigoRol], [CodigoPatente], [DVH]) VALUES (4, 39, N'69c1c66d7f041e57ea397515ccd0e466f7bbb62eb88cb2329376597b86da9ba9')
INSERT [dbo].[RolPatente] ([CodigoRol], [CodigoPatente], [DVH]) VALUES (4, 40, N'c70d4cde7960ee3a04b08019750bff29103c76becb965d1e55ae1cfe059965fc')
GO
INSERT [dbo].[Usuarios] ([DNI], [Nombre], [Apellido], [Email], [Bloqueado], [Contrasena], [Username], [Activo], [CodigoRol], [IntentosRealizados], [FechaUltimoIntento], [IdiomaId], [DVH]) VALUES (N'12345672', N'Nahuel', N'Sanche', N'error@test.com                                                                                                                                                                                          ', 0, N'1cdcfc3885f538bc4c6ef194013dc7450af9a44e26cfa2a4d0f89344406caf26', N'12345672Nahuel', 1, 2, 0, NULL, N'es-AR', N'41ccf7c458f031f91189da441c2761688f160a79d6257d6c3ca9642de51c527f')
INSERT [dbo].[Usuarios] ([DNI], [Nombre], [Apellido], [Email], [Bloqueado], [Contrasena], [Username], [Activo], [CodigoRol], [IntentosRealizados], [FechaUltimoIntento], [IdiomaId], [DVH]) VALUES (N'124231241', N'fas', N'asf', N'asf@gmail.com                                                                                                                                                                                           ', 0, N'd2689e1efc50095936ec5f2d18d28b66734d192b63ef64c15f42703ede94292b', N'124231241fas', 0, 1, 0, NULL, N'es-AR', N'f471ab8452cc81f69dabfc16d85c54e811949b3b870bec88ec93555d14caf2f2')
INSERT [dbo].[Usuarios] ([DNI], [Nombre], [Apellido], [Email], [Bloqueado], [Contrasena], [Username], [Activo], [CodigoRol], [IntentosRealizados], [FechaUltimoIntento], [IdiomaId], [DVH]) VALUES (N'23456789', N'as', N'as', N'AS@gmail.com                                                                                                                                                                                            ', 0, N'2194518bf4e3e64b7363f0768136569ae2f9204a44252623f85f9471048c2827', N'23456789as', 1, 1, 0, NULL, N'es-AR', N'1ad46763825ad78bfd573dcb445fa443a0ce4e219ea55ad747dd3742ff61f28d')
INSERT [dbo].[Usuarios] ([DNI], [Nombre], [Apellido], [Email], [Bloqueado], [Contrasena], [Username], [Activo], [CodigoRol], [IntentosRealizados], [FechaUltimoIntento], [IdiomaId], [DVH]) VALUES (N'46905013', N'Lucas', N'Molinari', N'lucas@gmail.com                                                                                                                                                                                         ', 0, N'934797de82eee77c0eca90f94d93cdfe9b4f51d41d80aa49d6f130b0caa3709b', N'46905013Lucas', 1, 12, 0, NULL, N'es-AR', N'43f8a333aef919829cb63bb9fd1455c6350a09880b08c5e186a4e60ec8b660ff')
INSERT [dbo].[Usuarios] ([DNI], [Nombre], [Apellido], [Email], [Bloqueado], [Contrasena], [Username], [Activo], [CodigoRol], [IntentosRealizados], [FechaUltimoIntento], [IdiomaId], [DVH]) VALUES (N'46948668', N'Maximo', N'Kirichuk', N'MaximoKirichuk@gmail.com                                                                                                                                                                                ', 0, N'f3af358d1bca104bf69aaf41ee785504f50d22ea78cbce4d060268db618b4e97', N'46948668Maximo', 1, 4, 0, NULL, N'es-AR', N'258a86db838ad04edb1ade00cd51916aeb73936a5bb973473b843c90b1f0ca03')
INSERT [dbo].[Usuarios] ([DNI], [Nombre], [Apellido], [Email], [Bloqueado], [Contrasena], [Username], [Activo], [CodigoRol], [IntentosRealizados], [FechaUltimoIntento], [IdiomaId], [DVH]) VALUES (N'47006530', N'Benjamin', N'Aguila', N'benjaaguila6@gmail.com                                                                                                                                                                                  ', 0, N'21481fa29aa0a6048019b0e3690484221ceed8d77bf4f9c14721c7f56a5035ba', N'47006530Benjamin', 1, 2, 0, NULL, N'es-AR', N'398ab007324ab388e4f43a9978523f8eabb0efb834d9719306c80f6c6efb2b1a')
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [UQ_Familias_Nombre]    Script Date: 7/7/2026 11:12:27 PM ******/
ALTER TABLE [dbo].[Familias] ADD  CONSTRAINT [UQ_Familias_Nombre] UNIQUE NONCLUSTERED 
(
	[Nombre] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [UQ_Patentes_Nombre]    Script Date: 7/7/2026 11:12:27 PM ******/
ALTER TABLE [dbo].[Patentes] ADD  CONSTRAINT [UQ_Patentes_Nombre] UNIQUE NONCLUSTERED 
(
	[Nombre] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [UQ__Roles__75E3EFCF6E3FB7D0]    Script Date: 7/7/2026 11:12:27 PM ******/
ALTER TABLE [dbo].[Roles] ADD  CONSTRAINT [UQ__Roles__75E3EFCF6E3FB7D0] UNIQUE NONCLUSTERED 
(
	[Nombre] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [IX_Usuarios]    Script Date: 7/7/2026 11:12:27 PM ******/
ALTER TABLE [dbo].[Usuarios] ADD  CONSTRAINT [IX_Usuarios] UNIQUE NONCLUSTERED 
(
	[Username] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [IX_Usuarios_Email]    Script Date: 7/7/2026 11:12:27 PM ******/
ALTER TABLE [dbo].[Usuarios] ADD  CONSTRAINT [IX_Usuarios_Email] UNIQUE NONCLUSTERED 
(
	[Email] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
ALTER TABLE [dbo].[DigitoVerificador_83KI] ADD  CONSTRAINT [DF_DigitoVerificador_Fecha]  DEFAULT (getdate()) FOR [FechaActualizacion]
GO
ALTER TABLE [dbo].[FamiliaFamilia] ADD  CONSTRAINT [DF_FamiliaFamilia_DVH]  DEFAULT ('') FOR [DVH]
GO
ALTER TABLE [dbo].[FamiliaPatente] ADD  CONSTRAINT [DF_FamiliaPatente_DVH]  DEFAULT ('') FOR [DVH]
GO
ALTER TABLE [dbo].[Familias] ADD  CONSTRAINT [DF_Familias_DVH]  DEFAULT ('') FOR [DVH]
GO
ALTER TABLE [dbo].[Patentes] ADD  CONSTRAINT [DF_Patentes_DVH]  DEFAULT ('') FOR [DVH]
GO
ALTER TABLE [dbo].[Roles] ADD  CONSTRAINT [DF_Roles_DVH]  DEFAULT ('') FOR [DVH]
GO
ALTER TABLE [dbo].[RolFamilia] ADD  CONSTRAINT [DF_RolFamilia_DVH]  DEFAULT ('') FOR [DVH]
GO
ALTER TABLE [dbo].[RolPatente] ADD  CONSTRAINT [DF_RolPatente_DVH]  DEFAULT ('') FOR [DVH]
GO
ALTER TABLE [dbo].[Usuarios] ADD  CONSTRAINT [DF_Usuarios_Activo]  DEFAULT ((1)) FOR [Activo]
GO
ALTER TABLE [dbo].[Usuarios] ADD  CONSTRAINT [DF_Usuarios_Intentos]  DEFAULT ((0)) FOR [IntentosRealizados]
GO
ALTER TABLE [dbo].[Usuarios] ADD  CONSTRAINT [DF_Usuarios_IdiomaId]  DEFAULT ('es-AR') FOR [IdiomaId]
GO
ALTER TABLE [dbo].[Usuarios] ADD  CONSTRAINT [DF_Usuarios_DVH]  DEFAULT ('') FOR [DVH]
GO
ALTER TABLE [dbo].[BitacoraEventos]  WITH CHECK ADD  CONSTRAINT [FK_BitacoraEventos_BitacoraEventos] FOREIGN KEY([Username])
REFERENCES [dbo].[Usuarios] ([Username])
GO
ALTER TABLE [dbo].[BitacoraEventos] CHECK CONSTRAINT [FK_BitacoraEventos_BitacoraEventos]
GO
ALTER TABLE [dbo].[FamiliaFamilia]  WITH CHECK ADD  CONSTRAINT [FK_FamiliaFamilia_Hija] FOREIGN KEY([CodigoFamiliaHija])
REFERENCES [dbo].[Familias] ([CodigoFamilia])
GO
ALTER TABLE [dbo].[FamiliaFamilia] CHECK CONSTRAINT [FK_FamiliaFamilia_Hija]
GO
ALTER TABLE [dbo].[FamiliaFamilia]  WITH CHECK ADD  CONSTRAINT [FK_FamiliaFamilia_Padre] FOREIGN KEY([CodigoFamiliaPadre])
REFERENCES [dbo].[Familias] ([CodigoFamilia])
GO
ALTER TABLE [dbo].[FamiliaFamilia] CHECK CONSTRAINT [FK_FamiliaFamilia_Padre]
GO
ALTER TABLE [dbo].[FamiliaPatente]  WITH CHECK ADD  CONSTRAINT [FK_FamiliaPatente_Familias] FOREIGN KEY([CodigoFamilia])
REFERENCES [dbo].[Familias] ([CodigoFamilia])
GO
ALTER TABLE [dbo].[FamiliaPatente] CHECK CONSTRAINT [FK_FamiliaPatente_Familias]
GO
ALTER TABLE [dbo].[FamiliaPatente]  WITH CHECK ADD  CONSTRAINT [FK_FamiliaPatente_Patentes] FOREIGN KEY([CodigoPatente])
REFERENCES [dbo].[Patentes] ([CodigoPatente])
GO
ALTER TABLE [dbo].[FamiliaPatente] CHECK CONSTRAINT [FK_FamiliaPatente_Patentes]
GO
ALTER TABLE [dbo].[RolFamilia]  WITH CHECK ADD  CONSTRAINT [FK_RolFamilia_Familias] FOREIGN KEY([CodigoFamilia])
REFERENCES [dbo].[Familias] ([CodigoFamilia])
GO
ALTER TABLE [dbo].[RolFamilia] CHECK CONSTRAINT [FK_RolFamilia_Familias]
GO
ALTER TABLE [dbo].[RolFamilia]  WITH CHECK ADD  CONSTRAINT [FK_RolFamilia_Roles] FOREIGN KEY([CodigoRol])
REFERENCES [dbo].[Roles] ([CodigoRol])
GO
ALTER TABLE [dbo].[RolFamilia] CHECK CONSTRAINT [FK_RolFamilia_Roles]
GO
ALTER TABLE [dbo].[RolPatente]  WITH CHECK ADD  CONSTRAINT [FK_RolPatente_Patentes] FOREIGN KEY([CodigoPatente])
REFERENCES [dbo].[Patentes] ([CodigoPatente])
GO
ALTER TABLE [dbo].[RolPatente] CHECK CONSTRAINT [FK_RolPatente_Patentes]
GO
ALTER TABLE [dbo].[RolPatente]  WITH CHECK ADD  CONSTRAINT [FK_RolPatente_Roles] FOREIGN KEY([CodigoRol])
REFERENCES [dbo].[Roles] ([CodigoRol])
GO
ALTER TABLE [dbo].[RolPatente] CHECK CONSTRAINT [FK_RolPatente_Roles]
GO
ALTER TABLE [dbo].[Usuarios]  WITH CHECK ADD  CONSTRAINT [FK_Usuarios_Roles] FOREIGN KEY([CodigoRol])
REFERENCES [dbo].[Roles] ([CodigoRol])
GO
ALTER TABLE [dbo].[Usuarios] CHECK CONSTRAINT [FK_Usuarios_Roles]
GO
ALTER TABLE [dbo].[FamiliaFamilia]  WITH CHECK ADD  CONSTRAINT [CK_FamiliaFamilia_NoAutoReferencia] CHECK  (([CodigoFamiliaPadre]<>[CodigoFamiliaHija]))
GO
ALTER TABLE [dbo].[FamiliaFamilia] CHECK CONSTRAINT [CK_FamiliaFamilia_NoAutoReferencia]
GO
USE [master]
GO
ALTER DATABASE [GestionUsuarios] SET  READ_WRITE 
GO
