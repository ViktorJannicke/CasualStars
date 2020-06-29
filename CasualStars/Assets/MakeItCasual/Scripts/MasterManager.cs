using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class MasterManager : MonoBehaviour
{
    public static MasterManager mm;
    public string nextScene;
    public int AdSpotLength;

    public List<PlayerData> playerData;
    public AudioData audioData;
    public int lastScore;

    public int difficulty;

    public AudioMixer mixer;

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


        if (SaveSystem.AudioDataExists())
        {
            audioData = SaveSystem.LoadAudio();
        }
        if (SaveSystem.PlayerDataExists())
        {
            playerData = SaveSystem.LoadPlayer();
        }
        if(!SaveSystem.PlayerDataExists() || audioData.notFirstStart1 == false)
        {
            Debug.Log("reset Data");
            playerData = new List<PlayerData>();
            SaveSystem.SavePlayer(playerData);
        }
        if (!SaveSystem.AudioDataExists() || audioData.notFirstStart1 == false)
        {
            audioData = new AudioData(0.5f, 0.5f, 0.25f);
            audioData.notFirstStart1 = true;
            SaveSystem.SaveAudio(audioData);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
