using MongoDB.Driver;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Linq;
using TMPro;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

public class GameManager : MonoBehaviour
{
    // Total, bet input, bet, thank you delay duration
    public int myTotal { get; set; }
    public int myBetInput { get; set; }
    public int myBetGame { get; set; }
    public int LastBetGame = 0;
    private bool doubleStand=false;

    private int currentDur { get; set; }
    private Timer myTimer;

    public GameObject OneBut;
    public GameObject TwoBut;
    public GameObject FiveBut;
    public GameObject TenBut;
    public Button SplitBut;
    public Button DoubleBut;
    public Button HitBut;
    public Button StandBut;
    public Button BetBut;
    public Button ClearBetBut;
    public Button LastBetBut;
    public Button RestartBut;
    public Button SurrenderBut;
    public AudioSource cardSound;

    public Text inputText;
    public Text outputText;
    public Text totalText;
    public GameObject playerPanel;
    public GameObject playerHandPrefab;
    public Image temp;
    public GameObject playerCardImage;
    public GameObject dealerHand;
    public Text HighName;
    public Text HighScore;
    public Text Info;

    private string username = "";

    // Dealer card total and player card total
    public int dealerCardTotal { get; set; }
    List<int> playerCardTotal = new List<int>();

    // Hand count and stand count
    int handCount = 0;
    int standCount = 0;
    // playerHands list contains playerCards lists
    List<Kart> playerCards = new List<Kart>();
    List<List<Kart>> playerHands = new List<List<Kart>>();

    List<GameObject> cardImages = new List<GameObject>();
    List<List<GameObject>> playerHandImages = new List<List<GameObject>>();

    List<int> myBets = new List<int>();
    List<Image> betTurn=new List<Image>();
    
    List<Text> myBetTexts = new List<Text>();

    // dealerCards list
    List<Kart> dealerCards = new List<Kart>();

    // Deck generation
    Hashtable deste = new Hashtable();
    Kart myKart = null;
    int ID = 0;
    System.Random randomer = new System.Random();
    int myTempRandom = 0;
    List<int> myRandom = new List<int>();
    int cardNo = 0;

    // player hand stackpanels list
    List<GameObject> myStacks = new List<GameObject>();


    // Start is called before the first frame update
    void Start()
    {
        GenerateDeck();
        myTotal = PlayerPrefs.GetInt("score");
        myBetInput = 0;
        myBetGame = 0;
        username=PlayerPrefs.GetString("username");
        GetHighScores();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        inputText.text = myBetInput.ToString();
        totalText.text = myTotal.ToString();

    }

    // Generates 8 decks with IDs 1-416
    // random ID list to generate deck order for first 280 cards (230 is shuffle time)
    private void GenerateDeck()
    {
        for (int k = 0; k < 8; k++)
        {
            for (int i = 2; i < 15; i++)
            {
                ID++;
                myKart = new Kart("kupa", i);
                deste.Add(ID, myKart);
            }
            for (int i = 2; i < 15; i++)
            {
                ID++;
                myKart = new Kart("karo", i);
                deste.Add(ID, myKart);
            }
            for (int i = 2; i < 15; i++)
            {
                ID++;
                myKart = new Kart("sinek", i);
                deste.Add(ID, myKart);
            }
            for (int i = 2; i < 15; i++)
            {
                ID++;
                myKart = new Kart("maca", i);
                deste.Add(ID, myKart);
            }
        }

        while (true)
        {
            int i = 0;
            myTempRandom = randomer.Next(1, 417);
            if (!myRandom.Contains(myTempRandom))
                myRandom.Add(myTempRandom);
            i++;
            if (myRandom.Count == 280)
                break;
        }
    }


    public void One_Click()
    {
        if (myTotal > 99)
        {
            myBetInput += 100;
            myTotal -= 100;
        }
    }
    public void Two_Click()
    {
        if (myTotal > 199)
        {
            myBetInput += 200;
            myTotal -= 200;
        }
    }
    public void Five_Click()
    {
        if (myTotal > 499)
        {
            myBetInput += 500;
            myTotal -= 500;
        }
    }
    public void Ten_Click()
    {
        if (myTotal > 999)
        {
            myBetInput += 1000;
            myTotal -= 1000;
        }
    }
    public void Bet_Click()
    {
        if (myBetInput > 0)
        {

            myBetGame = myBetInput;
            inputText.gameObject.SetActive(false);
            myBets.Add(myBetGame);
            GameStart();
        }




    }
    public void ClearBet_Click()
    {
        myTotal = myTotal + myBetInput;
        myBetInput = 0;
    }

