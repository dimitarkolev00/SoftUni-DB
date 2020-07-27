using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PetStore.Models;

namespace PetStore.Data.Configurations
{
    public class ClientEntityConfiguration : IEntityTypeConfiguration<Client>
    {
        public void Configure(EntityTypeBuilder<Client> builder)
        {
            builder
                .Property(c => c.Username)
                .HasMaxLength(30)
                .IsUnicode(false);

            builder
                .Property(c => c.Email)
                .HasMaxLength(50)
                .IsUnicode(false);

            builder
                .Property(c => c.FirstName)
                .HasMaxLength(50)
                .IsUnicode(true);

            builder
                .Property(c => c.LastName)
                .HasMaxLength(50)
                .IsUnicode(true);

        }
    }
}
