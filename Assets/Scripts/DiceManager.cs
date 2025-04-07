using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using TMPro;

public class DiceManager : MonoBehaviour
{
    public GameObject dicePrefab;
    public Transform[] spawnPoints;
    public Transform patternPanelParent;
    public GameObject clearTilePrefab;
    public GameObject gameStartText;
    public GameObject clearPatternNoticeText;

    public GameManager gameManager;
    private List<GameObject> diceList = new List<GameObject>();

    void Start()
    {
        StartCoroutine(StartGameAfterDiceRoll());
    }

    IEnumerator StartGameAfterDiceRoll()
    {
        SpawnAllDice();
        RollAllDice();

        yield return new WaitForSeconds(3f); // 회전 시간

        foreach (GameObject dice in diceList)
        {
            dice.GetComponent<Dice>().StopRoll();
        }

        Color[] pattern = GetTopFaceColors();
        SetClearPatternPanel(pattern);

        if (clearPatternNoticeText != null)
        {
            clearPatternNoticeText.SetActive(true);
            yield return new WaitForSeconds(1f);
            clearPatternNoticeText.SetActive(false);
        }

        foreach (GameObject dice in diceList)
        {
            Destroy(dice);
        }

        gameStartText.SetActive(true);
        yield return new WaitForSeconds(1f);
        gameStartText.SetActive(false);

        gameManager.StartGame(pattern);
    }

    void SpawnAllDice()
    {
        for (int i = 0; i < spawnPoints.Length; i++)
        {
            GameObject dice = Instantiate(dicePrefab, spawnPoints[i].position, Random.rotation);
            diceList.Add(dice);
        }
    }

    void RollAllDice()
    {
        foreach (GameObject dice in diceList)
        {
            dice.GetComponent<Dice>().Roll();
        }
    }

    Color[] GetTopFaceColors()
    {
        List<Color> result = new List<Color>();
        foreach (GameObject dice in diceList)
        {
            result.Add(dice.GetComponent<Dice>().DetectTopColor());
        }
        return result.ToArray();
    }

    void SetClearPatternPanel(Color[] pattern)
    {
        foreach (Transform child in patternPanelParent)
        {
            Destroy(child.gameObject);
        }

        foreach (Color c in pattern)
        {
            GameObject tile = Instantiate(clearTilePrefab, patternPanelParent);
            tile.GetComponent<UnityEngine.UI.Image>().color = c;
        }
    }
}