    private void GameStart()
    {
        PlayerScoreUpdateDB();
        LastBetGame = myBetGame;
        OneBut.SetActive(false);
        TwoBut.SetActive(false);
        FiveBut.SetActive(false);
        TenBut.SetActive(false);
        BetBut.gameObject.SetActive(false);
        LastBetBut.gameObject.SetActive(false);
        ClearBetBut.gameObject.SetActive(false);

        // Get two random player cards and one random dealer card

        Kart temp = (Kart)deste[myRandom[cardNo]];
        playerCards.Add(temp);
        cardNo++;
        temp = (Kart)deste[myRandom[cardNo]];
        playerCards.Add(temp);
        playerHands.Add(playerCards);
        cardNo++;
        temp = (Kart)deste[myRandom[cardNo]];
        dealerCards.Add(temp);
        cardNo++;

        playerHandImages.Add(cardImages);
        playerHandImages.Add(cardImages);
        playerHandImages.Add(cardImages);
        playerHandImages.Add(cardImages);
        playerHandImages.Add(cardImages);
        playerHandImages.Add(cardImages);
        playerHandImages.Add(cardImages);

        GameObject playerStack = Instantiate(playerHandPrefab,playerPanel.transform);
        myStacks.Add(playerStack);
        myBetTexts.Add(playerStack.GetComponentInChildren<Text>());
        myBetTexts[0].text = myBets[0].ToString();
        betTurn.Add(playerStack.GetComponentInChildren<Image>());

        playerCardTotal.Add(0);
        playerCardTotal[0] = 0;
        foreach (Kart kart in playerHands[0])
        {
            if (kart.Mag < 11)
                playerCardTotal[0] += kart.Mag;
            else if (kart.Mag > 10 && kart.Mag < 14)
                playerCardTotal[0] += 10;
            else
            {
                playerCardTotal[0] += 1;
            }
        }

        if (dealerCards[0].Mag < 11)
            dealerCardTotal = dealerCards[0].Mag;
        else if (dealerCards[0].Mag > 10 && dealerCards[0].Mag < 14)
            dealerCardTotal = 10;
        else
            dealerCardTotal = 1;

        SpawnGameStart();

    }

