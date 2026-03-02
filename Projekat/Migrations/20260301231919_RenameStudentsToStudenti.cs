using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Projekat.Migrations
{
    /// <inheritdoc />
    public partial class RenameStudentsToStudenti : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Ispiti_Students_StudentID",
                table: "Ispiti");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Students",
                table: "Students");

            migrationBuilder.RenameTable(
                name: "Students",
                newName: "Studenti");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Studenti",
                table: "Studenti",
                column: "StudentID");

            migrationBuilder.AddForeignKey(
                name: "FK_Ispiti_Studenti_StudentID",
                table: "Ispiti",
                column: "StudentID",
                principalTable: "Studenti",
                principalColumn: "StudentID",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Ispiti_Studenti_StudentID",
                table: "Ispiti");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Studenti",
                table: "Studenti");

            migrationBuilder.RenameTable(
                name: "Studenti",
                newName: "Students");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Students",
                table: "Students",
                column: "StudentID");

            migrationBuilder.AddForeignKey(
                name: "FK_Ispiti_Students_StudentID",
                table: "Ispiti",
                column: "StudentID",
                principalTable: "Students",
                principalColumn: "StudentID",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
