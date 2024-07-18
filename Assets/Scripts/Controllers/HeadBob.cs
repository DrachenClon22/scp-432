using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeadBob : MonoBehaviour {

    public bool doHeadbob = true;

    public static float noiseLevel { get; private set; }

    [System.Serializable]
    public class SoundSettings
    {
        public bool playSound = false;
        [Space(5)]
        public AudioClip[] defaultSounds;
        public AudioClip[] concreteSounds;
        public AudioClip[] woodSounds;
        public AudioClip[] dirtSounds;
        [Space(5)]
        public float lookOutAngle = 15f;
        public float lookOutSpeed = 3f;

        public AudioSource source;
    }
    [Space]
    public SoundSettings soundSettings = new SoundSettings();
    [Space]
    public float height = 0.1f;
    public float multiplier = 1f;

    private Vector3 startPosition;
    private Vector3 offsetPosition;
    private Vector3 lastPosition;
    private Vector3 velocity;

    public Transform lookOut;

    private Rigidbody body;
    private PlayerController player;
    private Quaternion tempAngle;

    Quaternion cornerRotationRight;
    Quaternion cornerRotationLeft;

    private bool isCameraInDefaultPosition = true;

    private float timer;

    private void Start()
    {
        cornerRotationRight = Quaternion.Euler(lookOut.localRotation.x, lookOut.localRotation.y, tempAngle.eulerAngles.z - soundSettings.lookOutAngle);
        cornerRotationLeft = Quaternion.Euler(lookOut.localRotation.x, lookOut.localRotation.y, tempAngle.eulerAngles.z + soundSettings.lookOutAngle);

        player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();
        if (!soundSettings.source && soundSettings.playSound)
            soundSettings.playSound = false;

        if (PlayerController.controllerType == PlayerController.ControllerType.RBODY)
        {
            if (player.gameObject.GetComponent<Rigidbody>()) body = player.gameObject.GetComponent<Rigidbody>();
            else ErrorLogger.Log("No Rigidbody attached to player object", this);
        }
        lastPosition = transform.parent.position;
        startPosition = transform.localPosition;

        if (lookOut != null)
            tempAngle = lookOut.localRotation;
        isCameraInDefaultPosition = true;

        StartCoroutine(NoiseManager());
    }

    private void MakeLookOut()
    {
        if (lookOut != null)
        {
            if (isCameraInDefaultPosition)
                lookOut.localRotation = Quaternion.Lerp(lookOut.localRotation, tempAngle, soundSettings.lookOutSpeed * 2 * Time.deltaTime);

            if (isCameraInDefaultPosition != (Input.GetAxis("Corner") == 0)) 
                isCameraInDefaultPosition = (Input.GetAxis("Corner") == 0);

            if (Input.GetAxis("Corner") > 0)
                lookOut.localRotation = Quaternion.Lerp(lookOut.localRotation, cornerRotationRight, soundSettings.lookOutSpeed * Time.deltaTime);
            else if (Input.GetAxis("Corner") < 0)
                lookOut.localRotation = Quaternion.Lerp(lookOut.localRotation, cornerRotationLeft, soundSettings.lookOutSpeed * Time.deltaTime);
        }
    }

    IEnumerator NoiseManager()
    {
        while (true)
        {
            if (soundSettings.source.isPlaying)
            {
                noiseLevel = soundSettings.source.volume * 10;
                yield return new WaitWhile(() => soundSettings.source.isPlaying);
            } else
            {
                noiseLevel = 0.05f;
                yield return new WaitWhile(() => !soundSettings.source.isPlaying);
            }
        }
    }

    private void Update()
    {
        if (soundSettings.playSound && Mathf.Sin(2f * (timer % 360)) <= -0.95f)
        {
            switch (PlayerController.groundTag)
            {
                case "Concrete":
                    {
                        soundSettings.source.clip = soundSettings.concreteSounds[Random.Range(0, soundSettings.concreteSounds.Length)];
                        break;
                    }
                case "Wood":
                    {
                        soundSettings.source.clip = soundSettings.woodSounds[Random.Range(0, soundSettings.woodSounds.Length)];
                        break;
                    }
                case "Dirt":
                    {
                        soundSettings.source.clip = soundSettings.dirtSounds[Random.Range(0, soundSettings.dirtSounds.Length)];
                        break;
                    }
                default:
                    {
                        soundSettings.source.clip = soundSettings.defaultSounds[Random.Range(0, soundSettings.defaultSounds.Length)];
                        break;
                    }
            }

            soundSettings.source.volume = 0.08f / (player.walkSpeed / PlayerController.currentSpeed);
            soundSettings.source.Play();
        }
    }

    private void FixedUpdate()
    {
        MakeLookOut();

        if (velocity.magnitude > 0)
            timer += Time.fixedDeltaTime * velocity.magnitude * multiplier * (PlayerController.currentSpeed / 60);
        else if (velocity.magnitude == 0 && timer != 0)
            timer = 0f;
        
        offsetPosition = transform.localPosition;
        offsetPosition.y = startPosition.y + Mathf.Sin(2f * (timer % 360)) * height * (velocity.magnitude / 5f);
        offsetPosition.x = startPosition.x + Mathf.Sin(timer % 360) * height * (velocity.magnitude / 5f);

        transform.localPosition = Vector3.Slerp(transform.localPosition, offsetPosition, 0.1f);

        if (PlayerController.controllerType == PlayerController.ControllerType.NONRBODY)
        {
            Vector3 currFrameVelocity = (transform.parent.position - lastPosition) / Time.fixedDeltaTime;
            velocity = Vector3.Lerp(velocity, currFrameVelocity, 1f);
            lastPosition = transform.parent.position;
        } else
            velocity = body.velocity;
    }
}
