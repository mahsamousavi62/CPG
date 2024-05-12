using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CPG.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class deletePaymentDestinationIban : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            

            migrationBuilder.DropColumn(
                name: "DestinationDepositIban",
                table: "PaymentRequest");

           
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
         
            

            migrationBuilder.AddColumn<string>(
                name: "DestinationDepositIban",
                table: "PaymentRequest",
                type: "nvarchar(26)",
                maxLength: 26,
                nullable: true);

            
        }
    }
}
