using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class scriptwerbung : MonoBehaviour
{
	float Zeit;
    MasterManager mm;
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
			SceneManager.LoadSceneAsync(mm.nextScene);
		}
			
    }
}
