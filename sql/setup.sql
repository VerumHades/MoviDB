USE [master]
GO
/****** Object:  Database [MoviesDB]    Script Date: 08.01.2026 20:58:55 ******/
CREATE DATABASE [MoviesDB]
GO
ALTER DATABASE [MoviesDB] SET COMPATIBILITY_LEVEL = 160
GO
IF (1 = FULLTEXTSERVICEPROPERTY('IsFullTextInstalled'))
begin
EXEC [MoviesDB].[dbo].[sp_fulltext_database] @action = 'enable'
end
GO
ALTER DATABASE [MoviesDB] SET ANSI_NULL_DEFAULT OFF 
GO
ALTER DATABASE [MoviesDB] SET ANSI_NULLS OFF 
GO
ALTER DATABASE [MoviesDB] SET ANSI_PADDING OFF 
GO
ALTER DATABASE [MoviesDB] SET ANSI_WARNINGS OFF 
GO
ALTER DATABASE [MoviesDB] SET ARITHABORT OFF 
GO
ALTER DATABASE [MoviesDB] SET AUTO_CLOSE ON 
GO
ALTER DATABASE [MoviesDB] SET AUTO_SHRINK OFF 
GO
ALTER DATABASE [MoviesDB] SET AUTO_UPDATE_STATISTICS ON 
GO
ALTER DATABASE [MoviesDB] SET CURSOR_CLOSE_ON_COMMIT OFF 
GO
ALTER DATABASE [MoviesDB] SET CURSOR_DEFAULT  GLOBAL 
GO
ALTER DATABASE [MoviesDB] SET CONCAT_NULL_YIELDS_NULL OFF 
GO
ALTER DATABASE [MoviesDB] SET NUMERIC_ROUNDABORT OFF 
GO
ALTER DATABASE [MoviesDB] SET QUOTED_IDENTIFIER OFF 
GO
ALTER DATABASE [MoviesDB] SET RECURSIVE_TRIGGERS OFF 
GO
ALTER DATABASE [MoviesDB] SET  ENABLE_BROKER 
GO
ALTER DATABASE [MoviesDB] SET AUTO_UPDATE_STATISTICS_ASYNC OFF 
GO
ALTER DATABASE [MoviesDB] SET DATE_CORRELATION_OPTIMIZATION OFF 
GO
ALTER DATABASE [MoviesDB] SET TRUSTWORTHY OFF 
GO
ALTER DATABASE [MoviesDB] SET ALLOW_SNAPSHOT_ISOLATION OFF 
GO
ALTER DATABASE [MoviesDB] SET PARAMETERIZATION SIMPLE 
GO
ALTER DATABASE [MoviesDB] SET READ_COMMITTED_SNAPSHOT OFF 
GO
ALTER DATABASE [MoviesDB] SET HONOR_BROKER_PRIORITY OFF 
GO
ALTER DATABASE [MoviesDB] SET RECOVERY SIMPLE 
GO
ALTER DATABASE [MoviesDB] SET  MULTI_USER 
GO
ALTER DATABASE [MoviesDB] SET PAGE_VERIFY CHECKSUM  
GO
ALTER DATABASE [MoviesDB] SET DB_CHAINING OFF 
GO
ALTER DATABASE [MoviesDB] SET FILESTREAM( NON_TRANSACTED_ACCESS = OFF ) 
GO
ALTER DATABASE [MoviesDB] SET TARGET_RECOVERY_TIME = 60 SECONDS 
GO
ALTER DATABASE [MoviesDB] SET DELAYED_DURABILITY = DISABLED 
GO
ALTER DATABASE [MoviesDB] SET ACCELERATED_DATABASE_RECOVERY = OFF  
GO
ALTER DATABASE [MoviesDB] SET QUERY_STORE = ON
GO
ALTER DATABASE [MoviesDB] SET QUERY_STORE (OPERATION_MODE = READ_WRITE, CLEANUP_POLICY = (STALE_QUERY_THRESHOLD_DAYS = 30), DATA_FLUSH_INTERVAL_SECONDS = 900, INTERVAL_LENGTH_MINUTES = 60, MAX_STORAGE_SIZE_MB = 1000, QUERY_CAPTURE_MODE = AUTO, SIZE_BASED_CLEANUP_MODE = AUTO, MAX_PLANS_PER_QUERY = 200, WAIT_STATS_CAPTURE_MODE = ON)
GO
USE [MoviesDB]
GO
/****** Object:  User [normal_user]    Script Date: 08.01.2026 20:58:55 ******/
CREATE USER [normal_user] FOR LOGIN [normal_user_login] WITH DEFAULT_SCHEMA=[dbo]
GO
/****** Object:  User [moderator_user]    Script Date: 08.01.2026 20:58:55 ******/
CREATE USER [moderator_user] FOR LOGIN [moderator_user_login] WITH DEFAULT_SCHEMA=[dbo]
GO
/****** Object:  User [library_manager]    Script Date: 08.01.2026 20:58:55 ******/
CREATE USER [library_manager] FOR LOGIN [library_manager_login] WITH DEFAULT_SCHEMA=[dbo]
GO
/****** Object:  Table [dbo].[genre]    Script Date: 08.01.2026 20:58:55 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[genre](
    [id] [int] IDENTITY(1,1) NOT NULL,
    [name] [nvarchar](255) NOT NULL,
    PRIMARY KEY CLUSTERED
(
[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
    ) ON [PRIMARY]
    GO
/****** Object:  Table [dbo].[media]    Script Date: 08.01.2026 20:58:55 ******/
    SET ANSI_NULLS ON
    GO
    SET QUOTED_IDENTIFIER ON
    GO
