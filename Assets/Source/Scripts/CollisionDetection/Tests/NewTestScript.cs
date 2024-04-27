using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using UnityEngine.TestTools.Constraints;

public class NewTestScript
{
    RaycastHit[] hits = new RaycastHit[2];
    // A Test behaves as an ordinary method
    [Test]
    public void NewTestScriptSimplePasses()
    {
        Assert.That(() =>
        {
            //Physics.SphereCastNonAlloc(Vector3.zero, 3, Vector3.zero, hits, 3f);
            //Physics.CheckSphere(Vector3.zero, 3f);
            //Physics.SphereCast(Vector3.zero, 3, Vector3.zero, out RaycastHit hit, 3f);
        }, UnityEngine.TestTools.Constraints.Is.Not.AllocatingGCMemory());
    }

    // A UnityTest behaves like a coroutine in Play Mode. In Edit Mode you can use
    // `yield return null;` to skip a frame.
    [UnityTest]
    public IEnumerator NewTestScriptWithEnumeratorPasses()
    {
        // Use the Assert class to test conditions.
        // Use yield to skip a frame.
        yield return null;

        Assert.That(() =>
        {
            Physics.SphereCastNonAlloc(Vector3.zero, 3, Vector3.zero, hits, 3f);
            //Physics.CheckSphere(Vector3.zero, 3f);
            //Physics.SphereCast(Vector3.zero, 3, Vector3.zero, out RaycastHit hit, 3f);
        }, UnityEngine.TestTools.Constraints.Is.Not.AllocatingGCMemory());
    }
}
