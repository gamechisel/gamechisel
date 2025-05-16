using UnityEngine;

public class DummyManager : MonoBehaviour
{
    // public DamageReceiver damageReceiver;
    public ParticleSystem hitEffect;

    public void ReceiveDamage(DamageData damageData)
    {
        Debug.Log(damageData.damageAmount);
        hitEffect.Play();
    }
}
