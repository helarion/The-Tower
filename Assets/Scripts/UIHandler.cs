using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
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
    [SerializeField] RectTransform tutorialCanvas;
    [SerializeField] GameObject InformationPanel;
    [SerializeField] GameObject TutoBackground;
    [SerializeField] TextMeshProUGUI informationTitleText;
    [SerializeField] TextMeshProUGUI informationContentText;
    [SerializeField] RectTransform pointTutoPanel;
    [SerializeField] TextMeshProUGUI pointTutoText;
    [SerializeField] RectTransform bullePointTuto;
    [SerializeField] Button pointNextButton;
    [SerializeField] Button infoNextButton;
    [SerializeField] Vector2 pointOffset;
    [SerializeField] GameObject enTutoStepParent;
    [SerializeField] GameObject frTutoStepParent;
    private List<TutoStep> tutoStepList;
    [SerializeField] Camera cam;

    int stage;
    int stepIndex = 0;
    bool needsCanvasUpdate = false;

    bool restart = false;

    private void Awake()
    {
        darkFade.gameObject.SetActive(true);
    }

    private void Start()
    {
        darkFade.SetTrigger("FadeStart");
        stageAnimator.SetTrigger("FadeFloor");
        infoNextButton.onClick.AddListener(NextStepTutorial);
        pointNextButton.onClick.AddListener(NextStepTutorial);
        tutoStepList = new List<TutoStep>();
        TutoStep[] temp;
        if(MusicHandler.Instance.language==1) temp = frTutoStepParent.GetComponentsInChildren<TutoStep>();
        else temp = enTutoStepParent.GetComponentsInChildren<TutoStep>();

        foreach (TutoStep t in temp)
        {
            tutoStepList.Add(t);
        }
        if (MusicHandler.Instance.playTutorial) Invoke("StartTutorial", 3f);
        else tutorialCanvas.gameObject.SetActive(false);
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


    public void RestartWin()
    {
        restart = true;
        FadeOutWin();
    }

    public void RestartLost()
    {
        restart = true;
        FadeOutLost();
    }

    public void QuitWin()
    {
        restart = false;
        FadeOutWin();
    }

    public void QuitLost()
    {
        restart = false;
        FadeOutLost();
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
        if (restart) GameManager.Instance.RestartGame();
        else SceneManager.LoadScene(0);
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

    public void StartTutorial()
    {
        tutorialCanvas.gameObject.SetActive(true);
        tutoStepList[0].Activate();
        GameManager.Instance.blockAttack = true;
        GameManager.Instance.blockAttackDraw = true;
        GameManager.Instance.blockPotionDraw = true;
        GameManager.Instance.blockExplore= true;
        GameManager.Instance.blockPotionUse= true;
        //StartCoroutine(TutorialSteps());
        //NextStepTutorial();
    }

    public void NextStepTutorial()
    {
        if (tutoStepList[stepIndex].waitForScript && tutoStepList[stepIndex].needsLaunch)
        {
            tutoStepList[stepIndex].waitForScript = false;
            tutoStepList[stepIndex].Activate();
            return;
        }
        stepIndex++;
        if (stepIndex < tutoStepList.Count)
        {
            needsCanvasUpdate = true;
            tutoStepList[stepIndex].Activate();
        }
        else
        {
            tutorialCanvas.gameObject.SetActive(false);
            GameManager.Instance.blockAttack = false;
            GameManager.Instance.blockAttackDraw = false;
            GameManager.Instance.blockPotionDraw = false;
            GameManager.Instance.blockExplore = false;
            GameManager.Instance.blockPotionUse = false;
            MusicHandler.Instance.playTutorial = false;
            PlayerPrefs.SetInt("PlayedTutorial", 1);
        }
    }

    public void InformationCanvas(string title, string text)
    {
        TutoBackground.gameObject.SetActive(true);
        pointTutoPanel.gameObject.SetActive(false);
        InformationPanel.SetActive(true);
        informationTitleText.text = title;
        informationContentText.text = text;
    }

    public void IsPointCanvasOutsideOfScreen()
    {
        int x = 0;
        int y = 0;
        float bx= -(pointTutoPanel.sizeDelta.x/2);
        float by= -(pointTutoPanel.sizeDelta.y/2);
        //print("anchor:" + pointTutoPanel.transform.position);
        if (pointTutoPanel.transform.position.x + pointTutoPanel.sizeDelta.x > 1920)
        {
            //print("Le cadre est trop à droite");
            x = 1;
            bx = (pointTutoPanel.sizeDelta.x/2);
        }
        else if (pointTutoPanel.transform.position.x - pointTutoPanel.sizeDelta.x < 0)
        {
            //print("Le cadre est trop à gauche");
            x = 0;
            //bx = -((pointTutoPanel.sizeDelta.x/2)) - 25;
        }
        if (pointTutoPanel.transform.position.y + pointTutoPanel.sizeDelta.y > 1080)
        {
            //print("Le cadre est trop en haut");
            y = 1;
            by = (pointTutoPanel.sizeDelta.y/2);
        }
        else if (pointTutoPanel.transform.position.y + pointTutoPanel.sizeDelta.y < 0)
        {
            //print("Le cadre est trop en bas");
            y = 0;
           //by = -((pointTutoPanel.sizeDelta.y/2));
        }
        bullePointTuto.anchoredPosition = new Vector2(bx,by);
        pointTutoPanel.pivot = new Vector2(x, y);
    }

    public void PointCanvasFromScene(string text, Transform pos)
    {
        TutoBackground.gameObject.SetActive(false);
        pointTutoPanel.gameObject.SetActive(true);
        InformationPanel.SetActive(false);
        pointTutoText.text = text;

        Vector2 screenPoint = RectTransformUtility.WorldToScreenPoint(Camera.main, pos.position);
        screenPoint += pointOffset;
        Vector2 anchor = screenPoint - tutorialCanvas.sizeDelta / 2f;
        pointTutoPanel.anchoredPosition = anchor;
    }

    public void PointCanvasFromUI(string text, Transform pos)
    {
        TutoBackground.gameObject.SetActive(false);
        pointTutoPanel.gameObject.SetActive(true);
        InformationPanel.SetActive(false);
        pointTutoText.text = text;

        Vector2 screenPoint = pos.position;
        screenPoint += pointOffset;
        pointTutoPanel.anchoredPosition = screenPoint - tutorialCanvas.sizeDelta / 2f;
    }

    public void PointNext(bool next)
    {
        pointNextButton.gameObject.SetActive(next);
    }

    public void HideTuto()
    {
        tutorialCanvas.gameObject.SetActive(false);
    }

    public void ShowTuto()
    {
        tutorialCanvas.gameObject.SetActive(true);
    }

    //IEnumerator TutorialSteps()
    //{
    //    while(stepIndex<tutoStepList.Count)
    //    {

    //    }
    //}
}
