using System.Security.Cryptography;

namespace DAYA.Cloud.Framework.V2.SymmetricEncryption;

class SymmetricEncryption : ISymmetricEncryption
{
    private readonly SymmetricAlgorithmConfig _symmetricAlgorithmConfig;
    private readonly IBinaryToTextConverter _binaryToText;
    private readonly SymmetricAlgorithm _symmetricAlgorithm;

    public SymmetricEncryption(
        SymmetricAlgorithmConfig symmetricAlgorithmConfig,
        IBinaryToTextConverter binaryToText,
        SymmetricAlgorithm symmetricAlgorithm)
    {
        _symmetricAlgorithmConfig = symmetricAlgorithmConfig;
        _binaryToText = binaryToText;
        _symmetricAlgorithm = symmetricAlgorithm;
    }

    public string Encrypt(string value)
    {
        var resultArray = Transform(value, CryptoType.Encrypt);
        return _binaryToText.Encode(resultArray);
    }

    public byte[] Decrypt(string value)
    {
        var resultArray = Transform(value, CryptoType.Decrypt);
        return resultArray;
    }

    public string Encrypt(byte[] value)
    {
        var encryptedBytes = TransformBlock(value, CryptoType.Encrypt);
        return _binaryToText.Encode(encryptedBytes);
    }

    public string Decrypt(byte[] value)
    {
        var decryptedBytes = TransformBlock(value, CryptoType.Decrypt);
        return _binaryToText.Encode(decryptedBytes);
    }

    private byte[] Transform(string value, CryptoType cryptoType)
    {
        byte[] inputBuffer = GetInputBuffer(value);
        return TransformBlock(inputBuffer, cryptoType);
    }

    private byte[] TransformBlock(byte[] inputBuffer, CryptoType cryptoType)
    {
        var key = _symmetricAlgorithmConfig.HexKey;
        var symmetricAlgorithm = _symmetricAlgorithm;
        symmetricAlgorithm.Key = key.ToByteArray();
        symmetricAlgorithm.Mode = _symmetricAlgorithmConfig.CipherMode;
        symmetricAlgorithm.Padding = _symmetricAlgorithmConfig.PaddingMode;

        ICryptoTransform cTransform = GetICryptoTransform(symmetricAlgorithm, cryptoType);
        byte[] resultArray = cTransform.TransformFinalBlock(inputBuffer, 0, inputBuffer.Length);
        symmetricAlgorithm.Clear();
        return resultArray;
    }

    private byte[] GetInputBuffer(string value)
    {
        return _binaryToText.Decode(value);
    }

    private static ICryptoTransform GetICryptoTransform(SymmetricAlgorithm symmetricAlgorithm, CryptoType cryptoType)
    {
        return cryptoType == CryptoType.Encrypt ? symmetricAlgorithm.CreateEncryptor() : symmetricAlgorithm.CreateDecryptor();
    }
}
