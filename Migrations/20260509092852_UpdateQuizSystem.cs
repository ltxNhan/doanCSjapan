using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace JapanApp.Migrations
{
    /// <inheritdoc />
    public partial class UpdateQuizSystem : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_QuizAnswers_Seasons_SeasonID",
                table: "QuizAnswers");

            migrationBuilder.DropIndex(
                name: "IX_QuizAnswers_SeasonID",
                table: "QuizAnswers");

            migrationBuilder.DropColumn(
                name: "SeasonID",
                table: "QuizAnswers");

            migrationBuilder.RenameColumn(
                name: "SuggestSeasonID",
                table: "QuizAnswers",
                newName: "Points");

            migrationBuilder.CreateTable(
                name: "QuizResults",
                columns: table => new
                {
                    QuizResultID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserID = table.Column<int>(type: "int", nullable: false),
                    TotalScore = table.Column<int>(type: "int", nullable: false),
                    SuggestedSeasonID = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_QuizResults", x => x.QuizResultID);
                    table.ForeignKey(
                        name: "FK_QuizResults_Seasons_SuggestedSeasonID",
                        column: x => x.SuggestedSeasonID,
                        principalTable: "Seasons",
                        principalColumn: "SeasonID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_QuizResults_Users_UserID",
                        column: x => x.UserID,
                        principalTable: "Users",
                        principalColumn: "UserID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_QuizResults_SuggestedSeasonID",
                table: "QuizResults",
                column: "SuggestedSeasonID");

            migrationBuilder.CreateIndex(
                name: "IX_QuizResults_UserID",
                table: "QuizResults",
                column: "UserID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "QuizResults");

            migrationBuilder.RenameColumn(
                name: "Points",
                table: "QuizAnswers",
                newName: "SuggestSeasonID");

            migrationBuilder.AddColumn<int>(
                name: "SeasonID",
                table: "QuizAnswers",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_QuizAnswers_SeasonID",
                table: "QuizAnswers",
                column: "SeasonID");

            migrationBuilder.AddForeignKey(
                name: "FK_QuizAnswers_Seasons_SeasonID",
                table: "QuizAnswers",
                column: "SeasonID",
                principalTable: "Seasons",
                principalColumn: "SeasonID",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
