using System;

namespace SAIS.Model
{
    public static class EnumUtil
    {
        public static CodeT ToEnum<CodeT>(this string text)
                    where CodeT : struct
        {
            CodeT code;
            if (!Enum.TryParse(text, out code))
            {
                throw new FormatException(string.Format("Код {0} не се поддържа от enum {1}.", text, typeof(CodeT).Name));
            }
            return code;
        }
        public static CodeT? ToEnumNullable<CodeT>(this string text)
            where CodeT : struct
        {
            if (!string.IsNullOrWhiteSpace(text))
            {
                return text.ToEnum<CodeT>();
            }
            else
            {
                return null;
            }
        }
    }
}
