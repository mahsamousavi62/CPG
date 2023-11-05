using CPG.Domain.SharedKernel.ApplicationSettings;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CPG.Infrastructure.Persistence.DbContexts.EntityConfigurations
{
    public class ApplicationSettingsConfiguration : IEntityTypeConfiguration<ApplicationSettings>
    {
        public void Configure(EntityTypeBuilder<ApplicationSettings> entity)
        {
            entity.ToTable("Application_Settings");
            entity.HasKey(x => x.Id);

            entity
               .Property(e => e.Id)
               .HasColumnName("Id")
               .UseIdentityColumn();

            entity
               .Property(e => e.EntityType)
               .HasColumnName("Entity_Type")
               .IsRequired();

            entity
                   .Property(e => e.Key)
                   .HasColumnName("Key")
                   .IsRequired();

            entity
                .Property(e => e.Value)
                .HasColumnName("Value")
                .IsRequired();
        }
    }
}
