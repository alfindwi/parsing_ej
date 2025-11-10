using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace parsing_jrn_ej.Migrations
{
    /// <inheritdoc />
    public partial class CreateAtmTransaksi : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "atm_transaksi",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    jenis_file = table.Column<string>(type: "text", nullable: true),
                    no_transaksi = table.Column<int>(type: "integer", nullable: true),
                    waktu = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    no_kartu = table.Column<string>(type: "text", nullable: true),
                    jenis_transaksi = table.Column<string>(type: "text", nullable: true),
                    terminal_id = table.Column<string>(type: "text", nullable: true),
                    atm_id = table.Column<string>(type: "text", nullable: true),
                    lokasi = table.Column<string>(type: "text", nullable: true),
                    op_code = table.Column<string>(type: "text", nullable: true),
                    jumlah = table.Column<decimal>(type: "numeric", nullable: true),
                    saldo = table.Column<decimal>(type: "numeric", nullable: true),
                    struk = table.Column<string>(type: "text", nullable: true),
                    no_ref = table.Column<string>(type: "text", nullable: true),
                    no_rekening = table.Column<string>(type: "text", nullable: true),
                    pesan_error = table.Column<string>(type: "text", nullable: true),
                    dibuat_pada = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
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
