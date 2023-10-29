using Pathfinding;
using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(AIPath))]
[RequireComponent(typeof(Seeker))]
[RequireComponent(typeof(AIDestinationSetter))]
[RequireComponent(typeof(AudioSource))]
public class Enemy : MonoBehaviour
{
    public int health = 100;
    protected bool isDeath;
    protected DamagePointer[] ragdollElements = new DamagePointer[11];
    [SerializeField] protected AudioClip damageSound;
    [SerializeField] protected AudioClip deathSound;
    [HideInInspector] public bool isChecked;

    protected AudioSource source;
    protected Animator animator;
    protected PoolManager poolManager;
    protected AIPath pathfinding;
    [SerializeField] protected GameObject skeletonObject;
    protected float timeToPlayPainSound;
    protected GameManager gameManager;
    protected Transform player;

    /// <summary> Assign all necessary variables. To place in Awake(). </summary>
    protected void AssignComponents()
    {
        source = GetComponent<AudioSource>();
        animator = GetComponent<Animator>();
        try { pathfinding = GetComponent<AIPath>(); } catch { }
        poolManager = FindObjectOfType<PoolManager>();
        //poolManager = FindObjectOfType<PoolManager>();
        //pathfinding = GetComponent<AIPath>();
        ragdollElements = GetComponentsInChildren<DamagePointer>();
        gameManager = FindObjectOfType<GameManager>();
        player = FindObjectOfType<Player>().transform;
        StartCoroutine(CountTimeToPainSounds());
    }

    /// <summary> Give damage to enemy. </summary>
    /// <param name="value"></param>
    public virtual void GetDamage(int value)
    {
        health -= value;

        #region Playing pain sounds

        if (timeToPlayPainSound < 0)
        {
            if (health > 0)
                source.PlayOneShot(damageSound);
            else if (!isDeath)
                source.PlayOneShot(deathSound);
            timeToPlayPainSound = 0.1f;
        }

        #endregion

        #region Blood stains

        GameObject bloodObject = poolManager.GetObjectFromPool(1);
        bloodObject.transform.position = new Vector3(skeletonObject.transform.position.x, 0.001f, skeletonObject.transform.position.z);
        bloodObject.transform.eulerAngles = new Vector3(90, 0, Random.Range(0, 360));

        #endregion

        if (health <= 0)
        {
            SetRagdollActive(true);
            animator.enabled = false;
            pathfinding.enabled = false;
            isDeath = true;
            // animals have collider to detect contact with a tractor, turn it off to don't disturb ragdoll
            if (GetComponent<Collider>()) { GetComponent<Collider>().enabled = false; }
            StopAllCoroutines();
        }
    }

    /// <summary> Push ragdoll. </summary>
    /// <param name="force"></param>
    public void AddForceToRagdoll(Vector3 force)
    {
        foreach (DamagePointer dp in ragdollElements)
        {
            dp.GetComponent<Rigidbody>().AddForce(force);
        }
    }

    /// <summary> Make ragdoll kinematic or vice versa. </summary>
    protected void SetRagdollActive(bool value)
    {
        foreach (DamagePointer dp in ragdollElements)
        {
            dp.GetComponent<Rigidbody>().isKinematic = !value;
        }
    }

    IEnumerator CountTimeToPainSounds()
    {
        while(true)
        {
            timeToPlayPainSound -= Time.deltaTime;
            yield return null;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        // enemy dies after touch a scythe
        if(other.CompareTag("Scythe"))
            GetDamage(100);
    }
}
