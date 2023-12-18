using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CPG.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddIndexToUserAndRole : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_UserRole_RoleType_UserId",
                table: "UserRole",
                columns: new[] { "RoleType", "UserId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_User_NationalCode",
                table: "User",
                column: "NationalCode",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_UserRole_RoleType_UserId",
                table: "UserRole");

            migrationBuilder.DropIndex(
                name: "IX_User_NationalCode",
                table: "User");
        }
    }
}
