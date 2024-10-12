using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using System.Linq;

public class SpellManager : MonoBehaviourPunCallbacks // This class manages spell-related actions in the game.
{
    // Transforms to hold player and opponent spell cards.
    public Transform spellCardsPlayer, spellCardsOpponent, spellCardBattleObj, opponentSpellBattleObject;

    // Prefabs for creating spell cards.
    public GameObject spellCardPrefab, spellBettleCardPrefeb;

    // Temporary storage for instantiated spell cards.
    private GameObject tempSpellCardObj;

    // Deck of spell cards and displays for player and opponent.
    public List<SpellCard> spellCardsDeck;
    public List<SpellCardDisplay> opponentCards = new List<SpellCardDisplay>();
    public List<SpellCardDisplay> playerCards = new List<SpellCardDisplay>();
    public List<BattleCardDisplay> opponentBattleCards = new List<BattleCardDisplay>();
    public List<BattleCardDisplay> playerBattleCards = new List<BattleCardDisplay>();

    // Transform references for the showcase of cards.
    public Transform showCaseParent, showCaseRef;

    // Singleton instance of SpellManager for easy access.
    public static SpellManager instance;

    // Photon view component for handling networked actions.
    public PhotonView photonView;

    // Static variables to manage pet attack states.
    public static bool IsPetAttacking;
    public static bool PetAlreadyAttacked;

    // UI Button reference for pet attack.
    public Button PetAttk;

    // Player's current mana.
    public int Mana = 0;

    // References to main decks for player and opponent.
    public GameObject MyMainDeck, OppoMainDeck;

    // List to track spawned card IDs.
    public List<int> spawned_ids = new List<int>();

    // Prefabs for card destruction effects.
    public GameObject DestroyCardEff, DestroyCardEffOppo;

    // Static variable to indicate if a pet attack has started.
    public static bool petAttackStarted;

    // Awake method initializes the singleton instance.
    private void Awake()
    {
        // If no instance exists, set the current instance.
        if (instance == null)
        {
            instance = this; // Sets this instance as the singleton.
        }
    }

    // Start method gets the PhotonView component.
    void Start()
    {
        photonView = GetComponent<PhotonView>(); // Initializes the photon view for networking.
    }

    // Method to reset spell data.
    public void ResetData()
    {
        // Clear the list of spawned IDs.
        spawned_ids = new List<int>();

        // Destroy all spell cards currently held by the player.
        foreach (Transform item in spellCardsPlayer)
        {
            DestroyImmediate(item.gameObject); // Destroys the spell card immediately.
        }

        // Calls the RPC to reset data on other clients.
        photonView.RPC("ResetDataRPC", RpcTarget.Others);
    }

    // RPC method to reset data on other clients.
    [PunRPC]
    public void ResetDataRPC()
    {
        // Clear the list of spawned IDs.
        spawned_ids = new List<int>();

        // Destroy all spell cards currently held by the player.
        foreach (Transform item in spellCardsPlayer)
        {
            DestroyImmediate(item.gameObject); // Destroys the spell card immediately.
        }

        // Starts spawning pets on other clients.
        StartCoroutine(PVPManager.manager.SpawnPets());
    }

