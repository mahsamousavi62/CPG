using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CPG.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class BankProviderSeedData : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"INSERT INTO [dbo].[BankProvider] ([BankId], [ProviderType], [CreationDate], [CreationUserId], [ModificationDate], [ModificationUserId], [IsActive])
                                   VALUES
                                        ('22', '1', '2023-01-01 00:00:00.0000000', 1, null, null, 1),
                                        ('3', '1', '2023-01-01 00:00:00.0000000', 1, null, null, 1);");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
