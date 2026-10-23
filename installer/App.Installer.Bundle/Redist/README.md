# Redist

Nothing is checked in here. Both chained prerequisites (`DotNetDesktopRuntime.wxs`,
`WindowsAppRuntime.wxs`) reference their installer by Hash/Size/DownloadUrl instead of a
local file - refresh those values when the corresponding package version changes.
