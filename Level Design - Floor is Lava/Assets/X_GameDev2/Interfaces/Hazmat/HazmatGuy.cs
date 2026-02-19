using UnityEngine;
using System.Collections;

public class HazmatGuy : MonoBehaviour
{
    public int health = 10;
    public MeshRenderer rend;

    public Material normal;
    public Material red;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void TakeDamage()
    {
        //Take damage.
        health -= 1;

        //Swap materials on the guy to make it flash red when hit.
        //Note: This is a quick solution. Probably better ones out there. 
        StopAllCoroutines();
        StartCoroutine(FlashRed());

        //When the enemy dies...
        if (health <= 0)
        {
            PlayDeathAnimation();
            DropAmmo();
            Destroy(gameObject);
        }
    }

    private IEnumerator FlashRed()
    {
        //Swap the material on the barrelt to a white one.
        //Once time is elapsed, the barrel returns to its normal material.
        rend.material = red;
        yield return new WaitForSeconds(0.05f);
        rend.material = normal;
    }

    private void DropAmmo()
    {
        //Drop ammo for player.
    }

    private void PlayDeathAnimation()
    {
        //Play death animation.
    }
}
