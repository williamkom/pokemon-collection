FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build

WORKDIR /src

COPY ["Pokemon.Api/Pokemon.Api.csproj", "Pokemon.Api/"]

RUN dotnet restore "Pokemon.Api/Pokemon.Api.csproj"

COPY . .

WORKDIR "/src/Pokemon.Api"

RUN dotnet publish \
    "Pokemon.Api.csproj" \
    -c Release \
    -o /app/publish \
    /p:UseAppHost=false


FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final

WORKDIR /app

COPY --from=build /app/publish .

EXPOSE 8080

ENV ASPNETCORE_URLS=http://+:8080

ENTRYPOINT ["dotnet", "Pokemon.Api.dll"]