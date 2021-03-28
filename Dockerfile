FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build-env
WORKDIR /app
COPY . .
RUN apt-get update && apt-get install -y --no-install-recommends make
RUN make build_artifact

FROM mcr.microsoft.com/dotnet/aspnet:5.0
WORKDIR /app
COPY --from=build-env /app/dist .
ENV ASPNETCORE_URLS http://+:80
ENTRYPOINT ["dotnet", "ShoppingService.Api.dll"]
