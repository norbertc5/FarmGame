using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bush : MonoBehaviour
{
    [SerializeField] Transform leavesHolder;
    Transform playerTrans;
    GameObject[] leaves;
    GameManager gameManager;

    private void Start()
    {
        playerTrans = FindObjectOfType<Player>().transform;
        gameManager = FindObjectOfType<GameManager>();

        leaves = new GameObject[leavesHolder.childCount];
        for (int i = 0; i < leavesHolder.childCount; i++)
        {
            leaves[i] = leavesHolder.GetChild(i).gameObject;
        }
        SetLeavesActive(10);
        gameManager.BushOptymalization += CheckDistToPlayer;
    }

    /// <summary> Enable leaves based on distance to player. </summary>
    void CheckDistToPlayer()
    {
        float dist = Vector3.Distance(transform.position, playerTrans.position);

        if (dist < 50 && dist > 10)
            SetLeavesActive(5);
        if (dist < 10)
            SetLeavesActive(1);
    }

    /// <summary> Disable all leaves. Next enable every [jump] leaf. </summary>
    /// <param name="jump"></param>
    void SetLeavesActive(int jump)
    {
        // disabling leaves is needed to optymalization (leaves are very expensive to render)
        // disable all leaves
        foreach (GameObject leave in leaves)
        {
            leave.SetActive(false);
        }

        // enable selected leaves
        for (int i = 0; i < leavesHolder.childCount; i += jump)
        {
            leaves[i].SetActive(true);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Scythe"))
        {
            GetComponent<Animator>().enabled = true;
        }
    }
}
