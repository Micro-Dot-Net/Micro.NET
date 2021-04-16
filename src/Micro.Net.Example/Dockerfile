FROM mcr.microsoft.com/dotnet/aspnet:5.0 AS base
ARG buildPath
COPY ${buildPath} /app
WORKDIR /app
ENV Host:Assembly=/app/Micro.Net.Example.dll
ENV Host:ConfigClass=