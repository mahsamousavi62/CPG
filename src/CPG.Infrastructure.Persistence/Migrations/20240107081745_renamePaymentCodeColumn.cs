using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CPG.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class renamePaymentCodeColumn : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
             name: "Code",
             table: "PaymentRequest",
             newName: "PaymentCode",
             schema: "dbo");


        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
              name: "PaymentCode",
              table: "PaymentRequest",
              newName: "Code",
              schema: "dbo");
        }
    }
}
