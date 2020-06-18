using System.Collections.Generic;
using Unity.Collections;
using UnityEngine;

public class MasterManager : MonoBehaviour
{
    public static MasterManager mm;
    public string nextScene;
    public int AdSpotLength;

    public List<PlayerData> playerDatas = new List<PlayerData>();

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

        playerDatas.Add(new PlayerData("I", 0));
        playerDatas.Add(new PlayerData("Bob", 10000000));
        playerDatas.Add(new PlayerData("Sam", 2500));
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
