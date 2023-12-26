using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CPG.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddTransactionAndIPGTransaction : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "IPGTransaction",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TrackId = table.Column<string>(type: "varchar(255)", nullable: false),
                    Status = table.Column<short>(type: "smallint", nullable: false),
                    CompanyIPGId = table.Column<long>(type: "bigint", nullable: false),
                    IPGToken = table.Column<string>(type: "varchar(255)", nullable: false),
                    ProviderTrackerId = table.Column<string>(type: "nvarchar(255)", nullable: false),
                    ReferenceNumber = table.Column<string>(type: "varchar(255)", nullable: false),
                    EncryptCardNumber = table.Column<string>(type: "varchar(255)", nullable: false),
                    VerificationTimeLimit = table.Column<int>(type: "int", nullable: false),
                    PredicateDateTime = table.Column<DateTime>(type: "datetime2(7)", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreationDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreationUserId = table.Column<long>(type: "bigint", nullable: false),
                    ModificationDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ModificationUserId = table.Column<long>(type: "bigint", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_IPGTransaction", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Transaction",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PaymentRquestId = table.Column<long>(type: "bigint", nullable: false),
                    ReferenceTransactionId = table.Column<long>(type: "bigint", nullable: false),
                    TransactionMethodType = table.Column<short>(type: "smallint", nullable: false),
                    CompanyId = table.Column<long>(type: "bigint", nullable: false),
                    DestinationDepositId = table.Column<long>(type: "bigint", nullable: false),
                    Amount = table.Column<decimal>(type: "numeric(18,0)", precision: 18, scale: 0, nullable: false),
                    ApplicationId = table.Column<long>(type: "bigint", nullable: false),
                    PredictedSettlementDateTime = table.Column<DateTime>(type: "datetime2(7)", nullable: false),
                    Status = table.Column<short>(type: "smallint", nullable: false),
                    IPGTransactionId = table.Column<long>(type: "bigint", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreationDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreationUserId = table.Column<long>(type: "bigint", nullable: false),
                    ModificationDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ModificationUserId = table.Column<long>(type: "bigint", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Transaction", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Transaction_IPGTransaction_IPGTransactionId",
                        column: x => x.ReferenceTransactionId,
                        principalTable: "IPGTransaction",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Transaction_PaymentRequest_PaymentRquestId",
                        column: x => x.PaymentRquestId,
                        principalTable: "PaymentRequest",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Transaction_IPGTransactionId",
                table: "Transaction",
                column: "IPGTransactionId");

            migrationBuilder.CreateIndex(
                name: "IX_Transaction_PaymentRquestId",
                table: "Transaction",
                column: "PaymentRquestId",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Transaction");

            migrationBuilder.DropTable(
                name: "IPGTransaction");
        }
    }
}
