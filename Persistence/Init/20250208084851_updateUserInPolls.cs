using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SurveyManagementSystemApi.Persistence.Init
{
    /// <inheritdoc />
    public partial class updateUserInPolls : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Votes_AspNetUsers_UserIdentitiyId",
                table: "Votes");

            migrationBuilder.DropIndex(
                name: "IX_Votes_UserIdentitiyId",
                table: "Votes");

            migrationBuilder.DropColumn(
                name: "UserIdentitiyId",
                table: "Votes");

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "6dc6528a-b280-4770-9eae-82671ee81ef7",
                column: "PasswordHash",
                value: "AQAAAAIAAYagAAAAEIbRdBJYIsToJbhukcdDTvg8bZyNV2ZJCJ3NSnRW0jonBMrpSnSoaMAX8gZmEViSaQ==");

            migrationBuilder.AddForeignKey(
                name: "FK_Votes_AspNetUsers_UserId",
                table: "Votes",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Votes_AspNetUsers_UserId",
                table: "Votes");

            migrationBuilder.AddColumn<string>(
                name: "UserIdentitiyId",
                table: "Votes",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "6dc6528a-b280-4770-9eae-82671ee81ef7",
                column: "PasswordHash",
                value: "AQAAAAIAAYagAAAAEJVVgPbcVdUWz75QBvSF1zt4r/mMJkgP8ZkUNOV33JMCtVf4oE6MoydnCmD/Cg3Z2A==");

            migrationBuilder.CreateIndex(
                name: "IX_Votes_UserIdentitiyId",
                table: "Votes",
                column: "UserIdentitiyId");

            migrationBuilder.AddForeignKey(
                name: "FK_Votes_AspNetUsers_UserIdentitiyId",
                table: "Votes",
                column: "UserIdentitiyId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
