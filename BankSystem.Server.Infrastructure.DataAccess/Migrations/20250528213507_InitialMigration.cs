using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace BankSystem.Server.Infrastructure.DataAccess.Migrations
{
    // <inheritdoc />
    public partial class InitialMigration : Migration
    {
        // <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "BANK");

            migrationBuilder.CreateTable(
                name: "USER",
                schema: "BANK",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Username = table.Column<string>(type: "text", nullable: false),
                    Email = table.Column<string>(type: "text", nullable: false),
                    Password = table.Column<string>(type: "text", nullable: false),
                    FullName = table.Column<string>(type: "text", nullable: false),
                    Role = table.Column<string>(type: "text", nullable: false),
                    DateOfBirth = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    TwoFAEnabled = table.Column<bool>(type: "boolean", nullable: false),
                    TwoFACode = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_USER", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AUDIT_ERROR",
                schema: "BANK",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    UserId = table.Column<long>(type: "bigint", nullable: true),
                    Action = table.Column<string>(type: "text", nullable: false),
                    Description = table.Column<string>(type: "text", nullable: false),
                    Timestamp = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AUDIT_ERROR", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AUDIT_ERROR_USER_UserId",
                        column: x => x.UserId,
                        principalSchema: "BANK",
                        principalTable: "USER",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "AUDIT_LOGS",
                schema: "BANK",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    UserId = table.Column<long>(type: "bigint", nullable: false),
                    Action = table.Column<string>(type: "text", nullable: false),
                    Timestamp = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Description = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AUDIT_LOGS", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AUDIT_LOGS_USER_UserId",
                        column: x => x.UserId,
                        principalSchema: "BANK",
                        principalTable: "USER",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "BANK_ACCOUNT",
                schema: "BANK",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    UserId = table.Column<long>(type: "bigint", nullable: false),
                    AccountNumber = table.Column<string>(type: "text", nullable: false),
                    AccountType = table.Column<string>(type: "text", nullable: false),
                    Currency = table.Column<string>(type: "text", nullable: false),
                    Balance = table.Column<decimal>(type: "numeric", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BANK_ACCOUNT", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BANK_ACCOUNT_USER_UserId",
                        column: x => x.UserId,
                        principalSchema: "BANK",
                        principalTable: "USER",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "LOGIN_TOKEN",
                schema: "BANK",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    UserId = table.Column<long>(type: "bigint", nullable: false),
                    RefreshToken = table.Column<string>(type: "text", nullable: false),
                    ExpiryDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    RevokedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LOGIN_TOKEN", x => x.Id);
                    table.ForeignKey(
                        name: "FK_LOGIN_TOKEN_USER_UserId",
                        column: x => x.UserId,
                        principalSchema: "BANK",
                        principalTable: "USER",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TRANSACTION",
                schema: "BANK",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    SenderAccountId = table.Column<long>(type: "bigint", nullable: false),
                    ReciverAccountId = table.Column<long>(type: "bigint", nullable: false),
                    Amount = table.Column<decimal>(type: "numeric", nullable: false),
                    TransactionType = table.Column<string>(type: "text", nullable: false),
                    Date = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Details = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TRANSACTION", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TRANSACTION_BANK_ACCOUNT_ReciverAccountId",
                        column: x => x.ReciverAccountId,
                        principalSchema: "BANK",
                        principalTable: "BANK_ACCOUNT",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TRANSACTION_BANK_ACCOUNT_SenderAccountId",
                        column: x => x.SenderAccountId,
                        principalSchema: "BANK",
                        principalTable: "BANK_ACCOUNT",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AUDIT_ERROR_UserId",
                schema: "BANK",
                table: "AUDIT_ERROR",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_AUDIT_LOGS_UserId",
                schema: "BANK",
                table: "AUDIT_LOGS",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_BANK_ACCOUNT_UserId",
                schema: "BANK",
                table: "BANK_ACCOUNT",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_LOGIN_TOKEN_UserId",
                schema: "BANK",
                table: "LOGIN_TOKEN",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_TRANSACTION_ReciverAccountId",
                schema: "BANK",
                table: "TRANSACTION",
                column: "ReciverAccountId");

            migrationBuilder.CreateIndex(
                name: "IX_TRANSACTION_SenderAccountId",
                schema: "BANK",
                table: "TRANSACTION",
                column: "SenderAccountId");
        }

        // <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AUDIT_ERROR",
                schema: "BANK");

            migrationBuilder.DropTable(
                name: "AUDIT_LOGS",
                schema: "BANK");

            migrationBuilder.DropTable(
                name: "LOGIN_TOKEN",
                schema: "BANK");

            migrationBuilder.DropTable(
                name: "TRANSACTION",
                schema: "BANK");

            migrationBuilder.DropTable(
                name: "BANK_ACCOUNT",
                schema: "BANK");

            migrationBuilder.DropTable(
                name: "USER",
                schema: "BANK");
        }
    }
}
