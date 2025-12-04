using PrimeTween;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;

public class CardController : MonoBehaviour
{
    [SerializeField] Card cardPrefab;
    [SerializeField] Transform gridTransform;
    [SerializeField] Sprite[] sprites;
    [SerializeField] Sprite[] sprites2x2;
    [SerializeField] Sprite[] sprites2x3;
    [SerializeField] Sprite[] sprites5x6;
    [SerializeField] GameObject LevelPanel,Homepanel; 
    [SerializeField] GameObject WinPanel; 

    public List<Sprite> spritePairs;
    public Text matchText;
    public Text turnText;
    public Text scoreText;

    Card firstSelected;
    Card secondSelected;

    int matchCount = 0;
    int turnCount = 0;
    int score = 0;

    
    // Start is called before the first frame update
    void Start()
    {
        if (!PlayerPrefs.HasKey("PlayerScore")) // Setup PlayerPrefs for saving Score
        {
            PlayerPrefs.SetInt("PlayerScore", 0);
            PlayerPrefs.Save();
        }

        score = PlayerPrefs.GetInt("PlayerScore");
        scoreText.text = "Score: " + score;
        
    }

    // Fetching Sprites for the Cards as per the Grid
    private void PrepareSprites(Sprite[] newSpirtes) 
    {
        spritePairs = new List<Sprite>();
        for (int i = 0; i < newSpirtes.Length; i++)
        {
            spritePairs.Add(newSpirtes[i]);
            spritePairs.Add(newSpirtes[i]);
        }
        ShuffleSprites(spritePairs);
    }

    // Shuffle sprites for the arrangement
    private void ShuffleSprites(List<Sprite> spriteLists)
    {
        for (int i = spriteLists.Count - 1; i > 0; i--)
        {
            int randomIndex = Random.Range(0, i + 1);
            Sprite temp = spriteLists[i];
            spriteLists[i] = spriteLists[randomIndex];
            spriteLists[randomIndex] = temp;
        }
    }

    // Genrate cards in the Grid
    private void CreateCards(int x, int y)
    {
        for (int i = 0; i < (x * y); i++) 
        {
            Card card = Instantiate(cardPrefab, gridTransform);
            card.SetIconSprite(spritePairs[i]);
            card.controller = this;
            Tween.Rotation(transform, new Vector3(0f, 180f, 0f), 10f);
            Tween.Delay(10f, () => Tween.Rotation(transform, new Vector3(0f, 0f, 0f), 10f));
            card.Show();
            Tween.Delay(1f, () => card.Hide());
        }

        

    }

    // Selected Cards
    public void SetSelected(Card card)
    {
        if (card.isSelected == false)
        {
            card.Show();

            if (firstSelected == null)
            {
                firstSelected = card;
                AudioController.Instance.PlayCardFlipSound(); // Playing Audio for Card Flip
                return;
            }

            if (secondSelected == null)
            {
                secondSelected = card;
                StartCoroutine(CheckMatching(firstSelected, secondSelected));
                firstSelected = null;
                secondSelected = null;
                turnCount++;
                turnText.text = "Turns : " + turnCount;
            }
        }
    }

    // Checking for Cards Matching
    IEnumerator CheckMatching(Card a, Card b)
    {
        yield return new WaitForSeconds(0.3f);
        if (a.iconSprite == b.iconSprite)
        {
            // Matched
            matchCount++;
            matchText.text = "Matches : " + matchCount;

            AudioController.Instance.PlayCardMatchingSound(); // Playing Audio for Matched

            if (matchCount >= spritePairs.Count / 2)
            {
                PrimeTween.Sequence.Create()
                    .Chain(PrimeTween.Tween.Scale(gridTransform, Vector3.one * 1.2f, 0.2f, ease: PrimeTween.Ease.OutBack))
                    .Chain(PrimeTween.Tween.Scale(gridTransform, Vector3.one, 0.2f));

                Tween.Scale(WinPanel.transform, Vector3.one, 1f);
                AddPoints(5);
                AudioController.Instance.PlayWinPopUpSound(); // Playing Audio for Win Pop up

            }
        }
        else
        {
            // Unmatched
            AudioController.Instance.PlayCardMisMatchedSound(); // Playing Audio for MisMatched

            a.Hide();
            b.Hide();
        }
    }

    // For Add/Show Score
    public void AddPoints(int points)
    {
        score += points;
        PlayerPrefs.SetInt("PlayerScore", score);
        scoreText.text = "Score: " + score;
        PrimeTween.Sequence.Create()
                    .Chain(PrimeTween.Tween.Scale(scoreText.transform, Vector3.one * 1.2f, 0.2f, ease: PrimeTween.Ease.OutBack))
                    .Chain(PrimeTween.Tween.Scale(scoreText.transform, Vector3.one, 0.2f));

    }

    // Set up Grid for the Cards
    public void SetupGrid(string arg)
    {
        AudioController.Instance.PlayButtonClickSound(); // Playing Audio for Button

        string[] parts = arg.Split(',');
        int x = int.Parse(parts[0]);
        int y = int.Parse(parts[1]);
        if(x == 5)
            gridTransform.gameObject.GetComponent<GridLayoutGroup>().cellSize = new Vector2(120f, 150f);
        else
            gridTransform.gameObject.GetComponent<GridLayoutGroup>().cellSize = new Vector2(200f, 250f);

        gridTransform.gameObject.GetComponent<GridLayoutGroup>().constraintCount = x;
        

        if(y == 2)
            PrepareSprites(sprites2x2);
        else if(y == 3)
            PrepareSprites(sprites2x3);
        else if(y == 6)
            PrepareSprites(sprites5x6);

        CreateCards(x, y);
        LevelPanel.SetActive(false);
    }

    // Back to Main
    public void BackToLevel(string s)
    {
        AudioController.Instance.PlayButtonClickSound(); // Playing Audio for Button

        if (s == "win")
            Tween.Scale(WinPanel.transform, Vector3.zero, 0.1f);
        
        foreach (Transform child in gridTransform)
        {
            Destroy(child.gameObject);
        }
        LevelPanel.SetActive(true);

        matchCount = 0;
        turnCount = 0;
        turnText.text = "Turns : ";
        matchText.text = "Matches : ";

    }

    public void StartTheGame()
    {
        AudioController.Instance.PlayButtonClickSound(); // Playing Audio for Button
        Homepanel.SetActive(false);
    }

    public void BackToHome()
    {
        AudioController.Instance.PlayButtonClickSound(); // Playing Audio for Button
        Homepanel.SetActive(true);
    }

}
