using UnityEngine;

public class DoorClickForwarder : MonoBehaviour
{
    void OnMouseDown()
    {
        //forward the click up to the parent with the script
        transform.parent.GetComponent<LockedDoor>()?.HandleClick(transform.position);
    }
}
