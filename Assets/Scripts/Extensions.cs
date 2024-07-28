using System;
using System.Collections.Generic;
using UnityEditor.Build;
using UnityEditor.Rendering;
using UnityEngine;

public static class Extensions
{
    public static float ScaledRadius(this SphereCollider sphereCollider)
    {
        return sphereCollider.radius * sphereCollider.transform.lossyScale.y;
    }

    public static bool TryAdd<T>(this List<T> list, T item)
    {
        if (list.Contains(item) == false)
        {
            list.Add(item);
            return true;
        }

        return false;
    }

    public static Vector3 RoundFloat(this Vector3 value, int digits = 5)
    {
        value.x = (float)Math.Round(value.x, digits);
        value.y = (float)Math.Round(value.y, digits);
        value.z = (float)Math.Round(value.z, digits);

        return value;
    }

    public static float GetMax(this Vector3 vector)
    {
        return Mathf.Max(vector.x, vector.y, vector.z);
    }

    public static Vector3 RotateAroundPivot(this Vector3 point, Vector3 pivot, Quaternion angle)
    {
        return angle * (point - pivot) + pivot;
    }

    public static HitInfo ToHitInfo(this RaycastHit2D hit, Collider2D selfCollider)
    {
        var dist = hit.distance;
        bool haveHit = hit.collider != null;
        var collDist = 0f;
        if (haveHit)
        {
            var d = hit.collider.Distance(selfCollider);
            collDist = d.distance;
        }
        var hitInfo = new HitInfo()
        {
            Point = hit.point,
            Normal = hit.normal,
            Distance = dist,
            HaveHit = haveHit,
            Transform = hit.transform,
            ColliderDistance = collDist,
        };

        return hitInfo;
    }

    public static HitInfo ToHitInfo(this RaycastHit hit)
    {
        var hitInfo = new HitInfo()
        {
            Point = hit.point,
            Normal = hit.normal,
            Distance = hit.distance,
            ColliderDistance = hit.distance + MovementConfig.ContactOffset,
            HaveHit = hit.collider != null,
            Transform = hit.transform
        };

        return hitInfo;
    }
}

public static class VectorEnhance
{
    public static Vector3 ClampHorizontalPosition(Vector3 vector, Vector3 min, Vector3 max)
    {
        vector.x = Mathf.Clamp(vector.x, min.x, max.x);
        vector.z = Mathf.Clamp(vector.z, min.z, max.z);

        return vector;
    }

    public static Vector2 ClampPosition(Vector2 vector, Vector2 min, Vector2 max)
    {
        vector.x = Mathf.Clamp(vector.x, min.x, max.x);
        vector.y = Mathf.Clamp(vector.y, min.y, max.y);

        return vector;
    }

    public static Vector2 ClampMagnitude(Vector2 vector, float min, float max)
    {
        vector = Vector2.ClampMagnitude(vector, max);
        vector = ClampMagnitude(vector, min);

        return vector;
    }

    public static Vector2 ClampMagnitude(Vector2 vector, float minLength)
    {
        float num = vector.sqrMagnitude;
        if (num < minLength * minLength)
        {
            float num2 = (float)Math.Sqrt(num);
            float num3 = vector.x / num2;
            float num4 = vector.y / num2;
            return new Vector2(num3 * minLength, num4 * minLength);
        }

        return vector;
    }

    public static Vector3 ClampMin(this Vector3 vectorA, Vector3 min)
    {
        vectorA.x = Mathf.Clamp(vectorA.x, min.x, vectorA.x);
        vectorA.y = Mathf.Clamp(vectorA.y, min.y, vectorA.y);
        vectorA.z = Mathf.Clamp(vectorA.z, min.z, vectorA.z);

        return vectorA;
    }

    public static Vector3 ClampMagnitude(this Vector3 vector, float minMagnitude, float maxMagnitude) 
    {
        vector = vector.ClampMagnitude(minMagnitude);
        vector = Vector3.ClampMagnitude(vector, maxMagnitude);

        return vector;
    }

    public static Vector3 ClampMagnitude(this Vector3 vector, float minLength)
    {
        float magnitude = vector.magnitude;
        if (magnitude < minLength)
        {
            float x = vector.x / magnitude;
            float y = vector.y / magnitude;
            float z = vector.z / magnitude;
            return new Vector3(x * minLength, y * minLength, z * minLength);
        }

        return vector;
    }

    public static Vector3 Horizontal(this Vector3 vector)
    {
        vector.y = 0;
        return vector;
    }

    public static bool IsSameDirection(this Vector3 vectorA, Vector3 vectorB)
    {
        return Vector3.Angle(vectorA, vectorB) < 90;
    }
}
