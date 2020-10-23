FROM mcr.microsoft.com/dotnet/core/sdk:3.1-alpine AS build
LABEL stage=build
WORKDIR /src
COPY ./DotInitializr ./DotInitializr

# Need to publish with version-specific RID. See https://github.com/libgit2/libgit2sharp/issues/1798
RUN dotnet publish ./DotInitializr/Server/DotInitializr.Server.csproj -r alpine.3.9-x64 -c Release -o /app

FROM mcr.microsoft.com/dotnet/core/aspnet:3.1-alpine
WORKDIR /app
COPY --from=build /app .
ARG aspnetenv=Production

ENV ASPNETCORE_ENVIRONMENT ${aspnetenv}
CMD ASPNETCORE_URLS=http://*:$PORT dotnet DotInitializr.Server.dll