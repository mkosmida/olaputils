using System;
using System.Collections.Generic;
using System.Linq;
using OLAPUtils.Connection.Attributes;
using OLAPUtils.Connection.Enums;

namespace OLAPUtils.Connection
{
    public class OlapConnectionStringBuilder
    {
        private const string PROPERTY_PROVIDER = "Provider";
        private const string PROPERTY_USERID = "User ID";
        private const string PROPERTY_PASSWORD = "Password";
        private const string PROPERTY_DATASOURCE = "DataSource";
        private const string PROPERTY_DATA_SOURCE = "Data Source";
        private const string PROPERTY_CATALOG = "Catalog";
        private const string PROPERTY_INITIAL_CATALOG = "Initial Catalog";
        private const string PROPERTY_INTEGRATED_SECURITY = "Integrated Security";
        private const string PROPERTY_SSPI = "SSPI";
        private const string PROPERTY_PERSIST_SECURITY_INFO = "Persist Security Info";
        private const string PROPERTY_PROMPT = "Prompt";
        private const string PROPERTY_LOCALE_IDENTIFIER = "LocaleIdentifier";
        //HELLO!

        private readonly Dictionary<string, string> _connectionStringKeyWords = new Dictionary<string, string>();


        private readonly HashSet<string> _validKeyWords = new HashSet<string>
        {
            PROPERTY_PROVIDER,
            PROPERTY_USERID,
            PROPERTY_PASSWORD,
            PROPERTY_DATASOURCE,
            PROPERTY_DATA_SOURCE,
            PROPERTY_CATALOG,
            PROPERTY_INITIAL_CATALOG,
            PROPERTY_INTEGRATED_SECURITY,
            PROPERTY_SSPI,
            PROPERTY_PERSIST_SECURITY_INFO,
            PROPERTY_PROMPT,
            PROPERTY_LOCALE_IDENTIFIER
        };

        public OlapConnectionStringBuilder(string connectionString)
        {
            ConnectionString = connectionString;
        }

        public OlapConnectionStringBuilder()
        {
        }

        public int LocaleIdentifier
        {
            get
            {
                int i;
                if (int.TryParse(GetValue(PROPERTY_LOCALE_IDENTIFIER), out i))
                {
                    return i;
                }
                return 0;
            }
            set { SetValue(PROPERTY_LOCALE_IDENTIFIER, value.ToString()); }
        }

        public string ConnectionString
        {
            get { return string.Join(";", _connectionStringKeyWords.Select(x => x.Key + "=" + x.Value).ToArray()); }
            set { SetConnectionString(value); }
        }

        public string Provider
        {
            get { return GetValue(PROPERTY_PROVIDER); }
            set { SetValue(PROPERTY_PROVIDER, value); }
        }

        [Names(PROPERTY_USERID)]
        public string UserID
        {
            get { return GetValue(PROPERTY_USERID); }

            set { SetValue(PROPERTY_USERID, value); }
        }

        public string Password
        {
            get { return GetValue(PROPERTY_PASSWORD); }

            set { SetValue(PROPERTY_PASSWORD, value); }
        }

        [Names(PROPERTY_DATASOURCE + "," + PROPERTY_DATA_SOURCE)]
        public string DataSource
        {
            get
            {
                var v = GetValue(PROPERTY_DATASOURCE);
                return !string.IsNullOrEmpty(v) ? v : GetValue(PROPERTY_DATA_SOURCE);
            }

            set
            {
                // SetValue(PROPERTY_DATASOURCE, value);
                SetValue(PROPERTY_DATA_SOURCE, value);
            }
        }

        [Names(PROPERTY_INITIAL_CATALOG + "," + PROPERTY_CATALOG)]
        public string Catalog
        {
            get
            {
                var v = GetValue(PROPERTY_CATALOG);
                return !string.IsNullOrEmpty(v) ? v : GetValue(PROPERTY_INITIAL_CATALOG);
            }
            set
            {
                //SetValue(PROPERTY_CATALOG, value);
                SetValue(PROPERTY_INITIAL_CATALOG, value);
            }
        }

