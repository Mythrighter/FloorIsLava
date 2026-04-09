using Unity.Cinemachine;
using UnityEngine;

public class PickUp : MonoBehaviour
{
    bool isHolding = false;

    [SerializeField]
    float throwForce = 300;
    [SerializeField]
    float maxDistance = 3f;

    float distance;


    TempParent tempParent;
    Rigidbody rb;

    Vector3 objectPos;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        tempParent = TempParent.Instance;
        
    }

    // Update is called once per frame
    void Update()
    {
        if(isHolding)
        {
            Hold();
        }
    }

    void OnMouseDown()
    {
        //pickup
        if(tempParent != null)
        {
            distance = Vector3.Distance(this.transform.position, tempParent.transform.position);

            if (distance <= maxDistance)
            {
                isHolding = true;
                rb.useGravity = false;
                rb.detectCollisions = true;
                                
                this.transform.SetParent(tempParent.transform);

                Item item = GetComponent<Item>();
                if (item != null) item.isHeldByPlayer = true;

                if (tag == "CouchCushion")
                {
                    ScoreManager.sManager.IncreaseScoreCouchCushion(10);
                }
                
                if (tag == "BigCushion")
                {
                    ScoreManager.sManager.IncreaseScoreBigCushion(20);
                }

                if (tag == "Bowl")
                {
                    ScoreManager.sManager.IncreaseScoreBowl(5);
                }
                
            }

        }
        else
        {
            Debug.Log("Temp Parent item not found in scene");
        }
    }

    private void OnMouseUp()
    {
        Drop();
    }

    private void OnMouseExit()
    {
        Drop();
    }

    private void Hold()
    {
        distance = Vector3.Distance(this.transform.position, tempParent.transform.position);

        if(distance >= maxDistance)
        {
            Drop();
        }

        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;

        if(Input.GetMouseButtonDown(1))
        {
            rb.AddForce(tempParent.transform.forward * throwForce);
            Drop();
        }
    }

    private void Drop()
    {
        if(isHolding)
        {
            isHolding = false;
            objectPos = this.transform.position;
            this.transform.position = objectPos;
            this.transform.SetParent(null);
            rb.useGravity = true;

            Item item = GetComponent<Item>();
            if (item != null) item.isHeldByPlayer = false;
        }
    }
}
