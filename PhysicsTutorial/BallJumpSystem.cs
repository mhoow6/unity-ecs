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
                // ������ �� ���� y������ 5��ŭ�� ���� ����
                if (Input.GetKeyDown(KeyCode.Space))
                {
                    physicsVelocity.Linear.y = 5f;
                }
            });
        }
    }
}

