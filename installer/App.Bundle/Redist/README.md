# Redist

Third-party redistributables chained into `App.Bundle`, kept out of source control
(large, Microsoft-owned binaries — see `.gitignore`).

Before building `App.Bundle.wixproj`, download the .NET 10 Desktop Runtime (x64)
installer into this folder as `windowsdesktop-runtime-win-x64.exe`:

https://aka.ms/dotnet/10.0/windowsdesktop-runtime-win-x64.exe
