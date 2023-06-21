# dotnet-api-template

## Requirements
The basic template depends on the below resources.

* MySQL
* Redis

If you would like to add outbox pattern functionality, you are required to have 

* RabbitMQ

You can find the docker images for the above resources via [here](https://github.com/kapozade/dockerfiles)

## How-to

To install the template you have two options 


The first option is cloning the source code and then run the statements below.

```bash
> cd template
> dotnet new install .
```

The second option is installing the template and run
```bash
> dotnet new install Supreme.Dotnet.Api.Template
```

To run
```bash
> ## dotnet new supremeapi [options] [template options]
> dotnet new supremeapi -o "MyService" -eop -eot -erl
```

PS: If you choose the option 1, then before running, you might consider to change the directory.

## Template Options
Options are stated below.

| Option | Description |
| ------ | ----------- |
| -f, --framework | Currently net7.0 is supported. The default value is net7.0 |
| -eop, --enable-outbox-pattern | Enables the outbox pattern by using [CAP](https://cap.dotnetcore.xyz/) |
| -eot, --enable-open-telemetry | Adds open telemetry configuration with Jaeger support |
| -erl, --enable-rate-limiting | Adds basic rate limiting action filters that uses Redis behind |
