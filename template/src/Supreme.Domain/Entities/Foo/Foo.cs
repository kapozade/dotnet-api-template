using Supreme.Domain.SeedWork;

namespace  Supreme.Domain.Entities.Foo;

public class Foo : BaseEntity
{
  public long Id {get; private set;}
  public string Name {get; private set;} = string.Empty;

  private Foo(){}

  public static Foo Create(string name, int executedBy, DateTime executedOn){
    var foo = new Foo {
      Name = name,
      CreatedBy = executedBy,
      ModifiedBy = executedBy,
      CreatedOn = executedOn,
      ModifiedOn = executedOn
    };

    return foo;
  }
}