using Supreme.Domain.Entities.Foo;

namespace Supreme.Infrastructure.Db.Repositories;

public sealed class FooRepository : BaseRepository<Foo>, IFooRepository
{
    public FooRepository(SupremeContext dbContext) : base(dbContext)
  {
  }
}