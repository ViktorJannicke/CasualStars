﻿using UnityEngine;
using UnityEngine.SceneManagement;

public class scriptwerbung : MonoBehaviour
{
	float Zeit;
    MasterManager mm;
    public SceneManagement sm;
    void Start()
    {
        mm = MasterManager.mm;
    }

    // Update is called once per frame
    void Update()
    {
		Zeit += Time.deltaTime;
		if (Zeit >= mm.AdSpotLength)
		{
            sm.LoadGameEnd();
            Zeit = -1000;
		}
			
    }

    public void skip()
    {
        sm.LoadGameEnd();
        Zeit = -1000;
    }
}
