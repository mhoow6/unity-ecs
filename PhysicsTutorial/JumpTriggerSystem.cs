using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Jobs;
using UnityEngine;
using Unity.Physics;
using Unity.Mathematics;
using Unity.Burst;
using Unity.Physics.Systems;

namespace ECSPhysicsTutorial
{
    public class JumpTriggerSystem : JobComponentSystem
    {
        BuildPhysicsWorld buildPhysicsWorld;
        StepPhysicsWorld stepPhysicsWorld;

        [BurstCompile]
        struct TriggerJob : ITriggerEventsJob
        {
            public ComponentDataFromEntity<PhysicsVelocity> physicsVelocityEntities;

            public void Execute(TriggerEvent triggerEvent)
            {
                // Ʈ���Ÿ� �ߵ���Ų ��ƼƼ�� PhysicsVelocity�� ������ �ִ°�?
                if (physicsVelocityEntities.HasComponent(triggerEvent.EntityA))
                {
                    // Ʈ���Ÿ� �ߵ���Ų ��ƼƼ�� PhysicsVelocity
                    PhysicsVelocity physicsVelocity = physicsVelocityEntities[triggerEvent.EntityA];
                    // y������ 5��ŭ ���
                    physicsVelocity.Linear.y = 5f;
                    // ��½�Ų physicsVelcoity�� ��ƼƼ�� ������
                    physicsVelocityEntities[triggerEvent.EntityA] = physicsVelocity;
                }
                
                if (physicsVelocityEntities.HasComponent(triggerEvent.EntityB))
                {
                    PhysicsVelocity physicsVelocity = physicsVelocityEntities[triggerEvent.EntityB];
                    physicsVelocity.Linear.y = 5f;
                    physicsVelocityEntities[triggerEvent.EntityB] = physicsVelocity;
                }
            }
        }

        protected override void OnCreate()
        {
            buildPhysicsWorld = World.GetOrCreateSystem<BuildPhysicsWorld>();
            stepPhysicsWorld = World.GetOrCreateSystem<StepPhysicsWorld>();
        }
        protected override JobHandle OnUpdate(JobHandle inputDeps)
        {
            // Job ����
            TriggerJob triggerJob = new TriggerJob
            {
                // ���� ������Ʈ�� �ٿ��ִ� PhysicsVelocity ������Ʈ�� �����´�.
                physicsVelocityEntities = GetComponentDataFromEntity<PhysicsVelocity>()
            };
            // ���� ���迡�� Job �����층�ϱ�
            return triggerJob.Schedule(stepPhysicsWorld.Simulation, ref buildPhysicsWorld.PhysicsWorld, inputDeps);
        }
    }
}

