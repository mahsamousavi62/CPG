using CPG.Domain.SeedWork;
using CPG.Infrastructure.Persistence.DbContexts.ReadModels;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CPG.Infrastructure.Persistence.DbContexts.EntityConfigurations.ReadModelsConfiguration
{
    public class UserReadModelConfiguration : IEntityTypeConfiguration<UserReadModel>
    {
        public void Configure(EntityTypeBuilder<UserReadModel> readModel)
        {
            readModel.ToTable("User");
            readModel.HasKey(x => x.Id);


            readModel.Property(x => x.Id)
                .HasColumnName("Id");

            readModel.Property(x => x.IDPId)
              .HasColumnName("IDPId")
            .IsRequired();

            readModel.Property(x => x.NationalCode)
                     .HasColumnName("NationalCode").HasColumnType("char(10)")
            .IsRequired();

            readModel.Property(x => x.PhoneNumber)
                      .HasColumnName("PhoneNumber").HasColumnType("char(11)").IsRequired();

            readModel.Property(x => x.FirstName)
                .HasColumnName("FirstName")
                .IsRequired();

            readModel.Property(x => x.LastName)
                .HasColumnName("LastName")
                .IsRequired();

            readModel.Property(x => x.IsActive)
                  .HasColumnName("IsActive");

            readModel.Property(x => x.IsLegal)
              .HasColumnName("IsLegal");

            readModel.Property(x => x.CompanyId)
               .HasColumnName("CompanyId");

            readModel.Property(x => x.KYCStatus)
               .HasColumnName("KYCStatus");

          
        }
    }
}
