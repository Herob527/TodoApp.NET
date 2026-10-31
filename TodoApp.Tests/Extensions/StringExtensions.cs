namespace TodoApp.Tests.Extensions;
public static class StringExtensions
{

  const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";

  extension(string) {

    public static string RandomText(int length) {

        var random = new Random();

        return new(
            [.. Enumerable.Repeat(chars, length).Select(s => s[random.Next(s.Length)])]
        );
    }

  }
}


