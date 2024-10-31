///////////////////////////////////////////////
/// DeckManager.cs
/// 
/// This script is used to manage the player's deck.

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using TMPro;
using System.Linq;

public class DeckManager : MonoBehaviour
{

    public List<SpellCard> playerDeck;//The player's deck
    public List<SpellCard> PetInventory;//The player's pet inventory
    public static DeckManager instance;//The instance of the DeckManager
    public Transform deck_parent;//The deck parent
    public GameObject deckDispPref;//The deck display prefab
    public Transform inventory_parent;//The inventory parent
    public GameObject inventoryDispPref;//The inventory display prefab
    public TextMeshProUGUI count;//The count text
    public TextMeshProUGUI title;//The title text
    public UI_panel PetPanel;//The pet panel
    public string currentScreen;//The current screen
    public string GameUpdatesLink = "https://peakd.com/@ragnarok.game/posts";//The game updates link
    public bool isPetOpen//Is the pet open
    {
        get//Get the value
        {
            return currentScreen == "Pet";//Return the value
        }
    }

    void Awake()
    {
        instance = this;//Set the instance to this
    }

    void Start()
    {
        string deck_str = PlayerPrefs.GetString("player_deck", "");//Get the player deck string
        if (deck_str != "")//If the deck string is not empty
        {
            foreach (var item in deck_str.Split('_'))//Loop through the deck string
            {
                if (!playerDeck.Contains(GetCard(System.Convert.ToInt32(item)))) playerDeck.Add(GetCard(System.Convert.ToInt32(item)));//Add the card to the player deck
            }
        }
    }

    public void AddCard(SpellCard card)//Method to add a card to the deck
    {
        playerDeck.Add(card);//Add the card to the deck
        UpdateDeckPreview(currentScreen);//Update the deck preview
    }
    public void RemoveCard(SpellCard card)//Method to remove a card from the deck
    {
        playerDeck.Remove(card);//Remove the card from the deck
        UpdateDeckPreview(currentScreen);//Update the deck preview
    }

    public void UpdateDeckPreview(string s)
    {
        foreach (Transform item in inventory_parent)//Loop through the inventory parent
        {
            Destroy(item.gameObject);//Destroy the item
        }
        foreach (Transform item in deck_parent)//Loop through the deck parent
        {
            Destroy(item.gameObject);//Destroy the item
        }
        int i = 0;//Set the i value to 0
        foreach (var item in playerDeck.Where(i => i.cardType == (isPetOpen ? CardType.Pet : CardType.Spell)))//Loop through the player deck
        {
            GameObject obj = Instantiate(deckDispPref, deck_parent);//Instantiate the deck display prefab
            obj.GetComponent<DeckDisplay>().Set(item);//Set the card data
            i++;//Increment i
        }

        i = 0;//Set i to 0
        foreach (var item in PetInventory)//Loop through the pet inventory
        {
            if (item.cardType == (isPetOpen ? CardType.Spell : CardType.Pet))//If the card type is equal to the pet or spell
                continue;
            if (playerDeck.Contains(item))//If the player deck contains the item
                continue;
            GameObject obj = Instantiate(inventoryDispPref, inventory_parent);//Instantiate the inventory display prefab
            obj.GetComponent<SpellCardDisplay>().card = item;//Set the card data
            obj.transform.localPosition = Vector3.zero;//Set the local position to zero
            obj.GetComponent<SpellCardDisplay>().cardPosition = SpellCardPosition.petHomePlayer;//Set the card position
            obj.GetComponent<SpellCardDisplay>().index = i;//Set the index
            obj.transform.localScale = Vector3.one * 0.7f;//Set the local scale
            obj.GetComponent<SpellCardDisplay>().canvas.sortingOrder = 7;//Set the sorting order
            obj.GetComponent<SpellCardDisplay>().IsPreview = true;//Set the is preview value
            i++;//Increment i
        }
        count.text = isPetOpen ? playerDeck.Where(i => i.cardType == (CardType.Pet)).Count() + " / 33" : playerDeck.Where(i => i.cardType == (CardType.Spell)).Count() + "";//Set the count text
    }

    public SpellCard GetCard(int id)//Method to get the card by id
    {
        foreach (var item in PetInventory)//Loop through the pet inventory
        {
            if (item.cardId == id)//If the card id is equal to the id
                return item;//Return the item
        }
        return null;
    }

