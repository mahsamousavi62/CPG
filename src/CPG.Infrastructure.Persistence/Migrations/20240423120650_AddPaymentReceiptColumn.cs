using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CPG.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddPaymentReceiptColumn : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "VerificationDateTime",
                table: "PaymentReceiptTransaction",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "VerifiedBy",
                table: "PaymentReceiptTransaction",
                type: "bigint",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "VerificationDateTime",
                table: "PaymentReceiptTransaction");

            migrationBuilder.DropColumn(
                name: "VerifiedBy",
                table: "PaymentReceiptTransaction");
        }
    }
}
