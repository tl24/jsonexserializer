using System;
using System.Collections.Generic;
using System.Text;
using System.Globalization;

namespace JsonExSerializer.MetaData
{
    /// <summary>
    /// Uses Pascal case naming strategy, first letter is capitalized, and first letter
    /// of subsequent words are capitalized.  This is the recommended naming strategy
    /// for .NET classes, interfaces, properties, method names, etc.
    /// Example: SomePropertyName
    /// </summary>
    public class PascalCaseNamingStrategy : CustomNamingStrategyBase
    {
        /// <summary>
        /// Creates a PascalCaseNamingStrategy instance with using the current culture
        /// </summary>
        public PascalCaseNamingStrategy()
        {
        }

        /// <summary>
        /// Creates a PascalCaseNamingStrategy instance with using the specified <paramref name="culture"/>
        /// </summary>
        /// <param name="culture">The culture to use for any upper/lower casing operations</param>
        public PascalCaseNamingStrategy(CultureInfo culture) : base(culture)
        {
        }

        public override string GetName(string originalName)
        {
            IList<string> parts = GetNameParts(originalName);
            StringBuilder sb = new StringBuilder(originalName.Length);
            foreach (string word in parts)
            {
                sb.Append(UpperCaseFirst(word));
            }
            return sb.ToString();
        }
    }
}
