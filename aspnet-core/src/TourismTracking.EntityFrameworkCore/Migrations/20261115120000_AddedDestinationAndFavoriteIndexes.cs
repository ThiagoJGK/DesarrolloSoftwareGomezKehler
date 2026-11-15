using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TourismTracking.Migrations
{
    /// <inheritdoc />
    public partial class AddedDestinationAndFavoriteIndexes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "AppDestinations",
                type: "nvarchar(256)",
                maxLength: 256,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "Country",
                table: "AppDestinations",
                type: "nvarchar(128)",
                maxLength: 128,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.CreateIndex(
                name: "IX_AppDestinations_Name_Country",
                table: "AppDestinations",
                columns: new[] { "Name", "Country" });

            migrationBuilder.CreateIndex(
                name: "IX_AppFavorites_UserId_DestinationId",
                table: "AppFavorites",
                columns: new[] { "UserId", "DestinationId" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_AppDestinations_Name_Country",
                table: "AppDestinations");

            migrationBuilder.DropIndex(
                name: "IX_AppFavorites_UserId_DestinationId",
                table: "AppFavorites");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "AppDestinations",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(256)",
                oldMaxLength: 256);

            migrationBuilder.AlterColumn<string>(
                name: "Country",
                table: "AppDestinations",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(128)",
                oldMaxLength: 128);
        }
    }
}
