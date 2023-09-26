namespace Verticular.Console
{
  internal static class PlatformHelpers
  {
#if !NET6_0_OR_GREATER
    private static readonly System.Text.RegularExpressions.Regex WindowsVersionRegex =
      new("Microsoft Windows (?'major'[0-9]*).(?'minor'[0-9]*).(?'build'[0-9]*)\\s*$", System.Text.RegularExpressions.RegexOptions.Compiled);
#endif

    public static bool IsWindows() =>
#if NETCOREAPP
      System.OperatingSystem.IsWindows();
#elif NETFRAMEWORK
      System.Environment.OSVersion.Platform == System.PlatformID.Win32NT;
#else
      System.Runtime.InteropServices.RuntimeInformation.IsOSPlatform(System.Runtime.InteropServices.OSPlatform.Windows);
#endif

    public static bool TryGetWindowsVersionInformation(out int major, out int build)
    {
      major = 0;
      build = 0;

      if (!IsWindows())
      {
        return false;
      }

#if NET6_0_OR_GREATER
      var version = System.Environment.OSVersion.Version;
      major = version.Major;
      build = version.Build;
      return true;
#endif

#if NETSTANDARD
      var versionString = System.Runtime.InteropServices.RuntimeInformation.OSDescription;
#elif NETFRAMEWORK
      var versionString = Interop.WindowsKernel32.GetWindowsVersion();
#endif

#if NETSTANDARD || NETFRAMEWORK
      if (versionString == null)
      {
        return false;
      }
      var match = WindowsVersionRegex.Match(versionString);
      return match.Success
        && int.TryParse(match.Groups["major"].Value, out major)
        && int.TryParse(match.Groups["build"].Value, out build);
#endif
    }

    internal static bool InEnvironment(string variable)
    {
      var variables = System.Environment.GetEnvironmentVariables();
      return variables.Contains(variable) || variables.Contains(variable.ToUpperInvariant()) || variables.Contains(variable.ToLowerInvariant());
    }

    internal static bool TryGetNativeAnsiSupport(bool standardOutput, out bool supportsAnsi, out bool legacyConsole) =>
      Interop.WindowsKernel32.TryGetNativeAnsiSupport(standardOutput, out supportsAnsi, out legacyConsole);
  }
}
