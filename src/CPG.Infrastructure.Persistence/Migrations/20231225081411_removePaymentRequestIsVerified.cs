using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CPG.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class removePaymentRequestIsVerified : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
           
            migrationBuilder.DropColumn(
                name: "IsVerified",
                table: "PaymentRequest");

        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
           
            migrationBuilder.AddColumn<bool>(
                name: "IsVerified",
                table: "PaymentRequest",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }
    }
}
