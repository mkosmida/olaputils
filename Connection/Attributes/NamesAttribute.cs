using System;
using System.Collections.Generic;

namespace OLAPUtils.Connection.Attributes
{
    internal class NamesAttribute : Attribute
    {
        public NamesAttribute(string names)
        {
            Names = new HashSet<string>(names.Split(','));
        }

        public HashSet<string> Names { get; }
    }
}