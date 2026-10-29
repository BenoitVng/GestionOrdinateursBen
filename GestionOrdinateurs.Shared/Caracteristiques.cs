
namespace GestionOrdinateurs.Shared;

public class Caracteristiques
{
    public string Marque { get; set; } = string.Empty;
    public int RamGo { get; set; }
    public string Processeur { get; set; } = string.Empty;
    public int StockageGo { get; set; }
    public TypeOrdinateur TypeOrdi { get; set; }

    public Caracteristiques() { }

    public Caracteristiques(
        string marque,
        int ramGo,
        string processeur,
        int stockageGo,
        TypeOrdinateur typeOrdi)
    {
        Marque = marque;
        RamGo = ramGo;
        Processeur = processeur;
        StockageGo = stockageGo;
        TypeOrdi = typeOrdi;
    }
}
