using System;
using System.Buffers.Text;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Micro.Net.Extensions
{
    public static class GuidExtensions
    {
        private const byte ForwardSlashByte = (byte)'/';
        private const byte DashByte = (byte)'-';
        private const byte PlusByte = (byte)'+';
        private const byte UnderscoreByte = (byte)'_';

        public static string EncodeBase64String(this Guid guid)
        {
            Span<byte> guidBytes = stackalloc byte[16];
            Span<byte> encodedBytes = stackalloc byte[24];

            MemoryMarshal.TryWrite(guidBytes, ref guid); // write bytes from the Guid
            Base64.EncodeToUtf8(guidBytes, encodedBytes, out _, out _);

            // replace any characters which are not URL safe
            for (var i = 0; i < 22; i++)
            {
                if (encodedBytes[i] == ForwardSlashByte)
                    encodedBytes[i] = DashByte;

                if (encodedBytes[i] == PlusByte)
                    encodedBytes[i] = UnderscoreByte;
            }

            // skip the last two bytes as these will be '==' padding
            var final = Encoding.UTF8.GetString(encodedBytes.Slice(0, 22).ToArray());

            return final;
        }

        public static string EncodeBase85String(this Guid guid)
        {
            return Ascii85.Encode(guid);
        }
    }

    internal static class Ascii85
    {
        /// <summary>
        /// 85 printable ascii characters with no lower case ones, so database 
        /// collation can't bite us. No ' ' character either so database can't 
        /// truncate it!
        /// Unfortunately, these limitation mean resorting to some strange 
        /// characters like 'Æ' but we won't ever have to type these, so it's ok.
        /// </summary>
        private static readonly char[] kEncodeMap = new[]
        {
        '0','1','2','3','4','5','6','7','8','9',  // 10
        'A','B','C','D','E','F','G','H','I','J',  // 20
        'K','L','M','N','O','P','Q','R','S','T',  // 30
        'U','V','W','X','Y','Z','|','}','~','{',  // 40
        '!','"','#','$','%','&','\'','(',')','`', // 50
        '*','+',',','-','.','/','[','\\',']','^', // 60
        ':',';','<','=','>','?','@','_','¼','½',  // 70
        '¾','ß','Ç','Ð','€','«','»','¿','•','Ø',  // 80
        '£','†','‡','§','¥'                       // 85
    };

        /// <summary>
        /// A reverse mapping of the <see cref="kEncodeMap"/> array for decoding 
        /// purposes.
        /// </summary>
        private static readonly IDictionary<char, byte> kDecodeMap;

        /// <summary>
        /// Initialises the <see cref="kDecodeMap"/>.
        /// </summary>
        static Ascii85()
        {
            kDecodeMap = new Dictionary<char, byte>();

            for (byte i = 0; i < kEncodeMap.Length; i++)
            {
                kDecodeMap.Add(kEncodeMap[i], i);
            }
        }

        /// <summary>
        /// Decodes an Ascii-85 encoded Guid.
        /// </summary>
        /// <param name="ascii85Encoding">The Guid encoded using Ascii-85.</param>
        /// <returns>A Guid decoded from the parameter.</returns>
        public static Guid Decode(string ascii85Encoding)
        {
            // Ascii-85 can encode 4 bytes of binary data into 5 bytes of Ascii.
            // Since a Guid is 16 bytes long, the Ascii-85 encoding should be 20
            // characters long.
            if (ascii85Encoding.Length != 20)
            {
                throw new ArgumentException(
                    "An encoded Guid should be 20 characters long.",
                    "ascii85Encoding");
            }

            // We only support upper case characters.
            ascii85Encoding = ascii85Encoding.ToUpper();

            // Split the string in half and decode each substring separately.
            var higher = ascii85Encoding.Substring(0, 10).AsciiDecode();
            var lower = ascii85Encoding.Substring(10, 10).AsciiDecode();

            // Convert the decoded substrings into an array of 16-bytes.
            var byteArray = new[]
            {
            (byte)((higher & 0xFF00000000000000) >> 56),
            (byte)((higher & 0x00FF000000000000) >> 48),
            (byte)((higher & 0x0000FF0000000000) >> 40),
            (byte)((higher & 0x000000FF00000000) >> 32),
            (byte)((higher & 0x00000000FF000000) >> 24),
            (byte)((higher & 0x0000000000FF0000) >> 16),
            (byte)((higher & 0x000000000000FF00) >> 8),
            (byte)((higher & 0x00000000000000FF)),
            (byte)((lower  & 0xFF00000000000000) >> 56),
            (byte)((lower  & 0x00FF000000000000) >> 48),
            (byte)((lower  & 0x0000FF0000000000) >> 40),
            (byte)((lower  & 0x000000FF00000000) >> 32),
            (byte)((lower  & 0x00000000FF000000) >> 24),
            (byte)((lower  & 0x0000000000FF0000) >> 16),
            (byte)((lower  & 0x000000000000FF00) >> 8),
            (byte)((lower  & 0x00000000000000FF)),
        };

            return new Guid(byteArray);
        }

        /// <summary>
        /// Encodes binary data into a plaintext Ascii-85 format string.
        /// </summary>
        /// <param name="guid">The Guid to encode.</param>
        /// <returns>Ascii-85 encoded string</returns>
        public static string Encode(Guid guid)
        {
            // Convert the 128-bit Guid into two 64-bit parts.
            var byteArray = guid.ToByteArray();
            var higher =
                ((UInt64)byteArray[0] << 56) | ((UInt64)byteArray[1] << 48) |
                ((UInt64)byteArray[2] << 40) | ((UInt64)byteArray[3] << 32) |
                ((UInt64)byteArray[4] << 24) | ((UInt64)byteArray[5] << 16) |
                ((UInt64)byteArray[6] << 8) | byteArray[7];

            var lower =
                ((UInt64)byteArray[8] << 56) | ((UInt64)byteArray[9] << 48) |
                ((UInt64)byteArray[10] << 40) | ((UInt64)byteArray[11] << 32) |
                ((UInt64)byteArray[12] << 24) | ((UInt64)byteArray[13] << 16) |
                ((UInt64)byteArray[14] << 8) | byteArray[15];

            var encodedStringBuilder = new StringBuilder();

            // Encode each part into an ascii-85 encoded string.
            encodedStringBuilder.AsciiEncode(higher);
            encodedStringBuilder.AsciiEncode(lower);

            return encodedStringBuilder.ToString();
        }

        /// <summary>
        /// Encodes the given integer using Ascii-85.
        /// </summary>
        /// <param name="encodedStringBuilder">The <see cref="StringBuilder"/> to 
        /// append the results to.</param>
        /// <param name="part">The integer to encode.</param>
        private static void AsciiEncode(
            this StringBuilder encodedStringBuilder, UInt64 part)
        {
            // Nb, the most significant digits in our encoded character will 
            // be the right-most characters.
            var charCount = (UInt32)kEncodeMap.Length;

            // Ascii-85 can encode 4 bytes of binary data into 5 bytes of Ascii.
            // Since a UInt64 is 8 bytes long, the Ascii-85 encoding should be 
            // 10 characters long.
            for (var i = 0; i < 10; i++)
            {
                // Get the remainder when dividing by the base.
                var remainder = part % charCount;

                // Divide by the base.
                part /= charCount;

                // Add the appropriate character for the current value (0-84).
                encodedStringBuilder.Append(kEncodeMap[remainder]);
            }
        }

        /// <summary>
        /// Decodes the given string from Ascii-85 to an integer.
        /// </summary>
        /// <param name="ascii85EncodedString">Decodes a 10 character Ascii-85 
        /// encoded string.</param>
        /// <returns>The integer representation of the parameter.</returns>
        private static UInt64 AsciiDecode(this string ascii85EncodedString)
        {
            if (ascii85EncodedString.Length != 10)
            {
                throw new ArgumentException(
                    "An Ascii-85 encoded Uint64 should be 10 characters long.",
                    "ascii85EncodedString");
            }

            // Nb, the most significant digits in our encoded character 
            // will be the right-most characters.
            var charCount = (UInt32)kEncodeMap.Length;
            UInt64 result = 0;

            // Starting with the right-most (most-significant) character, 
            // iterate through the encoded string and decode.
            for (var i = ascii85EncodedString.Length - 1; i >= 0; i--)
            {
                // Multiply the current decoded value by the base.
                result *= charCount;

                // Add the integer value for that encoded character.
                result += kDecodeMap[ascii85EncodedString[i]];
            }

            return result;
        }
    }
}
