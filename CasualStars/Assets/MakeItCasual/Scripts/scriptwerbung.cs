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
            if (mm.nextScene == "Game")
            {
                SceneManager.LoadSceneAsync(mm.nextScene);
            }
            else
            {
                SceneManager.UnloadSceneAsync("Werbung");
                SceneManager.LoadSceneAsync(mm.nextScene, LoadSceneMode.Additive);
            }
            Zeit = -1000;
		}
			
    }

    public void skip()
    {
        if (mm.nextScene == "Game")
        {
            SceneManager.LoadSceneAsync(mm.nextScene);
        }
        else
        {
            SceneManager.UnloadSceneAsync("Werbung");
            SceneManager.LoadSceneAsync(mm.nextScene, LoadSceneMode.Additive);
        }
    }
}
