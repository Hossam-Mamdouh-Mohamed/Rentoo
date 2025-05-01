using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Rentoo.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class last : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_RateCodes_AspNetUsers_UserId",
                table: "RateCodes");

            migrationBuilder.DropForeignKey(
                name: "FK_RequestReviews_Requests_RequestId",
                table: "RequestReviews");

            migrationBuilder.DropForeignKey(
                name: "FK_UserDocuments_AspNetUsers_UserId",
                table: "UserDocuments");

            migrationBuilder.DropIndex(
                name: "IX_RequestReviews_RequestId",
                table: "RequestReviews");

            migrationBuilder.AlterColumn<string>(
                name: "UserId",
                table: "UserDocuments",
                type: "nvarchar(450)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AlterColumn<int>(
                name: "CarId",
                table: "Requests",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<int>(
                name: "RequestId",
                table: "RequestReviews",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<string>(
                name: "UserId",
                table: "RateCodes",
                type: "nvarchar(450)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AlterColumn<int>(
                name: "CarDocumentId",
                table: "Cars",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<string>(
                name: "LicenseNumber",
                table: "CarDocuments",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.CreateIndex(
                name: "IX_RequestReviews_RequestId",
                table: "RequestReviews",
                column: "RequestId",
                unique: true,
                filter: "[RequestId] IS NOT NULL");

            migrationBuilder.AddForeignKey(
                name: "FK_RateCodes_AspNetUsers_UserId",
                table: "RateCodes",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_RequestReviews_Requests_RequestId",
                table: "RequestReviews",
                column: "RequestId",
                principalTable: "Requests",
                principalColumn: "ID");

            migrationBuilder.AddForeignKey(
                name: "FK_UserDocuments_AspNetUsers_UserId",
                table: "UserDocuments",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_RateCodes_AspNetUsers_UserId",
                table: "RateCodes");

            migrationBuilder.DropForeignKey(
                name: "FK_RequestReviews_Requests_RequestId",
                table: "RequestReviews");

            migrationBuilder.DropForeignKey(
                name: "FK_UserDocuments_AspNetUsers_UserId",
                table: "UserDocuments");

            migrationBuilder.DropIndex(
                name: "IX_RequestReviews_RequestId",
                table: "RequestReviews");

            migrationBuilder.AlterColumn<string>(
                name: "UserId",
                table: "UserDocuments",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(450)",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "CarId",
                table: "Requests",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "RequestId",
                table: "RequestReviews",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "UserId",
                table: "RateCodes",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(450)",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "CarDocumentId",
                table: "Cars",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "LicenseNumber",
                table: "CarDocuments",
                type: "int",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.CreateIndex(
                name: "IX_RequestReviews_RequestId",
                table: "RequestReviews",
                column: "RequestId",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_RateCodes_AspNetUsers_UserId",
                table: "RateCodes",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_RequestReviews_Requests_RequestId",
                table: "RequestReviews",
                column: "RequestId",
                principalTable: "Requests",
                principalColumn: "ID",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_UserDocuments_AspNetUsers_UserId",
                table: "UserDocuments",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
