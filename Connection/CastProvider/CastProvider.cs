using System;
using System.Collections.Generic;

namespace OLAPUtils.Connection.CastProvider
{
    public static class CastProvider
    {
        private static readonly HashSet<ICastProvider> Providers = new HashSet<ICastProvider>
        {
            new BooleanCastProvider(),
            new PromptCastProvider(),
            new SspiCastProvider(),
            new IntegratedSecurityCastProvider()
        };

        public static string Stringify(Type t, object value)
        {
            var provider = GetProvider(t);
            return null != provider ? provider.Stringify(value) : string.Empty;
        }

        public static object ToValue(Type t, string def)
        {
            var provider = GetProvider(t);
            return provider?.ToValue(def);
        }

        private static ICastProvider GetProvider(Type t)
        {
            foreach (var provider in Providers)
            {
                if (provider.IsApplicable(t))
                {
                    return provider;
                }
            }
            return null;
        }
    }
}