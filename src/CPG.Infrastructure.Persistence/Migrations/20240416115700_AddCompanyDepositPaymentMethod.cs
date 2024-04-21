using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CPG.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddCompanyDepositPaymentMethod : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CompanyPaymentMethods_Company_CompanyId",
                table: "CompanyPaymentMethods");

            migrationBuilder.DropPrimaryKey(
                name: "PK_CompanyPaymentMethods",
                table: "CompanyPaymentMethods");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Application_Settings",
                table: "Application_Settings");

            migrationBuilder.RenameTable(
                name: "CompanyPaymentMethods",
                newName: "CompanyPaymentMethod");

            migrationBuilder.RenameTable(
                name: "Application_Settings",
                newName: "ApplicationSetting");

            migrationBuilder.RenameIndex(
                name: "IX_CompanyPaymentMethods_CompanyId",
                table: "CompanyPaymentMethod",
                newName: "IX_CompanyPaymentMethod_CompanyId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_CompanyPaymentMethod",
                table: "CompanyPaymentMethod",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ApplicationSetting",
                table: "ApplicationSetting",
                column: "Id");

            migrationBuilder.CreateTable(
                name: "CompanyDepositPaymentMethod",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MethodType = table.Column<byte>(type: "tinyint", nullable: false),
                    CompanyDepositId = table.Column<long>(type: "bigint", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreationDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreationUserId = table.Column<long>(type: "bigint", nullable: false),
                    ModificationDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ModificationUserId = table.Column<long>(type: "bigint", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CompanyDepositPaymentMethod", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CompanyDepositPaymentMethod_CompanyDeposit_CompanyDepositId",
                        column: x => x.CompanyDepositId,
                        principalTable: "CompanyDeposit",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CompanyDepositPaymentMethod_CompanyDepositId",
                table: "CompanyDepositPaymentMethod",
                column: "CompanyDepositId");

            migrationBuilder.AddForeignKey(
                name: "FK_CompanyPaymentMethod_Company_CompanyId",
                table: "CompanyPaymentMethod",
                column: "CompanyId",
                principalTable: "Company",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CompanyPaymentMethod_Company_CompanyId",
                table: "CompanyPaymentMethod");

            migrationBuilder.DropTable(
                name: "CompanyDepositPaymentMethod");

            migrationBuilder.DropPrimaryKey(
                name: "PK_CompanyPaymentMethod",
                table: "CompanyPaymentMethod");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ApplicationSetting",
                table: "ApplicationSetting");

            migrationBuilder.RenameTable(
                name: "CompanyPaymentMethod",
                newName: "CompanyPaymentMethods");

            migrationBuilder.RenameTable(
                name: "ApplicationSetting",
                newName: "Application_Settings");

            migrationBuilder.RenameIndex(
                name: "IX_CompanyPaymentMethod_CompanyId",
                table: "CompanyPaymentMethods",
                newName: "IX_CompanyPaymentMethods_CompanyId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_CompanyPaymentMethods",
                table: "CompanyPaymentMethods",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Application_Settings",
                table: "Application_Settings",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_CompanyPaymentMethods_Company_CompanyId",
                table: "CompanyPaymentMethods",
                column: "CompanyId",
                principalTable: "Company",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
