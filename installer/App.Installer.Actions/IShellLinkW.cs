using System.Runtime.InteropServices;
using System.Text;

namespace App.Installer.Actions;

[ComImport]
[Guid("000214F9-0000-0000-C000-000000000046")]
[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
public interface IShellLinkW
{
    void GetPath(StringBuilder pszFile, int cchMaxPath, IntPtr pfd, uint fFlags);

    void GetIDList(out IntPtr ppidl);

    void SetIDList(IntPtr pidl);

    void GetDescription(StringBuilder pszName, int cchMaxName);

    void SetDescription(string pszName);

    void GetWorkingDirectory(StringBuilder pszDir, int cchMaxPath);

    void SetWorkingDirectory(string pszDir);

    void GetArguments(StringBuilder pszArgs, int cchMaxPath);

    void SetArguments(string pszArgs);

    void GetHotkey(out short wHotkey);

    void SetHotkey(short wHotkey);

    void GetShowCmd(out int iShowCmd);

    void SetShowCmd(int iShowCmd);

    void GetIconLocation(StringBuilder pszIconPath, int cchIconPath, out int iIcon);

    void SetIconLocation(string pszIconPath, int iIcon);

    void SetRelativePath(string pszPathRel, uint dwReserved);

    void Resolve(IntPtr hwnd, uint fFlags);

    void SetPath(string pszFile);
}