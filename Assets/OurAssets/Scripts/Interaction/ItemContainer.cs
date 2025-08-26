using System;
using System.Collections.Generic;
using UnityEngine;

public abstract class ItemContainer : Interactable
{
    [SerializeField]
    List<string> acceptableItemsList = new List<string>();
    protected HashSet<string> acceptableItems;

    protected Holdable lastAcceptedItem;
    protected readonly HashSet<string> addedItems = new HashSet<string>();

    protected readonly object[] defaultParameters = new object[2];

    protected object[] extraChecksParameters;
    protected Predicate<object[]> extraChecks;

    protected object[] extraLogicParameters;
    protected Action<object[]> extraLogic;

    void Awake() => acceptableItems = new HashSet<string>(acceptableItemsList);

    public override bool Interact(params object[] parameters)
    {
        if (parameters.Length != 1)
        {
            #if UNITY_EDITOR
                Debug.LogWarning($"WARNING: ItemContainer objects need 1 parameter. Received {parameters.Length} parameter(s)");
            #endif
        }
        else
        {
            if (parameters[0] is Holdable item)
            {
                defaultParameters[0] = item;
                defaultParameters[1] = item.name.Replace("(Clone)", "").TrimEnd(' ');
                if (extraChecks == null)
                {
                    #if UNITY_EDITOR
                        Debug.LogWarning($"WARNING: extraChecks is null. Will only check if the item's name is in the list of acceptable items. If this was intended please ignore this message");
                    #endif
                }
                else if (extraChecksParameters == null)
                {
                    #if UNITY_EDITOR
                        Debug.LogWarning($"WARNING: extraChecksParameters is null. Will only check default values: the holdable object and the holdable object's name. If this was intended please ignore this message");
                    #endif
                }
                if (extraLogic == null)
                {
                    #if UNITY_EDITOR
                        Debug.LogWarning($"WARNING: extraLogic is null. Will not do any extra logic. If this was intended please ignore this message");
                    #endif
                }
                else if (extraLogicParameters == null)
                {
                    #if UNITY_EDITOR
                        Debug.LogWarning($"WARNING: extraLogicParameters is null. Will only do logic with the default values: the holdable object and the holdable object's name. If this was intended please ignore this message");
                    #endif
                }
                object[] eCP = extraChecksParameters ?? defaultParameters;
                bool rejected = !acceptableItems.Contains((string)defaultParameters[1]) || (extraChecks != null && !extraChecks(eCP));
                object[] eLP = extraLogicParameters ?? defaultParameters;
                if (!rejected)
                {
                    addedItems.Add((string)defaultParameters[1]);
                    lastAcceptedItem = item;
                    extraLogic?.Invoke(eLP);
                }
                return rejected;
            }
            else
            {
                #if UNITY_EDITOR
                    if (parameters[0] is not Holdable) Debug.LogWarning($"WARNING: Parameter 0 needs to be a holdable object. Received {parameters[0]} type {parameters[0].GetType()} as parameter 0");
                #endif
            }
        }
        return true; // End interaction by rejecting item (if applicable)
    }

    /// <summary>
    /// <para>
    /// This will destroy the item.
    /// </para>
    /// <para>
    /// If you want to make sure the player could grab that item again (in case they
    /// need to use it again for another potion or in the correct potion for example),
    /// you call Instantiate(lastAcceptedItem.gameObject, lastAcceptedItem.StartPos, lastAcceptedItem.StartRot)
    /// then call base.DestroyLastAcceptedItem() this allows for better abstraction
    /// with other containers that don't need to respawn the item.
    /// </para>
    /// </summary>
    public virtual void DestroyLastAcceptedItem()
    {
        if (lastAcceptedItem != null) Destroy(lastAcceptedItem.gameObject);
    }
}
