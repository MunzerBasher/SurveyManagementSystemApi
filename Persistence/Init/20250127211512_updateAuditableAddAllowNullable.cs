using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SurveyManagementSystemApi.Persistence.Init
{
    /// <inheritdoc />
    public partial class updateAuditableAddAllowNullable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "6dc6528a-b280-4770-9eae-82671ee81ef7",
                column: "PasswordHash",
                value: "AQAAAAIAAYagAAAAEEgacuHRPrKFpUllx84M+gLOLw3gV1sou6NvsFBAxNpNDRwNe/VYZT7NSBENT5Bx3w==");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "6dc6528a-b280-4770-9eae-82671ee81ef7",
                column: "PasswordHash",
                value: "AQAAAAIAAYagAAAAEO1HauPg04m1xIYi1d23e0T9C3g51f8mDIfmp2eOtiGtS7w9o0xoFvfC8oEv4MXv2g==");
        }
    }
}
