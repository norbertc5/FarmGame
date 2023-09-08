using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Quest : MonoBehaviour
{
    [SerializeField] List<QuestThis> quests;
    int killedEnemies;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (quests[0].questTask == QuestThis.Tasks.Kill)
        {
            foreach(Enemy enemy in quests[0].toKill)
            {
                if (enemy.health <= 0 && !enemy.isChecked)
                {
                    killedEnemies++;
                    enemy.isChecked = true;
                }
            }
            if (killedEnemies == quests[0].toKill.Length)
                Debug.Log("Completed");
        }
    }
}

[System.Serializable]
class QuestThis
{
    public enum Tasks { Kill}
    public Tasks questTask;
    public Enemy[] toKill;
}
