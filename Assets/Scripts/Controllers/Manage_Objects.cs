using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Manage_Objects : MonoBehaviour {

	public enum currentWeapon
    {
        FLASHLIGHT = 0,
        CAMERA
    }
    public static currentWeapon weapon = currentWeapon.FLASHLIGHT;

    public GameObject[] flashlight;
    public GameObject[] _camera;
    public GameObject[] documents;

    private static GameObject[] flashlight_static;
    private static GameObject[] _camera_static;
    private static GameObject[] documents_static;

    private void Awake()
    {
        weapon = currentWeapon.FLASHLIGHT;
    }

    public static void ChooseWeapon(currentWeapon target)
    {
        weapon = target;

        Initialize();
    }

    public static void UseBattery()
    {
        for (int i = 0; i < flashlight_static.Length; i++)
        {
            if (flashlight_static[i].GetComponent<Flashlight>() && flashlight_static[i].activeInHierarchy)
            {
                if (flashlight_static[i].GetComponent<Flashlight>().currentCharge < Flashlight.maxCharge)
                {
                    flashlight_static[i].GetComponent<Flashlight>().Charge();
                    RemoveBattery();
                }
            }
        }

        for (int i = 0; i < _camera_static.Length; i++)
        {
            if (_camera_static[i].GetComponent<Flashlight>() && _camera_static[i].activeInHierarchy)
            {
                if (_camera_static[i].GetComponent<Flashlight>().currentCharge < Flashlight.maxCharge)
                {
                    _camera_static[i].GetComponent<Flashlight>().Charge();
                    RemoveBattery();
                }
            }
        }
    }

    private static void RemoveBattery()
    {
        // remove battery
        Inventory.RemoveItem(3);
    }

    public static void ShowDocument()
    {
        for (int i = 0; i < documents_static.Length; i++)
        {
            documents_static[i].SetActive(true);
        }
    }

    public static void HideDocument_Static()
    {
        for (int i = 0; i < documents_static.Length; i++)
        {
            documents_static[i].SetActive(false);
        }
    }

    public void HideDocument()
    {
        for (int i = 0; i < documents_static.Length; i++)
        {
            documents_static[i].SetActive(false);
        }
    }

    private void Start()
    {
        flashlight_static = flashlight;
        _camera_static = _camera;
        documents_static = documents;

        Initialize();
    }

    private static void Initialize()
    {
        for (int i = 0; i < flashlight_static.Length; i++)
        {
            if (flashlight_static[i].GetComponent<Flashlight>())
            {
                if (flashlight_static[i].GetComponent<Flashlight>().active)
                {
                    if (!flashlight_static[i].activeInHierarchy)
                    {
                        flashlight_static[i].GetComponent<Flashlight>().ToggleFlashlight();
                    }
                }
            }
        }

        for (int i = 0; i < _camera_static.Length; i++)
        {
            if (_camera_static[i].GetComponent<Flashlight>())
            {
                if (_camera_static[i].GetComponent<Flashlight>().active)
                {
                    if (!_camera_static[i].activeInHierarchy)
                    {
                        _camera_static[i].GetComponent<Flashlight>().ToggleFlashlight();
                    }
                }
            }
        }

        switch (weapon)
        {
            case (currentWeapon.CAMERA):
                {
                    for (int i = 0; i < _camera_static.Length; i++)
                    {
                        _camera_static[i].SetActive(true);
                    }

                    for (int i = 0; i < flashlight_static.Length; i++)
                    {
                        flashlight_static[i].SetActive(false);
                    }
                    break;
                }
            case (currentWeapon.FLASHLIGHT):
                {
                    for (int i = 0; i < flashlight_static.Length; i++)
                    {
                        flashlight_static[i].SetActive(true);
                    }

                    for (int i = 0; i < _camera_static.Length; i++)
                    {
                        _camera_static[i].SetActive(false);
                    }
                    break;
                }
        }
    }
}
