using Supreme.Api.Models.Foo.Requests;
using Supreme.Api.Models.Foo.Responses;
using Supreme.Api.Models.Common;
using Supreme.Api.Mappers.Foo;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Supreme.Api.Controllers;

[ApiController]
[Route("api/v1/foo")]
[ProducesResponseType(typeof(ErrorResponseModel), StatusCodes.Status400BadRequest)]
public class FoosController : ControllerBase
{
    private readonly IMediator _mediator;
    
    public FoosController(
        IMediator mediator
        )
    {
        _mediator = mediator;
    }

    [HttpGet("{id}", Name = "GetIndividualFoo")]
    [ProducesResponseType(typeof(GetFooResponse), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAsync([FromRoute] long id, 
        CancellationToken cancellationToken)
    {
        var query = FooMapper.MapFrom(id);
        var result = await _mediator.Send(query, cancellationToken);
        var response = FooMapper.MapFrom(result);

        return StatusCode(StatusCodes.Status200OK, response);
    }

    [HttpPost]
    [ProducesResponseType(typeof(CreateFooResponse), StatusCodes.Status201Created)]
    public async Task<IActionResult> PostAsync([FromBody] CreateFooRequest request, 
        [FromHeader]int userId, CancellationToken cancellationToken)
    {
        var command = FooMapper.MapFrom(request, userId);
        var result = await _mediator.Send(command, cancellationToken);
        var response = FooMapper.MapFrom(result);

        return CreatedAtRoute("GetIndividualFoo", new {id = response.Id}, response);
    }
}
