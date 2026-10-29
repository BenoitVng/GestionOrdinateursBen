using GestionOrdinateurs.Shared;

namespace GestionOrdinateurs.Controllers;

public class OrdinateurMisAJourEventArgs : EventArgs
{
    public Ordinateur Ordinateur { get; }

    public OrdinateurMisAJourEventArgs(Ordinateur ordinateur)
    {
        Ordinateur = ordinateur;
    }
}