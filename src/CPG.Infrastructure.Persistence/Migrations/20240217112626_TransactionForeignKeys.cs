using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CPG.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class TransactionForeignKeys : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Transaction_DirectDebitTransactionId",
                table: "Transaction");

            migrationBuilder.DropIndex(
                name: "IX_Transaction_IPGTransactionId",
                table: "Transaction");

            migrationBuilder.AlterColumn<long>(
                name: "IPGTransactionId",
                table: "Transaction",
                type: "bigint",
                nullable: true,
                oldClrType: typeof(long),
                oldType: "bigint");

            migrationBuilder.AlterColumn<long>(
                name: "DirectDebitTransactionId",
                table: "Transaction",
                type: "bigint",
                nullable: true,
                oldClrType: typeof(long),
                oldType: "bigint");

            migrationBuilder.CreateIndex(
                name: "IX_Transaction_DirectDebitTransactionId",
                table: "Transaction",
                column: "DirectDebitTransactionId",
                unique: true,
                filter: "[DirectDebitTransactionId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Transaction_IPGTransactionId",
                table: "Transaction",
                column: "IPGTransactionId",
                unique: true,
                filter: "[IPGTransactionId] IS NOT NULL");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Transaction_DirectDebitTransactionId",
                table: "Transaction");

            migrationBuilder.DropIndex(
                name: "IX_Transaction_IPGTransactionId",
                table: "Transaction");

            migrationBuilder.AlterColumn<long>(
                name: "IPGTransactionId",
                table: "Transaction",
                type: "bigint",
                nullable: false,
                defaultValue: 0L,
                oldClrType: typeof(long),
                oldType: "bigint",
                oldNullable: true);

            migrationBuilder.AlterColumn<long>(
                name: "DirectDebitTransactionId",
                table: "Transaction",
                type: "bigint",
                nullable: false,
                defaultValue: 0L,
                oldClrType: typeof(long),
                oldType: "bigint",
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Transaction_DirectDebitTransactionId",
                table: "Transaction",
                column: "DirectDebitTransactionId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Transaction_IPGTransactionId",
                table: "Transaction",
                column: "IPGTransactionId",
                unique: true);
        }
    }
}
