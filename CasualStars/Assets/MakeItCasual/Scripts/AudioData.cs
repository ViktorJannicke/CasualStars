
[System.Serializable]
public class AudioData
{
    public enum AudioType { music, effects, ads};
    
    public float[] sliderValues = new float[3];

    public int version = 0;

    public AudioData(float _musicSlider, float _effectsSlider, float _adsSlider)
    {
        sliderValues[0] = _musicSlider;
        sliderValues[1] = _effectsSlider;
        sliderValues[2] = _adsSlider;
    }
}