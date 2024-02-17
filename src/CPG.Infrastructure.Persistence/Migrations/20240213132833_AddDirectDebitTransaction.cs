using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CPG.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddDirectDebitTransaction : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "DirectDebitTransactionId",
                table: "Transaction",
                type: "bigint",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "DirectDebitTransaction",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DirectDebitGrantId = table.Column<long>(type: "bigint", nullable: false),
                    Status = table.Column<byte>(type: "tinyint", nullable: false),
                    TrackId = table.Column<string>(type: "varchar(255)", nullable: false),
                    ProviderTrackerId = table.Column<string>(type: "varchar(255)", nullable: true),
                    ProviderData = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreationDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreationUserId = table.Column<long>(type: "bigint", nullable: false),
                    ModificationDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ModificationUserId = table.Column<long>(type: "bigint", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DirectDebitTransaction", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DirectDebitTransaction_DirectDebitGrant_DirectDebitGrantId",
                        column: x => x.DirectDebitGrantId,
                        principalTable: "DirectDebitGrant",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.NoAction);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Transaction_DirectDebitTransactionId",
                table: "Transaction",
                column: "DirectDebitTransactionId",
                unique: false);

            migrationBuilder.CreateIndex(
                name: "IX_DirectDebitTransaction_DirectDebitGrantId",
                table: "DirectDebitTransaction",
                column: "DirectDebitGrantId");

            migrationBuilder.AddForeignKey(
                name: "FK_Transaction_DirectDebitTransaction_DirectDebitTransactionId",
                table: "Transaction",
                column: "DirectDebitTransactionId",
                principalTable: "DirectDebitTransaction",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Transaction_DirectDebitTransaction_DirectDebitTransactionId",
                table: "Transaction");

            migrationBuilder.DropTable(
                name: "DirectDebitTransaction");

            migrationBuilder.DropIndex(
                name: "IX_Transaction_DirectDebitTransactionId",
                table: "Transaction");

            migrationBuilder.DropColumn(
                name: "DirectDebitTransactionId",
                table: "Transaction");
        }
    }
}
