
namespace GestionOrdinateurs.Services;
public class JournalService
{
    private static readonly object _lock = new object();

    public void Ecrire(string message, string dossier = "Reparations")
    {
        string fichier;
        if (dossier == "Reparations")
        {
            fichier = "reparations.log";
        }
        else
        {
            fichier = "ordinateurs.log";
        }
        string cheminDossier = Path.Combine("Logs", dossier);
        Directory.CreateDirectory(cheminDossier);
        string chemin = Path.Combine(cheminDossier, fichier);
        lock (_lock)
        {
            using var writer = new StreamWriter(chemin, append: true);
            writer.WriteLine(message);
        }
    }
}
