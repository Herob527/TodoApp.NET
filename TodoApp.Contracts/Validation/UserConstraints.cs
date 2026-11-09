namespace TodoApp.Contracts.Validation;

public static class UserConstraints
{
    public const string NamePattern = "^[0-9A-Za-z]+$";
    public const int NameMinLength = 3;
    public const int NameMaxLength = 40;
    public const int PasswordMinLength = 8;
    public const int PasswordMaxLength = 40;
}
