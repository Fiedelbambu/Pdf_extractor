using System;
using System.Security.Cryptography;
using System.Text;

class Program
{
    public static void Main(string[] args)
    {
        // Dein echter Connection-String
        var connectionString = "";
        // Verschlüsseln und speichern
        EncryptAndStoreConnectionString(connectionString);
        Console.WriteLine("Connection-String wurde verschlüsselt und gespeichert.");
    }

    public static void EncryptAndStoreConnectionString(string connectionString)
    {
        byte[] connectionStringBytes = Encoding.UTF8.GetBytes(connectionString);

        // Verschlüsselung mit DPAPI
        byte[] encryptedBytes = ProtectedData.Protect(connectionStringBytes, null, DataProtectionScope.CurrentUser);

        // Speichern in einer Datei
        System.IO.File.WriteAllBytes("encryptedConnectionString.dat", encryptedBytes);
    }
}
