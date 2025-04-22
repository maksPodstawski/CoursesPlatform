using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DAL.Migrations
{
    /// <inheritdoc />
    public partial class modelChange : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CourseSubcategory_Courses_CourseId",
                table: "CourseSubcategory");

            migrationBuilder.DropForeignKey(
                name: "FK_CourseSubcategory_Subcategories_SubcategoryId",
                table: "CourseSubcategory");

            migrationBuilder.DropPrimaryKey(
                name: "PK_CourseSubcategory",
                table: "CourseSubcategory");

            migrationBuilder.RenameTable(
                name: "CourseSubcategory",
                newName: "CourseSubcategories");

            migrationBuilder.RenameIndex(
                name: "IX_CourseSubcategory_SubcategoryId",
                table: "CourseSubcategories",
                newName: "IX_CourseSubcategories_SubcategoryId");

            migrationBuilder.RenameIndex(
                name: "IX_CourseSubcategory_CourseId",
                table: "CourseSubcategories",
                newName: "IX_CourseSubcategories_CourseId");

            migrationBuilder.AddColumn<string>(
                name: "VideoPath",
                table: "Stages",
                type: "text",
                nullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_CourseSubcategories",
                table: "CourseSubcategories",
                column: "Id");

            migrationBuilder.CreateTable(
                name: "Creators",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    CourseId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Creators", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Creators_Courses_CourseId",
                        column: x => x.CourseId,
                        principalTable: "Courses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Creators_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Creators_CourseId",
                table: "Creators",
                column: "CourseId");

            migrationBuilder.CreateIndex(
                name: "IX_Creators_UserId",
                table: "Creators",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_CourseSubcategories_Courses_CourseId",
                table: "CourseSubcategories",
                column: "CourseId",
                principalTable: "Courses",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_CourseSubcategories_Subcategories_SubcategoryId",
                table: "CourseSubcategories",
                column: "SubcategoryId",
                principalTable: "Subcategories",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CourseSubcategories_Courses_CourseId",
                table: "CourseSubcategories");

            migrationBuilder.DropForeignKey(
                name: "FK_CourseSubcategories_Subcategories_SubcategoryId",
                table: "CourseSubcategories");

            migrationBuilder.DropTable(
                name: "Creators");

            migrationBuilder.DropPrimaryKey(
                name: "PK_CourseSubcategories",
                table: "CourseSubcategories");

            migrationBuilder.DropColumn(
                name: "VideoPath",
                table: "Stages");

            migrationBuilder.RenameTable(
                name: "CourseSubcategories",
                newName: "CourseSubcategory");

            migrationBuilder.RenameIndex(
                name: "IX_CourseSubcategories_SubcategoryId",
                table: "CourseSubcategory",
                newName: "IX_CourseSubcategory_SubcategoryId");

            migrationBuilder.RenameIndex(
                name: "IX_CourseSubcategories_CourseId",
                table: "CourseSubcategory",
                newName: "IX_CourseSubcategory_CourseId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_CourseSubcategory",
                table: "CourseSubcategory",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_CourseSubcategory_Courses_CourseId",
                table: "CourseSubcategory",
                column: "CourseId",
                principalTable: "Courses",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_CourseSubcategory_Subcategories_SubcategoryId",
                table: "CourseSubcategory",
                column: "SubcategoryId",
                principalTable: "Subcategories",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
