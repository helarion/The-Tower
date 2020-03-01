﻿using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Card : MonoBehaviour
{
    #region variables
    [Header("Hovering")]
    [SerializeField] SpriteRenderer frontRenderer;
    [SerializeField] SpriteRenderer backRenderer;
    [SerializeField] SpriteRenderer hoverSpriteRenderer;

    [Header("Values")]
    public int damageValue;
    public int HP=-1;
    [SerializeField] int APWin=-1;

    [Header("State")]
    public GameManager.AtoutType atoutType;
    public GameManager.CardType cardType;
    public MonsterType monsterType;
    public bool isFlipped = false;
    public bool isFlippable = true;
    public bool isDraggable = true;
    public bool stillInDeck = true;
    public bool isInHand = false;
    private bool isCurrentlyDragged = false;
    private bool isAboutToHeal = false;
    Card targetAttackCard;
    //public Vector3 handPosition;

    [Header("UI")]
    [SerializeField] GameObject UIHolder;
    [SerializeField] TextMeshPro HPText;
    [SerializeField] TextMeshPro AttackText;

    [Header("Anim")]
    [SerializeField] Animator animator;
    public static readonly int hashMonsterAttack = Animator.StringToHash("MonsterAttack");
    public static readonly int hashPlayerAttack = Animator.StringToHash("PlayerAttack");
    public static readonly int hashDestroy = Animator.StringToHash("Destroy");
    [SerializeField] Explodable explodable;

    //Rotation
    [SerializeField] float rotationSpeed = 1f;
    [SerializeField] float returnSpeed = 1f;
    float targetRotation = 0;
    public bool returnToStartPos = false;

    public Vector3 startPosition;
    private Vector3 screenPoint;
    private Vector3 offset;
    private float timeSinceLastClick = 0;

    public enum MonsterType
    {
        Vallet,
        Dame,
        Roi,
        As
    }
    #endregion

    #region StartUpdate

    private void Awake()
    {
        HandlesStartFlipping();
    }

    void Start()
    {
        //explodable.fragmentInEditor();
        hoverSpriteRenderer.color = GameManager.Instance.basicHoverColor;
        if (cardType == GameManager.CardType.Monster)
        {
            isDraggable = false;
            SetMonsterValues(0);
        }
    }

    public void SetMonsterValues(int floorBonus)
    {
        if (monsterType == MonsterType.Vallet)
        {
            damageValue = GameManager.Instance.ValletDamageValue + floorBonus;
            HP = GameManager.Instance.ValletHP;
            APWin = GameManager.Instance.ValletAP;
        }
        else if (monsterType == MonsterType.Dame)
        {
            damageValue = GameManager.Instance.DameDamageValue + floorBonus;
            HP = GameManager.Instance.DameHP;
            APWin = GameManager.Instance.DameAP;
        }
        else if (monsterType == MonsterType.Roi)
        {
            damageValue = GameManager.Instance.RoiDamageValue + floorBonus;
            HP = GameManager.Instance.RoiHP;
            APWin = GameManager.Instance.RoiAP;
        }
        else if (monsterType == MonsterType.As)
        {
            damageValue = GameManager.Instance.AsDamageValue + floorBonus;
            HP = GameManager.Instance.AsHP;
            APWin = GameManager.Instance.AsAP;
        }
        UpdateUI();
    }

    // Update is called once per frame
    void Update()
    {
        if (transform.eulerAngles.y != targetRotation)
        {
            Vector3 newRot = transform.eulerAngles;
            newRot.y = Mathf.Lerp(newRot.y, targetRotation, Time.deltaTime * rotationSpeed);
            transform.eulerAngles = newRot;
        }

        if(returnToStartPos)
        {
            transform.position = Vector3.Lerp(transform.position, startPosition, Time.deltaTime * returnSpeed);
            if (transform.position == startPosition) returnToStartPos = false;
            if (isFlipped && isInHand) Flip();
        }
        else
        {
            if (explodable.fragments.Count>0 && explodable.fragments[0].transform.position.y <= -10)
            {
                foreach(GameObject g in explodable.fragments)
                {
                    Destroy(g);
                }
                Destroy(this.gameObject);
            }
        }
    }
    #endregion

    #region Hover
    private void OnTriggerStay2D(Collider2D collision)
    {
        if (!isCurrentlyDragged) return;
        Card cardCol = collision.gameObject.GetComponent<Card>();
        if (cardCol)
        {
            if(cardCol.atoutType != atoutType && (cardType==GameManager.CardType.Attack || cardType == GameManager.CardType.Heal) && cardCol.cardType==GameManager.CardType.Monster && cardCol.isFlipped==false)
            {
                cardCol.HoverAttack();
                HoverAttack();
                targetAttackCard = cardCol;
            }
        }
        else if(cardType==GameManager.CardType.Heal && collision.gameObject.tag == "Health")
        {
            isAboutToHeal = true;
            HoverHeal();
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        Card cardCol = collision.gameObject.GetComponent<Card>();
        if (cardCol)
        {
            cardCol.StopHover();
            if (cardCol == targetAttackCard) targetAttackCard = null;
        }
        else if (cardType == GameManager.CardType.Heal && collision.gameObject.tag == "Health")
        {
            isAboutToHeal = false;
        }
        StopHover();
    }

    private void OnMouseExit()
    {
        StopHover();
    }

    public void HoverBasic()
    {
        if (!hoverSpriteRenderer || !frontRenderer || !backRenderer) return;

        hoverSpriteRenderer.enabled = true;
        hoverSpriteRenderer .color = GameManager.Instance.basicHoverColor;
        frontRenderer.color = GameManager.Instance.basicHoverColor;
        backRenderer.color = GameManager.Instance.basicHoverColor;
    }

    public void HoverAttack()
    {
        if (!hoverSpriteRenderer || !frontRenderer || !backRenderer) return;

        hoverSpriteRenderer.enabled = true;
        hoverSpriteRenderer.color = GameManager.Instance.attackHoverColor;
        frontRenderer.color = GameManager.Instance.attackHoverColor;
        backRenderer.color = GameManager.Instance.attackHoverColor;
    }

    public void HoverHeal()
    {
        if (!hoverSpriteRenderer || !frontRenderer || !backRenderer) return;

        hoverSpriteRenderer.enabled = true;
        hoverSpriteRenderer.color = GameManager.Instance.healingHoverColor;
        frontRenderer.color = GameManager.Instance.healingHoverColor;
        backRenderer.color = GameManager.Instance.healingHoverColor;
    }

    public void StopHover()
    {
        if (!hoverSpriteRenderer || !frontRenderer || !backRenderer) return;

        hoverSpriteRenderer.enabled = false;
        frontRenderer.color = Color.white;
        backRenderer.color = Color.white;
    }
    #endregion

    private void OnMouseOver()
    {
        if(hoverSpriteRenderer.enabled==false) HoverBasic();
    }

    private void OnMouseDown()
    {
        if(Input.GetMouseButton(0))
        {
            float clickTime = Time.time - timeSinceLastClick;

            if (clickTime <= GameManager.Instance.doubleClickTimer)
            {
                //double click
                if (isInHand || !isFlippable || stillInDeck) return;
                if (cardType == GameManager.CardType.Monster)
                {
                    if (GameManager.Instance.UseAP())
                        Flip();
                }
                else
                    Flip();
            }
            else
            {
                if (isDraggable)
                {
                    if (!isInHand) startPosition = transform.position;
                    screenPoint = Camera.main.WorldToScreenPoint(transform.position);
                    offset = transform.position - Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, screenPoint.z));
                }
                else if (stillInDeck && cardType != GameManager.CardType.Monster)
                {
                    if (GameManager.Instance.UseAP())
                        GameManager.Instance.TryDraw(this);
                }
                else if (stillInDeck)
                {
                    GameManager.Instance.TryDraw(this);
                }
            }
            timeSinceLastClick = Time.time;
        }
    }

    private void OnMouseUp()
    {
        if(isDraggable)
        {
            returnToStartPos = true;
            isCurrentlyDragged = false;
            if(targetAttackCard)
            {
                animator.SetTrigger(hashPlayerAttack);
                returnToStartPos = false;
            }
            else if(isAboutToHeal)
            {
                if(GameManager.Instance.UseAP())
                {
                    GameManager.Instance.Heal(damageValue);
                    DestroyCard();
                }
                else
                {
                    print("feedback not engough point");
                }
            }
        }
        Vector3 newPos = transform.position;
        newPos.z = 0;
        transform.position = newPos;
    }

    private void OnMouseDrag()
    {
        // draging the card
        if (Input.GetMouseButton(0) && isDraggable)
        {
            isCurrentlyDragged = true;
            returnToStartPos = false;
            Vector3 curScreenPoint = new Vector3(Input.mousePosition.x, Input.mousePosition.y, screenPoint.z);
            Vector3 curPosition = Camera.main.ScreenToWorldPoint(curScreenPoint) + offset;
            curPosition.z = -1;
            curPosition = ClampVectorOnScreen(curPosition);
            transform.position = curPosition;
        }
    }

    private bool DoesDamageDouble(Card c1, Card c2)
    {
        if((c1.atoutType==GameManager.AtoutType.Carreau || c1.atoutType == GameManager.AtoutType.Coeur) && (c2.atoutType == GameManager.AtoutType.Pic || c2.atoutType == GameManager.AtoutType.Trefle))
        {
            return true;
        }
        else if((c1.atoutType == GameManager.AtoutType.Pic || c1.atoutType == GameManager.AtoutType.Trefle) && (c2.atoutType == GameManager.AtoutType.Carreau || c2.atoutType == GameManager.AtoutType.Coeur))
        {
            return true;
        }
        return false;
    }

    public void FinishedPlayerAttack()
    {
        targetAttackCard.DamageCard(this);
    }

    public void FinishedMonsterAttack()
    {
        GameManager.Instance.DamagePlayer(damageValue);
    }

    public int AnticipateDamage(Card monster)
    {
        if (atoutType == monster.atoutType) return 0;

        int damage = damageValue;
        if (DoesDamageDouble(this, monster))
        {
            damage *= 2;
        }
        return damage;
    }

    public void DamageCard(Card attacker)
    {
        int damage = attacker.damageValue;
        if(DoesDamageDouble(attacker,this))
        {
            damage *= 2;
        }
        HP -= damage;
        GameManager.Instance.DamageMonster();
        UpdateUI();
        if(HP<=0)
        {
            DestroyCard();
        }
        else
        {
            animator.SetTrigger(hashMonsterAttack);
        }
        attacker.DestroyCard();
    }

    public void DestroyCard()
    {
        backRenderer.enabled = false;
        explodable.explode();
        if (cardType == GameManager.CardType.Monster)
        {
            GameManager.Instance.GainAP(APWin);
            GameManager.Instance.UpdateRoomList(this);
        }
        else if (isInHand)
        {
            GameManager.Instance.RemoveHandCard(this);
        }
        Invoke("DisableCollider", 0.5f);
    }

    void DisableCollider()
    {
        foreach(GameObject c in explodable.fragments)
        {
            c.GetComponent<PolygonCollider2D>().enabled = false;
        }
    }

    public void Flip()
    {
        targetRotation += 180;
        if (targetRotation >= 360)
            targetRotation -= 360;
        isFlipped = !isFlipped;

        if (cardType == GameManager.CardType.Monster && !isFlipped)
        {
            isFlippable = false;
            UIHolder.SetActive(true);
            UpdateUI();
        }
    }

    private void UpdateUI()
    {
        HPText.text = HP.ToString();
        AttackText.text = damageValue.ToString();
    }

    private void HandlesStartFlipping()
    {
        Vector3 newRot = Vector3.zero;
        if (isFlipped)
        {
            newRot.y = 180;
            targetRotation = 180;
        }
        transform.eulerAngles = newRot;
    }

    private Vector3 ClampVectorOnScreen(Vector3 pos)
    {
        pos.x = Mathf.Clamp(pos.x, GameManager.Instance.minX, GameManager.Instance.maxX);
        pos.y = Mathf.Clamp(pos.y, GameManager.Instance.minY, GameManager.Instance.maxY);
        return pos;
    }
}