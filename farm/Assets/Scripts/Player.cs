/*using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("EnemyViewRange"))
        {
            other.GetComponentInParent<Enemy>().canSeePlayer = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("EnemyViewRange"))
        {
            other.GetComponentInParent<Enemy>().canSeePlayer = false;
            Debug.Log("wysz³em");
        }
    }
}
*/
