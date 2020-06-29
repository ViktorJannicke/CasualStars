using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class AudioSettingsControl : MonoBehaviour
{
    public Slider slider;
    public AudioMixer mixer;
    public string group;
    public AudioData.AudioType audioType;

    public bool set;

    // Start is called before the first frame update
    void Start()
    {
        
        slider.value = MasterManager.mm.audioData.sliderValues[(int)audioType];
        SetLevel(slider.value);

        Debug.Log(audioType + " Slider: " + MasterManager.mm.audioData.sliderValues[(int)audioType]);
    }

    private void Update()
    {
        if(set)
        {
            set = false;
            SetLevel(slider.value);
        }
    }

    public void SetLevel(float sliderValue)
    {
        mixer.SetFloat(group, Mathf.Log10(sliderValue) * 20);
        MasterManager.mm.audioData.sliderValues[(int)audioType] = sliderValue;
        SaveSystem.SaveAudio(MasterManager.mm.audioData);
    }


}
