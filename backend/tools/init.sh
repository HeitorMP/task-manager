#!/bin/sh

export SECRET_KEY=$(openssl rand -base64 64)

export ASPNETCORE_URLS=http://+:5170

# Rodar o aplicativo .NET
exec dotnet TaskApi.dll
