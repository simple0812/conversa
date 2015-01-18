// Copyright (c) Carlos Guzmán Álvarez. All rights reserved.
// Licensed under the New BSD License (BSD). See LICENSE file in the project root for full license information.

using Conversa.Net.Xmpp.Core;

namespace Conversa.Net.Xmpp.PersonalEventing
{
    /// <summary>
    /// Activity event for headline and normal messages
    /// </summary>
    public sealed class XmppMessageEvent
        : XmppEvent
    {
        private Message message;

        /// <summary>
        /// Gets the message information
        /// </summary>
        public Message Message
        {
            get { return this.message; }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="">XmppMessageEvent</see> class.
        /// </summary>
        /// <param name="message">The message information</param>
        public XmppMessageEvent(Message message)
        {
            this.message = message;
        }
    }
}
