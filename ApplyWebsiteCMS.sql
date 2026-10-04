-- =======================================================
-- SQL Script: Apply Website CMS Tables & Settings Columns
-- =======================================================

-- 1. ADD NEW COLUMNS TO Settings TABLE
IF OBJECT_ID(N'dbo.Settings', N'U') IS NOT NULL
BEGIN
    IF COL_LENGTH('dbo.Settings', 'JobCardNote') IS NULL ALTER TABLE dbo.Settings ADD [JobCardNote] nvarchar(max) NULL;
    IF COL_LENGTH('dbo.Settings', 'JobCardTitleBackgroundUrl') IS NULL ALTER TABLE dbo.Settings ADD [JobCardTitleBackgroundUrl] nvarchar(max) NULL;
    IF COL_LENGTH('dbo.Settings', 'JobCardTitleBackgroundKey') IS NULL ALTER TABLE dbo.Settings ADD [JobCardTitleBackgroundKey] nvarchar(max) NULL;
    IF COL_LENGTH('dbo.Settings', 'SignatureUrl') IS NULL ALTER TABLE dbo.Settings ADD [SignatureUrl] nvarchar(max) NULL;
    IF COL_LENGTH('dbo.Settings', 'SignatureKey') IS NULL ALTER TABLE dbo.Settings ADD [SignatureKey] nvarchar(max) NULL;
    IF COL_LENGTH('dbo.Settings', 'SignatureName') IS NULL ALTER TABLE dbo.Settings ADD [SignatureName] nvarchar(max) NULL;
    IF COL_LENGTH('dbo.Settings', 'SignatureTitle') IS NULL ALTER TABLE dbo.Settings ADD [SignatureTitle] nvarchar(max) NULL;
    IF COL_LENGTH('dbo.Settings', 'EmailOne') IS NULL ALTER TABLE dbo.Settings ADD [EmailOne] nvarchar(max) NULL;
    IF COL_LENGTH('dbo.Settings', 'PhoneOne') IS NULL ALTER TABLE dbo.Settings ADD [PhoneOne] nvarchar(max) NULL;
    IF COL_LENGTH('dbo.Settings', 'FacebookPage') IS NULL ALTER TABLE dbo.Settings ADD [FacebookPage] nvarchar(max) NULL;
    IF COL_LENGTH('dbo.Settings', 'TwitterPage') IS NULL ALTER TABLE dbo.Settings ADD [TwitterPage] nvarchar(max) NULL;
    IF COL_LENGTH('dbo.Settings', 'InstagramPage') IS NULL ALTER TABLE dbo.Settings ADD [InstagramPage] nvarchar(max) NULL;
    IF COL_LENGTH('dbo.Settings', 'LinkedInPage') IS NULL ALTER TABLE dbo.Settings ADD [LinkedInPage] nvarchar(max) NULL;
    IF COL_LENGTH('dbo.Settings', 'YoutubePage') IS NULL ALTER TABLE dbo.Settings ADD [YoutubePage] nvarchar(max) NULL;
    IF COL_LENGTH('dbo.Settings', 'AddressOne') IS NULL ALTER TABLE dbo.Settings ADD [AddressOne] nvarchar(max) NULL;
    IF COL_LENGTH('dbo.Settings', 'StateOne') IS NULL ALTER TABLE dbo.Settings ADD [StateOne] nvarchar(max) NULL;
    IF COL_LENGTH('dbo.Settings', 'CountryOne') IS NULL ALTER TABLE dbo.Settings ADD [CountryOne] nvarchar(max) NULL;
    IF COL_LENGTH('dbo.Settings', 'BankName') IS NULL ALTER TABLE dbo.Settings ADD [BankName] nvarchar(max) NULL;
    IF COL_LENGTH('dbo.Settings', 'AccountNumber') IS NULL ALTER TABLE dbo.Settings ADD [AccountNumber] nvarchar(max) NULL;
    IF COL_LENGTH('dbo.Settings', 'AccountName') IS NULL ALTER TABLE dbo.Settings ADD [AccountName] nvarchar(max) NULL;
    IF COL_LENGTH('dbo.Settings', 'BankNameTwo') IS NULL ALTER TABLE dbo.Settings ADD [BankNameTwo] nvarchar(max) NULL;
    IF COL_LENGTH('dbo.Settings', 'AccountNumberTwo') IS NULL ALTER TABLE dbo.Settings ADD [AccountNumberTwo] nvarchar(max) NULL;
    IF COL_LENGTH('dbo.Settings', 'AccountNameTwo') IS NULL ALTER TABLE dbo.Settings ADD [AccountNameTwo] nvarchar(max) NULL;
    IF COL_LENGTH('dbo.Settings', 'ReportCloseTime') IS NULL ALTER TABLE dbo.Settings ADD [ReportCloseTime] datetime2 NULL;
    IF COL_LENGTH('dbo.Settings', 'DefaultAttendanceStartTime') IS NULL ALTER TABLE dbo.Settings ADD [DefaultAttendanceStartTime] time NULL;
    IF COL_LENGTH('dbo.Settings', 'DefaultAttendanceStartTimeSaturday') IS NULL ALTER TABLE dbo.Settings ADD [DefaultAttendanceStartTimeSaturday] time NULL;
    IF COL_LENGTH('dbo.Settings', 'DefaultAttendanceCloseTime') IS NULL ALTER TABLE dbo.Settings ADD [DefaultAttendanceCloseTime] time NULL;
    IF COL_LENGTH('dbo.Settings', 'EarlyMinute') IS NULL ALTER TABLE dbo.Settings ADD [EarlyMinute] int NOT NULL CONSTRAINT DF_Settings_EarlyMinute DEFAULT 0;
    IF COL_LENGTH('dbo.Settings', 'OntimeMinute') IS NULL ALTER TABLE dbo.Settings ADD [OntimeMinute] int NOT NULL CONSTRAINT DF_Settings_OntimeMinute DEFAULT 0;
    IF COL_LENGTH('dbo.Settings', 'LateMinute') IS NULL ALTER TABLE dbo.Settings ADD [LateMinute] int NOT NULL CONSTRAINT DF_Settings_LateMinute DEFAULT 0;
    IF COL_LENGTH('dbo.Settings', 'VeryLateMinute') IS NULL ALTER TABLE dbo.Settings ADD [VeryLateMinute] int NOT NULL CONSTRAINT DF_Settings_VeryLateMinute DEFAULT 0;
    IF COL_LENGTH('dbo.Settings', 'EnableBreakingNewsRibon') IS NULL ALTER TABLE dbo.Settings ADD [EnableBreakingNewsRibon] bit NOT NULL CONSTRAINT DF_Settings_EnableBreakingNewsRibon DEFAULT 0;
    IF COL_LENGTH('dbo.Settings', 'BreakingNewsRibonTitle') IS NULL ALTER TABLE dbo.Settings ADD [BreakingNewsRibonTitle] nvarchar(max) NULL;
    IF COL_LENGTH('dbo.Settings', 'RibonPosition') IS NULL ALTER TABLE dbo.Settings ADD [RibonPosition] int NOT NULL CONSTRAINT DF_Settings_RibonPosition DEFAULT 0;
    IF COL_LENGTH('dbo.Settings', 'ShowTwoProducts') IS NULL ALTER TABLE dbo.Settings ADD [ShowTwoProducts] bit NOT NULL CONSTRAINT DF_Settings_ShowTwoProducts DEFAULT 0;
    IF COL_LENGTH('dbo.Settings', 'ShowThreeProducts') IS NULL ALTER TABLE dbo.Settings ADD [ShowThreeProducts] bit NOT NULL CONSTRAINT DF_Settings_ShowThreeProducts DEFAULT 0;
    IF COL_LENGTH('dbo.Settings', 'ShowSixProducts') IS NULL ALTER TABLE dbo.Settings ADD [ShowSixProducts] bit NOT NULL CONSTRAINT DF_Settings_ShowSixProducts DEFAULT 0;
    IF COL_LENGTH('dbo.Settings', 'ShowProductsInMenu') IS NULL ALTER TABLE dbo.Settings ADD [ShowProductsInMenu] bit NOT NULL CONSTRAINT DF_Settings_ShowProductsInMenu DEFAULT 0;
    IF COL_LENGTH('dbo.Settings', 'ShowProductsInFooter') IS NULL ALTER TABLE dbo.Settings ADD [ShowProductsInFooter] bit NOT NULL CONSTRAINT DF_Settings_ShowProductsInFooter DEFAULT 0;
    IF COL_LENGTH('dbo.Settings', 'ActivateProductsInHome') IS NULL ALTER TABLE dbo.Settings ADD [ActivateProductsInHome] bit NOT NULL CONSTRAINT DF_Settings_ActivateProductsInHome DEFAULT 0;
    IF COL_LENGTH('dbo.Settings', 'CustomMenuTop') IS NULL ALTER TABLE dbo.Settings ADD [CustomMenuTop] nvarchar(max) NULL;
    IF COL_LENGTH('dbo.Settings', 'UseCustomMenuTop') IS NULL ALTER TABLE dbo.Settings ADD [UseCustomMenuTop] bit NOT NULL CONSTRAINT DF_Settings_UseCustomMenuTop DEFAULT 0;
    IF COL_LENGTH('dbo.Settings', 'UseCategoryBlog') IS NULL ALTER TABLE dbo.Settings ADD [UseCategoryBlog] bit NOT NULL CONSTRAINT DF_Settings_UseCategoryBlog DEFAULT 0;
    IF COL_LENGTH('dbo.Settings', 'RemitaTitle') IS NULL ALTER TABLE dbo.Settings ADD [RemitaTitle] nvarchar(max) NULL;
    IF COL_LENGTH('dbo.Settings', 'RemitaInstruction') IS NULL ALTER TABLE dbo.Settings ADD [RemitaInstruction] nvarchar(max) NULL;
    IF COL_LENGTH('dbo.Settings', 'RemitaButton') IS NULL ALTER TABLE dbo.Settings ADD [RemitaButton] nvarchar(max) NULL;
