$version="0.0.3"

$package="TestBucket.AI.Xunit"
cd src/${package}
dotnet pack -p:PackageVersion=$version
nuget push ./bin/Release/${package}.${version}.nupkg -Source https://api.nuget.org/v3/index.json
cd ../..
