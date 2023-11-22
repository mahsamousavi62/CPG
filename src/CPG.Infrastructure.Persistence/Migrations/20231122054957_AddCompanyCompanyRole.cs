using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CPG.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddCompanyCompanyRole : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {

            migrationBuilder.CreateTable(
                name: "Company",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PersianName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    EnglishName = table.Column<string>(type: "varchar(256)", maxLength: 256, nullable: false),
                    Logo = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    NationalCodeMatchingRequied = table.Column<bool>(type: "bit", nullable: false),
                    CreationDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreationUserId = table.Column<long>(type: "bigint", nullable: false),
                    ModificationDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ModificationUserId = table.Column<long>(type: "bigint", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Company", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "CompanyPaymentMethods",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MethodType = table.Column<short>(type: "smallint", nullable: false),
                    CompanyId = table.Column<long>(type: "bigint", nullable: false),
                    CreationDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreationUserId = table.Column<long>(type: "bigint", nullable: false),
                    ModificationDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ModificationUserId = table.Column<long>(type: "bigint", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CompanyPaymentMethods", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CompanyPaymentMethods_Company_CompanyId",
                        column: x => x.CompanyId,
                        principalTable: "Company",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CompanyPaymentMethods_CompanyId",
                table: "CompanyPaymentMethods",
                column: "CompanyId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CompanyPaymentMethods");

            migrationBuilder.DropTable(
                name: "Company");

            migrationBuilder.DropColumn(
                name: "CreationDate",
                table: "CPGUser");

            migrationBuilder.DropColumn(
                name: "CreationUserId",
                table: "CPGUser");

            migrationBuilder.DropColumn(
                name: "ModificationDate",
                table: "CPGUser");

            migrationBuilder.DropColumn(
                name: "ModificationUserId",
                table: "CPGUser");

            migrationBuilder.RenameColumn(
                name: "IsActive",
                table: "Application_Settings",
                newName: "IsDeleted");
        }
    }
}
