﻿using System;
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
        public GrandHotelDbContext(DbContextOptions<GrandHotelDbContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Adresse>(entity =>
            {
                entity.HasKey(e => e.IdClient);

                entity.Property(e => e.IdClient).ValueGeneratedNever();

                entity.Property(e => e.CodePostal)
                    .IsRequired()
                    .HasMaxLength(5)
                    .IsUnicode(false);

                entity.Property(e => e.Complement).HasMaxLength(40);

                entity.Property(e => e.Rue)
                    .IsRequired()
                    .HasMaxLength(40);

                entity.Property(e => e.Ville)
                    .IsRequired()
                    .HasMaxLength(40);

                entity.HasOne(d => d.IdClientNavigation)
                    .WithOne(p => p.Adresse)
                    .HasForeignKey<Adresse>(d => d.IdClient)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Adresse_Client");
            });

            modelBuilder.Entity<Calendrier>(entity =>
            {
                entity.HasKey(e => e.Jour);

                entity.Property(e => e.Jour).HasColumnType("date");
            });

            modelBuilder.Entity<Chambre>(entity =>
            {
                entity.HasKey(e => e.Numero);

                entity.Property(e => e.Numero).ValueGeneratedNever();

                entity.Property(e => e.Douche).HasDefaultValueSql("((1))");

                entity.Property(e => e.Wc)
                    .HasColumnName("WC")
                    .HasDefaultValueSql("((1))");
            });

            modelBuilder.Entity<Client>(entity =>
            {
                entity.Property(e => e.Civilite)
                    .IsRequired()
                    .HasMaxLength(4)
                    .IsUnicode(false);

                entity.Property(e => e.Email)
                    .IsRequired()
                    .HasMaxLength(40);

                entity.Property(e => e.Nom)
                    .IsRequired()
                    .HasMaxLength(40);

                entity.Property(e => e.Prenom)
                    .IsRequired()
                    .HasMaxLength(40);

                entity.Property(e => e.Societe).HasMaxLength(100);
            });

            modelBuilder.Entity<Facture>(entity =>
            {
                entity.Property(e => e.CodeModePaiement)
                    .IsRequired()
                    .HasMaxLength(3)
                    .IsUnicode(false);

                entity.Property(e => e.DateFacture).HasColumnType("date");

                entity.Property(e => e.DatePaiement).HasColumnType("date");

                entity.HasOne(d => d.CodeModePaiementNavigation)
                    .WithMany(p => p.Facture)
                    .HasForeignKey(d => d.CodeModePaiement)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Facture_Paiement");

                entity.HasOne(d => d.IdClientNavigation)
                    .WithMany(p => p.Facture)
                    .HasForeignKey(d => d.IdClient)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Facture_Client");
            });

            modelBuilder.Entity<LigneFacture>(entity =>
            {
                entity.HasKey(e => new { e.IdFacture, e.NumLigne });

                entity.Property(e => e.MontantHt)
                    .HasColumnName("MontantHT")
                    .HasColumnType("decimal(12, 3)");

                entity.Property(e => e.Quantite).HasDefaultValueSql("((1))");

                entity.Property(e => e.TauxReduction)
                    .HasColumnType("decimal(6, 3)")
                    .HasDefaultValueSql("((0))");

                entity.Property(e => e.TauxTva)
                    .HasColumnName("TauxTVA")
                    .HasColumnType("decimal(6, 3)");

                entity.HasOne(d => d.IdFactureNavigation)
                    .WithMany(p => p.LigneFacture)
                    .HasForeignKey(d => d.IdFacture)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_LigneFacture_Facture");
            });

            modelBuilder.Entity<ModePaiement>(entity =>
            {
                entity.HasKey(e => e.Code);

                entity.Property(e => e.Code)
                    .HasMaxLength(3)
                    .IsUnicode(false)
                    .ValueGeneratedNever();

                entity.Property(e => e.Libelle)
                    .IsRequired()
                    .HasMaxLength(40);
            });

            modelBuilder.Entity<Reservation>(entity =>
            {
                entity.HasKey(e => new { e.NumChambre, e.Jour });

                entity.HasIndex(e => e.IdClient)
                    .HasName("IDX_ReservationClient_FK");

                entity.Property(e => e.Jour).HasColumnType("date");

                entity.Property(e => e.HeureArrivee).HasDefaultValueSql("((17))");

                entity.HasOne(d => d.IdClientNavigation)
                    .WithMany(p => p.Reservation)
                    .HasForeignKey(d => d.IdClient)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Reservation_Client");

                entity.HasOne(d => d.JourNavigation)
                    .WithMany(p => p.Reservation)
                    .HasForeignKey(d => d.Jour)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Reservation_Calendrier");

                entity.HasOne(d => d.NumChambreNavigation)
                    .WithMany(p => p.Reservation)
                    .HasForeignKey(d => d.NumChambre)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Reservation_Chambre");
            });

            modelBuilder.Entity<Tarif>(entity =>
            {
                entity.HasKey(e => e.Code);

                entity.Property(e => e.Code)
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .ValueGeneratedNever();

                entity.Property(e => e.DateDebut).HasColumnType("date");

                entity.Property(e => e.Prix).HasColumnType("decimal(12, 3)");
            });

            modelBuilder.Entity<TarifChambre>(entity =>
            {
                entity.HasKey(e => new { e.NumChambre, e.CodeTarif });

                entity.Property(e => e.CodeTarif)
                    .HasMaxLength(20)
                    .IsUnicode(false);

                entity.HasOne(d => d.CodeTarifNavigation)
                    .WithMany(p => p.TarifChambre)
                    .HasForeignKey(d => d.CodeTarif)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_TarifChambre_Tarif");

                entity.HasOne(d => d.NumChambreNavigation)
                    .WithMany(p => p.TarifChambre)
                    .HasForeignKey(d => d.NumChambre)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_TarifChambre_Chambre");
            });

            modelBuilder.Entity<Telephone>(entity =>
            {
                entity.HasKey(e => e.Numero);

                entity.Property(e => e.Numero)
                    .HasMaxLength(12)
                    .IsUnicode(false)
                    .ValueGeneratedNever();

                entity.Property(e => e.CodeType)
                    .IsRequired()
                    .HasColumnType("char(1)");

                entity.HasOne(d => d.IdClientNavigation)
                    .WithMany(p => p.Telephone)
                    .HasForeignKey(d => d.IdClient)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Telephone_Client");
            });
            // Customize the ASP.NET Identity model and override the defaults if needed.
            // For example, you can rename the ASP.NET Identity table names and more.
            // Add your customizations after calling base.OnModelCreating(builder);
        }

        public DbSet<GrandHotel_PF.Models.Reservation> Reservation { get; set; }
    }
}
