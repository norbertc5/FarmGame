using System;
using System.Collections;
using TMPro;
using Unity.Burst.CompilerServices;
using UnityEngine;
using UnityEngine.UI;

public class Shooting : MonoBehaviour
{
    [Header("General")]
    public static float recoil;
    Weapon actualWeapon;
    bool canShoot = true;
    bool autoFireDelay = true;
    Coroutine reloadCoroutine;
    bool isReloading;
    bool[] unlockedWeapons;
    float fireRateTime;
    public static bool canPlayerChangeWeapon = true;

    [Header("References")]
    [SerializeField] Animator armsAnim;
    [SerializeField] SpriteRenderer flashSprite;
    [SerializeField] TextMeshProUGUI ammoText;
    [SerializeField] Weapon[] weapons;
    [SerializeField] Animation weaponChangeAnim;
    [SerializeField] GameObject weaponWheel;
    [SerializeField] TextMeshProUGUI weaponNameText;
    [SerializeField] Button[] buttonsInWheel;
    Crosshair crosshair;
    PoolManager poolManager;
    GameManager gameManager;

    [Header("Sounds")]
    [SerializeField] AudioClip emptyGunSound;
    [SerializeField] AudioClip pickUpSound;
    [SerializeField] AudioClip meleeHitSound;
    AudioSource source;

    private void Start()
    {
        actualWeapon = weapons[0];
        UpdateAmmoText();
        weaponNameText.text = actualWeapon.weaponName;
        crosshair = FindObjectOfType<Crosshair>();
        poolManager = FindObjectOfType<PoolManager>();
        gameManager = FindObjectOfType<GameManager>();
        source = GetComponent<AudioSource>();
        unlockedWeapons = new bool[weapons.Length];
        unlockedWeapons[0] = true;
        gameManager.OnInteractionWithVehicle += HideWeaponWheel;
    }

    void Update()
    {
        // scale of weapons is independent of Player scale
        transform.localScale = new Vector3(1 / transform.parent.localScale.x,
        1 / transform.parent.localScale.y, 1 / transform.parent.localScale.z);

        if (fireRateTime >= 0)
            fireRateTime -= Time.deltaTime;

        #region Reload

        if (Input.GetKeyDown(KeyCode.R) && actualWeapon.ammoInMagazine < actualWeapon.magazineCapacity)
        {
            reloadCoroutine = StartCoroutine(Reload());
        }

        #endregion

        #region Weapon Change

        if(canPlayerChangeWeapon)
        {
            // changing with keyboard
            if (Input.GetKeyDown(KeyCode.Alpha1))
                ChangeWeapon(0);
            else if (Input.GetKeyDown(KeyCode.Alpha2))
                ChangeWeapon(1);
            else if (Input.GetKeyDown(KeyCode.Alpha3))
                ChangeWeapon(2);
            else if (Input.GetKeyDown(KeyCode.Alpha4))
                ChangeWeapon(3);
            else if (Input.GetKeyDown(KeyCode.Alpha5))
                ChangeWeapon(4);
            else if (Input.GetKeyDown(KeyCode.Alpha6))
                ChangeWeapon(5);

            // changing with weapon wheel
            if (Input.GetKeyDown(KeyCode.Tab))
            {
                weaponWheel.SetActive(true);
                PlayerMovement.ChangeMovementPossibility(false);
                canShoot = false;
                Cursor.lockState = CursorLockMode.None;
                crosshair.gameObject.SetActive(false);
            }
            else if (Input.GetKeyUp(KeyCode.Tab))
            {
                HideWeaponWheel();
                PlayerMovement.ChangeMovementPossibility(true);
                Cursor.lockState = CursorLockMode.Locked;
                crosshair.gameObject.SetActive(true);

                if (!isReloading)
                    canShoot = true;
            }
        }

        #endregion

        // it must be on end of Update
        #region Raycasting

        if (!actualWeapon.isUseable)
            return;

        if (fireRateTime <= 0 && ((!actualWeapon.isAuto && Input.GetMouseButtonDown(0)) ||
            (actualWeapon.isAuto && Input.GetMouseButton(0) && autoFireDelay)))
        {
            // playing emptyGunSound
            if (actualWeapon.ammoAmount <= 0 && actualWeapon.ammoInMagazine <= 0 && Input.GetMouseButtonDown(0) && !weaponWheel.activeSelf && !actualWeapon.isMelee)
                GameManager.playerSource.PlayOneShot(emptyGunSound);

            if (canShoot && actualWeapon.ammoInMagazine > 0)
            {
                // create suitable amount of rays
                for (int i = 0; i < actualWeapon.raysAmount; i++)
                {
                    StartCoroutine(Shoot());
                }

                // melee weapons work as same as normal ones but throw ray on small distance
                if (!actualWeapon.isMelee)
                {
                    armsAnim.CrossFade("recoilAnim", 0);
                    StartCoroutine(ShowFlash());
                    actualWeapon.ammoInMagazine--;
                    UpdateAmmoText();
                    CheckIfMagazineEmpty();
                    gameManager.OnLoudShoot?.Invoke();
                }
                else
                {
                    armsAnim.CrossFade("batAttack", 0);
                }

                Crosshair.spread += actualWeapon.crosshairSpreadWhenShoot;
                source.PlayOneShot(actualWeapon.shootSound);
                fireRateTime = actualWeapon.fireRate;

                if (!PlayerMovement.IsCrouching)
                    recoil += actualWeapon.recoil;

                if (actualWeapon.isAuto)
                    StartCoroutine(AutoFire());

                if (actualWeapon == weapons[3] && actualWeapon.ammoInMagazine > 0)
                    armsAnim.CrossFade("ShotgunPump", 0f);
            }
        }

        #endregion
    }

