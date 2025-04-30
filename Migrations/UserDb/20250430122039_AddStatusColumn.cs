using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AuctionApp.Migrations.UserDb
{
    /// <inheritdoc />
    public partial class AddStatusColumn : Migration
    {
		/// <inheritdoc />
		protected override void Up(MigrationBuilder migrationBuilder)
		{
			migrationBuilder.AddColumn<string>(
				name: "Status",
				table: "AspNetUsers",
				type: "nvarchar(max)",
				nullable: false,
				defaultValue: "");
		}

		protected override void Down(MigrationBuilder migrationBuilder)
		{
			migrationBuilder.DropColumn(
				name: "Status",
				table: "AspNetUsers");
		}

	}
}
