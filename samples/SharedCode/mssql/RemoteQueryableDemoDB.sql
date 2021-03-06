USE [master]
GO

PRINT '## DROP DATABASE #######################################################'
GO
IF DB_ID (N'RemoteQueryableDemoDB_MAY2020') IS NOT NULL
BEGIN
    PRINT '   DROP DATABASE [RemoteQueryableDemoDB_MAY2020]'
    EXEC msdb.dbo.sp_delete_database_backuphistory @database_name = N'RemoteQueryableDemoDB_MAY2020'
    ALTER DATABASE [RemoteQueryableDemoDB_MAY2020] SET SINGLE_USER WITH ROLLBACK IMMEDIATE
    DROP DATABASE [RemoteQueryableDemoDB_MAY2020];
END

PRINT '## CREATE DATABASE #######################################################'
GO
DECLARE @sql NVARCHAR(1024), @path VARCHAR(256)

SELECT @path = PHYSICAL_NAME FROM sys.master_files WHERE database_id = DB_ID(N'master') AND TYPE_DESC = 'ROWS'
SET @path = REVERSE(RIGHT(REVERSE(@path),(LEN(@path)-CHARINDEX('\\', REVERSE(@path),1))+1))

PRINT '   CREATE DATABASE [RemoteQueryableDemoDB_MAY2020]'
PRINT '   '+@path+'RemoteQueryableDemoDB_MAY2020.mdf'
PRINT '   '+@path+'RemoteQueryableDemoDB_MAY2020_log.ldf'

