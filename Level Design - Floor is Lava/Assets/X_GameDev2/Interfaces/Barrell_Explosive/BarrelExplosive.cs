using UnityEngine;
using System.Collections;

public class BarrelExplosive : MonoBehaviour
{
    public int hits_before_explode = 3;
    public MeshRenderer rend;

    public Material normal;
    public Material white;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void RespondToHit()
    {
        //Take a hit.
        hits_before_explode -= 1;

        //Swap materials on the barrel to make it flash white when hit.
        //Note: This is a quick solution. Probably better ones out there. 
        StopAllCoroutines();
        StartCoroutine(FlashWhite());

        //Explode when no health left.
        if(hits_before_explode <= 0)
        {
            Explode();
        }
    }

    private IEnumerator FlashWhite()
    {
        //Swap the material on the barrel to a white one.
        //Once time is elapsed, the barrel returns to its normal material.
        rend.material = white;
        yield return new WaitForSeconds(0.05f);
        rend.material = normal;
    }

    private void Explode()
    {
        Destroy(gameObject);
    }


}
