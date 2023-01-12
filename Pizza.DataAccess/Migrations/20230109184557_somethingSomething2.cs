using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Pizza.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class somethingSomething2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "WorkerCode",
                table: "AspNetUsers",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "WorkerCode",
                table: "AspNetUsers");
        }
    }
}
