using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Shooting : MonoBehaviour
{
    [SerializeField] float crosshairSpread = 20;
    [SerializeField] Animator armsAnim;
    [SerializeField] SpriteRenderer flashSprite;
    [SerializeField] TextMeshProUGUI ammoText;
    public static float recoil;
    int ammoAmount = 20;
    int ammoInMagazine = 8;
    int magazineCapacity = 8;
    bool canShoot = true;

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
            armsAnim.CrossFade("pistolReloading", 0);
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
