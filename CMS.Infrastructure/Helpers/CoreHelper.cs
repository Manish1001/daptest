namespace CMS.Infrastructure.Helpers
{
    using System.Security.Cryptography;
    using System.Text;
    using System.Text.RegularExpressions;

    public static class CoreHelper
    {
        public static string Replace(string whatToReplace, string replaceWith, string value)
        {
            return Regex.Replace(value, whatToReplace, replaceWith);
        }

        public static string RandomString(int length)
        {
            var random = new Random();
            return new string(Enumerable.Repeat("ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789", length).Select(s => s[random.Next(s.Length)]).ToArray());
        }

        public static int RandomNumber(int min, int max)
        {
            if (min <= 0)
            {
                min = 0;
            }

            if (max <= 0)
            {
                max = 0;
            }

            var randomNumberBuffer = new byte[10];
#pragma warning disable SYSLIB0023
            new RNGCryptoServiceProvider().GetBytes(randomNumberBuffer);
#pragma warning restore SYSLIB0023
            return new Random(BitConverter.ToInt32(randomNumberBuffer, 0)).Next(min, max);
        }

        public static string RandomPassword(int length)
        {
            const int NoOfChunks = 4;
            var finalValue = string.Empty;

            // Initial randoms
            var initialRandoms = new List<int>();
            for (var i = 0; i < NoOfChunks; i++)
            {
                initialRandoms.Add(new Random().Next(1, 3));
            }

            // Padding
            for (;;)
            {
                if (initialRandoms.Sum() >= length)
                {
                    break;
                }

                var indexRandom = new Random().Next(0, NoOfChunks);
                initialRandoms[indexRandom] = initialRandoms[indexRandom] + 1;
            }

            // Capital Letters
            for (var i = 0; i < initialRandoms[0]; i++)
            {
                var capitalLetterNumber = new Random().Next(65, 91);
                var capitalLetter = Convert.ToChar(capitalLetterNumber);
                finalValue += capitalLetter;
            }

            // Small Letters
            for (var i = 0; i < initialRandoms[1]; i++)
            {
                var smallLetterNumber = new Random().Next(97, 123);
                var smallLetter = Convert.ToChar(smallLetterNumber);
                finalValue += smallLetter;
            }

            // Numbers
            for (var i = 0; i < initialRandoms[2]; i++)
            {
                var number = new Random().Next(0, 9);
                finalValue += number;
            }

            // Special Character
            var randomSpecialCharacters = new List<int>
            {
                33, 34, 35, 36, 37, 38, 39, 40, 41, 42, 43, 44, 45, 46, 47, 58, 59, 60, 61, 62, 63, 64, 91, 92, 93, 94,
                95, 96, 123, 124, 125, 126
            };

            for (var i = 0; i < initialRandoms[3]; i++)
            {
                var specialNumber = new Random().Next(0, randomSpecialCharacters.Count);
                var specialLetter = Convert.ToChar(randomSpecialCharacters[specialNumber]);
                finalValue += specialLetter;
            }

            // Jumble
            var jumble = new StringBuilder(finalValue);
            var random = new Random();
            for (var i = jumble.Length - 1; i > 0; i--)
            {
                var j = random.Next(i);
                (jumble[j], jumble[i]) = (jumble[i], jumble[j]);
            }

            return jumble.ToString();
        }

        public static T ToEnum<T>(this string value) => (T)Enum.Parse(typeof(T), value, true);

        public static string FullName(string firstName, string middleName, string lastName) => firstName + " " + (string.IsNullOrEmpty(middleName) ? string.Empty : middleName + " ") + lastName;

        public static byte[] ToImageBytes(string image) => !string.IsNullOrEmpty(image) ? Convert.FromBase64String(Regex.Replace(image, "data:image/(png|jpg|PNG|JPG|JPEG|jpeg|gif|GIF|bmp|BMP);base64,", string.Empty)) : null;

        public static string ToImageString(byte[] image) => image == null ? string.Empty : "data:image/png;base64," + Convert.ToBase64String(image);

        public static string GetYearMonthSequence(string value, string type)
        {
            var year = DateTime.Now.ToString("yy");
            var month = DateTime.Now.ToString("MM");

            if (string.IsNullOrEmpty(value))
            {
                return type + year + month + "0001";
            }

            value = value.Replace(type, string.Empty);
            var lastTxnYear = value.Substring(0, 2);
            var lastTxnMonth = value.Substring(2, 2);
            var counter = Convert.ToInt32(value.Substring(4, value.Length - 4));

            if (lastTxnYear != year && lastTxnMonth != month)
            {
                return type + year + month + "0001";
            }

            return type + year + month + (counter + 1).ToString("D4");
        }
    }
}
