using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PariniFSL.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddActivityFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "Activities");

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                table: "Activities",
                type: "nvarchar(2000)",
                maxLength: 2000,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(2000)",
                oldMaxLength: 2000,
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DocenteReferenteId",
                table: "Activities",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "OreComplessive",
                table: "Activities",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "Periodo",
                table: "Activities",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "SedeAttivita",
                table: "Activities",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "SoggettoOspitante",
                table: "Activities",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Tipologia",
                table: "Activities",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_Activities_DocenteReferenteId",
                table: "Activities",
                column: "DocenteReferenteId");

            migrationBuilder.AddForeignKey(
                name: "FK_Activities_AspNetUsers_DocenteReferenteId",
                table: "Activities",
                column: "DocenteReferenteId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Activities_AspNetUsers_DocenteReferenteId",
                table: "Activities");

            migrationBuilder.DropIndex(
                name: "IX_Activities_DocenteReferenteId",
                table: "Activities");

            migrationBuilder.DropColumn(
                name: "DocenteReferenteId",
                table: "Activities");

            migrationBuilder.DropColumn(
                name: "OreComplessive",
                table: "Activities");

            migrationBuilder.DropColumn(
                name: "Periodo",
                table: "Activities");

            migrationBuilder.DropColumn(
                name: "SedeAttivita",
                table: "Activities");

            migrationBuilder.DropColumn(
                name: "SoggettoOspitante",
                table: "Activities");

            migrationBuilder.DropColumn(
                name: "Tipologia",
                table: "Activities");

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                table: "Activities",
                type: "nvarchar(2000)",
                maxLength: 2000,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(2000)",
                oldMaxLength: 2000);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                table: "Activities",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }
    }
}
