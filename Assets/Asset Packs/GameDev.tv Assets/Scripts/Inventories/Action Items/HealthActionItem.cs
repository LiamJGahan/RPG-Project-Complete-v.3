using GameDevTV.Inventories;
using RPG.Attributes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = ("GameDevTV/GameDevTV.UI.InventorySystem/Health Potion"))]
public class HealthActionItem : ActionItem
{
    [SerializeField] float healthPointsToRestore = 20f;

    public override bool Use(GameObject user)
    {
        user.GetComponent<Health>().Heal(healthPointsToRestore);

        return true;
    }
}
