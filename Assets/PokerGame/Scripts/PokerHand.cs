using System;
using System.Collections.Generic;
using System.Linq;

using System.Text;
using UnityEngine;

[System.Serializable]
public class PokerHand
{
    public bool RYL_FLUSH;
    public bool STRAIGHT_FLUSH;
    public bool FOUR_A_KIND;
    public bool FULL_HOUSE;
    public bool FLUSH;
    public bool STRAIGHT;
    public bool THREE_A_KIND;
    public bool TWO_PAIR;
    public bool PAIR;
    public bool HIGH_CARD;
    public List<Card> highCard;
    public int strength;

    //Temp Win
    public bool RYL_FLUSH_Temp;
    public bool STRAIGHT_FLUSH_Temp;
    public bool FOUR_A_KIND_Temp;
    public bool FULL_HOUSE_Temp;
    public bool FLUSH_Temp;
    public bool STRAIGHT_Temp;
    public bool THREE_A_KIND_Temp;
    public bool TWO_PAIR_Temp;
    public bool PAIR_Temp;
    public bool HIGH_CARD_Temp;
    //
    //Compare two pairs
    public CardValue pairCarValue, twoPairSet1CardValue = CardValue.none, twoPairSet2CardValue = CardValue.none, twoPairHigherCardValue = CardValue.none, straightHighCardValue = CardValue.none, flushHighCardValue = CardValue.none, royalFlushHighCardValue = CardValue.none;

