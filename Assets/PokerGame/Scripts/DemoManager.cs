﻿////////////////////////////////////////////////////////////////////////////////////////////////////////
//FileName: DemoManager.cs
//FileType: C# Source file
//Description : This is a c# script which generate deck cards and handles deck cards logic for pvp game
////////////////////////////////////////////////////////////////////////////////////////////////////////
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using System.Linq;


public class DemoManager : MonoBehaviour
{
    public static DemoManager instance;                                 //Script instance

    public List<Card_SO> deck;                                          //List of scriptable card objects- Original deck

    public GameObject cardPrefab;                                       //Card UI object prefab
    public List<Card_SO> listofCards;                                   //Reference list of all cards

    public int offset_card = 0;//50                                    //Position offset used for placing cards (x- position related offset)

    #region placeholders variables
    public GameObject placeholderBoard;                                //Board reference
    public GameObject placeholderHand;                                 //Player hand reference

    public GameObject[] placeholderPlayerHands;                        //Player hand card reference objects
    public GameObject[] compare_Buttons;                               //Used for comparing hands
    #endregion

    public List<Card> board_cards;                                     //Cards on board (common for both players)

    public List<Card>[] player_cards = new List<Card>[4];              //List of player cards

    public List<RectTransform> _playerCardPosition = new List<RectTransform>();     //Player card position list

    public GameObject _pokerButtons;                                    //Poker button object
    public bool _isFristTimeCard = true;                                //Not important

    private GameObject cardTempObj;                                     //Temporary object used to hold reference for card instantiation

    private int num_hand = 1;                                           //Not in use
    private int num_cards_board = 0;                                    //Number of cards in player's hand count
    public List<Card> demoCards = new List<Card>();                     //Temp demo cards
    public Sprite backSprite;                                           //Card back sprite

    private PhotonView _photonView;                                     //Photon view on this script

