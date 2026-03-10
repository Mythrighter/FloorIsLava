using UnityEngine;

public class Faucet : MonoBehaviour
{
    public static bool WaterFlow = false;
    public GameObject waterSystem;
  
    public void Update()
    {
        if(Input.GetMouseButtonDown(1))
        {
            if (WaterFlow)
            {
                On();
            }
            else
            {
                Off();
            }
        }
    }

    void On()
    {
        waterSystem.SetActive(true);
    }

    void Off()
    {
        waterSystem.SetActive(false);
    }
}
