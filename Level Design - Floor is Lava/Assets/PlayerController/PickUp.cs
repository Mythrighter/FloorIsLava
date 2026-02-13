using UnityEngine;

public class PickUp : MonoBehaviour
{
    bool canPick;
    private bool isHolding = false;
    //private float maxHeldObjects = 1f;
    private float currentHeldObjects = 0f;
    [SerializeField] GameObject target;
    [SerializeField] GameObject centered;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E) && canPick == true)
        {
            
            if (isHolding)
            {
                DropObject();
            }
            else
            {
                if (currentHeldObjects == 0f)
                {
                    PickUpObject();
                    Debug.Log("Holding Object");
                }
                else
                {
                    DropObject();
                    Debug.Log("Dropped Object");
                }

            }
            

        }

    }

    void PickUpObject()
    {
        this.transform.parent = target.transform;
        
        
        //Transform centered = pickUpCenter.transform;

        
        this.transform.localEulerAngles = new Vector3(0, 0, 0); //this is a bit janky..
                     //is just as likely to move around. how do I make it consistent?
        this.GetComponent<Rigidbody>().isKinematic = true;
        currentHeldObjects = 1;

        if (currentHeldObjects == 1)
        {
            this.transform.position = target.transform.position;
        }
    }

    void DropObject()
    {
        this.transform.parent = null;
        this.GetComponent<Rigidbody>().isKinematic = false;
        currentHeldObjects = 0;
    }

    void OnTriggerEnter(Collider other)
    {
        canPick = true;
    }

    void OnTriggerExit(Collider other)
    {
        canPick = false;
    }

} 
