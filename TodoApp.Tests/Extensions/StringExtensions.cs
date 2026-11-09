using System.Text;

namespace TodoApp.Tests.Extensions;
public static class StringExtensions
{

  private const string uppercase = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
  private const string lowercase = "abcdefghijklmnopqrstuvwxyz";
  private const string numbers =  "0123456789";
  private const string special = "!@#$%^*()?";

  // Username may not contain special characters, so they're omitted there
  private const string chars = uppercase + lowercase + numbers;

  private static readonly Random Random = Random.Shared;

  extension(string) {
    public static string RandomText(int length) {
        if (length <= 0) return "";
        return new(
            [.. Enumerable.Repeat(chars, length).Select(s => s[Random.Next(s.Length)])]
        );
    }

    public static string RandomPassword(int length) {

      if (length <= 0) return "";

      var res = new StringBuilder(length);
      for (int index = 0; length > index; index++) {
        string collection = index switch {
            _ when index % 5 == 0 => special,
            _ when index % 4 == 0 => numbers,
            _ when index % 3 == 0 => lowercase,
            _ when index % 2 == 0 => uppercase,
            _ => uppercase
        };
        var item = collection[Random.Next(collection.Length)];
        res = res.Append(item);
      }
      return res.ToString();
    }

  }
}


