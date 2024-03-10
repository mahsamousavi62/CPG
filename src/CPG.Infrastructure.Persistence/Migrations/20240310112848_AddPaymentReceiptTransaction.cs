using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CPG.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddPaymentReceiptTransaction : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "PaymentReceiptTransactionId",
                table: "Transaction",
                type: "bigint",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "PaymentReceiptTransaction",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SourceIban = table.Column<string>(type: "varchar(26)", maxLength: 26, nullable: false),
                    ReferenceNumber = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    ReceiptDateTime = table.Column<DateTime>(type: "datetime2(7)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    ReceiptImage = table.Column<string>(type: "varchar(max)", nullable: false),
                    Status = table.Column<byte>(type: "tinyint", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreationDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreationUserId = table.Column<long>(type: "bigint", nullable: false),
                    ModificationDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ModificationUserId = table.Column<long>(type: "bigint", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PaymentReceiptTransaction", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Transaction_PaymentReceiptTransactionId",
                table: "Transaction",
                column: "PaymentReceiptTransactionId",
                unique: true,
                filter: "[PaymentReceiptTransactionId] IS NOT NULL");

            migrationBuilder.AddForeignKey(
                name: "FK_Transaction_PaymentReceiptTransaction_PaymentReceiptTransactionId",
                table: "Transaction",
                column: "PaymentReceiptTransactionId",
                principalTable: "PaymentReceiptTransaction",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Transaction_PaymentReceiptTransaction_PaymentReceiptTransactionId",
                table: "Transaction");

            migrationBuilder.DropTable(
                name: "PaymentReceiptTransaction");

            migrationBuilder.DropIndex(
                name: "IX_Transaction_PaymentReceiptTransactionId",
                table: "Transaction");

            migrationBuilder.DropColumn(
                name: "PaymentReceiptTransactionId",
                table: "Transaction");
        }
    }
}
