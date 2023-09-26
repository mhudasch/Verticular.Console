namespace Verticular.Console
{
  using System;
  using System.Collections.Generic;
  using System.Linq;
  using System.Text;
  using System.Text.RegularExpressions;
  using System.Threading.Tasks;

  internal static class TerminalDetector
  {
    private static readonly Regex[] Regexes =
    {
        new("^xterm", RegexOptions.Compiled), // xterm, PuTTY, Mintty
        new("^rxvt", RegexOptions.Compiled), // RXVT
        new("^eterm", RegexOptions.Compiled), // Eterm
        new("^screen", RegexOptions.Compiled), // GNU screen, tmux
        new("tmux", RegexOptions.Compiled), // tmux
        new("^vt100", RegexOptions.Compiled), // DEC VT series
        new("^vt102", RegexOptions.Compiled), // DEC VT series
        new("^vt220", RegexOptions.Compiled), // DEC VT series
        new("^vt320", RegexOptions.Compiled), // DEC VT series
        new("ansi", RegexOptions.Compiled ), // ANSI
        new("scoansi", RegexOptions.Compiled  ), // SCO ANSI
        new("cygwin", RegexOptions.Compiled), // Cygwin, MinGW
        new("linux", RegexOptions.Compiled), // Linux console
        new("konsole", RegexOptions.Compiled), // Konsole
        new("bvterm", RegexOptions.Compiled), // Bitvise SSH Client
        new("^st-256color", RegexOptions.Compiled), // Suckless Simple Terminal, st
        new("alacritty", RegexOptions.Compiled), // Alacritty
    };

    internal static (bool SupportsAnsi, bool LegacyConsole) DetectCapabilitiesFromTerm()
    {
      // Running under ConEmu?
      var conEmu = Environment.GetEnvironmentVariable("ConEmuANSI");
      if (!string.IsNullOrEmpty(conEmu) && conEmu.Equals("On", StringComparison.OrdinalIgnoreCase))
      {
        return (true, false);
      }

      // Check if the terminal is of type ANSI/VT100/xterm compatible.
      var term = Environment.GetEnvironmentVariable("TERM");
      if (!string.IsNullOrWhiteSpace(term))
      {
        if (Regexes.Any(regex => regex.IsMatch(term)))
        {
          return (true, false);
        }
      }

      return (false, true);
    }
  }
}
