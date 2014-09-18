mkdir input\lib\net40
del /Q input\lib\net40\*.*

msbuild .\..\RouteLocalization.Mvc\RouteLocalization.Mvc.csproj /p:Configuration=Release;OutputPath=.\..\RouteLocalization.Mvc.Nuget\input\lib\net40

mkdir output
..\.nuget\nuget.exe pack /o output .\RouteLocalization.Mvc.nuspec