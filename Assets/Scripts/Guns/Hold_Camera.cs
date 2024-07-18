using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Flashlight))]
public class Hold_Camera : MonoBehaviour {

    public Image[] chargeMeter;
    public Image[] soundMeter;

    public Text soundLevel;
    public Text nightVision;
    public Text noData;

    private float[] listener = new float[64];

    private int spectrumDivider;
    private int chargeDivider;
    private int currentCharge;

    private Flashlight fLight;

    private void Start()
    {
        fLight = GetComponent<Flashlight>();

        spectrumDivider = Mathf.FloorToInt(listener.Length / soundMeter.Length);
        chargeDivider = Mathf.FloorToInt(Flashlight.maxCharge / chargeMeter.Length);
    }

    private void Update()
    {
        ShowCharge();

        if (fLight.currentCharge > 0)
        {
            nightVision.text = (fLight.active) ? "ON" : "OFF";

            if (soundLevel.text == "No Data")
                soundLevel.text = "0";
            AnalyzeSpectrum();

            if (noData.text == "No Data")
                noData.text = "";
        } else
        {
            nightVision.text = "No Data";
            soundLevel.text = "No Data";
            noData.text = "No Data";
        }
    }

    private void ShowCharge()
    {
        currentCharge = Mathf.CeilToInt(fLight.currentCharge / chargeDivider);

        for (int i = 0; i < chargeMeter.Length; i++)
            chargeMeter[i].enabled = (i >= currentCharge) ? false : true;
    }

    private void AnalyzeSpectrum()
    {
        AudioListener.GetSpectrumData(listener, 0, FFTWindow.BlackmanHarris);

        for (int i = 0; i < soundMeter.Length; i++)
        {
            float normalize = 0f;
            float dbSum = 0f;
            for (int o = spectrumDivider * i; o < spectrumDivider * i + spectrumDivider; o++)
            {
                normalize += listener[o];
                dbSum += listener[o] * listener[o];
            }
            soundLevel.text = (Mathf.Lerp(float.Parse(soundLevel.text),
                Mathf.Clamp(10f * Mathf.Log10(Mathf.Sqrt(dbSum / listener.Length) / 0.1f), -60f, 120f), 0.1f)).ToString("F0");

            normalize /= listener.Length;

            soundMeter[i].GetComponent<RectTransform>().sizeDelta = new Vector2(13f, 
                Mathf.Lerp(soundMeter[i].GetComponent<RectTransform>().sizeDelta.y, Mathf.Clamp(normalize * 1000000f, 0f, 63f), 0.1f));
        }
    }
}
