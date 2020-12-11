using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace HYDB.FilterEngine
{
    class TypeExtractor
    {

        public static Type GetDataType(string value)
        {
            if (IsInt(value))
            {
                return typeof(int);
            }
            else if (IsDouble(value))
            {
                return typeof(double);
            }
            else if (IsString(value))
            {
                return typeof(string);
            }
            else
            {
                return null;
            }
        }

        private static bool IsInt(string value)
        {
            int number;
            if (int.TryParse(value, out number))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        private static bool IsDouble(string value)
        {
            double number;
            if (double.TryParse(value, out number))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        private static bool IsString(string value)
        {
            string pattern = @"'([^']*)'";
            var matches = Regex.Match(value, pattern);
            if (matches.Success)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public static string GetStringValue(string value)
        {
            return value.Replace("'", " ").Trim();
        }
    }
}
