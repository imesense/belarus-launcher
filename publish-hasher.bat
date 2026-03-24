dotnet publish src\Belarus.Launcher.CryptoHasher\Belarus.Launcher.CryptoHasher.csproj ^
    --configuration Release ^
    --runtime win-x64 ^
    -p:PublishAot=true ^
    -p:InvariantGlobalization=false ^
    -p:IsAotCompatible=true ^
    -p:DebugSymbols=false ^
    -p:DebugType=None
