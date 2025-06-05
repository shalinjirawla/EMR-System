FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build

WORKDIR /src
COPY ["src/EMRSystem.Web.Host/EMRSystem.Web.Host.csproj", "src/EMRSystem.Web.Host/"]
COPY ["src/EMRSystem.Web.Core/EMRSystem.Web.Core.csproj", "src/EMRSystem.Web.Core/"]
COPY ["src/EMRSystem.Application/EMRSystem.Application.csproj", "src/EMRSystem.Application/"]
COPY ["src/EMRSystem.Core/EMRSystem.Core.csproj", "src/EMRSystem.Core/"]
COPY ["src/EMRSystem.EntityFrameworkCore/EMRSystem.EntityFrameworkCore.csproj", "src/EMRSystem.EntityFrameworkCore/"]
WORKDIR "/src/src/EMRSystem.Web.Host"
RUN dotnet restore 

WORKDIR /src
COPY ["src/EMRSystem.Web.Host", "src/EMRSystem.Web.Host"]
COPY ["src/EMRSystem.Web.Core", "src/EMRSystem.Web.Core"]
COPY ["src/EMRSystem.Application", "src/EMRSystem.Application"]
COPY ["src/EMRSystem.Core", "src/EMRSystem.Core"]
COPY ["src/EMRSystem.EntityFrameworkCore", "src/EMRSystem.EntityFrameworkCore"]
WORKDIR "/src/src/EMRSystem.Web.Host"
RUN dotnet publish -c Release -o /publish --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:8.0
EXPOSE 80
WORKDIR /app
COPY --from=build /publish .
ENTRYPOINT ["dotnet", "EMRSystem.Web.Host.dll"]
