using System;

namespace OLAPUtils.Connection.CastProvider
{
    public interface ICastProvider
    {
        string Stringify(object value);
        object ToValue(string def);
        bool IsApplicable(Type t);
    }
}