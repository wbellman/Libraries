using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Library.Authentication.Models;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace Library.Authentication.Services;

public class TokenService(
    IOptions<JwtSettings> jwtSettings
)
{
    private JwtSettings Settings { get; set; } = jwtSettings.Value;

    public string GenerateJwt(Guid userId, string username, string email)
    {
        var handler = new JwtSecurityTokenHandler();
        
        if(string.IsNullOrWhiteSpace(Settings.SecretKey))
            throw new Exception("JWT Secret Key is not set");
        
        var key = Encoding.UTF8.GetBytes(Settings.SecretKey);
        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, userId.ToString()),
            new Claim(JwtRegisteredClaimNames.UniqueName, username),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new Claim(ClaimTypes.NameIdentifier, userId.ToString()),
            new Claim(ClaimTypes.Email, email)
        };

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            Expires = DateTime.UtcNow.AddMinutes(Settings.ExpiryMinutes),
            Issuer = Settings.Issuer,
            Audience = Settings.Audience,
            SigningCredentials = new SigningCredentials(
                new SymmetricSecurityKey(key),
                SecurityAlgorithms.HmacSha256Signature
            )
        };

        var token = handler.CreateToken(tokenDescriptor);
        return handler.WriteToken(token);
    }

    public CredentialToken Create(string password)
    {
        // Generate a random salt
        var salt = GenerateSalt(32);

        // Derive key using PBKDF2
        var key = DeriveKeyFromPassword(password, salt);

        // Encrypt the derived key using AES
        var encryptedKey = EncryptKey(key);

        // Create a CredentialToken containing the Base64 encoded salt, IV, and encrypted key
        return new CredentialToken(
            Salt: Convert.ToBase64String(salt),
            Vector: Convert.ToBase64String(encryptedKey.iv),
            EncryptedToken: Convert.ToBase64String(encryptedKey.cipherText)
        );
    }

    public bool CheckPassword(string password, CredentialToken token)
    {
        // Decode the salt and IV from the stored token
        var salt = Convert.FromBase64String(token.Salt);
        var iv = Convert.FromBase64String(token.Vector);
        var encryptedToken = Convert.FromBase64String(token.EncryptedToken);

        // Derive the key from the password and salt
        var derivedKey = DeriveKeyFromPassword(password, salt);

        // Re-encrypt the derived key using the same IV
        var encryptedKey = EncryptKeyWithIv(derivedKey, iv);

        // Compare the re-encrypted key with the stored encrypted key
        return CompareByteArrays(encryptedKey.cipherText, encryptedToken);
    }

    private static byte[] GenerateSalt(int size)
    {
        var salt = new byte[size];
        RandomNumberGenerator.Fill(salt);
        return salt;
    }

    private static byte[] DeriveKeyFromPassword(string password, byte[] salt, int iterations = 10000,
        int keySize = 32)
    {
        using var pbkdf2 = new Rfc2898DeriveBytes(
            password, salt, iterations, HashAlgorithmName.SHA256
        );

        return pbkdf2.GetBytes(keySize);
    }

    private static (byte[] iv, byte[] cipherText) EncryptKey(byte[] key)
    {
        using var aes = Aes.Create();

        aes.KeySize = 256;
        aes.BlockSize = 128;
        aes.Mode = CipherMode.CBC;
        aes.Padding = PaddingMode.PKCS7;

        aes.GenerateIV();

        using var encryptor = aes.CreateEncryptor(key, aes.IV);
        using var ms = new MemoryStream();

        using (var cs = new CryptoStream(ms, encryptor, CryptoStreamMode.Write))
        {
            cs.Write(key, 0, key.Length);
            cs.FlushFinalBlock();
        }

        return (aes.IV, ms.ToArray());
    }

    private static (byte[] iv, byte[] cipherText) EncryptKeyWithIv(byte[] key, byte[] iv)
    {
        using var aes = Aes.Create();

        aes.KeySize = 256;
        aes.BlockSize = 128;
        aes.Mode = CipherMode.CBC;
        aes.Padding = PaddingMode.PKCS7;

        using var encryptor = aes.CreateEncryptor(key, iv);
        using var ms = new MemoryStream();

        using (var cs = new CryptoStream(ms, encryptor, CryptoStreamMode.Write))
        {
            cs.Write(key, 0, key.Length);
            cs.FlushFinalBlock();
        }

        return (iv, ms.ToArray());
    }

    private static bool CompareByteArrays(byte[] array1, byte[] array2)
    {
        if (array1.Length != array2.Length)
            return false;

        return !array1
            .Where((t, i) => t != array2[i])
            .Any();
    }
}