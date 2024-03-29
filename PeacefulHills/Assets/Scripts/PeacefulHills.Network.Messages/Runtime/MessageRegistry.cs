﻿using Unity.Burst;
using Unity.Collections;
using Unity.Entities;

namespace PeacefulHills.Network.Messages
{
    public class MessageRegistry : IMessageRegistry
    {
        public NativeList<MessageInfo> Messages => _messages;

        private NativeHashMap<ulong, ushort> _messageIdsByStableHash;
        private NativeList<MessageInfo> _messages;

        public MessageRegistry()
        {
            _messageIdsByStableHash = new NativeHashMap<ulong, ushort>(1, Allocator.Persistent);
            _messages = new NativeList<MessageInfo>(1, Allocator.Persistent);
        }

        public void Register<TMessage, TMessageSerializer>()
            where TMessage : unmanaged, IMessage
            where TMessageSerializer : unmanaged, IMessageSerializer<TMessage>
        {
            ushort id = (ushort) _messages.Length;
            TypeManager.TypeInfo typeInfo = TypeManager.GetTypeInfo<TMessage>();
            FunctionPointer<DeserializeAction> deserialize =
                MessageSerializerStatic<TMessage, TMessageSerializer>.DeserializeAction;

            _messages.Add(new MessageInfo(id, typeInfo, deserialize));
            _messageIdsByStableHash[typeInfo.StableTypeHash] = id;
        }

        public MessageInfo GetInfoById(ushort id)
        {
            return _messages[id];
        }

        public ushort GetIdByStableHash(ulong stableHash)
        {
            return _messageIdsByStableHash[stableHash];
        }

        public void Dispose()
        {
            _messageIdsByStableHash.Dispose();
            _messages.Dispose();
        }
    }
}