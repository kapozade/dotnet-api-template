# Supreme

## Requirements

* <b>.NET7</b>
* <b>Docker</b>

<br/>

## How-to

If you decide to generate database by DDLs manually, you can skip Step 1 and Step 2.

<br/>

<b>Step 1: Building the project</b>

```bash
dotnet build Supreme.sln -c Release
```
<br/>

<b>Step 2: Executing EFCore Migrations & Database Update</b>

Executing migrations and db update requires a working database. You can create a db instance by executing below command.

<br/>

```bash
docker compose up -d mysql
````

<i>Note: Before running below commands, be aware that those commands require `dotnet-ef` that should have already been installed globally. <br/>

To install globally run this command => ```dotnet tool install --global dotnet-ef``` </i>

<br/>

Change directory to Supreme.Infrastructure project folder. You should wait a few seconds until the database is up & running before running commands below. Having up & running database, you can run below migration generation and db update statements on your CLI.


```bash
cd src/Supreme.Infrastructure

dotnet ef migrations add InitialCreate -s "../Supreme.Api/" -o "Db/Migrations/"

dotnet ef database update -s "../Supreme.Api"
```

<br/>
<b>Step 3: Building docker image</b>

<br/>
<b>Go back to the solution folder before running below command.</b>

<br/>

```bash
docker build \
  --rm=false --file "./container/Dockerfile.app" -t app/supreme .
```

<br/>
<b>Step 4: Run dependencies and application</b>

<br/>
Run below statements one by one.
Before running the app, make sure that all dependencies are up & running.

```bash
docker compose up -d rabbitmq # If you didn't enable outbox pattern, you can skip this command.
docker compose up -d jaeger # If you didn't enable open telemetry, you can skip this command.
docker compose up -d mysql # Skip this if you already run this for Step #2.
docker compose up -d redis
docker compose up -d app
```