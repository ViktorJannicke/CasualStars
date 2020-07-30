using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Audio;

public class MasterManager : MonoBehaviour
{
    public static MasterManager mm;
    public int AdSpotLength;

    public List<PlayerData> playerData;
    public AudioData audioData;
    public int lastScore;
    public int maxScore;

    public int difficulty;
    public int maxDifficulty;

    public AudioMixer mixer;
    public int version = 2;
    public bool firstStart;

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
        if(!SaveSystem.PlayerDataExists() || audioData.version != version)
        {
            Debug.Log("reset Data");
            firstStart = true;
            playerData = new List<PlayerData>();
            SaveSystem.SavePlayer(playerData);
        }
        if (!SaveSystem.AudioDataExists() || audioData.version != version)
        {
            audioData = new AudioData(0.5f, 0.5f, 0.25f);
            audioData.version = version;
            SaveSystem.SaveAudio(audioData);
        }
    }
}
