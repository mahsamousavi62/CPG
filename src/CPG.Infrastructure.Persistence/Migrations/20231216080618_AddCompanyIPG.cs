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
                name: "CompanyIPG",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CompanyId = table.Column<long>(type: "bigint", nullable: false),
                    ProviderId = table.Column<long>(type: "bigint", nullable: false),
                    IPGTypeId = table.Column<long>(type: "bigint", nullable: false),
                    ProviderData = table.Column<string>(type: "varchar(max)", nullable: false),
                    VerificationTimeLimit = table.Column<byte>(type: "tinyint", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreationDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreationUserId = table.Column<long>(type: "bigint", nullable: false),
                    ModificationDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ModificationUserId = table.Column<long>(type: "bigint", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CompanyIPG", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CompanyIPG_Company_CompanyId",
                        column: x => x.CompanyId,
                        principalTable: "Company",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.NoAction);
                    table.ForeignKey(
                        name: "FK_CompanyIPG_IPGType_IPGTypeId",
                        column: x => x.IPGTypeId,
                        principalTable: "IPGType",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.NoAction);
                    table.ForeignKey(
                        name: "FK_CompanyIPG_Provider_ProviderId",
                        column: x => x.ProviderId,
                        principalTable: "Provider",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.NoAction);
                });

            migrationBuilder.CreateTable(
                name: "CompanyIPGDeposit",
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
                    table.PrimaryKey("PK_CompanyIPGDeposit", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CompanyIPGDeposit_CompanyDeposit_CompanyDepositId",
                        column: x => x.CompanyDepositId,
                        principalTable: "CompanyDeposit",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.NoAction);
                    table.ForeignKey(
                        name: "FK_CompanyIPGDeposit_CompanyIPG_CompanyIPGId",
                        column: x => x.CompanyIPGId,
                        principalTable: "CompanyIPG",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.NoAction);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CompanyIPG_CompanyId",
                table: "CompanyIPG",
                column: "CompanyId");

            migrationBuilder.CreateIndex(
                name: "IX_CompanyIPG_IPGTypeId",
                table: "CompanyIPG",
                column: "IPGTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_CompanyIPG_ProviderId",
                table: "CompanyIPG",
                column: "ProviderId");

            migrationBuilder.CreateIndex(
                name: "IX_CompanyIPGDeposit_CompanyDepositId",
                table: "CompanyIPGDeposit",
                column: "CompanyDepositId");

            migrationBuilder.CreateIndex(
                name: "IX_CompanyIPGDeposit_CompanyIPGId",
                table: "CompanyIPGDeposit",
                column: "CompanyIPGId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CompanyIPGDeposit");
                       
            migrationBuilder.DropTable(
                name: "CompanyIPG");
        }
    }
}
