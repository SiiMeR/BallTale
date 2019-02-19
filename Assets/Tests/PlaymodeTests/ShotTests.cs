using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace Tests
{
    public class ShotTests
    {
        // A Test behaves as an ordinary method
        [Test]
        public void NewTestScriptSimplePasses()
        {
            // Use the Assert class to test conditions
        }

        // A UnityTest behaves like a coroutine in Play Mode. In Edit Mode you can use
        // `yield return null;` to skip a frame.
        [UnityTest]
        public IEnumerator Shot_IsDestroyedWhen_OverMaxRange() // TODO make work
        {
            var shotGameObject = TestHelpers.InstantiatePrefab("shot");
            var shot = shotGameObject.GetComponent<Shot>();
            var shotData = new ShotData(Vector2.right, 10, 10);
            shot.ShotData = shotData;
            yield return null;
            // Use the Assert class to test conditions.
            // Use yield to skip a frame.
//            var timer = 0f;
//                
//            while ((timer += Time.deltaTime) < 5.0f)
//            {        
//                Debug.Log(Time.deltaTime);
//                Debug.Log("true");
//                yield return null;
//            }
//          //  yield return null;
//            Debug.Log(shot._distanceCovered);
//            Assert.IsTrue(shot.IsBeingDestroyed);
        }
    }
}