        [Names(PROPERTY_INTEGRATED_SECURITY)]
        public IntegratedSecurity IntegratedSecurity
        {
            get
            {
                return
                    (IntegratedSecurity)
                        CastProvider.CastProvider.ToValue(typeof(IntegratedSecurity),
                            GetValue(PROPERTY_INTEGRATED_SECURITY));
            }

            set
            {
                SetValue(PROPERTY_INTEGRATED_SECURITY,
                    CastProvider.CastProvider.Stringify(typeof(IntegratedSecurity), value));
            }
        }

        public SSPI SSPI
        {
            get { return (SSPI)  CastProvider.CastProvider.ToValue(typeof(SSPI), GetValue(PROPERTY_SSPI)); }
            set { SetValue(PROPERTY_SSPI, CastProvider.CastProvider.Stringify(typeof(SSPI), value)); }
        }

        [Names(PROPERTY_PERSIST_SECURITY_INFO)]
        public bool PersistSecurityInfo
        {
            get { return (bool) CastProvider.CastProvider.ToValue(typeof(bool), GetValue(PROPERTY_PERSIST_SECURITY_INFO)); }
            set { SetValue(PROPERTY_PERSIST_SECURITY_INFO, CastProvider.CastProvider.Stringify(typeof(bool), value)); }
        }

        public Prompt Prompt
        {
            get { return (Prompt) CastProvider.CastProvider.ToValue(typeof(Prompt), GetValue(PROPERTY_PROMPT)); }

            set { SetValue(PROPERTY_PROMPT, CastProvider.CastProvider.Stringify(typeof(Prompt), value)); }
        }

        private void SetConnectionString(string value)
        {
            var props = value.Split(';');

            foreach (var elem in props)
            {
                var kv = elem.Split('=');
                if (kv.Length > 1)
                {
                    var k = kv[0];
                    var v = kv[1];
                    if (ValidateProperty(k))
                        SetProperty(k, v);
                }
            }
        }

        private bool ValidateProperty(string propertyName)
        {
            return _validKeyWords.Contains(propertyName);
        }

        private void SetValue(string propertyName, string value)
        {
            if (!ValidateProperty(propertyName)) return;
            if (_connectionStringKeyWords.ContainsKey(propertyName))
                _connectionStringKeyWords[propertyName] = value;
            else
            {
                _connectionStringKeyWords.Add(propertyName, value);
            }
        }

        private string GetValue(string propertyName)
        {
            if (_connectionStringKeyWords.ContainsKey(propertyName))
                return _connectionStringKeyWords[propertyName];
            return string.Empty;
        }

        private void SetProperty(string name, string value)
        {
            if (string.IsNullOrEmpty(value)) return;

            var propertyInfo = GetType().GetProperty(name);

            if (propertyInfo != null)
            {
                if (propertyInfo.PropertyType.BaseType != typeof(Enum))
                    propertyInfo.SetValue(this, Convert.ChangeType(value, propertyInfo.PropertyType), null);
                else
                {
                    propertyInfo.SetValue(this, CastProvider.CastProvider.ToValue(propertyInfo.PropertyType, value),
                        null);
                }
            }
            else
            {
                var props = GetType().GetProperties().Where(
                    prop => Attribute.IsDefined(prop, typeof(NamesAttribute)));

                foreach (var p in props)
                {
                    var attrs = p.GetCustomAttributes(typeof(NamesAttribute), true);
                    var nameAttr = attrs.FirstOrDefault() as NamesAttribute;
                    if (nameAttr != null && nameAttr.Names.Contains(name))
                    {
                        if (p.PropertyType.BaseType != typeof(Enum))
                            p.SetValue(this, Convert.ChangeType(value, p.PropertyType), null);
                        else
                        {
                            p.SetValue(this, CastProvider.CastProvider.ToValue(p.PropertyType, value), null);
                        }
                    }
                }
            }
        }
    }
}