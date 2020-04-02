using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using System.Linq;

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
    //private bool isAboutToHeal = false;
    Card targetAttackCard;
    //public Vector3 handPosition;

    [Header("UI")]
    [SerializeField] GameObject UIHolder;
    public TextMeshPro HPText;
    public TextMeshPro AttackText;

    [Header("Anim")]
    [SerializeField] Animator animator;
    public static readonly int hashMonsterAttack = Animator.StringToHash("MonsterAttack");
    public static readonly int hashPlayerAttack = Animator.StringToHash("PlayerAttack");
    public static readonly int hashDestroy = Animator.StringToHash("Destroy");

    //Rotation
    [SerializeField] float rotationSpeed = 1f;
    [SerializeField] float returnSpeed = 1f;
    float targetRotation = 0;
    public bool returnToStartPos = false;

    public Vector3 startPosition;
    private Vector3 screenPoint;
    private Vector3 offset;
    private float timeSinceLastClick = 0;

    [SerializeField] private DissolveEffect dissolve;
    [SerializeField] AudioSource source;
    [SerializeField] AudioClip flipSound;
    [SerializeField] AudioClip clickSound;

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
        if (cardType == GameManager.CardType.Monster)
        {
            isDraggable = false;
            //SetMonsterValues(0);
        }
    }

    private void Start()
    {
        hoverSpriteRenderer.color = GameManager.Instance.basicHoverColor;
        ChooseSprite();
        //Invoke("ChooseSprite", 2f);
    }

    public void ChooseSprite()
    {
        //Sprite[] cardSpriteSheet = Resources.LoadAll<Sprite>("cards");
        string number = "";
        string type = "";
        if (cardType != GameManager.CardType.Monster) number = damageValue.ToString();
        else
        {
            switch (monsterType)
            {
                case MonsterType.As:
                    number = "a";
                    break;
                case MonsterType.Dame:
                    number = "q";
                    break;
                case MonsterType.Roi:
                    number = "k";
                    break;
                case MonsterType.Vallet:
                    number = "j";
                    break;
            }
        }
        switch (atoutType)
        {
            case GameManager.AtoutType.Carreau:
                type = "c";
                break;
            case GameManager.AtoutType.Coeur:
                type = "co";
                break;
            case GameManager.AtoutType.Pic:
                type = "p";
                break;
            case GameManager.AtoutType.Trefle:
                type = "t";
                break;
        }
        string predicat = "cards_" + number + type;
        //frontRenderer.sprite = cardSpriteSheet.Single(s => s.name == predicat);
        frontRenderer.sprite = GameManager.Instance.cardSpriteSheet.Single(s => s.name == predicat);
    }

    public void SetMonsterValues(int floorBonus)
    {
        //print("On :" + gameObject.name + " floorBonus = " + floorBonus);
        if (monsterType == MonsterType.Vallet)
        {
            damageValue = (GameManager.Instance.ValletDamageValue + floorBonus);
            HP = GameManager.Instance.ValletHP;
            APWin = GameManager.Instance.ValletAP;
        }
        else if (monsterType == MonsterType.Dame)
        {
            damageValue = (GameManager.Instance.DameDamageValue + floorBonus);
            HP = GameManager.Instance.DameHP;
            APWin = GameManager.Instance.DameAP;
        }
        else if (monsterType == MonsterType.Roi)
        {
            damageValue = (GameManager.Instance.RoiDamageValue + floorBonus);
            HP = GameManager.Instance.RoiHP;
            APWin = GameManager.Instance.RoiAP;
        }
        else if (monsterType == MonsterType.As)
        {
            damageValue = (GameManager.Instance.AsDamageValue + floorBonus);
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
            if (Vector3.Distance(transform.position,startPosition)<0.2f)
            {
                transform.position = startPosition;
                returnToStartPos = false;
                if (GameManager.Instance.saveWrongAttack) GameManager.Instance.TutoAttackSameType();
            }
            if (isFlipped && isInHand) Flip();
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
            if(!GameManager.Instance.blockAttack && (cardType==GameManager.CardType.Attack || cardType == GameManager.CardType.Heal) && cardCol.cardType==GameManager.CardType.Monster && cardCol.isFlipped==false)
            {
                if(targetAttackCard)
                {
                    if (Vector2.Distance(transform.position, cardCol.transform.position)> Vector2.Distance(transform.position, targetAttackCard.transform.position))
                    {
                        return;
                    }
                    targetAttackCard.StopHover();
                }
                
                if(!GameManager.Instance.waitForAttackSameType && cardCol.atoutType != atoutType)
                {
                    if (GameManager.Instance.waitForAttackTrefle && atoutType != GameManager.AtoutType.Trefle) return;
                    if (GameManager.Instance.waitForPotionAttack && cardType != GameManager.CardType.Heal) return;
                    cardCol.HoverAttack();
                    HoverAttack();
                    targetAttackCard = cardCol;
                }
                else
                {
                    cardCol.HoverCantAttack();
                    HoverCantAttack();
                }
            }
        }
        //else if(cardType==GameManager.CardType.Heal && collision.gameObject.tag == "Health")
        //{
        //    isAboutToHeal = true;
        //    HoverHeal();
        //}
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        Card cardCol = collision.gameObject.GetComponent<Card>();
        if (cardCol)
        {
            cardCol.StopHover();
            if (cardCol == targetAttackCard) targetAttackCard = null;
        }
        //else if (cardType == GameManager.CardType.Heal && collision.gameObject.tag == "Health")
        //{
        //    isAboutToHeal = false;
        //}
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

    public void HoverCantAttack()
    {
        if (!hoverSpriteRenderer || !frontRenderer || !backRenderer) return;

        hoverSpriteRenderer.enabled = true;
        hoverSpriteRenderer.color = GameManager.Instance.cantAttackHoverColor;
        frontRenderer.color = GameManager.Instance.cantAttackHoverColor;
        backRenderer.color = GameManager.Instance.cantAttackHoverColor;

        if (GameManager.Instance.waitForAttackSameType) GameManager.Instance.saveWrongAttack = true;
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
            source.clip = clickSound;
            source.Play();
            float clickTime = Time.time - timeSinceLastClick;

            if (clickTime <= GameManager.Instance.doubleClickTimer)
            {
                if (cardType == GameManager.CardType.Heal && !isFlipped)
                {
                    if(!GameManager.Instance.blockPotionUse && GameManager.Instance.UseAP())
                    {
                        GameManager.Instance.Heal(damageValue);
                        DestroyCard();
                    }
                }
                else if (isInHand || !isFlippable || stillInDeck) return;
                else if (cardType == GameManager.CardType.Monster)
                {
                    if(GameManager.Instance.waitForExploreFirst)
                    {
                        if(this == GameManager.Instance.roomList[0])
                        {
                            if (!GameManager.Instance.blockExplore && GameManager.Instance.UseAP())
                            {
                                Flip();
                                GameManager.Instance.TutoExplore();
                            }
                        }
                    }
                    else
                    {
                        if (!GameManager.Instance.blockExplore && GameManager.Instance.UseAP())
                            Flip();
                    }
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
            //else if(isAboutToHeal)
            //{
            //    if(GameManager.Instance.UseAP())
            //    {
            //        GameManager.Instance.Heal(damageValue);
            //        DestroyCard();
            //    }
            //    else
            //    {
            //        print("feedback not engough point");
            //    }
            //}
        }
        else
        {
            Vector3 newPos = transform.position;
            newPos.z = 0;
            transform.position = newPos;
        }

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
            if(Input.mousePosition.y>350)
                curPosition.y += 0.5f;
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
        GameManager.Instance.PlayDamageCard();
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
            if (GameManager.Instance.waitForAttackPotion && attacker.cardType == GameManager.CardType.Heal) GameManager.Instance.TutoPotionAttack();
        }
        else
        {
            Invoke("TriggerAttack", 1f);
        }
        attacker.DestroyCard();
    }

    void TriggerAttack()
    {
        animator.SetTrigger(hashMonsterAttack);
    }

    public void DestroyCard()
    {
        backRenderer.enabled = false;
        Destroy(UIHolder);
        if (cardType == GameManager.CardType.Monster)
        {
            GameManager.Instance.GainAP(APWin);
            GameManager.Instance.UpdateRoomList(this);
        }
        else if (isInHand)
        {
            GameManager.Instance.RemoveHandCard(this);
        }

        if (cardType == GameManager.CardType.Heal && targetAttackCard != null)
            dissolve.StartDissolve(GameManager.CardType.Attack);
        else
            dissolve.StartDissolve(cardType);

                GetComponent<BoxCollider2D>().enabled = false;
        Invoke("DestroyGameObject", 1.5f);
    }

    void DestroyGameObject()
    {
        Destroy(gameObject);
    }

    public void Flip()
    {
        targetRotation += 180;
        if (targetRotation >= 360)
            targetRotation -= 360;
        isFlipped = !isFlipped;
        if (GameManager.Instance.startGame)
        {
            source.clip = flipSound;
            source.Play();
        }

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
