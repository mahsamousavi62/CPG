using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CPG.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class DirectDebitPlanSeedData : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"INSERT INTO [dbo].[DirectDebitPlan] ([DurationPerMonth], [IsActive], [CreationDate], [CreationUserId])
                                   VALUES
                                        ('3', '1', GETDATE(), 1),
                                        ('6', '1', GETDATE(), 1),
                                        ('12', '1', GETDATE(), 1);");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
