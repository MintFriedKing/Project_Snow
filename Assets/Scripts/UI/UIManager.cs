using Michsky.UI.Shift;
using PS;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
[Serializable]
public class CharacterListUI //게임 매니저에서 할당한 계획
{
    public RectTransform rectTransform;
    public Slider slider;
    public Image characterIcon;
    public Image dieCharacterIcon;
    public Image listBackGround;
    public Image selecIcon;
    public Color dieBackGroundColor;
    public GameObject dieChildObject;
    public GameObject aliveChildObject;
}
public class UIManager : MonoBehaviour
{
   
    [SerializeField, Header("조준점 UI")]
    private Crosshair crosshair;
    [SerializeField, Header("쉴드바 ")]
    private ShieldBar shieldBar;
    
    public static UIManager Instance;
    public Slider healthSlider;
    public Slider easeHealthSlider;
    public float lerpSpeed = 0.05f;
    [Header("Time")]
    public TMP_Text timerText;
    [Header("AmmoUI")]
    public TMP_Text currentAmmoText;
    public Slider currentAmmoSlier;
    [Header("SkillUI")]
    public List<CircleSlider> circleSliders;   
    public TMP_Text nomalSkilltext;
    [Header("CharacterListUI")]
    public Image playerStatusIcon;
    public Image playerNormalSkillIcon;
    public Sprite selectedBackGroundSprite;
    public Sprite unselectedBackGroundSprite;
    public List<CharacterListUI> characterListUIs;
    [Header("Stemina")]
    public Slider steminaSlider;
    public ModalWindowManager pause;
    public Button exitButton;
    public Button goaheadButton;
    [Header("BossUI")]
    public Slider bossHealthUI;
    public Image bossIcon;
    public Sprite bossIconSprite;

    public Canvas mainCanvas;
    public Canvas defeatCanvas;
    public Button defeatHomeButton;
    public Button defeatAgainButton;
    public ShieldBar ShieldBar { get { return shieldBar; }  set { shieldBar = value; } }
    private void Awake()
    {
        Init();
        InitCharacterList();
    }
   
    void Update()
    {
        UpdateTimeText();
       
        UpdateAimUI();
        currentAmmoText.text = GameManager.Instance.CurrentPlayer.PlayerShootManager.gun.CurrentAmmo.ToString();
        SetAmmoSlier();

       

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            
            pause.gameObject.SetActive(true);
            pause.ModalWindowIn();
            Time.timeScale = 0f;
        }


        //스킬 쿨타임
        if (GameManager.Instance.CurrentPlayer.IsUseSkill == true)
        {
            nomalSkilltext.gameObject.SetActive(true);           
        }
        else 
        {
            foreach (CircleSlider circleSlider in circleSliders)
            {
                circleSlider.UpdateSlider(0);
            }
            nomalSkilltext.gameObject.SetActive(false);
        }
        //
        //헬스 바 연출 
        if (healthSlider.value != GameManager.Instance.CurrentPlayer.PlayerHealth.CurrentHealth)
        {
            healthSlider.value = GameManager.Instance.CurrentPlayer.PlayerHealth.CurrentHealth;
        }

        if (easeHealthSlider.value != healthSlider.value)
        {
            easeHealthSlider.value = Mathf.Lerp(easeHealthSlider.value, GameManager.Instance.CurrentPlayer.PlayerHealth.CurrentHealth,lerpSpeed);   
        }
        //쉴드 연출 
        if (shieldBar.shieldSlier.value != shieldBar.currentShield) //쉴드바 동기화
        {
            shieldBar.shieldSlier.value = shieldBar.currentShield;
        }
        if (shieldBar.easeShieldSlider.value != shieldBar.shieldSlier.value)
        {
            easeHealthSlider.value = Mathf.Lerp(shieldBar.easeShieldSlider.value , shieldBar.currentShield,lerpSpeed);

        }
        //스테미나 바---------------------------------------------------
        if (steminaSlider.gameObject.activeSelf == true
            && Input.GetKey(KeyCode.LeftShift))
        {
            UpdateStemina();
        }
        if (GameManager.Instance.CurrentPlayer.CurrentStemina < GameManager.Instance.CurrentPlayer.MaxStemina
        && Input.GetKey(KeyCode.LeftShift) == false)
        {
            RechargeStemina();
        }
        UpdateSteminaColor();
        //-----------------------------------------------------------------------

