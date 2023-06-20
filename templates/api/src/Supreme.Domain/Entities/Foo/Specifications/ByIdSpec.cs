using Ardalis.Specification;

namespace Supreme.Domain.Entities.Foo.Specifications;

public sealed class ByIdSpec : Specification<Foo>, ISingleResultSpecification<Foo>
{
    public ByIdSpec(long id, bool trackEntity)
    {
        Query.Where(x => x.Id == id)
            .AsNoTracking(trackEntity);
    }
}