    private async void SpawnGameStart()
    {
        SpawnPlayerCard(0, 0);
        await Task.Delay(800);
        SpawnDealerCard(0);
        await Task.Delay(800);
        SpawnPlayerCard(0, 1);

        if ((playerHands[0][0].Mag == playerHands[0][1].Mag) || (playerHands[0][0].Mag > 9 && playerHands[0][0].Mag < 14 && playerHands[0][1].Mag > 9 && playerHands[0][1].Mag < 14))
        {
            if (myBetGame <= myTotal)
            {

                SplitBut.gameObject.SetActive(true);

            }
        }


        HitBut.gameObject.SetActive(true);
        StandBut.gameObject.SetActive(true);
        bool surrenderAvailable = true;
        foreach (Kart kart in dealerCards)
        {
            if (kart.Mag == 14)
            {
                surrenderAvailable=false;
            }
        }
        if (surrenderAvailable)
            SurrenderBut.gameObject.SetActive(true);
        if (myBetGame <= myTotal)
            DoubleBut.gameObject.SetActive(true);
        BlackjackControl(0);

    }
    private void SpawnPlayerCard(int handNo, int cardNo)
    {
        cardSound.Play();
        Kart kart = playerHands[handNo][cardNo];
        int imageID;
        if (kart.ID % 52 == 0)
            imageID = 52;
        else if (kart.ID > 52)
            imageID = kart.ID % 52;
        else
            imageID = kart.ID;
        GameObject img = Instantiate(playerCardImage, myStacks[handNo].transform);
        img.gameObject.transform.localPosition += new Vector3(cardNo * 111.2f,0,0);
        myStacks[handNo].GetComponent<RectTransform>().sizeDelta = new Vector2(110.2f + cardNo * 111.2f, 222f);
        string x="Images/Cards/"+imageID.ToString();
        img.GetComponent<Image>().sprite = Resources.Load<Sprite>(x);
        playerHandImages[handNo].Add(img);
        

    }
    private void SpawnDealerCard(int cardNo)
    {
        cardSound.Play();
        Kart kart = dealerCards[cardNo];
        int imageID;
        if (kart.ID % 52 == 0)
            imageID = 52;
        else if (kart.ID > 52)
            imageID = kart.ID % 52;
        else
            imageID = kart.ID;
        GameObject img = Instantiate(playerCardImage, dealerHand.transform);
        img.gameObject.transform.localPosition += new Vector3(cardNo * 111.2f, 0, 0);
        dealerHand.GetComponent<RectTransform>().sizeDelta = new Vector2(110.2f + cardNo * 111.2f, 222f);
        string x = "Images/Cards/" + imageID.ToString();
        img.GetComponent<Image>().sprite = Resources.Load<Sprite>(x);
        
    }
    public void BlackjackControl(int handNo)
    {
        int tempTotal = 0;
        foreach (Kart kart in playerHands[handNo])
        {
            if (kart.Mag < 11)
                tempTotal += kart.Mag;
            else if (kart.Mag > 10 && kart.Mag < 14)
                tempTotal += 10;
            else
                tempTotal += 11;
        }

        if (tempTotal == 21 && dealerCards[0].Mag < 10 && handCount == 0)
        {
            HitBut.gameObject.SetActive(false);
            StandBut.gameObject.SetActive(false);
            DoubleBut.gameObject.SetActive(false);
            myTotal += 2 * myBets[handNo] + myBets[handNo] / 2;
            myBetTexts[handNo].text = (2 * myBets[handNo] + myBets[handNo] / 2).ToString() + "\nBLACKJACK";
            SurrenderBut.gameObject.SetActive(false);
            RestartBut.gameObject.SetActive(true);
        }
        
        else if (tempTotal == 21 && handCount == 0)
        {

            SurrenderBut.gameObject.SetActive(false);
            HitBut.gameObject.SetActive(false);
            StandBut.gameObject.SetActive(false);
            DoubleBut.gameObject.SetActive(false);
            int dealerCardTotal = 0;
            if (dealerCards[0].Mag < 11)
                dealerCardTotal += dealerCards[0].Mag;
            else if (dealerCards[0].Mag > 10 && dealerCards[0].Mag < 14)
                dealerCardTotal += 10;
            else
                dealerCardTotal += 1;

            Kart temp = (Kart)deste[myRandom[4]];
            dealerCards.Add(temp);
            SpawnDealerCard(1);

            if (temp.Mag < 11)
                dealerCardTotal += temp.Mag;
            else if (temp.Mag > 10 && temp.Mag < 14)
                dealerCardTotal += 10;
            else
                dealerCardTotal += 1;

            foreach (Kart kart in dealerCards)
            {
                if (kart.Mag == 14 && dealerCardTotal + 10 > 16 && dealerCardTotal + 10 < 22)
                {
                    dealerCardTotal += 10;
                }
            }

            if (dealerCardTotal == 21)
            {
                myTotal += myBets[handNo];
                myBetTexts[handNo].text = myBets[handNo].ToString() + "\nTIE";
                RestartBut.gameObject.SetActive(true);
            }

            if (dealerCardTotal < 21)
            {
                myTotal += 2 * myBets[handNo] + myBets[handNo] / 2;
                myBetTexts[handNo].text = (2 * myBets[handNo] + myBets[handNo] / 2).ToString() + "\nBLACKJACK";
                RestartBut.gameObject.SetActive(true);
            }
        }

    }

    public void Clearer()
    {
        playerCardTotal.Clear();
        playerCards.Clear();
        myStacks.Clear();
        playerHands.Clear();
        dealerCards.Clear();
        handCount = 0;
        standCount = 0;
        myBetInput = 0;
        myBets.Clear();
        OneBut.SetActive(true);
        TwoBut.SetActive(true);
        FiveBut.SetActive(true);
        TenBut.SetActive(true);
        LastBetBut.gameObject.SetActive(true);
        BetBut.gameObject.SetActive(true);
        ClearBetBut.gameObject.SetActive(true);
        HitBut.gameObject.SetActive(false);
        StandBut.gameObject.SetActive(false);
        DoubleBut.gameObject.SetActive(false);
        SplitBut.gameObject.SetActive(false);
        betTurn.Clear();
        myBetTexts.Clear();
        RestartBut.gameObject.SetActive(false);
        inputText.gameObject.SetActive(true);
        foreach (Transform child in playerPanel.transform)
        {
            Destroy(child.gameObject);
        }
        foreach (Transform child in dealerHand.transform)
        {
            Destroy(child.gameObject);
        }
        if (cardNo > 230)
        {
            cardNo = 0;
            GenerateDeck();
        }

        cardImages.Clear();
        playerHandImages.Clear();
        PlayerScoreUpdateDB();
    }

