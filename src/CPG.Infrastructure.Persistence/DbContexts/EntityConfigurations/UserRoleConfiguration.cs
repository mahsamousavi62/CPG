using CPG.Domain.AggregateModels.UserAggregate;
using CPG.Domain.SeedWork;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CPG.Infrastructure.Persistence.DbContexts.EntityConfigurations
{
    public class UserRoleConfiguration : IEntityTypeConfiguration<UserRole>
    {
        public void Configure(EntityTypeBuilder<UserRole> entity)
        {
            entity.ToTable("UserRole");
            entity.HasKey(x => x.Id);

            entity.Property(x => x.Id)
             .HasColumnName("Id")
            .UseIdentityColumn();

            entity.Property(x => x.UserId)
                .HasColumnName("UserId");
            
            
            entity.Property(x => x.CreationDate)
              .HasColumnName("CreationDate");

            entity.Property(x => x.ModificationDate)
          .HasColumnName("ModificationDate");
        }
    }
}
