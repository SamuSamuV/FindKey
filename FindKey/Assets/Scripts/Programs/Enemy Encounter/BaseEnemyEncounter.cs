using UnityEngine;
using static EnemyEncounterData;

public class BaseEnemyEncounter : MonoBehaviour
{
    public GameObject nonEnemyFindedPanel;

    [Tooltip("Arrastra aquí el objeto de la imagen del enemigo que quieres ocultar.")]
    public GameObject enemyVisuals;

    public MoveAppData moveAppData;

    void Start()
    {
        GameObject goMoveAppData = GameObject.FindGameObjectWithTag("MoveAppData");
        moveAppData = goMoveAppData.GetComponent<MoveAppData>();

        if (moveAppData.playerIsFrontCat)
        {
            nonEnemyFindedPanel.SetActive(false);
        }

        DesktopManager dm = FindObjectOfType<DesktopManager>();

        if (dm != null && dm.iconsToSpawn != null)
        {
            foreach (var data in dm.iconsToSpawn)
            {
                if (data.label == "FindKey.exe")
                {
                    if (data.isOpen && data.windowInstance != null)
                    {
                        if (moveAppData.playerIsFrontCat)
                        {
                            Moves moves = data.windowInstance.GetComponent<Moves>();
                            moves.GoToCatPosition();
                        }
                    }
                    break;
                }
            }
        }
    }

    void Update()
    {
        if (enemyVisuals != null && nonEnemyFindedPanel != null)
        {
            if (enemyVisuals.activeSelf == nonEnemyFindedPanel.activeSelf)
            {
                enemyVisuals.SetActive(!nonEnemyFindedPanel.activeSelf);
            }
        }
    }
}