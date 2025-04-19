using UnityEngine;

public class ObjectAlignment : MonoBehaviour
{
    public LayerMask groundLayer; // Specify the ground layer in the Inspector

    void Start()
    {
        // Iterate through each child object
        foreach (Transform child in transform)
        {
            // Raycast downward from the child object's position, only hitting objects on the ground layer
            RaycastHit hit;
            if (Physics.Raycast(child.position, Vector3.down, out hit, Mathf.Infinity, groundLayer))
            {
                // Align the child object with the ground
                child.position = hit.point;

                // Align the child object's up vector with the ground normal
                child.up = hit.normal;
            }
            else
            {
                Debug.LogWarning("No ground found for child object: " + child.name);
            }
        }
    }
}
