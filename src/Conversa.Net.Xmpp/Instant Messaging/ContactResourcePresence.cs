﻿// Copyright (c) Carlos Guzmán Álvarez. All rights reserved.
// Licensed under the New BSD License (BSD). See LICENSE file in the project root for full license information.

using Conversa.Net.Xmpp.Client;
using Conversa.Net.Xmpp.Core;
using DevExpress.Mvvm;
using System;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Threading.Tasks;

namespace Conversa.Net.Xmpp.InstantMessaging
{
    /// <summary>
    /// Contact resource presence handling
    /// </summary>
    public sealed class ContactResourcePresence
        : BindableBase
    {
        private Subject<ContactResource> presenceStream;
        private ContactResource          resource;

        /// <summary>
        /// Occurs when the contact presence is updated
        /// </summary>
        public IObservable<ContactResource> PresenceChanged
        {
            get { return this.presenceStream.AsObservable(); }
        }

        /// <summary>
        /// Gets or sets the presence priority.
        /// </summary>
        /// <value>The priority.</value>
        public int Priority
        {
            get { return GetProperty(() => Priority); }
            private set { SetProperty(() => Priority, value); }
        }

        /// <summary>
        /// Gets or sets the presence status.
        /// </summary>
        /// <value>The presence status.</value>
        public ShowType ShowAs
        {
            get { return GetProperty(() => ShowAs); }
            private set { SetProperty(() => ShowAs, value); }
        }

        /// <summary>
        /// Gets or sets the presence status message.
        /// </summary>
        /// <value>The presence status message.</value>
        public string StatusMessage
        {
            get { return GetProperty(() => StatusMessage); }
            private set { SetProperty(() => StatusMessage, value); }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ContactResourcePresence"/> class using
        /// the given session.
        /// </summary>
        /// <param name="session"></param>
        internal ContactResourcePresence(ContactResource resource)
        {
            var transport = XmppTransportManager.GetTransport();

            this.resource       = resource;
            this.presenceStream = new Subject<ContactResource>();
            this.ShowAs         = ShowType.Offline;

            transport.StateChanged
                     .Where(state => state == XmppTransportState.Closing)
                     .Subscribe(state => OnDisconnecting());
        }

        /// <summary>
        /// Gets the presence information for the current contact resource.
        /// </summary>
        /// <param name="address">User address</param>
        public async Task GetPresenceAsync()
        {
            var transport = XmppTransportManager.GetTransport();
            var presence  = new Presence
            {
                From          = transport.UserAddress
              , To            = this.resource.Address
              , Type          = PresenceType.Probe
              , TypeSpecified = true
            };

            await transport.SendAsync(presence).ConfigureAwait(false);
        }

        /// <summary>
        /// Request subscription to the current contact resource
        /// </summary>
        public async Task RequestSubscriptionAsync()
        {
            var transport = XmppTransportManager.GetTransport();
            var presence  = new Presence
            {
                To            = this.resource.Address
              , Type          = PresenceType.Subscribe
              , TypeSpecified = true
            };

            await transport.SendAsync(presence).ConfigureAwait(false);
        }

        /// <summary>
        /// Subscribes to presence updates of the current contact resource
        /// </summary>
        public async Task SubscribedAsync()
        {
            var transport = XmppTransportManager.GetTransport();
            var presence  = new Presence
            {
                To            = this.resource.Address
              , Type          = PresenceType.Subscribed
              , TypeSpecified = true
            };

            await transport.SendAsync(presence).ConfigureAwait(false);
        }

        /// <summary>
        /// Subscribes to presence updates of the current contact resource
        /// </summary>
        public async Task UnsuscribeAsync()
        {
            var transport = XmppTransportManager.GetTransport();
            var presence  = new Presence
            {
                To            = this.resource.Address
              , Type          = PresenceType.Unsubscribe
              , TypeSpecified = true
            };

            await transport.SendAsync(presence).ConfigureAwait(false);
        }

        /// <summary>
        /// Subscribes to presence updates of the current contact resource
        /// </summary>
        public async Task UnsuscribedAsync()
        {
            var transport = XmppTransportManager.GetTransport();
            var presence  = new Presence
            {
                To            = this.resource.Address
              , Type          = PresenceType.Unsubscribed
              , TypeSpecified = true
            };

            await transport.SendAsync(presence).ConfigureAwait(false);
        }

        private void OnDisconnecting()
        {
            this.resource = null;
            this.presenceStream.Dispose();
        }

        internal void Update(Presence presence)
        {
            this.ShowAs = ShowType.Online;

            if (presence.TypeSpecified && presence.Type == PresenceType.Unavailable)
            {
                this.ShowAs = ShowType.Offline;
            }
            else if (presence.ShowSpecified)
            {
                this.ShowAs = presence.Show;
            }

            if (presence.PrioritySpecified)
            {
                this.Priority = presence.Priority;
            }

            this.StatusMessage = ((presence.Status == null) ? String.Empty : presence.Status.Value);

            this.presenceStream.OnNext(this.resource);
        }
    }
}