    // Coroutine to handle the pet attack sequence.
    public IEnumerator StartPetAttack()
    {
        // Check if the game is in an all-in state and yield if true.
        if (PVPManager.manager.isAllIn || PVPManager.manager.IsAnyAllIn)
        {
            yield return null; // Exits the coroutine if the game is in an all-in state.
        }
        else
        {
            // Set the pet attack status to started.
            SpellManager.petAttackStarted = true;

            // Sort player and opponent battle cards by speed.
            playerBattleCards = playerBattleCards.OrderBy((item) => item.card.speed).ToList();
            opponentBattleCards = opponentBattleCards.OrderBy((item) => item.card.speed).ToList();

            // If there are opponent battle cards available, initiate the attack.
            if (opponentBattleCards.Count > 0)
            {
                int id = 0; // Initialize ID for tracking opponent cards.
                int max_len = Mathf.Max(playerBattleCards.Count, opponentBattleCards.Count); // Get the maximum length for iterations.

                // Iterate through player's battle cards.
                foreach (var item in playerBattleCards)
                {
                    // Skip cards that have already attacked this round.
                    if (item.IsAttackedThisRound) continue;

                    // Break if the game is over.
                    if (PVPManager.manager.PVPOver) break;

                    BattleCardDisplay oppo = null; // Initialize opponent card display.

                    // Find a non-dead opponent card.
                    while (id < opponentBattleCards.Count)
                    {
                        oppo = opponentBattleCards[id];
                        if (oppo != null)
                        {
                            // Check if the opponent card is not dead.
                            if (!oppo.IsDead)
                                break; // Exit loop if a valid opponent card is found.
                            else
                                id++; // Move to the next opponent card.
                        }
                        else
                        {
                            id++; // Move to the next opponent card.
                        }
                    }

                    // If a valid opponent card is found, initiate attack.
                    if (oppo != null)
                    {
                        item.Attack(oppo.card.cardId); // Perform attack on the opponent's card.
                        yield return new WaitForSeconds(0.5f); // Wait for half a second before the next action.
                        yield return new WaitWhile(() => IsPetAttacking); // Wait while the pet is attacking.
                    }
                    else
                    {
                        // If no opponent cards are available, perform a default attack.
                        item.Attack(-1, true);
                        yield return new WaitForSeconds(0.5f);
                        yield return new WaitWhile(() => IsPetAttacking); // Wait while the pet is attacking.
                    }

                    // Wait for a moment after each attack.
                    yield return new WaitForSeconds(0.5f);
                    yield return new WaitWhile(() => IsPetAttacking); // Wait while the pet is attacking.
                    yield return new WaitWhile(() => PVPManager.manager.isCheckWithoutReset); // Wait if a check is ongoing.
                }
            }
            // If there are no opponent battle cards, attack remaining player cards.
            else if (playerBattleCards.Count > 0)
            {
                // Iterate through player battle cards in reverse.
                for (int i = playerBattleCards.Count - 1; i >= 0; i--)
                {
                    BattleCardDisplay item = null;
                    // Break if the game is over.
                    if (PVPManager.manager.PVPOver) break;
                    if (i < playerBattleCards.Count)
                        item = playerBattleCards[i];

                    // If the player card exists, perform a default attack.
                    if (item != null)
                    {
                        item.Attack(-1, true); // Perform a default attack.
                        yield return new WaitForSeconds(0.5f);
                        yield return new WaitWhile(() => IsPetAttacking); // Wait while the pet is attacking.
                    }
                    yield return new WaitWhile(() => PVPManager.manager.isCheckWithoutReset); // Wait if a check is ongoing.
                }
            }
            SpellManager.petAttackStarted = false; // Reset the attack status after finishing.
        }
    }

    // Method to initiate pet attack.
    public void PetAttack()
    {
        // Check if it's the pet's turn and if it hasn't already attacked.
        if (PVPManager.Get().IsPetTurn && !PetAlreadyAttacked)
        {
            StartCoroutine(StartPetAttack()); // Start the pet attack coroutine.
        }
    }
    /// <summary>
    /// Executes an attack action by sending an RPC to other players.
    /// </summary>
    /// <param name="i">The identifier of the card being attacked.</param>
    /// <param name="IsPlayer">A boolean indicating if the attack is directed at the player.</param>
    /// <param name="cardId">The unique identifier of the spell card being used.</param>
    /// <param name="attacker">The unique identifier of the attacking character.</param>
    public void ExecuteAttack(int i, bool IsPlayer, int cardId, int attacker)
    {
        Debug.LogError("Spell Card Id" + cardId);
        // Notify other players to execute the attack
        photonView.RPC("ExecuteAttackRPC", RpcTarget.Others, i, IsPlayer, cardId, attacker);
    }

    /// <summary>
    /// RPC method that handles the execution of the attack for remote players. 
    /// It spawns a projectile representing the attack and applies the card's damage.
    /// </summary>
    /// <param name="i">The identifier of the card being attacked.</param>
    /// <param name="isplayer">Indicates if the attack is directed at the player.</param>
    /// <param name="cardId">The unique identifier of the spell card being used.</param>
    /// <param name="attacker">The unique identifier of the attacking character.</param>
    [PunRPC]
    public void ExecuteAttackRPC(int i, bool isplayer, int cardId, int attacker)
    {
        Debug.LogError("Opponent spell card " + cardId);
        SpellCard card = GameData.Get().GetPet(cardId); // Retrieve the spell card data
        SpellManager.IsPetAttacking = true; // Set the attacking state
                                            // Instantiate the spell projectile at the opponent's card position
        GameObject o = Instantiate(card.SpellProjectilePref, opponentBattleCards.Find(x => x.card.cardId == cardId).gameObject.transform.position, Quaternion.identity);
        Projectile proj = o.GetComponent<Projectile>(); // Get the projectile component
                                                        // Set the projectile's target based on whether it's aimed at the player or opponent
        proj.target = isplayer ? (PVPManager.Get().p1Image.gameObject) : playerBattleCards.Find(x => x.card.cardId == i).gameObject;
        proj.damage = card.Attack; // Set the projectile's damage
        proj.istargetPlayer = isplayer; // Set the target type
        proj.DealDamage = !isplayer; // Set whether the projectile should deal damage to the opponent
        proj.lifetime = 2f; // Set the projectile's lifetime
        PVPManager.manager.isCheckWithoutReset = true; // Prepare for checking win conditions
        StartCoroutine(PVPManager.manager.CheckWinNewWithoutReset(0.1f)); // Start coroutine to check for win
    }

