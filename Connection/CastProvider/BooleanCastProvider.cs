using System;
using System.Globalization;

namespace OLAPUtils.Connection.CastProvider
{
    public class BooleanCastProvider : ICastProvider
    {
        public string Stringify(object value)
        {
            return CultureInfo.CurrentCulture.TextInfo.ToTitleCase(value.ToString().ToLower());
        }

        public object ToValue(string def)
        {
            return def.ToUpper() == bool.TrueString.ToUpper();
        }

        public bool IsApplicable(Type t)
        {
            return t == typeof(bool);
        }
    }
}