    public void Hit_Click()
    {
        SurrenderBut.gameObject.SetActive(false);
        DoubleBut.gameObject.SetActive(false);
        SplitBut.gameObject.SetActive(false);
        Hit(standCount);
    }
    private void Hit(int handNo)
    {
        Kart temp = (Kart)deste[myRandom[cardNo]];
        playerHands[handNo].Add(temp);
        SpawnPlayerCard(handNo, playerHands[handNo].Count - 1);
        cardNo++;

        if (temp.Mag < 11)
            playerCardTotal[handNo] += temp.Mag;
        else if (temp.Mag > 10 && temp.Mag < 14)
            playerCardTotal[handNo] += 10;
        else
        {
            playerCardTotal[handNo] += 1;
        }
        OverCheck(handNo);
    }

    public async void Stand_Click()
    {
        SurrenderBut.gameObject.SetActive(false);
        if (handCount == standCount)
        {
            betTurn[standCount].gameObject.SetActive(false);
            Stand();
        }
        else
        {
            betTurn[standCount].gameObject.SetActive(false);
            ++standCount;
            Hit(standCount);
            await Task.Delay(800);
            if (playerHands[standCount][0].Mag == 14)
            {
                betTurn[standCount].gameObject.SetActive(true);
                HitBut.gameObject.SetActive(false);
                StandBut.gameObject.SetActive(false);
                DoubleBut.gameObject.SetActive(false);
                if (playerHands[standCount][0].Mag == 14 && playerHands[standCount][1].Mag == 14)
                {
                    SplitBut.gameObject.SetActive(true);
                }
                else
                {
                    Stand_Click();
                }
            }
            else
            {
                if (myBetGame <= myTotal)
                    DoubleBut.gameObject.SetActive(true);
                HitBut.gameObject.SetActive(true);
                if ((playerHands[standCount][0].Mag == playerHands[standCount][1].Mag) || (playerHands[standCount][0].Mag > 9 && playerHands[standCount][0].Mag < 14 && playerHands[standCount][1].Mag > 9 && playerHands[standCount][1].Mag < 14))
                {
                    if (myBetGame <= myTotal)
                        SplitBut.gameObject.SetActive(true);
                }
                betTurn[standCount].gameObject.SetActive(true);
            }

        }
    }

    private async void Stand()
    {

        HitBut.gameObject.SetActive(false);
        StandBut.gameObject.SetActive(false);
        DoubleBut.gameObject.SetActive(false);
        SplitBut.gameObject.SetActive(false);
        dealerCardTotal = 0;

        if (dealerCards[0].Mag < 11)
            dealerCardTotal += dealerCards[0].Mag;
        else if (dealerCards[0].Mag > 10 && dealerCards[0].Mag < 14)
            dealerCardTotal += 10;
        else
        {
            dealerCardTotal += 1;
        }

        while (dealerCardTotal < 17)
        {
            await Task.Delay(800);
            Kart temp = (Kart)deste[myRandom[cardNo]];
            dealerCards.Add(temp);
            SpawnDealerCard(dealerCards.Count - 1);
            cardNo++;

            if (temp.Mag < 11)
                dealerCardTotal += temp.Mag;
            else if (temp.Mag > 10 && temp.Mag < 14)
                dealerCardTotal += 10;
            else
            {
                dealerCardTotal += 1;
            }

            foreach (Kart kart in dealerCards)
            {
                if (kart.Mag == 14 && dealerCardTotal + 10 > 16 && dealerCardTotal + 10 < 22)
                {
                    dealerCardTotal += 10;
                }
            }
        }

        for (int p = 0; p < handCount + 1; p++)
        {
            foreach (Kart kart in playerHands[p])
            {
                if (kart.Mag == 14 && playerCardTotal[p] + 10 < 22)
                    playerCardTotal[p] += 10;
            }
        }

        CheckWin();
        RestartBut.gameObject.SetActive(true);
    }

