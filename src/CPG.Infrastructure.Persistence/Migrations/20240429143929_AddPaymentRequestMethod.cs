using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CPG.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddPaymentRequestMethod : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "PaymentRequestMethod",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PaymentRequestId = table.Column<long>(type: "bigint", nullable: false),
                    PaymentMethodType = table.Column<byte>(type: "tinyint", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreationDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreationUserId = table.Column<long>(type: "bigint", nullable: false),
                    ModificationDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ModificationUserId = table.Column<long>(type: "bigint", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PaymentRequestMethod", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PaymentRequestMethod_PaymentRequest_PaymentRequestId",
                        column: x => x.PaymentRequestId,
                        principalTable: "PaymentRequest",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.NoAction);
                });

            migrationBuilder.CreateTable(
                name: "PaymentRequestMethodDeposit",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PaymentRequestMethodId = table.Column<long>(type: "bigint", nullable: false),
                    CompanyDepositId = table.Column<long>(type: "bigint", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreationDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreationUserId = table.Column<long>(type: "bigint", nullable: false),
                    ModificationDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ModificationUserId = table.Column<long>(type: "bigint", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PaymentRequestMethodDeposit", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PaymentRequestMethodDeposit_CompanyDeposit_CompanyDepositId",
                        column: x => x.CompanyDepositId,
                        principalTable: "CompanyDeposit",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.NoAction);
                    table.ForeignKey(
                        name: "FK_PaymentRequestMethodDeposit_PaymentRequestMethod_PaymentRequestMethodId",
                        column: x => x.PaymentRequestMethodId,
                        principalTable: "PaymentRequestMethod",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.NoAction);
                });

            migrationBuilder.CreateTable(
                name: "PaymentRequestMethodIpgType",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PaymentRequestMethodId = table.Column<long>(type: "bigint", nullable: false),
                    IpgTypeId = table.Column<long>(type: "bigint", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreationDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreationUserId = table.Column<long>(type: "bigint", nullable: false),
                    ModificationDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ModificationUserId = table.Column<long>(type: "bigint", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PaymentRequestMethodIpgType", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PaymentRequestMethodIpgType_IPGType_IpgTypeId",
                        column: x => x.IpgTypeId,
                        principalTable: "IPGType",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.NoAction);
                    table.ForeignKey(
                        name: "FK_PaymentRequestMethodIpgType_PaymentRequestMethod_PaymentRequestMethodId",
                        column: x => x.PaymentRequestMethodId,
                        principalTable: "PaymentRequestMethod",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.NoAction);
                });

            migrationBuilder.CreateIndex(
                name: "IX_PaymentRequestMethod_PaymentRequestId",
                table: "PaymentRequestMethod",
                column: "PaymentRequestId");

            migrationBuilder.CreateIndex(
                name: "IX_PaymentRequestMethodDeposit_CompanyDepositId",
                table: "PaymentRequestMethodDeposit",
                column: "CompanyDepositId");

            migrationBuilder.CreateIndex(
                name: "IX_PaymentRequestMethodDeposit_PaymentRequestMethodId",
                table: "PaymentRequestMethodDeposit",
                column: "PaymentRequestMethodId");

            migrationBuilder.CreateIndex(
                name: "IX_PaymentRequestMethodIpgType_IpgTypeId",
                table: "PaymentRequestMethodIpgType",
                column: "IpgTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_PaymentRequestMethodIpgType_PaymentRequestMethodId",
                table: "PaymentRequestMethodIpgType",
                column: "PaymentRequestMethodId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PaymentRequestMethodDeposit");

            migrationBuilder.DropTable(
                name: "PaymentRequestMethodIpgType");

            migrationBuilder.DropTable(
                name: "PaymentRequestMethod");
        }
    }
}
