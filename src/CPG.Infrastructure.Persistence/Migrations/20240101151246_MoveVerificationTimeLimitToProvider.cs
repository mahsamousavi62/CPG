using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CPG.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class MoveVerificationTimeLimitToProvider : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "VerificationTimeLimit",
                table: "CompanyIPG");

            migrationBuilder.AddColumn<string>(
                name: "IpgBaseUrl",
                table: "Provider",
                type: "varchar(1000)",
                maxLength: 1000,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<byte>(
                name: "IpgVerificationTimeLimit",
                table: "Provider",
                type: "tinyint",
                nullable: false,
                defaultValue: (byte)0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IpgBaseUrl",
                table: "Provider");

            migrationBuilder.DropColumn(
                name: "IpgVerificationTimeLimit",
                table: "Provider");

            migrationBuilder.AddColumn<byte>(
                name: "VerificationTimeLimit",
                table: "CompanyIPG",
                type: "tinyint",
                nullable: false,
                defaultValue: (byte)0);
        }
    }
}
