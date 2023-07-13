using Pathfinding;
using System.Collections;
using UnityEngine;
using UnityEngine.Animations.Rigging;
using static UnityEngine.GraphicsBuffer;

[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(AIPath))]
[RequireComponent(typeof(Seeker))]
[RequireComponent(typeof(AIDestinationSetter))]
[RequireComponent(typeof(CharacterController))]
[RequireComponent(typeof(AudioSource))]
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
    float timeToPlayPainSound;

    [Header("Weapon - data")]
    [SerializeField] WeaponTypes weaponType;
    [SerializeField] GameObject[] weaponsModels;
    [SerializeField] float[] delaysForShooting;
    [SerializeField] AudioClip[] weaponsSounds;
    [SerializeField] int[] raysAmount;
    enum WeaponTypes { Pisol, Rifle, Shotgun }
    int weaponIndex = 0;

    [Header("Weapon - rigging")]
    [SerializeField] RigBuilder rigBuilder;
    [SerializeField] TwoBoneIKConstraint rightArmIK;
    [SerializeField] TwoBoneIKConstraint leftArmIK;
    [Space(2)]
    [SerializeField] Transform[] rightArmIKTargets;
    [SerializeField] Transform[] rightArmIKHints;
    [Space(2)]
    [SerializeField] Transform[] leftArmIKTargets;
    [SerializeField] Transform[] leftArmIKHints;

    void Awake()
    {
        ragdollElements = GetComponentsInChildren<DamagePointer>();
        animator = GetComponent<Animator>();
        poolManager = FindObjectOfType<PoolManager>();
        pathfinding = GetComponent<AIPath>();
        GetComponent<AIDestinationSetter>().target = GameObject.FindGameObjectWithTag("Player").transform;
        player = GetComponent<AIDestinationSetter>().target;
        source = GetComponent<AudioSource>();
        StartCoroutine(CheckDistance());
        distanceChecker.parent = null;

        #region Setting weapon in hands

        switch(weaponType)
        {
            case WeaponTypes.Pisol: weaponIndex = 0; break;
            case WeaponTypes.Rifle: weaponIndex = 1; break;
            case WeaponTypes.Shotgun: weaponIndex = 2; break;
        }
        weaponsModels[weaponIndex].SetActive(true);
        rightArmIK.data.target = rightArmIKTargets[weaponIndex];
        rightArmIK.data.hint = rightArmIKHints[weaponIndex];
        leftArmIK.data.target = leftArmIKTargets[weaponIndex];
        leftArmIK.data.hint = leftArmIKHints[weaponIndex];
        rigBuilder.Build();

        #endregion
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

        if(timeToPlayPainSound >= 0)
            timeToPlayPainSound -= Time.deltaTime;
    }

    /// <summary> Give damage to enemy. </summary>
    /// <param name="value"></param>
    public void GetDamage(int value)
    {
        GetComponent<Enemy>().enabled = true;  // idk why but it turns off when enemy gets damage
        health -= value;

        #region Playing pain sounds

        if (timeToPlayPainSound < 0)
        {
            if (health > 0)
                source.PlayOneShot(damageSound);
            else if(!isDeath)
                source.PlayOneShot(deathSound);
            timeToPlayPainSound = 0.1f;
        }

        #endregion

        #region Blood stains

        GameObject bloodObject = poolManager.GetObjectFromPool(1);
        bloodObject.transform.position = new Vector3(skeletonObject.transform.position.x, 0.001f, skeletonObject.transform.position.z);
        bloodObject.transform.eulerAngles = new Vector3(90, 0, Random.Range(0, 360));

        #endregion

        #region Death

        if (health <= 0)
        {
            if(!isDeath)
            {
                GameObject weaponObject = poolManager.GetObjectFromPool(2 + weaponIndex);
                weaponObject.transform.position = skeletonObject.transform.position;  // drop weapon after death
                SetRagdollActive(false);
                animator.enabled = false;
                pathfinding.enabled = false;
                GetComponent<CharacterController>().enabled = false;
                SetRagdollActive(true);

                if (shootingCoroutine != null)
                    StopCoroutine(shootingCoroutine);

                // turn off weapon models
                foreach(GameObject weaponModel in weaponsModels)
                    weaponModel.SetActive(false);
            }
            isDeath = true;
        }

        #endregion
    }

    /// <summary> Make ragdoll kinematic or vice versa. </summary>
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
            yield return new WaitForSeconds(Random.Range(1, 3));
            int howManyShoots = Random.Range(1, 10);
            
            // it works similar to shooting by player, in Shooting.cs script it's better described
            for (int i = 0; i < howManyShoots; i++)
            {
                for (int j = 0; j <= raysAmount[weaponIndex]; j++)
                {
                    RaycastHit hit;
                    float recoil = Random.Range(-1.3f, 1.3f);
                    Physics.Raycast(shootPoint.position,
                        (player.position + Vector3.up * recoil + Vector3.right * recoil - shootPoint.position), out hit);

                    if (hit.transform.CompareTag("Player"))
                    {
                        Debug.Log("trafiony");
                    }
                }
                source.PlayOneShot(weaponsSounds[weaponIndex]);
                yield return new WaitForSeconds(delaysForShooting[weaponIndex]);
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
            pathfinding.enabled = false;

            if(shootingCoroutine == null)
                shootingCoroutine = StartCoroutine(Shooting());
        }
        else
        {
            pathfinding.enabled = true;

            try
            {
                StopCoroutine(shootingCoroutine);
                shootingCoroutine = null;
            }
            catch { }
        }
        isFighting = canSeePlayer;
    }
}