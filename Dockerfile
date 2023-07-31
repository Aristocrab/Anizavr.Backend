FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src

# copy all the layers' csproj files into respective folders
COPY ["./Anizavr.Backend.Application/Anizavr.Backend.Application.csproj", "src/Anizavr.Backend.Application/"]
COPY ["./Anizavr.Backend.WebApi/Anizavr.Backend.WebApi.csproj", "src/Anizavr.Backend.WebApi/"]

# run restore over API project - this pulls restore over the dependent projects as well
RUN dotnet restore "src/Anizavr.Backend.WebApi/Anizavr.Backend.WebApi.csproj"

COPY . .

# run build over the API project
WORKDIR "/src/Anizavr.Backend.WebApi/"
RUN dotnet build -c Release -o /app/build

# run publish over the API project
FROM build AS publish
RUN dotnet publish -c Release -o /app/publish

FROM base AS runtime
WORKDIR /app

COPY --from=publish /app/publish .
ENTRYPOINT [ "dotnet", "Anizavr.Backend.WebApi.dll" ]