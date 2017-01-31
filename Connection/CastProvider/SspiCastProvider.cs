using System;
using OLAPUtils.Connection.Enums;

namespace OLAPUtils.Connection.CastProvider
{
    public class SspiCastProvider : ICastProvider
    {
        private const string ANONYMOUS_USER = "Anonymous User";

        public string Stringify(object value)
        {
            var v = (SSPI) value;
            switch (v)
            {
                case SSPI.AnonymousUser:
                    return ANONYMOUS_USER;
                default:
                    return Enum.GetName(typeof(SSPI), value);
            }
        }

        public object ToValue(string def)
        {
            try
            {
                return Enum.Parse(typeof(SSPI), def);
            }
            catch
            {
                return SSPI.NTLM;
            }
        }

        public bool IsApplicable(Type t)
        {
            return t == typeof(SSPI);
        }
    }
}