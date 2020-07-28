using Unity.Entities;
using Unity.Jobs;
using Unity.Transforms;
using UnityEngine;
using Unity.Mathematics;


[UpdateAfter(typeof(MoveForwardSystem))]
public class TimedDestroySystem : JobComponentSystem
{
	EndSimulationEntityCommandBufferSystem buffer;
	
	 // protected override void OnCreateManager()
	 // {
	 // 	buffer = World.DefaultGameObjectInjectionWorld.GetOrCreateSystem<EndSimulationEntityCommandBufferSystem>();
	 // }

	 protected override void OnCreate()
	 {
		 buffer = World.DefaultGameObjectInjectionWorld.GetOrCreateSystem<EndSimulationEntityCommandBufferSystem>();

	 }

	 struct CullingJob : IJobForEachWithEntity<TimeToLive>
	{
		public EntityCommandBuffer.Concurrent commands;
		public float dt;

		public void Execute(Entity entity, int jobIndex, ref TimeToLive timeToLive)
		{
			timeToLive.Value -= dt;
			if (timeToLive.Value <= 0f)
				commands.DestroyEntity(jobIndex, entity);
		}
	}

	protected override JobHandle OnUpdate(JobHandle inputDeps)
	{
		var job = new CullingJob
		{
			commands = buffer.CreateCommandBuffer().ToConcurrent(),
			dt = Time.DeltaTime
		};

		var handle = job.Schedule(this, inputDeps);
		buffer.AddJobHandleForProducer(handle);

		return handle;
	}
}

/*
public class TimedDestroySystem : SystemBase
{
	public struct Position : IComponentData
	{
		public float3 Value;
	}

	public struct Velocity : IComponentData
	{
		public float3 Value;
	}

	protected override void OnUpdate()
	{
// Local variable captured in ForEach
		float dT = Time.DeltaTime;

		Entities
			.WithName("Culling_Job")
			.ForEach(
				(ref Position position, in Velocity velocity) =>
				{
					position = new Position()
					{
						Value = position.Value + velocity.Value * dT
					};
				}
			)
			.ScheduleParallel();
	}
}
*/

