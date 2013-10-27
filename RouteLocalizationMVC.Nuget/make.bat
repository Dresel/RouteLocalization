mkdir input\lib\net40
del /Q input\lib\net40\*.*

msbuild ..\RouteLocalizationMVC\RouteLocalizationMVC.csproj /p:Configuration=Release;OutputPath=..\RouteLocalizationMVC.Nuget\input\lib\net40

mkdir output
..\.nuget\nuget.exe pack /o output .\RouteLocalizationMVC.nuspec