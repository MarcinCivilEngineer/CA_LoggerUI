using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CA_LoggerUI
{
    public class LibSystem
    {
        public static string CreateFolderToStore(string folder)
        {
            string path = Directory.GetCurrentDirectory();
            if (!Directory.Exists(folder))
            {
                Directory.CreateDirectory(folder);
            }
            return path + "\\" + folder + "\\";
            //return PathDirectory = folder;
        }

        public string ConvertAsciiToUTF8(string input)
        {

            string unicodeString = input;

            // Create two different encodings.
            Encoding ascii = Encoding.Default;
            Encoding unicode = Encoding.UTF8;

            // Convert the string into a byte array.
            byte[] unicodeBytes = unicode.GetBytes(unicodeString);

            // Perform the conversion from one encoding to the other.
            byte[] asciiBytes = Encoding.Convert(unicode, ascii, unicodeBytes);

            // Convert the new byte[] into a char[] and then into a string.
            char[] asciiChars = new char[ascii.GetCharCount(asciiBytes, 0, asciiBytes.Length)];
            ascii.GetChars(asciiBytes, 0, asciiBytes.Length, asciiChars, 0);
            string asciiString = new string(asciiChars);

            return asciiString;
        }


        public string ScrubFileName(string value)
        {
            var sb = new StringBuilder(value);
            foreach (char item in Path.GetInvalidFileNameChars())
            {
                sb.Replace(item.ToString(), "");
            }
            return sb.ToString();
        }

    }



}
