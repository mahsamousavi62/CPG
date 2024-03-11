using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CPG.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddCharismaCardTransaction : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "CharismaCardTransactionId",
                table: "Transaction",
                type: "bigint",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "CharismaCardTransaction",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TrackId = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: false),
                    ProviderTrackId = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    ReferenceNumber = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: false),
                    Status = table.Column<byte>(type: "tinyint", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreationDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreationUserId = table.Column<long>(type: "bigint", nullable: false),
                    ModificationDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ModificationUserId = table.Column<long>(type: "bigint", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CharismaCardTransaction", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Transaction_CharismaCardTransactionId",
                table: "Transaction",
                column: "CharismaCardTransactionId",
                unique: true,
                filter: "[CharismaCardTransactionId] IS NOT NULL");

            migrationBuilder.AddForeignKey(
                name: "FK_Transaction_CharismaCardTransaction_CharismaCardTransactionId",
                table: "Transaction",
                column: "CharismaCardTransactionId",
                principalTable: "CharismaCardTransaction",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Transaction_CharismaCardTransaction_CharismaCardTransactionId",
                table: "Transaction");

            migrationBuilder.DropTable(
                name: "CharismaCardTransaction");

            migrationBuilder.DropIndex(
                name: "IX_Transaction_CharismaCardTransactionId",
                table: "Transaction");

            migrationBuilder.DropColumn(
                name: "CharismaCardTransactionId",
                table: "Transaction");
        }
    }
}
