FROM microdotnet/microdotnet-host:latest AS base
ARG buildPath
COPY ${buildPath} /app
WORKDIR /app
ENV Host:Assembly=/app/Micro.Net.Example.dll
ENV Host:ConfigClass=ExampleMicroserviceConfigurable