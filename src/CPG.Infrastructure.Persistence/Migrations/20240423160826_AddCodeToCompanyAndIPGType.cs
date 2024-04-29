using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CPG.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddCodeToCompanyAndIPGType : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<byte>(
                name: "Code",
                table: "IPGType",
                type: "tinyint",
                nullable: false,
                defaultValue: (byte)0);

            migrationBuilder.AddColumn<byte>(
                name: "Code",
                table: "Company",
                type: "tinyint",
                nullable: true);

            migrationBuilder.Sql(@"UPDATE [dbo].[IPGType] SET [Code] = 1 WHERE [EnglishName] = 'Asan_Pardakht';
                                   UPDATE [dbo].[IPGType] SET [Code] = 2 WHERE [EnglishName] = 'Beh_Pradakht';
                                   UPDATE [dbo].[IPGType] SET [Code] = 3 WHERE [EnglishName] = 'Pardakht_Electronic_Saman_Kish';
                                   UPDATE [dbo].[IPGType] SET [Code] = 4 WHERE [EnglishName] = 'Tejarat_Electronic_Parsian';
                                   UPDATE [dbo].[IPGType] SET [Code] = 5 WHERE [EnglishName] = 'Pardakht_Electronic_Pasargad';
                                   UPDATE [dbo].[IPGType] SET [Code] = 6 WHERE [EnglishName] = 'Tejarat_Electronic_Ertebat_Farda';");

        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Code",
                table: "IPGType");

            migrationBuilder.DropColumn(
                name: "Code",
                table: "Company");
        }
    }
}
