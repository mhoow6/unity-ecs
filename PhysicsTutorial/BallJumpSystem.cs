using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Physics;
using UnityEngine;

namespace ECSPhysicsTutorial
{
    public class BallJumpSystem : ComponentSystem
    {
        protected override void OnUpdate()
        {
            Entities.ForEach((ref PhysicsVelocity physicsVelocity) =>
            {
                // 점프할 때 마다 y축으로 5만큼의 힘을 가함
                if (Input.GetKeyDown(KeyCode.Space))
                {
                    physicsVelocity.Linear.y = 5f;
                }
            });
        }
    }
}

