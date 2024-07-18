using System.Collections;
using UnityEngine;

[RequireComponent(typeof(HeadBob))]
public class CameraController : MonoBehaviour {

    public enum mode { LOCKED = 0, HIDDEN, LOCKED_HIDDEN };
    public static mode cursorMode = mode.LOCKED_HIDDEN;

    [Header("General")]
    public float sensitivity = 3f;

    [Header("Advanced")]
    public float minY = -90;
    public float maxY = 90;
    [Range(0, 4)] public int VSYNCMode = 1;
    [Range(1, 300)] public int maxFPS = 60;

    [Header("GUI")]
    public GameObject[] pause;
    public GameObject[] inventory;
    public GameObject[] endScreen;
    public float noiseDuration = 2f;
    public AudioClip noiseSound;
    public GameObject noisePicture;

    private float posX;
    private float posY;
    private float tempFOV = 75f;

    private Quaternion startCameraRotation;
    private Quaternion playerRotation;
    private Quaternion xaxis;
    private Quaternion yaxis;

    private bool inventoryActive = false;
    private bool pauseActive = false;

    private RaycastHit hit;

    private void Awake()
    {
        cursorMode = mode.LOCKED_HIDDEN;
    }

    private void Start()
    {
        tempFOV = GetComponent<Camera>().fieldOfView;

        playerRotation = PlayerController.currentTransform.rotation;
        startCameraRotation = transform.localRotation;

        DefineSettings();
    }

    private void DefineSettings()
    {
        PlayerController.enableController = true;
        QualitySettings.vSyncCount = VSYNCMode;
        Application.targetFrameRate = maxFPS;

        switch (cursorMode)
        {
            case mode.HIDDEN:
                {
                    Cursor.visible = false;
                    break;
                }

            case mode.LOCKED:
                {
                    Cursor.lockState = CursorLockMode.Locked;
                    break;
                }

            case mode.LOCKED_HIDDEN:
                {
                    Cursor.visible = false;
                    Cursor.lockState = CursorLockMode.Locked;
                    break;
                }
        }
    }

    public static void PauseOrInventory()
    {
        PlayerController.enableController = false;
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }

    private void Update()
    {
        // Сделать по человечески
        GetComponent<Camera>().fieldOfView = (PlayerController.currentState == PlayerController.CurrentState.WALKING || 
            PlayerController.currentState == PlayerController.CurrentState.CROUCHING) 
            ? Mathf.Lerp(GetComponent<Camera>().fieldOfView, tempFOV, 0.1f) : Mathf.Lerp(GetComponent<Camera>().fieldOfView, tempFOV + 5f, 0.1f);

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (inventoryActive)
            {
                inventoryActive = false;
                Manage_Objects.HideDocument_Static();

                DefineSettings();
                SetActive(inventoryActive, inventory);
            } else
            {
                if (pauseActive)
                {
                    DoUnpause();
                }
                else
                {
                    DoPause();
                }
            }
        }

        if (Input.GetKeyDown(KeyCode.I) && !pauseActive)
        {
            inventoryActive = !inventoryActive;

            if (inventoryActive) PauseOrInventory();
            else DefineSettings();

            SetActive(inventoryActive, inventory);
            Manage_Objects.HideDocument_Static();
        }

        if (!inventoryActive && !pauseActive)
        {
            if (Physics.Raycast(transform.position, transform.forward * 10, out hit))
            {
                if (Input.GetKeyDown(KeyCode.F))
                {
                    if (hit.transform.GetComponent<ObjectData>())
                    {
                        Destroy(hit.transform.gameObject);
                        for (int i = 0; i < hit.transform.GetComponent<ObjectData>().amount; i++)
                            Inventory.AddItem(hit.transform.GetComponent<ObjectData>().id);
                    } else
                    {
                        // INSPECT OBJECT
                    }
                }
            }
        }
    }

    public void EndGame()
    {
        PauseOrInventory();
        pauseActive = true;
        SetActive(false, pause);
        Time.timeScale = 0f;

        StartCoroutine(EndSequence());

        Manage_Objects.ChooseWeapon(Manage_Objects.currentWeapon.FLASHLIGHT);
        enabled = false;
    }

    private IEnumerator EndSequence()
    {
        SetActive(true, endScreen);
        SetActive(true, noisePicture);

        yield return new WaitForEndOfFrame();

        EndScreenCalculations.Calculate();

        SoundController.PlaySound(noiseSound, 1f);

        yield return new WaitForSeconds(noiseDuration);
        SetActive(false, noisePicture);
    }

    public void DoPause()
    {
        PauseOrInventory();
        pauseActive = true;

        SetActive(true, pause);
        Time.timeScale = 0f;
    }

    public void DoUnpause()
    {
        DefineSettings();
        pauseActive = false;

        SetActive(false, pause);
        Time.timeScale = 1f;
    }

    private void FixedUpdate()
    {
        if (!inventoryActive && !pauseActive)
        {
            posX += Input.GetAxis("Mouse X") * sensitivity;
            posY += Input.GetAxis("Mouse Y") * sensitivity;

            posY = Mathf.Clamp(posY, minY, maxY);

            xaxis = Quaternion.AngleAxis(posX, Vector3.up);
            yaxis = Quaternion.AngleAxis(posY, Vector3.left);

            transform.localRotation = startCameraRotation * yaxis;
            PlayerController.currentTransform.localRotation = playerRotation * xaxis;
        }
    }

    private void SetActive(bool state, GameObject[] objects)
    {
        foreach(GameObject go in objects)
            go.SetActive(state);
    }

    private void SetActive(bool state, GameObject go)
    {
        go.SetActive(state);
    }
}