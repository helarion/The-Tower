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
    public Sprite[] cardSpriteSheet;

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

    public List<Card> roomList;

    [Header("UI")]
    [SerializeField] TextMeshProUGUI hpText;
    [SerializeField] TextMeshProUGUI apText;
    [SerializeField] ScreenShake screenShake;
    public UIHandler uiHandler;

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

    public bool saveWrongAttack = false;
    bool waitForMonsterAttack = false;
    public bool waitForExploreFirst = false;
    public bool waitForAttackDraw = false;
    public bool waitForAttackSameType = false;
    public bool waitForAttackTrefle = false;
    public bool waitForAttackPotion = false;
    public bool waitForPotionDraw = false;
    public bool waitForPotionUse = false;
    public bool waitForPotionAttack = false;
    public bool waitForDamagePlayer = false;
    public bool waitForClearStage = false;

    public bool blockPotionUse = false;
    public bool blockAttackDraw = false;
    public bool blockPotionDraw = false;
    public bool blockAttack = false;
    public bool blockExplore = false;

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
        cardSpriteSheet = Resources.LoadAll<Sprite>("cards");
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

            //c.ChooseSprite();
        }

        InitializeGame();
        MusicHandler.Instance.UpdateVolumeSources();
    }

    void Swap<T>(IList<T> list, int indexA, int indexB)
    {
        T tmp = list[indexA];
        list[indexA] = list[indexB];
        list[indexB] = tmp;
    }

    private void InitializeGame()
    {
        currentPlayerHP = startPlayerHP;
        currentPlayerAP = startPlayerAP;
        UpdatePlayerValues();

        if(MusicHandler.Instance.playTutorial)
        {
            Swap<Card>(MonsterList, 0, LookForCard(AtoutType.Pic, CardType.Monster, Card.MonsterType.Roi, -1));
            Swap<Card>(PotionList, 0, LookForCard(AtoutType.Coeur, CardType.Heal, Card.MonsterType.As, 5));
            Swap<Card>(PotionList, 1, LookForCard(AtoutType.Coeur, CardType.Heal, Card.MonsterType.As, 9));
            Swap<Card>(AttackList, 0, LookForCard(AtoutType.Pic, CardType.Attack, Card.MonsterType.As, 4));
            Swap<Card>(AttackList, 1, LookForCard(AtoutType.Trefle, CardType.Attack, Card.MonsterType.As, 9));
            Swap<Card>(AttackList, 2, LookForCard(AtoutType.Carreau, CardType.Attack, Card.MonsterType.As, 3));
            Swap<Card>(AttackList, 3, LookForCard(AtoutType.Pic, CardType.Attack, Card.MonsterType.As, 2));
            Swap<Card>(AttackList, 4, LookForCard(AtoutType.Pic, CardType.Attack, Card.MonsterType.As, 8));
        }
        else
        {
            ShuffleList(MonsterList);
            ShuffleList(PotionList);
            ShuffleList(AttackList);
        }

        MoveDeckTo(MonsterList, monsterDeckPosition.position);
        MoveDeckTo(AttackList, attackDeckPosition.position);
        MoveDeckTo(PotionList, potionDeckPosition.position);

        DrawMonsters();
        FirstDraw();
        startGame = true;
    }
    #endregion

    private int LookForCard(AtoutType atout, CardType type, Card.MonsterType monster, int value)
    {
        if(type==CardType.Monster)
        {
            for(int i=0; i<MonsterList.Count; i++)
            {
                if (MonsterList[i].atoutType == atout && MonsterList[i].monsterType == monster) return i;
            }
        }
        else if(type==CardType.Heal)
        {
            for(int i= 0; i < PotionList.Count; i++)
            {
                if (PotionList[i].damageValue == value) return i;
            }
        }
        else
        {
            for (int i = 0; i < AttackList.Count; i++)
            {
                if (AttackList[i].atoutType == atout && AttackList[i].damageValue == value) return i;
            }
        }
        return -1;
    }

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
        int rand = Random.Range(0, 3);
        if(!MusicHandler.Instance.playTutorial) roomList[rand].Flip();

        if (waitForClearStage)
        {
            TutoClearStage();
            blockAttack = true;
            blockAttackDraw = true;
            blockExplore = true;
            blockPotionDraw = true;
            blockPotionUse = true;
        }
    }

    public void TryDraw(Card c)
    {
            //if (MonsterList.Contains(c) && roomList.Count <= 0)
            //{
            //    DrawMonsters();
            //}
        if (!blockAttackDraw && AttackList.Contains(c) && UseAP())
        {
            if (AttackList.Count > 1) AttackList[1].gameObject.SetActive(true);
            c.gameObject.SetActive(true);
            AddToHand(c);
            if(waitForAttackDraw)
            {
                TutoAttackDraw();
            }
        }
        else if (!blockPotionDraw && PotionList.Contains(c) && UseAP())
        {
            if (PotionList.Count > 1) PotionList[1].gameObject.SetActive(true);
            c.gameObject.SetActive(true);
            AddToHand(c);
            if (waitForPotionDraw) TutoPotionDraw();
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
        if (waitForPotionUse) TutoPotionUse();
    }

    public void DamageMonster()
    {
        if(waitForAttackTrefle)
        {
            TutoAttackTrefle();
        }
        if(waitForMonsterAttack)
        {
            TutoMonsterAttack();
        }
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
        if (waitForPotionAttack) TutoPotionAttack();
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
        if (waitForDamagePlayer) Invoke("TutoDamagePlayer", 1.5f);
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
        SceneManager.LoadScene(1);
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
            //print("roomExploredcount : " + roomExplored);
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

    private void WaitForMonsterAttack()
    {
        waitForMonsterAttack = true;
    }

    private void WaitForExploreFirst()
    {
        waitForExploreFirst = true;
    }

    private void WaitForAttackDraw()
    {
        waitForAttackDraw = true;
    }

    private void WaitForAttackSameType()
    {
        waitForAttackSameType = true;
    }

    private void WaitForAttackTrefle()
    {
        waitForAttackTrefle = true;
    }

    private void WaitForAttackPotion()
    {
        waitForAttackPotion = true;
    }

    private void WaitForPotionUse()
    {
        waitForPotionUse = true;
    }

    private void WaitForPotionDraw()
    {
        waitForPotionDraw = true;
    }

    private void WaitForPotionAttack()
    {
        waitForPotionAttack = true;
    }

    private void WaitForDamagePlayer()
    {
        waitForDamagePlayer = true;
    }

    private void WaitForClearStage()
    {
        waitForClearStage = true;
    }

    public void TutoExplore()
    {
        waitForExploreFirst = false;
        blockExplore = true;
        Invoke("InvokeNextStep", 1f);
    }

    public void TutoAttackTrefle()
    {
        waitForAttackTrefle = false;
        blockAttack = true;
        uiHandler.NextStepTutorial();
        Invoke("WaitBeforeTimeScale", 0.5f);
    }

    void WaitBeforeTimeScale()
    {
        Time.timeScale = 0;
    }

    void TutoMonsterAttack()
    {
        waitForMonsterAttack = false;
        uiHandler.NextStepTutorial();
        blockAttack = true;
    }

    void TutoDamagePlayer()
    {
        waitForDamagePlayer = false;
        uiHandler.NextStepTutorial();
    }

    void TutoAttackDraw()
    {
        waitForAttackDraw = false;
        blockAttackDraw = true;
        Invoke("InvokeNextStep", 1.5f);
    }

    void InvokeNextStep()
    {
        uiHandler.NextStepTutorial();
    }

    public void TutoAttackSameType()
    {
        saveWrongAttack = false;
        waitForAttackSameType = false;
        uiHandler.NextStepTutorial();
        blockAttack = true;
    }

    public void TutoPotionDraw()
    {
        waitForPotionDraw = false;
        blockPotionDraw = true;
        Invoke("InvokeNextStep", 1.5f);
    }

    public void TutoPotionUse()
    {
        waitForPotionUse = false;
        uiHandler.NextStepTutorial();
        blockPotionUse = true;
    }

    public void TutoPotionAttack()
    {
        waitForPotionAttack = false;
        uiHandler.NextStepTutorial();
        blockAttack = true;
    }

    public void TutoClearStage()
    {
        waitForClearStage = false;
        Invoke("InvokeNextStep", 1.5f);
    }

    public Transform FindPosition(int id)
    {
        switch (id)
        {
            case 0:
                return apText.transform;
                break;
            case 1:
                return roomList[0].transform;
                break;
            case 2:
                return roomList[0].AttackText.transform;
                break;
            case 3:
                return roomList[0].HPText.transform;
                break;
            case 4:
                return attackDeckPosition.transform;
                break;
            case 5:
                return HandList[HandList.Count - 1].transform;
                break;
            case 6:
                return HandList[1].transform;
                break;
            case 8:
                return hpText.transform;
                break;
            case 9:
                return PotionList[1].transform;
                break;
            case 10:
                return HandList[HandList.Count - 1].transform;
                return null;
                break;
            case 11:
                foreach (Card c in HandList)
                {
                    if (c.cardType == CardType.Heal)
                        return c.transform;
                }
                break;
        }
        return null;
    }

    public void FindScript(int id)
    {
        switch (id)
        {
            case 1:
                WaitForExploreFirst();
                blockExplore = false;
                break;
            case 4:
                WaitForAttackDraw();
                blockAttackDraw = false;
                break;
            case 5:
                WaitForAttackSameType();
                blockAttack = false;
                break;
            case 6:
                WaitForAttackTrefle();
                blockAttack = false;
                break;
            case 7:
                WaitForDamagePlayer();
                break;
            case 9:
                WaitForPotionDraw();
                blockPotionDraw = false;
                break;
            case 10:
                WaitForPotionUse();
                blockPotionUse = false;
                break;
            case 11:
                WaitForPotionAttack();
                blockAttack = false;
                break;
            case 12:
                WaitForClearStage();
                GainAP(4);
                blockAttack = false;
                blockAttackDraw = false;
                blockPotionDraw = false;
                blockExplore = false;
                blockPotionUse = false;
                break;
        }
    }

}
