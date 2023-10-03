using System.Collections;
using System.Collections.Generic;
using System.Text;
using TMPro;
using UnityEngine;

public class QuestsManager : MonoBehaviour
{
    [Header("Quests managing")]
    [SerializeField] TextMeshProUGUI taskBarText;
    [SerializeField] int dayIndex = 0;
    [SerializeField] QuestsCollection[] questsCollections;
    int actualQuestIndex;
    Quest actualQuest;
    int killedEnemies;
    bool areAllCompleted;

    [Header("References")]
    [SerializeField] Transform target;
    [SerializeField] Transform pin;
    [SerializeField] RotateToTarget directionPin;
    Player player;

    private void Awake()
    {
        player = FindObjectOfType<Player>();
       // questsCollections = FindObjectsOfType<QuestsCollection>();
        actualQuest = questsCollections[dayIndex].questsForDay[0];
        UpdateMiniMapPin();
    }

    void Update()
    {
        switch(actualQuest.questTask)
        {
            case Quest.Tasks.Kill:
                #region Quests based on killing.

                // check if enemies to kill are killed or not
                foreach (Enemy enemy in actualQuest.toKill)
                {
                    if (enemy.health <= 0 && !enemy.isChecked)
                    {
                        killedEnemies++;
                        enemy.isChecked = true;
                    }
                }

                // end quest when finished and skip to next one
                if (killedEnemies == actualQuest.toKill.Length)
                    MoveToNextQuest();

                #endregion
                break;
            case Quest.Tasks.WaterPlant:
                #region Quests based on watering.

                // check if player is in watering area and using watering can
                if (actualQuest.wateringCan.isWatering && player.isInWateringArea)
                {
                    // player must water plants for wateringDuration time
                    actualQuest.wateringDuration -= Time.deltaTime;

                    if (actualQuest.wateringDuration <= 0)
                        MoveToNextQuest();
                }

                #endregion
                break;
            case Quest.Tasks.PickUp:
                #region Quests based on picking up objects.

                if (!actualQuest.objectToTake.gameObject.activeSelf)
                    MoveToNextQuest();

                #endregion
                break;
        }

        // update task bar in each frame to refresh values
        UpdateTaskBar(actualQuest.taskDescription, killedEnemies, actualQuest.toKill.Length);

        if (actualQuest.isTargetMoving)
            UpdateMiniMapPin();
    }

    /// <summary> Update task bar and optional show values. </summary>
    /// <param name="text"></param>
    /// <param name="intValue"></param>
    /// <param name="targetValue"></param>
    void UpdateTaskBar(string text, int intValue = 0, int targetValue = 0)
    {
        // stop when all questsCollections completed
        if (areAllCompleted)
        {
            taskBarText.text = "Completed";
            return;
        }

        StringBuilder sb = new StringBuilder();

        // show values (e.g "2/5") only if needed
        if (actualQuest.showValuesInDescription)
            text += " " + intValue.ToString() + "/" + targetValue;

        sb.Append("<font=AlteHaasGroteskRegular SDF><mark=#38383880>" + " " + text + " ‎" + "</font></mark>");

        taskBarText.text = sb.ToString();
    }

    /// <summary> Make next quest active. </summary>
    void MoveToNextQuest()
    {
        // don't skip to next questsCollections when all are completed
        if (actualQuestIndex < questsCollections[dayIndex].questsForDay.Count - 1)
            actualQuestIndex++;
        else
            areAllCompleted = true;

        killedEnemies = 0;
        actualQuest = questsCollections[dayIndex].questsForDay[actualQuestIndex];
        UpdateMiniMapPin();
    }

    void UpdateMiniMapPin()
    {
        pin.transform.position = actualQuest.pinTrans.position;
        directionPin.target = actualQuest.pinTrans;
    }
}

[System.Serializable]
public class Quest
{
    public enum Tasks { Kill, WaterPlant, PickUp}
    public Tasks questTask;
    public string taskDescription;
    public bool showValuesInDescription;
    public Transform pinTrans;

    [Header("Kill")]
    public Enemy[] toKill;
    public bool isTargetMoving;  // if true pin on mini map will update in each frame

    [Header("WaterPlant")]
    public WateringCan wateringCan;
    public float wateringDuration = 1;

    [Header("PickUp")]
    public ObjectToTake objectToTake;
}
