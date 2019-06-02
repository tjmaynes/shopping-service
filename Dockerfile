FROM microsoft/dotnet:sdk AS build-env
WORKDIR /app

COPY . ./

RUN dotnet publish -c Release -o dist

FROM microsoft/dotnet:aspnetcore-runtime
WORKDIR /app
COPY --from=build-env /app/ShoppingService.Api/dist .
ENV ASPNETCORE_URLS http://+:80
ENTRYPOINT ["dotnet", "ShoppingService.Api.dll"]
