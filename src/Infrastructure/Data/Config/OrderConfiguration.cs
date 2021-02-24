using ApplicationCore.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Data.Config
{
    public class OrderConfiguration : IEntityTypeConfiguration<Order>
    {
        public void Configure(EntityTypeBuilder<Order> builder)
        {
            builder.Property(x => x.FirstName)
                .IsRequired()
                .HasMaxLength(50);

            builder.Property(x => x.LastName)
                .IsRequired()
                .HasMaxLength(50);

            builder.OwnsOne(x => x.ShipToAddress, x =>
            {
                x.WithOwner();

                x.Property(x => x.Street)
                    .IsRequired()
                    .HasMaxLength(180);

                x.Property(x => x.City)
                    .IsRequired()
                    .HasMaxLength(100);

                x.Property(x => x.State)
                    .HasMaxLength(60);

                x.Property(x => x.Country)
                    .IsRequired()
                    .HasMaxLength(90);

                x.Property(x => x.ZipCode)
                    .IsRequired()
                    .HasMaxLength(18);
            });

            builder.HasMany(x => x.OrderItems)
                .WithOne()
                .HasForeignKey(x => x.OrderId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