    /// <summary>
    /// Requests to destroy a specific card by notifying other players through RPC.
    /// </summary>
    /// <param name="i">The unique identifier of the card to be destroyed.</param>
    public void DestroyOb(int i)
    {
        photonView.RPC("OnDestroyRPC", RpcTarget.Others, i); // Notify others to destroy the card
    }

    /// <summary>
    /// RPC method that handles the destruction of a card for remote players. 
    /// It removes the specified card from the opponent's battle cards.
    /// </summary>
    /// <param name="cardId">The unique identifier of the card to be destroyed.</param>
    [PunRPC]
    public void OnDestroyRPC(int cardId)
    {
        // Find and remove the specified card from opponent's battle cards
        BattleCardDisplay battleCard = opponentBattleCards.Find(x => x.card.cardId == cardId);
        SpellManager.instance.opponentBattleCards.Remove(battleCard); // Remove from the list
    }

    /// <summary>
    /// The offset value for card positioning, used for layout adjustments.
    /// </summary>
    public int offset_card = 50;

    /// <summary>
    /// Instantiates a spell card object at a specified position and parent.
    /// </summary>
    /// <param name="sO">The spell card object to instantiate.</param>
    /// <param name="pos">The position to instantiate the card at.</param>
    /// <param name="parent">The parent transform for the instantiated card.</param>
    /// <returns>The instantiated GameObject representing the spell card.</returns>
    public GameObject InstantiateSpellCard(SpellCard sO, Vector3 pos, Transform parent)
    {
        tempSpellCardObj = Instantiate(spellCardPrefab, pos, Quaternion.identity, parent); // Instantiate the card prefab
        tempSpellCardObj.GetComponent<SpellCardDisplay>().card = sO; // Assign the spell card data to the display component
        return tempSpellCardObj; // Return the instantiated object
    }

    /// <summary>
    /// Instantiates a battle card object at a specified position and parent.
    /// </summary>
    /// <param name="sO">The spell card object to instantiate as a battle card.</param>
    /// <param name="pos">The position to instantiate the battle card at.</param>
    /// <param name="parent">The parent transform for the instantiated battle card.</param>
    /// <param name="num_card">An identifier for the number of cards.</param>
    /// <returns>The instantiated GameObject representing the battle card.</returns>
    public GameObject InstantiateSpellBattleCard(SpellCard sO, Vector3 pos, Transform parent, int num_card)
    {
        tempSpellCardObj = Instantiate(spellBettleCardPrefeb, pos, Quaternion.identity, parent); // Instantiate the battle card prefab
        tempSpellCardObj.GetComponent<BattleCardDisplay>().card = sO; // Assign the spell card data to the display component
        tempSpellCardObj.transform.localScale = Vector3.one; // Set the scale to one
        return tempSpellCardObj; // Return the instantiated object
    }

    /// <summary>
    /// Removes old spell data by destroying existing cards in various collections.
    /// </summary>
    public void RemoveOldSpellData()
    {
        // Destroy player battle cards
        for (int i = 0; i < playerBattleCards.Count; i++)
        {
            if (playerBattleCards[i])
                Destroy(playerBattleCards[i].gameObject); // Destroy the card object
        }
        // Destroy player cards
        for (int i = 0; i < playerCards.Count; i++)
        {
            if (playerCards[i])
                Destroy(playerCards[i].gameObject); // Destroy the card object
        }
        // Destroy opponent cards
        for (int i = 0; i < opponentCards.Count; i++)
        {
            if (opponentCards[i])
                Destroy(opponentCards[i].gameObject); // Destroy the card object
        }
        // Destroy opponent battle cards
        for (int i = 0; i < opponentBattleCards.Count; i++)
        {
            if (opponentBattleCards[i])
                Destroy(opponentBattleCards[i].gameObject); // Destroy the card object
        }
        // Clear all lists of cards
        playerBattleCards.Clear();
        playerCards.Clear();
        opponentBattleCards.Clear();
        opponentCards.Clear();
    }