    public PokerHand()
    {
        this.RYL_FLUSH = false;
        this.STRAIGHT_FLUSH = false;
        this.FOUR_A_KIND = false;
        this.FULL_HOUSE = false;
        this.FLUSH = false;
        this.STRAIGHT = false;
        this.THREE_A_KIND = false;
        this.TWO_PAIR = false;
        this.PAIR = false;
        this.HIGH_CARD = false;
        this.highCard = new List<Card>();
        this.strength = 0;
    }
    public int tempstrength = 0;
    public List<CardCombination> cardCombinations;
    public List<int> combinationStrengths = new List<int>();
    public int currentCombinationIndex = 0, bestIndex = 0;
    public Card match1 = null, match2 = null, match3 = null;
    public void setPokerHand(Card[] cardArray)
    {
        SaveCards(cardArray);
        cardCombinations = combinations;

        //Choose the best hand combination from board and player cards
        for (int i = 0; i < cardCombinations.Count; i++)
        {
            currentCombinationIndex = i;
            List<Card> cards = new List<Card>();
            //  Debug.LogError("======PLAYER CARDS=======");
            for (int itm = 0; itm < 5; itm++)
            {
                cards.Add(cardArray[combinations[i].items[itm]]);
                //  Debug.LogError("Card Name " + cardArray[combinations[i].items[itm]].name);
            }

            //foreach(Card c in cardArray)
            //{
            //    cards.Add(c);
            //    Debug.LogError("Card Name " + c.name);
            //}

            // Debug.LogError("======PLAYER CARDS=======");

            //SaveCards(cardArray);

            if (isRoyal(cards))
            {
            }
            else if (isStraightFlush(cards))
            { }// return;
            else if (isFourAKind(cards))
            { }// return;
            else if (isFullHouse(cards)) { }//return;
            else if (isFlush(cards)) { } //return;
            else if (isStraight(cards)) { } //return;
            else if (isThreeAKind(cards)) { } //return;
            else if (isTwoPair(cards)) { }// return;
            else if (isPair(cards)) { }// return;
            else isHigh(cards);
            //Debug.LogError("STRENGTH OF COMB -" + i + "  " + strength);
            combinationStrengths.Add(strength);

        }



        strength = tempstrength;
        ResetAllTempItem();
        switch (strength)
        {
            case 0:
                HIGH_CARD_Temp = true;
                break;

            case 1:
                PAIR_Temp = true;
                break;

            case 2:
                TWO_PAIR_Temp = true;
                break;

            case 3:
                THREE_A_KIND_Temp = true;
                break;

            case 4:
                STRAIGHT_Temp = true;
                break;
            case 5:
                FLUSH_Temp = true;
                break;

            case 6:
                FULL_HOUSE_Temp = true;
                break;

            case 7:
                FOUR_A_KIND_Temp = true;
                break;
            case 8:
                STRAIGHT_FLUSH_Temp = true;
                break;
            case 9:
                RYL_FLUSH_Temp = true;
                break;
            default:
                break;
        }
        // Debug.LogError("FULL HOUSE  " + FULL_HOUSE_Temp + " THREE " + THREE_A_KIND_Temp);
        //   Debug.LogError("TEMP STRENGTH " + tempstrength + " STRENGTH " + strength);
        if (TWO_PAIR_Temp)
        {
            //   Debug.LogError("TWO PAIR CONDITION");
            match1 = null; match2 = null; match3 = null;
            foreach (Card item in cardArray)
            {
                int count = 0;
                foreach (Card loppitem in cardArray)
                {
                    if (item.cardValue == loppitem.cardValue)
                    {
                        count++;
                    }
                }
                if (count >= 2)
                {
                    if (match1 == null)
                    { match1 = item; }
                    else if (match2 == null)
                    {
                        match2 = item;
                    }
                    else if (match3 == null)
                    {
                        match3 = item;
                    }
                }
            }

            List<Card> pairs = new List<Card>();
            if (match1 != null) pairs.Add(match1);
            if (match2 != null) pairs.Add(match2);
            if (match3 != null) pairs.Add(match3);
            pairs = pairs.OrderBy(x => x.cardValue).ToList();
            highCard.Clear();
            highCard.Add(pairs.Last());
            highCard.Add(pairs[0]);
            twoPairHigherCardValue = highCard.ElementAt(0).cardValue;
            Game.Get().MyHighCardValue = (int)twoPairHigherCardValue;
            Game.Get().MySecondHighCardValue = (int)highCard.ElementAt(1).cardValue;
            //
            List<Card> ascendingOrder = cardArray.ToList().OrderBy(x => x.cardValue).ToList();
            //highCard.Clear();
            // highCard.Add(ascendingOrder.Last());

            //
            List<int> removeList = new List<int>();
            for (int i = 0; i < ascendingOrder.Count; i++)
            {
                if (ascendingOrder[i].cardValue == match1.cardValue)
                {
                    removeList.Add(i);

                }
                if (ascendingOrder[i].cardValue == match2.cardValue)
                {
                    removeList.Add(i);

                }
            }
            if (removeList != null)
            {

                foreach (int item in removeList)
                {
                    ascendingOrder[item].cardValue = CardValue.none;
                }
            }
            removeList.Clear();
            //
            ascendingOrder.RemoveAll(x => x.cardValue == CardValue.none);
            ascendingOrder = ascendingOrder.ToList().OrderBy(x => x.cardValue).ToList();
            Game.Get().MyHighCardList.Clear();
            for (int i = ascendingOrder.Count - 1; i >= 0; i--)
            {
                highCard.Add(ascendingOrder[i]);
                Game.Get().MyHighCardList.Add((int)ascendingOrder[i].cardValue);
                Debug.Log("Value ADDED ");
            }
            //
            //  Game.Get().MySecondHighCardValue = (allCArds[4] != null) && allCArds[4]> ? (int)allCArds[4].cardValue : -1;
            Debug.Log("TWO PAIR HIGHER CARD VAL " + twoPairHigherCardValue);

        }
        else if (FULL_HOUSE_Temp)
        {
            //   Debug.LogError("FULL HOUSE CONDITION");
            match1 = null; match2 = null; match3 = null;

            int match1Count = 0, match2Count = 0;
            //  List<Card> descendingOrder
            cardArray = cardArray.ToList().OrderByDescending(x => x.cardValue).ToList().ToArray();
            //Decide Five Cards 

            int matchIndex = -1;
            for (int i = 0; i < 7; i++)
            {

                for (int j = 0; j < 7; j++)
                {
                    if (cardArray[i].cardValue == cardArray[j].cardValue)
                    {
                        match1Count++;
                        // match1 = cardArray[i];
                        matchIndex = i;
                    }
                }
                Debug.Log("M COUNTE" + match1Count);
                if (matchIndex != -1 && (match1Count == 2 || match1Count == 3))
                {

                    match1 = cardArray[matchIndex];

                }
                match1Count = 0;
            }

            matchIndex = -1;

            for (int i = 0; i < 7; i++)
            {
                // int matchIndex = -1;
                for (int j = 0; j < 7; j++)
                {
                    if (cardArray[i].cardValue == cardArray[j].cardValue && cardArray[i].cardValue != match1.cardValue)
                    {
                        match2Count++;
                        matchIndex = i;
                        // match2 = cardArray[i];
                    }
                }
                if (matchIndex != -1 && (match2Count == 2 || match2Count == 3) && cardArray[i].cardValue != match1.cardValue)
                {
                    match2 = cardArray[matchIndex];

                }
                match2Count = 0;
            }


            //     Debug.LogError("MATCH 1 " + match1.cardValue + "  Match 2 " + match2.cardValue);
            if (match2.cardValue > match1.cardValue)
            {
                Card temp = match1;
                match1 = match2;
                match2 = temp;

            }
            highCard.Clear();
            // Debug.LogError("SECONG CARD " + match2.cardValue);
            this.highCard.Add(match1);
            this.highCard.Add(match2);


            //



            Game.Get().MyHighCardValue = (int)highCard.ElementAt(0).cardValue;
            Game.Get().MySecondHighCardValue = (int)highCard.ElementAt(1).cardValue;


        }
        else if (PAIR_Temp)
        {
            //  Debug.LogError("PAIR CONDITION");
            match1 = null; match2 = null; match3 = null;
            foreach (Card item in cardArray)
            {
                int count = 0;
                foreach (Card loppitem in cardArray)
                {
                    if (item.cardValue == loppitem.cardValue)
                    {
                        count++;
                    }
                }
                if (count >= 2)
                {
                    if (match1 == null)
                    { match1 = item; }
                }
            }

            List<Card> pairs = new List<Card>();
            if (match1 != null) pairs.Add(match1);
            pairs = pairs.OrderBy(x => x.cardValue).ToList();
            highCard.Clear();
            highCard.Add(pairs.Last());

            List<Card> ascendingOrder = cardArray.ToList().OrderBy(x => x.cardValue).ToList();
            Game.Get().MyHighCardValue = (int)match1.cardValue;
            //highCard.Clear();
            // highCard.Add(ascendingOrder.Last());
            List<int> removeList = new List<int>();
            for (int i = 0; i < ascendingOrder.Count; i++)
            {
                if (ascendingOrder[i].cardValue == match1.cardValue)
                {
                    removeList.Add(i);

                }
            }


            if (removeList != null)
            {
                foreach (int item in removeList)
                {
                    //    Debug.LogError("STOPPED " + item);
                    ascendingOrder[item].cardValue = CardValue.none;
                }
            }
            removeList.Clear();

            ascendingOrder.RemoveAll(x => x.cardValue == CardValue.none);
            ascendingOrder = ascendingOrder.ToList().OrderBy(x => x.cardValue).ToList();
            Game.Get().MyHighCardList.Clear();
            for (int i = ascendingOrder.Count - 1; i >= 0; i--)
            {
                highCard.Add(ascendingOrder[i]);
                Game.Get().MyHighCardList.Add((int)ascendingOrder[i].cardValue);
                Debug.Log("Value ADDED ");
            }
            // Game.Get().MyHighCardValue = (int)match1.cardValue;// (int)highCard.ElementAt(0).cardValue;
            // Game.Get().MyHighCardValue =(int) highCard.ElementAt(0).cardValue;
            Game.Get().MySecondHighCardValue = Game.Get().MyHighCardList[0];
            //  List<Card> allCArds = new List<Card>();
            //  allCArds = cardArray.ToList().OrderByDescending(x => x.cardValue).ToList();
            //for(int i = 0 ; i < allCArds.Count ; i++)
            //{
            //    if((int)allCArds[i].cardValue > Game.Get().MyHighCardValue)
            //    {
            //        if(Game.Get().MySecondHighCardValue != -1) 
            //        {
            //            if(Game.Get().MySecondHighCardValue< (int)allCArds[i].cardValue) 
            //            {
            //                Game.Get().MySecondHighCardValue = (int)allCArds[i].cardValue;
            //            }

            //        }
            //        else
            //        {
            //            Game.Get().MySecondHighCardValue = (int)allCArds[i].cardValue;
            //        }
            //    }
            //}
            //Game.Get().MySecondHighCardValue =(allCArds[1] != null)? (int)allCArds[1].cardValue : -1;
            Debug.Log(" PAIR HIGHER CARD VAL " + Game.Get().MySecondHighCardValue);
            Debug.Log(" PAIR HIGHER CARD FIRST VAL " + Game.Get().MyHighCardValue);
        }
        else if (THREE_A_KIND_Temp)
        {
            // Debug.LogError("THREE  CONDITION");
            match1 = null; match2 = null; match3 = null;
            foreach (Card item in cardArray)
            {
                int count = 0;
                foreach (Card loppitem in cardArray)
                {
                    if (item.cardValue == loppitem.cardValue)
                    {
                        count++;
                    }
                }
                if (count >= 3)
                {
                    if (match1 == null)
                    { match1 = item; }
                    else if (match2 == null)
                    {
                        match2 = item;
                    }
                }
            }

            List<Card> pairs = new List<Card>();
            if (match1 != null) pairs.Add(match1);
            if (match2 != null) pairs.Add(match2);

            pairs = pairs.OrderBy(x => x.cardValue).ToList();
            highCard.Clear();
            highCard.Add(pairs.Last());

            Game.Get().MyHighCardValue = (int)highCard.ElementAt(0).cardValue;
            List<Card> ascendingOrder = cardArray.ToList().OrderBy(x => x.cardValue).ToList();
            //highCard.Clear();
            // highCard.Add(ascendingOrder.Last());
            List<int> removeList = new List<int>();
            for (int i = 0; i < ascendingOrder.Count; i++)
            {
                if (ascendingOrder[i].cardValue == match1.cardValue)
                {
                    removeList.Add(i);

                }
            }
            if (removeList != null)
            {
                foreach (int item in removeList)
                {
                    ascendingOrder[item].cardValue = CardValue.none;
                }
            }
            removeList.Clear();
            ascendingOrder.RemoveAll(x => x.cardValue == CardValue.none);
            ascendingOrder = ascendingOrder.ToList().OrderBy(x => x.cardValue).ToList();
            Game.Get().MyHighCardList.Clear();
            for (int i = ascendingOrder.Count - 1; i >= 0; i--)
            {
                highCard.Add(ascendingOrder[i]);
                Game.Get().MyHighCardList.Add((int)ascendingOrder[i].cardValue);
                Debug.Log("Value ADDED ");
            }
            Game.Get().MyHighCardValue = (int)highCard.ElementAt(0).cardValue;
            // Game.Get().MyHighCardValue =(int) highCard.ElementAt(0).cardValue;
            Game.Get().MySecondHighCardValue = Game.Get().MyHighCardList[0];
            Debug.Log(" PAIR HIGHER CARD VAL " + Game.Get().MySecondHighCardValue);

        }
        else if (FOUR_A_KIND_Temp)
        {
            //  Debug.LogError("FOUR A KIND CONDITION");
            match1 = null; match2 = null; match3 = null;
            foreach (Card item in cardArray)
            {
                int count = 0;
                foreach (Card loppitem in cardArray)
                {
                    if (item.cardValue == loppitem.cardValue)
                    {
                        count++;
                    }
                }
                if (count >= 3)
                {
                    if (match1 == null)
                    { match1 = item; }
                    else if (match2 == null)
                    {
                        match2 = item;
                    }
                }
            }

            List<Card> pairs = new List<Card>();
            if (match1 != null) pairs.Add(match1);
            if (match2 != null) pairs.Add(match2);

            pairs = pairs.OrderBy(x => x.cardValue).ToList();
            highCard.Clear();
            highCard.Add(pairs.Last());

            Game.Get().MyHighCardValue = (int)highCard.ElementAt(0).cardValue;
            Debug.Log(" PAIR HIGHER CARD VAL " + twoPairHigherCardValue);
            List<Card> ascendingOrder = cardArray.ToList().OrderBy(x => x.cardValue).ToList();
            //highCard.Clear();
            // highCard.Add(ascendingOrder.Last());
            List<int> removeList = new List<int>();
            for (int i = 0; i < ascendingOrder.Count; i++)
            {
                if (ascendingOrder[i].cardValue == match1.cardValue)
                {
                    removeList.Add(i);

                }
            }
            if (removeList != null)
            {
                foreach (int item in removeList)
                {
                    ascendingOrder[item].cardValue = CardValue.none;
                }
            }
            removeList.Clear();
            ascendingOrder.RemoveAll(x => x.cardValue == CardValue.none);
            ascendingOrder = ascendingOrder.ToList().OrderBy(x => x.cardValue).ToList();
            Game.Get().MyHighCardList.Clear();
            for (int i = ascendingOrder.Count - 1; i >= 0; i--)
            {
                highCard.Add(ascendingOrder[i]);
                Game.Get().MyHighCardList.Add((int)ascendingOrder[i].cardValue);
                Debug.Log("Value ADDED ");
            }
            Game.Get().MySecondHighCardValue = Game.Get().MyHighCardList[0];

        }
        else if (STRAIGHT_Temp)
        {
            // Debug.LogError("STRAIGHT CONDITION");

            // highCard.Clear();
            //highCard.Add((int)straightHighCardValue);
            Game.Get().MyHighCardValue = (int)straightHighCardValue; //(int)highCard.ElementAt(0).cardValue;
            Debug.Log("High Card Val" + Game.Get().MyHighCardValue);

        }
        else if (FLUSH_Temp)
        {
            //      Debug.LogError("FLUSH CONDITION");
            // highCard.Clear();
            //highCard.Add((int)straightHighCardValue);
            Game.Get().MyHighCardValue = (int)flushHighCardValue; //(int)highCard.ElementAt(0).cardValue;
            Debug.Log("High Card Val" + Game.Get().MyHighCardValue);

        }
        else if (RYL_FLUSH_Temp)
        {
            //  Debug.LogError("ROYAL FLUSH CONDITION");
            // highCard.Clear();
            //highCard.Add((int)straightHighCardValue);
            Game.Get().MyHighCardValue = (int)CardValue.ace; //(int)highCard.ElementAt(0).cardValue;
            Debug.Log("High Card Val" + Game.Get().MyHighCardValue);

        }
        else if (STRAIGHT_FLUSH_Temp)
        {
            //  Debug.LogError("STRAIGHT FLUSH CONDITION");
            // highCard.Clear();
            //highCard.Add((int)straightHighCardValue);
            Game.Get().MyHighCardValue = (int)flushHighCardValue;
            Debug.Log("High Card Val" + Game.Get().MyHighCardValue);

        }
        else if (HIGH_CARD_Temp)
        {
            //  Debug.LogError("HIGH CARD CONDITION");
            List<Card> ascendingOrder = cardArray.ToList().OrderBy(x => x.cardValue).ToList();
            highCard.Clear();
            // highCard.Add(ascendingOrder.Last());
            Game.Get().MyHighCardList.Clear();
            for (int i = ascendingOrder.Count - 1; i > 1; i--)
            {
                highCard.Add(ascendingOrder[i]);
                Game.Get().MyHighCardList.Add((int)ascendingOrder[i].cardValue);
                Debug.Log("Value ADDED ");
            }
            Game.Get().MyHighCardValue = (int)highCard.ElementAt(0).cardValue;
            Debug.Log("High Card Val");
            combinations[0].items = new int[5] { -1, -1, -1, -1, -1 };
            for (int i = 0; i < highCard.Count; i++)
            {
                for (int j = 0; j < cardArray.Length; j++)
                {
                    if (highCard[i] == cardArray[j])
                    {
                        combinations[0].items[i] = j;
                        //if(combinations[0].items.ToList().FindAll(x => x == -1).Count > 0)
                        //{
                        //    for(int k = 0 ; k < combinations[0].items.Count() ; k++)
                        //    {
                        //        if(combinations[0].items[k] == -1)
                        //        {
                        //            combinations[0].items[k] = i;
                        //        }
                        //    }
                        //}
                    }

                }
            }
        }
        //List<Card> cards = new List<Card>();
        //Debug.LogError("======PLAYER CARDS=======");
        //foreach (Card c in cardArray)
        //{
        //    cards.Add(c);
        //    Debug.LogError("Card Name " + c.name);
        //}

        //Debug.LogError("======PLAYER CARDS=======");

        //SaveCards(cardArray);

        //if (isRoyal(cards)) return;
        //if (isStraightFlush(cards)) return;
        //if (isFourAKind(cards)) return;
        //if (isFullHouse(cards)) return;
        //if (isFlush(cards)) return;
        //if (isStraight(cards)) return;
        //if (isThreeAKind(cards)) return;
        //if (isTwoPair(cards)) return;
        //if (isPair(cards)) return;
        //isHigh(cards);

    }

