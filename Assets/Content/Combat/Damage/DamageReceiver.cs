using UnityEngine;

public class DamageReceiver : MonoBehaviour
{
    public DamageDataEvent onTakeDamage;

    public void TakeDamage(DamageData damageData)
    {
        onTakeDamage?.Invoke(damageData);
    }
}
