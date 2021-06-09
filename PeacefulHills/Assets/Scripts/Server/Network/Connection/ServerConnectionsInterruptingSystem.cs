﻿using PeacefulHills.ECS.World;
using Unity.Entities;
using Unity.Networking.Transport;

namespace PeacefulHills.Network.Connection
{
    [UpdateInGroup(typeof(NetworkSimulationGroup))]
    public class ServerConnectionsInterruptingSystem : SystemBase
    {
        private EndNetworkSimulationBuffer _endSimulationBuffer;

        protected override void OnCreate()
        {
            _endSimulationBuffer = World.GetOrCreateSystem<EndNetworkSimulationBuffer>();
        }

        protected override void OnUpdate()
        {
            EntityCommandBuffer commandBuffer = _endSimulationBuffer.CreateCommandBuffer();
            var network = World.GetExtension<INetwork>();

            network.DriverDependency = Entities
                                       .WithName("Clear_interrupted_connections")
                                       .WithEntityQueryOptions(EntityQueryOptions.FilterWriteGroup)
                                       .WithAll<InterruptedNetworkConnection>()
                                       .ForEach((Entity entity, in NetworkConnectionWrapper connectionTarget) =>
                                       {
                                           commandBuffer.DestroyEntity(entity);
                                       })
                                       .Schedule(network.DriverDependency);

            NetworkDriver driver = network.Driver;

            network.DriverDependency = Entities
                                       .WithName("Interrupt_connections")
                                       .WithEntityQueryOptions(EntityQueryOptions.FilterWriteGroup)
                                       .WithAll<InterruptNetworkConnection>()
                                       .ForEach((Entity entity, ref NetworkConnectionWrapper connectionTarget) =>
                                       {
                                           if (!connectionTarget.Connection.IsCreated)
                                           {
                                               connectionTarget.Connection.Disconnect(driver);
                                           }
                                       })
                                       .Schedule(network.DriverDependency);

            Dependency = network.DriverDependency;
            _endSimulationBuffer.AddJobHandleForProducer(network.DriverDependency);
        }
    }
}