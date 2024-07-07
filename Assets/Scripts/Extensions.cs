using UnityEngine;

public static class Extensions
{

    public static HitInfo ToHitInfo(this RaycastHit2D hit, Collider2D selfCollider)
    {
        var dist = hit.distance - 0.015f;
        bool haveHit = hit.collider != null;
        var hitInfo = new HitInfo()
        {
            point = hit.point,
            normal = hit.normal,
            distance = dist,
            HaveHit = haveHit,
            Transform = hit.transform
        };

        return hitInfo;
    }

    public static HitInfo ToHitInfo(this RaycastHit hit)
    {
        var hitInfo = new HitInfo()
        {
            point = hit.point,
            normal = hit.normal,
            distance = hit.distance,
            HaveHit = hit.collider != null,
            Transform = hit.transform
        };

        return hitInfo;
    }

}
