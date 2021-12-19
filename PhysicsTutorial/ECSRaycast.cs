using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Physics.Systems;

namespace ECSPhysicsTutorial
{
    public class ECSRaycast : MonoBehaviour
    {
        Entity Raycast(float3 fromPosition, float3 toPosition)
        {
            #region World 설명
            // 게임에서 ECS의 System을 다루는 오브젝트
            // 게임을 실행하면 Default World가 자동 생성
            #endregion

            BuildPhysicsWorld bpw = World.DefaultGameObjectInjectionWorld.GetExistingSystem<BuildPhysicsWorld>();
            CollisionWorld cw = bpw.PhysicsWorld.CollisionWorld;

            RaycastInput input = new RaycastInput
            {
                Start = fromPosition,
                End = toPosition,
                Filter = new CollisionFilter
                {
                    // Collider가 속한 Layer
                    BelongsTo = ~0u,
                    // Collider가 충돌하고 싶은 Layer
                    CollidesWith = ~0u,
                    // 어떤 오브젝트와도 충돌을 원하지 않을 경우 음수값
                    GroupIndex = 0
                }
            };
            Unity.Physics.RaycastHit hit = new Unity.Physics.RaycastHit();

            // 누군가가 ray에 맞게 될 경우
            if (cw.CastRay(input, out hit))
            {
                // Entity의 Index 접근
                Entity hitEntity = bpw.PhysicsWorld.Bodies[hit.RigidBodyIndex].Entity;
                return hitEntity;
            }
            else
            {
                return Entity.Null;
            }
        }

        void Update()
        {
            // 왼쪽 버튼을 누를 경우
            if (Input.GetMouseButtonDown(0))
            {
                // 마우스 포인터를 기준으로 한 Ray 생성
                UnityEngine.Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                // Ray의 거리
                float rayDistance = 100f;
                // 현재 Ray의 위치에서 Ray 방향 * 100 만큼의 위치까지 Raycast
                Debug.Log(Raycast(ray.origin, ray.direction * rayDistance));
            }
        }
    }
}

