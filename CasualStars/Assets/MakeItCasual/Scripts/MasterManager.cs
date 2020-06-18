using System.Collections.Generic;
using Unity.Collections;
using UnityEngine;

public class MasterManager : MonoBehaviour
{
    public static MasterManager mm;
    public string nextScene;
    public int AdSpotLength;

    public List<PlayerData> playerData;
    public int lastScore;

    public int difficulty;

    // Start is called before the first frame update
    void Start()
    {
        if(mm != null)
        {
            Destroy(gameObject);
            return;
        }

        mm = this;
        DontDestroyOnLoad(gameObject);

        if (SaveSystem.PlayerDataExists())
        {
            playerData = SaveSystem.LoadPlayer();
        }
        else
        {
            playerData = new List<PlayerData>();
            SaveSystem.SavePlayer(playerData);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
