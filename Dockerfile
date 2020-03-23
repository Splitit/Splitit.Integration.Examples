FROM mcr.microsoft.com/dotnet/core/aspnet:2.2-stretch-slim AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/core/sdk:2.2-stretch AS build
WORKDIR /src
ARG BRANCH_NAME=master

COPY ["dotnet/mvc22/Splitit.Integration.Example.Mvc22/.", "Splitit.Integration.Example/"]

RUN dotnet restore "Splitit.Integration.Example/Splitit.Integration.Example.Mvc22.csproj"
COPY . .
WORKDIR "/src/Splitit.Integration.Example"
RUN ls
RUN cp -f "./appsettings.SpititDemo.json" "./appsettings.json"
RUN dotnet build "Splitit.Integration.Example.Mvc22.csproj" -c Release -o /app
#COPY ["Splitit.Web.LandingPages/cert-aspnetcore.pfx", "/app"]

FROM build AS publish
RUN dotnet publish "Splitit.Integration.Example.Mvc22.csproj" -c Release -o /app

FROM base AS final
WORKDIR /app
ENV ASPNETCORE_URLS=http://*:80;https://*:443 DOTNET_RUNNING_IN_CONTAINER=true 
ENV ASPNETCORE_HTTPS_PORT=443
ENV ASPNETCORE_Kestrel__Certificates__Default__Password="123456" 
ENV ASPNETCORE_Kestrel__Certificates__Default__Path=./certs/domain.pfx 
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "Splitit.Integration.Example.Mvc22.dll"]
