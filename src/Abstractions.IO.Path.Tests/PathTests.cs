using System;
using Xunit;

namespace Abstractions.IO
{
  public class PathTests
  {
    [Theory]
    [InlineData(@"\.\")]
    [InlineData(@".")]
    [InlineData(@"..")]
    [InlineData(@"..\.")]
    [InlineData(@"..\..")]
    [InlineData(@"C:\")]
    [InlineData(@"C:\file1.txt")]
    [InlineData(@"C:\folder2\file1.txt")]
    [InlineData(@"C:\folder3/folder2\file1.txt")]
    public void ConstructorTests(string value)
    {
      var path = new Path(value);
      var actual = path.Value;

      var expected = value.Trim();
      Assert.Equal(expected, actual);
    }

    [Theory]
    [InlineData(null, typeof(ArgumentNullException), "Value cannot be null.\r\nParameter name: value")]
    [InlineData(@"", typeof(ArgumentException), "Argument is empty string or whitespace.\r\nParameter name: value")]
    [InlineData(@"C::\", typeof(ArgumentException), "Invalid path.\r\nParameter name: value")]
    public void ConstructorFailureTests(string value, Type exceptionType, string exceptionMessage)
    {
      try
      {
        new Path(value);
      }
      catch (ArgumentException ex)
      {
        Assert.Equal(exceptionType, ex.GetType());
        Assert.Equal(exceptionMessage, ex.Message);

        return;
      }

      throw new InvalidOperationException();
    }

    [Theory]
    [InlineData("\"C:\\file1.txt\"", @"C:\file1.txt")]
    [InlineData("  \"C:\\file1.txt\"", @"C:\file1.txt")]
    [InlineData("\"C:\\file1.txt\"  ", @"C:\file1.txt")]
    [InlineData("  \"C:\\file1.txt\"  ", @"C:\file1.txt")]
    [InlineData("'C:\\file1.txt'", @"C:\file1.txt")]
    [InlineData("  'C:\\file1.txt'", @"C:\file1.txt")]
    [InlineData("'C:\\file1.txt'  ", @"C:\file1.txt")]
    [InlineData("  'C:\\file1.txt'  ", @"C:\file1.txt")]
    public void TryParseFromUserInputTests(string value, string expected)
    {
      var actual = Path.TryParseFromUserInput(value)?.Value;

      Assert.Equal(expected, actual);
    }

    [Theory]
    [InlineData("\"C:\\file1.txt")]
    [InlineData("C:\\file1.txt\"")]
    [InlineData("  \"C:\\file1.txt")]
    [InlineData("C:\\file1.txt\"  ")]
    [InlineData("  C:\\file1.txt\"")]
    [InlineData("\"C:\\file1.txt  ")]
    [InlineData("'C:\\file1.txt")]
    [InlineData("C:\\file1.txt'")]
    [InlineData("  'C:\\file1.txt")]
    [InlineData("C:\\file1.txt'  ")]
    [InlineData("  C:\\file1.txt'")]
    [InlineData("'C:\\file1.txt  ")]
    [InlineData("'C:\\file1.txt\"")]
    [InlineData("  'C:\\file1.txt\"")]
    [InlineData("'C:\\file1.txt\"  ")]
    [InlineData("  'C:\\file1.txt\"  ")]
    [InlineData("\"C:\\file1.txt'")]
    [InlineData("  \"C:\\file1.txt'")]
    [InlineData("\"C:\\file1.txt'  ")]
    [InlineData("  \"C:\\file1.txt'  ")]
    public void TryParseFromUserInputFailureTests(string value)
    {
      Assert.Equal(null, Path.TryParseFromUserInput(value));
    }

    [Fact]
    public void OperatorSlashTests()
    {
      var path = @"C:\folder1" / new Path(@"folder2/../folder2\folder3") / @"folder4\../folder4\folder5" / new Path(@"file6.txt");

      Assert.Equal(@"C:\folder1\folder2/../folder2\folder3\folder4\../folder4\folder5\file6.txt", path.Value);
      Assert.Equal(@"C:\folder1\folder2\folder3\folder4\folder5\file6.txt", path.FullPath);
    }

    [Fact]
    public void OperatorPlusTests()
    {
      var path = @"C:\folder1" + new Path(@"folder2/../folder2\folder3") + @"folder4\../folder4\folder5" + new Path(@"file6.txt");

      Assert.Equal(@"C:\folder1\folder2/../folder2\folder3\folder4\../folder4\folder5\file6.txt", path.Value);
      Assert.Equal(@"C:\folder1\folder2\folder3\folder4\folder5\file6.txt", path.FullPath);
    }
  }
}
