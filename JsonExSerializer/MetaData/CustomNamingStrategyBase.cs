using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.Globalization;

namespace JsonExSerializer.MetaData
{
    /// <summary>
    /// Base class for custom naming strategies.  Provides helper methods for common functions such
    /// as splitting a name into different parts, casing functions, etc.
    /// </summary>
    public abstract class CustomNamingStrategyBase : IPropertyNamingStrategy
    {
        private static Regex _pattern = new Regex(@"(\p{Lu}\P{Lu}|\p{Lu}{2,}?(?=\p{Lu}\P{Lu})|\p{Lu}+$)");
        protected CultureInfo _culture = CultureInfo.CurrentCulture;

        public abstract string GetName(string originalName);

        /// <summary>
        /// Creates a CustomNamingStrategyBase instance with using the current culture
        /// </summary>
        public CustomNamingStrategyBase()
        {
        }

        /// <summary>
        /// Creates a CustomNamingStrategyBase instance with using the specified <paramref name="culture"/>
        /// </summary>
        /// <param name="culture">The culture to use for any upper/lower casing operations</param>
        public CustomNamingStrategyBase(CultureInfo culture)
        {
            _culture = culture;
        }

        /// <summary>
        /// Splits a name into parts using upper to lower case boundaries
        /// </summary>
        /// <param name="name">the name to split</param>
        /// <returns>list of name parts</returns>
        protected virtual IList<string> GetNameParts(string name)
        {
            if (string.IsNullOrEmpty(name))
                throw new ArgumentException("Name can not be empty or null", "name");

            string delimitedName = name.Replace("_", ","); // use any existing underscores as delimeters
            delimitedName = _pattern.Replace(delimitedName, @",$1");

            return new List<string>(delimitedName.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries));
        }

        /// <summary>
        /// Uppercases the first letter of a word and lowercases all remaining letters
        /// </summary>
        /// <param name="word"></param>
        /// <returns></returns>
        protected virtual string UpperCaseFirst(string word)
        {
            if (word.Length > 1)
                return char.ToUpper(word[0], _culture) + word.Substring(1).ToLower(_culture);
            else
                return word.ToUpper(_culture);
        }
    }
}
