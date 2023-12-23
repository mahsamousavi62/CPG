using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CPG.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class addCompanyColumn : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
           
            migrationBuilder.AddColumn<short>(
                name: "IpgRedirectionMethodType",
                table: "Company",
                type: "smallint",
                nullable: false,
                defaultValue: (short)1);

            migrationBuilder.AddColumn<string>(
                name: "SiteAddress",
                table: "Company",
                type: "varchar(1000)",
                nullable: false,
                defaultValue: "");

            

        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
           
            migrationBuilder.DropColumn(
                name: "IpgRedirectionMethodType",
                table: "Company");

            migrationBuilder.DropColumn(
                name: "SiteAddress",
                table: "Company");

           
        }
    }
}
