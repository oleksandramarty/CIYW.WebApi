# Migrations DataContext
> dotnet ef migrations add Init --context DataContext --output-dir Migrations --startup-project ..\CIYW.ClientApi\CIYW.ClientApi.csproj

# MacOs
> dotnet ef migrations add AdminIsActive --context DataContext --output-dir Migrations --startup-project ../CIYW.ClientApi/CIYW.ClientApi.csproj

#dotnet-ef not found - check version
> dotnet tool install --global dotnet-ef --version 8.0.100

# dotnet --version
> 8.0.100

### OpenAPI json
> /swagger/index.html

### Swagger
> /swagger/index.html


# NSWAG CMD
> nswag run


# Docker
> docker build -t ciyw-image .
> 
> docker run -p 8080:80 ciyw-image