    /// <summary> Throw ray. Next make bullet hole and check if hit enemy. </summary>
    IEnumerator Shoot()
    {
        yield return new WaitForSeconds(actualWeapon.additionalDelayForRay);  // delay between press button and shoot

        Ray ray;
        RaycastHit hit;
        // make dispersion according to crosshair spread
        Vector2 randomTarget = UnityEngine.Random.insideUnitCircle * Crosshair.spread / 5;
        randomTarget *= actualWeapon.additionalDispersionSize;  // additional dispersion for specific weapons
        ray = Camera.main.ScreenPointToRay(Input.mousePosition + new Vector3(randomTarget.x, randomTarget.y));
        Physics.Raycast(ray, out hit, actualWeapon.range);


        // doesn't throw error when no hit (e.g. when shooting to sky)
        try
        {
            #region Bullet holes

            if (!actualWeapon.isMelee)
            {
                // holes don't appear on enemies
                if (!hit.transform.CompareTag("Enemy"))
                {
                    GameObject bulletHole = poolManager.GetObjectFromPool(0);
                    bulletHole.transform.position = hit.point;
                    bulletHole.transform.rotation = Quaternion.FromToRotation(Vector3.up, hit.normal);
                }
            }

            #endregion

            #region Giving damage to enemy

            if (hit.collider.transform.CompareTag("Enemy"))
            {
                Enemy enemy = hit.collider.GetComponentInParent<Enemy>();
                enemy.enabled = false;
                switch (hit.collider.gameObject.GetComponent<DamagePointer>().bodyPart)
                {
                    case DamagePointer.BodyParts.Limb: enemy.GetDamage(2 * actualWeapon.damage); break;
                    case DamagePointer.BodyParts.Body: enemy.GetDamage(5 * actualWeapon.damage); break;
                    case DamagePointer.BodyParts.Head: enemy.GetDamage(10 * actualWeapon.damage); break;
                }

                // pushing ragdoll according to ray direction
                if (enemy.health <= 0)
                {
                    Vector3 force = ray.direction;
                    force.Normalize();
                    enemy.AddForceToRagdoll(force * 10 * actualWeapon.additionalForceToRagdoll);
                }

                /*if(enemy.GetComponent<Cow>())
                {
                    enemy.AddForceToRagdoll(new Vector3(0.5f, -1f) * 100 * actualWeapon.additionalForceToRagdoll);
                }*/

            }

            #endregion

            #region Melee hit sound

            if (actualWeapon.isMelee && hit.collider.CompareTag("Enemy"))
                source.PlayOneShot(meleeHitSound);

            #endregion
        }
        catch { }
    }

    /// <summary> Show flash on end of the weapon's barrel for short time. </summary>
    IEnumerator ShowFlash()
    {
        flashSprite.gameObject.SetActive(true);
        yield return new WaitForSeconds(0.1f);
        flashSprite.gameObject.SetActive(false);
    }

    void UpdateAmmoText()
    {
        if (actualWeapon.isMelee)
            return;

        ammoText.text = "Ammo: " + actualWeapon.ammoInMagazine + "/" + actualWeapon.ammoAmount;
    }