    /// <summary>
    /// Draws a card from the deck, ensuring the player can only have a limited number of cards.
    /// </summary>
    public void DrawCard()
    {
        // Check if all cards have been drawn
        if (spawned_ids.Count == spellCardsDeck.Count)
        {
            MyMainDeck.SetActive(false); // Disable the main deck if all cards are drawn
            photonView.RPC("SetDeckIm", RpcTarget.Others, false); // Notify others to update deck visibility
            return;
        }
        else
        {
            MyMainDeck.SetActive(true); // Enable the main deck if cards can still be drawn
            photonView.RPC("SetDeckIm", RpcTarget.Others, true); // Notify others to update deck visibility
        }

        // Randomly select a card index that hasn't been spawned yet
        int i = Random.Range(0, spellCardsDeck.Count);
        while (spawned_ids.Contains(i))
        {
            i = Random.Range(0, spellCardsDeck.Count); // Find a unique card index
        }
        spawned_ids.Add(i); // Track the drawn card index

        // If the player already has 9 cards, destroy one
        if (spellCardsPlayer.childCount == 9)
            DestroyCard(i); // Destroy an existing card
        else
            GenerateCardsForPlayer(i); // Generate a new card for the player
    }

    /// <summary>
    /// RPC method to update the visibility of the opponent's main deck.
    /// </summary>
    /// <param name="b">Boolean value indicating whether to show or hide the deck.</param>
    [PunRPC]
    public void SetDeckIm(bool b)
    {
        OppoMainDeck.SetActive(b); // Set the visibility of the opponent's main deck
    }

    /// <summary>
    /// Destroys a specified card, instantiating its representation in the player's UI.
    /// </summary>
    /// <param name="i">The index of the card to be destroyed.</param>
    public void DestroyCard(int i)
    {
        // Instantiate a spell card representation at the player's main deck
        GameObject obj = InstantiateSpellCard(spellCardsDeck[i], Vector3.one, MyMainDeck.transform);
        obj.transform.localPosition = Vector3.zero; // Set position to zero
                                                    // Set up the card display component with appropriate properties
        obj.GetComponent<SpellCardDisplay>().cardPosition = SpellCardPosition.petHomePlayer;
        obj.GetComponent<SpellCardDisplay>().index = i;
        obj.GetComponent<SpellCardDisplay>().set(true);
        obj.transform.localScale = Vector3.one * 0.7f; // Scale the card down
        obj.GetComponent<SpellCardDisplay>().canvas.sortingOrder = spawned_ids.Count + 1; // Set the sorting order for UI layering

        // Move the card to the player's hand
        LeanTween.move(obj, spellCardsPlayer.position, 0.3f).setOnComplete(() =>
        {
            PVPManager.manager.myObj.cards.Remove(spellCardsDeck[i]); // Remove the card from the player's data
                                                                      // Instantiate destruction effects based on the master client status
            if (PhotonNetwork.IsMasterClient)
                Instantiate(DestroyCardEff, spellCardsPlayer.position, Quaternion.identity);
            else
                Instantiate(DestroyCardEffOppo, spellCardsPlayer.position, Quaternion.identity);
            Destroy(obj.gameObject, 0.1f); // Destroy the card object after a delay
        });
        // Notify others to destroy the card representation
        photonView.RPC("DestroyCardRPC", RpcTarget.Others, i);
    }

