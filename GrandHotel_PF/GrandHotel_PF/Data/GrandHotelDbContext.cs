    using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using GrandHotel_PF.Models;

namespace GrandHotel_PF.Data
{
    public class GrandHotelDbContext : IdentityDbContext<ApplicationUser>
    {
        public virtual DbSet<Adresse> Adresse { get; set; }
        public virtual DbSet<Calendrier> Calendrier { get; set; }
        public virtual DbSet<Chambre> Chambre { get; set; }
        public virtual DbSet<Client> Client { get; set; }
        public virtual DbSet<Facture> Facture { get; set; }
        public virtual DbSet<LigneFacture> LigneFacture { get; set; }
        public virtual DbSet<ModePaiement> ModePaiement { get; set; }
        public virtual DbSet<Reservation> Reservation { get; set; }
        public virtual DbSet<Tarif> Tarif { get; set; }
        public virtual DbSet<TarifChambre> TarifChambre { get; set; }
        public virtual DbSet<Telephone> Telephone { get; set; }

        public GrandHotelDbContext(DbContextOptions<GrandHotelDbContext> options) : base(options) { }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. See http://go.microsoft.com/fwlink/?LinkId=723263 for guidance on storing connection strings.
                optionsBuilder.UseSqlServer(@"Server=(localdb)\mssqllocaldb;Database=GrandHotel;Trusted_Connection=True;");
            }
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            // Customize the ASP.NET Identity model and override the defaults if needed.
            // For example, you can rename the ASP.NET Identity table names and more.
            // Add your customizations after calling base.OnModelCreating(builder);
        }
    }
}
