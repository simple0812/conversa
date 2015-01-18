// Copyright (c) Carlos Guzmán Álvarez. All rights reserved.
// Licensed under the New BSD License (BSD). See LICENSE file in the project root for full license information.

using Conversa.Net.Xmpp.Client;
using Conversa.Net.Xmpp.Core;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Reactive.Linq;

namespace Conversa.Net.Xmpp.PersonalEventing
{
    /// <summary>
    /// XMPP Activity
    /// </summary>
    public sealed class XmppActivity
        : XmppMessageProcessor, IEnumerable<XmppEvent>, INotifyCollectionChanged
    {
        public event NotifyCollectionChangedEventHandler CollectionChanged;

        private ObservableCollection<XmppEvent>	activities;
        private IDisposable                     messageSubscription;

        /// <summary>
        /// Initializes a new instance of the <see cref="XmppSession"/> class
        /// </summary>
        internal XmppActivity(XmppClient client)
            : base(client)
        {
            this.activities	= new ObservableCollection<XmppEvent>();
        }

        /// <summary>
        /// Clears the activity list
        /// </summary>
        public void Clear()
        {
        	this.activities.Clear();

            if (this.CollectionChanged != null)
            {
                this.CollectionChanged(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
            }
        }

        IEnumerator<XmppEvent> IEnumerable<XmppEvent>.GetEnumerator()
        {
            return this.activities.GetEnumerator();
        }

        public IEnumerator GetEnumerator()
        {
            return this.activities.GetEnumerator();
        }

        protected override void OnClientConnected()
        {
            this.messageSubscription = this.Client
                                           .MessageStream
                                           .Where(m => m.Type == MessageType.Headline || m.Type == MessageType.Normal)
                                           .Subscribe(message => { this.OnMessageReceived(message); });

            this.activities.CollectionChanged += new NotifyCollectionChangedEventHandler(OnCollectionChanged);

            base.OnClientConnected();
        }

        private void Unsubscribe()
        {
            if (this.messageSubscription != null)
            {
                this.messageSubscription.Dispose();
                this.messageSubscription = null;
            }

            this.activities.CollectionChanged -= new NotifyCollectionChangedEventHandler(OnCollectionChanged);
        }

        private void OnMessageReceived(Message message)
        {
            switch (message.Type)
            {
                case MessageType.Normal:
                    this.activities.Add(new XmppMessageEvent(message));
                    break;

                case MessageType.Headline:
                    this.activities.Add(new XmppMessageEvent(message));
                    break;
            }
        }

        private void OnEventMessage(Message message)
        {
#warning TODO: Reimplement
//            // Add the activity based on the source event
//            if (XmppEvent.IsActivityEvent(message.Event))
//            {
//                XmppEvent xmppevent = XmppEvent.Create(this.session.Roster[message.From.BareIdentifier], message.Event);

//#warning TODO: see what to do when it is null
//                if (xmppevent != null)
//                {
//#warning TODO: This needs to be changed
//                    if (xmppevent is XmppUserTuneEvent && ((XmppUserTuneEvent)xmppevent).IsEmpty)
//                    {
//                        // And empty tune means no info available or that the user
//                        // cancelled the tune notifications ??
//                    }
//                    else
//                    {
//                        this.activities.Add(xmppevent);
//                    }
//                }
//            }
        }

        private void OnCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (this.CollectionChanged != null)
            {
                this.CollectionChanged(this, e);
            }
        }
    }
}
