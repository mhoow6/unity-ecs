using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;
using Unity.Transforms;

namespace ECSTutorial
{
    public class CubeMoveSystem : ComponentSystem
    {
        protected override void OnUpdate()
        {
            Entities.ForEach((ref Translation translation, ref CubeComponent comp) =>
            {
                translation.Value.y += comp.speed * Time.DeltaTime * 0.1f;
            });
        }
    }
}

