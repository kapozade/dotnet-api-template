# dotnet-api-template

## Content
1. [Requirements](https://github.com/kapozade/dotnet-api-template#requirements)
2. [How-to](https://github.com/kapozade/dotnet-api-template#how-to)
3. [Template Options](https://github.com/kapozade/dotnet-api-template#template-options)
4. [Version History](https://github.com/kapozade/dotnet-api-template#version-history)
5. [Contributors](https://github.com/kapozade/dotnet-api-template#contributors)

<br/>

## Requirements
The basic template depends on the below resources.

* MySQL
* Redis

If you would like to add outbox pattern functionality, you are required to have 

* RabbitMQ

You can find the docker images for the above resources via [here](https://github.com/kapozade/dockerfiles)

<br/>

## How-to

First, install the template
```bash
> dotnet new install Supreme.Dotnet.Api.Template
```

Run
```bash
> ## dotnet new supremeapi [options] [template options]
> dotnet new supremeapi -n "MyService" -eop -eot -erl
```

<br/>

## Template Options
Options are stated below.

| Option | Description |
| ------ | ----------- |
| -f, --framework | Currently net7.0 is supported. The default value is net7.0 |
| -eop, --enable-outbox-pattern | Enables the outbox pattern by using [CAP](https://cap.dotnetcore.xyz/) |
| -eot, --enable-open-telemetry | Adds open telemetry configuration with Jaeger support |
| -erl, --enable-rate-limiting | Adds basic rate limiting action filters that uses Redis behind |

<br/>

## Version History
<br/>

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