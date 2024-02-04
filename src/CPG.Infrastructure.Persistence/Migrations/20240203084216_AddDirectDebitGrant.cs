using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CPG.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddDirectDebitGrant : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "DirectDebitGrant",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<long>(type: "bigint", nullable: false),
                    BankId = table.Column<int>(type: "int", nullable: false),
                    AccountNumber = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: true),
                    PhoneNumber = table.Column<string>(type: "char(11)", maxLength: 11, nullable: false),
                    SuccessTransactionCountLimitPerMonth = table.Column<int>(type: "int", nullable: false),
                    AmountLimitPerTransaction = table.Column<decimal>(type: "numeric(18,0)", nullable: false),
                    TrackId = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: true),
                    ExpirationDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    RevokeDateTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ProviderId = table.Column<long>(type: "bigint", nullable: false),
                    GrantToken = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: true),
                    AuthorizationId = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: true),
                    Status = table.Column<byte>(type: "tinyint", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreationDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreationUserId = table.Column<long>(type: "bigint", nullable: false),
                    ModificationDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ModificationUserId = table.Column<long>(type: "bigint", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DirectDebitGrant", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DirectDebitGrant_Provider_ProviderId",
                        column: x => x.ProviderId,
                        principalTable: "Provider",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_BankDirectDebitSetting_ProviderId",
                table: "BankDirectDebitSetting",
                column: "ProviderId");

            migrationBuilder.CreateIndex(
                name: "IX_DirectDebitGrant_ProviderId",
                table: "DirectDebitGrant",
                column: "ProviderId");

            migrationBuilder.AddForeignKey(
                name: "FK_BankDirectDebitSetting_Provider_ProviderId",
                table: "BankDirectDebitSetting",
                column: "ProviderId",
                principalTable: "Provider",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BankDirectDebitSetting_Provider_ProviderId",
                table: "BankDirectDebitSetting");

            migrationBuilder.DropTable(
                name: "DirectDebitGrant");

            migrationBuilder.DropIndex(
                name: "IX_BankDirectDebitSetting_ProviderId",
                table: "BankDirectDebitSetting");
        }
    }
}