    public void confirm()//Method to confirm the deck
    {
        if (isPetOpen)//If the pet is open
        {
            if (playerDeck.Where(item => item.cardType == CardType.Pet).Count() > 0)//If the player deck contains the pet
            {
                SetDeck();//Set the deck
                PetPanel.Close();//Close the pet panel
            }
        }
        else
        {
            SetDeck();//Set the deck
            PetPanel.Close();//Close the pet panel
        }
    }


    public void ClearAll()
    {
        playerDeck.RemoveAll((i) => i.cardType == (isPetOpen ? CardType.Pet : CardType.Spell));//Remove all the cards
        UpdateDeckPreview(currentScreen);//Update the deck preview
    }
    public void AddAll()//Method to add all the cards
    {
        //playerDeck.Clear();
        if (isPetOpen)//If the pet is open
        {
            playerDeck.AddRange(PetInventory.Where((i) => i.cardType == CardType.Pet && !playerDeck.Contains(i)));//Add all the pets
        }
        else
        {
            for (int i = 0; i < PetInventory.Count; i++)//Loop through the pet inventory
            {
                if (PetInventory[i].cardType == CardType.Spell) playerDeck.Add(PetInventory[i]);//Add the spell to the player deck
            }
        }
        UpdateDeckPreview(currentScreen);//Update the deck preview
    }

    public void SetDeck()//Method to set the deck
    {
        List<int> deckIds = new List<int>();//List of deck ids
        foreach (var item in playerDeck)//Loop through the player deck
        {
            deckIds.Add(item.cardId);//Add the card id
        }
        string deckStr = string.Join('_', deckIds);//Join the deck ids
        ExitGames.Client.Photon.Hashtable data = PhotonNetwork.LocalPlayer.CustomProperties;//Get the custom properties
        if (data.ContainsKey("PlayerDeck")) data["PlayerDeck"] = deckStr;//If the data contains the player deck, set the player deck
        else data.Add("PlayerDeck", deckStr);//Add the player deck
        PhotonNetwork.LocalPlayer.CustomProperties = data;//Set the custom properties
        PlayerPrefs.SetString("player_deck", deckStr);//Set the player deck
    }



    public void SetOpponentDeck()//Method to set the opponent deck
    {
        for (int i = 0; i < PhotonNetwork.PlayerList.Length; i++)//Loop through the player list
        {
            if (!PhotonNetwork.PlayerList[i].IsMasterClient) //If the player is not the master client
            {
                List<int> deckIds = new List<int>();//List of deck ids
                foreach (var item in playerDeck)//Loop through the player deck
                {
                    deckIds.Add(item.cardId);//Add the card id
                }
                string deckStr = string.Join('_', deckIds);//Join the deck ids
                ExitGames.Client.Photon.Hashtable data = PhotonNetwork.LocalPlayer.CustomProperties;//Get the custom properties
                if (data.ContainsKey("PlayerDeck")) data["PlayerDeck"] = deckStr;//If the data contains the player deck, set the player deck
                else data.Add("PlayerDeck", deckStr);//Add the player deck
                PhotonNetwork.LocalPlayer.CustomProperties = data;//Set the custom properties
                PlayerPrefs.SetString("player_deck", deckStr);//Set the player deck
            }
        }
    }
    public const string DeckCounter = "DeckCounter";//Deck counter
    public int GetNewDeckCounter()//Method to get the new deck counter
    {
        int counterValue = 0;//Counter value
        if (PlayerPrefs.HasKey(DeckCounter)) counterValue = PlayerPrefs.GetInt(DeckCounter);//If the player prefs has the deck counter, get the deck counter
        counterValue += 1;//Increment the counter value
        PlayerPrefs.SetInt(DeckCounter, counterValue);//Set the deck counter
        return counterValue;//Return the counter value
    }
    public void Open(string s)//Method to open the pet or spell
    {
        currentScreen = s;//Set the current screen
        if (currentScreen == "Pet") title.text = "Pets";//If the current screen is pet, set the title to pets
        else title.text = "Spells";//Set the title to spells
        PetPanel.Open(true);//Open the pet panel
        UpdateDeckPreview(s);//Update the deck preview
    }

    void StarGame()//Method to start the game
    {
        PhotonNetwork.LoadLevel("Game");//Load the game scene
    }
    public void ShowUpdatesPage()//Method to show the updates page
    {
        Application.OpenURL(GameUpdatesLink);//Open the game updates link
    }
}
[System.Serializable]
public class DeckDetails//Deck details
{
    public int deckId;//Deck id
    public string deckName;//Deck name
    public string deckString;//Deck string
}

