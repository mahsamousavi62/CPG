using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CPG.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class IPGTypeSeedData : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"INSERT INTO [dbo].[IPGType] ([PersianName], [EnglishName], [Logo], [CreationDate], [CreationUserId], [ModificationDate], [ModificationUserId], [IsActive])
                                   VALUES
                                        ('آسان پرداخت', 'Asan_Pardakht', 'IPGType/Logo/Asan_Pardakht.svg', '2023-01-01 00:00:00.0000000', 1, null, null, 1),
                                        ('بانک ملت', 'Beh_Pradakht', 'IPGType/Logo/Mellat.svg', '2023-01-01 00:00:00.0000000', 1, null, null, 1),
                                        ('بانک سامان', 'Pardakht_Electronic_Saman_Kish', 'IPGType/Logo/Saman.svg', '2023-01-01 00:00:00.0000000', 1, null, null, 1),
                                        ('بانک پارسیان', 'Tejarat_Electronic_Parsian', 'IPGType/Logo/Parsian.svg', '2023-01-01 00:00:00.0000000', 1, null, null, 1),
                                        ('بانک پاسارگاد', 'Pardakht_Electronic_Pasargad', 'IPGType/Logo/Pasargad.svg', '2023-01-01 00:00:00.0000000', 1, null, null, 1)");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
