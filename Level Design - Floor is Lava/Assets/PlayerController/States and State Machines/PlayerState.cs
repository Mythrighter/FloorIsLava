using UnityEngine;

//Intermediate state for states that belong to the player.
//***We can create other intermediate states for other objects. Not just the player.
public class PlayerState : State
{
    protected Player player;

    private void Start()
    {
        //Get Components.
        player = transform.root.GetComponent<Player>();
    }
}
