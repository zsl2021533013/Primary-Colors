using UnityEngine;

namespace Script.View_Controller.Character_System.Player
{
    public class DrawSensorArea : MonoBehaviour
    {
        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireCube(transform.position, transform.localScale);
        }
    }
}