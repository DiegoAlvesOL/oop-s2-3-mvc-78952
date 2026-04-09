# Fase 1 — Build
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /app

# Copia os ficheiros de projecto e restaura as dependências
COPY *.slnx ./
COPY src/VgcCollege.Domain/VgcCollege.Domain.csproj src/VgcCollege.Domain/
COPY src/VgcCollege.Application/VgcCollege.Application.csproj src/VgcCollege.Application/
COPY src/VgcCollege.Data/VgcCollege.Data.csproj src/VgcCollege.Data/
COPY src/VgcCollege.Web/VgcCollege.Web.csproj src/VgcCollege.Web/
COPY tests/VgcCollege.Domain.Tests/VgcCollege.Domain.Tests.csproj tests/VgcCollege.Domain.Tests/
COPY tests/VgcCollege.Application.Tests/VgcCollege.Application.Tests.csproj tests/VgcCollege.Application.Tests/
COPY tests/VgcCollege.Data.Tests/VgcCollege.Data.Tests.csproj tests/VgcCollege.Data.Tests/

RUN dotnet restore

# Copia o código fonte e compila
COPY . .
RUN dotnet publish src/VgcCollege.Web/VgcCollege.Web.csproj -c Release -o /app/publish

# Fase 2 — Runtime
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS runtime
WORKDIR /app

COPY --from=build /app/publish .

EXPOSE 8080

ENTRYPOINT ["dotnet", "VgcCollege.Web.dll"]