    /// <summary>
    /// Set instance of script
    /// </summary>
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }

        //get card from resource
    }
    /// <summary>
    /// Reset cards and generate deck
    /// </summary>
    /// <param name="_gameOver">True : In case of gameover</param>
    public void ResetnumCards(bool _gameOver)
    {
        isGameOver = _gameOver;
        num_cards_board = 0; num_hand = 1;
        for (int i = 0; i < player_cards.Length; i++)
        {
            player_cards[i] = new List<Card>();
        }
        
        foreach (Transform item in placeholderHand.transform)
        {
            if (item.gameObject.GetComponent<Card>())
            {

                Destroy(item.gameObject);
                //Debug.LogError("***CHILD" + item.name);
            }
        }
        board_cards.Clear();
        PVPManager.manager.BoardCards = placeholderHand;
       
        GenerateDeck();
        PhotonNetwork.SendAllOutgoingCommands();
        //Debug.LogError("SecondTimeSuffleCall"); 
        //Invoke("SecondTimeShuffle",1f);

    }
    /// <summary>
    /// Trigger shuffle after some delay
    /// </summary>
    public void SecondTimeShuffleCall()
    {
        //Debug.LogError("SecondTimeSuffleCall");
        Invoke("SecondTimeShuffle", 1f);
    }
    /// <summary>
    /// Shuffle cards from master client
    /// </summary>
    void Start()
    {
        //Initialize the Array
        for (int i = 0; i < player_cards.Length; i++)
        {
            player_cards[i] = new List<Card>();
        }


        _photonView = GetComponent<PhotonView>();
        if (PhotonNetwork.IsMasterClient)
        {
            //listofCards.ToList().Shuffle();
            _photonView.RPC("GetSuffledIndex", RpcTarget.MasterClient);
        }
        PhotonNetwork.SendAllOutgoingCommands();


    }
    /// <summary>
    /// Update deck cards in masterclient- shuffle
    /// </summary>
    public void UpdateCards()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            //listofCards.ToList().Shuffle();
            _photonView.RPC("GetSuffledIndex", RpcTarget.MasterClient);
        }
        PhotonNetwork.SendAllOutgoingCommands();
    }
    /// <summary>
    /// Sync shuffled cards in all player's deck
    /// </summary>
    [PunRPC]
    public void GetSuffledIndex()
    {
        // PVPManager.manager.deckFullList.Shuffle();
        Debug.Log(" ELEM " + listofCards[0].name);
        listofCards.Shuffle();

        Debug.Log(" ELEM AFTER Shuffle " + listofCards[0].name);
        //Debug.LogError("LIST SHUFFLED");
        for (int i = 0; i < listofCards.Count; i++)
        {
            // listofCards[i]=(PVPManager.manager.deckFullList[i]);

            _photonView.RPC("SetSuffledCard", RpcTarget.Others, listofCards[i].name, i);
            PhotonNetwork.SendAllOutgoingCommands();
        }
        _photonView.RPC("StartWithDealy_RPC", RpcTarget.Others);
        PhotonNetwork.SendAllOutgoingCommands();

        Invoke("StartWithDelay", 1f);
    }
    /// <summary>
    /// Set shuffled cards in all player's after resetting data
    /// </summary>
    [PunRPC]
    public void GetSuffledIndexSedondTime()
    {
        if (FindObjectOfType<DackPrefab>())
        {
            Destroy(FindObjectOfType<DackPrefab>().gameObject);
        }
        GameObject obj = Instantiate(Resources.Load("DeckPrefab") as GameObject, this.gameObject.transform.parent);
        obj.GetComponent<DackPrefab>().DeckCards.Shuffle();
        PVPManager.manager.deckFullList.Shuffle();
        Debug.Log("FIST ELEM " + listofCards[0].name);
        //listofCards = PVPManager.manager.deckFullList.ToList();
        listofCards = obj.GetComponent<DackPrefab>().DeckCards;
        Debug.Log("FIST ELEM AFTER Shuffle " + listofCards[0].name);
        //Debug.LogError("LIST SHUFFLED");
        for (int i = 0; i < listofCards.Count; i++)
        {
            // listofCards[i]=(PVPManager.manager.deckFullList[i]);

            _photonView.RPC("SetSuffledCardSedondTime", RpcTarget.Others, listofCards[i].name, i);
            PhotonNetwork.SendAllOutgoingCommands();

        }
        _photonView.RPC("StartWithDealySedondTime_RPC", RpcTarget.Others);
        PhotonNetwork.SendAllOutgoingCommands();

        Invoke("StartWithDelaySecondTime", 1f);


    }
    /// <summary>
    /// Shuffle logic
    /// </summary>
    [PunRPC]
    public void SetSuffledCard(string name, int index)
    {
        Card_SO card = listofCards.ElementAt(index);
        //Debug.LogError("List Of Cards Count " + listofCards.Count);
        for (int i = 0; i < listofCards.Count; i++)
        {
            if (listofCards.ElementAt(i).name == name)
            {
                Swap(listofCards, index, i);
            }
        }
        // Invoke("StartWithDelay", 5f);
    }
    /// <summary>
    /// RPC: used to set card details for oppoent device
    /// </summary>
    /// <param name="cardValues">list of card values</param>
    /// <param name="cardColors">list of card types</param>
    /// <param name="bestIndex">best card index</param>
    [PunRPC]
    public void setOpponentHandCardValues(int[] cardValues, int[] cardColors, int bestIndex)
    {
        PVPManager.manager.OpponentBestIndex = bestIndex;
        PVPManager.manager.opponetHandCardValues.Clear();
        PVPManager.manager.opponetHandCardColors.Clear();
        for (int i = 0; i < cardValues.Length; i++)
        {

            PVPManager.manager.opponetHandCardValues.Add(cardValues[i]);

            PVPManager.manager.opponetHandCardColors.Add(cardColors[i]);
        }
    }
    /// <summary>
    /// RPC : Generate deck 
    /// </summary>
    [PunRPC]
    public void GenerateDeck()
    {
        if (FindObjectOfType<DackPrefab>())
        {
            Destroy(FindObjectOfType<DackPrefab>().gameObject);
        }
        GameObject obj = Instantiate(Resources.Load("DeckPrefab") as GameObject, this.gameObject.transform.parent);

        // GameObject obj =PhotonNetwork.Instantiate("DeckPrefab", this.gameObject.transform.position,Quaternion.identity);
        listofCards = obj.GetComponent<DackPrefab>().DeckCards;
        // Invoke("StartWithDelay", 5f);
    }
    /// <summary>
    /// RPC :Shuffle all cards for second time after a match
    /// </summary>
    [PunRPC]
    public void SetSuffledCardSedondTime(string name, int index)
    {
        Card_SO card = listofCards.ElementAt(index);

        for (int i = 0; i < listofCards.Count; i++)
        {
            if (listofCards.ElementAt(i).name == name)
            {
                Swap(listofCards, index, i);
            }
        }
        // Invoke("StartWithDelay", 5f);


    }
    /// <summary>
    /// Swap lits logic
    /// </summary>
    /// <typeparam name="T">List type</typeparam>
    /// <param name="list">List of items</param>
    public static void Swap<T>(IList<T> list, int indexA, int indexB)
    {
        T tmp = list[indexA];
        list[indexA] = list[indexB];
        list[indexB] = tmp;
    }
    /// <summary>
    /// Copy cards from deck full list after delay
    /// </summary>
    public void StartWithDelay()
    {

        deck = listofCards;
        PVPManager.manager.deckFullList = new Card_SO[listofCards.Count];
        listofCards.CopyTo(PVPManager.manager.deckFullList, 0);
        // Debug.LogError(PVPManager.manager.deckFullList.Length);
    }
    public bool isGameOver = false;     //Boolean used for gameover scenario 
    /// <summary>
    /// Generate 3 poker game cards in pvp mode after Resetting data
    /// </summary>
    public void StartWithDelaySecondTime()
    {

        deck = listofCards;
        // PVPManager.manager.deckFullList = new Card_SO[listofCards.Count];
        // listofCards.CopyTo(PVPManager.manager.deckFullList, 0);
        //Debug.LogError(PVPManager.manager.deckFullList.Length);
        if (PhotonNetwork.IsMasterClient && !isGameOver)
        {
            //Debug.LogError("***Three Cards Generated from here");
            Debug.Log("CARD GENERATION CALL 1");
            DemoManager.instance.Generate3CardsStack();
        }
    }

    /// <summary>
    /// RPC : start card list copy with delay
    /// </summary>
    [PunRPC]
    public void StartWithDealy_RPC()
    {
        Debug.Log("RPC CALLED ");
        Invoke("StartWithDelay", 1);
    }
    /// <summary>
    /// RPC : Copy deck without delay
    /// </summary>
    [PunRPC]
    public void StartWithDealySedondTime_RPC()
    {
        deck = listofCards;
        Debug.Log("RPC CALLED ");
        // Invoke("StartWithDelaySecondTime", 1);
    }
    /// <summary>
    /// Generate 3 cards in poker game
    /// </summary>
    public void Generate3CardsStack()
    {
        Button_DrawFLop();
    }
    /// <summary>
    /// RPC : Shuffle second time
    /// </summary>
    [PunRPC]
    public void SecondTimeShuffle()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            //listofCards.ToList().Shuffle();
            _photonView.RPC("GetSuffledIndexSedondTime", RpcTarget.MasterClient);
            PhotonNetwork.SendAllOutgoingCommands();
        }

    }
    /// <summary>
    /// RPC: Set same stack of cards in other player device
    /// </summary>
    /// <param name="i">Index of card</param>
    /// <param name="_isBool">True :In case for first three cards, False: 4th and 5th card</param>
    /// <param name="cardValue">Value of card</param>
    /// <param name="cardColor">Type of card</param>
    [PunRPC]
    private void RPC_setSameStackToOtherPlayer(int i, bool _isBool, string cardValue, string cardColor)
    {
        if (_isBool)
        {
            var cardObj = InstantiateCard(deck[i], placeholderBoard.transform.position, placeholderHand.transform, i);

            if (placeholderHand.transform.childCount != 0)
            {
                cardObj.GetComponent<RectTransform>().anchoredPosition3D = new Vector3(40 * (placeholderHand.transform.childCount - 1), 0, 0);
            }
            else
            {
                cardObj.GetComponent<RectTransform>().anchoredPosition3D = new Vector3(40 * i, 0, 0);
            }

            cardObj.GetComponent<RectTransform>().localRotation = Quaternion.EulerRotation(0, 0, 0);

            Debug.Log("<color=yellow> if button drawflop other player </color>" + i);
            board_cards.Add(cardObj.GetComponent<Card>());
            cardObj.SetActive(false);

            deck.Remove(deck[i]);
            num_cards_board++;
        }
        else
        {


            var cardObj = InstantiateCard(deck.Find(f => f.cardValue.ToString() == cardValue && f.cardColor.ToString() == cardColor), placeholderBoard.transform.position, placeholderHand.transform, num_cards_board++);

            if (placeholderHand.transform.childCount != i)
            {
                cardObj.GetComponent<RectTransform>().anchoredPosition3D = new Vector3(40 * (placeholderHand.transform.childCount - 1), 0, 0);
            }
            else
            {
                cardObj.GetComponent<RectTransform>().anchoredPosition3D = new Vector3(0, 0, 0);
            }

            Debug.Log("<color=yellow> else button drawflop </color>");

            cardObj.GetComponent<RectTransform>().localRotation = Quaternion.EulerRotation(0, 0, 0);


            board_cards.Add(cardObj.GetComponent<Card>());
            deck.Remove(deck[0]);
        }
    }
    public int count = 0; //Used for child count reference
    /// <summary>
    /// Button to draw the 3 first cards. Any consecutive call will draw one card till there are 5
    /// </summary>
    private void Button_DrawFLop()
    {
        //deck.Shuffle();
        count = placeholderHand.transform.childCount;
        //Debug.LogError("***Num Cards Counts "+ num_cards_board);
        if (num_cards_board < 3) //if there's less than 3 we haven't started yet, so we draw "The Flop", 3 cards on the board
        {

            for (int i = 0; i < 3; i++)
            {
                _photonView.RPC("RPC_setSameStackToOtherPlayer", RpcTarget.Others, i, true, "nun", "nun");
                PhotonNetwork.SendAllOutgoingCommands();
                var cardObj = InstantiateCard(deck[i], placeholderBoard.transform.position, placeholderHand.transform, i);
                //num_hand++;
                //Debug.LogError("***CHILDS " + placeholderHand.transform.childCount);
                //if (placeholderHand.transform.childCount!=0)
                //{

                //    cardObj.GetComponent<RectTransform>().anchoredPosition3D = new Vector3(30 * (placeholderHand.transform.childCount-1), 0, 0);
                //}
                //else
                //{
                //    cardObj.GetComponent<RectTransform>().anchoredPosition3D = new Vector3(30 * i, 0, 0);
                //}

                cardObj.GetComponent<RectTransform>().anchoredPosition3D = new Vector3(40 * i, 0, 0);


                cardObj.GetComponent<RectTransform>().localRotation = Quaternion.EulerRotation(0, 0, 0);
                cardObj.SetActive(false);
                //Debug.LogError("***Deactivated from here");
                //Debug.Log("<color=yellow> if button drawflop master player </color>"+i);
                board_cards.Add(cardObj.GetComponent<Card>());
                deck.Remove(deck[i]);
                num_cards_board++;
            }
        }
        else if (num_cards_board < 5) //We drew the flop already and we need to draw two other cards on the board
        {
            // Debug.LogError("***Code Called");
            _isFristTimeCard = false;

            var cardObj = InstantiateCard(deck[0], placeholderBoard.transform.position, placeholderHand.transform, num_cards_board++);

            _photonView.RPC("RPC_setSameStackToOtherPlayer", RpcTarget.Others, 0, false, deck[0].cardValue.ToString(), deck[0].cardColor.ToString());
            PhotonNetwork.SendAllOutgoingCommands();

            //if (placeholderHand.transform.childCount != 0)
            //{
            //    cardObj.GetComponent<RectTransform>().anchoredPosition3D = new Vector3(30 * (placeholderHand.transform.childCount - 1), 0, 0);
            //}
            //else
            //{
            //    cardObj.GetComponent<RectTransform>().anchoredPosition3D = new Vector3(0, 0, 0);
            //}
            cardObj.GetComponent<RectTransform>().anchoredPosition3D = new Vector3(40 * (num_cards_board - 1), 0, 0);
            Debug.Log("<color=yellow> else button drawflop </color>");

            cardObj.GetComponent<RectTransform>().localRotation = Quaternion.EulerRotation(0, 0, 0);
            board_cards.Add(cardObj.GetComponent<Card>());
            deck.Remove(deck[0]);


        }


        //generate player cards
        _photonView.RPC("RPC_Generate2Cards", RpcTarget.All);
    }

    /// <summary>
    /// RPC generate 2 cards
    /// </summary>
    [PunRPC]
    private void RPC_Generate2Cards()
    {
        Button_Draw4PlayerHands();
    }

    /// <summary>
    /// Draws 2 card per player for 4 players.
    /// </summary>
    private void Button_Draw4PlayerHands()
    {
        //Debug.Log("<color=yellow>=============== Button_Draw4PlayerHands ============== </color>");
        if (PhotonNetwork.IsMasterClient)
        {
            if (player_cards[0].Count == 0)
            {
                // deck.Shuffle();
                for (int p = 2; p < 4; p++)
                {
                    if (deck[p] != null)
                    {
                        deck.Remove(deck[p]);
                    }
                }
                for (int i = 0; i < 1; i++)
                {
                    for (int j = 0; j < 2; j++)
                    {
                        var cardObj = InstantiateCard(deck[j], placeholderPlayerHands[i].transform.position, placeholderPlayerHands[i].transform, j);

                        //set this card to player poisition
                        cardObj.GetComponent<RectTransform>().parent = _playerCardPosition[j];
                        cardObj.GetComponent<RectTransform>().anchoredPosition3D = new Vector3(40 * j, 0, 0);
                        cardObj.GetComponent<RectTransform>().anchorMax = new Vector2(0, 0.5f);
                        cardObj.GetComponent<RectTransform>().anchorMin = new Vector2(0, 0.5f);
                        cardObj.GetComponent<RectTransform>().pivot = new Vector2(0, 0.5f);

                        cardObj.GetComponent<RectTransform>().localRotation = Quaternion.EulerRotation(0, 0, 0);

                        player_cards[i].Add(cardObj.GetComponent<Card>());
                        deck.Remove(deck[j]);
                        opponentCardColor[j] = (int)(cardObj.GetComponent<Card>().cardColor);

                        opponentCardValue[j] = (int)(cardObj.GetComponent<Card>().cardValue);
                        //Debug.Log("else button draw4PlayerHands");
                    }
                    for (int r = 0; r < 1; r++)
                    {
                        if (deck[r] != null)
                        {
                            deck.Remove(deck[r]);
                        }
                    }
                    //compare_Buttons[i].SetActive(true);
                }
                _photonView.RPC("SetOpponentCard_RPC", RpcTarget.Others, opponentCardColor, opponentCardValue);
                Photon.Pun.PhotonNetwork.SendAllOutgoingCommands();

            }
        }
        else
        {
            if (player_cards[0].Count == 0)
            {
                // deck.Shuffle();
                for (int q = 0; q < 2; q++)
                {
                    if (deck[q] != null)
                    {
                        Debug.Log("REMOVED CARDS ");
                        deck.Remove(deck[q]);
                    }
                }
                for (int i = 0; i < 1; i++)
                {
                    for (int j = 0; j < 2; j++)
                    {
                        var cardObj = InstantiateCard(deck[j], placeholderPlayerHands[i].transform.position, placeholderPlayerHands[i].transform, j);

                        //set this card to player poisition
                        cardObj.GetComponent<RectTransform>().parent = _playerCardPosition[j];
                        cardObj.GetComponent<RectTransform>().anchoredPosition3D = new Vector3(40 * j, 0, 0);
                        cardObj.GetComponent<RectTransform>().anchorMax = new Vector2(0, 0.5f);
                        cardObj.GetComponent<RectTransform>().anchorMin = new Vector2(0, 0.5f);
                        cardObj.GetComponent<RectTransform>().pivot = new Vector2(0, 0.5f);

                        cardObj.GetComponent<RectTransform>().localRotation = Quaternion.EulerRotation(0, 0, 0);

                        player_cards[i].Add(cardObj.GetComponent<Card>());

                        deck.Remove(deck[j]);
                        opponentCardColor[j] = (int)(cardObj.GetComponent<Card>().cardColor);

                        opponentCardValue[j] = (int)(cardObj.GetComponent<Card>().cardValue);
                        //Debug.Log("else button draw4PlayerHands");
                    }
                    // compare_Buttons[i].SetActive(true);
                }
                for (int s = 0; s < 1; s++)
                {
                    if (deck[s] != null)
                    {
                        Debug.Log("REMOVED CARDS ");
                        deck.Remove(deck[s]);
                    }
                }
                _photonView.RPC("SetOpponentCard_RPC", RpcTarget.Others, opponentCardColor, opponentCardValue);
                Photon.Pun.PhotonNetwork.SendAllOutgoingCommands();

            }
        }
        //INFO : Original code
        //if (player_cards[0].Count == 0)
        //{
        //    deck.Shuffle();

        //    for (int i = 0; i < 1; i++)
        //    {
        //        for (int j = 0; j < 2; j++)
        //        {
        //            var cardObj = InstantiateCard(deck[j], placeholderPlayerHands[i].transform.position, placeholderPlayerHands[i].transform, j);

        //            //set this card to player poisition
        //            cardObj.GetComponent<RectTransform>().parent = _playerCardPosition[j];
        //            cardObj.GetComponent<RectTransform>().anchoredPosition3D = new Vector3(30*j, 0, 0);
        //            cardObj.GetComponent<RectTransform>().anchorMax = new Vector2(0, 0.5f);
        //            cardObj.GetComponent<RectTransform>().anchorMin = new Vector2(0, 0.5f);
        //            cardObj.GetComponent<RectTransform>().pivot = new Vector2(0, 0.5f);

        //            cardObj.GetComponent<RectTransform>().localRotation = Quaternion.EulerRotation(0, 0, 0);

        //            player_cards[i].Add(cardObj.GetComponent<Card>());
        //            deck.Remove(deck[j]);
        //            opponentCardColor[j] = (int)( cardObj.GetComponent<Card>().cardColor);

        //            opponentCardValue[j] = (int)(cardObj.GetComponent<Card>().cardValue);
        //            //Debug.Log("else button draw4PlayerHands");
        //        }
        //       // compare_Buttons[i].SetActive(true);
        //    }
        //    _photonView.RPC("SetOpponentCard_RPC",RpcTarget.Others,opponentCardColor,opponentCardValue);
        //    Photon.Pun.PhotonNetwork.SendAllOutgoingCommands();

        //}
    }
    int[] opponentCardColor = new int[2] { -1, -1 };  //Opponent card type detail
    int[] opponentCardValue = new int[2] { -1, -1 };  //Opponent card value detail

    /// <summary>
    /// Set cards in opponent device in pvp script
    /// </summary>
    /// <param name="opponentColor">list of card types</param>
    /// <param name="opponentValue">list of card values</param>
    [PunRPC]
    public void SetOpponentCard_RPC(int[] opponentColor, int[] opponentValue)
    {
        for (int i = 0; i < opponentColor.Length; i++)
        {
            PVPManager.manager.opponentCardColor[i] = opponentColor[i];
            PVPManager.manager.opponentCardValue[i] = opponentValue[i];
        }
        PVPManager.manager.GenerateOpponentPlayerCards();
    }
    /// <summary>
    /// Checks what kind of match you have onyour hand and writes it to the UI
    /// </summary>
    /// <param name="player_num"></param>
    public void CompareHand(int player_num)
    {
        Debug.Log(" ========== CompareHand start ======= ");

        List<Card> handToCompare = new List<Card>();

        //cards on the board
        for (int i = 0; i < board_cards.Count; i++)
        {
            handToCompare.Add(board_cards[i]);

            Debug.Log(" ========== CompareHand 1 for ======= ");
        }

        //cards on the hand of player
        for (int i = 0; i < player_cards[player_num].Count; i++)
        {
            handToCompare.Add(player_cards[player_num][i]);

            Debug.Log(" ========== CompareHand 2 ======= ");
        }

        PokerHand pk = new PokerHand();

        //compare them out
        pk.setPokerHand(handToCompare.ToArray());

        compare_Buttons[player_num].transform.GetChild(1).GetComponent<TMPro.TextMeshProUGUI>().text = pk.printResult();

        Debug.Log(" ========== CompareHand end ======= " + pk.printResult());

    }
    public PokerHand pk;           //Poker hand instance
    public List<Card> handToCompare = new List<Card>();   //List of cards in a hand to compare
    /// <summary>
    /// Compart player hands and return poker hand
    /// </summary>
    /// <param name="player_num"></param>
    /// <returns></returns>
    public PokerHand CompareHandWithStrength(int player_num)
    {
        Debug.Log(" ========== CompareHand start ======= ");
        handToCompare.Clear();
        handToCompare = new List<Card>();

        //cards on the board
        for (int i = 0; i < board_cards.Count; i++)
        {
            handToCompare.Add(board_cards[i]);

            Debug.Log(" ========== CompareHand 1 for ======= ");
        }

        //cards on the hand of player
        for (int i = 0; i < player_cards[player_num].Count; i++)
        {
            handToCompare.Add(player_cards[player_num][i]);

            Debug.Log(" ========== CompareHand 2 ======= ");
        }

        pk = new PokerHand();

        //Use demo cards temp
        if (PhotonNetwork.IsMasterClient)
        {
            demoCards[0].cardValue = CardValue.eight;
            demoCards[0].cardColor = CardColor.diamonds;
            demoCards[1].cardValue = CardValue.eight;
            demoCards[1].cardColor = CardColor.spades;
            demoCards[2].cardValue = CardValue.jack;
            demoCards[2].cardColor = CardColor.diamonds;
            demoCards[3].cardValue = CardValue.six;
            demoCards[3].cardColor = CardColor.hearts;
            demoCards[4].cardValue = CardValue.four;
            demoCards[4].cardColor = CardColor.clubs;
            demoCards[5].cardValue = CardValue.three;
            demoCards[5].cardColor = CardColor.spades;
            demoCards[6].cardValue = CardValue.two;
            demoCards[6].cardColor = CardColor.diamonds;

        }
        else
        {
            demoCards[0].cardValue = CardValue.eight;
            demoCards[0].cardColor = CardColor.diamonds;
            demoCards[1].cardValue = CardValue.seven;
            demoCards[1].cardColor = CardColor.spades;
            demoCards[2].cardValue = CardValue.king;
            demoCards[2].cardColor = CardColor.diamonds;
            demoCards[3].cardValue = CardValue.seven;
            demoCards[3].cardColor = CardColor.diamonds;
            demoCards[4].cardValue = CardValue.ace;
            demoCards[4].cardColor = CardColor.hearts;
            demoCards[5].cardValue = CardValue.three;
            demoCards[5].cardColor = CardColor.diamonds;
            demoCards[6].cardValue = CardValue.two;
            demoCards[6].cardColor = CardColor.hearts;
        }
        // handToCompare = demoCards;
        //compare them out
        pk.setPokerHand(handToCompare.ToArray());
        //int index = 0;
        //for(int c = 0 ; c < pk.combinationStrengths.Count ; c++)
        //{
        //    if(pk.combinationStrengths[c]== pk.combinationStrengths.Find(x => x == Mathf.Max(pk.combinationStrengths.Max(y => y))))
        //        {
        //        index = c;
        //    }
        //}
        List<int> myHandCardValues = new List<int>();

        List<int> myHandCardColors = new List<int>();
        if (pk.strength == 0) { pk.bestIndex = 0; }
        for (int i = 0; i < pk.cardCombinations[pk.bestIndex].items.Count(); i++)
        {
            //myHandCardValues.Add( ((int)handToCompare[pk.cardCombinations[pk.bestIndex].items[i]]));
            // myHandCardColors.Add(((int)handToCompare[pk.cardCombinations[pk.bestIndex].items[i]]));
            myHandCardValues.Add(pk.cardCombinations[pk.bestIndex].items[i]);
            myHandCardColors.Add(pk.cardCombinations[pk.bestIndex].items[i]);
        }

        _photonView.RPC("setOpponentHandCardValues", RpcTarget.Others, myHandCardValues.ToArray(), myHandCardColors.ToArray(), pk.bestIndex);
        //HighLightWinnerHand();
        compare_Buttons[player_num].transform.GetChild(1).GetComponent<TMPro.TextMeshProUGUI>().text = pk.printResult();
        // Debug.LogError("*** CompareHand end ======= " + pk.printResult());
        return pk;
        //  return pk.strength;
    }
    /// <summary>
    /// Highlight cards of winner (Cards which are used to decide player's win result)
    /// </summary>
    /// <param name="isPlayerWin"></param>
    public void HighLightWinnerHand(bool isPlayerWin)
    {
        if (isPlayerWin)
        {
            for (int i = 0; i < pk.cardCombinations.Count; i++)
            {
                if (i == pk.bestIndex)
                {
                    // Debug.LogError("*WIN HAND INDEX " + pk.bestIndex);
                    for (int j = 0; j < pk.cardCombinations[i].items.Count(); j++)
                    {
                        // Debug.LogError("*WIN HAND CARD " + pk.cardCombinations[i].items[j]);
                        //  LeanTween.scale(handToCompare[pk.cardCombinations[i].items[j]].gameObject,Vector3.one*(1.2f),.5f);
                        Outline outline = handToCompare[pk.cardCombinations[i].items[j]].gameObject.transform.GetChild(1).gameObject.AddComponent<Outline>();
                        outline.effectColor = Color.green;
                        outline.effectDistance = new Vector2(2, -2);

                    }
                }
            }
        }
        else
        {
            for (int i = 0; i < pk.cardCombinations.Count; i++)
            {
                if (i == PVPManager.manager.OpponentBestIndex)
                {
                    //  Debug.LogError("*WIN HAND INDEX " + pk.bestIndex);
                    for (int j = 0; j < pk.cardCombinations[i].items.Count(); j++)
                    {
                        if (pk.cardCombinations[i].items[j] != 5 && pk.cardCombinations[i].items[j] != 6)
                        {
                            //Debug.LogError("*WIN HAND CARD " + pk.cardCombinations[i].items[j]);
                            //  LeanTween.scale(handToCompare[pk.cardCombinations[i].items[j]].gameObject,Vector3.one*(1.2f),.5f);
                            Outline outline = handToCompare[pk.cardCombinations[i].items[j]].gameObject.transform.GetChild(1).gameObject.AddComponent<Outline>();
                            outline.effectColor = Color.green;
                            outline.effectDistance = new Vector2(2, -2);
                        }
                        else if (pk.cardCombinations[i].items[j] == 5)
                        {
                            Outline outline = PVPManager.manager.OpponentPlayerCardPositions[0].GetChild(0).GetChild(1).gameObject.AddComponent<Outline>();

                            outline.effectColor = Color.green;
                            outline.effectDistance = new Vector2(2, -2);
                        }
                        else if (pk.cardCombinations[i].items[j] == 6)
                        {
                            Outline outline = PVPManager.manager.OpponentPlayerCardPositions[1].GetChild(0).GetChild(1).gameObject.AddComponent<Outline>();


                            outline.effectColor = Color.green;
                            outline.effectDistance = new Vector2(2, -2);
                        }

                    }
                }
            }
            

        }
        PVPManager.manager.OpponentPlayerCardPositions[0].GetChild(0).GetChild(1).gameObject.GetComponent<Image>().sprite = PVPManager.manager.opponentCardSprite[0];
        PVPManager.manager.OpponentPlayerCardPositions[1].GetChild(0).GetChild(1).gameObject.GetComponent<Image>().sprite = PVPManager.manager.opponentCardSprite[1];

    }

    /// <summary>
    /// Instantiates a card 
    /// </summary>
    /// <param name="sO">scriptable object with card data</param>
    /// <param name="pos">position where to instantiate it</param>
    /// <param name="parent">parent gameobject</param>
    /// <param name="num_card">number of card in hand</param>
    /// <returns>Card as a Gameobject</returns>
    public GameObject InstantiateCard(Card_SO sO, Vector3 pos, Transform parent, int num_card)
    {


        cardTempObj = Instantiate(cardPrefab, new Vector3(pos.x + (offset_card * num_card), pos.y - (2 * offset_card), 0), Quaternion.identity, parent);



        cardTempObj.GetComponent<Card>().cardValue = sO.cardValue;
        cardTempObj.GetComponent<Card>().cardColor = sO.cardColor;
        cardTempObj.name = sO.cardValue.ToString() + sO.cardColor.ToString();
        cardTempObj.transform.GetChild(1).GetComponent<Image>().sprite = sO.cardSprite;


        return cardTempObj;

    }
    /// <summary>
    /// Instantiate card for opponent
    /// </summary>
    public GameObject InstantiateCardOpponent(Card_SO sO, Vector3 pos, Transform parent, int num_card)
    {
        cardTempObj = Instantiate(cardPrefab, new Vector3(pos.x + (offset_card * num_card), pos.y - (2 * offset_card), 0), Quaternion.identity, parent);

        cardTempObj.GetComponent<Card>().cardValue = sO.cardValue;
        cardTempObj.GetComponent<Card>().cardColor = sO.cardColor;
        cardTempObj.name = sO.cardValue.ToString() + sO.cardColor.ToString();
        cardTempObj.transform.GetChild(1).GetComponent<Image>().sprite = backSprite;


        return cardTempObj;

    }
}