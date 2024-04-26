dotnet publish src\ImeSense.Launchers.Belarus.Desktop\ImeSense.Launchers.Belarus.Desktop.csproj ^
    --configuration Release ^
    --runtime win-x64 ^
    -p:PublishAot=true ^
    -p:InvariantGlobalization=false ^
    -p:IsAotCompatible=true ^
    -p:DebugSymbols=false ^
    -p:DebugType=None
