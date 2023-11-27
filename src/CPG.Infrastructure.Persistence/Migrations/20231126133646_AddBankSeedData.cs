using Microsoft.EntityFrameworkCore.Migrations;
using System;

#nullable disable

namespace CPG.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddBankSeedData : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"INSERT INTO [dbo].[User] ([Id],[IDPId],[NationalCode],[CompanyId],[FirstName],[LastName],[PhoneNumber],[LastUpdateFromIDP],[KYCStatus],[IsLegal],[IsActive],[CreatationDateTime],[ModificationDate])
                                        VALUES (1,1,'1111111111',null,N'کاربر',N'سوپر ادمین','9999999999','2020-01-01 00:00:00.0000000',1,0,1,'2020-01-01 00:00:00.0000000','2020-01-01 00:00:00.0000000')
                                   GO");


            migrationBuilder.InsertData(
                schema: "dbo",
                table: "Bank",
                columns: new[] { "Name", "LogoAddress", "IbanPrefix", "CreationDate", "CreationUserId", "ModificationDate", "ModificationUserId", "IsActive" },
                values: new object[,]
                {
                    { "بانک پارسیان", "Bank/Logo/parsian.svg", "054", new DateTime(2023, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1, null, null, true },
                    { "بانک اقتصاد نوین", "Bank/Logo/eghtesadnovin.svg", "055", new DateTime(2023, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1, null, null, true },
                    { "بانک سامان", "Bank/Logo/saman.svg", "056", new DateTime(2023, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1, null, null, true },
                    { "بانک دی", "Bank/Logo/day.svg", "066", new DateTime(2023, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1, null, null, true },
                    { "بانک پاسارگاد", "Bank/Logo/pasargad.svg", "057", new DateTime(2023, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1, null, null, true },
                    { "بانک انصار", "Bank/Logo/ansar.svg", "063", new DateTime(2023, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1, null, null, true },
                    { "بانک ملل", "Bank/Logo/melal.svg", "075", new DateTime(2023, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1, null, null, true },
                    { "بانک حکمت ایرانیان", "Bank/Logo/hekmat.svg", "065", new DateTime(2023, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1, null, null, true },
                    { "بانک مسکن", "Bank/Logo/maskan.svg", "014", new DateTime(2023, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1, null, null, true },
                    { "بانک سینا", "Bank/Logo/sina.svg", "059", new DateTime(2023, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1, null, null, true },
                    { "بانک سرمایه", "Bank/Logo/sarmayeh.svg", "058", new DateTime(2023, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1, null, null, true },
                    { "بانک ایران ونزویلا", "Bank/Logo/iranvenezuela.svg", "095", new DateTime(2023, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1, null, null, true },
                    { "بانک ایران زمین", "Bank/Logo/iranzamin.svg", "069", new DateTime(2023, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1, null, null, true },
                    { "بانک مهر ایران", "Bank/Logo/mehriran.svg", "060", new DateTime(2023, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1, null, null, true },
                    { "بانک مهر اقتصاد", "Bank/Logo/mehreghtesad.svg", "079", new DateTime(2023, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1, null, null, true },
                    { "پست بانک ایران", "Bank/Logo/postbank.svg", "021", new DateTime(2023, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1, null, null, true },
                    { "بانک صنعت و معدن", "Bank/Logo/sanatmadan.svg", "011", new DateTime(2023, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1, null, null, true },
                    { "بانک ملی ایران", "Bank/Logo/melli.svg", "017", new DateTime(2023, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1, null, null, true },
                    { "موسسه توسعه", "Bank/Logo/toseae.svg", "051", new DateTime(2023, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1, null, null, true },
                    { "بانک سپه", "Bank/Logo/sepah.svg", "015", new DateTime(2023, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1, null, null, true },
                    { "بانک آینده", "Bank/Logo/ayandeh.svg", "062", new DateTime(2023, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1, null, null, true },
                    { "بانک ملت", "Bank/Logo/mellat.svg", "012", new DateTime(2023, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1, null, null, true },
                    { "بانک تجارت", "Bank/Logo/tejarat.svg", "018", new DateTime(2023, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1, null, null, true },
                    { "بانک کشاورزی", "Bank/Logo/keshavarzi.svg", "016", new DateTime(2023, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1, null, null, true },
                    { "بانک خاورمیانه", "Bank/Logo/khavarmianeh.svg", "078", new DateTime(2023, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1, null, null, true },
                    { "بانک رسالت", "Bank/Logo/resalat.svg", "070", new DateTime(2023, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1, null, null, true },
                    { "بانک صادرات", "Bank/Logo/saderat.svg", "019", new DateTime(2023, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1, null, null, true },
                    { "بانک  رفاه", "Bank/Logo/refah.svg", "013", new DateTime(2023, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1, null, null, true },
                    { "بانک شهر", "Bank/Logo/shahr.svg", "061", new DateTime(2023, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1, null, null, true },
                    { "توسعه تعاون", "Bank/Logo/toseaetaavon.svg", "022", new DateTime(2023, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1, null, null, true },
                    { "بانک قوامین", "Bank/Logo/qavamin.svg", "052", new DateTime(2023, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1, null, null, true },
                    { "نامشخص", null, "073", new DateTime(2023, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1, null, null, true },
                    { "بانک گردشگری", "Bank/Logo/gardeshgari.svg", "064", new DateTime(2023, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1, null, null, true },
                    { "کارافرین", "Bank/Logo/karafarin.svg", "053", new DateTime(2023, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1, null, null, true },
                    { "موسسه اعتباری نور", "Bank/Logo/noor.svg", "080", new DateTime(2023, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1, null, null, true },
                    { "توسعه صادرات", "Bank/Logo/toseaesaderat.svg", "020", new DateTime(2023, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1, null, null, true },
                    { "بانک مرکزی", "Bank/Logo/markazi.svg", "010", new DateTime(2023, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1, null, null, true },
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
