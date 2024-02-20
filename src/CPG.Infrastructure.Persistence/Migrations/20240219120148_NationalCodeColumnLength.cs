using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CPG.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class NationalCodeColumnLength : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "NationalCode",
                table: "User",
                type: "char(11)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "char(10)");

            migrationBuilder.CreateIndex(
                name: "IX_DirectDebitGrant_BankId",
                table: "DirectDebitGrant",
                column: "BankId");

            migrationBuilder.AddForeignKey(
                name: "FK_DirectDebitGrant_Bank_BankId",
                table: "DirectDebitGrant",
                column: "BankId",
                principalTable: "Bank",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DirectDebitGrant_Bank_BankId",
                table: "DirectDebitGrant");

            migrationBuilder.DropIndex(
                name: "IX_DirectDebitGrant_BankId",
                table: "DirectDebitGrant");

            migrationBuilder.AlterColumn<string>(
                name: "NationalCode",
                table: "User",
                type: "char(10)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "char(11)");
        }
    }
}
