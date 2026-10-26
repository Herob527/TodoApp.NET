using Microsoft.AspNetCore.Identity;

public interface IPasswordHelper
{
    string HashPassword(string userName, string password);
    bool VerifyPassword(string userName, string hashedPassword, string password);
}

public class PasswordHelper(IPasswordHasher<string> _passwordHasher) : IPasswordHelper
{
    public string HashPassword(string userName, string password)
    {
        return _passwordHasher.HashPassword(userName, password);
    }

    public bool VerifyPassword(string userName, string hashedPassword, string password)
    {
        var result = _passwordHasher.VerifyHashedPassword(userName, hashedPassword, password);
        // if required, you can handle if result is SuccessRehashNeeded
        return result == PasswordVerificationResult.Success;
    }
}