    /// <summary>
    /// RPC method to handle the destruction of a card representation for remote players.
    /// </summary>
    /// <param name="i">The index of the card to be destroyed.</param>
    [PunRPC]
    public void DestroyCardRPC(int i)
    {
        // Instantiate a spell card representation in the opponent's UI
        GameObject obj = InstantiateSpellCard(GameData.Get().GetPet(i), Vector3.one, OppoMainDeck.transform);
        obj.transform.localPosition = Vector3.zero; // Set position to zero
                                                    // Set up the card display component with appropriate properties
        obj.GetComponent<SpellCardDisplay>().cardPosition = SpellCardPosition.petHomeOppoent;
        obj.GetComponent<SpellCardDisplay>().index = i;
        obj.GetComponent<SpellCardDisplay>().set(false);
        obj.GetComponent<SpellCardDisplay>().canvas.sortingOrder = spawned_ids.Count + 1; // Set the sorting order
        obj.GetComponent<SpellCardDisplay>().BackSide.gameObject.SetActive(true); // Show the back side of the card
        obj.transform.localScale = Vector3.one * 0.7f; // Scale the card down

        // Move the card to the opponent's hand
        LeanTween.move(obj, spellCardsOpponent.position, 0.3f).setOnComplete(() =>
        {
            // Instantiate destruction effects based on the master client status
            if (PhotonNetwork.IsMasterClient)
                Instantiate(DestroyCardEff, spellCardsOpponent.position, Quaternion.identity);
            else
                Instantiate(DestroyCardEffOppo, spellCardsOpponent.position, Quaternion.identity);
            Destroy(obj.gameObject, 0.1f); // Destroy the card object after a delay
        });
    }
    // Generates and displays a card for the player in their play area.
    public void GenerateCardsForPlayer(int i)
    {
        // Instantiate a spell card from the deck at the given index.
        GameObject obj = InstantiateSpellCard(spellCardsDeck[i], Vector3.one, MyMainDeck.transform);
        obj.transform.localPosition = Vector3.zero; // Set the initial position of the card.

        // Set the card's display properties.
        obj.GetComponent<SpellCardDisplay>().cardPosition = SpellCardPosition.petHomePlayer; // Define the card's position.
        obj.GetComponent<SpellCardDisplay>().index = i; // Assign the index of the card.
        obj.GetComponent<SpellCardDisplay>().set(true); // Mark the card as active.
        obj.transform.localScale = Vector3.one * 0.7f; // Scale the card down to 70% of its original size.
        obj.GetComponent<SpellCardDisplay>().canvas.sortingOrder = spawned_ids.Count + 1; // Set the canvas sorting order for the card.

        // Rotate the card if the current player is not the master client.
        if (PhotonNetwork.IsMasterClient == false)
            LeanTween.rotate(obj, new Vector3(0, 0, -180), 0); // Rotate the card for display.

        // Add the card to the player's list of cards.
        playerCards.Add(obj.GetComponent<SpellCardDisplay>());

        // Animate the card movement to the player's card position.
        LeanTween.move(obj, spellCardsPlayer.position, 0.3f).setOnComplete(() =>
        {
            // Set the parent of the card after the movement is complete.
            obj.transform.SetParent(spellCardsPlayer);
            obj.transform.position =
            // Call the RPC to generate the corresponding card for the opponent.
            photonView.RPC("GenerateCardsForOppnent", RpcTarget.Others, spellCardsDeck[i].cardId);
            PhotonNetwork.SendAllOutgoingCommands(); // Send all outgoing commands to ensure synchronization.
        });
    }

    // Cast a spell using the card at the specified index.
    public IEnumerator CastSpell(int i)
    {
        SpellCard card = spellCardsDeck[i]; // Retrieve the card from the deck.
        StartCoroutine(PVPManager.Get().DistributeSpellAttack(card.Attack)); // Distribute the spell's attack.
        PVPManager.Get().canContinue = false; // Prevent further actions until this is resolved.
        yield return new WaitUntil(() => PVPManager.Get().canContinue); // Wait until the action can continue.
    }

    // Cast a spell using the provided SpellCard object.
    public IEnumerator CastSpell(SpellCard card)
    {
        StartCoroutine(PVPManager.Get().DistributeSpellAttack(card.Attack)); // Distribute the spell's attack.
        PVPManager.Get().canContinue = false; // Prevent further actions until this is resolved.
        yield return new WaitUntil(() => PVPManager.Get().canContinue); // Wait until the action can continue.
    }

    // RPC method to cast a spell using the player's special attack.
    [PunRPC]
    public void CastSpellWithCard()
    {
        SpellCard card = PVPManager.manager.p2Char.SpecialAttack; // Get the special attack card for player 2.
        GameObject o = Instantiate(card.SpellProjectilePref, PVPManager.Get().p2Image.gameObject.transform.position, Quaternion.identity); // Instantiate the spell projectile.
        Projectile proj = o.GetComponent<Projectile>(); // Get the Projectile component.

        // Set the target and properties of the projectile.
        proj.target = PVPManager.Get().p1Image.gameObject; // Set the target as player 1's image.
        proj.damage = card.Attack; // Set the damage of the projectile.
        proj.istargetPlayer = true; // Indicate that the projectile targets a player.
        proj.DealDamage = false; // Set whether the projectile deals damage.
        proj.lifetime = 2f; // Set the lifetime of the projectile.
    }

