# Redist

Third-party redistributables chained into `App.Bundle`.

`windowsdesktop-runtime-win-x64.exe` is the .NET 10 Desktop Runtime (x64) installer,
checked in here so `App.Bundle.wixproj` builds without a separate download step. It's a
~60 MB Microsoft-owned binary that gets a new patch release roughly monthly, so refresh
it periodically from:

https://aka.ms/dotnet/10.0/windowsdesktop-runtime-win-x64.exe
