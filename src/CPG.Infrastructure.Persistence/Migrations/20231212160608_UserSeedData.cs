using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CPG.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class UserSeedData : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"INSERT INTO [dbo].[User] ([IDPId] ,[NationalCode] ,[CompanyId] ,[FirstName] ,[LastName] ,[PhoneNumber] ,[LastUpdateFromIDP] ,[KYCStatus] ,[IsLegal] ,[IsActive] ,[CreationDate] ,[ModificationDate] ,[CreationUserId] ,[ModificationUserId])
                                   VALUES (null, '0440061423', null, N'محسن', N'کریمی' ,'09931867732', null, 0, 0, 1, GETDATE(), null, 1, null),
                                          (null, '0453684769', null, N'وحید', N'صابری', '09124751426', null, 0, 0, 1, GETDATE(), null, 1, null),
                                          (null, '0083049509', null, N'مریم', N'سپاسی', '09123110564', null, 0, 0, 1, GETDATE(), null, 1, null),
                                          (null, '0068426879', null, N'مهسا', N'موسوی', '09122267449', null, 0, 0, 1, GETDATE(), null, 1, null),
                                          (null, '4420684925', null, N'مهدخت', N'کلانتری', '09135262863', null, 0, 0, 1, GETDATE(), null, 1, null),
                                          (null, '0079780423', null, N'سید علی', N'کیانور', '09360303421', null, 0, 0, 1, GETDATE(), null, 1, null),
                                          (null, '0017280206', null, N'آیدین', N'میرزایی', '09127116044', null, 0, 0, 1, GETDATE(), null, 1, null);
                                  
                                   IF EXISTS (SELECT * FROM [dbo].[User] WHERE [NationalCode] = '0440061423')
                                   INSERT INTO [dbo].[UserRole]
                                              ([UserId] ,[RoleType], [CreationDate] ,[ModificationDate] , [CreationUserId] ,[IsActive] ,[ModificationUserId])
                                        VALUES
                                              ((SELECT [Id] FROM [dbo].[User] WHERE [NationalCode] = '0440061423'), 1, GETDATE(), null , 1 , 1 , null)
                                   GO
                                   IF EXISTS (SELECT * FROM [dbo].[User] WHERE [NationalCode] = '0453684769')
                                   INSERT INTO [dbo].[UserRole]
                                              ([UserId] ,[RoleType], [CreationDate] ,[ModificationDate] , [CreationUserId] ,[IsActive] ,[ModificationUserId])
                                        VALUES
                                              ((SELECT [Id] FROM [dbo].[User] WHERE [NationalCode] = '0453684769'), 1, GETDATE(), null , 1 , 1 , null)
                                   GO
                                   IF EXISTS (SELECT * FROM [dbo].[User] WHERE [NationalCode] = '0083049509')
                                   INSERT INTO [dbo].[UserRole]
                                              ([UserId] ,[RoleType], [CreationDate] ,[ModificationDate] , [CreationUserId] ,[IsActive] ,[ModificationUserId])
                                        VALUES
                                              ((SELECT [Id] FROM [dbo].[User] WHERE [NationalCode] = '0083049509'), 1, GETDATE(), null , 1 , 1 , null)
                                   GO
                                   IF EXISTS (SELECT * FROM [dbo].[User] WHERE [NationalCode] = '0068426879')
                                   INSERT INTO [dbo].[UserRole]
                                              ([UserId] ,[RoleType], [CreationDate] ,[ModificationDate] , [CreationUserId] ,[IsActive] ,[ModificationUserId])
                                        VALUES
                                              ((SELECT [Id] FROM [dbo].[User] WHERE [NationalCode] = '0068426879'), 1, GETDATE(), null , 1 , 1 , null)
                                   GO
                                   IF EXISTS (SELECT * FROM [dbo].[User] WHERE [NationalCode] = '4420684925')
                                   INSERT INTO [dbo].[UserRole]
                                              ([UserId] ,[RoleType], [CreationDate] ,[ModificationDate] , [CreationUserId] ,[IsActive] ,[ModificationUserId])
                                        VALUES
                                              ((SELECT [Id] FROM [dbo].[User] WHERE [NationalCode] = '4420684925'), 1, GETDATE(), null , 1 , 1 , null)
                                   GO
                                   IF EXISTS (SELECT * FROM [dbo].[User] WHERE [NationalCode] = '0079780423')
                                   INSERT INTO [dbo].[UserRole]
                                              ([UserId] ,[RoleType], [CreationDate] ,[ModificationDate] , [CreationUserId] ,[IsActive] ,[ModificationUserId])
                                        VALUES
                                              ((SELECT [Id] FROM [dbo].[User] WHERE [NationalCode] = '0079780423'), 1, GETDATE(), null , 1 , 1 , null)
                                   GO
                                   IF EXISTS (SELECT * FROM [dbo].[User] WHERE [NationalCode] = '0017280206')
                                   INSERT INTO [dbo].[UserRole]
                                              ([UserId] ,[RoleType], [CreationDate] ,[ModificationDate] , [CreationUserId] ,[IsActive] ,[ModificationUserId])
                                        VALUES
                                              ((SELECT [Id] FROM [dbo].[User] WHERE [NationalCode] = '0017280206'), 1, GETDATE(), null , 1 , 1 , null)
                                   GO");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            
        }
    }
}
