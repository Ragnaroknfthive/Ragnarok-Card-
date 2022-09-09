using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using TMPro;

public class DeckManager : MonoBehaviour
{

    public List<SpellCard> playerDeck;
    public List<SpellCard> Inventory;
    public static DeckManager instance;

    public Transform deck_parent;
    public GameObject deckDispPref;

    public Transform inventory_parent;
    public GameObject inventoryDispPref;
    public TextMeshProUGUI count;
    public UI_panel panel;

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
        string deck_str = PlayerPrefs.GetString("player_deck","");

        if(deck_str != ""){
            foreach (var item in deck_str.Split('_'))
            {
                playerDeck.Add(GetCard(System.Convert.ToInt32(item)));
            }
        }

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void AddCard(SpellCard card){
        playerDeck.Add(card);
        UpdateDeckPreview();
    }
    public void RemoveCard(SpellCard card){
        playerDeck.Remove(card);
        UpdateDeckPreview();
    }

    public void UpdateDeckPreview(){
        foreach (Transform item in inventory_parent)
        {
            Destroy(item.gameObject);
        }
        foreach (Transform item in deck_parent)
        {
            Destroy(item.gameObject);
        }

        

        int i = 0;
        foreach (var item in playerDeck)
        {
            GameObject obj = Instantiate(deckDispPref,deck_parent);
            obj.GetComponent<DeckDisplay>().Set(item);
            i++;
        }

        i = 5;
        foreach (var item in Inventory)
        {
            if(playerDeck.Contains(item))
                continue;
            GameObject obj = Instantiate(inventoryDispPref,inventory_parent);
            obj.GetComponent<SpellCardDisplay>().card = item;
            obj.transform.localPosition = Vector3.zero;
            obj.GetComponent<SpellCardDisplay>().cardPosition = SpellCardPosition.petHomePlayer;
            obj.GetComponent<SpellCardDisplay>().index = i;
            obj.transform.localScale = Vector3.one * 0.7f;
            obj.GetComponent<SpellCardDisplay>().canvas.sortingOrder = 5;
            obj.GetComponent<SpellCardDisplay>().IsPreview = true;
            i++;
        }

        count.text = playerDeck.Count+" / 33";
    }

    public SpellCard GetCard(int id){
        foreach (var item in Inventory)
        {
            if(item.cardId == id)
                return item;
        }
        return null;
    }

    public void confirm(){
        if(playerDeck.Count == 33){
            SetDeck();
            panel.Close();
        }
        

    }

    public void ClearAll(){
        playerDeck.Clear();
        UpdateDeckPreview();
    }
    public void AddAll(){
        playerDeck.Clear();
        for (int i = 0; i < 33; i++)
        {
            playerDeck.Add(Inventory[i]);
        }
        UpdateDeckPreview();
    }

    public void SetDeck(){
        List<int> deckIds = new List<int>();
        foreach (var item in playerDeck)
        {
            deckIds.Add(item.cardId);
        }
        string deckStr = string.Join('_',deckIds);
        ExitGames.Client.Photon.Hashtable data = PhotonNetwork.LocalPlayer.CustomProperties;
        if(data.ContainsKey("PlayerDeck"))
            data["PlayerDeck"] = deckStr;
        else
            data.Add("PlayerDeck",deckStr);
        PhotonNetwork.LocalPlayer.CustomProperties = data;

        PlayerPrefs.SetString("player_deck",deckStr);

        //Debug.Log(" _Enkampfen_  /"+PhotonNetwork.LocalPlayer.CustomProperties["PlayerDeck"] +"/ und und und "+ deckStr);
        //Invoke("StarGame",0.3f);
    }

    public void Open(){
        panel.Open();
        UpdateDeckPreview();
    }

    void StarGame()
    {
        PhotonNetwork.LoadLevel("Game");
    }
}

