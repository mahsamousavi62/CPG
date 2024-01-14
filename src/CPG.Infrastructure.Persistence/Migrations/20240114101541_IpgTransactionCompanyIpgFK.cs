using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CPG.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class IpgTransactionCompanyIpgFK : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_IPGTransaction_CompanyIPGId",
                table: "IPGTransaction",
                column: "CompanyIPGId");

            migrationBuilder.AddForeignKey(
                name: "FK_IPGTransaction_CompanyIPG_CompanyIPGId",
                table: "IPGTransaction",
                column: "CompanyIPGId",
                principalTable: "CompanyIPG",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_IPGTransaction_CompanyIPG_CompanyIPGId",
                table: "IPGTransaction");

            migrationBuilder.DropIndex(
                name: "IX_IPGTransaction_CompanyIPGId",
                table: "IPGTransaction");
        }
    }
}
