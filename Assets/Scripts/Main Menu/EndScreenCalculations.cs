using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EndScreenCalculations : MonoBehaviour {

    private static Text m_text;
    private static string temp_text;

    private void Start()
    {
        m_text = GetComponent<Text>();
        temp_text = m_text.text;
    }

    public static void Calculate()
    {
        m_text.text = temp_text.Replace("%dnum%", Random.Range(0, 9999).ToString("0000"));
        m_text.text = m_text.text.Replace("%exp_count%", Random.Range(0, 9999).ToString("0000"));
        m_text.text = m_text.text.Replace("%lost_time%", (PlayerController.CommonTime/60).ToString("F0"));
        m_text.text = m_text.text.Replace("%count_sections%", (PlayerController.PassedSections).ToString());
        m_text.text = m_text.text.Replace("%passed_dist%", (PlayerController.PassedSections * 5f).ToString("F1"));
        m_text.text = m_text.text.Replace("%exact_time_sec%", (PlayerController.CommonTime % 60).ToString("F0"));
        m_text.text = m_text.text.Replace("%exact_time_min%", (PlayerController.CommonTime / 60).ToString("F0"));
    }
}
