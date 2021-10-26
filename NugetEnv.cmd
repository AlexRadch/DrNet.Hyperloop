
powershell "$Env:nugetAPIKEY = 'APIKEY'"
powershell "$Env:HyperloopCommit = 'COMMIT'"

powershell "[Environment]::SetEnvironmentVariable('nugetAPIKEY', 'APIKEY', 'User')"
powershell "[Environment]::SetEnvironmentVariable('HyperloopCommit', 'COMMIT', 'User')"

SET NugetPackage=PACKAGE
dotnet nuget push "%NugetPackage%" --api-key %nugetAPIKEY% --source https://api.nuget.org/v3/index.json
