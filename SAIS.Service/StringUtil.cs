using System;
using System.Collections.Generic;
using System.Text;

namespace SAIS.Service
{
    public static class StringUtil
    {
        public static string Truncate(this string value, int maxLength)
        {
            return value == null || value.Length <= maxLength ? value : value.Substring(0, maxLength);
        }

        public static string TransliterateCyrToLat(this string s)
        {
            if (s == null)
            {
                return null;
            }
            StringBuilder sb = new StringBuilder();

            Dictionary<char, string> corrTable = new Dictionary<char, string>()
            {
                { 'а', "a" },
                { 'б', "b" },
                { 'в', "v" },
                { 'г', "g" },
                { 'д', "d" },
                { 'е', "e" },
                { 'ж', "zh" },
                { 'з', "z" },
                { 'и', "i" },
                { 'й', "y" },
                { 'к', "k" },
                { 'л', "l" },
                { 'м', "m" },
                { 'н', "n" },
                { 'о', "o" },
                { 'п', "p" },
                { 'р', "r" },
                { 'с', "s" },
                { 'т', "t" },
                { 'у', "u" },
                { 'ф', "f" },
                { 'х', "h" },
                { 'ц', "ts" },
                { 'ч', "ch" },
                { 'ш', "sh" },
                { 'щ', "sht" },
                { 'ъ', "a" },
                { 'ь', "y" },
                { 'ю', "yu" },
                { 'я', "ya" },
            };

            int i = 0;
            while(i < s.Length)
            {
                string dest;
                if (s.SubstringSafe(i, "българия".Length).ToLower().StartsWith("българия"))
                {
                    string src = s.Substring(i, "българия".Length);
                    dest =  MatchCase(src, "bulgaria");
                    i += "българия".Length;
                }
                else if (s.SubstringSafe(i, "ия".Length).ToLower().StartsWith("ия") && (s.Length == i + 2 || !char.IsLetter(s[i + 1])))
                {
                    string src = s.Substring(i, "ия".Length);
                    dest = MatchCase(src, "ia");
                    i += "ия".Length;
                }
                else
                {
                    char key = char.ToLower(s[i]);
                    if (corrTable.ContainsKey(key))
                    {
                        char? prevChar = i > 0 ? s[i - 1] : (char?)null;
                        char src = s[i];
                        dest = corrTable[key];
                        char? nextChar = i < s.Length - 1 ? s[i + 1] : (char?)null;
                        dest = SetTransliterateCase(src, prevChar, nextChar, dest);
                    }
                    else
                    {
                        dest = s.Substring(i, 1);
                    }
                    i++;
                }

                sb.Append(dest);
            }
            return sb.ToString();
        }


        private static string SubstringSafe(this string s, int start, int count)
        {
            if (start >= s.Length)
            {
                return string.Empty;
            }
            if (start + count > s.Length)
            {
                return s.Substring(start);
            }
            else
            {
                return s.Substring(start, count);
            }
        }

        private static string MatchCase(string src, string dest)
        {
            string res = string.Empty;
            for (int i = 0; i < src.Length; i++)
            {
                res += char.IsUpper(src[i]) ? char.ToUpper(dest[i]) : char.ToLower(dest[i]);
            }
            return res;
        }

        private static string SetTransliterateCase(char src, char? srcPrev, char? srcNext, string dest)
        {
            if (Char.IsLower(src))
            {
                return dest;//.ToLower();
            }
            else
            {
                if (srcPrev.HasValue && char.IsLetter(srcPrev.Value) && char.IsUpper(srcPrev.Value) ||
                    (!srcPrev.HasValue || !char.IsLetter(srcPrev.Value)) && srcNext.HasValue && char.IsLetter(srcNext.Value) && char.IsUpper(srcNext.Value))
                {
                    return dest.ToUpper();
                }
                else
                {
                    return char.ToUpper(dest[0]) + dest.Substring(1);
                }
            }
        }
    }
}
