using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Rentoo.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class inial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "UserImage",
                table: "AspNetUsers",
                newName: "userimage");

            migrationBuilder.AddColumn<string>(
                name: "UserImage",
                table: "AspNetUsers",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "UserImage",
                table: "AspNetUsers");

            migrationBuilder.RenameColumn(
                name: "userimage",
                table: "AspNetUsers",
                newName: "UserImage");
        }
    }
}
