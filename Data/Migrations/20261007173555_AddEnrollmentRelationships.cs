using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PariniFSL.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddEnrollmentRelationships : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Activities_AspNetUsers_DocenteReferenteId",
                table: "Activities");

            migrationBuilder.RenameColumn(
                name: "CourseId",
                table: "Enrollments",
                newName: "ActivityId");

            migrationBuilder.AlterColumn<string>(
                name: "StudentId",
                table: "Enrollments",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AddColumn<string>(
                name: "EnrolledById",
                table: "Enrollments",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_Enrollments_ActivityId",
                table: "Enrollments",
                column: "ActivityId");

            migrationBuilder.CreateIndex(
                name: "IX_Enrollments_EnrolledById",
                table: "Enrollments",
                column: "EnrolledById");

            migrationBuilder.CreateIndex(
                name: "IX_Enrollments_StudentId_ActivityId",
                table: "Enrollments",
                columns: new[] { "StudentId", "ActivityId" },
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Activities_AspNetUsers_DocenteReferenteId",
                table: "Activities",
                column: "DocenteReferenteId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Enrollments_Activities_ActivityId",
                table: "Enrollments",
                column: "ActivityId",
                principalTable: "Activities",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Enrollments_AspNetUsers_EnrolledById",
                table: "Enrollments",
                column: "EnrolledById",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Enrollments_AspNetUsers_StudentId",
                table: "Enrollments",
                column: "StudentId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Activities_AspNetUsers_DocenteReferenteId",
                table: "Activities");

            migrationBuilder.DropForeignKey(
                name: "FK_Enrollments_Activities_ActivityId",
                table: "Enrollments");

            migrationBuilder.DropForeignKey(
                name: "FK_Enrollments_AspNetUsers_EnrolledById",
                table: "Enrollments");

            migrationBuilder.DropForeignKey(
                name: "FK_Enrollments_AspNetUsers_StudentId",
                table: "Enrollments");

            migrationBuilder.DropIndex(
                name: "IX_Enrollments_ActivityId",
                table: "Enrollments");

            migrationBuilder.DropIndex(
                name: "IX_Enrollments_EnrolledById",
                table: "Enrollments");

            migrationBuilder.DropIndex(
                name: "IX_Enrollments_StudentId_ActivityId",
                table: "Enrollments");

            migrationBuilder.DropColumn(
                name: "EnrolledById",
                table: "Enrollments");

            migrationBuilder.RenameColumn(
                name: "ActivityId",
                table: "Enrollments",
                newName: "CourseId");

            migrationBuilder.AlterColumn<string>(
                name: "StudentId",
                table: "Enrollments",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AddForeignKey(
                name: "FK_Activities_AspNetUsers_DocenteReferenteId",
                table: "Activities",
                column: "DocenteReferenteId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
