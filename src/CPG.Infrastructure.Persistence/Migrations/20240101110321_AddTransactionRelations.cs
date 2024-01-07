using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CPG.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddTransactionRelations : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Transaction_IPGTransaction_IPGTransactionId",
                table: "Transaction");

            migrationBuilder.DropForeignKey(
                name: "FK_Transaction_PaymentRequest_PaymentRquestId",
                table: "Transaction");

            migrationBuilder.AddColumn<DateTime>(
                name: "VerificationDateTime",
                table: "IPGTransaction",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.CreateIndex(
                name: "IX_Transaction_DestinationDepositId",
                table: "Transaction",
                column: "DestinationDepositId");

            migrationBuilder.AddForeignKey(
                name: "FK_Transaction_CompanyDeposit_DestinationDepositId",
                table: "Transaction",
                column: "DestinationDepositId",
                principalTable: "CompanyDeposit",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Transaction_IPGTransaction_IPGTransactionId",
                table: "Transaction",
                column: "IPGTransactionId",
                principalTable: "IPGTransaction",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Transaction_PaymentRequest_PaymentRquestId",
                table: "Transaction",
                column: "PaymentRquestId",
                principalTable: "PaymentRequest",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Transaction_CompanyDeposit_DestinationDepositId",
                table: "Transaction");

            migrationBuilder.DropForeignKey(
                name: "FK_Transaction_IPGTransaction_IPGTransactionId",
                table: "Transaction");

            migrationBuilder.DropForeignKey(
                name: "FK_Transaction_PaymentRequest_PaymentRquestId",
                table: "Transaction");

            migrationBuilder.DropIndex(
                name: "IX_Transaction_DestinationDepositId",
                table: "Transaction");

            migrationBuilder.DropColumn(
                name: "VerificationDateTime",
                table: "IPGTransaction");

            migrationBuilder.AddForeignKey(
                name: "FK_Transaction_IPGTransaction_IPGTransactionId",
                table: "Transaction",
                column: "IPGTransactionId",
                principalTable: "IPGTransaction",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Transaction_PaymentRequest_PaymentRquestId",
                table: "Transaction",
                column: "PaymentRquestId",
                principalTable: "PaymentRequest",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
