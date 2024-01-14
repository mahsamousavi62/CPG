using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CPG.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class CompanyShaparakSetting : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "CompanyShaparakSetting",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CompanyId = table.Column<long>(type: "bigint", nullable: false),
                    Key = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: true),
                    Iv = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: true),
                    ThirdPartyCode = table.Column<int>(type: "int", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreationDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreationUserId = table.Column<long>(type: "bigint", nullable: false),
                    ModificationDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ModificationUserId = table.Column<long>(type: "bigint", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CompanyShaparakSetting", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CompanyShaparakSetting_Company_CompanyId",
                        column: x => x.CompanyId,
                        principalTable: "Company",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CompanyShaparakSetting_CompanyId",
                table: "CompanyShaparakSetting",
                column: "CompanyId",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CompanyShaparakSetting");
        }
    }
}
