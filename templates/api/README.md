# Supreme

## How to run

If you choose to generate database by DDLs manually, you can skip Step 1 and Step 2.
<br/>
<br/>

<b>Step 1: Building the project</b>

```bash
dotnet build Supreme.sln -c Release
```
<br/>
<b>Step 2: Executing EFCore migrations</b>

This process requires a database up and running and db schema.

<i>Note: Before running the below commands, be aware that those commands require dotnet-ef that should have already been installed globally. <br/>

To install run the command => ```dotnet tool install --global dotnet-ef``` </i>


```bash
docker compose up -d mysql
````

<br/>

Change directory to Infrastructure project. And then run migration generation and db update.

```bash
cd src/Supreme.Infrastructure
dotnet ef migrations add InitialCreate -s "../Supreme.Api/" -o "Db/Migrations/"
dotnet ef database update -s "../Supreme.Api"
```

<br/>
<b>Step 3: Building docker image</b>

<br/>
Go back to the solution folder

```bash
cd ../../
docker build \
  --rm=false --file "./container/Dockerfile.app" -t app/supreme .
```

<br/>
<b>Step 4: Run dependencies and application</b>

<br/>
Run dependencies individually.

```bash
docker compose up -d rabbitmq # If you didn't enable outbox pattern, you can skip this command.
docker compose up -d jaeger # If you didn't enable open telemetry, you can skip this command.
docker compose up -d mysql # Skip this if you already run this for Step #2.
docker compose up -d redis
docker compose up -d app
```