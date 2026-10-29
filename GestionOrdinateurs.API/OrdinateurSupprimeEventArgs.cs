using GestionOrdinateurs.Shared;

namespace GestionOrdinateurs.Controllers;

public class OrdinateurSupprimeEventArgs : EventArgs
{
    public Ordinateur Ordinateur { get; }

    public OrdinateurSupprimeEventArgs(Ordinateur ordinateur)
    {
        Ordinateur = ordinateur;
    }
}