using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Supreme.Api.Tests;
using Supreme.Infrastructure.Db;
using Xunit;

namespace Supreme.Application.Tests;

public abstract class BaseTestFixture : IClassFixture<WebAppFactory>
{
    protected readonly IMediator _mediator;
    protected readonly SupremeContext _dbContext;
    
    protected BaseTestFixture(WebAppFactory webAppFactory)
    {
        var scope = webAppFactory.Services.CreateScope();
        _mediator = scope.ServiceProvider.GetRequiredService<IMediator>();
        _dbContext = scope.ServiceProvider.GetRequiredService<SupremeContext>();
    }
}