    public String printResult()
    {

        if (this.RYL_FLUSH)
        {
            return "Royal Flush" + "- Strength:" + this.strength;
        }
        else if (this.STRAIGHT_FLUSH)
        {
            return "Straight Flush" + "- Strength:" + this.strength;
        }
        else if (this.FOUR_A_KIND)
        {
            return "Four of a kind" + "- Strength:" + this.strength;
        }
        else if (this.FULL_HOUSE)
        {
            return "Full house" + "- Strength:" + this.strength;
        }
        else if (this.FLUSH)
        {
            return "Flush" + "- Strength:" + this.strength;
        }
        else if (this.STRAIGHT)
        {
            return "Straight" + "- Strength:" + this.strength;
        }
        else if (this.THREE_A_KIND)
        {
            return "Three of a kind" + "- Strength:" + this.strength;
        }
        else if (this.TWO_PAIR)
        {
            return "Two Pair" + "- Strength:" + this.strength;
        }
        else if (this.PAIR)
        {
            return "Pair" + "- Strength:" + this.strength;
        }
        else if (this.HIGH_CARD)
        {
            return "High card" + "- Strength:" + this.strength;
        }
        else
            return "error setting hand.";
    }
    public bool CheckHighCardConditionForRoyalFlush(Card max)
    {
        bool isAce = false;
        this.highCard.Clear();
        this.highCard.Add(max);
        if (max.cardValue == CardValue.ace)
        {
            isAce = true;
        }
        return isAce;

    }
    private bool isRoyal(List<Card> cards)
    {
        cards = cards.OrderBy(x => x.cardValue).ToList();
        this.highCard = new List<Card>();
        this.highCard.Add(cards.ElementAt(4));
        Debug.Log(this.highCard.ElementAt(0));

        if (isFlush(cards) && isStraight(cards) && CheckHighCardConditionForRoyalFlush(cards.ElementAt(4))) //this.highCard.ElementAt(0).cardValue == CardValue.ace)
        {
            ResetAllTempItem();
            RYL_FLUSH_Temp = true;
            this.RYL_FLUSH = true;
            this.strength = 9;
            if (strength > tempstrength)
            {
                tempstrength = strength;
                bestIndex = currentCombinationIndex;
            }

        }
        return this.RYL_FLUSH;
    }
    public void ResetAllTempItem()
    {
        RYL_FLUSH_Temp = false;
        STRAIGHT_FLUSH_Temp = false;
        FOUR_A_KIND_Temp = false;
        FULL_HOUSE_Temp = false;
        FLUSH_Temp = false;
        STRAIGHT_Temp = false;
        THREE_A_KIND_Temp = false;
        TWO_PAIR_Temp = false;
        PAIR_Temp = false;
        HIGH_CARD_Temp = false;

    }
    private bool isStraightFlush(List<Card> cards)
    {
        cards = cards.OrderBy(x => x.cardValue).ToList();

        this.highCard = new List<Card>();
        this.highCard.Add(cards.ElementAt(4));
        if (isFlush(cards) && isStraight(cards))
        {
            ResetAllTempItem();
            STRAIGHT_FLUSH_Temp = true;
            this.STRAIGHT_FLUSH = true;
            this.strength = 8;
            if (strength > tempstrength)
            {
                tempstrength = strength;
                bestIndex = currentCombinationIndex;
            }
            return true;
        }
        return false;
    }

