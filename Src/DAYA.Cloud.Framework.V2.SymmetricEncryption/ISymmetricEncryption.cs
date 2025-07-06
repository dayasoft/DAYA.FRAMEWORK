namespace DAYA.Cloud.Framework.V2.SymmetricEncryption;

public interface ISymmetricEncryption
{
    string Encrypt(string value);
    string Encrypt(byte[] value);
    string Decrypt(byte[] value);
    byte[] Decrypt(string encrypted);
}
