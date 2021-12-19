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
            #region World ����
            // ���ӿ��� ECS�� System�� �ٷ�� ������Ʈ
            // ������ �����ϸ� Default World�� �ڵ� ����
            #endregion

            BuildPhysicsWorld bpw = World.DefaultGameObjectInjectionWorld.GetExistingSystem<BuildPhysicsWorld>();
            CollisionWorld cw = bpw.PhysicsWorld.CollisionWorld;

            RaycastInput input = new RaycastInput
            {
                Start = fromPosition,
                End = toPosition,
                Filter = new CollisionFilter
                {
                    // Collider�� ���� Layer
                    BelongsTo = ~0u,
                    // Collider�� �浹�ϰ� ���� Layer
                    CollidesWith = ~0u,
                    // � ������Ʈ�͵� �浹�� ������ ���� ��� ������
                    GroupIndex = 0
                }
            };
            Unity.Physics.RaycastHit hit = new Unity.Physics.RaycastHit();

            // �������� ray�� �°� �� ���
            if (cw.CastRay(input, out hit))
            {
                // Entity�� Index ����
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
            // ���� ��ư�� ���� ���
            if (Input.GetMouseButtonDown(0))
            {
                // ���콺 �����͸� �������� �� Ray ����
                UnityEngine.Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                // Ray�� �Ÿ�
                float rayDistance = 100f;
                // ���� Ray�� ��ġ���� Ray ���� * 100 ��ŭ�� ��ġ���� Raycast
                Debug.Log(Raycast(ray.origin, ray.direction * rayDistance));
            }
        }
    }
}

