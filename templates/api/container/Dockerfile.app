FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build-env

COPY /container /container
COPY /nuget.config .
COPY /Directory.Build.targets .
COPY /Supreme.sln .
COPY /test /test
COPY /src /src
COPY /.editorconfig /.editorconfig

RUN dotnet build ./Supreme.sln -c Release

WORKDIR /src/Supreme.Api

RUN dotnet publish -c Release --no-build --no-restore -v m -o '../../artifacts'

FROM mcr.microsoft.com/dotnet/aspnet:7.0

RUN sed -i 's/MinProtocol = TLSv1.2/MinProtocol = TLSv1/' /etc/ssl/openssl.cnf \
    && sed -i 's/CipherString = DEFAULT@SECLEVEL=2/CipherString = DEFAULT@SECLEVEL=1/' /etc/ssl/openssl.cnf

WORKDIR /app

COPY --from=build-env /artifacts .

RUN apt-get update && \
    apt-get install -y curl && \
    apt-get install gettext-base && \
    rm -rf /var/lib/apt/lists/*

ENV ASPNETCORE_ENVIRONMENT=staging
ENV DB_SERVER=mysql
ENV DB_NAME=Supreme
ENV DB_USER=sqlsa
ENV DB_PASSWORD=SuperPass1
ENV DB_CONNECTION_TIMEOUT=15
ENV RABBITMQ_HOST=rabbitmq
ENV RABBITMQ_PORT=5672
ENV RABBITMQ_VHOST=/
ENV RABBITMQ_USERNAME=guest
ENV RABBITMQ_PASSWORD=guest
ENV REDIS_HOST=redis
ENV REDIS_PORT=6379
ENV REDIS_DBID=1
ENV REDIS_USER=kapozade
ENV REDIS_PASSWORD=7b0evpiTayUA9kw0VYLRyWzc0mxPChtU
ENV JAEGER_HOST=http://localhost:4317
ENV ASPNETCORE_URLS=http://*:5000

RUN envsubst < /app/appsettings.json.tmpl > /app/appsettings.json

EXPOSE 5000

ENTRYPOINT ["dotnet", "Supreme.Api.dll"]