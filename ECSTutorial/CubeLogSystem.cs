using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

namespace ECSTutorial
{
    // ECS - System
    // ������Ʈ�� ���� ������ �����Ѵ�.
    public class CubeLogSystem : ComponentSystem
    {
        protected override void OnUpdate()
        {
            Entities.ForEach((ref CubeComponent comp) =>
            {
                // Debug.Log(comp.speed);
            });
        }
    }
}

