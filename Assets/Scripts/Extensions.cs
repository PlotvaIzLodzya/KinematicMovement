﻿using UnityEngine;

public static class Extensions
{

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
            point = hit.point,
            normal = hit.normal,
            distance = dist,
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
            point = hit.point,
            normal = hit.normal,
            distance = hit.distance - 0.015f,
            HaveHit = hit.collider != null,
            Transform = hit.transform
        };

        return hitInfo;
    }

}