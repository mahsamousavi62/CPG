using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CPG.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddApplicationSetting : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"INSERT INTO [dbo].[Application_Settings]
                                         ([Entity_Type],[Key],[Value],[CreationDate],[CreationUserId],[IsActive])
                                  VALUES (7,'Referrer_Page','referrer-cpg',GETDATE(),1,1),
                                         (7,'Callback_Page','callback-cpg',GETDATE(),1,1)");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
