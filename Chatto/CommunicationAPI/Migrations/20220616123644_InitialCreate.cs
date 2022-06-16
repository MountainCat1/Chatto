using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CommunicationAPI.Migrations
{
    public partial class InitialCreate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Chats_Users_UserGuid",
                table: "Chats");

            migrationBuilder.DropIndex(
                name: "IX_Chats_UserGuid",
                table: "Chats");

            migrationBuilder.DropColumn(
                name: "UserGuid",
                table: "Chats");

            migrationBuilder.CreateTable(
                name: "ChatUser",
                columns: table => new
                {
                    ChatsGuid = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UsersGuid = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ChatUser", x => new { x.ChatsGuid, x.UsersGuid });
                    table.ForeignKey(
                        name: "FK_ChatUser_Chats_ChatsGuid",
                        column: x => x.ChatsGuid,
                        principalTable: "Chats",
                        principalColumn: "Guid",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ChatUser_Users_UsersGuid",
                        column: x => x.UsersGuid,
                        principalTable: "Users",
                        principalColumn: "Guid",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ChatUser_UsersGuid",
                table: "ChatUser",
                column: "UsersGuid");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ChatUser");

            migrationBuilder.AddColumn<Guid>(
                name: "UserGuid",
                table: "Chats",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Chats_UserGuid",
                table: "Chats",
                column: "UserGuid");

            migrationBuilder.AddForeignKey(
                name: "FK_Chats_Users_UserGuid",
                table: "Chats",
                column: "UserGuid",
                principalTable: "Users",
                principalColumn: "Guid");
        }
    }
}
