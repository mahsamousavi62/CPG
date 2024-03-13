using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CPG.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class CreateCardTransaction_DepositDefaultCharismaCard : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsDefaultForCharismaCard",
                table: "CompanyDeposit",
                type: "bit",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsDefaultForCharismaCard",
                table: "CompanyDeposit");
        }
    }
}
