﻿using PeacefulHills.Bootstrap;
using PeacefulHills.Network.Profiling;
using Unity.Entities;

namespace PeacefulHills.Network
{
    // [UpdateInWorld(typeof(NetworkWorld))] todo:
    [UpdateInGroup(typeof(InitializationSystemGroup))]
    public class NetworkInitializationGroup : ComponentSystemGroup
    {
    }

    [UpdateInGroup(typeof(NetworkInitializationGroup), OrderFirst = true)]
    public class BeginNetworkInitializationBuffer : EntityCommandBufferSystem
    {
    }

    [UpdateInGroup(typeof(NetworkInitializationGroup), OrderLast = true)]
    public class EndNetworkInitializationBuffer : EntityCommandBufferSystem
    {
    }

    // [UpdateInWorld(typeof(NetworkWorld))] todo:
    [UpdateInGroup(typeof(SimulationSystemGroup))]
    public class NetworkSimulationGroup : ComponentSystemGroup
    {
    }

    [UpdateInGroup(typeof(NetworkSimulationGroup), OrderFirst = true)]
    public class BeginNetworkSimulationBuffer : EntityCommandBufferSystem
    {
        protected override void OnUpdate()
        {
            NetworkProfilerCounters.BytesSent.Value = 0;
            NetworkProfilerCounters.BytesReceived.Value = 0;
            base.OnUpdate();
        }
    }

    [UpdateInGroup(typeof(NetworkSimulationGroup), OrderLast = true)]
    public class EndNetworkSimulationBuffer : EntityCommandBufferSystem
    {
        protected override void OnUpdate()
        {
            base.OnUpdate();
            NetworkProfilerCounters.BytesSent.Sample();
            NetworkProfilerCounters.BytesReceived.Sample();
        }
    }
}