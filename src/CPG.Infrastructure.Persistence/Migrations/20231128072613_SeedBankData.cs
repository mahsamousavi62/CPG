using CPG.Domain.SeedWork;
using Microsoft.EntityFrameworkCore.Migrations;
using System;

#nullable disable

namespace CPG.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class SeedBankData : Migration
    {
        private static readonly string[] columns = new[] { "Name", "LogoAddress", "IbanPrefix", "CreationDate", "CreationUserId", "ModificationDate", "ModificationUserId", "IsActive" };

        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"INSERT INTO [dbo].[User] ([IDPId],[NationalCode],[CompanyId],[FirstName],[LastName],[PhoneNumber],[LastUpdateFromIDP],[KYCStatus],[IsLegal],[IsActive],[CreatationDateTime],[ModificationDate])
                                    VALUES (1,'1111111111',null,N'کاربر',N'سوپر ادمین','9999999999','2020-01-01 00:00:00.0000000',1,0,1,'2020-01-01 00:00:00.0000000','2020-01-01 00:00:00.0000000');

                                        INSERT INTO [dbo].[Bank] ([Name], [LogoAddress], [IbanPrefix], [CreationDate], [CreationUserId], [ModificationDate], [ModificationUserId], [IsActive])
                                        VALUES
                                        ('بانک پارسیان', 'Bank/Logo/parsian.svg', '054', '2023-01-01 00:00:00.0000000', 1, null, null, 1),
                                        ('بانک اقتصاد نوین', 'Bank/Logo/eghtesadnovin.svg', '055', '2023-01-01 00:00:00.0000000', 1, null, null, 1),
                                        ('بانک سامان', 'Bank/Logo/saman.svg', '056', '2023-01-01 00:00:00.0000000', 1, null, null, 1),
                                        ('بانک دی', 'Bank/Logo/day.svg', '066', '2023-01-01 00:00:00.0000000', 1, null, null, 1),
                                        ('بانک پاسارگاد', 'Bank/Logo/pasargad.svg', '057', '2023-01-01 00:00:00.0000000', 1, null, null, 1),
                                        ('بانک انصار', 'Bank/Logo/ansar.svg', '063', '2023-01-01 00:00:00.0000000', 1, null, null, 1),
                                        ('بانک ملل', 'Bank/Logo/melal.svg', '075', '2023-01-01 00:00:00.0000000', 1, null, null, 1),
                                        ('بانک حکمت ایرانیان', 'Bank/Logo/hekmat.svg', '065', '2023-01-01 00:00:00.0000000', 1, null, null, 1),
                                        ('بانک مسکن', 'Bank/Logo/maskan.svg', '014', '2023-01-01 00:00:00.0000000', 1, null, null, 1),
                                        ('بانک سینا', 'Bank/Logo/sina.svg', '059', '2023-01-01 00:00:00.0000000', 1, null, null, 1),
                                        ('بانک سرمایه', 'Bank/Logo/sarmayeh.svg', '058', '2023-01-01 00:00:00.0000000', 1, null, null, 1),
                                        ('بانک ایران ونزویلا', 'Bank/Logo/iranvenezuela.svg', '095', '2023-01-01 00:00:00.0000000', 1, null, null, 1),
                                        ('بانک ایران زمین', 'Bank/Logo/iranzamin.svg', '069', '2023-01-01 00:00:00.0000000', 1, null, null, 1),
                                        ('بانک مهر ایران', 'Bank/Logo/mehriran.svg', '060', '2023-01-01 00:00:00.0000000', 1, null, null, 1),
                                        ('بانک مهر اقتصاد', 'Bank/Logo/mehreghtesad.svg', '079', '2023-01-01 00:00:00.0000000', 1, null, null, 1),
                                        ('پست بانک ایران', 'Bank/Logo/postbank.svg', '021', '2023-01-01 00:00:00.0000000', 1, null, null, 1),
                                        ('بانک صنعت و معدن', 'Bank/Logo/sanatmadan.svg', '011', '2023-01-01 00:00:00.0000000', 1, null, null, 1),
                                        ('بانک ملی ایران', 'Bank/Logo/melli.svg', '017', '2023-01-01 00:00:00.0000000', 1, null, null, 1),
                                        ('موسسه توسعه', 'Bank/Logo/toseae.svg', '051', '2023-01-01 00:00:00.0000000', 1, null, null, 1),
                                        ('بانک سپه', 'Bank/Logo/sepah.svg', '015', '2023-01-01 00:00:00.0000000', 1, null, null, 1),
                                        ('بانک آینده', 'Bank/Logo/ayandeh.svg', '062', '2023-01-01 00:00:00.0000000', 1, null, null, 1),
                                        ('بانک ملت', 'Bank/Logo/mellat.svg', '012', '2023-01-01 00:00:00.0000000', 1, null, null, 1),
                                        ('بانک تجارت', 'Bank/Logo/tejarat.svg', '018', '2023-01-01 00:00:00.0000000', 1, null, null, 1),
                                        ('بانک کشاورزی', 'Bank/Logo/keshavarzi.svg', '016', '2023-01-01 00:00:00.0000000', 1, null, null, 1),
                                        ('بانک خاورمیانه', 'Bank/Logo/khavarmianeh.svg', '078', '2023-01-01 00:00:00.0000000', 1, null, null, 1),
                                        ('بانک رسالت', 'Bank/Logo/resalat.svg', '070', '2023-01-01 00:00:00.0000000', 1, null, null, 1),
                                        ('بانک صادرات', 'Bank/Logo/saderat.svg', '019', '2023-01-01', 1, NULL, NULL, 1),
                                        ('بانک رفاه', 'Bank/Logo/refah.svg', '013', '2023-01-01', 1, NULL, NULL, 1),
                                        ('بانک شهر', 'Bank/Logo/shahr.svg', '061', '2023-01-01', 1, NULL, NULL, 1),
                                        ('توسعه تعاون', 'Bank/Logo/toseaetaavon.svg', '022', '2023-01-01', 1, NULL, NULL, 1),
                                        ('بانک قوامین', 'Bank/Logo/qavamin.svg', '052', '2023-01-01', 1, NULL, NULL, 1),
                                        ('نامشخص', '' , '073', '2023-01-01', 1, NULL, NULL, 1),
                                        ('بانک گردشگری', 'Bank/Logo/gardeshgari.svg', '064', '2023-01-01', 1, NULL, NULL, 1),
                                        ('کارافرین', 'Bank/Logo/karafarin.svg', '053', '2023-01-01', 1, NULL, NULL, 1),
                                        ('موسسه اعتباری نور', 'Bank/Logo/noor.svg', '080', '2023-01-01', 1, NULL, NULL, 1),
                                        ('توسعه صادرات', 'Bank/Logo/toseaesaderat.svg', '020', '2023-01-01', 1, NULL, NULL, 1),
                                        ('بانک مرکزی', 'Bank/Logo/markazi.svg', '010', '2023-01-01', 1, NULL, NULL, 1);"
            );
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
