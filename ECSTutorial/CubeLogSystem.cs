using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

namespace ECSTutorial
{
    // ECS - System
    // 컴포넌트에 대한 로직만 정의한다.
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

