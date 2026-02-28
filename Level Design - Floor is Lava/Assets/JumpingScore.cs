using UnityEngine;

public class JumpingScore : MonoBehaviour
{
    void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.CompareTag("CouchCushion"))
        {
            ScoreManager.sManager.IncreaseCushionWalk(5);
        }

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}


//on trigger enter + tag = score certain points
//connect to score manager