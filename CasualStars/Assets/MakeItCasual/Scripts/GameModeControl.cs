using TMPro;
using UnityEngine;

public class GameModeControl : MonoBehaviour
{
    public TMP_Dropdown dropdown;
    public int max;

    public void valueChanged()
    {
        if(dropdown.value > max)
        {
            dropdown.value = max;
        }

        MasterManager.mm.difficulty = dropdown.value;
    }
}