    private bool isFourAKind(List<Card> cards)
    {
        cards = cards.OrderBy(x => x.cardValue).ToList();

        this.highCard = new List<Card>();
        int count;

        for (int i = 0; i < 5; i++)
        {
            count = 0;
            Card curentCard = cards.ElementAt(i);
            foreach (Card card in cards)
            {
                if (curentCard.cardValue == card.cardValue)
                    count++;

                if (count == 4)
                {
                    ResetAllTempItem();
                    FOUR_A_KIND_Temp = true;
                    this.FOUR_A_KIND = true;
                    this.highCard.Add(card);
                    this.strength = 7;
                    if (strength > tempstrength)
                    {
                        tempstrength = strength;
                        bestIndex = currentCombinationIndex;
                    }

                    // find the kicker card
                    foreach (Card c in cards)
                    {
                        if ((int)c.cardValue != (int)highCard.ElementAt(0).cardValue)
                            this.highCard.Add(c);
                    }
                    return true;
                }
            }
        }
        return false;
    }

    private bool isFullHouse(List<Card> cards)
    {
        cards = cards.OrderBy(x => x.cardValue).ToList();
        this.highCard = new List<Card>();
        int count;
        Card match1;
        Card match2 = null;

        for (int i = 0; i < 5; i++)
        {
            count = 0;
            Card curentCard = cards.ElementAt(i);
            foreach (Card card in cards)
            {
                if (curentCard.cardValue == card.cardValue)
                    count++;

                // found three of a kind
                if (count == 3)
                {
                    match1 = card;

                    // find the second match for the full house
                    foreach (Card c2 in cards)
                    {
                        if (match1.cardValue != c2.cardValue)
                        {
                            match2 = c2;
                        }
                    }

                    if (match2 != null)
                    {
                        count = 0;
                        foreach (Card c3 in cards)
                        {
                            if (match2.cardValue == c3.cardValue)
                            {
                                count++;
                                if (count == 2 || count == 3)
                                {
                                    this.highCard.Add(match1);
                                    this.highCard.Add(match2);
                                    ResetAllTempItem();
                                    FULL_HOUSE_Temp = true;
                                    this.FULL_HOUSE = true;
                                    this.strength = 6;
                                    Debug.Log("FULL HOUSE HIGH CARD ");
                                    Game.Get().MyHighCardValue = (int)highCard.ElementAt(0).cardValue;
                                    if (strength > tempstrength)
                                    {
                                        tempstrength = strength;
                                        bestIndex = currentCombinationIndex;
                                    }
                                    return true;
                                }
                            }
                        }
                    }
                }
            }
        }
        return false;
    }

