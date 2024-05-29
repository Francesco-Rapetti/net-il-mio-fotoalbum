using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FotoAlbum.Migrations
{
    /// <inheritdoc />
    public partial class MessageMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CategoryPhoto_Categories_categoriesId",
                table: "CategoryPhoto");

            migrationBuilder.DropPrimaryKey(
                name: "PK_CategoryPhoto",
                table: "CategoryPhoto");

            migrationBuilder.DropIndex(
                name: "IX_CategoryPhoto_categoriesId",
                table: "CategoryPhoto");

            migrationBuilder.RenameColumn(
                name: "categoriesId",
                table: "CategoryPhoto",
                newName: "CategoriesId");

            migrationBuilder.AlterColumn<string>(
                name: "UserId",
                table: "Photos",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Photos",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                table: "Photos",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_CategoryPhoto",
                table: "CategoryPhoto",
                columns: new[] { "CategoriesId", "PhotosId" });

            migrationBuilder.CreateTable(
                name: "Messages",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Text = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    UserId = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Messages", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Photos_UserId",
                table: "Photos",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_CategoryPhoto_PhotosId",
                table: "CategoryPhoto",
                column: "PhotosId");

            migrationBuilder.AddForeignKey(
                name: "FK_CategoryPhoto_Categories_CategoriesId",
                table: "CategoryPhoto",
                column: "CategoriesId",
                principalTable: "Categories",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Photos_AspNetUsers_UserId",
                table: "Photos",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CategoryPhoto_Categories_CategoriesId",
                table: "CategoryPhoto");

            migrationBuilder.DropForeignKey(
                name: "FK_Photos_AspNetUsers_UserId",
                table: "Photos");

            migrationBuilder.DropTable(
                name: "Messages");

            migrationBuilder.DropIndex(
                name: "IX_Photos_UserId",
                table: "Photos");

            migrationBuilder.DropPrimaryKey(
                name: "PK_CategoryPhoto",
                table: "CategoryPhoto");

            migrationBuilder.DropIndex(
                name: "IX_CategoryPhoto_PhotosId",
                table: "CategoryPhoto");

            migrationBuilder.RenameColumn(
                name: "CategoriesId",
                table: "CategoryPhoto",
                newName: "categoriesId");

            migrationBuilder.AlterColumn<string>(
                name: "UserId",
                table: "Photos",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Photos",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50);

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                table: "Photos",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(500)",
                oldMaxLength: 500);

            migrationBuilder.AddPrimaryKey(
                name: "PK_CategoryPhoto",
                table: "CategoryPhoto",
                columns: new[] { "PhotosId", "categoriesId" });

            migrationBuilder.CreateIndex(
                name: "IX_CategoryPhoto_categoriesId",
                table: "CategoryPhoto",
                column: "categoriesId");

            migrationBuilder.AddForeignKey(
                name: "FK_CategoryPhoto_Categories_categoriesId",
                table: "CategoryPhoto",
                column: "categoriesId",
                principalTable: "Categories",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
