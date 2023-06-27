<img align="left" width="116" height="116" src="https://raw.githubusercontent.com/kapozade/dotnet-api-template/main/images/bricks.png" />

# dotnet-api-template

[![Build](https://github.com/kapozade/dotnet-api-template/actions/workflows/dotnet.yml/badge.svg?branch=main)](https://github.com/kapozade/dotnet-api-template/actions/workflows/dotnet.yml)
[![CodeQL](https://github.com/kapozade/dotnet-api-template/actions/workflows/codeql.yml/badge.svg?branch=main)](https://github.com/kapozade/dotnet-api-template/actions/workflows/codeql.yml)
[![NuGet Package](https://img.shields.io/nuget/v/Supreme.Dotnet.Api.Template.svg)](https://www.nuget.org/packages/Supreme.Dotnet.Api.Template)
[![Supreme.Dotnet.Api.Template NuGet Package Downloads](https://img.shields.io/nuget/dt/Supreme.Dotnet.Api.Template)](https://www.nuget.org/packages/Supreme.Dotnet.Api.Template)


## Content
1. [Requirements](https://github.com/kapozade/dotnet-api-template#requirements)
2. [How-to](https://github.com/kapozade/dotnet-api-template#how-to)
3. [Template Options](https://github.com/kapozade/dotnet-api-template#template-options)
4. [Version History](https://github.com/kapozade/dotnet-api-template#version-history)
5. [Contributors](https://github.com/kapozade/dotnet-api-template#contributors)

<br/>
The generated project implements Layered Architecture. Internal implementation uses CQRS principle to split query use cases from command use cases. 

```
Supreme.Api: Responsible for showing information to user and interpreting user's commands.
Supreme.Application: Defines the jobs that project is supposed to do and directs expressive domain objects to work.
Supreme.Domain: Responsible for representing concepts of business, information about business situation and business rules.
Supreme.Infrastructure: Provides generic technical capabilities that support higher layers.
```
<i>PS: When you name your project Supreme will be replaced with the name of project.</i>

<br/>

* DomainEvents, IntegrationEvents and outbox messaging can be used so as to have eventual consistency and strong consistency (requires enabled outbox pattern. Check [Template Options](https://github.com/kapozade/dotnet-api-template#template-options)). 

* You can add distributed rate limiting per endpoints. If you enable rate limiting, you can find rate limit types under Supreme.Api/Filters/RateLimits folder. Concurrent, Fixed Window, Sliding Window, Token Bucket rate limits are implemented. Currently, adding requests to queue when limit has been reached not supported.
```C#
[FixedWindowRateLimitFilter(policyKey: "foo", limit: 5, expireInSeconds: 60)]
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
```

* You can enable open telemetry (Check [Template Options](https://github.com/kapozade/dotnet-api-template#template-options)). 

<br/>

## Requirements
The basic template depends on below resources.

* .NET7
* MySQL
* Redis

If you would like to add outbox pattern functionality, you are required to have 

* RabbitMQ

You can find docker images for above resources via [here](https://github.com/kapozade/dockerfiles)

<br/>

## How-to

First, install template
```bash
dotnet new install Supreme.Dotnet.Api.Template
```

Run

```bash
## dotnet new supremeapi [options] [template options]
dotnet new supremeapi -n "MyService" -eop -eot -erl
```

<br/>

## Template Options

| Option | Description |
| ------ | ----------- |
| -f, --framework | Currently net7.0 is supported. The default value is net7.0 |
| -eop, --enable-outbox-pattern | Enables outbox pattern by using [CAP](https://cap.dotnetcore.xyz/) |
| -eot, --enable-open-telemetry | Adds open telemetry configuration with Jaeger support |
| -erl, --enable-rate-limiting | Adds basic rate limiting action filters that uses Redis behind |

<br/>

## Version History
* <b>1.1.0</b>

    <b> 🎉 What is new?</b>

    * Containerization support [#3](https://github.com/kapozade/dotnet-api-template/issues/3)
    * Project README.md file generation improvements.
    
    <br/>

   <b> 🐞 What is fixed?</b>

   * appsettings.json template generation issue fixed. [#4](https://github.com/kapozade/dotnet-api-template/issues/4)
<hr/>
<br/>

## Contributors

* Owner: [@kapozade](https://github.com/kapozade)
