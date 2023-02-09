using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CityInfoAPI.Migrations
{
    /// <inheritdoc />
    public partial class dataseedupdate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Cities",
                columns: new[] { "Id", "Description", "Name" },
                values: new object[] { 4, "Oldest town in Kenya", "Kisumu" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Cities",
                keyColumn: "Id",
                keyValue: 4);
        }
    }
}
