using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CPG.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class updateProvider : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IpgBaseUrl",
                table: "Provider");

            migrationBuilder.DropColumn(
                name: "IpgVerificationTimeLimit",
                table: "Provider");

            migrationBuilder.AlterColumn<byte>(
                name: "MethodType",
                table: "CompanyPaymentMethods",
                type: "tinyint",
                nullable: false,
                oldClrType: typeof(short),
                oldType: "smallint");

            migrationBuilder.CreateTable(
                name: "ProviderPaymentMethod",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MethodType = table.Column<byte>(type: "tinyint", nullable: false),
                    ProviderId = table.Column<long>(type: "bigint", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreationDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreationUserId = table.Column<long>(type: "bigint", nullable: false),
                    ModificationDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ModificationUserId = table.Column<long>(type: "bigint", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProviderPaymentMethod", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProviderPaymentMethod_Provider_ProviderId",
                        column: x => x.ProviderId,
                        principalTable: "Provider",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ProviderPaymentMethod_ProviderId",
                table: "ProviderPaymentMethod",
                column: "ProviderId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ProviderPaymentMethod");

            migrationBuilder.AddColumn<string>(
                name: "IpgBaseUrl",
                table: "Provider",
                type: "varchar(1000)",
                maxLength: 1000,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<byte>(
                name: "IpgVerificationTimeLimit",
                table: "Provider",
                type: "tinyint",
                nullable: false,
                defaultValue: (byte)0);

            migrationBuilder.AlterColumn<short>(
                name: "MethodType",
                table: "CompanyPaymentMethods",
                type: "smallint",
                nullable: false,
                oldClrType: typeof(byte),
                oldType: "tinyint");
        }
    }
}
