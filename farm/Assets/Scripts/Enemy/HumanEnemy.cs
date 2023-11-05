using Pathfinding;
using System.Collections;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Animations.Rigging;

[RequireComponent(typeof(CharacterController))]
public class HumanEnemy : Enemy
{
    [Header("General")]
    [SerializeField] Transform[] randomTargets;
    bool isFighting;
    Coroutine shootingCoroutine;
    float meleeDamageCooldown;
    Vector3 velocity;

    [Header("References")]
    [SerializeField] Transform distanceChecker;
    [SerializeField] GameObject weaponModel;
    [SerializeField] Transform shootPoint;
    DamageIndicator damageIndicator;

    [Header("Weapon - data")]
    [SerializeField] WeaponTypes weaponType;
    [SerializeField, Range(1, 10)] int[] damages;
    [SerializeField] GameObject[] weaponsModels;
    [SerializeField] float[] fireRates;
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
        AssignComponents();
        player = GameObject.FindGameObjectWithTag("Player").transform;
        StartCoroutine(CheckDistance());
        distanceChecker.parent = null;
        damageIndicator = FindObjectOfType<DamageIndicator>();

        #region Setting pathfinding target

Transform randTargetsParent = GameObject.Find("RandomEnemysTargets").transform;
        for (int i = 0; i < randTargetsParent.childCount; i++)  // setting up randomTargets[]
            randomTargets[i] = randTargetsParent.GetChild(i);

        GetComponent<AIDestinationSetter>().target = randomTargets[Random.Range(0, randomTargets.Length)];

        #endregion

        #region Setting weapon in hands

        switch (weaponType)
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
        if (!pathfinding.enabled && !isDeath)
        {
            transform.LookAt(player);
            // don't rotate in x axis because it looks weird
            transform.eulerAngles = new Vector3(0, transform.eulerAngles.y);
        }

        if(meleeDamageCooldown >= 0)
            meleeDamageCooldown -= Time.deltaTime;
    }

    public override void GetDamage(int value)
    {
        enabled = true;  // idk why but it turns off when enemy gets damage
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
                GetComponent<CharacterController>().enabled = false;
                int randomNum = Random.Range(1, 6);
                if(randomNum <= 3)
                {
                    GameObject weaponObject = poolManager.GetObjectFromPool(2 + weaponIndex);
                    weaponObject.transform.position = skeletonObject.transform.position;  // drop weapon after death
                }
                else
                {
                    GameObject heartObject = poolManager.GetObjectFromPool(5);
                    heartObject.transform.position = skeletonObject.transform.position;  // drop heart
                }

                transform.GetChild(7).gameObject.SetActive(false);
                SetRagdollActive(false);
                animator.enabled = false;
                pathfinding.enabled = false;
                SetRagdollActive(true);
                StopAllCoroutines();

                // turn off weapon models
                foreach(GameObject weaponModel in weaponsModels)
                    weaponModel.SetActive(false);
            }
            isDeath = true;
        }

        #endregion
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
            //if(IsPlayerBehindWall())
              //  StartCoroutine(ChangeBehaviourIfSeePlayer(false));

            yield return new WaitForSeconds(Random.Range(0.2f, 0.7f));
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

                    #region Giving damage to player

                    try
                    {
                        if (hit.transform.CompareTag("Player"))
                        {
                            int randomNumber = Random.Range(1, 6);
                            int actualDamage = damages[weaponIndex];
                            damageIndicator.SetTarget(transform);

                            // damage to player isn't based on ray's hit
                            // it works on random
                            Player playerScript = player.GetComponent<Player>();
                            if (randomNumber == 1)
                                playerScript.GiveDamageToPlayer(actualDamage * GameManager.HEAD_DMG_MULTIPLAYER / 3);
                            else if (randomNumber > 1 && randomNumber <= 3)
                                playerScript.GiveDamageToPlayer(actualDamage * GameManager.BODY_DMG_MULTIPLAYER / 3);
                            else if (randomNumber > 3 && randomNumber <= 5)
                                playerScript.GiveDamageToPlayer(actualDamage * GameManager.LIMBS_DMG_MULTIPLAYER / 3);
                        }
                    }
                    catch { }

                    #endregion
                }
                source.PlayOneShot(weaponsSounds[weaponIndex]);
                yield return new WaitForSeconds(fireRates[weaponIndex]);
            }
            yield return new WaitForSeconds(Random.Range(1, 3));
        }
    }

    /// <summary> Change enemy's behaviour according to enemy's possibility to see player. </summary>
    /// <param name="canSeePlayer"></param>
    public IEnumerator ChangeBehaviourIfSeePlayer(bool canSeePlayer)
    {
        if (isDeath)
            yield break;

        yield return new WaitForSeconds(0.2f);  // to avoid glitch
        if (canSeePlayer && !IsPlayerBehindWall())
        {
            if (Vector3.Distance(transform.position, player.position) < 10)
                pathfinding.enabled = false;
            else
                pathfinding.enabled = true;

            if (shootingCoroutine == null)
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

    /// <summary> Return true if the player in hidden behind wall and enemy can't see him. </summary>
    bool IsPlayerBehindWall()
    {
        RaycastHit hit;
        Physics.Raycast(shootPoint.position, (player.position - shootPoint.position), out hit);

        if (hit.transform.CompareTag("Player"))
            return false;
        else
            return true;
    }
}