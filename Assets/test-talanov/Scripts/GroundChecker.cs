using UnityEngine;

namespace Dolls.Movement
{
    public class GroundChecker : MonoBehaviour
    {
        [SerializeField] private PlayerController playerController;

        private void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.CompareTag("Ground")) playerController.SetGrounded(true);
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.gameObject.CompareTag("Ground")) playerController.SetGrounded(false);
        }

    }
}