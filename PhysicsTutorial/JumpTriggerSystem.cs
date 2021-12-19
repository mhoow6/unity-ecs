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
                // 트리거를 발동시킨 엔티티가 PhysicsVelocity를 가지고 있는가?
                if (physicsVelocityEntities.HasComponent(triggerEvent.EntityA))
                {
                    // 트리거를 발동시킨 엔티티의 PhysicsVelocity
                    PhysicsVelocity physicsVelocity = physicsVelocityEntities[triggerEvent.EntityA];
                    // y축으로 5만큼 상승
                    physicsVelocity.Linear.y = 5f;
                    // 상승시킨 physicsVelcoity를 엔티티에 재적용
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
            // Job 생성
            TriggerJob triggerJob = new TriggerJob
            {
                // 현재 오브젝트에 붙여있는 PhysicsVelocity 컴포넌트를 가져온다.
                physicsVelocityEntities = GetComponentDataFromEntity<PhysicsVelocity>()
            };
            // 물리 세계에서 Job 스케쥴링하기
            return triggerJob.Schedule(stepPhysicsWorld.Simulation, ref buildPhysicsWorld.PhysicsWorld, inputDeps);
        }
    }
}

