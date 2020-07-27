using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PetStore.Models;

namespace PetStore.Data.Configurations
{
    public class BreedEntityConfiguration : IEntityTypeConfiguration<Breed>
    {
        public void Configure(EntityTypeBuilder<Breed> builder)
        {
            builder
                .Property(b => b.Name)
                .HasMaxLength(30)
                .IsUnicode(true);
        }
    }
}
