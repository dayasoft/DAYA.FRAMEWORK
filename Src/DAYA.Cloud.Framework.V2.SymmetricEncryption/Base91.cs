using System.Text;

namespace DAYA.Cloud.Framework.V2.SymmetricEncryption;

class Base91 : IBinaryToTextConverter
{
    public string Encode(byte[] input)
    {
        return Encode(input, -1);
    }

    public byte[] Decode(string text)
    {
        string s = text;
        if (s == null)
        {
            throw new ArgumentNullException(nameof(s));
        }

        int dbq = 0, dn = 0, dv = -1;
        List<byte> data = new();
        for (int i = 0; i < s.Length; ++i)
        {
            if (dectab[s[i]] == 91)
            {
                continue;
            }
            if (dv == -1)
            {
                dv = dectab[s[i]];
            }
            else
            {
                dv += dectab[s[i]] * 91;
                dbq |= dv << dn;
                dn += (dv & 8191) > 88 ? 13 : 14;
                do
                {
                    data.Add((byte)dbq);
                    dbq >>= 8;
                    dn -= 8;
                } while (dn > 7);
                dv = -1;
            }
        }
        if (dv != -1)
        {
            data.Add((byte)(dbq | dv << dn));
        }
        return data.ToArray();
    }


    private static readonly char[] enctab = new char[] {
            'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'I', 'J', 'K', 'L', 'M',
            'N', 'O', 'P', 'Q', 'R', 'S', 'T', 'U', 'V', 'W', 'X', 'Y', 'Z',
            'a', 'b', 'c', 'd', 'e', 'f', 'g', 'h', 'i', 'j', 'k', 'l', 'm',
            'n', 'o', 'p', 'q', 'r', 's', 't', 'u', 'v', 'w', 'x', 'y', 'z',
            '0', '1', '2', '3', '4', '5', '6', '7', '8', '9', '!', '#', '$',
            '%', '&', '(', ')', '*', '+', ',', '.', '/', ':', ';', '<', '=',
            '>', '?', '@', '[', ']', '^', '_', '`', '{', '|', '}', '~', '"'
        };

    private static readonly byte[] dectab = new byte[]
    {
            91, 91, 91, 91, 91, 91, 91, 91, 91, 91, 91, 91, 91, 91, 91, 91,
            91, 91, 91, 91, 91, 91, 91, 91, 91, 91, 91, 91, 91, 91, 91, 91,
            91, 62, 90, 63, 64, 65, 66, 91, 67, 68, 69, 70, 71, 91, 72, 73,
            52, 53, 54, 55, 56, 57, 58, 59, 60, 61, 74, 75, 76, 77, 78, 79,
            80,  0,  1,  2,  3,  4,  5,  6,  7,  8,  9, 10, 11, 12, 13, 14,
            15, 16, 17, 18, 19, 20, 21, 22, 23, 24, 25, 81, 91, 82, 83, 84,
            85, 26, 27, 28, 29, 30, 31, 32, 33, 34, 35, 36, 37, 38, 39, 40,
            41, 42, 43, 44, 45, 46, 47, 48, 49, 50, 51, 86, 87, 88, 89, 91,
            91, 91, 91, 91, 91, 91, 91, 91, 91, 91, 91, 91, 91, 91, 91, 91,
            91, 91, 91, 91, 91, 91, 91, 91, 91, 91, 91, 91, 91, 91, 91, 91,
            91, 91, 91, 91, 91, 91, 91, 91, 91, 91, 91, 91, 91, 91, 91, 91,
            91, 91, 91, 91, 91, 91, 91, 91, 91, 91, 91, 91, 91, 91, 91, 91,
            91, 91, 91, 91, 91, 91, 91, 91, 91, 91, 91, 91, 91, 91, 91, 91,
            91, 91, 91, 91, 91, 91, 91, 91, 91, 91, 91, 91, 91, 91, 91, 91,
            91, 91, 91, 91, 91, 91, 91, 91, 91, 91, 91, 91, 91, 91, 91, 91,
            91, 91, 91, 91, 91, 91, 91, 91, 91, 91, 91, 91, 91, 91, 91, 91
    };

    private string Encode(byte[] ib, int count)
    {
        if (ib == null)
        {
            throw new ArgumentNullException(nameof(ib));
        }

        if (count > ib.Length)
        {
            throw new ArgumentOutOfRangeException(nameof(count));
        }

        if (count == -1)
            count = ib.Length;
        int ebq = 0, en = 0;
        StringBuilder sb = new StringBuilder();
        for (int i = 0; i < count; ++i)
        {
            ebq |= (ib[i] & 255) << en;
            en += 8;
            if (en > 13)
            {
                int ev = ebq & 8191;

                if (ev > 88)
                {
                    ebq >>= 13;
                    en -= 13;
                }
                else
                {
                    ev = ebq & 16383;
                    ebq >>= 14;
                    en -= 14;
                }
                sb.Append(enctab[ev % 91]);
                sb.Append(enctab[ev / 91]);
            }
        }
        if (en > 0)
        {
            sb.Append(enctab[ebq % 91]);
            if (en > 7 || ebq > 90)
                sb.Append(enctab[ebq / 91]);
        }
        return sb.ToString();
    }
}
