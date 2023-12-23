# Supreme

## Requirements

* .NET7
* Redis
* MySQL or Postgres (depending on your choice)
* Docker

## How-to

If you decide to generate database by DDLs manually, you can skip Step 1 and Step 2.


### Step 1: Building the project

```bash
dotnet build Supreme.sln -c Release
```

### Step 2: Run tests
Tests contains both unit and integration. Integration tests are being handled by Testcontainers by default. Currently, below containers are registered as default.

* MySQL or Postgres (depending on your db choice)
* RabbitMQ (depending on your outbox pattern choice)

To execute tests run below command.

```bash
dotnet test
```

### Step 3: Executing EFCore Migrations & Database Update

Executing migrations and db update requires a working database. You can create a db instance by executing below command.

```bash
docker compose up -d database
````

Note: Before running below commands, be aware that those commands require `dotnet-ef` that should have already been installed globally.

To install globally run this command => ```dotnet tool install --global dotnet-ef``` </i>


Change directory to Supreme.Infrastructure project folder. You should wait a few seconds until the database is up & running before running commands below. Having up & running database, you can run below migration generation and db update statements on your CLI.


```bash
cd src/Supreme.Infrastructure

dotnet ef migrations add InitialCreate -s "../Supreme.Api/" -o "Db/Migrations/"

dotnet ef database update -s "../Supreme.Api"
```

### Step 4: Building docker image

Go back to the solution folder before running below command.

```bash
docker build \
  --rm=false --file "./container/Dockerfile.app" -t app/supreme .
```

### Step 5: Run dependencies and application

Run below statements one by one.
Before running the app, make sure that all dependencies are up & running.

```bash
docker compose up -d rabbitmq # If you didn't enable outbox pattern, you can skip this command.
docker compose up -d jaeger # If you didn't enable open telemetry, you can skip this command.
docker compose up -d database # Skip this if you already run this for Step #2.
docker compose up -d redis
docker compose up -d app
```
