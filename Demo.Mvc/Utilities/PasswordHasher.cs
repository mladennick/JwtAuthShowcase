using System.Security.Cryptography;
using System.Text;

namespace Demo.Mvc.Utilities;

public static class PasswordHasher
{
    public static string Hash(string value)
    {
        var bytes = SHA256.HashData(Encoding.UTF8.GetBytes(value));
        return Convert.ToHexString(bytes);
    }
}
