dotnet build
dotnet pack
dotnet tool uninstall dotnet-storedprocsgen -g
dotnet tool install dotnet-storedprocsgen -g --add-source "C:\Users\serge\Source\Repos\dotnet-storedprocsgen\dotnet-storedprocsgen\bin\Debug"


REM https://docs.microsoft.com/en-us/dotnet/core/tools/dotnet-tool-install