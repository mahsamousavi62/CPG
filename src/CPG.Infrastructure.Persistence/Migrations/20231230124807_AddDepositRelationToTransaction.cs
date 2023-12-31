using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CPG.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddDepositRelationToTransaction : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_Transaction_DestinationDepositId",
                table: "Transaction",
                column: "DestinationDepositId");

            migrationBuilder.AddForeignKey(
                name: "FK_Transaction_CompanyDeposit_DestinationDepositId",
                table: "Transaction",
                column: "DestinationDepositId",
                principalTable: "CompanyDeposit",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Transaction_CompanyDeposit_DestinationDepositId",
                table: "Transaction");

            migrationBuilder.DropIndex(
                name: "IX_Transaction_DestinationDepositId",
                table: "Transaction");
        }
    }
}