    /// <summary> Reload actual weapon. </summary>
    IEnumerator Reload()
    {
        // break if no ammo
        if(actualWeapon.ammoAmount <= 0)
            yield break;

        // when ammoInMagazine is equals to 0 and Player changed weapon, it waits until weaponChange anim ends
        if (armsAnim.GetCurrentAnimatorClipInfo(0).Length > 0 && armsAnim.GetCurrentAnimatorClipInfo(0)[0].clip.name == "weaponChange")
            yield return new WaitForSeconds(0.5f);

        armsAnim.CrossFade(actualWeapon.weaponName + "Reloading", 0);
        // play animation for second hand while reloading
        if (actualWeapon.additinonalReloadingAnim != null)
            actualWeapon.additinonalReloadingAnim.CrossFade("Reloading", 0);

        canShoot = false;
        isReloading = true;
        yield return new WaitForSeconds(0.1f);  // to avoid glitches
        yield return new WaitForSeconds(armsAnim.GetCurrentAnimatorClipInfo(0)[0].clip.length - 0.5f);  // wait until anim ends

        // can't shoot when wheel is opened
        if(!weaponWheel.activeSelf)
            canShoot = true;

        // count how many ammo add to magazine
        int ammoToAdd = 0;
        for (int i = actualWeapon.ammoInMagazine; i < actualWeapon.magazineCapacity; i++)
        {
            if (actualWeapon.ammoAmount > 0)
            {
                ammoToAdd++;
                actualWeapon.ammoAmount--;
            }
        }

        actualWeapon.ammoInMagazine += ammoToAdd;
        isReloading = false;
        UpdateAmmoText();
    }

    /// <summary> Change weapon when it isn't visible in reloading animation.</summary>
    IEnumerator WeaponModelChange()
    {
        yield return new WaitForSeconds(0.5f);
        foreach (Weapon weaponObj in weapons)
        {
            weaponObj.gameObject.SetActive(false);
        }
        actualWeapon.gameObject.SetActive(true);
        CheckIfMagazineEmpty();
        UpdateAmmoText();
    }

    /// <summary> Make delays in auto fire, fire rate.</summary>
    IEnumerator AutoFire()
    {
        autoFireDelay = false;
        yield return new WaitForSeconds(actualWeapon.fireRate);
        autoFireDelay = true;
        yield return new WaitForSeconds(actualWeapon.fireRate);
    }

    /// <summary> Start reloading when no ammo in magazine.</summary>
    void CheckIfMagazineEmpty()
    {
        if (actualWeapon.ammoInMagazine <= 0)
            reloadCoroutine = StartCoroutine(Reload());
    }

    /// <summary> Change weapon which is Player using now.</summary>
    /// <param name="newWeapon"></param>
    public void ChangeWeapon(int newWeapon)
    {
        if (weapons[newWeapon] == actualWeapon || unlockedWeapons[newWeapon] == false)
            return;

        armsAnim.CrossFade("weaponChange", 0);
        actualWeapon = weapons[newWeapon];
        StartCoroutine(WeaponModelChange());
        weaponNameText.text = actualWeapon.weaponName;

        if (!actualWeapon.isMelee)
            flashSprite.transform.localPosition = actualWeapon.flashTrasform.localPosition;
        else
            ammoText.text = "";

        // when reloading is stopped by weapon changing
        if (isReloading && reloadCoroutine != null)
        {
            StopCoroutine(reloadCoroutine);
            canShoot = true;
            isReloading = false;
        }
    }


    /// <summary> Give player access to weapon or (if player has already access) give him ammo. </summary>
    /// <param name="weaponIndex"></param>
    public void TakeWeapon(int weaponIndex)
    {
        StartCoroutine(gameManager.ShowInfoText("New weapon"));
        GameManager.playerSource.PlayOneShot(pickUpSound);

        // if Player has weapon, he gets ammo
        if (unlockedWeapons[weaponIndex] == true)
        {
            weapons[weaponIndex].ammoAmount += weapons[weaponIndex].magazineCapacity;
            UpdateAmmoText();
            CheckIfMagazineEmpty();
            return;
        }


        // when Player doesn't have weapon, it unlocks
        unlockedWeapons[weaponIndex] = true;
        buttonsInWheel[weaponIndex].transform.GetChild(0).GetComponent<Image>().enabled = true;
        buttonsInWheel[weaponIndex].GetComponentInChildren<TextMeshProUGUI>().enabled = true;
        buttonsInWheel[weaponIndex].interactable = true;
    }

    /// <summary> Just hide weapon wheel. </summary>
    void HideWeaponWheel()
    {
        // needed when player get on vehicle
        weaponWheel.SetActive(false);
    }
}