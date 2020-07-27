using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PetStore.Models;

namespace PetStore.Data.Configurations
{
    public class OrderEntityConfiguration : IEntityTypeConfiguration<Order>
    {
        public void Configure(EntityTypeBuilder<Order> builder)
        {
            builder
                .Property(o => o.Town)
                .HasMaxLength(40)
                .IsUnicode(true);

            builder
                .Property(o => o.Address)
                .HasMaxLength(70)
                .IsUnicode(true);

            builder
                .Ignore(o => o.TotalPrice);
        }
    }
}
