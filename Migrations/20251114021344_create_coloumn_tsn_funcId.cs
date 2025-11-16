using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace parsing_jrn_ej.Migrations
{
    /// <inheritdoc />
    public partial class create_coloumn_tsn_funcId : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "dibuat_pada",
                table: "atm_transaksi",
                newName: "CreatedAt");

            migrationBuilder.AddColumn<string>(
                name: "function_identifier",
                table: "atm_transaksi",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "trans_seq_number",
                table: "atm_transaksi",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "tsi",
                table: "atm_transaksi",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "tvr",
                table: "atm_transaksi",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "function_identifier",
                table: "atm_transaksi");

            migrationBuilder.DropColumn(
                name: "trans_seq_number",
                table: "atm_transaksi");

            migrationBuilder.DropColumn(
                name: "tsi",
                table: "atm_transaksi");

            migrationBuilder.DropColumn(
                name: "tvr",
                table: "atm_transaksi");

            migrationBuilder.RenameColumn(
                name: "CreatedAt",
                table: "atm_transaksi",
                newName: "dibuat_pada");
        }
    }
}
