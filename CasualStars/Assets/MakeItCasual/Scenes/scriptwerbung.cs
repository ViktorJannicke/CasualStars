using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class scriptwerbung : MonoBehaviour
{
	float Zeit;
	public float Max;


    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
		Zeit += Time.deltaTime;
		if (Zeit >= Max)
		{
			SceneManager.LoadSceneAsync("Game");
		}
			
    }
}
