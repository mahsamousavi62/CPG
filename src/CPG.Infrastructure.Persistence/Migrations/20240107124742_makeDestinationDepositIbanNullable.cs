using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CPG.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class makeDestinationDepositIbanNullable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {           
            migrationBuilder.AlterColumn<string>(
                name: "DestinationDepositIban",
                table: "PaymentRequest",
                type: "nvarchar(26)",
                maxLength: 26,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(26)",
                oldMaxLength: 26);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
                migrationBuilder.AlterColumn<string>(
                name: "DestinationIban",
                table: "PaymentRequest",
                type: "nvarchar(26)",
                maxLength: 26,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(26)",
                oldMaxLength: 26,
                oldNullable: true);
        }
    }
}
