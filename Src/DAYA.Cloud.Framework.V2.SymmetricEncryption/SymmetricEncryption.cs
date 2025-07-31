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

        byte[] resultArray;

        if (cryptoType == CryptoType.Encrypt)
        {
            // For encryption, generate a random IV
            symmetricAlgorithm.GenerateIV();
            ICryptoTransform encryptor = symmetricAlgorithm.CreateEncryptor();
            byte[] encryptedData = encryptor.TransformFinalBlock(inputBuffer, 0, inputBuffer.Length);
            
            // Prepend IV to encrypted data for storage/transmission
            resultArray = new byte[symmetricAlgorithm.IV.Length + encryptedData.Length];
            Buffer.BlockCopy(symmetricAlgorithm.IV, 0, resultArray, 0, symmetricAlgorithm.IV.Length);
            Buffer.BlockCopy(encryptedData, 0, resultArray, symmetricAlgorithm.IV.Length, encryptedData.Length);
        }
        else
        {
            // For decryption, extract IV from the beginning of the data
            int ivLength = symmetricAlgorithm.BlockSize / 8; // Block size is in bits, convert to bytes
            if (inputBuffer.Length < ivLength)
            {
                throw new ArgumentException("Invalid encrypted data: insufficient length for IV");
            }

            // Extract IV
            byte[] iv = new byte[ivLength];
            Buffer.BlockCopy(inputBuffer, 0, iv, 0, ivLength);
            symmetricAlgorithm.IV = iv;

            // Extract encrypted data
            byte[] encryptedData = new byte[inputBuffer.Length - ivLength];
            Buffer.BlockCopy(inputBuffer, ivLength, encryptedData, 0, encryptedData.Length);

            ICryptoTransform decryptor = symmetricAlgorithm.CreateDecryptor();
            resultArray = decryptor.TransformFinalBlock(encryptedData, 0, encryptedData.Length);
        }

        symmetricAlgorithm.Clear();
        return resultArray;
    }

    private byte[] GetInputBuffer(string value)
    {
        return _binaryToText.Decode(value);
    }

}
