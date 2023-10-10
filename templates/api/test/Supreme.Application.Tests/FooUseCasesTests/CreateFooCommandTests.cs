using Microsoft.EntityFrameworkCore;
using Supreme.Api.Tests;
using Supreme.Application.FooUseCases.Commands;
using Supreme.Domain.Constants;
using Supreme.Domain.Entities.Foo;
using Supreme.Domain.Exceptions;
using Xunit;

namespace Supreme.Application.Tests.FooUseCasesTests;

public sealed class CreateFooCommandTests : BaseTestFixture
{
    public CreateFooCommandTests(WebAppFactory webAppFactory) : base(webAppFactory)
    {
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    public async Task ShouldThrowValidationException_WhenNameIsNullOrEmpty(string name)
    {
        var command = new CreateFooCommand(name, 1);
        var validationException = await Assert.ThrowsAsync<ValidationException>(() => _mediator.Send(command, CancellationToken.None));
        Assert.Equal(ValidationMessageKeys.FooMessages._nameRequired, validationException.ValidationErrors[0].Message);
    }

    [Fact]
    public async Task ShouldThrowValidationException_WhenNameLengthIsLessThan5()
    {
        var command = new CreateFooCommand("1234", 1);
        var validationException = await Assert.ThrowsAsync<ValidationException>(() => _mediator.Send(command, CancellationToken.None));
        Assert.Equal(ValidationMessageKeys.FooMessages._nameLengthShouldBeGreaterThanOrEqualTo, validationException.ValidationErrors[0].Message);
        Assert.Equal(FieldConstants.FooFields._nameMinLength, validationException.ValidationErrors[0].Data);
    }

    [Fact]
    public async Task ShouldThrowValidationException_WhenNameLengthIsMoreThan255()
    {
        var command = new CreateFooCommand("1234123412341234123412341234123412341234123412341234123412341234123412341234123412341234123412341234123412341234123412341234123412341234123412341234123412341234123412341234123412341234123412341234123412341234123412341234123412341234123412341234123412341234", 1);
        var validationException = await Assert.ThrowsAsync<ValidationException>(() => _mediator.Send(command, CancellationToken.None));
        Assert.Equal(ValidationMessageKeys.FooMessages._nameLengthShouldBeLessThanOrEqualTo, validationException.ValidationErrors[0].Message);
        Assert.Equal(FieldConstants.FooFields._nameMaxLength, validationException.ValidationErrors[0].Data);
    }

    [Fact]
    public async Task ShouldThrowValidationException_WhenExecutedByIsLessThan1()
    {
        var command = new CreateFooCommand("12345", 0);
        var validationException = await Assert.ThrowsAsync<ValidationException>(() => _mediator.Send(command, CancellationToken.None));
        Assert.Equal(ValidationMessageKeys._userIdShouldBeGreaterThan, validationException.ValidationErrors[0].Message);
        Assert.Equal(FieldConstants._userIdMinValue, validationException.ValidationErrors[0].Data);
    }

    [Fact]
    public async Task ShouldCreateNewFoo_WhenCalled()
    {
        var command = new CreateFooCommand("This is a test", 1);
        var result = await _mediator.Send(command, CancellationToken.None);
        var createdFoo = await _dbContext.Set<Foo>().FirstOrDefaultAsync(x => x.Id == result.Id);
        
        Assert.NotNull(createdFoo);
    }
}
