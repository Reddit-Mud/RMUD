using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Akkoteaque
{
    public class Cipher
    {
        static String[] LetterEncodings = new String[] 
        {
            "──┐", "──┘", "──┤", "─┬┐", "─┬┘", "─┬┤", "─┴┐", "─┴┘", "─┴┤", "─┼┐", "─┼┘", "─┼┤", "┬─┐", "┬─┘", "┬─┤", "┬┬┐", "┬┬┘", "┬┬┤", "┬┴┐", "┬┴┘", "┬┴┤", "┬┼┐", "┬┼┘", "┬┼┤", "┴─┐", "┴─┘"
        };

        static String[] PunctuationEncodings = new String[] { "►──", "►┼┤", "►─┐" };

        static Dictionary<char, string> Encodings = null;

        static string GetEncodingSequence(char c)
        {
            if (Encodings == null)
            {
                Encodings = new Dictionary<char, string>();
                for (char x = 'a'; x <= 'z'; ++x)
                    Encodings.Add(x, LetterEncodings[x - 'a']);
                Encodings.Add('.', PunctuationEncodings[0]);
                Encodings.Add('!', PunctuationEncodings[1]);
                Encodings.Add('?', PunctuationEncodings[2]);
            }

            if (c >= 'A' && c <= 'Z')
                return ExtendLastCharacter(Encodings[(char)((c - 'A') + 'a')]) + "◄";
            else if (!Encodings.ContainsKey(c)) return "";
            return Encodings[c] + " ";
        }

        static string ExtendLastCharacter(string s)
        {
            if (String.IsNullOrEmpty(s)) return s;
            if (s.Last() == '┐')
                return s.Substring(0, s.Length - 1) + "┬";
            else if (s.Last() == '┤')
                return s.Substring(0, s.Length - 1) + "┼";
            else if (s.Last() == '┘')
                return s.Substring(0, s.Length - 1) + "┴";
            else
                return s;
        }

        static string EncodeString(string str)
        {
            var prevSpace = true;
            var nextSpace = true;
            var r = "";
            for (int i = 0; i < str.Length; ++i)
            {
                if (str[i] == ' ')
                {
                    prevSpace = true;
                    r += "     ";
                }
                else
                {
                    nextSpace = (i == str.Length - 1) || (str[i + 1] == ' ');
                    if (prevSpace && nextSpace) r += "►";
                    else if (prevSpace) r += "┌";
                    else if (nextSpace) r += "└";
                    else r += "├";
                    prevSpace = false;
                    r += GetEncodingSequence(str[i]);
                }
            }
            return r;
        }

        public static string EncodeParagraph(params string[] Segments)
        {
            var encodedSegments = Segments.Select(s => EncodeString(s));
            var r = "";
            var rows = encodedSegments.Max(s => s.Length);
            for (int i = 0; i < rows; i += 5)
            {
                foreach (var s in encodedSegments)
                {
                    if (s.Length <= i) r += "      ";
                    else r += s.Substring(i, 5) + " ";
                }
                r += "\n";
            }
            return r;
        }
    }
}