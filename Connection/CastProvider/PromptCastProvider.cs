using System;
using OLAPUtils.Connection.Enums;

namespace OLAPUtils.Connection.CastProvider
{
    public class PromptCastProvider : ICastProvider
    {
        public string Stringify(object value)
        {
            var v = (int) value;
            return v.ToString();
        }

        public object ToValue(string def)
        {
            try
            {
                return Enum.Parse(typeof(Prompt), def);
            }
            catch
            {
                return Prompt.NoPrompt;
            }
        }

        public bool IsApplicable(Type t)
        {
            return t == typeof(Prompt);
        }
    }
}