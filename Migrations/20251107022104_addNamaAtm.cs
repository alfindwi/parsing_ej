using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace parsing_jrn_ej.Migrations
{
    /// <inheritdoc />
    public partial class addNamaAtm : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "nama_atm",
                table: "atm_transaksi",
                type: "text",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "nama_atm",
                table: "atm_transaksi");
        }
    }
}
