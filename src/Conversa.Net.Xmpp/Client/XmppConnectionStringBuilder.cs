﻿// Copyright (c) Carlos Guzmán Álvarez. All rights reserved.
// Licensed under the New BSD License (BSD). See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace Conversa.Net.Xmpp.Client
{
    /// <summary>
    /// XMPP Connection String Builder
    /// </summary>
    public sealed class XmppConnectionStringBuilder
    {
        private Dictionary<string, object> options;

        /// <summary>
        /// Gets or sets the server.
        /// </summary>
        /// <value>The server.</value>
        public string HostName
        {
            get { return this.GetString(ConnectionStringKeywords.Server); }
            set { this.SetValue(ConnectionStringKeywords.Server, value); }
        }

        /// <summary>
        /// Gets a value indicating whether to resolve host names.
        /// </summary>
        /// <value>
        ///   <c>true</c> if host name should be resolved; otherwise, <c>false</c>.
        /// </value>
        public bool ResolveHostName
        {
            get { return this.GetBoolean(ConnectionStringKeywords.ResolveHostName); }
            set { this.SetValue(ConnectionStringKeywords.ResolveHostName, value); }
        }

        /// <summary>
        /// Gets or sets the service name.
        /// </summary>
        /// <value>The service name.</value>
        public string ServiceName
        {
            get { return this.GetString(ConnectionStringKeywords.ServiceName); }
            set { this.SetValue(ConnectionStringKeywords.ServiceName, value); }
        }

        /// <summary>
        /// Gets or sets the user ID.
        /// </summary>
        /// <value>The user ID.</value>
        public string UserId
        {
            get { return this.GetString(ConnectionStringKeywords.UserId); }
            set { this.SetValue(ConnectionStringKeywords.UserId, value); }
        }

        /// <summary>
        /// Gets or sets the password.
        /// </summary>
        /// <value>The password.</value>
        public string Password
        {
            get { return this.GetString(ConnectionStringKeywords.UserPassword); }
            set { this.SetValue(ConnectionStringKeywords.UserPassword, value); }
        }

        /// <summary>
        /// Gets or sets the user XMPP resource
        /// </summary>
        public string Resource
        {
            get { return this.GetString(ConnectionStringKeywords.Resource); }
            set { this.SetValue(ConnectionStringKeywords.Resource, value); }
        }

        /// <summary>
        /// Gets or sets the connection timeout.
        /// </summary>
        /// <value>The connection timeout.</value>
        public int ConnectionTimeout
        {
            get { return this.GetInt32(ConnectionStringKeywords.ConnectionTimeout); }
            set { this.SetValue(ConnectionStringKeywords.ConnectionTimeout, value); }
        }

        /// <summary>
        /// Gets or sets a value that indicates whether the connection should be done throught a proxy
        /// </summary>
        public bool UseProxy
        {
            get { return this.GetBoolean(ConnectionStringKeywords.UseProxy); }
            set { this.SetValue(ConnectionStringKeywords.UseProxy, value); }
        }

        /// <summary>
        /// Gets or sets the proxy type
        /// </summary>
        public string ProxyType
        {
            get { return this.GetString(ConnectionStringKeywords.ProxyType); }
            set { this.SetValue(ConnectionStringKeywords.ProxyType, value); }
        }

        /// <summary>
        /// Gets or sets the proxy server
        /// </summary>
        public string ProxyServer
        {
            get { return this.GetString(ConnectionStringKeywords.ProxyServer); }
            set { this.SetValue(ConnectionStringKeywords.ProxyServer, value); }
        }

        /// <summary>
        /// Gets or sets the proxy port number
        /// </summary>
        public int ProxyPortNumber
        {
            get { return this.GetInt32(ConnectionStringKeywords.ProxyPortNumber); }
            set { this.SetValue(ConnectionStringKeywords.ProxyPortNumber, value); }
        }

        /// <summary>
        /// Gets or sets the proxy user name
        /// </summary>
        public string ProxyUserName
        {
            get { return this.GetString(ConnectionStringKeywords.ProxyUserName); }
            set { this.SetValue(ConnectionStringKeywords.ProxyUserName, value); }
        }

        /// <summary>
        /// Gets or sets the proxy password
        /// </summary>
        public string ProxyPassword
        {
            get { return this.GetString(ConnectionStringKeywords.ProxyPassword); }
            set { this.SetValue(ConnectionStringKeywords.ProxyPassword, value); }
        }

        /// <summary>
        /// Gets or sets a value that indicates whether http binding should be used
        /// </summary>
        public bool UseHttpBinding
        {
            get { return this.GetBoolean(ConnectionStringKeywords.HttpBinding); }
            set { this.SetValue(ConnectionStringKeywords.HttpBinding, value); }
        }

        /// <summary>
        /// Gets the buffer packet size
        /// </summary>
        public uint PacketSize
        {
            get { return this.GetUInt32(ConnectionStringKeywords.PacketSize); }
            set { this.SetValue(ConnectionStringKeywords.PacketSize, value); }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:XmppConnectionStringBuilder"/> class.
        /// </summary>
        public XmppConnectionStringBuilder()
        {
            this.options = new Dictionary<string, object>();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:XmppConnectionStringBuilder"/> class with
        /// the given connection string.
        /// </summary>
        /// <param name="connectionString">The connection string.</param>
        public XmppConnectionStringBuilder(string connectionString)
            : this()
        {
            this.Load(connectionString);
        }

        public XmppConnectionString ToConnectionString()
        {
            return new XmppConnectionString(this.ToString());
        }

        /// <summary>
        /// Returns a <see cref="T:System.String"></see> that represents the current <see cref="T:System.Object"></see>.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.String"></see> that represents the current <see cref="T:System.Object"></see>.
        /// </returns>
        public override string ToString()
        {
            var cs = new StringBuilder();
            var e  = this.options.GetEnumerator();

            while (e.MoveNext())
            {
                if (e.Current.Value != null)
                {
                    if (cs.Length > 0)
                    {
                        cs.Append(";");
                    }

                    cs.AppendFormat(CultureInfo.CurrentUICulture, "{0}={1}", e.Current.Key, e.Current.Value);
                }
            }

            return cs.ToString();
        }

        private void Load(string connectionString)
        {
            if (connectionString != null)
            {
                return;
            }

            string[] keyPairs = connectionString.Split(';');

            this.options.Clear();

            foreach (string keyPair in keyPairs)
            {
                string[] values = keyPair.Split('=');

                if (values.Length == 2
                 && values[0] != null && values[0].Length > 0
                 && values[1] != null && values[1].Length > 0)
                {
                    values[0] = values[0].ToLower();

                    if (XmppConnectionString.IsSynonym(values[0]))
                    {
                        this.options[XmppConnectionString.GetSynonym(values[0])] = values[1].Trim();
                    }
                }
            }
        }

        private string GetString(string key)
        {
            if (this.options.ContainsKey(key))
            {
                return (string)this.options[key];
            }

            return null;
        }

        private int GetInt32(string key)
        {
            if (this.options.ContainsKey(key))
            {
                return Convert.ToInt32(this.options[key], CultureInfo.CurrentUICulture);
            }

            return 0;
        }

        private uint GetUInt32(string key)
        {
            if (this.options.ContainsKey(key))
            {
                return Convert.ToUInt32(this.options[key], CultureInfo.CurrentUICulture);
            }

            return 0;
        }

        private bool GetBoolean(string key)
        {
            if (this.options.ContainsKey(key))
            {
                return Convert.ToBoolean(this.options[key], CultureInfo.CurrentUICulture);
            }

            return false;
        }

        private void SetValue(string key, object value)
        {
            if (this.options.ContainsKey(key))
            {
                this.options[key] = value;
            }
            else
            {
                this.options.Add(key, value);
            }
        }
    }
}
