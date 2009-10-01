using System;
using System.Collections.Generic;
using System.Text;
using System.Globalization;

namespace JsonExSerializer.MetaData
{
    /// <summary>
    /// Naming strategy that separates words in names using the underscore character.  There
    /// are 3 different styles: original, lower case, mixed case, upper case.
    /// Example: some_property_name
    /// </summary>
    public class UnderscoreNamingStrategy : CustomNamingStrategyBase
    {
        public enum UnderscoreCaseStyle
        {
            OriginalCase,
            MixedCase,
            LowerCase,
            UpperCase
        }

        private UnderscoreCaseStyle _style = UnderscoreCaseStyle.OriginalCase;

        /// <summary>
        /// Creates a UnderscoreNamingStrategy instance with using the current culture and
        /// default style of Mixed Case
        /// </summary>
        public UnderscoreNamingStrategy()
        {
        }

        /// <summary>
        /// Creates a UnderscoreNamingStrategy instance with using the current culture and
        /// default mode of Mixed Case
        /// </summary>
        public UnderscoreNamingStrategy(UnderscoreCaseStyle style)
        {
            _style = style;
        }

        /// <summary>
        /// Creates a UnderscoreNamingStrategy instance with using the specified <paramref name="culture"/>
        /// and <paramref name="style"/>.
        /// </summary>
        /// <param name="culture">The culture to use for any upper/lower casing operations</param>
        /// <param name="style">The casing style to use</param>
        public UnderscoreNamingStrategy(CultureInfo culture, UnderscoreCaseStyle style)
            : base(culture)
        {
            _style = style;
        }

        public override string GetName(string originalName)
        {
            StringBuilder builder = new StringBuilder(originalName.Length + 5);
            IList<string> parts = GetNameParts(originalName);
            builder.Append(ChangeCase(parts[0]));
            for(int i = 1; i < parts.Count; i++)
            {
                builder.Append("_").Append(ChangeCase(parts[i]));
            }
            return builder.ToString();
        }

        public virtual string ChangeCase(string word)
        {
            switch (_style)
            {
                case UnderscoreCaseStyle.MixedCase:
                    return UpperCaseFirst(word);
                case UnderscoreCaseStyle.LowerCase:
                    return word.ToLower(_culture);
                case UnderscoreCaseStyle.UpperCase:
                    return word.ToUpper(_culture);
                case UnderscoreCaseStyle.OriginalCase:
                default:
                    return word;
            }
        }
    }
}
