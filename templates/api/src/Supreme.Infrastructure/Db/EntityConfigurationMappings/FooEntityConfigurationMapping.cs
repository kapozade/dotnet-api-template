using Supreme.Domain.Entities.Foo;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Supreme.Infrastructure.Db.EntityConfigurationMappings;

public class FooEntityConfigurationMapping : IEntityTypeConfiguration<Foo>
{
  public void Configure(EntityTypeBuilder<Foo> builder)
  {
      builder.ToTable("Foo");
      builder.HasKey(i => i.Id);
      builder.Property(i => i.Id).HasColumnName("Id");
      builder.Property(i => i.Name).HasColumnName("Name");
      builder.Property(i => i.CreatedBy).HasColumnName("CreatedBy");
      builder.Property(i => i.ModifiedBy).HasColumnName("ModifiedBy");
      builder.Property(i => i.CreatedOn).HasColumnName("CreatedOn");
      builder.Property(i => i.ModifiedOn).HasColumnName("ModifiedOn");
  }
}