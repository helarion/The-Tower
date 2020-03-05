using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIHandler : MonoBehaviour
{
    [SerializeField] Animator damagePlayerAnimator;
    [SerializeField] Animator healPlayerAnimator;
    [SerializeField] Animator damageMonsterAnimator;
    [SerializeField] Animator winPanel;
    [SerializeField] Animator lostPanel;
    [SerializeField] SpriteRenderer background;
    [SerializeField] GameObject[] toDisableWhenFade;
    [SerializeField] Animator darkFade;
    [SerializeField] TextMeshProUGUI warningText;
    [SerializeField] TextMeshProUGUI HPBonusText;
    [SerializeField] TextMeshProUGUI APBonusText;
    [SerializeField] TextMeshProUGUI DamageMalusText;
    [SerializeField] Animator HPBonusAnimator;
    [SerializeField] Animator APBonusAnimator;
    [SerializeField] Animator DamageMalusAnimator;
    [SerializeField] TextMeshProUGUI stageText;
    [SerializeField] Animator stageAnimator;
    [SerializeField] Animator slashAnimator;
    [SerializeField] TextMeshPro floorText;

    [SerializeField] Sprite[] listBackground;

    int stage;

    private void Awake()
    {
        darkFade.gameObject.SetActive(true);
    }

    private void Start()
    {
        darkFade.SetTrigger("FadeStart");
        stageAnimator.SetTrigger("FadeFloor");
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
        foreach(GameObject g in toDisableWhenFade)
        {
            g.SetActive(false);
        }
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

    public void APBonus(string value)
    {
        APBonusText.text = value;
        APBonusAnimator.SetTrigger("FadeInFadeOut");
    }

    public void HealthBonus(string value)
    {
        HPBonusText.text = value;
        HPBonusAnimator.SetTrigger("FadeInFadeOut");
    }

    public void DamageMalus(string value)
    {
        DamageMalusText.text = value;
        DamageMalusAnimator.SetTrigger("FadeInFadeOut");
        slashAnimator.SetTrigger("FadeInFadeOut");
    }

    public void NewStage(int stage)
    {
        darkFade.SetTrigger("FadeFloor");
        stageText.text = "Etage " + stage;
        stageAnimator.SetTrigger("FadeFloor");
        this.stage = stage - 1;
        Invoke("SwitchBackground",0.5f);
    }

    void SwitchBackground()
    {
        floorText.text = "Floor " + (stage+1);
        background.sprite = listBackground[stage];
    }
}
