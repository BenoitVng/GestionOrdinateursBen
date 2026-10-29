using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GestionOrdinateurs.API.Migrations
{
    /// <inheritdoc />
    public partial class upgradeStructure : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Processeur",
                table: "Ordinateurs",
                newName: "Caracteristiques_Processeur");

            migrationBuilder.RenameColumn(
                name: "Marque",
                table: "Ordinateurs",
                newName: "Caracteristiques_Marque");

            migrationBuilder.RenameColumn(
                name: "RAM",
                table: "Ordinateurs",
                newName: "Etat");

            migrationBuilder.RenameColumn(
                name: "EtatOrdinateur",
                table: "Ordinateurs",
                newName: "Caracteristiques_TypeOrdi");

            migrationBuilder.AddColumn<int>(
                name: "Caracteristiques_RamGo",
                table: "Ordinateurs",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Caracteristiques_StockageGo",
                table: "Ordinateurs",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Caracteristiques_RamGo",
                table: "Ordinateurs");

            migrationBuilder.DropColumn(
                name: "Caracteristiques_StockageGo",
                table: "Ordinateurs");

            migrationBuilder.RenameColumn(
                name: "Caracteristiques_Processeur",
                table: "Ordinateurs",
                newName: "Processeur");

            migrationBuilder.RenameColumn(
                name: "Caracteristiques_Marque",
                table: "Ordinateurs",
                newName: "Marque");

            migrationBuilder.RenameColumn(
                name: "Etat",
                table: "Ordinateurs",
                newName: "RAM");

            migrationBuilder.RenameColumn(
                name: "Caracteristiques_TypeOrdi",
                table: "Ordinateurs",
                newName: "EtatOrdinateur");
        }
    }
}
