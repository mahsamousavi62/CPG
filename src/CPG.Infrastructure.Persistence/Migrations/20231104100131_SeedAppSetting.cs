using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CPG.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class SeedAppSetting : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@" insert into [dbo].[application_settings]([entity_type],[key],[value]) values(1,'api_key','pay__daryaftyar_api')
                                    go
                                    insert into [dbo].[application_settings]([entity_type],[key],[value]) values(1,'api_secret','4lpuezpvrold')
                                    go
                                    insert into [dbo].[application_settings]([entity_type],[key],[value]) values(1,'scopes','profile openid pay__gateway')
                                    go  
                                    insert into [dbo].[application_settings]([entity_type],[key],[value]) values(1,'client_secret','')
                                    go
");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
