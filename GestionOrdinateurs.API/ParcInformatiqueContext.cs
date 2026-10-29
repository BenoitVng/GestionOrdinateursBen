using GestionOrdinateurs.Shared;
using Microsoft.EntityFrameworkCore;

public class ParcInformatiqueContext : DbContext
{
    // liste des ordinateurs
    public DbSet<Ordinateur> Ordinateurs { get; set; }
    public DbSet<Reparation> Reparations { get; set; }
    public DbSet<User> Users { get; set; }
     public ParcInformatiqueContext(
        DbContextOptions<ParcInformatiqueContext> options) : base(options)
    {
    }
    // règle la base de données
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder){
        if (!optionsBuilder.IsConfigured) {
            optionsBuilder.UseSqlite("Data Source=parcinfo.db");
        } // chemin de la base
    }
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.Entity<Ordinateur>()
            .HasKey(ordinateur => ordinateur.Id);

        modelBuilder.Entity<Ordinateur>()
            .OwnsOne(ordinateur => ordinateur.Caracteristiques);

        modelBuilder.Entity<Ordinateur>()
            .HasMany(ordinateur => ordinateur.Reparations)
            .WithOne(reparation => reparation.UnOrdinateur);
    }
}