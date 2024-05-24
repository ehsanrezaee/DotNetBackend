using System.Security.Cryptography;
using System.Text;

namespace ErSoftDev.Common.Utilities;

public static class SecurityHelper
{

    public static EncryptResponse GetMd5(string str, string? salt = null)
    {
        str = Hash64(str);

        salt ??= Salt();
        str += salt;
        var textBytes = Encoding.Default.GetBytes(str);
        try
        {
            var cryptHandler = new MD5CryptoServiceProvider();
            var hash = cryptHandler.ComputeHash(textBytes);
            var ret = "";
            foreach (var a in hash)
            {
                if (a < 16)
                    ret += "0" + a.ToString("x");
                else
                    ret += a.ToString("x");
            }
            return new EncryptResponse() { Salt = salt, EncrypedData = ret };
        }
        catch
        {
            throw new Exception();
        }

    }
    private static string Hash64(string str)
    {
        str = "!#-&'()" + str + "*+,%$./";
        try
        {
            var encDataByte = Encoding.UTF8.GetBytes(str);
            return Convert.ToBase64String(encDataByte);
        }
        catch
        {
            return str;
        }
    }

    public static byte[] aesKey = "AP^sK#XTtn&MtE2+AZV##3jChpv)2wPH"u8.ToArray();
    public static byte[] aesIv = "HDudwQJXSM*e9PhI"u8.ToArray();
    public static EncryptResponse Encrypt(string plaintext)
    {
        if (string.IsNullOrWhiteSpace(plaintext))
            return new EncryptResponse() { };

        var salt = Salt();

        using var aesAlg = Aes.Create();
        aesAlg.Key = aesKey;
        aesAlg.IV = aesIv;
        var encryptor = aesAlg.CreateEncryptor(aesAlg.Key, aesAlg.IV);
        using var msEncrypt = new MemoryStream();
        using (var csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
        {
            var plainBytes = Encoding.UTF8.GetBytes(plaintext + salt);
            csEncrypt.Write(plainBytes, 0, plainBytes.Length);
        }
        var encryptedBytes = msEncrypt.ToArray();
        return new EncryptResponse() { Salt = salt, EncrypedData = Convert.ToBase64String(encryptedBytes) };
    }
    public static string Decrypt(string ciphertext, string salt)
    {
        if (string.IsNullOrWhiteSpace(ciphertext))
            return ciphertext;

        var encryptedByte = Convert.FromBase64String(ciphertext);

        using var aesAlg = Aes.Create();
        aesAlg.Key = aesKey;
        aesAlg.IV = aesIv;
        var decryptor = aesAlg.CreateDecryptor(aesAlg.Key, aesAlg.IV);
        byte[] decryptedBytes;
        using (var msDecrypt = new MemoryStream(encryptedByte))
        {
            using var csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read);
            using var msPlain = new MemoryStream();
            csDecrypt.CopyTo(msPlain);
            decryptedBytes = msPlain.ToArray();
        }
        return Encoding.UTF8.GetString(decryptedBytes).Split(salt)[1];
    }

    public static string StringValidation(string inputStr)
    {
        var mode = "H";
        if (inputStr == null || string.IsNullOrEmpty(inputStr))
            return inputStr;

        inputStr = inputStr.ToLower();
        //For all string 
        inputStr = inputStr.Trim();
        inputStr = inputStr.Replace(",", "_");
        inputStr = inputStr.Replace("'", "_");
        inputStr = inputStr.Replace("\"", "_");
        inputStr = inputStr.Replace("ي", "ی");
        inputStr = inputStr.Replace("ی", "ی");
        inputStr = inputStr.Replace("ك", "ک");
        inputStr = inputStr.Replace("ڪ", "ک");
        inputStr = inputStr.Replace("۱", "1");
        inputStr = inputStr.Replace("۲", "2");
        inputStr = inputStr.Replace("۳", "3");
        inputStr = inputStr.Replace("۴", "4");
        inputStr = inputStr.Replace("۵", "5");
        inputStr = inputStr.Replace("۶", "6");
        inputStr = inputStr.Replace("۷", "7");
        inputStr = inputStr.Replace("۸", "8");
        inputStr = inputStr.Replace("۹", "9");
        inputStr = inputStr.Replace("۰", "0");

        switch (mode)
        {
            case "H":
                inputStr = inputStr.Replace("'", "''");
                inputStr = inputStr.Replace(">", ">");
                inputStr = inputStr.Replace("<", "<");
                inputStr = inputStr.Replace("/*", " ");
                inputStr = inputStr.Replace("*/", " ");
                inputStr = inputStr.ToLower().Replace("script", "BLOCKED");
                inputStr = inputStr.ToLower().Replace("xp_", " ");
                inputStr = inputStr.ToLower().Replace("union", "_union");
                inputStr = inputStr.ToLower().Replace("having", "_having");
                inputStr = inputStr.ToLower().Replace("insert", "_insert");
                inputStr = inputStr.ToLower().Replace("select", "_select");
                inputStr = inputStr.ToLower().Replace("delete", "_delete");
                inputStr = inputStr.ToLower().Replace("update", "_update");
                inputStr = inputStr.ToLower().Replace("drop", "_drop");
                inputStr = inputStr.ToLower().Replace("exec", "_exec");
                inputStr = inputStr.ToLower().Replace("sp_", "_sp_");
                inputStr = inputStr.ToLower().Replace("shutdown", "_shutdown");
                inputStr = inputStr.ToLower().Replace("javascript\\:", "BLOCKED ");
                inputStr = inputStr.ToLower().Replace("vbscript\\:", "BLOCKED ");

                break;
            case "M":
                inputStr = inputStr.Replace("'", "''");
                inputStr = inputStr.ToLower().Replace("script", "BLOCKED");
                inputStr = inputStr.ToLower().Replace("javascript\\:", "BLOCKED ");
                inputStr = inputStr.ToLower().Replace("vbscript\\:", "BLOCKED ");

                break;
            case "L":
                inputStr = inputStr.Replace("'", "''");
                break;
        }

        string functionReturnValue = inputStr;
        return functionReturnValue;
    }

    private static string Salt()
    {
        var randomNumber = new byte[10];
        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(randomNumber);
        return Convert.ToBase64String(randomNumber);
    }

    public class EncryptResponse
    {
        public string Salt { get; set; }
        public string EncrypedData { get; set; }
    }
}
