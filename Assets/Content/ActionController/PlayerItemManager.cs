using UnityEngine;

public class PlayerItemManager : MonoBehaviour
{
    [Header("References")]
    public PlayerAction playerAction;
    public bool switch_weapon1;
    public bool switch_weapon2;
    public Sword sowrd;

    [Header("Equipped Items")]
    public Transform rightItem;
    public Transform leftItem;
    public Transform back1Item;
    public Transform back2Item;
    public Transform back3Item;

    [Header("Item References")]
    public Transform rightHand;
    public Transform rightHandHolder;
    public Transform leftHand;
    public Transform leftHandHolder;
    public Transform back;
    public Transform backHolder;

    public void FixedUpdate()
    {
        UpdateItemPos();



    }

    private void UpdateItemPos()
    {
        backHolder.position = back.position;
        backHolder.rotation = back.rotation;
        rightHandHolder.position = rightHand.position;
        rightHandHolder.rotation = rightHand.rotation;
        leftHandHolder.position = leftHand.position;
        leftHandHolder.rotation = leftHand.rotation;
    }

    public void SwitchItem1(bool _state)
    {

    }

    public void SwitchItem2(bool _state)
    {

    }

    public void UseSword(bool _state)
    {
        if (_state)
        {
            sowrd.StartSwing();
        }
        else
        {
            sowrd.EndSwing();
        }
    }

}
