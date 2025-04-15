using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BankSystem.Server.Infrastructure.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class InitialMigrationV2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AUDIT_LOGS_Users_UserId",
                schema: "BANK",
                table: "AUDIT_LOGS");

            migrationBuilder.DropForeignKey(
                name: "FK_BANK_ACCOUNT_Users_UserId",
                schema: "BANK",
                table: "BANK_ACCOUNT");

            migrationBuilder.DropForeignKey(
                name: "FK_LOGIN_TOKEN_Users_UserId",
                schema: "BANK",
                table: "LOGIN_TOKEN");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Users",
                schema: "BANK",
                table: "Users");

            migrationBuilder.RenameTable(
                name: "Users",
                schema: "BANK",
                newName: "USER",
                newSchema: "BANK");

            migrationBuilder.AddPrimaryKey(
                name: "PK_USER",
                schema: "BANK",
                table: "USER",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_AUDIT_LOGS_USER_UserId",
                schema: "BANK",
                table: "AUDIT_LOGS",
                column: "UserId",
                principalSchema: "BANK",
                principalTable: "USER",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_BANK_ACCOUNT_USER_UserId",
                schema: "BANK",
                table: "BANK_ACCOUNT",
                column: "UserId",
                principalSchema: "BANK",
                principalTable: "USER",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_LOGIN_TOKEN_USER_UserId",
                schema: "BANK",
                table: "LOGIN_TOKEN",
                column: "UserId",
                principalSchema: "BANK",
                principalTable: "USER",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AUDIT_LOGS_USER_UserId",
                schema: "BANK",
                table: "AUDIT_LOGS");

            migrationBuilder.DropForeignKey(
                name: "FK_BANK_ACCOUNT_USER_UserId",
                schema: "BANK",
                table: "BANK_ACCOUNT");

            migrationBuilder.DropForeignKey(
                name: "FK_LOGIN_TOKEN_USER_UserId",
                schema: "BANK",
                table: "LOGIN_TOKEN");

            migrationBuilder.DropPrimaryKey(
                name: "PK_USER",
                schema: "BANK",
                table: "USER");

            migrationBuilder.RenameTable(
                name: "USER",
                schema: "BANK",
                newName: "Users",
                newSchema: "BANK");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Users",
                schema: "BANK",
                table: "Users",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_AUDIT_LOGS_Users_UserId",
                schema: "BANK",
                table: "AUDIT_LOGS",
                column: "UserId",
                principalSchema: "BANK",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_BANK_ACCOUNT_Users_UserId",
                schema: "BANK",
                table: "BANK_ACCOUNT",
                column: "UserId",
                principalSchema: "BANK",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_LOGIN_TOKEN_Users_UserId",
                schema: "BANK",
                table: "LOGIN_TOKEN",
                column: "UserId",
                principalSchema: "BANK",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
