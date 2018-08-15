using UnityEngine;

namespace BayatGames.SaveGameFree.Examples
{
    public class ExampleMoveObject : MonoBehaviour
    {
        private void Update()
        {
            var position = transform.position;
            position.x += Input.GetAxis("Horizontal");
            position.y += Input.GetAxis("Vertical");
            transform.position = position;
        }
    }
}