using System.Text;

namespace DAYA.Cloud.Framework.V2.SymmetricEncryption;

class Base85 : IBinaryToTextConverter
{
    public int LineLength = int.MaxValue;

    private const int _asciiOffset = 33;

    private readonly uint[] pow85 = { 85 * 85 * 85 * 85, 85 * 85 * 85, 85 * 85, 85, 1 };

    public byte[] Decode(string s)
    {
        byte[] _decodedBlock = new byte[4];
        uint _tuple = 0;
        MemoryStream ms = new();
        int count = 0;
        foreach (char c in s)
        {
            bool processChar;
            switch (c)
            {
                case 'z':
                    if (count != 0)
                    {
                        throw new Exception("The character 'z' is invalid inside an ASCII85 block.");
                    }
                    _decodedBlock[0] = 0;
                    _decodedBlock[1] = 0;
                    _decodedBlock[2] = 0;
                    _decodedBlock[3] = 0;
                    ms.Write(_decodedBlock, 0, 4);
                    processChar = false;
                    break;
                case '\n':
                case '\r':
                case '\t':
                case '\0':
                case '\f':
                case '\b':
                    processChar = false;
                    break;
                default:
                    if (c < '!' || c > 'u')
                    {
                        throw new Exception("Bad character '" + c + "' found. ASCII85 only allows characters '!' to 'u'.");
                    }
                    processChar = true;
                    break;
            }

            if (processChar)
            {
                _tuple += (uint)(c - _asciiOffset) * pow85[count];
                count++;
                if (count == 5)
                {
                    DecodeBlock(_tuple, _decodedBlock);
                    ms.Write(_decodedBlock, 0, 4);
                    _tuple = 0;
                    count = 0;
                }
            }
        }

        // if we have some bytes left over at the end..
        if (count != 0)
        {
            if (count == 1)
            {
                //throw new Exception("The last block of ASCII85 data cannot be a single byte.");
            }
            count--;
            _tuple += pow85[count];
            DecodeBlock(count, _tuple, _decodedBlock);
            for (int i = 0; i < count; i++)
            {
                ms.WriteByte(_decodedBlock[i]);
            }
        }

        return ms.ToArray();
    }

    public string Encode(byte[] ba)
    {
        byte[] _encodedBlock = new byte[5];
        StringBuilder sb = new StringBuilder(ba.Length * (5 / 4));

        int count = 0;
        uint _tuple = 0;
        foreach (byte b in ba)
        {
            if (count >= 3)
            {
                _tuple |= b;
                if (_tuple == 0)
                {
                    AppendChar(sb, 'z');
                }
                else
                {
                    EncodeBlock(sb, _tuple, _encodedBlock);
                }
                _tuple = 0;
                count = 0;
            }
            else
            {
                _tuple |= (uint)(b << 24 - count * 8);
                count++;
            }
        }

        if (count > 0)
        {
            EncodeBlock(count + 1, sb, _tuple, _encodedBlock);
        }

        return sb.ToString();
    }

    private void EncodeBlock(StringBuilder sb, uint _tuple, byte[] _encodedBlock)
    {
        EncodeBlock(5, sb, _tuple, _encodedBlock);
    }

    private void EncodeBlock(int count, StringBuilder sb, uint _tuple, byte[] _encodedBlock)
    {
        for (int i = 4; i >= 0; i--)
        {
            _encodedBlock[i] = (byte)(_tuple % 85 + _asciiOffset);
            _tuple /= 85;
        }

        for (int i = 0; i < count; i++)
        {
            char c = (char)_encodedBlock[i];
            AppendChar(sb, c);
        }

    }

    private void DecodeBlock(uint _tuple, byte[] _decodedBlock)
    {
        DecodeBlock(4, _tuple, _decodedBlock);
    }

    private void DecodeBlock(int bytes, uint _tuple, byte[] _decodedBlock)
    {
        for (int i = 0; i < bytes; i++)
        {
            _decodedBlock[i] = (byte)(_tuple >> 24 - i * 8);
        }
    }

    private void AppendChar(StringBuilder sb, char c)
    {
        sb.Append(c);
    }
}
