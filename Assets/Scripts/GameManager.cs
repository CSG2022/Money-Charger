using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class GameManager : MonoBehaviour
{
    #region Static Variables
    public static float CashAmount;
    public static TMP_Text CashAmountTxt;
    public static int CashPerSecond;
    public static Material BatteryFill;
    public static Material CablePowerMaterial;
    public static float PassiveIncomeDelayInSeconds;
    public static bool draggingNewSwitch;
    [SerializeField] public static float _batteryFillStartingIncrement = 0.01f;
    [SerializeField] public static float _emptyBatteryFillValue = 0.0f;
    [SerializeField] public static float _fullBatteryFillValue = 1.7f;
    [SerializeField] public static Draggable[] SwitchButtonParents;
    [SerializeField] public static Draggable[] SwitchButtonParentsActive;
    [SerializeField] int _upgradeBatteryCost = 200;
    [SerializeField] TMP_Text _upgradeBatteryText;
    public static Material ButtonActiveMaterial;
    public static Material[] BoxLevel1Materials;
    public static Material[] BoxLevel2Materials;
    public static Material[] BoxLevel3Materials;
    public static bool ButtonFrienzy;
    #endregion

    #region TextComponents
    [SerializeField] TMP_Text _cashAmountPrivateTxt;
    [SerializeField] TMP_Text _buySwitchTxt;
    #endregion

    #region Range and Value Variables
    [SerializeField] [Range (0, 2)] float _passiveIncomeDelayInSeconds = .5f;
    [SerializeField] [Range(0,100)] int _cashPerSecond = 1;
    [SerializeField] float _spawnCoinDelay = .45f;
    [SerializeField] float _maxKolicineStruje = 60; // 0 - 100
    int batteryUpgrade = 0;
    [SerializeField] int switchBuyCost = 50;
    #endregion

    #region Coin Transform Components
    [SerializeField] Transform _coinSpawnPosition;
    [SerializeField] Transform _coinGoToPosition;
    [SerializeField] Transform _coinHidePosition;
    #endregion

    #region Materials
    [SerializeField] Material _material;
    [SerializeField] Material _lightningSignMaterial;
    [SerializeField] Material _cablePowerMaterial;
    [SerializeField] Material[] _batteryLvl2Materials;
    [SerializeField] Material[] _batteryLvl3Materials;
    [SerializeField] Material[] _activeCableMaterials;
    [SerializeField] Material[] _inactiveCableMaterials;
    [SerializeField] Material _buttonActiveMaterial;
    [SerializeField] Material _buttonInactiveMaterial;
    [SerializeField] Material[] _boxLevel1Materials;
    [SerializeField] Material[] _boxLevel2Materials;
    [SerializeField] Material[] _boxLevel3Materials;
    [SerializeField] Material _buttonFrienzyMaterial;
    #endregion

    #region OtherTypes
    [SerializeField] GameObject _coinPrefab;
    [SerializeField] public Mesh BatteryUpgradeLvl2;
    [SerializeField] public Mesh BatteryUpgradeLvl3;
    [SerializeField] Battery _battery;
    [SerializeField] Button _buyNewSwitchButton;
    [SerializeField] bool _startWithSoundMuted;
    [SerializeField] public Draggable[] SwitchButtonParentsDynamic;
    [SerializeField] public MeshRenderer[] CableMeshRenderers;
    [SerializeField] GameObject _lvl2BatteryUpgradeImg;
    [SerializeField] GameObject _lvl3BatteryUpgradeImg;
    [SerializeField] GameObject _lvl1SwitchBuyImg;
    [SerializeField] GameObject _lvl2SwitchBuyImg;
    [SerializeField] GameObject _debugMenu;
    [SerializeField] Sprite _batteryCanBuy;
    [SerializeField] Sprite _batteryCannotBuy;
    [SerializeField] Sprite _switchCanBuy;
    [SerializeField] Sprite _switchCannotBuy;
    [SerializeField] Image _batteryButtonImage;
    [SerializeField] Image _switchButtonImage;
    [SerializeField] Transform _cameraPos1;
    [SerializeField] Transform _cameraPos2;
    ClickerBehaviour _clickerBehaviour;
    [SerializeField] CoinPool _coinPool;
    #endregion

    #region Audio
    public static AudioSource SwitchButtonSound;
    public static AudioSource BatteryBzzSound;
    public static AudioSource CableFillingSound;
    [SerializeField] AudioSource _menuButtonSound;
    [SerializeField] AudioSource _cableFillingSound;
    [SerializeField] AudioSource _switchButtonSound;
    [SerializeField] AudioSource _batteryFilledSound;
    #endregion

    #region Start and Update
    void Start()
    {
        BoxLevel1Materials = _boxLevel1Materials;
        BoxLevel2Materials = _boxLevel2Materials;
        BoxLevel3Materials = _boxLevel3Materials;
        ButtonActiveMaterial = _buttonActiveMaterial;
        CableFillingSound = _cableFillingSound;
        BatteryBzzSound = _batteryFilledSound;
        SwitchButtonSound = _switchButtonSound;
        _clickerBehaviour = GetComponent<ClickerBehaviour>();
        SwitchButtonParents = SwitchButtonParentsDynamic;
        SwitchButtonParentsActive = SwitchButtonParents.Where(x => x.isActiveAndEnabled).ToArray();
        AudioListener.volume = 0.6f;
        AudioListener.pause = SoundManager.SoundIsOn ? false : true;
        

        BatteryFill = _material;
        CablePowerMaterial = _cablePowerMaterial;
        CashAmount = 0;
        PassiveIncomeDelayInSeconds = _passiveIncomeDelayInSeconds;
        CashPerSecond = _cashPerSecond;
        CashAmountTxt = _cashAmountPrivateTxt;
        
        StartCoroutine(PassiveIncome());
        StartCoroutine(ChangeActiveSwitchButton());
        
    }


    private void Update()
    {
        if(CashAmount < 1100)
            CashAmountTxt.text = String.Format("{0}",CashAmount.ToString("F1"));
        else if(CashAmount < 1000000)
        {
            CashAmountTxt.text = String.Format("{0}K", (CashAmount / 1000).ToString("F1"));
        }
        else
            CashAmountTxt.text = String.Format("{0}M",(CashAmount / 1000000).ToString("F1"));

                
        _batteryButtonImage.sprite = CashAmount > _upgradeBatteryCost ?  _batteryCanBuy : _batteryCannotBuy;
        _switchButtonImage.sprite = CashAmount > switchBuyCost ?  _switchCanBuy : _switchCannotBuy;
    }
    #endregion

    #region AllCoroutines
    IEnumerator PassiveIncome()
    {
        while(true)
        {
            // This is called when the battery fills up
            if (BatteryFill.GetFloat("Vector1_bf3afcb57e66442ebfa625df53b0c77e") >= _fullBatteryFillValue)
            {
                BatteryFill.SetFloat("Vector1_bf3afcb57e66442ebfa625df53b0c77e", _emptyBatteryFillValue);
                _batteryFilledSound.Play();
                _lightningSignMaterial.SetFloat("kolicina_struje", 0);
                if (SoundManager.VibrationIsOn)
                    Handheld.Vibrate();
                StartCoroutine(SpawnCoins());
            }
            float newFillValue = BatteryFill.GetFloat("Vector1_bf3afcb57e66442ebfa625df53b0c77e") + _batteryFillStartingIncrement;
            BatteryFill.SetFloat("Vector1_bf3afcb57e66442ebfa625df53b0c77e", newFillValue);
            var percent = BatteryFill.GetFloat("Vector1_bf3afcb57e66442ebfa625df53b0c77e") / 100;
            if(_lightningSignMaterial.GetFloat("kolicina_struje") < _maxKolicineStruje)
            _lightningSignMaterial.SetFloat("kolicina_struje", _lightningSignMaterial.GetFloat("kolicina_struje") + percent * _maxKolicineStruje);
            yield return new WaitForSeconds(PassiveIncomeDelayInSeconds);
        }
    }

    IEnumerator SpawnCoins()
    {
        for(var i = 0; i < 20; i++)
        {
            //var coin = Instantiate(_coinPrefab, _coinSpawnPosition);
            var coin = _coinPool.GetPooledObject();
            if(coin != null)
            {
                Coin coinComponent = coin.GetComponent<Coin>();
                coin.transform.position = coinComponent.coinInstantiatePosition;
                coinComponent.StartingTransform = _coinGoToPosition;
                coinComponent.HiddenTransform = _coinHidePosition;
                coin.SetActive(true);
                coinComponent.InitiateTurningOff = true;
            }
            CashAmount += CashPerSecond;
            yield return new WaitForSeconds(_spawnCoinDelay);
        }

    }

    IEnumerator ChangeActiveSwitchButton()
    {
        while(true)
        {
            if(GameManager.ButtonFrienzy)
            {
                for (int i = 0; i < SwitchButtonParentsActive.Length; i++)
                {
                    CableMeshRenderers[i].materials = _activeCableMaterials;
                    SwitchButtonParentsActive[i]._clickableButton.Working = true;
                    SwitchButtonParentsActive[i]._clickableButton.GetComponent<MeshRenderer>().material = _buttonFrienzyMaterial;
                }
                yield return new WaitUntil(() => !ButtonFrienzy );
            }
            else
            {
                var turnOnThisOne = Random.Range(0, SwitchButtonParentsActive.Length);     

                for(int i = 0; i < SwitchButtonParentsActive.Length; i++)
                {
                    // Change button material and switch some property to Inactive
                    CableMeshRenderers[i].materials = _inactiveCableMaterials;
                    SwitchButtonParentsActive[i]._clickableButton.Working = false;
                    SwitchButtonParentsActive[i]._clickableButton.GetComponent<MeshRenderer>().material = _buttonInactiveMaterial;
                }

                //CableMeshRenderers[turnOnThisOne].materials = _activeCableMaterials; // Deprecate this
                SwitchButtonParentsActive[turnOnThisOne]._clickableButton.CableMeshRenderer.materials = _activeCableMaterials; // into this
                SwitchButtonParentsActive[turnOnThisOne]._clickableButton.Working = true;
                SwitchButtonParentsActive[turnOnThisOne]._clickableButton.GetComponent<MeshRenderer>().material = _buttonActiveMaterial;
                yield return ButtonFrienzy ? null : new WaitForSeconds(5f);
                _clickerBehaviour.ResetElectricImpulse();
            }
        }
    }

    IEnumerator InitButtonFrienzy()
    {
        yield return new WaitForSeconds(5f);
        ButtonFrienzy = false;
    }
    #endregion

    #region DebugMenu
    public void ShowDebugMenu()
    {
        if(!_debugMenu.activeInHierarchy)
        {
            _debugMenu.SetActive(true);
        }
        _menuButtonSound.time = .3f;
        _menuButtonSound.Play();
    }
    public void HideDebugMenu()
    {
        if(_debugMenu.activeInHierarchy)
        {
            _debugMenu.SetActive(false);
        }
        _menuButtonSound.time = .3f;
        _menuButtonSound.Play();
    }

    public void ChangeCameraPosition()
    {
        _menuButtonSound.time = .3f;
        _menuButtonSound.Play();
        var cam = Camera.main;
        if(cam.transform.position == _cameraPos1.position)
        {
            cam.transform.position = _cameraPos2.position;
            cam.transform.rotation = _cameraPos2.rotation;
        }
        else
        {
            cam.transform.position = _cameraPos1.position;
            cam.transform.rotation = _cameraPos1.rotation;
        }
    }

    public void AquireMoney()
    {
        _menuButtonSound.time = .3f;
        _menuButtonSound.Play();
        CashAmount = 999990;
    }
    
    public void SpeedUpBat()
    {
        _menuButtonSound.time = .3f;
        _menuButtonSound.Play();
        PassiveIncomeDelayInSeconds -= .01f;
        if (PassiveIncomeDelayInSeconds <= 0)
            PassiveIncomeDelayInSeconds = 0;
    }
    public void SlowDownBat()
    {
        _menuButtonSound.time = .3f;
        _menuButtonSound.Play();
        PassiveIncomeDelayInSeconds += .01f; 
    }
    #endregion

    #region SpendMoney
    public void UpgradeBattery()
    {
        if(CashAmount > _upgradeBatteryCost)
        {
            _menuButtonSound.time = .3f;
            _menuButtonSound.Play();
            if (batteryUpgrade == 0)
            {
                _battery.MeshFilter.mesh = BatteryUpgradeLvl2;
                _battery.MeshRenderer.materials = _batteryLvl2Materials;
                _lvl3BatteryUpgradeImg.SetActive(true);
                _lvl2BatteryUpgradeImg.SetActive(false);
                batteryUpgrade++;
                CashAmount -= _upgradeBatteryCost;
                _upgradeBatteryCost += 800;
                _upgradeBatteryText.text = "1000";
            }
            else if(batteryUpgrade == 1)
            {
                _battery.MeshFilter.mesh = BatteryUpgradeLvl3;
                _battery.MeshRenderer.materials = _batteryLvl3Materials;
                batteryUpgrade++;
                CashAmount -= _upgradeBatteryCost;
                _upgradeBatteryText.text = "Max";
            }
        }
    }

    public void BuySwitch()
    {
        if(CashAmount >= switchBuyCost)
        {
            _menuButtonSound.time = .3f;
            _menuButtonSound.Play();
            if(SwitchButtonParentsActive.Length < SwitchButtonParents.Length)
            {
            // If there are unactive switches activate one of them and skidaddle
                foreach(var parent in SwitchButtonParents)
                {
                    if(!parent.isActiveAndEnabled)
                    {
                        CashAmount -= switchBuyCost;
                        parent.gameObject.SetActive(true);
                        Draggable[] tmp = new Draggable[SwitchButtonParentsActive.Length + 1];
                        SwitchButtonParentsActive.CopyTo(tmp, 0);
                        tmp[SwitchButtonParentsActive.Length] = parent;
                        SwitchButtonParentsActive = tmp;
                        var btn = parent.GetComponentInChildren<SwitchButton>();
                        btn.GetComponent<MeshFilter>().mesh = _clickerBehaviour.Lvl1SwitchMesh;
                        btn.GetComponent<MeshRenderer>().material = _buttonInactiveMaterial;
                        btn.BoxMeshRenderer.materials = BoxLevel1Materials;
                        btn.Level = 0;
                        btn.amountPerClick = 0.015f;
                        btn.Working = false;
                        break;
                    }
                }
            }
            else if(SwitchButtonParentsActive.FirstOrDefault(x => x._clickableButton.Level == 0) != null)
            {
                _switchButtonSound.Play();
                // If there are active switches amongs which there is lvl1 switch just upgrade 1 of the lvl1 switches
                foreach (var parent in SwitchButtonParentsActive)
                {
                    if (parent._clickableButton.Level == 0)
                    {
                        CashAmount -= switchBuyCost;
                        var btn = parent.GetComponentInChildren<SwitchButton>();
                        btn.GetComponent<MeshFilter>().mesh = _clickerBehaviour.Lvl2SwitchMesh;
                        btn.BoxMeshRenderer.materials = BoxLevel2Materials;
                        btn.Level = 1;
                        btn.amountPerClick = 0.03f;
                        break;
                    }
                }
            }
            else if(SwitchButtonParentsActive.FirstOrDefault(x => x._clickableButton.Level == 1) != null)
            {
                _switchButtonSound.Play();
                // If there are active switches all above lvl1 then upgrade one switch lvl2
                foreach (var parent in SwitchButtonParentsActive)
                {
                    if (parent._clickableButton.Level == 1)
                    {
                        CashAmount -= switchBuyCost;
                        var btn = parent.GetComponentInChildren<SwitchButton>();
                        btn.GetComponent<MeshFilter>().mesh = _clickerBehaviour.Lvl3SwitchMesh;
                        btn.BoxMeshRenderer.materials = BoxLevel3Materials;
                        btn.Level = 2;
                        btn.amountPerClick = 0.05f;
                        break;
                    }
                }
            }

            

            // Check if all 3 are there and they are the same level - WOW! moment
            if(SwitchButtonParentsActive.Length > 2)
            {
                if (SwitchButtonParentsActive[0]._clickableButton.Level == SwitchButtonParentsActive[1]._clickableButton.Level 
                 && SwitchButtonParentsActive[0]._clickableButton.Level == SwitchButtonParentsActive[2]._clickableButton.Level)
                {
                    // Levels are same! Start a Coroutine that enables pressing on all 3 buttons via only 1 button and make coins fly out everywhere
                    // That will be enabled only for 5 seconds or so and then it gets disabled until a new set of switches are same level
                    // Maybe disable merging during the wow moment so it doesnt cause any bugs
                    ButtonFrienzy = true;
                    StartCoroutine(InitButtonFrienzy());
                }
            }

            // You bought smt now check if change is needed on the Buy SwitchButton
            if(SwitchButtonParentsActive.Length > 2 && SwitchButtonParentsActive.FirstOrDefault(x => x._clickableButton.Level == 0) == null)
            {
                // change graphic
                _lvl1SwitchBuyImg.SetActive(false);
                _lvl2SwitchBuyImg.SetActive(true);
                _buySwitchTxt.text = "100";
                switchBuyCost = 100;
            }
            else
            {
                _lvl2SwitchBuyImg.SetActive(false);
                _lvl1SwitchBuyImg.SetActive(true);
                _buySwitchTxt.text = "50";
                switchBuyCost = 50;
            }
        }
    }
    #endregion

    public void MainMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }
}
