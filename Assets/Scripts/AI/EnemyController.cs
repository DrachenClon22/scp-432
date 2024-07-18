using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class EnemyController : MonoBehaviour {

    // SEARCH - just walking around checking territory, can ignore some noises
    // LOST - always after CHASE, actively looking for player and listeting to any noises, making more noises
    // ATTACK - always after CHASE, attack phase
    // CHASE - chasing the player but not trying to kill but for fun to scare the player
    // WARN - always after SEARCH, happens when hears some loud noises
    public enum state { SEARCH = 0, LOST, ATTACK, CHASE, WARN }
    public static state currentState { get; private set; } = state.SEARCH;
    public static bool isChasing { get; private set; } = false;

    public static float DistanceToPlayer = 0.0f;

    public float fov = 120f;

    public AudioSource roarSource;
    public AudioSource stepSource;

    public Animator animator;

    public AudioClip[] roarSounds;
    public AudioClip[] stepSounds;

#if UNITY_EDITOR
    public GameObject testPlayer;
    public GameObject testPathh;
#endif

    CameraController _camera;
    NavMeshAgent agent;

    private bool playRoar = false;

    private Vector3 lastSeenPlayersPosition = Vector3.zero;
    private Vector3 currentPath = Vector3.zero;

    private float chasingSpeed = 4f;
    private float walkingSpeed = 2.5f;
    private float tempRotationOffset = 0f;

    private void Start()
    {
        fov /= 2f;
        agent = GetComponent<NavMeshAgent>();
        EventManager.current.DoEnemyStateChange(currentState = state.SEARCH);
        agent.speed = walkingSpeed;
        DistanceToPlayer = 99999f;
        agent.SetDestination(currentPath = RandomNavmeshLocation(30f));

        if (GetComponent<Animator>())
            animator = GetComponent<Animator>();
        _camera = Camera.main.GetComponent<CameraController>();

        StartCoroutine(stepSound());
        StartCoroutine(randomSound());

        roarSource.clip = roarSounds[Random.Range(0, roarSounds.Length)];
        roarSource.Play();
    }

    private void LateUpdate()
    {
        if (isChasing != (currentState == state.CHASE))
        {
            isChasing = (currentState == state.CHASE);
        }
    }

    private void Update()
    {
#if UNITY_EDITOR
        if (testPathh && testPlayer)
        {
            testPathh.transform.position = currentPath;
            testPlayer.transform.position = lastSeenPlayersPosition;
        }
#endif

        if (animator)
            animator.SetFloat("speed", agent.velocity.magnitude);

        if (PlayerController.groundTag.Equals("Concrete"))
            agent.SetDestination(SpawnEnemy.lastSpawnedPosition);
        else
        {
            if (lastSeenPlayersPosition == Vector3.zero)
            {        
                if (agent.destination != currentPath)
                {
                    agent.SetDestination(currentPath);
                }

                if (calculateDistance(currentPath) < 0.5f)
                {
                    if (currentState == state.LOST)
                        EventManager.current.DoEnemyStateChange(currentState = state.SEARCH);
                    currentPath = RandomNavmeshLocation(60f);
                }
            } else
            {
                if (agent.destination != lastSeenPlayersPosition)
                {
                    agent.SetDestination(lastSeenPlayersPosition);
                }

                if (calculateDistance(lastSeenPlayersPosition) < 0.5f)
                {
                    lastSeenPlayersPosition = Vector3.zero;
                    if (currentState != state.LOST)
                        EventManager.current.DoEnemyStateChange(currentState = state.LOST);
                    agent.speed = walkingSpeed;
                    agent.SetDestination(currentPath = RandomNavmeshLocation(40f, PlayerController.currentTransform.position));
                }
            }
        }
        //agent.SetDestination(PlayerController.currentTransform.position);

        DistanceToPlayer = calculateDistance(PlayerController.currentTransform.position);

        if ((HeadBob.noiseLevel / DistanceToPlayer) > ((isChasing) ? 0.16f : 0.1f))
        {
            lastSeenPlayersPosition = PlayerController.currentTransform.position;
            if (currentState != state.CHASE)
            {
                EventManager.current.DoEnemyStateChange(currentState = state.CHASE);
                agent.speed = chasingSpeed;
            }
            //if (agent.speed < chasingSpeed)  agent.speed = chasingSpeed;
            agent.speed += (Time.deltaTime / 100);
        }

        canSeePlayer();
        // Это что и зачем
        if (DistanceToPlayer < 10f && !playRoar)
        {
            playRoar = true;
            //roarSource.clip = roarSounds[1];
            roarSource.Play();
        } else if (playRoar && DistanceToPlayer > 10f)
            playRoar = false;
    }

    private bool canSeePlayer()
    {
        if (Physics.Raycast(transform.position, PlayerController.currentTransform.position - transform.position, out RaycastHit hit, 20))
        {
            if (hit.collider.tag.Equals("Player"))
            {
                tempRotationOffset = Quaternion.FromToRotation(transform.position, hit.collider.transform.position - transform.position).eulerAngles.y + transform.rotation.eulerAngles.y;
                if (tempRotationOffset > (360 - fov) || tempRotationOffset < fov)
                {
#if UNITY_EDITOR
                    Debug.DrawRay(transform.position, PlayerController.currentTransform.position - transform.position, Color.green);
#endif
                    lastSeenPlayersPosition = hit.collider.transform.position;
                    if (currentState != state.CHASE)
                    {
                        EventManager.current.DoEnemyStateChange(currentState = state.CHASE);
                        agent.speed = chasingSpeed;
                    }
                    //if (agent.speed < chasingSpeed) agent.speed = chasingSpeed;
                    agent.speed += (Time.deltaTime / 100);
                } else
                {
#if UNITY_EDITOR
                    Debug.DrawRay(transform.position, PlayerController.currentTransform.position - transform.position, Color.yellow);
#endif
                }
            }
            else
            {
#if UNITY_EDITOR
                Debug.DrawRay(transform.position, PlayerController.currentTransform.position - transform.position, Color.red);
#endif
            }
        }

        return false;
    }
    private IEnumerator stepSound()
    {
        while (true)
        {
            yield return new WaitForSeconds(1 / agent.speed);

            stepSource.clip = stepSounds[Random.Range(0, stepSounds.Length)];
            stepSource.Play();
        }
    }

    private IEnumerator randomSound()
    {
        while (true)
        {
            yield return new WaitForSeconds(Random.Range(10f, 20f));

            if (!roarSource.isPlaying)
            {
                roarSource.clip = roarSounds[Random.Range(0, roarSounds.Length)];
                roarSource.Play();
            }
        }
    }
    // Это переделать концовка есть нормальная
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            _camera.EndGame();
            SpawnEnemy.doSpawn = false;
            Destroy(gameObject);
        }
    }

    public Vector3 RandomNavmeshLocation(float radius, Vector3 player = new Vector3())
    {
        Vector3 randomDirection = Random.insideUnitSphere * radius;
        randomDirection += (player != Vector3.zero) ? player : transform.position;
        NavMeshHit hit;
        Vector3 finalPosition = Vector3.zero;
        if (NavMesh.SamplePosition(randomDirection, out hit, radius, 1))
        {
            finalPosition = hit.position;
        }
        return finalPosition;
    }

    private float calculateDistance(Vector3 target)
    {
        return Mathf.Sqrt((new Vector3(transform.position.x, 0, transform.position.z) - new Vector3(target.x, 0, target.z)).sqrMagnitude);
    }
}
