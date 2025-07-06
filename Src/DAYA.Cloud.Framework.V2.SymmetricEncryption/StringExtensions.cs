namespace DAYA.Cloud.Framework.V2.SymmetricEncryption;

internal static class StringExtensions
{
    public static byte[] ToByteArray(this string hex)
    {
        int length = hex.Length;
        byte[] bytes = new byte[length >> 1];
        for (int i = 0, j = 0; i < length; i += 2, j++)
        {
            bytes[j] = Convert.ToByte(hex.Substring(i, 2), 16);
        }
        return bytes;
    }

    public static IEnumerable<string> SplitInParts(this string s, int partLength)
    {
        if (s == null)
        {
            throw new ArgumentNullException(nameof(s));
        }
        if (partLength <= 0)
        {
            throw new ArgumentException("Part length has to be positive.", nameof(partLength));
        }

        for (var i = 0; i < s.Length; i += partLength)
        {
            yield return s.Substring(i, Math.Min(partLength, s.Length - i));
        }
    }
}
