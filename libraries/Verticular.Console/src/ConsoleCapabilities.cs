namespace Verticular.Console
{
  using System.Diagnostics;
  using System.IO;
  using System.Text;

  /// <summary>
  /// Represents console capabilities.
  /// </summary>
  [DebuggerDisplay("Capabilities = {ToString()}")]
  public sealed class ConsoleOutputCapabilities : IReadOnlyConsoleOutputCapabilities
  {
    /// <inheritdoc />
    public ConsoleColorSystem ColorSystem { get; set; }

    /// <inheritdoc />
    public bool SupportsAnsiSequences { get; set; }

    /// <inheritdoc />
    public bool SupportsLinks { get; set; }

    /// <inheritdoc />
    public bool IsLegacy { get; set; }

    /// <inheritdoc />
    public bool SupportsUnicode { get; set; }

    /// <inheritdoc />
    public bool SupportsAlternateBuffer { get; set; }

    /// <summary>
    /// Detects the capabilities of a given console text writer.
    /// </summary>
    /// <param name="writer">The writer or standard output or error streams.</param>
    /// <param name="isStandardOutput">True when the given writer is for standard output; otherwise false.</param>
    /// <returns>The capabilties of the given console text writer.</returns>
    public static IReadOnlyConsoleOutputCapabilities Detect(TextWriter writer, bool isStandardOutput = true)
    {
      var (supportsAnsi, legacyConsole) = isStandardOutput
        ? ConsoleOutputAnsiDetector.DetectForStandardOut()
        : ConsoleOutputAnsiDetector.DetectForStandardError();

      return new ConsoleOutputCapabilities
      {
        ColorSystem = ConsoleOutputColorSystemDetector.Detect(supportsAnsi),
        IsLegacy = legacyConsole,
        SupportsAlternateBuffer = supportsAnsi && !legacyConsole,
        SupportsAnsiSequences = supportsAnsi,
        SupportsLinks = supportsAnsi && !legacyConsole,
        SupportsUnicode = writer.Encoding.EncodingName == Encoding.Unicode.EncodingName,
      };
    }

    /// <inheritdoc />
    public override string ToString() =>
      $"lgc={this.IsLegacy},ansi={this.SupportsAnsiSequences},cs={this.ColorSystem},lks={this.SupportsLinks},uni={this.SupportsUnicode},alt={this.SupportsAlternateBuffer}";
  }
}
