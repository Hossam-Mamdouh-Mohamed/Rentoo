using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Rentoo.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class testoo : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Cars_RateCodes_RateCodeId",
                table: "Cars");

            migrationBuilder.DropIndex(
                name: "IX_RateCodeDays_RateCodeId_Day",
                table: "RateCodeDays");

            migrationBuilder.DropColumn(
                name: "Price",
                table: "RateCodes");

            migrationBuilder.DropColumn(
                name: "Day",
                table: "RateCodeDays");

            migrationBuilder.AddColumn<string>(
                name: "UserId",
                table: "RateCodes",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "DayId",
                table: "RateCodeDays",
                type: "int",
                maxLength: 20,
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<float>(
                name: "Price",
                table: "RateCodeDays",
                type: "real",
                nullable: false,
                defaultValue: 0f);

            migrationBuilder.AlterColumn<int>(
                name: "RateCodeId",
                table: "Cars",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.CreateIndex(
                name: "IX_RateCodes_UserId",
                table: "RateCodes",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_RateCodeDays_RateCodeId_DayId",
                table: "RateCodeDays",
                columns: new[] { "RateCodeId", "DayId" });

            migrationBuilder.AddForeignKey(
                name: "FK_Cars_RateCodes_RateCodeId",
                table: "Cars",
                column: "RateCodeId",
                principalTable: "RateCodes",
                principalColumn: "ID");

            migrationBuilder.AddForeignKey(
                name: "FK_RateCodes_AspNetUsers_UserId",
                table: "RateCodes",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Cars_RateCodes_RateCodeId",
                table: "Cars");

            migrationBuilder.DropForeignKey(
                name: "FK_RateCodes_AspNetUsers_UserId",
                table: "RateCodes");

            migrationBuilder.DropIndex(
                name: "IX_RateCodes_UserId",
                table: "RateCodes");

            migrationBuilder.DropIndex(
                name: "IX_RateCodeDays_RateCodeId_DayId",
                table: "RateCodeDays");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "RateCodes");

            migrationBuilder.DropColumn(
                name: "DayId",
                table: "RateCodeDays");

            migrationBuilder.DropColumn(
                name: "Price",
                table: "RateCodeDays");

            migrationBuilder.AddColumn<float>(
                name: "Price",
                table: "RateCodes",
                type: "real",
                nullable: false,
                defaultValue: 0f);

            migrationBuilder.AddColumn<string>(
                name: "Day",
                table: "RateCodeDays",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AlterColumn<int>(
                name: "RateCodeId",
                table: "Cars",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_RateCodeDays_RateCodeId_Day",
                table: "RateCodeDays",
                columns: new[] { "RateCodeId", "Day" });

            migrationBuilder.AddForeignKey(
                name: "FK_Cars_RateCodes_RateCodeId",
                table: "Cars",
                column: "RateCodeId",
                principalTable: "RateCodes",
                principalColumn: "ID",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
