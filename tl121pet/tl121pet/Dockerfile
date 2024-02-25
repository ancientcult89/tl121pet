#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["tl121pet/tl121pet.csproj", "tl121pet/"]
COPY ["tl121pet.Entities/tl121pet.Entities.csproj", "tl121pet.Entities/"]
COPY ["tl121pet.DAL/tl121pet.DAL.csproj", "tl121pet.DAL/"]
COPY ["tl121pet.Services/tl121pet.Services.csproj", "tl121pet.Services/"]
RUN dotnet restore "tl121pet/tl121pet.csproj"
COPY . .
WORKDIR "/src/tl121pet"
RUN dotnet build "tl121pet.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "tl121pet.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "tl121pet.dll"]