using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ErSoftDev.Identity.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class changeTableName : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Logins_Users_UserId",
                schema: "dbo",
                table: "Logins");

            migrationBuilder.DropForeignKey(
                name: "FK_RefreshTokens_Users_UserId",
                schema: "dbo",
                table: "RefreshTokens");

            migrationBuilder.DropPrimaryKey(
                name: "PK_RefreshTokens",
                schema: "dbo",
                table: "RefreshTokens");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Logins",
                schema: "dbo",
                table: "Logins");

            migrationBuilder.RenameTable(
                name: "RefreshTokens",
                schema: "dbo",
                newName: "UserRefreshTokens",
                newSchema: "dbo");

            migrationBuilder.RenameTable(
                name: "Logins",
                schema: "dbo",
                newName: "UserLogins",
                newSchema: "dbo");

            migrationBuilder.RenameIndex(
                name: "IX_RefreshTokens_UserId",
                schema: "dbo",
                table: "UserRefreshTokens",
                newName: "IX_UserRefreshTokens_UserId");

            migrationBuilder.RenameIndex(
                name: "IX_Logins_UserId",
                schema: "dbo",
                table: "UserLogins",
                newName: "IX_UserLogins_UserId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_UserRefreshTokens",
                schema: "dbo",
                table: "UserRefreshTokens",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_UserLogins",
                schema: "dbo",
                table: "UserLogins",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_UserLogins_Users_UserId",
                schema: "dbo",
                table: "UserLogins",
                column: "UserId",
                principalSchema: "dbo",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_UserRefreshTokens_Users_UserId",
                schema: "dbo",
                table: "UserRefreshTokens",
                column: "UserId",
                principalSchema: "dbo",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UserLogins_Users_UserId",
                schema: "dbo",
                table: "UserLogins");

            migrationBuilder.DropForeignKey(
                name: "FK_UserRefreshTokens_Users_UserId",
                schema: "dbo",
                table: "UserRefreshTokens");

            migrationBuilder.DropPrimaryKey(
                name: "PK_UserRefreshTokens",
                schema: "dbo",
                table: "UserRefreshTokens");

            migrationBuilder.DropPrimaryKey(
                name: "PK_UserLogins",
                schema: "dbo",
                table: "UserLogins");

            migrationBuilder.RenameTable(
                name: "UserRefreshTokens",
                schema: "dbo",
                newName: "RefreshTokens",
                newSchema: "dbo");

            migrationBuilder.RenameTable(
                name: "UserLogins",
                schema: "dbo",
                newName: "Logins",
                newSchema: "dbo");

            migrationBuilder.RenameIndex(
                name: "IX_UserRefreshTokens_UserId",
                schema: "dbo",
                table: "RefreshTokens",
                newName: "IX_RefreshTokens_UserId");

            migrationBuilder.RenameIndex(
                name: "IX_UserLogins_UserId",
                schema: "dbo",
                table: "Logins",
                newName: "IX_Logins_UserId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_RefreshTokens",
                schema: "dbo",
                table: "RefreshTokens",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Logins",
                schema: "dbo",
                table: "Logins",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Logins_Users_UserId",
                schema: "dbo",
                table: "Logins",
                column: "UserId",
                principalSchema: "dbo",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_RefreshTokens_Users_UserId",
                schema: "dbo",
                table: "RefreshTokens",
                column: "UserId",
                principalSchema: "dbo",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
