FROM mcr.microsoft.com/dotnet/framework/wpf:4.8

WORKDIR /app

COPY . ./app

ENTRYPOINT ["FiscalFlowAdmin\FiscalFlowAdmin\bin\Debug\net8.0-windows\FiscalFlowAdmiin.exe"]