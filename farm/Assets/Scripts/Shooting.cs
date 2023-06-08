using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Animations.Rigging;

public class Shooting : MonoBehaviour
{
    [SerializeField] float crosshairSpread = 20;
    [SerializeField] Animator armsAnim;
    [SerializeField] SpriteRenderer flashSprite;
    [SerializeField] TextMeshProUGUI ammoText;
    [SerializeField] Weapon[] weapons;
    [SerializeField] Animator armAnim;
    public static float recoil;
    int ammoAmount = 20;
    int ammoInMagazine = 8;
    int magazineCapacity = 8;
    bool canShoot = true;
    int actualWeaponIndex;

    private void Start()
    {
        UpdateAmmoText();
    }

    void Update()
    {
        // scale of weapons is independent of player scale
        transform.localScale = new Vector3(1 / transform.parent.localScale.x,
        1 / transform.parent.localScale.y, 1 / transform.parent.localScale.z);

        RaycastHit hit;
        Ray ray;
        ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        if (Input.GetMouseButtonDown(0) && canShoot && ammoInMagazine > 0)
        {
            Crosshair.spread += crosshairSpread;
            Physics.Raycast(ray, out hit);
            armsAnim.CrossFade("recoilAnim", 0);
            Debug.Log(hit.collider.name);
            StartCoroutine(ShowFlash());
            ammoInMagazine--;
            UpdateAmmoText();

            if (ammoInMagazine <= 0)
                StartCoroutine(Reload());

            if (!PlayerMovement.IsCrouching)
                recoil += 2;
        }

        if (Input.GetKeyDown(KeyCode.R) && ammoInMagazine < magazineCapacity)
        {
            StartCoroutine(Reload());
        }

        if(Input.GetKeyDown(KeyCode.Alpha1))
        {
            if (actualWeaponIndex == 0)
            {
                actualWeaponIndex = 1;
                //armsAnims[actualWeaponIndex].CrossFade("Rifle", 0);
            }
            else
            {
                actualWeaponIndex = 0;
                //armsAnims[actualWeaponIndex].CrossFade("Pistol", 0);
            }

            foreach(Weapon weapon in weapons)
            {
                weapon.gameObject.SetActive(false);
            }

            weapons[actualWeaponIndex].gameObject.SetActive(true);
            armsAnim.CrossFade("weaponChange", 0);
        }
    }

    IEnumerator ShowFlash()
    {
        flashSprite.gameObject.SetActive(true);
        yield return new WaitForSeconds(0.1f);
        flashSprite.gameObject.SetActive(false);
    }

    void UpdateAmmoText()
    {
        ammoText.text = "Ammo: " + ammoInMagazine + "/" + ammoAmount;
    }

    IEnumerator Reload()
    {
        if(ammoAmount > 0)
        {
            switch(actualWeaponIndex)
            {
                case 0: armsAnim.CrossFade("pistolReloading", 0); break;
                case 1: armsAnim.CrossFade("rifleReloading", 0);
                    armAnim.CrossFade("rifleReloadingArm", 0); break;
            }
            canShoot = false;
            yield return new WaitForSeconds(0.1f);  // to avoid glitches
            yield return new WaitForSeconds(armsAnim.GetCurrentAnimatorClipInfo(0)[0].clip.length - 0.5f);  // wait until anim ends
            canShoot = true;

            // count how many ammo add to magazine
            int ammoToAdd = 0;
            for (int i = ammoInMagazine; i < magazineCapacity; i++)
            {
                if (ammoAmount > 0)
                {
                    ammoToAdd++;
                    ammoAmount--;
                }
            }

            ammoInMagazine += ammoToAdd;
            UpdateAmmoText();
        }
    }
}
