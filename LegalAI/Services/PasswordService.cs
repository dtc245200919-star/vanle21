using System.Security.Cryptography;
namespace LegalAI.Services;
public static class PasswordService
{
    public static string Hash(string password)
    {
        var salt = RandomNumberGenerator.GetBytes(16);
        var hash = Rfc2898DeriveBytes.Pbkdf2(password, salt, 120000, HashAlgorithmName.SHA256, 32);
        return $"120000.{Convert.ToBase64String(salt)}.{Convert.ToBase64String(hash)}";
    }
    public static bool Verify(string password, string encoded)
    {
        try { var p=encoded.Split('.'); var hash=Rfc2898DeriveBytes.Pbkdf2(password,Convert.FromBase64String(p[1]),int.Parse(p[0]),HashAlgorithmName.SHA256,32); return CryptographicOperations.FixedTimeEquals(hash,Convert.FromBase64String(p[2])); } catch { return false; }
    }
}
