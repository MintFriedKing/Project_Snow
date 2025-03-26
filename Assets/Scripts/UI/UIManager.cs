using PS;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
[Serializable]
public class CharacterListUI //게임 매니저에서 할당한 계획
{
    public RectTransform rectTransform;
    public Slider slider;
    public Image characterIcon;
    public Image listBackGround;
    public Image selecIcon;
}
public class UIManager : MonoBehaviour
{
   
    [SerializeField, Header("조준점 UI")]
    private Crosshair crosshair;
    [SerializeField, Header("쉴드바 ")]
    private ShieldBar shieldBar;
    
    public static UIManager Instance;
    public TMP_Text currentAmmoText;
    public List<CircleSlider> circleSliders;   
    public TMP_Text nomalSkilltext;
    public Image playerStatusIcon;
    public Image playerNormalSkillIcon;
    public Sprite selectedBackGroundSprite;
    public Sprite unselectedBackGroundSprite;
    public List<CharacterListUI> characterListUIs;
    public ShieldBar ShieldBar { get { return shieldBar; }  set { shieldBar = value; } }
    private void Awake()
    {
        Init();
        nomalSkilltext.gameObject.SetActive(false);
        GameManager gameManager = GameManager.Instance;
        playerStatusIcon.sprite = gameManager.players[gameManager.SelectNumber - 1].playerIcon;
        playerNormalSkillIcon.sprite =gameManager.players[gameManager.SelectNumber - 1].skillIcon;
        ChangeCharacterList();

    }
    void Update()
    { 
        UpdateAimUI();
        currentAmmoText.text = GameManager.Instance.CurrentPlayer.PlayerShootManager.gun.CurrentAmmo.ToString();
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

       
      

    }
    private void Init()
    {
        Instance = this;
      
    }
    private void UpdateAimUI()
    { 
        crosshair.SetActiveCrosshair(true);
        crosshair.UpdatePosition(Player.Instance.PlayerShootManager.AimPosition);
    }
    public void ChangeCharacterList()
    {
        for (int i = 0; i < characterListUIs.Count; i++)
        {
            characterListUIs[i].slider.value = 1f;
            characterListUIs[i].characterIcon.sprite = GameManager.Instance.players[i].playerIcon;
            characterListUIs[i].rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, 400f);
            characterListUIs[i].listBackGround.sprite = unselectedBackGroundSprite; 
            characterListUIs[i].selecIcon.gameObject.SetActive(false);
        }
        characterListUIs[GameManager.Instance.SelectNumber-1].rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, 500f);
        characterListUIs[GameManager.Instance.SelectNumber - 1].listBackGround.sprite = selectedBackGroundSprite;
        characterListUIs[GameManager.Instance.SelectNumber - 1].selecIcon.gameObject.SetActive(true);
    }
    public void ChangeSkillSprite()
    {
        playerNormalSkillIcon.sprite = GameManager.Instance.players[GameManager.Instance.SelectNumber - 1].skillIcon;
    }
  
}
