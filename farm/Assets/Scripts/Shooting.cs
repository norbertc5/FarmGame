using System.Collections;
using TMPro;
using UnityEngine;

public class Shooting : MonoBehaviour
{
    [SerializeField] float crosshairSpreadWhenShoot = 20;
    [SerializeField] Animator armsAnim;
    [SerializeField] SpriteRenderer flashSprite;
    [SerializeField] TextMeshProUGUI ammoText;
    [SerializeField] Weapon[] weapons;
    [SerializeField] Animation weaponChangeAnim;
    [SerializeField] GameObject weaponWheel;
    [SerializeField] TextMeshProUGUI weaponNameText;
    public static float recoil;
    Weapon actualWeapon;
    bool canShoot = true;
    bool autoFireDelay = true;
    Coroutine reloadCoroutine;
    bool isReloading;
    Crosshair crosshair;
    PoolManager poolManager;
    int actualBulletHoleIndex;

    private void Start()
    {
        actualWeapon = weapons[0];
        UpdateAmmoText();
        weaponNameText.text = actualWeapon.weaponName;
        crosshair = FindObjectOfType<Crosshair>();
        poolManager = FindObjectOfType<PoolManager>();
    }

    void Update()
    {
        // scale of weapons is independent of player scale
        transform.localScale = new Vector3(1 / transform.parent.localScale.x,
        1 / transform.parent.localScale.y, 1 / transform.parent.localScale.z);

        #region Raycasting

        RaycastHit hit;
        Ray ray;
        ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        if (!actualWeapon.isMelee && ((!actualWeapon.isAuto && Input.GetMouseButtonDown(0)) ||
            (actualWeapon.isAuto && Input.GetMouseButton(0) && autoFireDelay)))
        {
            if(canShoot && actualWeapon.ammoInMagazine > 0)
            {
                Crosshair.spread += crosshairSpreadWhenShoot;
                Physics.Raycast(ray, out hit);
                armsAnim.CrossFade("recoilAnim", 0);
                Debug.Log(hit.collider.name);
                StartCoroutine(ShowFlash());
                actualWeapon.ammoInMagazine--;
                UpdateAmmoText();
                CheckIfMagazineEmpty();

                #region Bullet holes

                GameObject bulletHole = poolManager.GetObjectFromPool(1);
                bulletHole.transform.position = hit.point;
                bulletHole.transform.rotation = Quaternion.FromToRotation(Vector3.up, hit.normal);

                #endregion

                if (!PlayerMovement.IsCrouching)
                    recoil += 2;

                if (actualWeapon.isAuto)
                    StartCoroutine(AutoFire());
            }
        }

        #endregion

        #region Reload

        if (Input.GetKeyDown(KeyCode.R) && actualWeapon.ammoInMagazine < actualWeapon.magazineCapacity)
        {
            reloadCoroutine = StartCoroutine(Reload());
        }

        #endregion

        #region Weapon Change

        // changing with keyboard
        if (Input.GetKeyDown(KeyCode.Alpha1))
            ChangeWeapon(0);
        else if (Input.GetKeyDown(KeyCode.Alpha2))
            ChangeWeapon(1);
        if (Input.GetKeyDown(KeyCode.Alpha3))
            ChangeWeapon(2);

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
            weaponWheel.SetActive(false);
            PlayerMovement.ChangeMovementPossibility(true);
            Cursor.lockState = CursorLockMode.Locked;
            crosshair.gameObject.SetActive(true);

            if (!isReloading)
                canShoot = true;
        }

        #endregion
    }

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

    IEnumerator Reload()
    {
        // break if no ammo
        if(actualWeapon.ammoAmount <= 0)
            yield break;

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

    /// <summary> Change weapon which is player using now.</summary>
    /// <param name="newWeapon"></param>
    public void ChangeWeapon(int newWeapon)
    {
        if (weapons[newWeapon] == actualWeapon)
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
}