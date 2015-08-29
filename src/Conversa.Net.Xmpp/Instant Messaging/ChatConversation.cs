﻿using Conversa.Net.Xmpp.Client;
using Conversa.Net.Xmpp.Core;
using System;
using System.Collections.Generic;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Foundation;

namespace Conversa.Net.Xmpp.InstantMessaging
{
    /// <summary>
    /// Represents a conversation in a chat client.
    /// </summary>
    public sealed class ChatConversation
    {
        public static ChatConversation Create(Contact contact)
        {
            return new ChatConversation(contact);
        }

        private Subject<RemoteParticipantComposingChangedEventData> remoteParticipantComposingChangedStream;
        private ChatMessageStore store;
       
        public IObservable<RemoteParticipantComposingChangedEventData> RemoteParticipantComposingChangedStream
        {
            get { return this.remoteParticipantComposingChangedStream.AsObservable(); }
        }

        /// <summary>
        /// Gets a Boolean value indicating if there are unread messages in the ChatConversation.
        /// </summary>
        public bool HasUnreadMessages
        {
            get;
        } = false;

        /// <summary>
        /// Gets the unique identifier for the ChatConversation.
        /// </summary>
        public string Id
        {
            get;
        } = IdentifierGenerator.Generate();
        
        /// <summary>
        /// Gets or puts a Boolean value indicating if the ChatConversation is muted.
        /// </summary>
        public bool IsConversationMuted
        {
            get;
            set;
        }

        /// <summary>
        /// Gets the ID of the most recent message in the conversation.
        /// </summary>
        public string MostRecentMessageId
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets a list of all the participants in the conversation.
        /// </summary>
        public IEnumerable<string> Participants
        {
            get
            {
                foreach (string participant in this.ThreadingInfo.Participants)
                {
                    yield return participant;
                }
            }
        }

        /// <summary>
        /// Read/write	Gets or puts the subject of a group conversation.
        /// </summary>
        public string Subject
        {
            get;
            set;
        }
         
        /// <summary>
        /// Gets the threading info for the ChatConversation.
        /// </summary>
        public ChatConversationThreadingInfo ThreadingInfo
        {
            get;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ChatConversation"/> class.
        /// </summary>
        /// <param name="contact">The conversation contact.</param>
        internal ChatConversation(Contact contact)
        {
            var transport = XmppTransportManager.GetTransport();

            this.remoteParticipantComposingChangedStream = new Subject<RemoteParticipantComposingChangedEventData>();

            this.ThreadingInfo = new ChatConversationThreadingInfo
            {
                Id              = IdentifierGenerator.Generate()
              , ContactId       = contact.Address.BareAddress
              , ConversationId  = this.Id
              , Custom          = null
              , Kind            = ChatConversationThreadingKind.ContactId
            };

            this.ThreadingInfo.Participants.Add(transport.UserAddress);
            this.ThreadingInfo.Participants.Add(contact.Address);

            this.store = XmppTransportManager.RequestStore(contact.Address.BareAddress);
            this.store.ChangeTracker.Enable();

            transport.MessageStream
                     .Where(message => !message.IsError)
                     .Subscribe(async message => await OnMessageReceived(message).ConfigureAwait(false));            
        }      

        /// <summary>
        /// 
        /// </summary>
        /// <returns>An asynchronous operation that returns a ChatMessageStore on successful completion.</returns>
        public IAsyncOperation<ChatMessageStore> RequestStoreAsync()
        {
            return AsyncInfo.Run(_ => Task.Run<ChatMessageStore>(() => { return store; }));
        }

        /// <summary>
        /// Asynchronously deletes all of the messages in the ChatConversation and the conversation itself.
        /// </summary>
        /// <returns>An async action indicating that the operation has completed.</returns>
        public IAsyncAction DeleteAsync()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Gets the ChatMessageReader for this ChatConversation.
        /// </summary>
        /// <returns>The ChatMessageReader for this ChatConversation.</returns>
        public ChatMessageReader GetMessageReader()
        {
            return this.store.GetMessageReader();
        }
        
        /// <summary>
        /// Asynchronously marks all the messages in the conversation as read.
        /// </summary>
        /// <returns>An async action indicating that the operation has completed.</returns>
        public IAsyncAction MarkMessagesAsReadAsync()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Asynchronously marks all the messages in the conversation before the specified DateTime as read.
        /// </summary>
        /// <param name="value">Mark all messages before this DateTime as read.</param>
        /// <returns>An async action indicating that the operation has completed.</returns>
        public IAsyncAction MarkMessagesAsReadAsync(DateTimeOffset value)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Call this to indicate that the local participant has started or has completed typing.
        /// </summary>
        /// <param name="transportId">Specifies the ChatMessageTransport to use.</param>
        /// <param name="participantAddress">The address of the remote participant.</param>
        /// <param name="isComposing">True if the local participant is typing, otherwise false.</param>
        public void NotifyLocalParticipantComposing(string transportId, XmppAddress participantAddress, bool isComposing)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Locally triggers the event that indicates that a remote participant is typing.
        /// </summary>
        /// <param name="transportId">Specifies the ChatMessageTransport to use.</param>
        /// <param name="participantAddress">The address of the remote participant.</param>
        /// <param name="isComposing">True if the remote participant is typing, otherwise false.</param>
        public void NotifyRemoteParticipantComposing(string transportId, XmppAddress participantAddress, bool isComposing)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// The chat message to be sent.
        /// </summary>
        /// <param name="chatMessage"></param>
        /// <returns></returns>
        public async Task SendMessageAsync(ChatMessage chatMessage)
        {
            if (chatMessage.ThreadingInfo == null)
            {
                chatMessage.ThreadingInfo = this.ThreadingInfo;
            }

            chatMessage.Id                      = IdentifierGenerator.Generate();
            chatMessage.From                    = XmppTransportManager.GetTransport().UserAddress;
            chatMessage.Subject                 = this.Subject;
            chatMessage.Status                  = ChatMessageStatus.Draft;
            chatMessage.RecipientsDeliveryInfos = new List<ChatRecipientDeliveryInfo>();
            
            foreach (var participant in this.Participants)
            {
                var deliveryInfo = new ChatRecipientDeliveryInfo
                {
                      Id                            = IdentifierGenerator.Generate()
                    , DeliveryTime                  = DateTimeOffset.UtcNow
                    , IsErrorPermanent              = false
                    , Status                        = chatMessage.Status
                    , TransportAddress              = participant
                    , TransportErrorCode            = 0
                    , TransportErrorCodeCategory    = XmppTransportErrorCodeCategory.None
                    , TransportInterpretedErrorCode = XmppTransportInterpretedErrorCode.None
                };

                chatMessage.RecipientsDeliveryInfos.Add(deliveryInfo);
            }

            await this.store.SendMessageAsync(chatMessage).ConfigureAwait(false);
        }

        private async Task OnMessageReceived(Message message)
        {
            var chatMessage = ChatMessage.Create(message);

            await this.store.SendMessageAsync(chatMessage).ConfigureAwait(false);
        }
    }
}
