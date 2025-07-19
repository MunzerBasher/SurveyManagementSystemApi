using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SurveyManagementSystemApi.Persistence.Init
{
    /// <inheritdoc />
    public partial class add_IsDisabledtoUser : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsDisabled",
                table: "AspNetUsers",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "6dc6528a-b280-4770-9eae-82671ee81ef7",
                columns: new[] { "IsDisabled", "PasswordHash" },
                values: new object[] { false, "AQAAAAIAAYagAAAAEDIPeC3RRh4ss9WPrah18eBtui4kChHtqmasXGRjRhdr/4dZr+QFZeNrNj87mLxQEg==" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsDisabled",
                table: "AspNetUsers");

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "6dc6528a-b280-4770-9eae-82671ee81ef7",
                column: "PasswordHash",
                value: "AQAAAAIAAYagAAAAEEgacuHRPrKFpUllx84M+gLOLw3gV1sou6NvsFBAxNpNDRwNe/VYZT7NSBENT5Bx3w==");
        }
    }
}