    // RPC method to cast a spell using the card at the specified index.
    [PunRPC]
    public void CastSpellRPC(int i)
    {
        SpellCard card = GameData.Get().GetPet(i); // Get the card from the game data.
        GameObject o = Instantiate(card.SpellProjectilePref, PVPManager.Get().p2Image.gameObject.transform.position, Quaternion.identity); // Instantiate the spell projectile.
        Projectile proj = o.GetComponent<Projectile>(); // Get the Projectile component.

        // Set the target and properties of the projectile.
        proj.target = PVPManager.Get().p1Image.gameObject; // Set the target as player 1's image.
        proj.damage = card.Attack; // Set the damage of the projectile.
        proj.istargetPlayer = true; // Indicate that the projectile targets a player.
        proj.DealDamage = false; // Set whether the projectile deals damage.
        proj.lifetime = 2f; // Set the lifetime of the projectile.

        // Destroy the corresponding card from the opponent's list of cards.
        Destroy(opponentCards.Find((item) => item.index == i).gameObject);
    }

    // Move the opponent's card to the battle area.
    public void MoveOpponentCardToBattleArea(int cardId, int battleId)
    {
        // Call the RPC to change the opponent's card position in the center.
        photonView.RPC("ChangeOpponentCardPostionToCenter_RPC", RpcTarget.Others, cardId, battleId);
        PhotonNetwork.SendAllOutgoingCommands(); // Send all outgoing commands to ensure synchronization.
    }

    // RPC method to change the position of the opponent's card to the center of the battle area.
    [Photon.Pun.PunRPC]
    public void ChangeOpponentCardPostionToCenter_RPC(int cardId, int battleId)
    {
        // Find the opponent's card and change its position to the center based on the battle ID.
        SpellManager.instance.opponentCards.Find(x => x.card.cardId == cardId).ChangeOpponentCardPostionToCenter(battleId);
    }

    // Generate and display a card for the opponent.
    [PunRPC]
    public void GenerateCardsForOppnent(int i)
    {
        // Instantiate a spell card from the opponent's deck at the given index.
        GameObject obj = InstantiateSpellCard(GameData.Get().GetPet(i), Vector3.one, OppoMainDeck.transform);
        obj.transform.localPosition = Vector3.zero; // Set the initial position of the card.

        // Set the card's display properties for the opponent.
        obj.GetComponent<SpellCardDisplay>().cardPosition = SpellCardPosition.petHomeOppoent; // Define the card's position.
        obj.GetComponent<SpellCardDisplay>().index = i; // Assign the index of the card.
        obj.GetComponent<SpellCardDisplay>().set(false); // Mark the card as inactive.
        obj.GetComponent<SpellCardDisplay>().canvas.sortingOrder = spawned_ids.Count + 1; // Set the canvas sorting order for the card.

        // Rotate the card for display depending on the master client status.
        if (PhotonNetwork.IsMasterClient)
        {
            LeanTween.rotate(obj, new Vector3(0, 0, 180), 0); // Rotate the card for the master client.
        }
        else
        {
            LeanTween.rotate(obj, new Vector3(0, 0, 180), 0); // Rotate the card for non-master clients.
        }

        obj.GetComponent<SpellCardDisplay>().BackSide.gameObject.SetActive(true); // Show the backside of the card.
        obj.transform.localScale = Vector3.one * 0.7f; // Scale the card down to 70% of its original size.

        // Add the card to the opponent's list of cards.
        opponentCards.Add(obj.GetComponent<SpellCardDisplay>());

        // Animate the card movement to the opponent's card position.
        LeanTween.move(obj, spellCardsOpponent.position, 0.3f).setOnComplete(() =>
        {
            // Set the parent of the card after the movement is complete.
            obj.transform.SetParent(spellCardsOpponent);
            obj.transform.position = Vector3.zero; // Reset the position to zero after setting the parent.
        });
    }

    // RPC method to handle mouse enter events for opponent cards.
    [Photon.Pun.PunRPC]
    public void MouseEnter_RPC(int cardId, bool isEnter)
    {
        // If the mouse enters the card area, showcase the card.
        if (isEnter)
        {
            SpellManager.instance.opponentCards.Find(x => x.card.cardId == cardId).ShowCaseCard();
        }
        else
        {
            // If the mouse exits the card area, resize the showcased card.
            SpellManager.instance.opponentCards.Find(x => x.card.cardId == cardId).ResizeShowCaseCard();
        }
    }

    // Boolean to track if a card is being showcased.
    public bool isShowCasing = false;
}