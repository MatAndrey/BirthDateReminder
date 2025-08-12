using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BirthDateReminder.Server.Migrations
{
    /// <inheritdoc />
    public partial class AddReminderTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "NotifyDayBefore",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "NotifyInBD",
                table: "AspNetUsers");

            migrationBuilder.CreateTable(
                name: "Reminders",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    UnitsType = table.Column<int>(type: "INTEGER", nullable: false),
                    UnitsCount = table.Column<int>(type: "INTEGER", nullable: false),
                    BirthdayId = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Reminders", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Reminders_BirthdayItems_BirthdayId",
                        column: x => x.BirthdayId,
                        principalTable: "BirthdayItems",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Reminders_BirthdayId",
                table: "Reminders",
                column: "BirthdayId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Reminders");

            migrationBuilder.AddColumn<bool>(
                name: "NotifyDayBefore",
                table: "AspNetUsers",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "NotifyInBD",
                table: "AspNetUsers",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);
        }
    }
}
