using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;


namespace ECSTutorial
{
    public class CubeRotateSystem : ComponentSystem
    {
        protected override void OnUpdate()
        {
            Entities.ForEach((ref Rotation rot, ref CubeComponent comp) =>
            {
                // 현재 쿼터니언에서, yaw축에 대해 Time.deltaTime * comp.speed만큼 회전한, 쿼터니언 값
                var desired = math.mul(math.normalizesafe(rot.Value), quaternion.AxisAngle(math.up(), Time.DeltaTime * comp.speed));

                rot = new Rotation { Value = desired };
            });
        }
    }
}

