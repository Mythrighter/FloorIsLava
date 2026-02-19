using UnityEngine;

public class Scoot : MonoBehaviour
{
    bool isHolding = false;

    [SerializeField]
    float maxDistance = 3f;
    [SerializeField]
    public float x;
    [SerializeField]
    public float y;
    [SerializeField]
    public float z;
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
        Physics.gravity = new Vector3(x, y, z);
        
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
                rb.useGravity = true;
                rb.detectCollisions = true;
                
                

                this.transform.SetParent(tempParent.transform);
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
        }
    }
}
