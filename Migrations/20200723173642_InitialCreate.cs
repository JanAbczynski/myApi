using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Comander.Migrations
{
    public partial class InitialCreate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Code",
                columns: table => new
                {
                    Idc = table.Column<string>(nullable: false),
                    UserId = table.Column<string>(nullable: true),
                    UserLogin = table.Column<string>(nullable: true),
                    Code = table.Column<string>(nullable: true),
                    CreationTime = table.Column<DateTime>(nullable: false),
                    ExpireTime = table.Column<DateTime>(nullable: false),
                    TypeOfCode = table.Column<string>(nullable: true),
                    AdditionalInfo = table.Column<string>(nullable: true),
                    WasUsed = table.Column<bool>(nullable: false),
                    UsingDate = table.Column<DateTime>(nullable: false),
                    CodeBeneficient = table.Column<string>(nullable: true),
                    IsActive = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Code", x => x.Idc);
                });

            migrationBuilder.CreateTable(
                name: "Commands",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    HowTo = table.Column<string>(maxLength: 250, nullable: false),
                    Line = table.Column<string>(nullable: false),
                    Platform = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Commands", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<string>(nullable: false),
                    UserLogin = table.Column<string>(nullable: true),
                    UserPass = table.Column<string>(nullable: true),
                    UserName = table.Column<string>(nullable: true),
                    UserSureName = table.Column<string>(nullable: true),
                    UserAddress = table.Column<string>(nullable: true),
                    UserCity = table.Column<string>(nullable: true),
                    UserZipCode = table.Column<string>(nullable: true),
                    UserMail = table.Column<string>(nullable: true),
                    UserPhoneNumber = table.Column<string>(nullable: true),
                    UserPhoneNumber2 = table.Column<string>(nullable: true),
                    UserSalt = table.Column<string>(nullable: true),
                    UserRole = table.Column<string>(nullable: true),
                    Confirmed = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Code");

            migrationBuilder.DropTable(
                name: "Commands");

            migrationBuilder.DropTable(
                name: "Users");
        }
    }
}
