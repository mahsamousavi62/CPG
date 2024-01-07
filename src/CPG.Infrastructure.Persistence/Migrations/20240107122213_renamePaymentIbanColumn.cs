using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CPG.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class renamePaymentIbanColumn : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
             name: "DestinationIban",
             table: "PaymentRequest",
             newName: "DestinationDepositIban",
             schema: "dbo");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
              name: "DestinationDepositIban",
              table: "PaymentRequest",
              newName: "DestinationIban",
              schema: "dbo");
        }
    }
}
