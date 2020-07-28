using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadGameTransition : MonoBehaviour
{
    public float Zeit;
    public float waitTill = 30f;
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
		if (Zeit >= waitTill)
		{
            sm.LoadGame();
            Zeit = -1000;
		}
			
    }
}
