using System;
using System.Text;

namespace NanBoxing
{
    class Program
    {
        static void Main(string[] args)
        {
            // Byte conversion
            var simpleNan = BitConverter.GetBytes(double.NaN);
            var one = BitConverter.GetBytes(1.0d);

            Console.WriteLine(StringOf(simpleNan));
            Console.WriteLine(StringOf(one));

            var randomDouble = BitConverter.ToDouble(new byte[]{ 0x0F,0x0F,0x0F,0x0F,0x0F,0x0F,0x0F,0x0F  }, 0);
            var zero = BitConverter.ToDouble(new byte[]{ 0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00  }, 0);
            var byteNan = BitConverter.ToDouble(new byte[]{ 0x00,0x00,0x00,0x00,0x00,0x00,0xF8,0x7F  }, 0);

            Console.WriteLine(randomDouble);
            Console.WriteLine(zero);
            Console.WriteLine(byteNan + " " + double.IsNaN(byteNan));


            const ulong NAN_FLAG =   0x7FF8000000000000; // Bits to make a quiet NaN
            const ulong UPPER_FOUR = 0x8007000000000000; // mask for top 4 available bits
            const ulong LOWER_32 =   0x00000000FFFFFFFF; // low 32 bits
            const ulong LOWER_51 =   0x0007FFFFFFFFFFFF; // all data bits available
            const ulong LOWER_48 =   0x0000FFFFFFFFFFFF; // 48 bits for pointers

            // Int64 conversion
            var whatevs = BitConverter.DoubleToInt64Bits(Math.PI);
            var longNan = BitConverter.Int64BitsToDouble(0x7FF8000000000000); // big-endian


            Console.WriteLine("d " + whatevs + "; nan: " + longNan);


            Console.WriteLine("Done. Press [Enter]");
            Console.ReadLine();
        }

        private static string StringOf(byte[] bytes)
        {
            var sb = new StringBuilder();

            for (int i = 0; i < bytes.Length; i++)
            {
                for (int j = 0; j < 8; j++)
                {
                    if (((bytes[i] >> j) & 1) == 0) {
                        sb.Append('0');
                    } else sb.Append('1');
                }
                sb.Append(' ');
            }

            return sb.ToString();
        }
    }
}
