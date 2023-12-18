using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CPG.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddCompanyIPG : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "CompanyIPGs",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CompanyId = table.Column<long>(type: "bigint", nullable: false),
                    ProviderId = table.Column<long>(type: "bigint", nullable: false),
                    IPGTypeId = table.Column<long>(type: "bigint", nullable: false),
                    ProviderData = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    VerificationTimeLimit = table.Column<short>(type: "smallint", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreationDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreationUserId = table.Column<long>(type: "bigint", nullable: false),
                    ModificationDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ModificationUserId = table.Column<long>(type: "bigint", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CompanyIPGs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CompanyIPGs_Company_CompanyId",
                        column: x => x.CompanyId,
                        principalTable: "Company",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CompanyIPGs_IPGType_IPGTypeId",
                        column: x => x.IPGTypeId,
                        principalTable: "IPGType",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CompanyIPGs_Provider_ProviderId",
                        column: x => x.ProviderId,
                        principalTable: "Provider",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CompanyIPGDeposits",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CompanyIPGId = table.Column<long>(type: "bigint", nullable: false),
                    CompanyDepositId = table.Column<long>(type: "bigint", nullable: false),
                    IsDefault = table.Column<bool>(type: "bit", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreationDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreationUserId = table.Column<long>(type: "bigint", nullable: false),
                    ModificationDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ModificationUserId = table.Column<long>(type: "bigint", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CompanyIPGDeposits", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CompanyIPGDeposits_CompanyDeposit_CompanyDepositId",
                        column: x => x.CompanyDepositId,
                        principalTable: "CompanyDeposit",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CompanyIPGDeposits_CompanyIPGs_CompanyIPGId",
                        column: x => x.CompanyIPGId,
                        principalTable: "CompanyIPGs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CompanyIPGDeposits_CompanyDepositId",
                table: "CompanyIPGDeposits",
                column: "CompanyDepositId");

            migrationBuilder.CreateIndex(
                name: "IX_CompanyIPGDeposits_CompanyIPGId",
                table: "CompanyIPGDeposits",
                column: "CompanyIPGId");

            migrationBuilder.CreateIndex(
                name: "IX_CompanyIPGs_CompanyId",
                table: "CompanyIPGs",
                column: "CompanyId");

            migrationBuilder.CreateIndex(
                name: "IX_CompanyIPGs_IPGTypeId",
                table: "CompanyIPGs",
                column: "IPGTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_CompanyIPGs_ProviderId",
                table: "CompanyIPGs",
                column: "ProviderId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CompanyIPGDeposits");

            migrationBuilder.DropTable(
                name: "CompanyIPGs");
        }
    }
}
