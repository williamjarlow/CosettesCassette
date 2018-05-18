using System.Collections;
using System.Collections.Generic;
using UnityEngine;

            // Trying out extensions. Cool shizzle

public static class RotationReset
{
    public static void ResetRotation(this Transform transformToReset)
    {
        transformToReset.localRotation = Quaternion.identity;
    }

}
