mkdir input\lib\net40
del /Q input\lib\net40\*.*

msbuild .\..\RouteLocalization.Http\RouteLocalization.Http.csproj /p:Configuration=Release;DefineConstants="ASPNETWEBAPI";OutputPath=.\..\RouteLocalization.Http.Nuget\input\lib\net40

mkdir output
..\.nuget\nuget.exe pack /o output .\RouteLocalization.WebApi.nuspec