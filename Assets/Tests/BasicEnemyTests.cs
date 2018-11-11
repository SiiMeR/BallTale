using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace Tests
{
    public class BasicEnemyTests
    {
        // A Test behaves as an ordinary method
        [Test]
        public void DemoTestSimplePasses()
        {
            // Use the Assert class to test conditions
        }

        [UnityTest]
        public IEnumerator TestBasicEnemyMovesProperly()
        {
            yield return null;
        }
    }
}