END
GO

-- 2. CREATE CMS TABLES IF NOT EXISTS

IF OBJECT_ID(N'dbo.DataConfigs', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.DataConfigs (
        [Id] bigint IDENTITY(1,1) NOT NULL PRIMARY KEY,
        [CategoryKey] nvarchar(max) NULL,
        [Key] nvarchar(max) NULL,
        [Value] nvarchar(max) NULL,
        [Description] nvarchar(max) NULL
    );
END
GO

IF OBJECT_ID(N'dbo.TemplateTypes', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.TemplateTypes (
        [Id] bigint IDENTITY(1,1) NOT NULL PRIMARY KEY,
        [Name] nvarchar(max) NULL,
        [TemplateKey] nvarchar(max) NULL,
        [IsActive] bit NOT NULL
    );
END
GO

IF OBJECT_ID(N'dbo.TemplateCategories', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.TemplateCategories (
        [Id] bigint IDENTITY(1,1) NOT NULL PRIMARY KEY,
        [Name] nvarchar(max) NULL,
        [TemplateType] int NOT NULL,
        [ThemeUrl] nvarchar(max) NULL,
        [LayoutNumber] int NOT NULL
    );
END
GO

IF OBJECT_ID(N'dbo.SuperSettings', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.SuperSettings (
        [Id] bigint IDENTITY(1,1) NOT NULL PRIMARY KEY,
        [CompanyWebsiteTitle] nvarchar(max) NULL,
        [CompanyWebsiteDescription] nvarchar(max) NULL,
        [WebsiteKeywords] nvarchar(max) NULL,
        [CompanyDescription] nvarchar(max) NULL,
        [TemplateKey] nvarchar(max) NULL,
        [HomeSortFrom] int NOT NULL,
        [Layout] int NOT NULL,
        [ActiveCategoryWeb] bit NOT NULL,
        [WebTheme] int NOT NULL,
        [ActivateTestimony] bit NOT NULL,
        [ShowCategoryOnSlider] bit NOT NULL,
        [DefaultThemeNumber] int NOT NULL,
        [HeaderType] int NOT NULL,
        [SliderType] int NOT NULL,
        [AboutType] int NOT NULL,
        [ServiceType] int NOT NULL,
        [ProjectCategoryType] int NOT NULL,
        [ProjectListType] int NOT NULL,
        [BlogType] int NOT NULL,
        [ContactType] int NOT NULL,
        [FooterType] int NOT NULL,
        [HeaderColorCode] nvarchar(max) NULL,
        [FooterColorCode] nvarchar(max) NULL,
        [ClientCategoryType] int NOT NULL,
        [ClientListType] int NOT NULL,
        [FaqType] int NOT NULL,
        [TestimonyType] int NOT NULL,
        [TeamCategoryType] int NOT NULL,
        [TeamListType] int NOT NULL,
        [VideoType] int NOT NULL,
        [CTASectionType] int NOT NULL,
        [OfferType] int NOT NULL,
        [DashboardType] int NOT NULL,
        [DirectChooseTemplate] bit NOT NULL,
        [EnableContactUsPage] bit NOT NULL,
        [EnableClientPage] bit NOT NULL,
        [EnableTeamPage] bit NOT NULL,
        [EnableProjectPage] bit NOT NULL,
        [EnableBlogPage] bit NOT NULL,
        [EnableShopPage] bit NOT NULL,
        [EnableTestimonyPage] bit NOT NULL,
        [EnableCareerPage] bit NOT NULL,
        [EnableFAQPage] bit NOT NULL,
        [EnableAboutPage] bit NOT NULL,
        [EnableGalleryPage] bit NOT NULL,
        [EnableServicePage] bit NOT NULL,
        [MailSender] int NOT NULL,
        [SendgridApiKey] nvarchar(max) NULL,
        [SendgridSenderEmail] nvarchar(max) NULL,
        [SendgridSenderName] nvarchar(max) NULL,
        [SmtpHost] nvarchar(max) NULL,
        [SmtpPort] int NOT NULL,
        [SmtpUsername] nvarchar(max) NULL,
        [SmtpPassword] nvarchar(max) NULL,
        [SmtpSenderEmail] nvarchar(max) NULL,
        [SmtpSenderName] nvarchar(max) NULL,
        [AwsAccessKey] nvarchar(max) NULL,
        [AwsSecretKey] nvarchar(max) NULL,
        [AwsBucketName] nvarchar(max) NULL,
        [AwsRegion] nvarchar(max) NULL
    );
END
GO

IF OBJECT_ID(N'dbo.Sliders', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.Sliders (
        [Id] bigint IDENTITY(1,1) NOT NULL PRIMARY KEY,
        [Title] nvarchar(max) NULL,
        [MiniTitle] nvarchar(max) NULL,
        [SubTitle] nvarchar(max) NULL,
        [ButtonText] nvarchar(max) NULL,
        [ButtonLink] nvarchar(max) NULL,
        [Url] nvarchar(max) NULL,
        [Key] nvarchar(max) NULL,
        [SortOrder] int NOT NULL,
        [Show] bit NOT NULL,
        [ButtonTextTwo] nvarchar(max) NULL,
        [ButtonLinkTwo] nvarchar(max) NULL,
        [SliderType] int NOT NULL,
        [YoutubeUrl] nvarchar(max) NULL,
        [DisplayType] int NOT NULL,
        [SliderPosition] int NOT NULL
    );
END
GO

IF OBJECT_ID(N'dbo.Testimonies', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.Testimonies (
        [Id] bigint IDENTITY(1,1) NOT NULL PRIMARY KEY,
        [Title] nvarchar(max) NULL,
        [MiniTitle] nvarchar(max) NULL,
        [FullName] nvarchar(max) NULL,
        [TestimonyNotes] nvarchar(max) NULL,
        [Url] nvarchar(max) NULL,
        [Key] nvarchar(max) NULL,
        [Star] int NOT NULL,
        [Position] nvarchar(max) NULL,
        [TestimonyType] int NOT NULL,
        [ShowOnHomePage] bit NOT NULL
    );
END
GO

IF OBJECT_ID(N'dbo.FAQs', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.FAQs (
        [Id] bigint IDENTITY(1,1) NOT NULL PRIMARY KEY,
        [Title] nvarchar(max) NULL,
        [Answer] nvarchar(max) NULL,
        [Show] bit NOT NULL,
        [SortOrder] int NOT NULL
    );
END
GO

IF OBJECT_ID(N'dbo.PostModal', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.PostModal (
        [Id] bigint IDENTITY(1,1) NOT NULL PRIMARY KEY,
        [Title] nvarchar(max) NULL,
        [Message] nvarchar(max) NULL,
        [Show] bit NOT NULL,
        [ButtonText] nvarchar(max) NULL,
        [ButtonLink] nvarchar(max) NULL,
        [Url] nvarchar(max) NULL,
        [Key] nvarchar(max) NULL,
        [Date] datetime2 NOT NULL,
        [Closed] bit NOT NULL,
        [GeneralPost] bit NOT NULL,
        [DontShow] bit NOT NULL
    );
END
GO

IF OBJECT_ID(N'dbo.BlogCategories', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.BlogCategories (
        [Id] bigint IDENTITY(1,1) NOT NULL PRIMARY KEY,
        [Title] nvarchar(max) NULL,
        [Description] nvarchar(max) NULL,
        [ShowInHome] bit NOT NULL
    );
END
GO

IF OBJECT_ID(N'dbo.Blogs', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.Blogs (
        [Id] bigint IDENTITY(1,1) NOT NULL PRIMARY KEY,
        [Title] nvarchar(max) NULL,
        [MiniTitle] nvarchar(max) NULL,
        [Description] nvarchar(max) NULL,
        [FullDescription] nvarchar(max) NULL,
        [SortOrder] int NOT NULL,
        [ShowInHome] bit NOT NULL,
        [Publish] bit NOT NULL,
        [Date] datetime2 NOT NULL,
        [Author] nvarchar(max) NULL,
        [Url] nvarchar(max) NULL,
        [Key] nvarchar(max) NULL,
        [VideoUrl] nvarchar(max) NULL,
        [BlogCategoryId] bigint NOT NULL,
        [BlogSectionPosition] int NOT NULL,
        [ViewCount] int NOT NULL,
        [SortOrderCategory] int NOT NULL,
        [Quote] nvarchar(max) NULL,
        [QuoteAuthor] nvarchar(max) NULL,
        [QuoteAuthorJobTitle] nvarchar(max) NULL,
        [QuoteAuthorImageUrl] nvarchar(max) NULL,
        [QuoteAuthorImageKey] nvarchar(max) NULL,
        CONSTRAINT FK_Blogs_BlogCategories_BlogCategoryId FOREIGN KEY ([BlogCategoryId]) REFERENCES dbo.BlogCategories([Id]) ON DELETE CASCADE
    );
END
GO

IF OBJECT_ID(N'dbo.PageCategories', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.PageCategories (
        [Id] bigint IDENTITY(1,1) NOT NULL PRIMARY KEY,
        [Title] nvarchar(max) NULL,
        [Description] nvarchar(max) NULL,
        [ShowInHome] bit NOT NULL,
        [ShowInMenu] bit NOT NULL,
        [ParentId] bigint NULL,
        [DirectUrl] nvarchar(max) NULL,
        [Url] nvarchar(max) NULL,
        [Key] nvarchar(max) NULL,
        [IconUrl] nvarchar(max) NULL,
        [IconKey] nvarchar(max) NULL,
        [SortOrder] int NOT NULL
    );
END
GO

IF OBJECT_ID(N'dbo.WebPages', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.WebPages (
        [Id] bigint IDENTITY(1,1) NOT NULL PRIMARY KEY,
        [Title] nvarchar(max) NULL,
        [MiniTitle] nvarchar(max) NULL,
        [Description] nvarchar(max) NULL,
        [FullDescription] nvarchar(max) NULL,
        [SortOrder] int NOT NULL,
        [ShowInHome] bit NOT NULL,
        [ShowInMenu] bit NOT NULL,
        [Publish] bit NOT NULL,
        [Date] datetime2 NOT NULL,
        [Author] nvarchar(max) NULL,
        [Url] nvarchar(max) NULL,
        [Key] nvarchar(max) NULL,
        [PageCategoryId] bigint NULL,
        [DirectUrl] nvarchar(max) NULL,
        [VideoUrl] nvarchar(max) NULL,
        [EnableSection] bit NOT NULL,
        [ViewCount] int NOT NULL,
        [ShowInFooter] bit NOT NULL,
        [Quote] nvarchar(max) NULL,
        [QuoteAuthor] nvarchar(max) NULL,
        [QuoteAuthorJobTitle] nvarchar(max) NULL,
        [QuoteAuthorImageUrl] nvarchar(max) NULL,
        [QuoteAuthorImageKey] nvarchar(max) NULL,
        CONSTRAINT FK_WebPages_PageCategories_PageCategoryId FOREIGN KEY ([PageCategoryId]) REFERENCES dbo.PageCategories([Id]) ON DELETE NO ACTION
    );
END
GO

IF OBJECT_ID(N'dbo.PageSections', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.PageSections (
        [Id] bigint IDENTITY(1,1) NOT NULL PRIMARY KEY,
        [Title] nvarchar(max) NULL,
        [MiniTitle] nvarchar(max) NULL,
        [Description] nvarchar(max) NULL,
        [FullDescription] nvarchar(max) NULL,
        [Url] nvarchar(max) NULL,
        [Key] nvarchar(max) NULL,
        [VideoUrl] nvarchar(max) NULL,
        [SortOrder] int NOT NULL,
        [ShowInHome] bit NOT NULL,
        [WebPageId] bigint NULL,
        [FixedSectionStyle] int NOT NULL,
        [StyleType] int NOT NULL,
        [ShowInPage] bit NOT NULL,
        [ButtonText] nvarchar(max) NULL,
        [ButtonLink] nvarchar(max) NULL,
        [ButtonTextTwo] nvarchar(max) NULL,
        [ButtonLinkTwo] nvarchar(max) NULL,
        [Quote] nvarchar(max) NULL,
        [QuoteAuthor] nvarchar(max) NULL,
        [QuoteAuthorJobTitle] nvarchar(max) NULL,
        [QuoteAuthorImageUrl] nvarchar(max) NULL,
        [QuoteAuthorImageKey] nvarchar(max) NULL,
        CONSTRAINT FK_PageSections_WebPages_WebPageId FOREIGN KEY ([WebPageId]) REFERENCES dbo.WebPages([Id]) ON DELETE CASCADE
    );
END
GO

IF OBJECT_ID(N'dbo.PageSectionLists', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.PageSectionLists (
        [Id] bigint IDENTITY(1,1) NOT NULL PRIMARY KEY,
        [Title] nvarchar(max) NULL,
        [MiniTitle] nvarchar(max) NULL,
        [Description] nvarchar(max) NULL,
        [FullDescription] nvarchar(max) NULL,
        [Url] nvarchar(max) NULL,
        [Key] nvarchar(max) NULL,
        [VideoUrl] nvarchar(max) NULL,
        [SortOrder] int NOT NULL,
        [PageSectionId] bigint NOT NULL,
        [ButtonText] nvarchar(max) NULL,
        [ButtonLink] nvarchar(max) NULL,
        [ButtonTextTwo] nvarchar(max) NULL,
        [ButtonLinkTwo] nvarchar(max) NULL,
        [IconText] nvarchar(max) NULL,
        [Quote] nvarchar(max) NULL,
        [QuoteAuthor] nvarchar(max) NULL,
        [QuoteAuthorJobTitle] nvarchar(max) NULL,
        [QuoteAuthorImageUrl] nvarchar(max) NULL,
        [QuoteAuthorImageKey] nvarchar(max) NULL,
        CONSTRAINT FK_PageSectionLists_PageSections_PageSectionId FOREIGN KEY ([PageSectionId]) REFERENCES dbo.PageSections([Id]) ON DELETE CASCADE
    );
END
GO

PRINT 'Website CMS Tables and Columns created successfully!';
