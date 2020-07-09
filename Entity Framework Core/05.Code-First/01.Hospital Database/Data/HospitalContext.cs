using Microsoft.EntityFrameworkCore;
using P01_HospitalDatabase.Data.Models;

namespace P01_HospitalDatabase.Data
{
    public class HospitalContext : DbContext
    {
        public HospitalContext()
        {

        }

        public HospitalContext(DbContextOptions options)
        : base(options)
        {

        }

        public DbSet<Diagnose> Diagnoses { get; set; }
        public DbSet<Medicament> Medicaments { get; set; }
        public DbSet<Patient> Patients { get; set; }
        public DbSet<PatientMedicament> Prescriptions { get; set; }
        public DbSet<Visitation> Visitations { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlServer(Config.Connection.ConnectionString);
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Diagnose>(entity =>
            {
                entity
                    .Property(e => e.Name)
                    .IsUnicode(true);
                entity
                    .Property(e => e.Comments)
                    .IsUnicode(true);
            });

            modelBuilder.Entity<Medicament>(entity =>
            {
                entity
                    .Property(e => e.Name)
                    .IsUnicode(true);

            });

            modelBuilder.Entity<Patient>(entity =>
            {
                entity
                    .Property(e => e.FirstName)
                    .IsUnicode(true);
                entity
                    .Property(e => e.LastName)
                    .IsUnicode(true);

                entity
                    .Property(e => e.Address)
                    .IsUnicode(true);
                entity
                    .Property(e => e.Email)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<Visitation>(entity =>
            {
                entity
                    .Property(e => e.Comments)
                    .IsUnicode(true);
            });

            modelBuilder.Entity<PatientMedicament>(entity =>
            {
                entity.HasKey(e => new
                {
                    e.PatientId,
                    e.MedicamentId
                });
            });
        }
    }
}
