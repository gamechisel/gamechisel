using UnityEngine;

public class OverrideParentTransform : MonoBehaviour
{
    [SerializeField] private Transform parentObject;

    private void FixedUpdate()
    {
        parentObject.transform.position += this.transform.localPosition;
        this.transform.localPosition = Vector3.zero;
    }
}
