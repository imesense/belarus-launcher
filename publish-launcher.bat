dotnet publish src\ImeSense.Launchers.Belarus.Avalonia\ImeSense.Launchers.Belarus.Avalonia.csproj ^
    --configuration Release ^
    --runtime win-x64 ^
    -p:PublishAot=true ^
    -p:InvariantGlobalization=true ^
    -p:IsAotCompatible=true ^
    -p:DebugSymbols=false ^
    -p:DebugType=None
