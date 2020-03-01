using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimatorHelper : MonoBehaviour
{
    [SerializeField] UIHandler ui;

    public void FinishedFadeIn()
    {
        ui.FinishedFadeIn();
    }

    public void FinishedFadeOut()
    {
        ui.FinishedFadeOut();
    }
}
