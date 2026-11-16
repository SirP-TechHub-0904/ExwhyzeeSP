using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ExwhyzeeSP.Data.Migrations
{
    /// <inheritdoc />
    public partial class ApplyWebsiteCMS : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
IF OBJECT_ID(N'dbo.SuperSettings', N'U') IS NOT NULL
BEGIN
    IF COL_LENGTH('dbo.SuperSettings', 'BucketName') IS NULL ALTER TABLE dbo.SuperSettings ADD [BucketName] nvarchar(max) NULL;
    IF COL_LENGTH('dbo.SuperSettings', 'UserNipssArea') IS NULL ALTER TABLE dbo.SuperSettings ADD [UserNipssArea] bit NOT NULL CONSTRAINT DF_SuperSettings_UserNipssArea DEFAULT (0);
    IF COL_LENGTH('dbo.SuperSettings', 'ActivateLoginInMenu') IS NULL ALTER TABLE dbo.SuperSettings ADD [ActivateLoginInMenu] bit NOT NULL CONSTRAINT DF_SuperSettings_ActivateLoginInMenu DEFAULT (0);
    IF COL_LENGTH('dbo.SuperSettings', 'CompanyNameAbbreviation') IS NULL ALTER TABLE dbo.SuperSettings ADD [CompanyNameAbbreviation] nvarchar(max) NULL;
    IF COL_LENGTH('dbo.SuperSettings', 'UseNewMISDashboard') IS NULL ALTER TABLE dbo.SuperSettings ADD [UseNewMISDashboard] bit NOT NULL CONSTRAINT DF_SuperSettings_UseNewMISDashboard DEFAULT (0);
    IF COL_LENGTH('dbo.SuperSettings', 'ReloaderIconUrl') IS NULL ALTER TABLE dbo.SuperSettings ADD [ReloaderIconUrl] nvarchar(max) NULL;
    IF COL_LENGTH('dbo.SuperSettings', 'ReloaderIconKey') IS NULL ALTER TABLE dbo.SuperSettings ADD [ReloaderIconKey] nvarchar(max) NULL;
    IF COL_LENGTH('dbo.SuperSettings', 'ProductTitle') IS NULL ALTER TABLE dbo.SuperSettings ADD [ProductTitle] nvarchar(max) NULL;
    IF COL_LENGTH('dbo.SuperSettings', 'ProductTitleHome') IS NULL ALTER TABLE dbo.SuperSettings ADD [ProductTitleHome] nvarchar(max) NULL;
    IF COL_LENGTH('dbo.SuperSettings', 'ShowMadeByXYZ') IS NULL ALTER TABLE dbo.SuperSettings ADD [ShowMadeByXYZ] bit NOT NULL CONSTRAINT DF_SuperSettings_ShowMadeByXYZ DEFAULT (0);
    IF COL_LENGTH('dbo.SuperSettings', 'SpecialMenuButtonText') IS NULL ALTER TABLE dbo.SuperSettings ADD [SpecialMenuButtonText] nvarchar(max) NULL;
    IF COL_LENGTH('dbo.SuperSettings', 'SpecialMenuButtonLink') IS NULL ALTER TABLE dbo.SuperSettings ADD [SpecialMenuButtonLink] nvarchar(max) NULL;
    IF COL_LENGTH('dbo.SuperSettings', 'SpecialMenuButtonActivate') IS NULL ALTER TABLE dbo.SuperSettings ADD [SpecialMenuButtonActivate] bit NOT NULL CONSTRAINT DF_SuperSettings_SpecialMenuButtonActivate DEFAULT (0);
    IF COL_LENGTH('dbo.SuperSettings', 'ActivateWorkWithUsFooter') IS NULL ALTER TABLE dbo.SuperSettings ADD [ActivateWorkWithUsFooter] bit NOT NULL CONSTRAINT DF_SuperSettings_ActivateWorkWithUsFooter DEFAULT (0);
    IF COL_LENGTH('dbo.SuperSettings', 'TitleWorkWithUsFooter') IS NULL ALTER TABLE dbo.SuperSettings ADD [TitleWorkWithUsFooter] nvarchar(max) NULL;
    IF COL_LENGTH('dbo.SuperSettings', 'ActivateDownloadOurAppFooter') IS NULL ALTER TABLE dbo.SuperSettings ADD [ActivateDownloadOurAppFooter] bit NOT NULL CONSTRAINT DF_SuperSettings_ActivateDownloadOurAppFooter DEFAULT (0);
    IF COL_LENGTH('dbo.SuperSettings', 'ActivateSubscribeFormOnFooter') IS NULL ALTER TABLE dbo.SuperSettings ADD [ActivateSubscribeFormOnFooter] bit NOT NULL CONSTRAINT DF_SuperSettings_ActivateSubscribeFormOnFooter DEFAULT (0);
    IF COL_LENGTH('dbo.SuperSettings', 'TitleDownloadOurAppFooter') IS NULL ALTER TABLE dbo.SuperSettings ADD [TitleDownloadOurAppFooter] nvarchar(max) NULL;
    IF COL_LENGTH('dbo.SuperSettings', 'DescriptionDownloadOurAppFooter') IS NULL ALTER TABLE dbo.SuperSettings ADD [DescriptionDownloadOurAppFooter] nvarchar(max) NULL;
    IF COL_LENGTH('dbo.SuperSettings', 'PlayStoreImageUrl') IS NULL ALTER TABLE dbo.SuperSettings ADD [PlayStoreImageUrl] nvarchar(max) NULL;
    IF COL_LENGTH('dbo.SuperSettings', 'PlayStoreImageKey') IS NULL ALTER TABLE dbo.SuperSettings ADD [PlayStoreImageKey] nvarchar(max) NULL;
    IF COL_LENGTH('dbo.SuperSettings', 'PlayStoreLink') IS NULL ALTER TABLE dbo.SuperSettings ADD [PlayStoreLink] nvarchar(max) NULL;
    IF COL_LENGTH('dbo.SuperSettings', 'AppStoreImageUrl') IS NULL ALTER TABLE dbo.SuperSettings ADD [AppStoreImageUrl] nvarchar(max) NULL;
    IF COL_LENGTH('dbo.SuperSettings', 'AppStoreImageKey') IS NULL ALTER TABLE dbo.SuperSettings ADD [AppStoreImageKey] nvarchar(max) NULL;
    IF COL_LENGTH('dbo.SuperSettings', 'AppStoreLink') IS NULL ALTER TABLE dbo.SuperSettings ADD [AppStoreLink] nvarchar(max) NULL;
    IF COL_LENGTH('dbo.SuperSettings', 'EncryptionNote') IS NULL ALTER TABLE dbo.SuperSettings ADD [EncryptionNote] nvarchar(max) NULL;
    IF COL_LENGTH('dbo.SuperSettings', 'EncryptionUrl') IS NULL ALTER TABLE dbo.SuperSettings ADD [EncryptionUrl] nvarchar(max) NULL;
    IF COL_LENGTH('dbo.SuperSettings', 'EncryptionKey') IS NULL ALTER TABLE dbo.SuperSettings ADD [EncryptionKey] nvarchar(max) NULL;
    IF COL_LENGTH('dbo.SuperSettings', 'PaystackPublicKey') IS NULL ALTER TABLE dbo.SuperSettings ADD [PaystackPublicKey] nvarchar(max) NULL;
    IF COL_LENGTH('dbo.SuperSettings', 'PaystackSecretKey') IS NULL ALTER TABLE dbo.SuperSettings ADD [PaystackSecretKey] nvarchar(max) NULL;
    IF COL_LENGTH('dbo.SuperSettings', 'PaystackCallbackUrl') IS NULL ALTER TABLE dbo.SuperSettings ADD [PaystackCallbackUrl] nvarchar(max) NULL;
    IF COL_LENGTH('dbo.SuperSettings', 'PaymentModuleActive') IS NULL ALTER TABLE dbo.SuperSettings ADD [PaymentModuleActive] bit NOT NULL CONSTRAINT DF_SuperSettings_PaymentModuleActive DEFAULT (0);
    IF COL_LENGTH('dbo.SuperSettings', 'CurrencySymbol') IS NULL ALTER TABLE dbo.SuperSettings ADD [CurrencySymbol] nvarchar(max) NULL;
    IF COL_LENGTH('dbo.SuperSettings', 'CurrencyCode') IS NULL ALTER TABLE dbo.SuperSettings ADD [CurrencyCode] nvarchar(max) NULL;
    IF COL_LENGTH('dbo.SuperSettings', 'OrganizationDisplayName') IS NULL ALTER TABLE dbo.SuperSettings ADD [OrganizationDisplayName] nvarchar(max) NULL;
    IF COL_LENGTH('dbo.SuperSettings', 'OrganizationPaymentNote') IS NULL ALTER TABLE dbo.SuperSettings ADD [OrganizationPaymentNote] nvarchar(max) NULL;
    IF COL_LENGTH('dbo.SuperSettings', 'ActivateDashboard') IS NOT NULL
       AND NOT EXISTS (SELECT 1 FROM sys.default_constraints dc INNER JOIN sys.columns c ON dc.parent_object_id = c.object_id AND dc.parent_column_id = c.column_id WHERE c.object_id = OBJECT_ID(N'dbo.SuperSettings') AND c.name = N'ActivateDashboard')
        ALTER TABLE dbo.SuperSettings ADD CONSTRAINT DF_SuperSettings_ActivateDashboard DEFAULT (0) FOR [ActivateDashboard];
END
IF OBJECT_ID(N'dbo.Sliders', N'U') IS NOT NULL
BEGIN
    IF COL_LENGTH('dbo.Sliders', 'DisableVideoButton') IS NULL ALTER TABLE dbo.Sliders ADD [DisableVideoButton] bit NOT NULL CONSTRAINT DF_Sliders_DisableVideoButton DEFAULT (0);
    IF COL_LENGTH('dbo.Sliders', 'VideoLink') IS NULL ALTER TABLE dbo.Sliders ADD [VideoLink] nvarchar(max) NULL;
    IF COL_LENGTH('dbo.Sliders', 'VideoButtonText') IS NULL ALTER TABLE dbo.Sliders ADD [VideoButtonText] nvarchar(max) NULL;
END
IF OBJECT_ID(N'dbo.WebPages', N'U') IS NOT NULL
BEGIN
    IF COL_LENGTH('dbo.WebPages', 'MainPageId') IS NULL ALTER TABLE dbo.WebPages ADD [MainPageId] bigint NULL;
END
IF OBJECT_ID(N'dbo.PageSections', N'U') IS NOT NULL
BEGIN
    IF COL_LENGTH('dbo.PageSections', 'BlogSectionPosition') IS NULL ALTER TABLE dbo.PageSections ADD [BlogSectionPosition] int NOT NULL CONSTRAINT DF_PageSections_BlogSectionPosition DEFAULT (0);
    IF COL_LENGTH('dbo.PageSections', 'ShowSectionInBlog') IS NULL ALTER TABLE dbo.PageSections ADD [ShowSectionInBlog] bit NOT NULL CONSTRAINT DF_PageSections_ShowSectionInBlog DEFAULT (0);
    IF COL_LENGTH('dbo.PageSections', 'AddToRibon') IS NULL ALTER TABLE dbo.PageSections ADD [AddToRibon] bit NOT NULL CONSTRAINT DF_PageSections_AddToRibon DEFAULT (0);
    IF COL_LENGTH('dbo.PageSections', 'RibonCustomDisplayTitle') IS NULL ALTER TABLE dbo.PageSections ADD [RibonCustomDisplayTitle] nvarchar(max) NULL;
    IF COL_LENGTH('dbo.PageSections', 'RibonSortOrder') IS NULL ALTER TABLE dbo.PageSections ADD [RibonSortOrder] int NOT NULL CONSTRAINT DF_PageSections_RibonSortOrder DEFAULT (0);
END
IF OBJECT_ID(N'dbo.PageSectionLists', N'U') IS NOT NULL
BEGIN
    IF COL_LENGTH('dbo.PageSectionLists', 'AddToRibon') IS NULL ALTER TABLE dbo.PageSectionLists ADD [AddToRibon] bit NOT NULL CONSTRAINT DF_PageSectionLists_AddToRibon DEFAULT (0);
    IF COL_LENGTH('dbo.PageSectionLists', 'RibonCustomDisplayTitle') IS NULL ALTER TABLE dbo.PageSectionLists ADD [RibonCustomDisplayTitle] nvarchar(max) NULL;
    IF COL_LENGTH('dbo.PageSectionLists', 'RibonSortOrder') IS NULL ALTER TABLE dbo.PageSectionLists ADD [RibonSortOrder] int NOT NULL CONSTRAINT DF_PageSectionLists_RibonSortOrder DEFAULT (0);
END
IF OBJECT_ID(N'dbo.Blogs', N'U') IS NOT NULL
BEGIN
    IF COL_LENGTH('dbo.Blogs', 'AddToRibon') IS NULL ALTER TABLE dbo.Blogs ADD [AddToRibon] bit NOT NULL CONSTRAINT DF_Blogs_AddToRibon DEFAULT (0);
    IF COL_LENGTH('dbo.Blogs', 'RibonCustomDisplayTitle') IS NULL ALTER TABLE dbo.Blogs ADD [RibonCustomDisplayTitle] nvarchar(max) NULL;
    IF COL_LENGTH('dbo.Blogs', 'RibonSortOrder') IS NULL ALTER TABLE dbo.Blogs ADD [RibonSortOrder] int NOT NULL CONSTRAINT DF_Blogs_RibonSortOrder DEFAULT (0);
END
IF OBJECT_ID(N'dbo.PostModal', N'U') IS NOT NULL
BEGIN
    IF COL_LENGTH('dbo.PostModal', 'ImageUrl') IS NULL ALTER TABLE dbo.PostModal ADD [ImageUrl] nvarchar(max) NULL;
    IF COL_LENGTH('dbo.PostModal', 'ImageKey') IS NULL ALTER TABLE dbo.PostModal ADD [ImageKey] nvarchar(max) NULL;
    IF COL_LENGTH('dbo.PostModal', 'Description') IS NULL ALTER TABLE dbo.PostModal ADD [Description] nvarchar(max) NULL;
    IF COL_LENGTH('dbo.PostModal', 'ModalTime') IS NULL ALTER TABLE dbo.PostModal ADD [ModalTime] int NOT NULL CONSTRAINT DF_PostModal_ModalTime DEFAULT (0);
    IF COL_LENGTH('dbo.PostModal', 'ModalOccurance') IS NULL ALTER TABLE dbo.PostModal ADD [ModalOccurance] int NOT NULL CONSTRAINT DF_PostModal_ModalOccurance DEFAULT (0);
    IF COL_LENGTH('dbo.PostModal', 'StartTime') IS NULL ALTER TABLE dbo.PostModal ADD [StartTime] datetime2 NULL;
    IF COL_LENGTH('dbo.PostModal', 'StopTime') IS NULL ALTER TABLE dbo.PostModal ADD [StopTime] datetime2 NULL;
    IF COL_LENGTH('dbo.PostModal', 'ShowOnlyImage') IS NULL ALTER TABLE dbo.PostModal ADD [ShowOnlyImage] bit NOT NULL CONSTRAINT DF_PostModal_ShowOnlyImage DEFAULT (0);
    IF COL_LENGTH('dbo.PostModal', 'ShowTitle') IS NULL ALTER TABLE dbo.PostModal ADD [ShowTitle] bit NOT NULL CONSTRAINT DF_PostModal_ShowTitle DEFAULT (0);
    IF COL_LENGTH('dbo.PostModal', 'ShowDescription') IS NULL ALTER TABLE dbo.PostModal ADD [ShowDescription] bit NOT NULL CONSTRAINT DF_PostModal_ShowDescription DEFAULT (0);
    IF COL_LENGTH('dbo.PostModal', 'ShowImage') IS NULL ALTER TABLE dbo.PostModal ADD [ShowImage] bit NOT NULL CONSTRAINT DF_PostModal_ShowImage DEFAULT (0);
    IF COL_LENGTH('dbo.PostModal', 'Publish') IS NULL ALTER TABLE dbo.PostModal ADD [Publish] bit NOT NULL CONSTRAINT DF_PostModal_Publish DEFAULT (0);
END
IF OBJECT_ID(N'dbo.TemplateTypes', N'U') IS NOT NULL
BEGIN
    IF COL_LENGTH('dbo.TemplateTypes', 'Url') IS NULL ALTER TABLE dbo.TemplateTypes ADD [Url] nvarchar(max) NULL;
    IF COL_LENGTH('dbo.TemplateTypes', 'Key') IS NULL ALTER TABLE dbo.TemplateTypes ADD [Key] nvarchar(max) NULL;
    IF COL_LENGTH('dbo.TemplateTypes', 'TrackKey') IS NULL ALTER TABLE dbo.TemplateTypes ADD [TrackKey] nvarchar(max) NOT NULL CONSTRAINT DF_TemplateTypes_TrackKey DEFAULT ('');
END
IF OBJECT_ID(N'dbo.TemplateCategories', N'U') IS NOT NULL
BEGIN
    IF COL_LENGTH('dbo.TemplateCategories', 'Layout') IS NULL ALTER TABLE dbo.TemplateCategories ADD [Layout] nvarchar(max) NOT NULL CONSTRAINT DF_TemplateCategories_Layout DEFAULT ('');
    IF COL_LENGTH('dbo.TemplateCategories', 'LoginLayout') IS NULL ALTER TABLE dbo.TemplateCategories ADD [LoginLayout] nvarchar(max) NOT NULL CONSTRAINT DF_TemplateCategories_LoginLayout DEFAULT ('');
    IF COL_LENGTH('dbo.TemplateCategories', 'TemplateName') IS NULL ALTER TABLE dbo.TemplateCategories ADD [TemplateName] nvarchar(max) NOT NULL CONSTRAINT DF_TemplateCategories_TemplateName DEFAULT ('');
    IF COL_LENGTH('dbo.TemplateCategories', 'TemplateColorPath') IS NULL ALTER TABLE dbo.TemplateCategories ADD [TemplateColorPath] nvarchar(max) NOT NULL CONSTRAINT DF_TemplateCategories_TemplateColorPath DEFAULT ('');
    IF COL_LENGTH('dbo.TemplateCategories', 'UniqueKey') IS NULL ALTER TABLE dbo.TemplateCategories ADD [UniqueKey] nvarchar(max) NOT NULL CONSTRAINT DF_TemplateCategories_UniqueKey DEFAULT ('');
END
IF OBJECT_ID(N'dbo.DataConfigs', N'U') IS NOT NULL
BEGIN
    IF COL_LENGTH('dbo.DataConfigs', 'LayoutJavaScript') IS NULL ALTER TABLE dbo.DataConfigs ADD [LayoutJavaScript] nvarchar(max) NULL;
    IF COL_LENGTH('dbo.DataConfigs', 'MisDashboardCSS') IS NULL ALTER TABLE dbo.DataConfigs ADD [MisDashboardCSS] nvarchar(max) NULL;
    IF COL_LENGTH('dbo.DataConfigs', 'MisJobBoardCSS') IS NULL ALTER TABLE dbo.DataConfigs ADD [MisJobBoardCSS] nvarchar(max) NULL;
    IF COL_LENGTH('dbo.DataConfigs', 'ElectionCustomCss') IS NULL ALTER TABLE dbo.DataConfigs ADD [ElectionCustomCss] nvarchar(max) NULL;
    IF COL_LENGTH('dbo.DataConfigs', 'MisLayoutJavaScript') IS NULL ALTER TABLE dbo.DataConfigs ADD [MisLayoutJavaScript] nvarchar(max) NULL;
    IF COL_LENGTH('dbo.DataConfigs', 'MailSender') IS NULL ALTER TABLE dbo.DataConfigs ADD [MailSender] int NOT NULL CONSTRAINT DF_DataConfigs_MailSender DEFAULT (0);
    IF COL_LENGTH('dbo.DataConfigs', 'OutlookServer') IS NULL ALTER TABLE dbo.DataConfigs ADD [OutlookServer] nvarchar(max) NULL;
    IF COL_LENGTH('dbo.DataConfigs', 'OutlookPort') IS NULL ALTER TABLE dbo.DataConfigs ADD [OutlookPort] int NOT NULL CONSTRAINT DF_DataConfigs_OutlookPort DEFAULT (0);
    IF COL_LENGTH('dbo.DataConfigs', 'OutlookUsername') IS NULL ALTER TABLE dbo.DataConfigs ADD [OutlookUsername] nvarchar(max) NULL;
    IF COL_LENGTH('dbo.DataConfigs', 'OutlookSecurity') IS NULL ALTER TABLE dbo.DataConfigs ADD [OutlookSecurity] nvarchar(max) NULL;
    IF COL_LENGTH('dbo.DataConfigs', 'OutlookSenderEmail') IS NULL ALTER TABLE dbo.DataConfigs ADD [OutlookSenderEmail] nvarchar(max) NULL;
    IF COL_LENGTH('dbo.DataConfigs', 'OutlookSSLEnable') IS NULL ALTER TABLE dbo.DataConfigs ADD [OutlookSSLEnable] bit NOT NULL CONSTRAINT DF_DataConfigs_OutlookSSLEnable DEFAULT (0);
    IF COL_LENGTH('dbo.DataConfigs', 'OutlookUseDefaultCredentials') IS NULL ALTER TABLE dbo.DataConfigs ADD [OutlookUseDefaultCredentials] bit NOT NULL CONSTRAINT DF_DataConfigs_OutlookUseDefaultCredentials DEFAULT (0);
    IF COL_LENGTH('dbo.DataConfigs', 'DisplayName') IS NULL ALTER TABLE dbo.DataConfigs ADD [DisplayName] nvarchar(max) NULL;
    IF COL_LENGTH('dbo.DataConfigs', 'GoogleServer') IS NULL ALTER TABLE dbo.DataConfigs ADD [GoogleServer] nvarchar(max) NULL;
    IF COL_LENGTH('dbo.DataConfigs', 'GooglePort') IS NULL ALTER TABLE dbo.DataConfigs ADD [GooglePort] int NOT NULL CONSTRAINT DF_DataConfigs_GooglePort DEFAULT (0);
    IF COL_LENGTH('dbo.DataConfigs', 'GoogleUsername') IS NULL ALTER TABLE dbo.DataConfigs ADD [GoogleUsername] nvarchar(max) NULL;
    IF COL_LENGTH('dbo.DataConfigs', 'GoogleSecurity') IS NULL ALTER TABLE dbo.DataConfigs ADD [GoogleSecurity] nvarchar(max) NULL;
    IF COL_LENGTH('dbo.DataConfigs', 'GoogleSenderEmail') IS NULL ALTER TABLE dbo.DataConfigs ADD [GoogleSenderEmail] nvarchar(max) NULL;
    IF COL_LENGTH('dbo.DataConfigs', 'GoogleSSLEnable') IS NULL ALTER TABLE dbo.DataConfigs ADD [GoogleSSLEnable] bit NOT NULL CONSTRAINT DF_DataConfigs_GoogleSSLEnable DEFAULT (0);
    IF COL_LENGTH('dbo.DataConfigs', 'GoogleUseDefaultCredentials') IS NULL ALTER TABLE dbo.DataConfigs ADD [GoogleUseDefaultCredentials] bit NOT NULL CONSTRAINT DF_DataConfigs_GoogleUseDefaultCredentials DEFAULT (0);
    IF COL_LENGTH('dbo.DataConfigs', 'WebmailServer') IS NULL ALTER TABLE dbo.DataConfigs ADD [WebmailServer] nvarchar(max) NULL;
    IF COL_LENGTH('dbo.DataConfigs', 'WebmailPort') IS NULL ALTER TABLE dbo.DataConfigs ADD [WebmailPort] int NOT NULL CONSTRAINT DF_DataConfigs_WebmailPort DEFAULT (0);
    IF COL_LENGTH('dbo.DataConfigs', 'WebmailUsername') IS NULL ALTER TABLE dbo.DataConfigs ADD [WebmailUsername] nvarchar(max) NULL;
    IF COL_LENGTH('dbo.DataConfigs', 'WebmailSecurity') IS NULL ALTER TABLE dbo.DataConfigs ADD [WebmailSecurity] nvarchar(max) NULL;
    IF COL_LENGTH('dbo.DataConfigs', 'WebmailSenderEmail') IS NULL ALTER TABLE dbo.DataConfigs ADD [WebmailSenderEmail] nvarchar(max) NULL;
    IF COL_LENGTH('dbo.DataConfigs', 'WebmailSSLEnable') IS NULL ALTER TABLE dbo.DataConfigs ADD [WebmailSSLEnable] bit NOT NULL CONSTRAINT DF_DataConfigs_WebmailSSLEnable DEFAULT (0);
    IF COL_LENGTH('dbo.DataConfigs', 'WebmailUseDefaultCredentials') IS NULL ALTER TABLE dbo.DataConfigs ADD [WebmailUseDefaultCredentials] bit NOT NULL CONSTRAINT DF_DataConfigs_WebmailUseDefaultCredentials DEFAULT (0);
    IF COL_LENGTH('dbo.DataConfigs', 'CCmails') IS NULL ALTER TABLE dbo.DataConfigs ADD [CCmails] nvarchar(max) NULL;
    IF COL_LENGTH('dbo.DataConfigs', 'BBmails') IS NULL ALTER TABLE dbo.DataConfigs ADD [BBmails] nvarchar(max) NULL;
END
IF OBJECT_ID(N'dbo.Testimonies', N'U') IS NOT NULL
BEGIN
    IF COL_LENGTH('dbo.Testimonies', 'CreatedAt') IS NULL
        ALTER TABLE dbo.Testimonies ADD [CreatedAt] datetime2 NOT NULL CONSTRAINT DF_Testimonies_CreatedAt DEFAULT ('0001-01-01T00:00:00.0000000');
    IF COL_LENGTH('dbo.Testimonies', 'Position') IS NULL
        ALTER TABLE dbo.Testimonies ADD [Position] nvarchar(max) NULL;
    IF COL_LENGTH('dbo.Testimonies', 'Rating') IS NULL
        ALTER TABLE dbo.Testimonies ADD [Rating] int NOT NULL CONSTRAINT DF_Testimonies_Rating DEFAULT (0);
    IF COL_LENGTH('dbo.Testimonies', 'SchoolName') IS NULL
        ALTER TABLE dbo.Testimonies ADD [SchoolName] nvarchar(max) NULL;
END");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Intentionally non-destructive to protect existing website data.
        }
    }
}
