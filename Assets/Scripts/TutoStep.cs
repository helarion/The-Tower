using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutoStep : MonoBehaviour
{
    public enum StepType
    {
        InfoTextNext,
        PointTextNext,
        PointTextWait,
    }

    public StepType stepType;
    public string title;
    [TextArea]
    public string text;
    public int id;
    public bool waitForScript;
    public bool needsLaunch=false;
    [SerializeField] bool isTransformUI = false;
    Transform targetPos;

    public void Activate()
    {
        Time.timeScale = 1;
        switch (stepType)
        {
            case StepType.InfoTextNext:
                GameManager.Instance.uiHandler.InformationCanvas(title, text);
                break;
            case StepType.PointTextNext:
                targetPos=GameManager.Instance.FindPosition(id);
                if(!isTransformUI)
                    GameManager.Instance.uiHandler.PointCanvasFromScene(text,targetPos);
                else
                    GameManager.Instance.uiHandler.PointCanvasFromUI(text, targetPos);
                GameManager.Instance.uiHandler.PointNext(true);
                GameManager.Instance.uiHandler.IsPointCanvasOutsideOfScreen();
                break;
            case StepType.PointTextWait:
                targetPos = GameManager.Instance.FindPosition(id);
                GameManager.Instance.FindScript(id);
                if (!isTransformUI)
                    GameManager.Instance.uiHandler.PointCanvasFromScene(text, targetPos);
                else
                    GameManager.Instance.uiHandler.PointCanvasFromUI(text, targetPos);
                GameManager.Instance.uiHandler.PointNext(false);
                GameManager.Instance.uiHandler.IsPointCanvasOutsideOfScreen();
                break;
        }
        if (waitForScript)
        {
            GameManager.Instance.uiHandler.HideTuto();
            GameManager.Instance.FindScript(id);
            needsLaunch = true;
        }
        else if (needsLaunch)
        {
            GameManager.Instance.uiHandler.ShowTuto();
        }
    }
}
