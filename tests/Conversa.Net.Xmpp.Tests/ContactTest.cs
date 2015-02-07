﻿// Copyright (c) Carlos Guzmán Álvarez. All rights reserved.
// Licensed under the New BSD License (BSD). See LICENSE file in the project root for full license information.

using Conversa.Net.Xmpp.Client;
using Conversa.Net.Xmpp.InstantMessaging;
using System;
using System.Linq;
using System.Reactive.Linq;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Conversa.Net.Xmpp.Tests
{
    public class ContactTest
    {
        [Fact]
        public void BlockContactTest()
        {
            InternalBlockContactAsync().Wait();
        }

        private async Task InternalBlockContactAsync()
        {
            var waiter = new AutoResetEvent(false);

            using (var client = new XmppClient(ConnectionStringHelper.GetDefaultConnectionString()))
            {
                client.Roster
                      .ContactListChangedStream
                      .Where(x => x.Item2 == ContactListChangedAction.Reset)
                      .Subscribe(x => waiter.Set());

                await client.OpenAsync().ConfigureAwait(false);

                waiter.WaitOne();

                var contact = client.Roster.First();

                contact.BlockingStream
                       .Where(x => x == ContactBlockingAction.Blocked)
                       .Subscribe(x => waiter.Set());

                await contact.BlockAsync().ConfigureAwait(false);

                waiter.WaitOne();
            }
        }
    }
}