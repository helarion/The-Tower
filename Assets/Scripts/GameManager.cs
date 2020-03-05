using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    #region variables
    private static GameManager instance = null;
    private List<Card> AttackList;
    private List<Card> PotionList;
    private List<Card> MonsterList;
    private List<Card> HandList;

    [Header("Deck Positions")]
    [SerializeField] Transform potionDeckPosition;
    [SerializeField] Transform attackDeckPosition;
    [SerializeField] Transform monsterDeckPosition;

    [SerializeField] Transform monsterUpPosition;
    [SerializeField] Transform monsterDownPosition;
    [SerializeField] Transform monsterLeftPosition;
    [SerializeField] Transform monsterRightPosition;

    [SerializeField] Transform handStartPos;
    private Vector3 currentHandStart;

    [Header("Clamp card positions")]
    public float minX;
    public float maxX;
    public float minY;
    public float maxY;
    [SerializeField] float cardSize = 0.5f;
    private float howManyAdded = 0;
    public float doubleClickTimer;

    [Header("Game Values")]

    [Header("Player Values")]
    public int startPlayerHP = 6;
    public int currentPlayerHP;
    [SerializeField] int startAttackHandCardNumber = 5;
    [SerializeField] int startPotionHandCardNumber = 1;
    [SerializeField] int startPlayerAP = 4;
    int currentPlayerAP;
    public int currentFloor = 1;

    [Header("Monster Values")]
    public int ValletDamageValue = 1;
    public int DameDamageValue = 2;
    public int RoiDamageValue = 2;
    public int AsDamageValue = 3;

    public int ValletHP = 10;
    public int DameHP = 14;
    public int RoiHP = 18;
    public int AsHP = 22;

    public int ValletAP = 1;
    public int DameAP = 2;
    public int RoiAP = 3;
    public int AsAP = 4;

    List<Card> roomList;

    [Header("UI")]
    [SerializeField] TextMeshProUGUI hpText;
    [SerializeField] TextMeshProUGUI apText;
    [SerializeField] ScreenShake screenShake;
    [SerializeField] UIHandler uiHandler;

    [Header("Hover Colors")]
    public Color basicHoverColor;
    public Color healingHoverColor;
    public Color attackHoverColor;
    public Color cantAttackHoverColor;
    public bool startGame = false;

    [SerializeField] AudioSource source;
    [SerializeField] AudioClip[] damagePlayerAudio;
    [SerializeField] AudioClip[] damageMonsterAudio;
    [SerializeField] AudioClip potionDrinkAudio;
    [SerializeField] AudioClip switchFloor;

    public enum AtoutType
    {
        Trefle,
        Pic,
        Carreau,
        Coeur
    }

    public enum CardType
    {
        Heal,
        Attack,
        Monster
    }
    #endregion

    #region Start&Initialisation
    // Game Instance Singleton
    public static GameManager Instance
    {
        get
        {
            return instance;
        }
    }

    private void Awake()
    {
        Application.targetFrameRate = 60;
        //// if the singleton hasn't been initialized yet
        //if (instance != null && instance != this)
        //{
        //    Destroy(this.gameObject);
        //}

        instance = this;
        //DontDestroyOnLoad(this.gameObject);
    }

    private void Start()
    {
        roomList = new List<Card>();
        MonsterList = new List<Card>();
        PotionList = new List<Card>();
        AttackList = new List<Card>();
        HandList = new List<Card>();
        Card[] temp = FindObjectsOfType<Card>();
        foreach (Card c in temp)
        {
            if (c.cardType == CardType.Monster)
                MonsterList.Add(c);
            else if (c.cardType == CardType.Heal)
                PotionList.Add(c);
            else if (c.cardType == CardType.Attack)
                AttackList.Add(c);
        }

        InitializeGame();
        MusicHandler.Instance.UpdateVolumeSources();
    }

    private void InitializeGame()
    {
        currentPlayerHP = startPlayerHP;
        currentPlayerAP = startPlayerAP;
        UpdatePlayerValues();

        ShuffleList(MonsterList);
        ShuffleList(PotionList);
        ShuffleList(AttackList);

        MoveDeckTo(MonsterList, monsterDeckPosition.position);
        MoveDeckTo(AttackList, attackDeckPosition.position);
        MoveDeckTo(PotionList, potionDeckPosition.position);

        DrawMonsters();
        FirstDraw();
        startGame = true;
    }
    #endregion

    private void UpdatePlayerValues()
    {
        hpText.text = currentPlayerHP.ToString();
        apText.text = currentPlayerAP.ToString();
    }

    private void DrawMonsters()
    {
        MonsterList[0].startPosition = monsterUpPosition.position;
        MonsterList[1].startPosition = monsterDownPosition.position;
        MonsterList[2].startPosition = monsterLeftPosition.position;
        MonsterList[3].startPosition = monsterRightPosition.position;
        if (MonsterList.Count > 4) MonsterList[4].gameObject.SetActive(true);

        if (startGame)
        {
            currentFloor++;
            //uiHandler.NewStage(currentFloor);
        }
        else
        {
            source.clip = switchFloor;
            source.Play();
        }
        monsterDeckPosition.GetComponent<AudioSource>().Play();

        for (int i = 0; i < 4; i++)
        {
            MonsterList[0].gameObject.SetActive(true);
            roomList.Add(MonsterList[0]);
            MonsterList[0].returnToStartPos = true;
            MonsterList[0].stillInDeck = false;
            MonsterList[0].SetMonsterValues((currentFloor - 1));
            MonsterList.RemoveAt(0);
        }
    }

    public void TryDraw(Card c)
    {
        //if (MonsterList.Contains(c) && roomList.Count <= 0)
        //{
        //    DrawMonsters();
        //}
        if (AttackList.Contains(c))
        {
            if (AttackList.Count > 1) AttackList[1].gameObject.SetActive(true);
            c.gameObject.SetActive(true);
            AddToHand(c);
        }
        else if (PotionList.Contains(c))
        {
            if (PotionList.Count > 1) PotionList[1].gameObject.SetActive(true);
            c.gameObject.SetActive(true);
            AddToHand(c);
        }
    }

    private void FirstDraw()
    {
        for (int i = 0; i < startAttackHandCardNumber; i++)
        {
            AttackList[0].gameObject.SetActive(true);
            AddToHand(AttackList[0]);
        }
        for (int i = 0; i < startPotionHandCardNumber; i++)
        {
            PotionList[0].gameObject.SetActive(true);
            AddToHand(PotionList[0]);
        }
        PotionList[0].gameObject.SetActive(true);
        AttackList[0].gameObject.SetActive(true);
        UpdateHandSize();
    }

    public void AddToHand(Card c)
    {
        HandList.Add(c);
        c.Flip();
        c.stillInDeck = false;
        c.isInHand = true;
        c.isDraggable = true;

        if (c.cardType == CardType.Monster) MonsterList.Remove(c);
        else if (c.cardType == CardType.Attack) AttackList.Remove(c);
        else if (c.cardType == CardType.Heal) PotionList.Remove(c);

        UpdateHandSize();
    }

    public void UpdateHandSize()
    {
        howManyAdded = 0;
        currentHandStart = handStartPos.position;
        currentHandStart += new Vector3(-(HandList.Count * (cardSize / 2)), 0, 0);
        for (int i = 0; i < HandList.Count; i++)
        {
            HandList[i].startPosition = currentHandStart; //relocating my card to the Start Position
            HandList[i].startPosition += new Vector3((howManyAdded * cardSize), 0, 0); // Moving my card 1f to the right
            HandList[i].returnToStartPos = true;
            howManyAdded++;
        }
    }

    public void RemoveHandCard(Card destroyedCard)
    {
        HandList.Remove(destroyedCard);
        UpdateHandSize();
    }

    private void MoveDeckTo(List<Card> deckToMove, Vector3 position)
    {
        int i = 0;
        foreach (Card c in deckToMove)
        {
            if (i != 0)
                c.gameObject.SetActive(false);
            c.transform.position = position;
            i++;
        }
    }

    private void ShuffleList(List<Card> list)
    {
        for (int i = 0; i < list.Count; i++)
        {
            Card temp = list[i];
            int randomIndex = Random.Range(i, list.Count);
            list[i] = list[randomIndex];
            list[randomIndex] = temp;
        }
    }

    public void UpdateRoomList(Card destroyedMonster)
    {
        roomList.Remove(destroyedMonster);
        CheckForWin();
    }

    private void CheckForWin()
    {
        if(roomList.Count <= 0)
        {
            if(MonsterList.Count <= 0)
            {
               Win();
            }
            else
            {
                Invoke("NewStage", 1.5f);
                
                Invoke("DrawMonsters",4.2f);
            }
        }
    }

    private void NewStage()
    {
        source.clip = switchFloor;
        source.Play();
        uiHandler.NewStage(currentFloor + 1);
    }

    public void Heal(int value)
    {
        uiHandler.HealthBonus("+" + value);
        uiHandler.HealPlayer();
        currentPlayerHP += value;
        UpdatePlayerValues();
        source.clip = potionDrinkAudio;
        source.Play();
    }

    public void DamageMonster()
    {
        uiHandler.DamageMonster();
        CheckIfLost();
    }

    public bool UseAP()
    {
        if (currentPlayerAP >= 1)
        {
            uiHandler.APBonus("-1");
            currentPlayerAP--;
;            UpdatePlayerValues();
            CheckIfLost();
            return true;
        }
        return false;
    }

    public void GainAP(int value)
    {
        uiHandler.APBonus("+"+value);
        currentPlayerAP += value;
        UpdatePlayerValues();
    }

    AudioClip RandomClip(AudioClip[] list)
    {
        int rand = Random.Range(0, list.Length);
        return list[rand];
    }

    public void PlayDamageCard()
    {
        source.clip = RandomClip(damageMonsterAudio);
        source.Play();
    }

    public void DamagePlayer(int value)
    {
        source.clip = RandomClip(damagePlayerAudio);
        source.Play();
        uiHandler.DamageMalus("-"+value);
        uiHandler.DamagePlayer();
        screenShake.TriggerShake(0.3f, ScreenShake.ShakeIntensity.high);
        currentPlayerHP -= value;
        UpdatePlayerValues();
        if (currentPlayerHP <= 0)
        {
            Lose();
        }
    }

    private void Win()
    {
        uiHandler.FadeInWin();
    }

    private void Lose()
    {
        uiHandler.FadeInLost();
    }

    public void RestartGame()
    {
        SceneManager.LoadScene(0);
    }

    private Card FindBestDamage(List<Card>Hand, Card monster)
    {
        int max = 0;
        Card r = Hand[0];
        foreach(Card c in Hand)
        {
            int d = c.AnticipateDamage(monster);
            if(d>max)
            {
                max = d;
                r = c;
            }
        }
        return r;
    }

    private void CheckIfLost()
    {
        if(currentPlayerAP==0)
        {
            // plus de pioche
            if (HandList.Count == 0)
            {
                uiHandler.UpdateWarningText("Plus assez d'AP pour pouvoir piocher");
                //Lose();
                return;
            }

            // plus d'AP et aucune room explorée
            int roomExplored = 0;
            bool canKillOneMonster = false;
            foreach (Card monster in roomList)
            {
                if (!monster.isFlipped)
                {
                    roomExplored++;
                    List<Card> tempHandList = new List<Card>(HandList);
                    int tempPlayerHP = currentPlayerHP;
                    int tempMonsterHP = monster.HP;
                    while (tempHandList.Count > 0)
                    {
                        Card temp = FindBestDamage(tempHandList, monster);
                        if(temp == null)
                        {
                            print("best damage null");
                            //break;
                        }
                        tempHandList.Remove(temp);
                        tempMonsterHP -= temp.AnticipateDamage(monster);
                        if (tempMonsterHP > 0)
                        {
                            tempPlayerHP -= monster.damageValue;
                            if (tempPlayerHP <= 0)
                            {
                                break;
                            }
                        }
                        else
                        {
                            canKillOneMonster = true;
                            break;
                        }
                    }
                }
            }
            print("roomExploredcount : " + roomExplored);
            if (roomExplored == 0)
            {
                uiHandler.UpdateWarningText("Plus assez d'AP pour explorer une salle et aucune explorée");
                print("=0");
                //Lose();
                return;
            }
            else if (!canKillOneMonster)
            {
                uiHandler.UpdateWarningText("Attaques insuffisantes pour tuer n'importe lequel des monstres sans mourrir");
                print("lol");
                //Lose();
                return;
            }
        }
    }
}
