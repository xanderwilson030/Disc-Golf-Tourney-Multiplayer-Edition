using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

/*
 *  This script controls the hole rating that displays after a player inputs a disc into the basket
 */


public class HoleRating : MonoBehaviour
{
    [Header("References")]
    public TMP_Text holeRatingTextUIObject;

    [Header("Fade Details")]
    public int fadeLength;

    private void Start()
    {
        GameEvents.instance.e_TriggerHoleRatingText.AddListener(CalculateHoleRating);
    }

    /*
     *  This method obtains the current par and then calculates the appropriate rating
     *  following standard golf hole ratings
     */
    private void CalculateHoleRating(int score)
    {
        string holeRatingText = string.Empty;
        int currentPar = CourseController.instance.GetCurrentPar();

        // Declaring all of our hole rating values
        int albatross = currentPar - 3;
        int eagle = currentPar - 2;
        int birdie = currentPar - 1;
        int par = currentPar;
        int bogey = currentPar + 1;
        int doubleBogey = currentPar + 2;
        int tripleBogey = currentPar + 3;

        if (score == albatross)
        {
            holeRatingText = "Albatross\n3 Under Par";
        }
        else if (score == eagle)
        {
            holeRatingText = "Eagle\n2 Under Par";
        }
        else if (score == birdie)
        {
            holeRatingText = "Birdie\n1 Under Par";
        }
        else if (score == par)
        {
            holeRatingText = "Par";
        }
        else if (score == bogey)
        {
            holeRatingText = "Bogey\n1 Over Par";
        }
        else if (score == doubleBogey)
        {
            holeRatingText = "Double Bogey\n2 Over Par";
        }
        else if (score ==  tripleBogey)
        {
            holeRatingText = "Triple Bogey\n3 Over Par";
        }
        else
        {
            holeRatingText = $"{score} Shots\n{score} Over Par";
        }

        holeRatingTextUIObject.text = holeRatingText;
        holeRatingTextUIObject.gameObject.SetActive(true);

        StartCoroutine(HoleRatingTextFade());
    }

    private IEnumerator HoleRatingTextFade()
    {
        int counter = fadeLength;

        while (counter > 0)
        {
            yield return new WaitForSeconds(1);
            counter--;
        }

        holeRatingTextUIObject.gameObject.SetActive(false);
    }
}
