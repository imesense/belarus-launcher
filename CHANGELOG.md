# Changelog

English | [Русский](./CHANGELOG.ru.md)

Full changelog of _Belarus Launcher_ project

## Release 3.0 (May 2024)

### Common

- Updated target framework to .NET 8 (@Drombeys)
- Updated XamlStyler settings (@Drombeys)
- Added basic config for PVS-Studio (@Drombeys)
- Updated NuGet packages (@Drombeys)
- Updated readme (@Drombeys)
- Restyled codebase (@Drombeys)
- Enabled Native AOT (@Drombeys)
- Enabled caching dependencies (@acidicMercury8)
- Enabled NuGet lock files (@acidicMercury8)
- Enabled markup formatting (@acidicMercury8)
- Updated Visual Studio Install config (@acidicMercury8)
- Added badges to readme (@acidicMercury8)
- Improved building workflows (@acidicMercury8)
- Fixed warnings (@Drombeys)

### Launcher

- Disabled launching multiple application instances (@acidicMercury8)
- Added file size output to log (@Drombeys)
- Reworked logger (@Drombeys)
- Implemented splash screen manager (@Drombeys)
- Implemented zipped release download (@Drombeys)
- Added launcher settings migrator (@Drombeys)
- Improved request logging (@Drombeys)
- Added pause on downloading (@Drombeys)
- Added locale if it's undefined in user settings (@Drombeys)
- Implemented caching of web resources and news (@Drombeys)
- Saved logs and current release to separate folder (@Drombeys)
- Decomposed `ImeSense.Launchers.Belarus` project (@Drombeys)
- Added SSL protocol settings (@Drombeys)
- Fixed pressed colors in `ComboBox` control (@Drombeys)
- Fixed excessive memory allocation in `AxamlLocaleManager` class (@Drombeys)
- Refactored locale manager module (@Drombeys)
- Reworked setup bindings in `AuthorizationViewModel` class (@Drombeys)
- Fixed selection colors in `ComboBox` control (@Drombeys)
- Load user settings in `OnFrameworkInitializationCompleted` method (@Drombeys)
- Redefine `ComboBoxItem` control theme (@acidicMercury8)
- Load web resources in separate thread (@Drombeys)
- Reworked news and validation uploads (@Drombeys)
- Added server connection check (@Drombeys)
- Reworked data initialization calling (@Drombeys)
- Implemented file size verification (@Drombeys)
- Reworked multi-threaded file hash calculation (@Drombeys)
- Fixed opening link to organization (@Drombeys)
- Implemented view models locator (@Drombeys)

### UI

- Implemented basic loading splash (@Drombeys)

### Utilities

- Implemented multithreaded file hash processing in `CryptoHasher` project (@Drombeys)
- Enabled logging in console applications (@Drombeys)

## Release 2.1 (January 2024)

### Launcher

- Fixed downloading game updates

### Utilities

- Fixed log output in `CryptoHasher` project

## Release 2.0 (January 2024)

### Common

- Implemented Avalonia application (@Drombeys)
- Added Avalonia themes project (@acidicMercury8)
- Updated Visual Studio Code configs (@acidicMercury8)
- Added NuGet config (@acidicMercury8)
- Updated NuGet packages (@Drombeys, @acidicMercury8)
- Updated EditorConfig filters (@acidicMercury8)
- Updated publish script (@Drombeys, @acidicMercury8)
- Updated building workflow (@acidicMercury8)
- Renamed global namespace (@acidicMercury8)
- Updated root configs (@Drombeys, @acidicMercury8)
- Updated root documents (@Drombeys, @acidicMercury8)
- Update target framework to .NET 7 (@Drombeys)

### Launcher

