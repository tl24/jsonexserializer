using System;
using System.Collections.Generic;
using System.Text;
using System.Globalization;

namespace JsonExSerializer.MetaData
{
    /// <summary>
    /// Uses Camel case naming strategy, first word is all lowercase, and first letter
    /// of subsequent words are capitalized.  This is the recommended naming strategy
    /// for method parameters and member and local variables in .NET
    /// Example: accountBalance
    /// </summary>
    public class CamelCaseNamingStrategy : CustomNamingStrategyBase
    {
        /// <summary>
        /// Creates a CamelCaseNamingStrategy instance with using the current culture
        /// </summary>
        public CamelCaseNamingStrategy()
        {
        }

        /// <summary>
        /// Creates a CamelCaseNamingStrategy instance with using the specified <paramref name="culture"/>
        /// </summary>
        /// <param name="culture">The culture to use for any upper/lower casing operations</param>
        public CamelCaseNamingStrategy(CultureInfo culture)
            : base(culture)
        {
        }

        public override string GetName(string originalName)
        {
            IList<string> parts = GetNameParts(originalName);
            StringBuilder sb = new StringBuilder(originalName.Length);
            foreach (string word in parts)
            {
                if (sb.Length == 0)
                    sb.Append(word.ToLower(_culture));
                else
                    sb.Append(UpperCaseFirst(word));
            }
            return sb.ToString();
        }
    }
}
