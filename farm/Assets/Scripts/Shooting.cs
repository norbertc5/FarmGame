using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Animations.Rigging;

public class Shooting : MonoBehaviour
{
    [SerializeField] float crosshairSpreadWhenShoot = 20;
    [SerializeField] Animator armsAnim;
    [SerializeField] SpriteRenderer flashSprite;
    [SerializeField] TextMeshProUGUI ammoText;
    [SerializeField] Weapon[] weapons;
    [SerializeField] Animation weaponChangeAnim;
    public static float recoil;
    Weapon actualWeapon;
    bool canShoot = true;
    int actualWeaponIndex;
    bool autoFireDelay = true;
    Coroutine reloadCoroutine;
    bool isReloading;

    private void Start()
    {
        actualWeapon = weapons[0];
        UpdateAmmoText();
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

        if ((!actualWeapon.isAuto && Input.GetMouseButtonDown(0)) ||
            (actualWeapon.isAuto && Input.GetMouseButton(0) && autoFireDelay))
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

        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            if (actualWeaponIndex == 0)
                actualWeaponIndex = 1;
            else
                actualWeaponIndex = 0;

            armsAnim.CrossFade("weaponChange", 0);
            actualWeapon = weapons[actualWeaponIndex];
            flashSprite.transform.localPosition = actualWeapon.flashTrasform.localPosition;
            StartCoroutine(WeaponChange());

            // when reloading is stopped by weapon changing
            if(isReloading && reloadCoroutine != null)
            {
                StopCoroutine(reloadCoroutine);
                canShoot = true;
                isReloading = false;
            }
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
        ammoText.text = "Ammo: " + actualWeapon.ammoInMagazine + "/" + actualWeapon.ammoAmount;
    }

    IEnumerator Reload()
    {
        if(actualWeapon.ammoAmount > 0)
        {
            switch(actualWeaponIndex)
            {
                case 0: armsAnim.CrossFade("pistolReloading", 0); break;
                case 1: armsAnim.CrossFade("rifleReloading", 0); break;
            }

            // play animation for second hand while reloading
            if(actualWeapon.additinonalReloadingAnim != null)
                actualWeapon.additinonalReloadingAnim.CrossFade("Reloading", 0);

            canShoot = false;
            isReloading = true;
            yield return new WaitForSeconds(0.1f);  // to avoid glitches
            yield return new WaitForSeconds(armsAnim.GetCurrentAnimatorClipInfo(0)[0].clip.length - 0.5f);  // wait until anim ends
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
    }

    /// <summary> Change weapon when it isn't visible in animation.</summary>
    IEnumerator WeaponChange()
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

    void CheckIfMagazineEmpty()
    {
        // start reloading when no ammo in magazine
        if (actualWeapon.ammoInMagazine <= 0)
            reloadCoroutine = StartCoroutine(Reload());
    }
}
