using System;
using OLAPUtils.Connection.Enums;

namespace OLAPUtils.Connection.CastProvider
{
    public class IntegratedSecurityCastProvider : ICastProvider
    {
        public string Stringify(object value)
        {
            return Enum.GetName(typeof(IntegratedSecurity), value);
        }

        public object ToValue(string def)
        {
            try
            {
                return Enum.Parse(typeof(IntegratedSecurity), def);
            }
            catch
            {
                return IntegratedSecurity.BASIC;
            }
        }

        public bool IsApplicable(Type t)
        {
            return t == typeof(IntegratedSecurity);
        }
    }
}