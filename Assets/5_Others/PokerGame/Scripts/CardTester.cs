using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardTester : MonoBehaviour
{
    public List<Card> listofcards;


    // Start is called before the first frame update
    void Start()
    {

        //List<Card> listofcards = new List<Card>();

        //Card testcard = new Card();

        //testcard.cardValue = Value.two;

        //listofcards.Add(testcard);


        //testcard.cardValue = Value.three;

        //listofcards.Add(testcard);


        int res = listofcards[0].CompareTo(listofcards[1]);


        if (res == 1)
        {
            Debug.Log(listofcards[0].cardValue.ToString() +  " es mayor que " + listofcards[1].cardValue.ToString());
        }
        else if (res == -1)
        {
            Debug.Log(listofcards[1].cardValue.ToString() + " es mayor que " + listofcards[0].cardValue.ToString());
        }
        else
        {
            Debug.Log(listofcards[1].cardValue.ToString() + " es mayor que " + listofcards[0].cardValue.ToString());
        }


    }

}