    private bool isFlush(List<Card> cards)
    {
        cards = cards.OrderBy(x => x.cardValue).ToList();
        // check to see what suite the players card is and see if all cards
        // share that suite.
        this.highCard = new List<Card>();
        int playerSuite;
        Card card = cards.ElementAt(0);
        playerSuite = (int)card.cardColor;
        highCard.Add(cards.Last());
        foreach (Card c in cards)
        {
            if ((int)c.cardColor != playerSuite)
                return false;
        }

        //  Debug.LogError("Working UPTO HERE ");
        for (int i = 4; i >= 0; i--)
        {
            this.highCard.Add(cards.ElementAt(i));
        }
        if (flushHighCardValue == CardValue.none)
        {
            flushHighCardValue = cards.Last().cardValue;
        }
        else
        {
            if (flushHighCardValue < cards.Last().cardValue)
            {
                flushHighCardValue = cards.Last().cardValue;
            }
        }
        ResetAllTempItem();
        FLUSH_Temp = true;
        this.FLUSH = true;
        this.strength = 5;
        if (strength > tempstrength)
        {
            tempstrength = strength;
            bestIndex = currentCombinationIndex;
        }
        return true;
    }

    private bool isStraight(List<Card> cards)
    {
        // Debug.Log("LENGTH " + cards.Count);
        this.highCard = new List<Card>();
        cards = cards.OrderBy(x => x.cardValue).ToList();
        //string values="";
        //for(int i = 0 ; i < cards.Count ; i++)
        //{
        //    if(i == 0) 
        //    {
        //        values += "{";
        //        values += cards[i].cardValue+", ";
        //    }
        //    else if(i==cards.Count-1) 
        //    {
        //        values += cards[i].cardValue + " }";
        //    }
        //    else 
        //    {
        //        values += cards[i].cardValue + ", ";
        //    }

        //}
        //Debug.Log("VALUES " + values);
        Card card1 = cards.ElementAt(0);
        Card card2 = cards.ElementAt(1);
        Card card3 = cards.ElementAt(2);
        Card card4 = cards.ElementAt(3);
        Card card5 = cards.ElementAt(4);

        // make sure the cards rank is in order
        if (((int)card3.cardValue - (int)card4.cardValue == -1) && ((int)card2.cardValue - (int)card3.cardValue == -1) && ((int)card1.cardValue - (int)card2.cardValue == -1))
        {
            if ((card4.cardValue - card5.cardValue == -1))
            {
                ResetAllTempItem();
                STRAIGHT_Temp = true;
                this.STRAIGHT = true;
                this.strength = 4;
                if (strength > tempstrength)
                {
                    tempstrength = strength;
                    bestIndex = currentCombinationIndex;

                }
                for (int i = 4; i >= 0; i--)
                {
                    this.highCard.Add(cards.ElementAt(i));
                }
                if (straightHighCardValue == CardValue.none)
                {
                    straightHighCardValue = card5.cardValue;
                }
                else if (card5.cardValue > straightHighCardValue)
                {
                    straightHighCardValue = card5.cardValue;
                }
            }

            else if ((int)card1.cardValue == 0 && (int)card5.cardValue == 12 /* ACE */)
            {
                ResetAllTempItem();
                STRAIGHT_Temp = true;
                this.STRAIGHT = true;
                this.strength = 4;
                if (strength > tempstrength)
                {
                    tempstrength = strength;
                    bestIndex = currentCombinationIndex;

                }

                for (int i = 4; i >= 0; i--)
                {
                    this.highCard.Add(cards.ElementAt(i));
                }
            }
            else
                return this.STRAIGHT;
        }
        return this.STRAIGHT;
    }

