using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace WPF_LoggerTray
{

    public class LibKey
    {
        static string base10ToBase26(string s)
        {
            char[] allowedLetters = "ABCDEFGHIJKLMNOPQRSTUVWXYZ".ToCharArray();

            decimal num = Convert.ToDecimal(s);
            int reminder = 0;

            char[] result = new char[s.ToString().Length + 1];
            int j = 0;


            while ((num >= 26))
            {
                reminder = Convert.ToInt32(num % 26);
                result[j] = allowedLetters[reminder];
                num = (num - reminder) / 26;
                j += 1;
            }

            result[j] = allowedLetters[Convert.ToInt32(num)];

            string returnNum = "";

            for (int k = j; k >= 0; k -= 1)
            {
                returnNum += result[k];
            }
            return returnNum;

        }
        static string base26ToBase10(string s)
        {
            string allowedLetters = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
            System.Numerics.BigInteger result = new System.Numerics.BigInteger();

            for (int i = 0; i <= s.Length - 1; i += 1)
            {
                BigInteger pow = powof(26, (s.Length - i - 1));
                result = result + allowedLetters.IndexOf(s.Substring(i, 1)) * pow;
            }
            return result.ToString();
        }
        static BigInteger powof(int x, int y)
        {
            BigInteger newNum = 1;

            if (y == 0)
            {
                return 1;
            }
            else if (y == 1)
            {
                return x;
            }
            else
            {
                for (int i = 0; i <= y - 1; i++)
                {
                    newNum = newNum * x;
                }
                return newNum;
            }
        }
        static int modulo(long _num, long _base)
        {
            return (int)(_num - _base * Convert.ToInt64(Math.Floor((decimal)_num / (decimal)_base)));
        }

        public static string CreateKey(Func<int, long> f, int mod)
        {
            System.Security.Cryptography.RNGCryptoServiceProvider rng = new System.Security.Cryptography.RNGCryptoServiceProvider();
            byte[] rndBytes = new byte[4];
            rng.GetBytes(rndBytes);
            int rand = modulo(BitConverter.ToInt32(rndBytes, 0), mod);
            int key = modulo(f(rand), mod);

            rng.GetBytes(rndBytes);
            int rand2 = modulo(BitConverter.ToInt32(rndBytes, 0), mod);
            int key2 = modulo(f(rand2), mod);

            rng.GetBytes(rndBytes);
            int rand3 = modulo(BitConverter.ToInt32(rndBytes, 0), mod);
            int key3 = modulo(f(rand3), mod);

            decimal outputData = 1; //this could've been 0 too, however, in that case, we would need
                                    //to take this into consideration when the key is deciphered (the length)

            outputData *= (decimal)Math.Pow(10, mod.ToString().Length);
            outputData += rand;
            outputData *= (decimal)Math.Pow(10, mod.ToString().Length); //maybe need a one somewhere to fill up the space
            outputData += key;
            outputData *= (decimal)Math.Pow(10, mod.ToString().Length);

            outputData += rand2;
            outputData *= (decimal)Math.Pow(10, mod.ToString().Length);
            outputData += key2;
            outputData *= (decimal)Math.Pow(10, mod.ToString().Length);

            outputData += rand3;
            outputData *= (decimal)Math.Pow(10, mod.ToString().Length);
            outputData += key3;

            string output = base10ToBase26(outputData.ToString());

            return output;
        }

        public static bool ValidateKey(string key, Func<int, long> f, int mod)
        {
            string base10 = base26ToBase10(key);
            int modLength = mod.ToString().Length;

            for (int i = 0; i < 3; i++)
            {
                if (modulo(f(Convert.ToInt32(base10.Substring(1, modLength))), mod) == Convert.ToInt32(base10.Substring(modLength + 1, modLength)))
                {
                    base10 = base10.Substring(2 * modLength);
                }
                else
                {
                    return false;
                }
            }

            return true;
        }

        static decimal maxModValue()
        {
            //this is the maximum length of mod variable considering we
            //have 3 points (1 point = 2 values).
            return (decimal.MaxValue.ToString().Length - 1) / 6;
        }
    }
}
