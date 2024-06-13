using System.Collections.Generic;
using UnityEngine;

public class PickupStatModifiers : PickUpItem
{
    [SerializeField] private List<CharacterStat> statsModifier = new List<CharacterStat>();
    protected override void OnPickedUp(GameObject receiver)
    {

        CharacterStatHandler statHandler = receiver.GetComponent<CharacterStatHandler>();
        foreach (CharacterStat stat in statsModifier)
        {
            statHandler.AddStatModifier(stat);
        }

        // HPBar Refresh¿ë
        HealthSystem healthSystem = receiver.GetComponent<HealthSystem>();
        healthSystem.ChangeHealth(0);
    }
}