    private void CheckWin()
    {

        for (int i = 0; i < handCount + 1; i++)
        {
            if (playerCardTotal[i] > 21)
            {
                myBetTexts[i].text = "0\nLOSE";
            }
            else if (dealerCardTotal > 21)
            {
                if (playerHands[i].Count == 2 && playerCardTotal[i] == 21)
                {
                    myTotal += 2 * myBets[i] + myBets[i] / 2;
                    myBetTexts[i].text = (2 * myBets[i] + myBets[i] / 2).ToString() + "\nBLACKJACK";
                }
                else
                {
                    myTotal += 2 * myBets[i];
                    myBetTexts[i].text = (2 * myBets[i]).ToString() + "\nWIN";
                }
            }
            else if (dealerCardTotal == 21 && playerCardTotal[i] == 21)
            {
                if (playerHands[i].Count == 2 && dealerCards.Count == 2)
                {
                    myTotal += myBets[i];
                    myBetTexts[i].text = myBets[i].ToString() + "\nTIE";
                }
                else if (playerHands[i].Count == 2)
                {
                    myTotal += 2 * myBets[i] + myBets[i] / 2;
                    myBetTexts[i].text = (2 * myBets[i] + myBets[i] / 2).ToString() + "\nBLACKJACK";
                }
                else if (dealerCards.Count == 2)
                {
                    myBetTexts[i].text = "0\nLOSE";
                }
                else
                {
                    myTotal += myBets[i];
                    myBetTexts[i].text = myBets[i].ToString() + "\nTIE";
                }
            }
            else if (playerCardTotal[i] == 21&& playerHands[i].Count == 2)
            {
                myTotal += 2 * myBets[i] + myBets[i] / 2;
                myBetTexts[i].text = (2 * myBets[i] + myBets[i] / 2).ToString() + "\nBLACKJACK";
            }
            else if (dealerCardTotal == playerCardTotal[i])
            {
                myTotal += myBets[i];
                myBetTexts[i].text = myBets[i].ToString() + "\nTIE";
            }

            else if (dealerCardTotal < playerCardTotal[i])
            {
                myTotal += 2 * myBets[i];
                myBetTexts[i].text = (2 * myBets[i]).ToString() + "\nWIN";
            }
            else
                myBetTexts[i].text = "0\nLOSE";
        }

    }

    private async void OverCheck(int handNo)
    {
        if (playerCardTotal[handNo]< 21) {
            doubleStand = true;
        }

        if (playerCardTotal[handNo] == 21)
        {
            HitBut.gameObject.SetActive(false);
            doubleStand = true;
        }
        if (playerCardTotal[handNo] > 21)
        {
            betTurn[standCount].gameObject.SetActive(false);
            HitBut.gameObject.SetActive(false);
            myBetTexts[handNo].text = "0\nLOSE";
            if (handCount == 0)
            {
                StandBut.gameObject.SetActive(false);
                RestartBut.gameObject.SetActive(true);
            }

            if (handCount > 0)
            {
                await Task.Delay(1200);

                if (handCount == standCount)
                {
                    if (StandNeeded())
                    {
                        betTurn[standCount].gameObject.SetActive(false);
                        Stand();
                    }
                        
                    else
                    {
                        for (int i = 0; i < handCount + 1; i++)
                        {
                            myBetTexts[i].text = "0\nLOSE";
                            StandBut.gameObject.SetActive(false);
                            RestartBut.gameObject.SetActive(true);
                        }
                    }

                }
                else
                {
                    ++standCount;
                    if (myBetGame <= myTotal)
                        DoubleBut.gameObject.SetActive(true);
                    HitBut.gameObject.SetActive(true);
                    Hit(handNo + 1);
                    betTurn[standCount].gameObject.SetActive(true);
                }

            }
        }

    }
    private Boolean StandNeeded()
    {
        bool needed = false;
        for (int i = 0; i < handCount; i++)
        {
            if (playerCardTotal[i] < 22)
                needed = true;
        }
        return needed;
    }
    public void Restart()
    {
        Clearer();
    }

    public void Double_Click()
    {
        doubleStand = false;
        SplitBut.gameObject.SetActive(false);
        DoubleBut.gameObject.SetActive(false);
        SurrenderBut.gameObject.SetActive(false);
        myTotal = myTotal - myBets[standCount];
        myBets[standCount] *= 2;
        myBetTexts[standCount].text = myBets[standCount].ToString();
        Hit(standCount);
        if (doubleStand)
        {
            Stand_Click();
        }

    }

