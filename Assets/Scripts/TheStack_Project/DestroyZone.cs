using UnityEngine;

namespace TheStack_Project
{
    public class DestroyZone : MonoBehaviour
    {
        private void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.name.Equals("Rubble"))
            {
                Destroy(other.gameObject);
            }
        }
    }

}