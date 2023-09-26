namespace Verticular.Console
{
  using System;

  internal static class ConsoleOutputColorSystemDetector
  {
    private const string NoColorEnvironmentVariable1 = "NO_COLOR";
    private const string NoColorEnvironmentVariable2 = "DOTNET_NO_COLOR";
    private const string ColorTermEnvironmentVariable = "COLORTERM";
    private const string TrueColorTerminal = "truecolor";
    private const string TwentyFourBitTerminal = "24Bit";

    public static ConsoleColorSystem Detect(bool supportsAnsi)
    {
      // No colors?
      if (PlatformHelpers.InEnvironment(NoColorEnvironmentVariable1)
        || PlatformHelpers.InEnvironment(NoColorEnvironmentVariable2))
      {
        return ConsoleColorSystem.NoColors;
      }

      // Windows?
      if (PlatformHelpers.IsWindows())
      {
        if (!supportsAnsi)
        {
          // Figure out what we should do here.
          // Does really all Windows terminals support
          // eight-bit colors? Probably not...
          return ConsoleColorSystem.EightBit;
        }

        // Windows 10.0.15063 and above support true color,
        // and we can probably assume that the next major
        // version of Windows will support true color as well.
        if (PlatformHelpers.TryGetWindowsVersionInformation(out var major, out var build))
        {
          if (major == 10 && build >= 15063)
          {
            return ConsoleColorSystem.TrueColor;
          }
          else if (major > 10)
          {
            return ConsoleColorSystem.TrueColor;
          }
        }
      }
      else
      {
        var colorTerm = Environment.GetEnvironmentVariable(ColorTermEnvironmentVariable);
        if (!string.IsNullOrWhiteSpace(colorTerm))
        {
          if (colorTerm.Equals(TrueColorTerminal, StringComparison.OrdinalIgnoreCase)
            || colorTerm.Equals(TwentyFourBitTerminal, StringComparison.OrdinalIgnoreCase))
          {
            return ConsoleColorSystem.TrueColor;
          }
        }
      }

      // Should we default to eight-bit colors?
      return ConsoleColorSystem.EightBit;
    }
  }
}
