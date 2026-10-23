using System.Runtime.InteropServices;

namespace App.Client.Controls;

/// <summary>
/// WinUI 3 has no built-in Window-to-Window modality. This reproduces what WinForms' own Form.ShowDialog
/// does internally: give the dialog a real owner (GWLP_HWNDPARENT) so it stays grouped with it in Alt-Tab
/// and minimizes/restores with it, then disable the owner's hwnd for as long as the dialog is shown.
/// </summary>
internal static partial class NativeMethods
{
    private const int GwlpHwndParent = -8;

    [LibraryImport("user32.dll")]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static partial bool EnableWindow(IntPtr hWnd, [MarshalAs(UnmanagedType.Bool)] bool bEnable);

    [LibraryImport("user32.dll", EntryPoint = "SetWindowLongPtrW")]
    private static partial IntPtr setWindowLongPtr(IntPtr hWnd, int nIndex, IntPtr dwNewLong);

    public static void SetOwner(IntPtr childHwnd, IntPtr ownerHwnd) => setWindowLongPtr(childHwnd, GwlpHwndParent, ownerHwnd);
}
