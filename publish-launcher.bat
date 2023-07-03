dotnet publish src\StalkerBelarus.Launcher.Legacy\StalkerBelarus.Launcher.Legacy.csproj ^
    --configuration Release ^
    --runtime win-x86 ^
    --self-contained true ^
    -p:PublishSingleFile=true ^
    -p:IncludeAllContentForSelfExtract=true ^
    -p:EnableCompressionInSingleFile=true ^
    -p:DebugSymbols=false ^
    -p:DebugType=None
