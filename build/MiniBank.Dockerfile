FROM mcr.microsoft.com/dotnet/sdk:6.0 AS src
WORKDIR /src
COPY . .

RUN dotnet build MiniBank.Web -c Release -r linux-x64
RUN dotnet test Tests/MiniBank.Core.Tests --no-build
RUN dotnet publish MiniBank.Web -c Release -r linux-x64 --no-build -o /dist

FROM mcr.microsoft.com/dotnet/aspnet:6.0 as final
WORKDIR /app
COPY --from=src /dist .

ARG BUILD_CONFIGURATION=Debug 
ENV ASPNETCORE_ENVIRONMENT=Development 
ENV DOTNET_USE_POLLING_FILE_WATCHER=true

ENV ASPNETCORE_URLS=http://+:5001
EXPOSE 5001

ENTRYPOINT ["dotnet", "MiniBank.Web.dll"]