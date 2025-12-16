using UnityEngine;

public class EnemyEncounterData : MonoBehaviour
{
    public MoveAppData moveAppData;

    public GameObject nonEnemyFindedPanel;

    void Start()
    {
        GameObject goMoveAppData = GameObject.FindGameObjectWithTag("MoveAppData");

        if (!moveAppData.catIsDead)
            nonEnemyFindedPanel.SetActive(false);
    }

    void Update()
    {
        
    }
}
