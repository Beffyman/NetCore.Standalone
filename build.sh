dotnet restore src\\NetCore.Standalone.sln;
dotnet clean src\\NetCore.Standalone.sln;
dotnet build src\\NetCore.Standalone.sln -c Debug;
dotnet build src\\NetCore.Standalone.sln -c Release;
dotnet test src\\NetCore.Standalone.Tests\\NetCore.Standalone.Tests.csproj;
dotnet publish src\\NetCore.Standalone\\NetCore.Standalone.csproj -c Release -o bin\\Publish;

nuget pack src\\NetCore.Standalone\\NetCore.Standalone.nuspec -version $TRAVIS_TAG
nuget push NetCore.Standalone.$TRAVIS_TAG.nupkg $API_KEY -Source https://www.nuget.org/api/v2/package