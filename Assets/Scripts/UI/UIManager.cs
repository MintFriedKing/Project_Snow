using PS;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;
    [SerializeField, Header("조준점 UI")]
    private Crosshair crosshair;
    [SerializeField, Header("쉴드바 ")]
    private ShieldBar shieldBar;
    public ShieldBar ShieldBar { get { return shieldBar; }  set { shieldBar = value; } }
    private void Awake()
    {
        Init();
    }

    void Update()
    { 
        UpdateAimUI();
        
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
}
