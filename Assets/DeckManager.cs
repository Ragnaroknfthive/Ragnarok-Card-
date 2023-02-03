using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using TMPro;
using System.Linq;

public class DeckManager : MonoBehaviour
{

    public List<SpellCard> playerDeck;
    public List<SpellCard> PetInventory;
    public static DeckManager instance;

    public Transform deck_parent;
    public GameObject deckDispPref;

    public Transform inventory_parent;
    public GameObject inventoryDispPref;
    public TextMeshProUGUI count;
    public TextMeshProUGUI title;
    public UI_panel PetPanel;

    public string currentScreen;
    public bool isPetOpen
    {
        get
        {
            return currentScreen == "Pet";
        }
    }

    /// <summary>
    /// Awake is called when the script instance is being loaded.
    /// </summary>
    void Awake()
    {
        instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        string deck_str = PlayerPrefs.GetString("player_deck", "");

        if (deck_str != "")
        {
            foreach (var item in deck_str.Split('_'))
            {
                if (!playerDeck.Contains(GetCard(System.Convert.ToInt32(item))))
                    playerDeck.Add(GetCard(System.Convert.ToInt32(item)));
            }
        }

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void AddCard(SpellCard card)
    {
        playerDeck.Add(card);
        UpdateDeckPreview(currentScreen);
    }
    public void RemoveCard(SpellCard card)
    {

        playerDeck.Remove(card);
        UpdateDeckPreview(currentScreen);
    }

    public void UpdateDeckPreview(string s)
    {
        foreach (Transform item in inventory_parent)
        {
            Destroy(item.gameObject);
        }
        foreach (Transform item in deck_parent)
        {
            Destroy(item.gameObject);
        }



        int i = 0;
        foreach (var item in playerDeck.Where(i => i.cardType == (isPetOpen ? CardType.Pet : CardType.Spell)))
        {
            GameObject obj = Instantiate(deckDispPref, deck_parent);
            obj.GetComponent<DeckDisplay>().Set(item);
            i++;
        }

        i = 0;
        foreach (var item in PetInventory)
        {
            if (item.cardType == (isPetOpen ? CardType.Spell : CardType.Pet))
                continue;
            if (playerDeck.Contains(item))
                continue;
            GameObject obj = Instantiate(inventoryDispPref, inventory_parent);
            obj.GetComponent<SpellCardDisplay>().card = item;
            obj.transform.localPosition = Vector3.zero;
            obj.GetComponent<SpellCardDisplay>().cardPosition = SpellCardPosition.petHomePlayer;
            obj.GetComponent<SpellCardDisplay>().index = i;
            obj.transform.localScale = Vector3.one * 0.7f;
            obj.GetComponent<SpellCardDisplay>().canvas.sortingOrder = 7;
            obj.GetComponent<SpellCardDisplay>().IsPreview = true;
            i++;
        }

        count.text = isPetOpen ? playerDeck.Where(i => i.cardType == (CardType.Pet)).Count() + " / 33" : playerDeck.Where(i => i.cardType == (CardType.Spell)).Count() + "";
    }

    public SpellCard GetCard(int id)
    {
        foreach (var item in PetInventory)
        {
            if (item.cardId == id)
                return item;
        }
        return null;
    }

    public void confirm()
    {
        if (isPetOpen)
        {
            if (playerDeck.Where(item => item.cardType == CardType.Pet).Count() == 33)
            {
                SetDeck();
                PetPanel.Close();
            }
        }
        else
        {
            SetDeck();
            PetPanel.Close();
        }
    }


    public void ClearAll()
    {
        // for (int i = 0; i < playerDeck.Count; i++)
        // {
        //     if(isPetOpen){
        //         if(playerDeck[i].cardType == CardType.Pet)
        //             playerDeck.Remove(playerDeck[i]);
        //     }else{
        //         if(playerDeck[i].cardType == CardType.Spell)
        //             playerDeck.Remove(playerDeck[i]);
        //     }
        // }
        playerDeck.RemoveAll((i) => i.cardType == (isPetOpen ? CardType.Pet : CardType.Spell));
        UpdateDeckPreview(currentScreen);
    }
    public void AddAll()
    {
        //playerDeck.Clear();
        if (isPetOpen)
        {
            //int i = 0;
            playerDeck.AddRange(PetInventory.Where((i) => i.cardType == CardType.Pet && !playerDeck.Contains(i)));
            // while(playerDeck.Where(item=>item.cardType == CardType.Pet).Count() < 33)
            // {
            //     if(PetInventory[i].cardType == CardType.Pet){
            //         playerDeck.Add(PetInventory[i]);
            //     }
            //     i++;
            // }
        }
        else
        {
            for (int i = 0; i < PetInventory.Count; i++)
            {
                if (PetInventory[i].cardType == CardType.Spell)
                    playerDeck.Add(PetInventory[i]);
            }
        }

        UpdateDeckPreview(currentScreen);
    }

    public void SetDeck()
    {
        List<int> deckIds = new List<int>();
        foreach (var item in playerDeck)
        {
            deckIds.Add(item.cardId);
        }
        string deckStr = string.Join('_', deckIds);
        ExitGames.Client.Photon.Hashtable data = PhotonNetwork.LocalPlayer.CustomProperties;
        if (data.ContainsKey("PlayerDeck"))
            data["PlayerDeck"] = deckStr;
        else
            data.Add("PlayerDeck", deckStr);
        PhotonNetwork.LocalPlayer.CustomProperties = data;

        PlayerPrefs.SetString("player_deck", deckStr);

        //Debug.Log(" _Enkampfen_  /"+PhotonNetwork.LocalPlayer.CustomProperties["PlayerDeck"] +"/ und und und "+ deckStr);
        //Invoke("StarGame",0.3f);
    }

    public void Open(string s)
    {
        currentScreen = s;
        if (currentScreen == "Pet")
        {
            title.text = "Pets";
        }
        else
        {
            title.text = "Spells";
        }
        PetPanel.Open();
        UpdateDeckPreview(s);
    }

    void StarGame()
    {
        PhotonNetwork.LoadLevel("Game");
    }
}


