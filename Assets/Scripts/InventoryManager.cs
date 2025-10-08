using UnityEngine;

public class InventoryManager : MonoBehaviour
{
    public int itemCount;

    public bool PickupItem(GameObject item)
    {
        switch (item.tag)
        {
            case Constants.TAG_FOOD:
                itemCount++;
                return true;
            default:
                return false;
        }
        
    }
}