        if (Input.GetKeyDown(KeyCode.Space))
        {
            GameManager.Instance.CurrentPlayer.PlayerHealth.TakeDamage(10f,Vector3.zero);
        }
        if (Input.GetKeyDown(KeyCode.Z))
        {
            shieldBar.TakeDamage(10f);
        }
    }
    private void Init()
    {
        Instance = this;
        mainCanvas.gameObject.SetActive(true);
        defeatCanvas.gameObject.SetActive(false);

        nomalSkilltext.gameObject.SetActive(false);
        GameManager gameManager = GameManager.Instance;
        playerStatusIcon.sprite = gameManager.players[gameManager.SelectNumber - 1].playerIcon;
        playerNormalSkillIcon.sprite = gameManager.players[gameManager.SelectNumber - 1].skillIcon;
        ChangeCharacterList();
        healthSlider.maxValue = (float)GameManager.Instance.CurrentPlayer.PlayerHealth.Maxhealth;
        easeHealthSlider.maxValue = (float)GameManager.Instance.CurrentPlayer.PlayerHealth.Maxhealth;

        healthSlider.value = (float)GameManager.Instance.CurrentPlayer.PlayerHealth.Maxhealth;
        easeHealthSlider.value = (float)GameManager.Instance.CurrentPlayer.PlayerHealth.Maxhealth;

        shieldBar.shieldSlier.value = 0f;
        shieldBar.easeShieldSlider.value = 0f;

        shieldBar.shieldSlier.maxValue = shieldBar.maxShield;
        shieldBar.easeShieldSlider.maxValue = shieldBar.maxShield;

        steminaSlider.maxValue = GameManager.Instance.CurrentPlayer.MaxStemina;
        steminaSlider.value = GameManager.Instance.CurrentPlayer.MaxStemina;
        steminaSlider.gameObject.SetActive(false);

        
        exitButton.onClick.AddListener( delegate 
        {
            Time.timeScale = 1f;
            pause.ModalWindowOut();
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None; // 커서 잠금 해제
            OnExitButton("Lobby");
        }
        );
        defeatHomeButton.onClick.AddListener(delegate
        {
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None; // 커서 잠금 해제
            OnExitButton("Lobby");
        }
        );
        defeatAgainButton.onClick.AddListener(delegate
        {
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked; 
            OnExitButton("SoloPlayInGame");
        }
        );
        goaheadButton.onClick.RemoveAllListeners();
        goaheadButton.onClick.AddListener(delegate 
        {
            Time.timeScale = 1f;
            pause.ModalWindowOut();
            //pause.gameObject.SetActive(false);
            }
        );

       
    }
    private void UpdateAimUI()
    { 
        crosshair.SetActiveCrosshair(true);
        crosshair.UpdatePosition(Player.Instance.PlayerShootManager.AimPosition);
    }
    public void ChangeCharacterList() //캐릭터 교체시 
    {
        for (int i = 0; i < characterListUIs.Count; i++)
        {

           if (GameManager.Instance.players[i].player.playerState != Player.PlayerState.DIE) //죽은 애들이 아니면 
           {
                characterListUIs[i].characterIcon.sprite = GameManager.Instance.players[i].playerIcon;
                characterListUIs[i].rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, 400f);
                characterListUIs[i].listBackGround.sprite = unselectedBackGroundSprite;
                characterListUIs[i].selecIcon.gameObject.SetActive(false);
           }
         
        }
        characterListUIs[GameManager.Instance.SelectNumber-1].rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, 500f);
        characterListUIs[GameManager.Instance.SelectNumber - 1].listBackGround.sprite = selectedBackGroundSprite;
        characterListUIs[GameManager.Instance.SelectNumber - 1].selecIcon.gameObject.SetActive(true);
    }
    public void ChangeSkillSprite()
    {
        playerNormalSkillIcon.sprite = GameManager.Instance.players[GameManager.Instance.SelectNumber - 1].skillIcon;
    }
    public void SetAmmoSlier()
    {
        int maxAmmo = GameManager.Instance.CurrentPlayer.PlayerShootManager.gun.MaxCapacity;
        int currentAmmo = GameManager.Instance.CurrentPlayer.PlayerShootManager.gun.CurrentAmmo;
        currentAmmoSlier.value = (float)currentAmmo / maxAmmo;
    }
    public void InitStatusBar()
    {
        steminaSlider.value = GameManager.Instance.CurrentPlayer.CurrentStemina;
        if (GameManager.Instance.CurrentPlayer.CurrentStemina != GameManager.Instance.CurrentPlayer.MaxStemina)
        {
            steminaSlider.gameObject.SetActive(true);
        }
        else 
        {
            steminaSlider.gameObject.SetActive(false);
        }
    }
    public void UpdateStemina()
    {    
        GameManager.Instance.CurrentPlayer.CurrentStemina -= 20f* Time.deltaTime;
        steminaSlider.value = GameManager.Instance.CurrentPlayer.CurrentStemina;       
    }
    public void RechargeStemina()
    {
        steminaSlider.gameObject.SetActive(true);
        GameManager.Instance.CurrentPlayer.CurrentStemina += 20f * Time.deltaTime;
        steminaSlider.value = GameManager.Instance.CurrentPlayer.CurrentStemina;
        if (GameManager.Instance.CurrentPlayer.CurrentStemina >= GameManager.Instance.CurrentPlayer.MaxStemina
            && Input.GetKey(KeyCode.LeftShift) == false)
        {
            steminaSlider.gameObject.SetActive(false);
        }
    }

    public void UpdateSteminaColor()
    {
        if (GameManager.Instance.CurrentPlayer.CurrentStemina / GameManager.Instance.CurrentPlayer.MaxStemina <= 0.3f)
        {
            steminaSlider.fillRect.GetComponent<Image>().color = Color.red;
        }
        else
        {
            steminaSlider.fillRect.GetComponent<Image>().color = Color.white;
        }
    }

    private void UpdateTimeText()
    {
        int minutes = Mathf.FloorToInt(GameManager.Instance.currentGameTime/60);
        int seconds = Mathf.FloorToInt(GameManager.Instance.currentGameTime % 60);
        timerText.text = $"{minutes:D2}:{seconds:D2}";
    }
    private void OnExitButton(string _sceneName)
    {
        Time.timeScale = 1;
        NextSceneManager.Instance.nextSceneName = _sceneName;
        SceneManager.LoadScene("Loading");
            
    }

    private void InitCharacterList()
    {
        for (int i = 0; i < characterListUIs.Count; i++)
        {
            characterListUIs[i].slider.maxValue = GameManager.Instance.players[i].player.PlayerHealth.Maxhealth;
            characterListUIs[i].slider.value = GameManager.Instance.players[i].player.PlayerHealth.Maxhealth;
            //Debug.Log(i);
        }
    }
    public void UpdateCharacterList()
    {
        int number = GameManager.Instance.SelectNumber - 1;
        characterListUIs[number].slider.value =
            GameManager.Instance.players[number].player.PlayerHealth.CurrentHealth;
        if (characterListUIs[number].slider.value <= 0f)
        {
            OnDieCharacterList();
        }

    }
    public void OnDieCharacterList()
    {
        int number = GameManager.Instance.SelectNumber - 1;
        if (characterListUIs[number].slider.value <= 0f)
        {
            characterListUIs[number].dieCharacterIcon.sprite = GameManager.Instance.players[number].playerIcon;
            characterListUIs[number].listBackGround.sprite = unselectedBackGroundSprite;
            characterListUIs[number].listBackGround.GetComponent<Image>().color = characterListUIs[number].dieBackGroundColor;
            characterListUIs[number].aliveChildObject.gameObject.SetActive(false);
            characterListUIs[number].dieChildObject.gameObject.SetActive(true);
            characterListUIs[number].rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, 182f);
            
        }
    }
}
