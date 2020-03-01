using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UIHandler : MonoBehaviour
{
    [SerializeField] Animator damagePlayerAnimator;
    [SerializeField] Animator healPlayerAnimator;
    [SerializeField] Animator damageMonsterAnimator;
    [SerializeField] Animator winPanel;
    [SerializeField] Animator lostPanel;
    [SerializeField] SpriteRenderer background;
    [SerializeField] GameObject mainPanel;
    [SerializeField] GameObject cards;
    [SerializeField] Animator darkFade;
    [SerializeField] TextMeshProUGUI warningText;

    private void Start()
    {
        darkFade.SetTrigger("FadeOut");
    }

    public void DamagePlayer()
    {
        damagePlayerAnimator.SetTrigger("FadeInFadeOut");
    }

    public void DamageMonster()
    {
        damageMonsterAnimator.SetTrigger("FadeInFadeOut");
    }

    public void HealPlayer()
    {
        healPlayerAnimator.SetTrigger("FadeInFadeOut");
    }

    public void FadeInWin()
    {
        winPanel.gameObject.SetActive(true);
        winPanel.SetTrigger("FadeIn");
    }

    public void FadeInLost()
    {
        lostPanel.gameObject.SetActive(true);
        lostPanel.SetTrigger("FadeIn");
    }

    public void FadeOutWin()
    {
        winPanel.SetTrigger("FadeOut");
    }

    public void FadeOutLost()
    {
        lostPanel.SetTrigger("FadeOut");
    }

    public void FinishedFadeIn()
    {
        cards.SetActive(false);
        mainPanel.gameObject.SetActive(false);
        background.gameObject.SetActive(false);
    }

    public void FinishedFadeOut()
    {
        GameManager.Instance.RestartGame();
    }

    public void UpdateWarningText(string newText)
    {
        warningText.text = newText;
    }
}