    public void sil()
    {
        if (handCount == 0 && playerCardTotal[standCount] > 21)
        {
            betTurn[standCount].gameObject.SetActive(false);
            myBetTexts[standCount].text = "0\nLOSE";
            StandBut.gameObject.SetActive(false);
            DoubleBut.gameObject.SetActive(false);
            RestartBut.gameObject.SetActive(true);
        }

        else
        {
            Stand_Click();
        }
    }



    public void Split()
    {
        
            SurrenderBut.gameObject.SetActive(false);
            SplitBut.gameObject.SetActive(false);
            ++handCount;
            int i = standCount;
            Destroy(playerHandImages[i][1].gameObject);
            myTotal = myTotal - myBetGame;
            if (myBetGame > myTotal)
            {
                DoubleBut.gameObject.SetActive(false);
            }
            myBets.Add(myBetGame);

            List<Kart> newPlayerCards = new List<Kart>();
            newPlayerCards.Add(playerHands[i][1]);
            playerHands[i].RemoveAt(1);
            playerHands.Add(newPlayerCards);
            if (playerHands[i][0].Mag < 11)
                playerCardTotal[i] = playerHands[i][0].Mag;
            else if (playerHands[i][0].Mag > 10 && playerHands[i][0].Mag < 14)
                playerCardTotal[i] = 10;
            else
            {
                playerCardTotal[i] = 1;
            }
            int temp = 0;
            if (playerHands[playerHands.Count - 1][0].Mag < 11)
                temp = playerHands[playerHands.Count - 1][0].Mag;
            else if (playerHands[playerHands.Count - 1][0].Mag > 10 && playerHands[playerHands.Count - 1][0].Mag < 14)
                temp = 10;
            else
            {
                temp = 1;
            }
            playerCardTotal.Add(temp);

            Destroy(playerHandImages[i][1].gameObject);
            myStacks[i].GetComponent<RectTransform>().sizeDelta = new Vector2(110.2f, 222f);

            GameObject playerStack = Instantiate(playerHandPrefab, playerPanel.transform);
            myStacks.Add(playerStack);
            myBetTexts.Add(playerStack.GetComponentInChildren<Text>());
            myBetTexts[myBetTexts.Count - 1].text = myBets[myBets.Count - 1].ToString();
            betTurn.Add(playerStack.GetComponentInChildren<Image>());
            betTurn[betTurn.Count - 1].gameObject.SetActive(false);
            SpawnPlayerCard(playerHands.Count - 1, 0);

            SplitHit(i);
        
 

    }

    private async void SplitHit(int i) {
        await Task.Delay(800);
        Hit(i);
        if (playerHands[i][0].Mag == 14) {
            HitBut.gameObject.SetActive(false);
            StandBut.gameObject.SetActive(false);
            DoubleBut.gameObject.SetActive(false);
            if(playerHands[i][0].Mag == 14&& playerHands[i][1].Mag == 14)
            {
                SplitBut.gameObject.SetActive(true);
            }
            else
            {
                Stand_Click();
            }
        }
        else if ((playerHands[i][0].Mag == playerHands[i][1].Mag) || (playerHands[i][0].Mag > 9 && playerHands[i][0].Mag < 14 && playerHands[i][1].Mag > 9 && playerHands[i][1].Mag < 14))
        {
            if (myBetGame <= myTotal)
            {
                SplitBut.gameObject.SetActive(true);
            }
        }

    }

    public void LastBet()
    {
        if (myTotal > LastBetGame)
        {
            myBetInput = LastBetGame;
            myTotal -= myBetInput;
        }

        if (myBetInput > 0)
        {

            myBetGame = myBetInput;
            inputText.gameObject.SetActive(false);
            myBets.Add(myBetGame);
            GameStart();
        }
    }

    public void Surrender()
    {
        myTotal += myBets[standCount]/2;
        myBetTexts[standCount].text = (myBets[standCount]/2).ToString() + "\nSURRENDER";
        HitBut.gameObject.SetActive(false);
        StandBut.gameObject.SetActive(false);
        DoubleBut.gameObject.SetActive(false);
        SurrenderBut.gameObject.SetActive(false);
        SplitBut.gameObject.SetActive(false);
        RestartBut.gameObject.SetActive(true);
    }


