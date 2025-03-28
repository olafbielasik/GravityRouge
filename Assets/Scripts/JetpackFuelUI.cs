using UnityEngine;
using UnityEngine.UI;

public class JetpackFuelUI : MonoBehaviour
{
    public AstroPlayerController player;
    public Image fuelFill;

    void Update()
    {
        if (player != null && fuelFill != null)
        {
            float fuelPercent = player.currentFuel / player.maxFuel;

            Debug.Log("Fuel % = " + fuelPercent);

            fuelFill.fillAmount = fuelPercent;

            if (fuelPercent > 0.5f)
                fuelFill.color = Color.green;
            else if (fuelPercent > 0.2f)
                fuelFill.color = Color.yellow;
            else
                fuelFill.color = Color.red;
        }
    }
}