    private bool isThreeAKind(List<Card> cards)
    {
        cards = cards.OrderBy(x => x.cardValue).ToList();

        this.highCard = new List<Card>();
        int count;

        for (int i = 0; i < 5; i++)
        {
            count = 0;
            Card curentCard = cards.ElementAt(i);
            foreach (Card card in cards)
            {
                if (curentCard.cardValue == card.cardValue)
                    count++;

                if (count == 3)
                {

                    this.highCard.Add(card);
                    ResetAllTempItem();
                    THREE_A_KIND_Temp = true;
                    this.THREE_A_KIND = true;
                    this.strength = 3;
                    if (strength > tempstrength)
                    {
                        tempstrength = strength;
                        bestIndex = currentCombinationIndex;
                    }

                    // find the two kickers
                    for (int j = 4; j >= 0; j--)
                    {
                        if (cards.ElementAt(j).cardValue != this.highCard.ElementAt(0).cardValue)
                            this.highCard.Add(cards.ElementAt(j));
                    }
                }
            }

        }
        return this.THREE_A_KIND;
    }

    private bool isTwoPair(List<Card> cards)
    {
        cards = cards.OrderBy(x => x.cardValue).ToList();

        this.highCard = new List<Card>();
        int count;
        Card match1 = null;

        for (int i = 0; i < 5; i++)
        {
            count = 0;
            Card curentCard = cards.ElementAt(i);

            foreach (Card card in cards)
            {
                if (curentCard.cardValue == card.cardValue)
                    count++;

                // found a pair
                if (count == 2)
                {
                    match1 = card;

                    twoPairSet1CardValue = match1.cardValue;
                    break;
                }
            }

            if (match1 != null) break;
        }

        // found one match
        if (match1 != null)
        {
            for (int i = 0; i < 5; i++)
            {
                count = 0;
                Card curentCard = cards.ElementAt(i);

                if (curentCard.cardValue != match1.cardValue)
                {
                    foreach (Card card in cards)
                    {
                        if (curentCard.cardValue == card.cardValue)
                        {
                            count++;
                            twoPairSet2CardValue = card.cardValue;
                        }

                        // found second pair
                        if (count == 2)
                        {
                            this.highCard.Add(card);
                            this.highCard.Add(match1);
                            ResetAllTempItem();
                            TWO_PAIR_Temp = true;
                            this.TWO_PAIR = true;
                            this.strength = 2;
                            if (strength > tempstrength)
                            {
                                tempstrength = strength;
                                bestIndex = currentCombinationIndex;
                            }
                            // get the kicker card
                            for (int j = 4; j >= 0; j--)
                            {
                                if (cards.ElementAt(j).cardValue != this.highCard.ElementAt(0).cardValue
                                        && cards.ElementAt(j).cardValue != this.highCard.ElementAt(1).cardValue)
                                    this.highCard.Add(cards.ElementAt(j));
                            }
                        }
                    }
                }
            }
        }
        if (TWO_PAIR)
        {
            if (twoPairSet1CardValue > twoPairSet2CardValue)
            {
                twoPairHigherCardValue = twoPairSet1CardValue;
            }
            else
            {
                twoPairHigherCardValue = twoPairSet2CardValue;
            }
            Game.Get().MyHighCardValue = (int)twoPairHigherCardValue;
            for (int i = 1; i < this.highCard.Count; i++)
            {
                // Debug.LogError("*** HIGH CARD " + (int)this.highCard.ElementAt(i).cardValue);
            }
            highCard.OrderByDescending(x => x.cardValue);
            // Debug.LogError("Hight card Count " + highCard.Count);
            Game.Get().MyHighCardValue = highCard.Count <= 0 ? 0 : (int)this.highCard.ElementAt(0).cardValue;
        }
        return this.TWO_PAIR;
    }

