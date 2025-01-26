using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CPG.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddIranKishIpgTypeDate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"INSERT INTO [dbo].[IPGType]
           ([PersianName],[EnglishName],[Logo],[IsActive],[CreationDate],[CreationUserId],[Code])
        VALUES
           (N'ایران کیش','Iran_Kish','/IPGType/Logo/Iran Kish.svg',1,GETDATE(),1,6)");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