SET @sql = 
N'CREATE DATABASE [RemoteQueryableDemoDB_MAY2020] 
  CONTAINMENT = NONE 
  ON  PRIMARY 
  ( NAME = N''RemoteQueryableDemoDB_MAY2020'', FILENAME = N'''+@path+N'RemoteQueryableDemoDB_MAY2020.mdf'' , SIZE = 5MB , MAXSIZE = UNLIMITED, FILEGROWTH = 1024KB ) 
  LOG ON 
  ( NAME = N''RemoteQueryableDemoDB_MAY2020_Log'', FILENAME = N'''+@path+N'RemoteQueryableDemoDB_MAY2020_log.ldf'' , SIZE = 1024KB , MAXSIZE = 2048GB , FILEGROWTH = 10%)'
EXEC sp_executesql @sql

ALTER DATABASE [RemoteQueryableDemoDB_MAY2020] SET COMPATIBILITY_LEVEL = 110
GO
IF (1 = FULLTEXTSERVICEPROPERTY('IsFullTextInstalled'))
begin
EXEC [RemoteQueryableDemoDB_MAY2020].[dbo].[sp_fulltext_database] @action = 'enable'
end
GO
ALTER DATABASE [RemoteQueryableDemoDB_MAY2020] SET ANSI_NULL_DEFAULT OFF 
GO
ALTER DATABASE [RemoteQueryableDemoDB_MAY2020] SET ANSI_NULLS OFF 
GO
ALTER DATABASE [RemoteQueryableDemoDB_MAY2020] SET ANSI_PADDING OFF 
GO
ALTER DATABASE [RemoteQueryableDemoDB_MAY2020] SET ANSI_WARNINGS OFF 
GO
ALTER DATABASE [RemoteQueryableDemoDB_MAY2020] SET ARITHABORT OFF 
GO
ALTER DATABASE [RemoteQueryableDemoDB_MAY2020] SET AUTO_CLOSE ON 
GO
ALTER DATABASE [RemoteQueryableDemoDB_MAY2020] SET AUTO_CREATE_STATISTICS ON 
GO
ALTER DATABASE [RemoteQueryableDemoDB_MAY2020] SET AUTO_SHRINK OFF 
GO
ALTER DATABASE [RemoteQueryableDemoDB_MAY2020] SET AUTO_UPDATE_STATISTICS ON 
GO
ALTER DATABASE [RemoteQueryableDemoDB_MAY2020] SET CURSOR_CLOSE_ON_COMMIT OFF 
GO
ALTER DATABASE [RemoteQueryableDemoDB_MAY2020] SET CURSOR_DEFAULT  GLOBAL 
GO
ALTER DATABASE [RemoteQueryableDemoDB_MAY2020] SET CONCAT_NULL_YIELDS_NULL OFF 
GO
ALTER DATABASE [RemoteQueryableDemoDB_MAY2020] SET NUMERIC_ROUNDABORT OFF 
GO
ALTER DATABASE [RemoteQueryableDemoDB_MAY2020] SET QUOTED_IDENTIFIER OFF 
GO
ALTER DATABASE [RemoteQueryableDemoDB_MAY2020] SET RECURSIVE_TRIGGERS OFF 
GO
ALTER DATABASE [RemoteQueryableDemoDB_MAY2020] SET  DISABLE_BROKER 
GO
ALTER DATABASE [RemoteQueryableDemoDB_MAY2020] SET AUTO_UPDATE_STATISTICS_ASYNC OFF 
GO
ALTER DATABASE [RemoteQueryableDemoDB_MAY2020] SET DATE_CORRELATION_OPTIMIZATION OFF 
GO
ALTER DATABASE [RemoteQueryableDemoDB_MAY2020] SET TRUSTWORTHY OFF 
GO
ALTER DATABASE [RemoteQueryableDemoDB_MAY2020] SET ALLOW_SNAPSHOT_ISOLATION OFF 
GO
ALTER DATABASE [RemoteQueryableDemoDB_MAY2020] SET PARAMETERIZATION SIMPLE 
GO
ALTER DATABASE [RemoteQueryableDemoDB_MAY2020] SET READ_COMMITTED_SNAPSHOT OFF 
GO
ALTER DATABASE [RemoteQueryableDemoDB_MAY2020] SET HONOR_BROKER_PRIORITY OFF 
GO
ALTER DATABASE [RemoteQueryableDemoDB_MAY2020] SET RECOVERY SIMPLE 
GO
ALTER DATABASE [RemoteQueryableDemoDB_MAY2020] SET  MULTI_USER 
GO
ALTER DATABASE [RemoteQueryableDemoDB_MAY2020] SET PAGE_VERIFY CHECKSUM  
GO
ALTER DATABASE [RemoteQueryableDemoDB_MAY2020] SET DB_CHAINING OFF 
GO
ALTER DATABASE [RemoteQueryableDemoDB_MAY2020] SET FILESTREAM( NON_TRANSACTED_ACCESS = OFF ) 
GO
ALTER DATABASE [RemoteQueryableDemoDB_MAY2020] SET TARGET_RECOVERY_TIME = 0 SECONDS 
GO
USE [RemoteQueryableDemoDB_MAY2020]
GO


PRINT '## CREATE USER #######################################################'
IF NOT EXISTS (SELECT * FROM master.sys.server_principals WHERE name = 'Demo')
  CREATE LOGIN [Demo] WITH PASSWORD = 'demo(!)Password', DEFAULT_DATABASE=[RemoteQueryableDemoDB_MAY2020];
CREATE USER [Demo] FOR LOGIN [Demo];
ALTER ROLE [db_owner] ADD MEMBER [Demo];
GO


--PRINT '## CREATE SCHEMA #######################################################'
--GO
--PRINT '   CREATE SCHEMA [dbo]'
--GO
--CREATE SCHEMA [dbo]
--GO


PRINT '## CREATE TABLES #######################################################'
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO

PRINT '   CREATE TABLE [dbo].[ProductCategories]'
GO
CREATE TABLE [dbo].[ProductCategories](
    [Id] [int] NOT NULL PRIMARY KEY,
    [Name] [varchar](50) NOT NULL
) ON [PRIMARY]
GO

PRINT '   CREATE TABLE [dbo].[ProductGroups]'
GO
CREATE TABLE [dbo].[ProductGroups](
    [Id] [int] NOT NULL PRIMARY KEY,
    [Name] [varchar](50) NOT NULL
) ON [PRIMARY]
GO

PRINT '   CREATE TABLE [dbo].[Products]'
GO
CREATE TABLE [dbo].[Products](
    [Id] [int] NOT NULL PRIMARY KEY,
    [ProductCategoryId] [int] NOT NULL FOREIGN KEY REFERENCES [dbo].[ProductCategories]([Id]),
    [Name] [varchar](50) NOT NULL,
    [Price] [money] NOT NULL
) ON [PRIMARY]
GO

PRINT '   CREATE TABLE [dbo].[OrderItems]'
GO
CREATE TABLE [dbo].[OrderItems](
    [Id] [int] NOT NULL PRIMARY KEY,
    [ProductId] [int] NOT NULL FOREIGN KEY REFERENCES [dbo].[Products]([Id]),
    [Quantity] [int] NOT NULL,
    [UnitPrice] [money] NOT NULL
) ON [PRIMARY]
GO

PRINT '   CREATE TABLE [dbo].[Markets]'
GO
CREATE TABLE [dbo].[Markets](
    [Id] [int] NOT NULL PRIMARY KEY,
    [Name] [varchar](50) NOT NULL
)
GO

PRINT '   CREATE TABLE [dbo].[Products_ProductGroups]'
GO
CREATE TABLE [dbo].[Products_ProductGroups](
    [ProductId] [int] NOT NULL FOREIGN KEY REFERENCES [dbo].[Products]([Id]),
    [ProductGroupId] [int] NOT NULL FOREIGN KEY REFERENCES [dbo].[ProductGroups]([Id]),
    PRIMARY KEY([ProductId],[ProductGroupId])
)
GO

PRINT '   CREATE TABLE [dbo].[Products_Markets]'
GO
CREATE TABLE [dbo].[Products_Markets](
    [ProductId] [int] NOT NULL FOREIGN KEY REFERENCES [dbo].[Products]([Id]),
    [MarketId] [int] NOT NULL FOREIGN KEY REFERENCES [dbo].[Markets]([Id]),
    PRIMARY KEY([ProductId],[MarketId])
)
GO
 
 
PRINT '## CREATE DATA #######################################################'
GO
USE [master]
GO
ALTER DATABASE [RemoteQueryableDemoDB_MAY2020] SET READ_WRITE 
GO
USE [RemoteQueryableDemoDB_MAY2020]
GO
SET NOCOUNT ON 
GO

PRINT '   INSERT PRODUCT CATEGORIES'
GO
INSERT INTO [dbo].[ProductCategories]([Id],[Name])
          SELECT 1, 'Fruits'
UNION ALL SELECT 2, 'Vehicles'
GO

PRINT '   INSERT PRODUCT GROUPS'
GO
INSERT INTO [dbo].[ProductGroups]([Id],[Name])
          SELECT 1, 'All'
UNION ALL SELECT 2, 'Food'
UNION ALL SELECT 3, 'NonFood'
GO

PRINT '   INSERT PRODUCTS'
GO
INSERT INTO [dbo].[Products]([Id],[ProductCategoryId],[Name],[Price])
          SELECT 101, 1, 'Apple', 1
UNION ALL SELECT 102, 1, 'Pear', 2
UNION ALL SELECT 103, 1, 'Pineapple', 3
UNION ALL SELECT 104, 2, 'Car', 33999
UNION ALL SELECT 105, 2, 'Bicycle', 150
GO

PRINT '   INSERT ORDER ITEMS'
INSERT INTO [dbo].[OrderItems]([Id],[ProductId],[Quantity],[UnitPrice])
          SELECT 10001, [ID], 2, [Price] FROM [dbo].[Products] WHERE [Id] = 101
UNION ALL SELECT 10002, [ID], 3, [Price] FROM [dbo].[Products] WHERE [Id] = 102
UNION ALL SELECT 10003, [ID], 3, [Price] FROM [dbo].[Products] WHERE [Id] = 105
GO

PRINT '   INSERT MARKETS ITEMS'
INSERT INTO [dbo].[Markets]([Id],[Name])
          SELECT 11, 'Product destination market 1'
UNION ALL SELECT 12, 'Product destination market 2'
UNION ALL SELECT 13, 'Product destination market 3'
UNION ALL SELECT 14, 'Product destination market 4'
GO

PRINT '   INSERT PRODUCTS_TO_MARKETS ITEMS'
INSERT INTO [dbo].[Products_Markets]([ProductId],[MarketId])
          SELECT 101, 11
UNION ALL SELECT 102, 11
UNION ALL SELECT 103, 11
UNION ALL SELECT 101, 13
UNION ALL SELECT 102, 13
UNION ALL SELECT 101, 14
GO

PRINT '   INSERT PRODUCTS_TO_PRODUCTGROUPS ITEMS'
INSERT INTO [dbo].[Products_ProductGroups]([ProductId],[ProductGroupId])
          SELECT 101, 1
UNION ALL SELECT 102, 1
UNION ALL SELECT 103, 1
UNION ALL SELECT 104, 1
UNION ALL SELECT 105, 1
UNION ALL SELECT 101, 2
UNION ALL SELECT 102, 2
UNION ALL SELECT 103, 2
UNION ALL SELECT 104, 3
UNION ALL SELECT 105, 3
GO
