using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CPG.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddBankDirectDebitSetting : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "HasDirectDebitFeature",
                table: "Bank",
                type: "bit",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "BankDirectDebitSetting",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ProviderId = table.Column<long>(type: "bigint", nullable: false),
                    BankId = table.Column<long>(type: "int", nullable: false),
                    DDBankCode = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: false),
                    MaxWithdrawalAmountPerDay = table.Column<decimal>(type: "numeric(18,0)", precision: 18, scale: 0, nullable: false),
                    MaxMandateValidityDurationPerMonth = table.Column<byte>(type: "tinyint", nullable: false),
                    AuthenticationType = table.Column<byte>(type: "tinyint", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreationDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreationUserId = table.Column<long>(type: "bigint", nullable: false),
                    ModificationDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ModificationUserId = table.Column<long>(type: "bigint", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BankDirectDebitSetting", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BankDirectDebitSetting_Bank_BankId",
                        column: x => x.BankId,
                        principalTable: "Bank",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_BankDirectDebitSetting_BankId",
                table: "BankDirectDebitSetting",
                column: "BankId",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BankDirectDebitSetting");

            migrationBuilder.DropColumn(
                name: "HasDirectDebitFeature",
                table: "Bank");
        }
    }
}