    private async void PlayerScoreUpdateDB()
    {
        Dictionary<string, int> playerScoreDb = new Dictionary<string, int>();

        const string connectionUri = "mongodb+srv://arda:admin@cluster0.qcgp3dk.mongodb.net/?retryWrites=true&w=majority";
        var settings = MongoClientSettings.FromConnectionString(connectionUri);

        // Create a new client and connect to the server
        var client = new MongoClient(settings);
        string databaseName = "gameDB";
        string collectionName = "Player";
        var db = client.GetDatabase(databaseName);
        var collection = db.GetCollection<Player>(collectionName);


        var filter = Builders<Player>.Filter.Eq(player => player.Name, username);
        var update = Builders<Player>.Update.Set(player => player.Score, myTotal.ToString());

        await collection.UpdateOneAsync(filter, update);

        var results = await collection.FindAsync(_ => true);

        foreach (var result in results.ToList())
        {
            playerScoreDb.Add(result.Name, int.Parse(result.Score));
        }

        var orderedDict = from entry in playerScoreDb orderby entry.Value descending select entry;

        Dictionary<string, int> orderedPlayerScoreDb = orderedDict.ToDictionary<KeyValuePair<string, int>, string, int>(pair => pair.Key, pair => pair.Value);
        string Names = "";
        string Scores = "";
        int l = 1;
        foreach (string result in orderedPlayerScoreDb.Keys)
        {
            Names += l.ToString() + ". " + result + "\n";
            Scores += orderedPlayerScoreDb[result] + "\n";
            l++;
        }
        HighName.text = Names;
        HighScore.text = Scores;
    }

    async void GetHighScores()
    {
        Dictionary<string, int> playerScoreDb = new Dictionary<string, int>();
        const string connectionUri = "mongodb+srv://arda:admin@cluster0.qcgp3dk.mongodb.net/?retryWrites=true&w=majority";
        var settings = MongoClientSettings.FromConnectionString(connectionUri);
        // Create a new client and connect to the server
        var client = new MongoClient(settings);
        string databaseName = "gameDB";
        string collectionName = "Player";
        var db = client.GetDatabase(databaseName);
        var collection = db.GetCollection<Player>(collectionName);

        var results = await collection.FindAsync(_ => true);

        foreach (var result in results.ToList())
        {
            playerScoreDb.Add(result.Name, int.Parse(result.Score));
        }

        var orderedDict = from entry in playerScoreDb orderby entry.Value descending select entry;

        Dictionary<string, int> orderedPlayerScoreDb = orderedDict.ToDictionary<KeyValuePair<string, int>, string, int>(pair => pair.Key, pair => pair.Value);
        string Names = "";
        string Scores = "";
        int l = 1;
        foreach (string result in orderedPlayerScoreDb.Keys)
        {
            Names += l.ToString() + ". " + result + "\n";
            Scores += orderedPlayerScoreDb[result] + "\n";
            l++;
        }
        HighName.text = Names;
        HighScore.text = Scores;
    }

    public async void GetDc()
    {
        Dictionary<string, int> playerDcDb = new Dictionary<string, int>();
        const string connectionUri = "mongodb+srv://arda:admin@cluster0.qcgp3dk.mongodb.net/?retryWrites=true&w=majority";
        var settings = MongoClientSettings.FromConnectionString(connectionUri);
        // Create a new client and connect to the server
        var client = new MongoClient(settings);
        string databaseName = "gameDB";
        string collectionName = "Player";
        var db = client.GetDatabase(databaseName);
        var collection = db.GetCollection<Player>(collectionName);

        var results = await collection.FindAsync(_ => true);

        foreach (var result in results.ToList())
        {
            playerDcDb.Add(result.Name, int.Parse(result.Dc));
        }

        if (playerDcDb[username] > 0 && myTotal<100)
        {
            int dc = myTotal+playerDcDb[username] * 100;
            var filter = Builders<Player>.Filter.Eq(player => player.Name, username);
            var update = Builders<Player>.Update.Set(player => player.Score, dc.ToString());
            await collection.UpdateOneAsync(filter, update);
            var update2 = Builders<Player>.Update.Set(player => player.Dc, "0");
            await collection.UpdateOneAsync(filter, update2);
            myTotal = dc;
            Info.text = "Discount added.";
            GetHighScores();
            
        }
        else if (myTotal > 99)
        {
            Info.text = "Score must be lower than 100.";
        }
        else if (playerDcDb[username] == 0)
        {
            Info.text = "Discount already used.";
        }
    }

}
