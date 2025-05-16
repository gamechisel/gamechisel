using UnityEngine;

public class EntityHitbox : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private string _targetID;
    [SerializeField] private Rigidbody _rigidbody;

    public Rigidbody GetRigidbody()
    {
        return _rigidbody;
    }

    public string GetTargetID()
    {
        return _targetID;
    }
}
