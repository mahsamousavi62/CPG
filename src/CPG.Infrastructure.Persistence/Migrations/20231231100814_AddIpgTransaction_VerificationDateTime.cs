using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CPG.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddIpgTransaction_VerificationDateTime : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Transaction_CompanyDeposit_DestinationDepositId",
                table: "Transaction");

            migrationBuilder.AddColumn<DateTime>(
                name: "VerificationDateTime",
                table: "IPGTransaction",
                type: "datetime2",
                nullable: true
              );

            migrationBuilder.AddForeignKey(
                name: "FK_Transaction_CompanyDeposit_DestinationDepositId",
                table: "Transaction",
                column: "DestinationDepositId",
                principalTable: "CompanyDeposit",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Transaction_CompanyDeposit_DestinationDepositId",
                table: "Transaction");

            migrationBuilder.DropColumn(
                name: "VerificationDateTime",
                table: "IPGTransaction");

            migrationBuilder.AddForeignKey(
                name: "FK_Transaction_CompanyDeposit_DestinationDepositId",
                table: "Transaction",
                column: "DestinationDepositId",
                principalTable: "CompanyDeposit",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
