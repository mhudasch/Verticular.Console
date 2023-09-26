namespace Verticular.Console
{
  /// <summary>
  /// Represents console capabilities.
  /// </summary>
  public interface IReadOnlyConsoleOutputCapabilities
  {
    /// <summary>
    /// Gets or sets the color system.
    /// </summary>
    ColorSystem ColorSystem { get; }

    /// <summary>
    /// Gets or sets a value indicating whether or not
    /// the console supports VT/ANSI control codes.
    /// </summary>
    bool SupportsAnsiSequences { get; }

    /// <summary>
    /// Gets or sets a value indicating whether or not
    /// the console support links.
    /// </summary>
    bool SupportsLinks { get; }

    /// <summary>
    /// Gets or sets a value indicating whether or not
    /// this is a legacy console (cmd.exe) on an OS
    /// prior to Windows 10.
    /// </summary>
    /// <remarks>
    /// Only relevant when running on Microsoft Windows.
    /// </remarks>
    public bool IsLegacy { get; }

    /// <summary>
    /// Gets or sets a value indicating whether
    /// or not the console supports Unicode.
    /// </summary>
    public bool SupportsUnicode { get; }

    /// <summary>
    /// Gets or sets a value indicating whether
    /// or not the console supports alternate buffers.
    /// </summary>
    public bool SupportsAlternateBuffer { get; }
  }
}
