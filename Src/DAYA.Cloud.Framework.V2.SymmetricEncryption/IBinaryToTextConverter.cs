namespace DAYA.Cloud.Framework.V2.SymmetricEncryption;

public interface IBinaryToTextConverter
{
    string Encode(byte[] input);
    byte[] Decode(string text);
}