    private bool isPair(List<Card> cards)
    {
        cards = cards.OrderBy(x => x.cardValue).ToList();
        this.highCard = new List<Card>();
        int count;

        Card curentCardF = cards.ElementAt(0);
        int maxCardVal = (int)curentCardF.cardValue;
        int pairCard = 0;
        for (int i = 1; i < 5; i++)
        {
            Card curentCardT = cards.ElementAt(i);
            if ((int)curentCardT.cardValue >= maxCardVal)
            {
                if ((int)curentCardT.cardValue == maxCardVal)
                {
                    pairCard = maxCardVal;
                }
                maxCardVal = (int)curentCardT.cardValue;
            }
        }



        for (int i = 0; i < 5; i++)
        {
            count = 0;
            Card curentCard = cards.ElementAt(i);
            foreach (Card card in cards)
            {
                //Debug.LogError("*** CARD " + Game.Get().MyHighCardValue);
                if (curentCard.cardValue == card.cardValue)
                {
                    pairCarValue = curentCard.cardValue;

                    Game.Get().MyHighCardValue = (int)pairCarValue;

                    count++;
                }

                if (count == 2)
                {
                    this.PAIR = true;
                    this.highCard.Add(card);
                    ResetAllTempItem();
                    PAIR_Temp = true;
                    this.strength = 1;
                    if (strength > tempstrength)
                    {
                        tempstrength = strength;
                        bestIndex = currentCombinationIndex;
                    }

                    for (int j = 4; j >= 0; j--)
                    {
                        if (this.highCard.ElementAt(0).cardValue != cards.ElementAt(j).cardValue)
                            this.highCard.Add(cards.ElementAt(j));
                    }
                }
            }

        }
        //highCard= highCard.OrderByDescending(x => x.cardValue);
        //   Debug.LogError("High Card Count " + highCard.Count);
        Game.Get().MyHighCardValue = highCard.Count <= 0 ? 0 : (int)this.highCard.ElementAt(0).cardValue;

        return this.PAIR;
    }

