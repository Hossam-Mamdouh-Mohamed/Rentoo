using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Rentoo.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class CarDetails : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_RateCodes_AspNetUsers_UserId",
                table: "RateCodes");

            migrationBuilder.DropIndex(
                name: "IX_RateCodeDays_RateCodeId_DayId",
                table: "RateCodeDays");

            migrationBuilder.AlterColumn<string>(
                name: "UserId",
                table: "RateCodes",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(450)",
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_RateCodeDays_RateCodeId",
                table: "RateCodeDays",
                column: "RateCodeId");

            migrationBuilder.AddForeignKey(
                name: "FK_RateCodes_AspNetUsers_UserId",
                table: "RateCodes",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_RateCodes_AspNetUsers_UserId",
                table: "RateCodes");

            migrationBuilder.DropIndex(
                name: "IX_RateCodeDays_RateCodeId",
                table: "RateCodeDays");

            migrationBuilder.AlterColumn<string>(
                name: "UserId",
                table: "RateCodes",
                type: "nvarchar(450)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.CreateIndex(
                name: "IX_RateCodeDays_RateCodeId_DayId",
                table: "RateCodeDays",
                columns: new[] { "RateCodeId", "DayId" });

            migrationBuilder.AddForeignKey(
                name: "FK_RateCodes_AspNetUsers_UserId",
                table: "RateCodes",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");
        }
    }
}