CREATE TABLE [dbo].[media](
    [id] [int] IDENTITY(1,1) NOT NULL,
    [created_at] [datetime] NOT NULL,
    [title] [nvarchar](255) NOT NULL,
    [description] [nvarchar](300) NULL,
    [type] [nvarchar](20) NOT NULL,
    [rating_count] [int] NOT NULL,
    [rating_sum] [float] NOT NULL,
    PRIMARY KEY CLUSTERED
(
[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
    ) ON [PRIMARY]
    GO
/****** Object:  Table [dbo].[movie]    Script Date: 08.01.2026 20:58:55 ******/
    SET ANSI_NULLS ON
    GO
    SET QUOTED_IDENTIFIER ON
    GO
CREATE TABLE [dbo].[movie](
    [media_id] [int] NOT NULL,
    [genre_id] [int] NOT NULL,
    [duration_minutes] [int] NULL,
     PRIMARY KEY CLUSTERED
    (
[media_id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
    ) ON [PRIMARY]
    GO
/****** Object:  View [dbo].[vw_movie]    Script Date: 08.01.2026 20:58:55 ******/
    SET ANSI_NULLS ON
    GO
    SET QUOTED_IDENTIFIER ON
    GO
CREATE VIEW [dbo].[vw_movie] AS
SELECT
    m.id AS media_id,
    m.title,
    m.description,
    g.id AS genre_id,
    g.name AS genre_name,
    m.rating_count,
    m.rating_sum,
    CASE WHEN m.rating_count > 0 THEN m.rating_sum / m.rating_count ELSE 0 END AS rating,
    mv.duration_minutes,
    m.created_at
FROM dbo.media m
         INNER JOIN dbo.movie mv ON mv.media_id = m.id
         INNER JOIN dbo.genre g ON g.id = mv.genre_id
WHERE m.type = 'movie';
GO
/****** Object:  Table [dbo].[series]    Script Date: 08.01.2026 20:58:55 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[series](
    [media_id] [int] NOT NULL,
    [genre_id] [int] NOT NULL,
     PRIMARY KEY CLUSTERED
    (
[media_id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
    ) ON [PRIMARY]
    GO
/****** Object:  Table [dbo].[season]    Script Date: 08.01.2026 20:58:55 ******/
    SET ANSI_NULLS ON
    GO
    SET QUOTED_IDENTIFIER ON
    GO
CREATE TABLE [dbo].[season](
    [id] [int] IDENTITY(1,1) NOT NULL,
    [series_id] [int] NOT NULL,
    [title] [nvarchar](255) NOT NULL,
    [number] [int] NOT NULL,
    PRIMARY KEY CLUSTERED
(
[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
    ) ON [PRIMARY]
    GO
/****** Object:  View [dbo].[vw_series]    Script Date: 08.01.2026 20:58:55 ******/
    SET ANSI_NULLS ON
    GO
    SET QUOTED_IDENTIFIER ON
    GO
CREATE VIEW [dbo].[vw_series] AS
SELECT
    s.media_id AS series_id,
    m.title AS title,
    m.description AS description,
    g.name AS genre_name,
    COUNT(se.id) AS season_count,
    m.rating_count AS rating_count,
    m.rating_sum AS rating_sum,
    CASE WHEN m.rating_count > 0 THEN m.rating_sum / m.rating_count ELSE 0 END AS rating,
    m.created_at AS created_at
FROM dbo.series s
         INNER JOIN dbo.media m ON s.media_id = m.id
         INNER JOIN dbo.genre g ON s.genre_id = g.id
         LEFT JOIN dbo.season se ON se.series_id = s.media_id
GROUP BY
    s.media_id,
    m.title,
    m.description,
    g.name,
    m.rating_count,
    m.rating_sum,
    m.created_at;
GO
/****** Object:  Table [dbo].[episode]    Script Date: 08.01.2026 20:58:55 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[episode](
    [id] [int] IDENTITY(1,1) NOT NULL,
    [season_id] [int] NOT NULL,
    [title] [nvarchar](255) NOT NULL,
    [episode_number] [int] NOT NULL,
    [created_at] [datetime] NOT NULL,
    PRIMARY KEY CLUSTERED
(
[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
    ) ON [PRIMARY]
    GO
/****** Object:  Table [dbo].[library_entry]    Script Date: 08.01.2026 20:58:55 ******/
    SET ANSI_NULLS ON
    GO
    SET QUOTED_IDENTIFIER ON
    GO
CREATE TABLE [dbo].[library_entry](
    [media_id] [int] NOT NULL,
    [user_id] [int] NOT NULL,
    [watched] [bit] NOT NULL,
    [created_at] [datetime] NOT NULL,
     CONSTRAINT [pk_library_entry] PRIMARY KEY CLUSTERED
    (
    [media_id] ASC,
[user_id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
    ) ON [PRIMARY]
    GO
/****** Object:  Table [dbo].[review]    Script Date: 08.01.2026 20:58:55 ******/
    SET ANSI_NULLS ON
    GO
    SET QUOTED_IDENTIFIER ON
    GO
CREATE TABLE [dbo].[review](
    [id] [int] IDENTITY(1,1) NOT NULL,
    [media_id] [int] NOT NULL,
    [user_id] [int] NOT NULL,
    [title] [nvarchar](255) NOT NULL,
    [content] [nvarchar](max) NOT NULL,
    [rating] [float] NOT NULL,
    [created_at] [datetime] NOT NULL,
    PRIMARY KEY CLUSTERED
(
[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
    ) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
    GO
/****** Object:  Table [dbo].[user]    Script Date: 08.01.2026 20:58:55 ******/
    SET ANSI_NULLS ON
    GO
    SET QUOTED_IDENTIFIER ON
    GO
CREATE TABLE [dbo].[user](
    [id] [int] IDENTITY(1,1) NOT NULL,
    [username] [nvarchar](255) NOT NULL,
    [password_hash] [nvarchar](255) NOT NULL,
    [role] [nvarchar](20) NOT NULL,
    [created_at] [datetime] NOT NULL,
    PRIMARY KEY CLUSTERED
(
[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
    ) ON [PRIMARY]
    GO
    SET IDENTITY_INSERT [dbo].[genre] ON
    GO
    INSERT [dbo].[genre] ([id], [name]) VALUES (3, N'Comedy')
    GO
    INSERT [dbo].[genre] ([id], [name]) VALUES (2, N'Drama')
    GO
    INSERT [dbo].[genre] ([id], [name]) VALUES (4, N'Horror')
    GO
    INSERT [dbo].[genre] ([id], [name]) VALUES (1, N'SciFi')
    GO
    SET IDENTITY_INSERT [dbo].[genre] OFF
    GO
    SET IDENTITY_INSERT [dbo].[media] ON
    GO
    INSERT [dbo].[media] ([id], [created_at], [title], [description], [type], [rating_count], [rating_sum]) VALUES (1, CAST(N'2026-01-08T20:58:05.787' AS DateTime), N'The Matrix', N'A computer hacker learns about the true nature of reality.', N'movie', 0, 0)
    GO
    INSERT [dbo].[media] ([id], [created_at], [title], [description], [type], [rating_count], [rating_sum]) VALUES (2, CAST(N'2026-01-08T20:58:05.800' AS DateTime), N'Inception', N'A thief who steals corporate secrets through dream-sharing technology.', N'movie', 0, 0)
    GO
    INSERT [dbo].[media] ([id], [created_at], [title], [description], [type], [rating_count], [rating_sum]) VALUES (3, CAST(N'2026-01-08T20:58:05.807' AS DateTime), N'Interstellar', N'Explorers travel through a wormhole in space to save humanity.', N'movie', 0, 0)
    GO
    INSERT [dbo].[media] ([id], [created_at], [title], [description], [type], [rating_count], [rating_sum]) VALUES (4, CAST(N'2026-01-08T20:58:05.810' AS DateTime), N'Blade Runner 2049', N'A young blade runner uncovers a long-buried secret that could plunge society into chaos.', N'movie', 0, 0)
    GO
    INSERT [dbo].[media] ([id], [created_at], [title], [description], [type], [rating_count], [rating_sum]) VALUES (5, CAST(N'2026-01-08T20:58:05.813' AS DateTime), N'The Godfather', N'The aging patriarch of an organized crime dynasty transfers control to his son.', N'movie', 0, 0)
    GO
    INSERT [dbo].[media] ([id], [created_at], [title], [description], [type], [rating_count], [rating_sum]) VALUES (6, CAST(N'2026-01-08T20:58:05.820' AS DateTime), N'Forrest Gump', N'The story of a man with a low IQ who achieves extraordinary things in life.', N'movie', 0, 0)
    GO
    INSERT [dbo].[media] ([id], [created_at], [title], [description], [type], [rating_count], [rating_sum]) VALUES (7, CAST(N'2026-01-08T20:58:05.820' AS DateTime), N'The Shawshank Redemption', N'Two imprisoned men bond over several years, finding solace and eventual redemption.', N'movie', 0, 0)
    GO
    INSERT [dbo].[media] ([id], [created_at], [title], [description], [type], [rating_count], [rating_sum]) VALUES (8, CAST(N'2026-01-08T20:58:05.827' AS DateTime), N'Parasite', N'A poor family schemes to become employed by a wealthy family.', N'movie', 0, 0)
    GO
    INSERT [dbo].[media] ([id], [created_at], [title], [description], [type], [rating_count], [rating_sum]) VALUES (9, CAST(N'2026-01-08T20:58:05.830' AS DateTime), N'Superbad', N'Two co-dependent high school seniors are forced to deal with separation anxiety.', N'movie', 0, 0)
    GO
    INSERT [dbo].[media] ([id], [created_at], [title], [description], [type], [rating_count], [rating_sum]) VALUES (10, CAST(N'2026-01-08T20:58:05.833' AS DateTime), N'The Hangover', N'Three friends lose the groom at a bachelor party in Las Vegas and must retrace their steps.', N'movie', 0, 0)
    GO
    INSERT [dbo].[media] ([id], [created_at], [title], [description], [type], [rating_count], [rating_sum]) VALUES (11, CAST(N'2026-01-08T20:58:05.837' AS DateTime), N'Step Brothers', N'Two middle-aged men become stepbrothers and struggle to coexist.', N'movie', 0, 0)
    GO
    INSERT [dbo].[media] ([id], [created_at], [title], [description], [type], [rating_count], [rating_sum]) VALUES (12, CAST(N'2026-01-08T20:58:05.840' AS DateTime), N'Anchorman', N'A top-rated news anchor in the 1970s struggles with a new female co-anchor.', N'movie', 0, 0)
    GO
    INSERT [dbo].[media] ([id], [created_at], [title], [description], [type], [rating_count], [rating_sum]) VALUES (13, CAST(N'2026-01-08T20:58:05.843' AS DateTime), N'The Conjuring', N'Paranormal investigators help a family terrorized by a dark presence.', N'movie', 0, 0)
    GO
    INSERT [dbo].[media] ([id], [created_at], [title], [description], [type], [rating_count], [rating_sum]) VALUES (14, CAST(N'2026-01-08T20:58:05.850' AS DateTime), N'It', N'A group of kids face the evil entity haunting their town every 27 years.', N'movie', 0, 0)
    GO
    INSERT [dbo].[media] ([id], [created_at], [title], [description], [type], [rating_count], [rating_sum]) VALUES (15, CAST(N'2026-01-08T20:58:05.853' AS DateTime), N'A Quiet Place', N'A family must live in silence while hiding from creatures that hunt by sound.', N'movie', 0, 0)
    GO
    INSERT [dbo].[media] ([id], [created_at], [title], [description], [type], [rating_count], [rating_sum]) VALUES (16, CAST(N'2026-01-08T20:58:05.857' AS DateTime), N'Get Out', N'A young African-American man uncovers disturbing secrets when meeting his white girlfriend''s family.', N'movie', 0, 0)
    GO
    INSERT [dbo].[media] ([id], [created_at], [title], [description], [type], [rating_count], [rating_sum]) VALUES (17, CAST(N'2026-01-08T20:58:05.860' AS DateTime), N'Mad Max: Fury Road', N'In a post-apocalyptic wasteland, a woman rebels against a tyrannical ruler.', N'movie', 0, 0)
    GO
    INSERT [dbo].[media] ([id], [created_at], [title], [description], [type], [rating_count], [rating_sum]) VALUES (18, CAST(N'2026-01-08T20:58:05.863' AS DateTime), N'Guardians of the Galaxy', N'A group of intergalactic criminals must save the universe.', N'movie', 0, 0)
    GO
    INSERT [dbo].[media] ([id], [created_at], [title], [description], [type], [rating_count], [rating_sum]) VALUES (19, CAST(N'2026-01-08T20:58:05.870' AS DateTime), N'Jojo Rabbit', N'A young boy in Nazi Germany discovers his mother is hiding a Jewish girl in their home.', N'movie', 0, 0)
    GO
    INSERT [dbo].[media] ([id], [created_at], [title], [description], [type], [rating_count], [rating_sum]) VALUES (20, CAST(N'2026-01-08T20:58:05.873' AS DateTime), N'Hereditary', N'A family is haunted after the death of their secretive grandmother.', N'movie', 0, 0)
    GO
    SET IDENTITY_INSERT [dbo].[media] OFF
    GO
    INSERT [dbo].[movie] ([media_id], [genre_id], [duration_minutes]) VALUES (1, 1, 136)
    GO
    INSERT [dbo].[movie] ([media_id], [genre_id], [duration_minutes]) VALUES (2, 1, 148)
    GO
    INSERT [dbo].[movie] ([media_id], [genre_id], [duration_minutes]) VALUES (3, 1, 169)
    GO
    INSERT [dbo].[movie] ([media_id], [genre_id], [duration_minutes]) VALUES (4, 1, 164)
    GO
    INSERT [dbo].[movie] ([media_id], [genre_id], [duration_minutes]) VALUES (5, 2, 175)
    GO
    INSERT [dbo].[movie] ([media_id], [genre_id], [duration_minutes]) VALUES (6, 2, 142)
    GO
    INSERT [dbo].[movie] ([media_id], [genre_id], [duration_minutes]) VALUES (7, 2, 142)
    GO
    INSERT [dbo].[movie] ([media_id], [genre_id], [duration_minutes]) VALUES (8, 2, 132)
    GO
    INSERT [dbo].[movie] ([media_id], [genre_id], [duration_minutes]) VALUES (9, 3, 113)
    GO
    INSERT [dbo].[movie] ([media_id], [genre_id], [duration_minutes]) VALUES (10, 3, 100)
    GO
    INSERT [dbo].[movie] ([media_id], [genre_id], [duration_minutes]) VALUES (11, 3, 98)
    GO
    INSERT [dbo].[movie] ([media_id], [genre_id], [duration_minutes]) VALUES (12, 3, 94)
    GO
    INSERT [dbo].[movie] ([media_id], [genre_id], [duration_minutes]) VALUES (13, 4, 112)
    GO
    INSERT [dbo].[movie] ([media_id], [genre_id], [duration_minutes]) VALUES (14, 4, 135)
    GO
    INSERT [dbo].[movie] ([media_id], [genre_id], [duration_minutes]) VALUES (15, 4, 90)
    GO
    INSERT [dbo].[movie] ([media_id], [genre_id], [duration_minutes]) VALUES (16, 4, 104)
    GO
    INSERT [dbo].[movie] ([media_id], [genre_id], [duration_minutes]) VALUES (17, 1, 120)
    GO
    INSERT [dbo].[movie] ([media_id], [genre_id], [duration_minutes]) VALUES (18, 1, 121)
    GO
    INSERT [dbo].[movie] ([media_id], [genre_id], [duration_minutes]) VALUES (19, 3, 108)
    GO
    INSERT [dbo].[movie] ([media_id], [genre_id], [duration_minutes]) VALUES (20, 4, 127)
    GO
/****** Object:  Index [uq_episode_perseason]    Script Date: 08.01.2026 20:58:55 ******/
ALTER TABLE [dbo].[episode] ADD  CONSTRAINT [uq_episode_perseason] UNIQUE NONCLUSTERED
    (
    [season_id] ASC,
    [episode_number] ASC
    )WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
    GO
    SET ANSI_PADDING ON
    GO
/****** Object:  Index [UQ__genre__72E12F1B6BE53013]    Script Date: 08.01.2026 20:58:55 ******/
ALTER TABLE [dbo].[genre] ADD UNIQUE NONCLUSTERED
    (
    [name] ASC
    )WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
    GO
/****** Object:  Index [uq_review_peruser]    Script Date: 08.01.2026 20:58:55 ******/
ALTER TABLE [dbo].[review] ADD  CONSTRAINT [uq_review_peruser] UNIQUE NONCLUSTERED
    (
    [media_id] ASC,
    [user_id] ASC
    )WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
    GO
/****** Object:  Index [uq_season_perseries]    Script Date: 08.01.2026 20:58:55 ******/
ALTER TABLE [dbo].[season] ADD  CONSTRAINT [uq_season_perseries] UNIQUE NONCLUSTERED
    (
    [series_id] ASC,
    [number] ASC
    )WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
    GO
    SET ANSI_PADDING ON
    GO
/****** Object:  Index [UQ__user__F3DBC57263684BAD]    Script Date: 08.01.2026 20:58:55 ******/
ALTER TABLE [dbo].[user] ADD UNIQUE NONCLUSTERED
    (
    [username] ASC
    )WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
    GO
ALTER TABLE [dbo].[episode] ADD  DEFAULT (getdate()) FOR [created_at]
    GO
ALTER TABLE [dbo].[library_entry] ADD  DEFAULT ((0)) FOR [watched]
    GO
ALTER TABLE [dbo].[library_entry] ADD  DEFAULT (getdate()) FOR [created_at]
    GO
ALTER TABLE [dbo].[media] ADD  DEFAULT (getdate()) FOR [created_at]
    GO
ALTER TABLE [dbo].[media] ADD  DEFAULT ((0)) FOR [rating_count]
    GO
ALTER TABLE [dbo].[media] ADD  DEFAULT ((0)) FOR [rating_sum]
    GO
ALTER TABLE [dbo].[review] ADD  DEFAULT (getdate()) FOR [created_at]
    GO
ALTER TABLE [dbo].[user] ADD  DEFAULT (getdate()) FOR [created_at]
    GO
ALTER TABLE [dbo].[episode]  WITH CHECK ADD FOREIGN KEY([season_id])
    REFERENCES [dbo].[season] ([id])
    ON DELETE CASCADE
GO
ALTER TABLE [dbo].[library_entry]  WITH CHECK ADD FOREIGN KEY([media_id])
    REFERENCES [dbo].[media] ([id])
    GO
ALTER TABLE [dbo].[library_entry]  WITH CHECK ADD FOREIGN KEY([user_id])
    REFERENCES [dbo].[user] ([id])
    GO
ALTER TABLE [dbo].[movie]  WITH CHECK ADD FOREIGN KEY([genre_id])
    REFERENCES [dbo].[genre] ([id])
    GO
ALTER TABLE [dbo].[movie]  WITH CHECK ADD FOREIGN KEY([media_id])
    REFERENCES [dbo].[media] ([id])
    ON DELETE CASCADE
GO
ALTER TABLE [dbo].[review]  WITH CHECK ADD FOREIGN KEY([media_id])
    REFERENCES [dbo].[media] ([id])
    GO
ALTER TABLE [dbo].[review]  WITH CHECK ADD FOREIGN KEY([user_id])
    REFERENCES [dbo].[user] ([id])
    GO
ALTER TABLE [dbo].[season]  WITH CHECK ADD FOREIGN KEY([series_id])
    REFERENCES [dbo].[series] ([media_id])
    ON DELETE CASCADE
GO
ALTER TABLE [dbo].[series]  WITH CHECK ADD FOREIGN KEY([genre_id])
    REFERENCES [dbo].[genre] ([id])
    GO
ALTER TABLE [dbo].[series]  WITH CHECK ADD FOREIGN KEY([media_id])
    REFERENCES [dbo].[media] ([id])
    ON DELETE CASCADE
GO
ALTER TABLE [dbo].[episode]  WITH CHECK ADD CHECK  (([episode_number]>(0)))
    GO
ALTER TABLE [dbo].[media]  WITH CHECK ADD CHECK  (([rating_count]>=(0)))
    GO
ALTER TABLE [dbo].[media]  WITH CHECK ADD CHECK  (([rating_sum]>=(0)))
    GO
ALTER TABLE [dbo].[media]  WITH CHECK ADD CHECK  (([type]='series' OR [type]='movie'))
    GO
ALTER TABLE [dbo].[movie]  WITH CHECK ADD CHECK  (([duration_minutes]>(0)))
    GO
ALTER TABLE [dbo].[review]  WITH CHECK ADD CHECK  (([rating]>=(0) AND [rating]<=(5)))
    GO
ALTER TABLE [dbo].[season]  WITH CHECK ADD CHECK  (([number]>(0)))
    GO
ALTER TABLE [dbo].[user]  WITH CHECK ADD CHECK  (([role]='moderator' OR [role]='normal'))
    GO
    USE [master]
    GO
ALTER DATABASE [MoviesDB] SET  READ_WRITE 
GO