    private void isHigh(List<Card> cards)
    {
        cards = cards.OrderBy(x => x.cardValue).ToList();

        this.highCard = new List<Card>();
        for (int j = 4; j >= 0; j--)
        {
            this.highCard.Add(cards.ElementAt(j));
        }
        int combinationHighCardValue = (int)highCard.Find(x => x.cardValue == highCard.Max(y => y.cardValue)).cardValue;
        if (combinationHighCardValue > Game.Get().MyHighCardValue)
        { Game.Get().MyHighCardValue = combinationHighCardValue; }
        ResetAllTempItem();
        HIGH_CARD_Temp = true;
        this.HIGH_CARD = true;
        strength = 0;
        if (strength > tempstrength)
        {
            tempstrength = strength;
            bestIndex = currentCombinationIndex;
        }
    }
    public void SaveCards(Card[] cards)
    {
        combinations.Clear();
        GFG.Main(new int[] { 0, 1, 2, 3, 4, 5, 6 }, 5);

        //Debug.LogError("CARD LENGTH " + cards.Length);
        //for (int i = 0; i < cards.Length; i++)
        //{
        //    Debug.Log("Value   " + cards[i].cardValue + "---Color " + cards[i].cardColor);
        //}

        //this.highCard = new List<Card>(cards.Length);
        //Card[] _cardsArray = cards.ToArray();
        //cards.CopyTo(_cardsArray, 0);
        //this.highCard = _cardsArray.ToList();
    }

    public static List<CardCombination> combinations = new List<CardCombination>();
}
[Serializable]
public class CardCombination
{
    public int[] items = new int[5];
}
public class GFG
{

    /* arr[] ---> Input Array
    data[] ---> Temporary array to
                store current combination
    start & end ---> Starting and Ending
                     indexes in arr[]
    index ---> Current index in data[]
    r ---> Size of a combination
           to be printed */
    static void combinationUtil(int[] arr, int n,
                                int r, int index,
                                int[] data, int i)
    {
        // Current combination is ready
        // to be printed, print it
        if (index == r)
        {
            int[] comb = new int[] { -1, -1, -1, -1, -1 };
            CardCombination combination = new CardCombination();
            for (int j = 0; j < r; j++)
            {

                comb[j] = data[j];
                combination.items[j] = data[j];
                //s+=" ,"+data[j].ToString();
                //Debug.Log(data[j] + " "); 
            }

            // Debug.Log("C "+ s);
            PokerHand.combinations.Add(combination);
            return;
        }

        // When no more elements are
        // there to put in data[]
        if (i >= n)
            return;

        // current is included, put
        // next at next location
        data[index] = arr[i];
        combinationUtil(arr, n, r,
                        index + 1, data, i + 1);

        // current is excluded, replace
        // it with next (Note that
        // i+1 is passed, but index
        // is not changed)
        combinationUtil(arr, n, r, index,
                        data, i + 1);
    }

    // The main function that prints
    // all combinations of size r
    // in arr[] of size n. This
    // function mainly uses combinationUtil()
    static void printCombination(int[] arr,
                                int n, int r)
    {
        // A temporary array to store
        // all combination one by one
        int[] data = new int[r];

        // Print all combination
        // using temporary array 'data[]'
        combinationUtil(arr, n, r, 0, data, 0);
    }

    // Driver Code
    static public void Main(int[] arr, int r)
    {
        // int[] arr = { 1,2,3,4,5 };
        // int r = 3;
        int n = arr.Length;
        printCombination(arr, n, r);
        Debug.Log("C COUNT " + PokerHand.combinations.Count);

    }
}