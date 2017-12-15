using System;
using JetBrains.Annotations;

namespace Abstractions.IO
{
  using System.Diagnostics;

  [Serializable]
  [DebuggerDisplay("{Value}")]
  public sealed class Path
  {
    public string Value { get; }

    public Path(string value)
    {
      if (value == null)
      {
        throw new ArgumentNullException(nameof(value));
      }

      if (string.IsNullOrWhiteSpace(value))
      {
        throw new ArgumentException("Argument is empty string or whitespace.", nameof(value));
      }

      try
      {
        System.IO.Path.GetFullPath(value);
      }
      catch (NotSupportedException)
      {
        throw new ArgumentException("Invalid path.", nameof(value));
      }

      Value = value;
    }

    [CanBeNull]
    public static Path TryParseFromUserInput([CanBeNull] string value)
    {
      // nothing is nothing
      if (value == null)
      {
        return null;
      }

      // first remove wrapping spaces as it is typical copy-paste issue
      value = value.Trim();

      var chars = new[] { '\'', '\"' };
      for (var i = 0; i < chars.Length; i += 1)
      {
        var first = chars[i] == value[0];
        var last = chars[i] == value[value.Length - 1];

        if (first && last)
        {
          value = value.Substring(1, value.Length - 2);
        }
        else if (first || last)
        {
          // different types of quotes on left and right is not acceptable
          return null;
        }
      }

      // do not trim afterwards as this value was inside quotes 
      // and mistake there must not be tolerated
      try
      {
        return new Path(value);
      }
      catch
      {
        return null;        
      }
    }

    public override string ToString()
    {
      return Value;
    }

    public override int GetHashCode()
    {
      return Value.GetHashCode();
    }

    public static Path operator /([NotNull] Path left, [NotNull] Path right)
    {
      if (left == null)
      {
        throw new ArgumentNullException(nameof(left));
      }

      if (right == null)
      {
        throw new ArgumentNullException(nameof(right));
      }

      return new Path(System.IO.Path.Combine(left.Value, right.Value));
    }

    public static Path operator /([NotNull] string left, [NotNull] Path right)
    {
      if (left == null)
      {
        throw new ArgumentNullException(nameof(left));
      }

      if (right == null)
      {
        throw new ArgumentNullException(nameof(right));
      }

      return new Path(System.IO.Path.Combine(left, right.Value));
    }

    public static Path operator /([NotNull] Path left, [NotNull] string right)
    {
      if (left == null)
      {
        throw new ArgumentNullException(nameof(left));
      }

      if (right == null)
      {
        throw new ArgumentNullException(nameof(right));
      }

      return new Path(System.IO.Path.Combine(left.Value, right));
    }

    public static Path operator +([NotNull] Path left, [NotNull] Path right)
    {
      if (left == null)
      {
        throw new ArgumentNullException(nameof(left));
      }

      if (right == null)
      {
        throw new ArgumentNullException(nameof(right));
      }

      return new Path(System.IO.Path.Combine(left.Value, right.Value));
    }

    public static Path operator +([NotNull] string left, [NotNull] Path right)
    {
      if (left == null)
      {
        throw new ArgumentNullException(nameof(left));
      }

      if (right == null)
      {
        throw new ArgumentNullException(nameof(right));
      }

      return new Path(System.IO.Path.Combine(left, right.Value));
    }

    public static Path operator +([NotNull] Path left, [NotNull] string right)
    {
      if (left == null)
      {
        throw new ArgumentNullException(nameof(left));
      }

      if (right == null)
      {
        throw new ArgumentNullException(nameof(right));
      }

      return new Path(System.IO.Path.Combine(left.Value, right));
    }
  }
}
