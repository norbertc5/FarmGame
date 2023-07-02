using Pathfinding;
using System.Collections;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class Enemy : MonoBehaviour
{
    [Header("General")]
    public int health = 100;
    bool isDeath;
    bool isFighting;
    DamagePointer[] ragdollElements = new DamagePointer[11];

    [Header("References")]
    [SerializeField] Transform distanceChecker;
    [SerializeField] GameObject skeletonObject;
    [SerializeField] GameObject weaponModel;
    [SerializeField] Transform shootPoint;
    Animator animator;
    PoolManager poolManager;
    Transform player;
    Coroutine shootingCoroutine;
    AIPath pathfinding;

    [Header("Sounds")]
    [SerializeField] AudioClip damageSound;
    [SerializeField] AudioClip deathSound;
    AudioSource source;

    void Awake()
    {
        ragdollElements = GetComponentsInChildren<DamagePointer>();
        animator = GetComponent<Animator>();
        poolManager = FindObjectOfType<PoolManager>();
        pathfinding = GetComponent<AIPath>();
        player = GetComponent<AIDestinationSetter>().target;
        source = GetComponent<AudioSource>();
        StartCoroutine(CheckDistance());
    }

    private void Update()
    {
        // make enemy looks at player while shooting
        if (isFighting && !isDeath)
        {
            transform.LookAt(player);
            // don't rotate in x axis because it looks weird
            transform.eulerAngles = new Vector3(0, transform.eulerAngles.y);
        }
    }

    /// <summary> Give damage to enemy. </summary>
    /// <param name="value"></param>
    public void GetDamage(int value)
    {
        health -= value;

        if(health > 0)
            source.PlayOneShot(damageSound);        

        #region Blood stains

        GameObject bloodObject = poolManager.GetObjectFromPool(1);
        bloodObject.transform.position = new Vector3(skeletonObject.transform.position.x, 0.001f, skeletonObject.transform.position.z);
        bloodObject.transform.eulerAngles = new Vector3(90, 0, Random.Range(0, 360));

        #endregion

        if (health <= 0)
        {
            if(!isDeath)
            {
                GameObject pistolObject = poolManager.GetObjectFromPool(2);
                pistolObject.transform.position = skeletonObject.transform.position;
                weaponModel.SetActive(false);
                SetRagdollActive(false);
                animator.enabled = false;
                pathfinding.enabled = false;
                GetComponent<CharacterController>().enabled = false;
                SetRagdollActive(true);
                source.PlayOneShot(deathSound);

                if (shootingCoroutine != null)
                    StopCoroutine(shootingCoroutine);
            }
            isDeath = true;
        }
    }

    /// <summary>
    /// Make ragdoll kinematic or vice versa.
    /// </summary>
    void SetRagdollActive(bool value)
    {
        foreach (DamagePointer dp in ragdollElements)
        {
            dp.GetComponent<Rigidbody>().isKinematic = !value;
        }
    }

    /// <summary>
    /// Push ragdoll.
    /// </summary>
    /// <param name="force"></param>
    public void AddForceToRagdoll(Vector3 force)
    {
        foreach (DamagePointer dp in ragdollElements)
        {
            dp.GetComponent<Rigidbody>().AddForce(force, ForceMode.Impulse);
        }
    }

    /// <summary>
    /// Check if player is in the same place or moving and playing suitable animation.
    /// </summary>
    IEnumerator CheckDistance()
    {
        while(true)
        {
            // leave distanceChecker on enemy's position
            // play suitable animation according to distance between enemy and distanceChecker
            yield return new WaitForSeconds(0.1f);

            if(Vector3.Distance(transform.position, distanceChecker.position) <= 0.05f)
                animator.CrossFade("Idle", 0.1f);
            else
                animator.CrossFade("Walking", 0.1f);

            yield return new WaitForSeconds(0.05f);  // to avoid errors associated with changing animations
            distanceChecker.position = transform.position;
        }
    }

    /// <summary>
    /// Wait some time and shoot ray in player's direction.
    /// </summary>
    IEnumerator Shooting()
    {
        while(true)
        {
            yield return new WaitForSeconds(1);
            RaycastHit hit;
            float recoil = Random.Range(-1.3f, 1.3f);
            Physics.Raycast(shootPoint.position, (player.position + Vector3.up * recoil + Vector3.right * recoil - shootPoint.position), out hit);

            if(hit.transform.CompareTag("Player"))
            {
                Debug.Log("trafiony");
            }
        }
    }

    /// <summary> Change enemy's behaviour according to enemy's possibility to see player. </summary>
    /// <param name="canSeePlayer"></param>
    public IEnumerator ChangeBehaviourIfSeePlayer(bool canSeePlayer)
    {
        if (isDeath)
            yield break;

        yield return new WaitForSeconds(0.2f);  // to avoid glitch

        // raycast is needed to detect if player isn't behind a wall
        RaycastHit hit;
        Physics.Raycast(shootPoint.position, (player.position - shootPoint.position), out hit);

        if (canSeePlayer && hit.transform.CompareTag("Player"))
        {
            yield return new WaitForSeconds(1);
            pathfinding.enabled = false;
            shootingCoroutine = StartCoroutine(Shooting());
        }
        else
        {
            pathfinding.enabled = true;
            StopCoroutine(shootingCoroutine);
        }
        isFighting = canSeePlayer;
    }
}