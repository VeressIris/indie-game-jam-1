using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;

public class VolumeSlider : MonoBehaviour
{
    [SerializeField] private AudioMixer SFXmixer;
    [SerializeField] private AudioMixer BGMmixer;
    [SerializeField] private Slider SFXslider;
    [SerializeField] private Slider BGMslider;
    private float SFXVol;
    private float BGMVol;

    private void Start()
    {
        SFXVol = PlayerPrefs.GetFloat("SFXVol", -5f);
        BGMVol = PlayerPrefs.GetFloat("BGMVol", 0f);
        SFXmixer.SetFloat("SFXVOL", SFXVol);
        BGMmixer.SetFloat("MusicVOL", BGMVol);
        SFXslider.value = SFXVol;
        BGMslider.value = BGMVol;

        SFXslider.onValueChanged.AddListener((v) =>
        {
            SFXmixer.SetFloat("SFXVOL", v);
            SFXVol = v;
            PlayerPrefs.SetFloat("SFXVol", SFXVol);
        });
        BGMslider.onValueChanged.AddListener((v) =>
        {
            BGMmixer.SetFloat("MusicVOL", v);
            BGMVol = v;
            PlayerPrefs.SetFloat("BGMVol", BGMVol);
        });
    }
}