- Reworked downloading service (@Drombeys)
- Integrated logging (@Drombeys)
- Reimplemented basic functionality (@Drombeys)
- Implemented application version output (@Drombeys)
- Added checks for existence of `binaries` и `resources` directories (@Drombeys)
- Implemented file hash verification (@Drombeys)
- Implemented getting GitHub release data (@Drombeys)
- Implemented getting JSON data from GitHub release (@Drombeys)
- Implemented resource loading (@Drombeys)
- Added logging to debug console (@Drombeys)
- Implemented dynamic loading of links to Internet resources (@Drombeys)
- Implemented multithreaded file hash validation (@Drombeys)
- Implemented update checking (@Drombeys)
- Implement application locale change (@Drombeys, @acidicMercury8)
- Added filename display during download process (@Drombeys)
- Added protection against corrupted configuration files (@Drombeys)
- Implemented server startup blocking (@Drombeys)
- Implemented process helper (@Drombeys)
- Implemented nickname validation (@Drombeys)
- Implemented Ip address validation (@Drombeys)
- Added requirement to run application as an administrator (@Drombeys)
- Added system information output (@Drombeys)
- Reworked user manager (@Drombeys)
- Changed base address to repository (@Drombeys)
- Implemented receiving tags (@Drombeys)
- Implemented release comparer service (@Drombeys)
- Implemented initializer manager (@Drombeys)
- Optimized application loading (@Drombeys)
- Optimized news loading (@Drombeys)
- Implemented update `SBLauncherUpdater` application (@Drombeys)

### Utilities

- Implemented `CryptoHasher` application (@Drombeys)
- Implemented launcher updater (@Drombeys)

### UI

- Reimplemented basic views (@Drombeys)

### Style

- Reimplemented basic styles (@Drombeys, @acidicMercury8)
- Added opacity to disabled button (@Drombeys, @acidicMercury8)

### Assets

- Localized all strings (@acidicMercury8)

## Release 1.1 (July 2023)

### Common

- Added installer sources (@acidicMercury8)
- Updated solution items (@Drombeys)
- Added `StalkerBelarus.Launcher.Core` project (@Drombeys)
- Fixed installer publishing pipeline (@acidicMercury8)
- Renamed `StalkerBelarus.Launcher` to `StalkerBelarus.Launcher.Legacy` (@Drombeys)
- Updated readme (@Drombeys, @acidicMercury8)

### Launcher

- Replaced `IReactiveCommand` with `ReactiveCommand` (@Drombeys)
- Renamed `MyDownloadManager` to `DownloadManager` (@Drombeys)
- Fixed indentation in `DownloadManager` class (@Drombeys)
- Implemented `GoWebSite` method (@Drombeys)
- Fixed `DownloadsImpl` method (@Drombeys)
- Moved `UserSetting` class in core project (@Drombeys)
- Moved `Manager` folder in core project (@Drombeys)
- Moved `Launcher` class in core project (@Drombeys)
- Improved load news content (@Drombeys)
- Improved view models (@Drombeys)
- Moved ViewModels in `StalkerBelarus.Launcher` project (@Drombeys)
- Fixed saving user settings (@Drombeys)
- Replaced `IHost` with `IServiceProvider` in App class (@Drombeys)

## Release 1.0 (June 2023)

### Common

- Implemented WPF application (@Drombeys)
- Added solution items (@Drombeys)
- Added Visual Studio Code configs (@acidicMercury8)
- Added basic EditorConfig (@acidicMercury8)
- Added Visual Studio Install config (@acidicMercury8)
- Added auxiliary building script (@acidicMercury8)
- Enabled GitHub Actions (@Drombeys, @acidicMercury8)
- Disabled symbols generation on release build (@Drombeys)
- Disabled `TargetFramework` appending to output path (@Drombeys)
- Enabled application publishing (@Drombeys)

### Launcher

- Integrated dependency injection library by Microsoft (@Drombeys)
- Implemented authorization (@Drombeys)
- Implemented navigation (@Drombeys)
- Implemented launcher (@Drombeys)
- Implemented start game (@Drombeys)
- Implemented window manager (@Drombeys)
- Implemented config saving (@Drombeys)
- Implemented closing launcher after game launch (@Drombeys)
- Implemented news and download services (@Hozar2002)

### UI

- Added authorization view (@Drombeys)
- Added news view (@Drombeys)
- Added news slider view (@Drombeys)
- Added menu view (@Drombeys)
- Added launcher view (@Drombeys)
- Added navigation between news (@Drombeys)
- Added check for updates button (@Drombeys)
- Added download button (@Drombeys)

### Style

- Added global style for buttons (@Drombeys)

### Assets

- Added Graffiti font (@Drombeys)
- Added background image (@Drombeys)
- Added logo image (@Drombeys)
- Added application icon file (@Drombeys)
