﻿using Unity.Entities;
using Unity.Networking.Transport;

namespace PeacefulHills.Network.Messages
{
    [InternalBufferCapacity(32)]
    public unsafe struct OutputMessage : IBufferElementData
    {
        public void* Data;
        public NetworkPipeline Pipeline;
    }
}