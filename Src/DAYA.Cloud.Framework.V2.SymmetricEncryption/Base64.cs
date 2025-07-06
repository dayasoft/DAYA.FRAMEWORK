namespace DAYA.Cloud.Framework.V2.SymmetricEncryption;

class Base64 : IBinaryToTextConverter
{
    public byte[] Decode(string text)
    {
        var base64EncodedBytes = Convert.FromBase64String(text);
        return base64EncodedBytes;
    }

    public string Encode(byte[] input)
    {
        return Convert.ToBase64String(input);
    }
}
