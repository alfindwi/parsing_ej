using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace parsing_jrn_ej.Migrations
{
    /// <inheritdoc />
    public partial class create_atm_transaksi : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "atm_transaksi",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    jenis_file = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    no_transaksi = table.Column<int>(type: "int", nullable: true),
                    waktu = table.Column<DateTime>(type: "datetime", nullable: true),
                    no_kartu = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    jenis_transaksi = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    terminal_id = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    nama_atm = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    atm_id = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    lokasi = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    op_code = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    jumlah = table.Column<decimal>(type: "numeric(18,0)", nullable: true),
                    saldo = table.Column<decimal>(type: "numeric(18,0)", nullable: true),
                    struk = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    no_ref = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    no_rekening = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    pesan_error = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    dibuat_pada = table.Column<DateTime>(type: "datetime", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_atm_transaksi", x => x.id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "atm_transaksi");
        }
    }
}
