/* 
    ------------------- Code Monkey -------------------

    Thank you for downloading this package
    I hope you find it useful in your projects
    If you have any questions let me know
    Cheers!

               unitycodemonkey.com
    --------------------------------------------------
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DissolveEffect : MonoBehaviour {

    [SerializeField] SpriteRenderer spriteRenderer;
    [SerializeField] private Material materialAttack;
    [SerializeField] private Material materialMonster;
    [SerializeField] private Material materialHeal;
    [SerializeField] float dissolveSpeed=0.8f;

    private float dissolveAmount;
    private bool isDissolving;

    private void Update()
    {
        if (isDissolving) {
            dissolveAmount = Mathf.Clamp01(dissolveAmount + dissolveSpeed * Time.deltaTime);
            spriteRenderer.material.SetFloat("_DissolveAmount", dissolveAmount);
        }
        //else {
        //    dissolveAmount = Mathf.Clamp01(dissolveAmount - dissolveSpeed * Time.deltaTime);
        //    material.SetFloat("_DissolveAmount", dissolveAmount);
        //}
    }

    public void StartDissolve(GameManager.CardType type)//, Color dissolveColor) 
    {
        dissolveAmount = 0;

        switch (type)
        {
            case GameManager.CardType.Attack:
                materialAttack.SetFloat("_DissolveAmount", dissolveAmount);
                spriteRenderer.material = materialAttack;
                break;
            case GameManager.CardType.Heal:
                materialHeal.SetFloat("_DissolveAmount", dissolveAmount);
                spriteRenderer.material = materialHeal;
                break;
            case GameManager.CardType.Monster:
                materialMonster.SetFloat("_DissolveAmount", dissolveAmount);
                spriteRenderer.material = materialMonster;
                break;
        }
        isDissolving = true;
    }

}
