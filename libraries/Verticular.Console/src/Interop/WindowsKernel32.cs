namespace Verticular.Console.Interop
{
  using System;
  using System.Diagnostics;
  using System.Runtime.InteropServices;

  internal static class WindowsKernel32
  {
    [DllImport(WindowsLibraryNames.Kernel32, SetLastError = true, CharSet = CharSet.Auto)]
    private static extern bool GetConsoleMode(IntPtr handle, out int mode);

    [DllImport(WindowsLibraryNames.Kernel32, SetLastError = true, CharSet = CharSet.Auto)]
    private static extern bool SetConsoleMode(IntPtr handle, int mode);

    [DllImport(WindowsLibraryNames.Kernel32, SetLastError = true, CharSet = CharSet.Auto)]
    private static extern IntPtr GetStdHandle(int type);

    [DllImport(WindowsLibraryNames.Kernel32, SetLastError = true, CharSet = CharSet.Ansi)]
    private static extern bool GetVersionEx(ref OSVERSIONINFOEX osVersionInfo);

    private const int DISABLE_NEWLINE_AUTO_RETURN = 0x0008;
    private const int ENABLE_VIRTUAL_TERMINAL_PROCESSING = 0x0004;

    private const int STD_OUTPUT_HANDLE = -11;
    private const int STD_ERROR_HANDLE = -12;

    [StructLayout(LayoutKind.Sequential)]
    private struct OSVERSIONINFOEX
    {
      public uint dwOSVersionInfoSize;
      public uint dwMajorVersion;
      public uint dwMinorVersion;
      public uint dwBuildNumber;
      public uint dwPlatformId;
      [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 128)]
      public string szCSDVersion;
      public ushort wServicePackMajor;
      public ushort wServicePackMinor;
      public ushort wSuiteMask;
      public byte wProductType;
      public byte wReserved;
    }

    private static string? osVersion;

    internal static string? GetWindowsVersion()
    {
      if (osVersion != null)
      {
        return osVersion;
      }

      var osVersionInfo = new OSVERSIONINFOEX
      {
        dwOSVersionInfoSize = (uint)Marshal.SizeOf(typeof(OSVERSIONINFOEX))
      };

      if (GetVersionEx(ref osVersionInfo))
      {
        osVersion = $"Microsoft Windows {osVersionInfo.dwMajorVersion}.{osVersionInfo.dwMinorVersion}.{osVersionInfo.dwBuildNumber}";
      }
      else
      {
        var error = GetLastError();
        Debug.Assert(osVersion == null, $"GetVersionEx failed with {error}.");
      }
      return osVersion;
    }

    private static int GetLastError() =>
#if NET6_0_OR_GREATER
      Marshal.GetLastPInvokeError();
#else
      Marshal.GetLastWin32Error();
#endif

    internal static bool TryGetNativeAnsiSupport(bool stdOut, out bool supportsAnsi, out bool legacyConsole)
    {
      supportsAnsi = false;
      legacyConsole = true;

      try
      {
        var @out = GetStdHandle(stdOut ? STD_OUTPUT_HANDLE : STD_ERROR_HANDLE);
        if (!GetConsoleMode(@out, out var mode))
        {
          supportsAnsi = false;
          legacyConsole = true;
          return true;
        }

        if ((mode & ENABLE_VIRTUAL_TERMINAL_PROCESSING) == 0)
        {
          legacyConsole = true;
          supportsAnsi = true;
          return true;
        }

        legacyConsole = false;
        supportsAnsi = true;
        return true;
      }
      catch
      {
        // All we know here is that we don't support ANSI.
        return false;
      }
    }
  }
}
