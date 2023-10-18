#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["ReviewsDeGames/ReviewsDeGames.csproj", "ReviewsDeGames/"]
RUN dotnet restore "ReviewsDeGames/ReviewsDeGames.csproj"
COPY . .
WORKDIR "/src/ReviewsDeGames"
RUN dotnet build "ReviewsDeGames.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "ReviewsDeGames.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Descomente este bloco para aplicar o update de migração
#RUN dotnet tool install --global dotnet-ef
#ENV PATH="$PATH:/root/.dotnet/tools"
#RUN dotnet ef database update

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "ReviewsDeGames.dll"]