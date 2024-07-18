using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Flashlight : MonoBehaviour {

    public static float maxCharge { get; private set; } = 100f;
    public float currentCharge { get; private set; } = 100f;

    public bool doDischarge = true;

    public float intensityMultiplier = 0.5f;
    public float lowIntensity = 0.3f;
    public float ultralowIntensity = 0.2f;
    public float startCharge = 100f;
    public float dischargeSpeed = 2f; // per random time

    public bool enabledOnStart = false;

    public Light[] lights;
    public Light[] pointLights;
    [Space(5)]
    public AudioClip[] enableSound;
    public AudioClip[] disableSound;

    public AudioSource source;

    [HideInInspector]
    public bool active = false;

    private List<float> intensityOnStart = new List<float>();

    private void Start()
    {
        if (startCharge > maxCharge)
        {
            startCharge = maxCharge;
        }

        active = enabledOnStart;
        currentCharge = startCharge;

        // check if all "light" are in place
        for (int i = 0; i < lights.Length; i++)
        {
            if (!lights[i])
            {
#if !UNITY_EDITOR
                ErrorLogger.Log("ERROR: Empty lights in flashlight's lights array, this");
#else
                Debug.LogError("ERROR: Empty lights in flashlight's lights array");
#endif
            }
        }

        for (int i = 0; i < pointLights.Length; i++)
        {
            if (!pointLights[i])
            {
#if !UNITY_EDITOR
                ErrorLogger.Log("ERROR: Empty lights in flashlight's pointLights array, this");
#else
                Debug.LogError("ERROR: Empty lights in flashlight's pointLights array");
#endif
            }
        }

        for (int i = 0; i < lights.Length; i++)
        {
            lights[i].enabled = enabledOnStart;

            lights[i].intensity *= intensityMultiplier;
            intensityOnStart.Add(lights[i].intensity);
        }

        for (int i = 0; i < pointLights.Length; i++)
        {
            pointLights[i].enabled = !enabledOnStart;
        }

        if (doDischarge)
        {
            StartCoroutine(DoDischarge());
            StartCoroutine(DoLowChargeBlink());
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.F))
        {
            ToggleFlashlight();
        }

        if (currentCharge <= 0)
        {
            active = false;

            for (int i = 0; i < lights.Length; i++)
            {
                lights[i].enabled = false;
            }

            for (int i = 0; i < pointLights.Length; i++)
            {
                pointLights[i].enabled = false;
            }
        }
    }

    public void Charge(float amount)
    {
        currentCharge += amount;
    }

    public void Charge()
    {
        currentCharge = maxCharge;
    }

    public void ToggleFlashlight()
    {
        active = !active;

        for (int i = 0; i < lights.Length; i++)
        {
            lights[i].enabled = !lights[i].enabled;
        }

        for (int i = 0; i < pointLights.Length; i++)
        {
            pointLights[i].enabled = !lights[0].enabled;
        }

        if (active)
        {
            if (enableSound.Length > 0)
            {
                source.clip = enableSound[Random.Range(0, enableSound.Length)];
                source.Play();
            }
        } else
        {
            if (source.isPlaying)
            {
                source.Stop();
            }

            if (disableSound.Length > 0)
            {
                source.clip = disableSound[Random.Range(0, disableSound.Length)];
                source.Play();
            }
        }
    }

    private IEnumerator DoDischarge()
    {
        while(true)
        {
            if (doDischarge && active)
            {
                yield return new WaitForSeconds(Random.Range(3f, 10f));
                currentCharge -= dischargeSpeed;
            } else
            {
                yield return null;
            }
        }
    }

    private IEnumerator DoLowChargeBlink()
    {
        while (true)
        {
            yield return new WaitForSeconds((currentCharge > 10f) ? Random.Range(5f, 13f) : Random.Range(1f, 3f));

            if ((currentCharge < 40f && currentCharge > 10f) && active)
            {
                for (int i = 0; i < lights.Length; i++)
                {
                    lights[i].intensity = intensityOnStart[i] * lowIntensity;
                }

                int rand = Random.Range(3, 10);
                rand += (rand % 2 == 0) ? 0 : 1;
                for (int o = 0; o < rand; o++)
                {
                    for (int i = 0; i < lights.Length; i++)
                    {
                        lights[i].enabled = !lights[i].enabled;
                    }
                    yield return new WaitForSeconds(Random.Range(0.01f, 0.2f));
                }
                
            }
            else if ((currentCharge <= 10f) && active)
            {
                for (int i = 0; i < lights.Length; i++)
                {
                    lights[i].intensity = intensityOnStart[i] * ultralowIntensity;
                }

                int rand = Random.Range(5, 16);
                rand += (rand % 2 == 0) ? 0 : 1;
                for (int o = 0; o < rand; o++)
                {
                    for (int i = 0; i < lights.Length; i++)
                    {
                        lights[i].enabled = !lights[i].enabled;
                    }
                    yield return new WaitForSeconds(Random.Range(0.01f, 0.2f));
                }
            }
            else
            {
                for (int i = 0; i < lights.Length; i++)
                {
                    lights[i].intensity = intensityOnStart[i];
                }
                yield return null;
            }
        }
    }
}
