namespace Verticular.Console
{
  internal class ConsoleOutputAnsiDetector
  {
    public static (bool SupportsAnsi, bool LegacyConsole) DetectForStandardOut() =>
      Detect(true);
    public static (bool SupportsAnsi, bool LegacyConsole) DetectForStandardError() =>
      Detect(false);

    private static (bool SupportsAnsi, bool LegacyConsole) Detect(bool standardOut)
    {
      if (PlatformHelpers.IsWindows())
      {
        if (!PlatformHelpers.TryGetNativeAnsiSupport(standardOut, out var supportsAnsi, out var legacyConsole))
        {
          return (false, true);
        }

        if (!supportsAnsi)
        {
          // give it a second try
          return TerminalDetector.DetectCapabilitiesFromTerm();
        }

        return (supportsAnsi, legacyConsole);
      }

      return TerminalDetector.DetectCapabilitiesFromTerm();
    }
  }
}
