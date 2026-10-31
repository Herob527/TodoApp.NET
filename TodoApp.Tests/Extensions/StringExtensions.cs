namespace TodoApp.Tests.Extensions;
public static class StringExtensions
{

  private const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
  private static readonly Random Random = Random.Shared;

  extension(string) {
    public static string RandomText(int length) {
        return new(
            [.. Enumerable.Repeat(chars, length).Select(s => s[Random.Next(s.Length)])]
        );
    }

  }
}


