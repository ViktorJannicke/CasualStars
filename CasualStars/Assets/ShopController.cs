using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShopController : MonoBehaviour
{

    public GameObject availableShips;
    public GameObject myShips;
    public GameObject CreditsOverview;

    private void Start()
    {
        availableShips.SetActive(true);
        myShips.SetActive(false);
        CreditsOverview.SetActive(false);
    }

    public void showAvailableShipsTab()
    {
        availableShips.SetActive(true);
        myShips.SetActive(false);
        CreditsOverview.SetActive(false);
    }

    public void showMyShipsTab()
    {
        availableShips.SetActive(false);
        myShips.SetActive(true);
        CreditsOverview.SetActive(false);
    }

    public void showCreditsOverviewTab()
    {
        availableShips.SetActive(false);
        myShips.SetActive(false);
        CreditsOverview.SetActive(true);
    }
}
