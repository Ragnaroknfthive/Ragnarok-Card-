////////////////////////////////////////////////////////////////////////////////////////////////////////
//FileName: AttackSlider.cs
//FileType: C# Source file
//Description : This script is used to manage attack done in poker game
////////////////////////////////////////////////////////////////////////////////////////////////////////
using UnityEngine;
using UnityEngine.UI;
using System;
using TMPro;
/// <summary>
/// Attack type in poker- Light, Medium or Heavy
/// </summary>
[System.Serializable]
public enum SliderAttack
{
    nun,
    LightAttack,
    MediumAttack,
    HeavyAttack,
}

public class AttackSlider : MonoBehaviour
{
    public static AttackSlider instance;                //Script instance

    public Slider _slider;                              //Attack slider reference
    [SerializeField] private Text _attackText;          //Attack text object
    [SerializeField] private Image _fillImage;          //Image used to display fill percentage in for attack slider
    [SerializeField] private Text _attackValueText;     //Attack value text object

    public SliderAttack _sliderAttack;                  //Attack type
    public GameObject UseSpeedObj;                      //Speed object reference
    public RectTransform SpeedFill;                     //Rectransform used to adjust fill percentage object for speed
    public RectTransform AttackFill;                    //Rectransform used to adjust fill percentage object for attack
    float ExtraSpeedAmt;                                //Extra speed amount allowed
    public TextMeshProUGUI speedTx;                     //Speed text object
    public TextMeshProUGUI UsedSpeedTx;                 //Used speed text object
    public GameObject UsedSpeedButton;                  //Speed button
    private PlayerAction action;                        //Player action in poker

    /// <summary>
    /// Set action reference
    /// </summary>
    public void setAction(PlayerAction action)
    {
        this.action = action;
    }
    /// <summary>
    /// Used to calculate differentiate heavy, medium and light attack
    /// </summary>
    float difference = 0;

    bool SpeedCanceled, SpeedFixed;                                     //Boolean used for speed related logic
  
    /// <summary>
    /// Set instance
    /// </summary>
    private void Awake()
    {
        if(instance == null)
        {
            instance = this;
        }
        instance.gameObject.transform.parent.gameObject.SetActive(false);
    }
    /// <summary>
    /// Setup slider value and speed realted logic
    /// </summary>
    void OnEnable()
    {
        if(PVPManager.manager.P1StaVal < 10f) 
        {
            PVPManager.manager.ShowLowStaminaPopup();
        }
       
        _slider.maxValue = (Mathf.Min(PVPManager.manager.P1StaVal * 10f,Mathf.Min(PVPManager.manager.P1RemainingHandHealth,PVPManager.manager.P2RemainingHandHealth))) / 2;
        if(action == PlayerAction.counterAttack)
        {
            if(PVPManager.manager.P2LastAttackValue >= PVPManager.manager.P2RemainingHandHealth)
            {
                _slider.maxValue = Mathf.Max(_slider.maxValue,PVPManager.manager.P1RemainingHandHealth);
            }
        }
        if(_slider.maxValue>= (2 * PVPManager.manager.P1RemainingHandHealth) || _slider.maxValue*2> PVPManager.manager.P1RemainingHandHealth) 
        {
            _slider.maxValue =Mathf.RoundToInt( PVPManager.manager.P1RemainingHandHealth / 2);
        }
       
        difference = (_slider.maxValue / 2) - (_slider.minValue / 2);
        _slider.minValue = Game.Get().lastAction == PlayerAction.attack || Game.Get().lastAction == PlayerAction.counterAttack ? (int)(PVPManager.manager.LastAtkAmt / 2) : 1;
        //_slider.minValue = action == PlayerAction.counterAttack ? ((int)(PVPManager.manager.LastAtkAmt / 2)) + 1 : _slider.minValue;
        _slider.minValue = action == PlayerAction.counterAttack ? ((int)(PVPManager.manager.LastAtkAmt / 2)) : _slider.minValue;
        _slider.value = _slider.minValue;
        _attackValueText.text = (_slider.minValue * 2).ToString();
        SpeedCanceled = false;
        SpeedFixed = false;
        ExtraSpeedAmt = 0;

        SpeedFill.anchorMax = Vector2.zero;
        UsedSpeedButton.SetActive(false);
        _slider.onValueChanged.AddListener(delegate { UpdateAttackValueText(); });

        Debug.LogError("Slider Max Val" + _slider.maxValue);
       
    }
    /// <summary>
    /// Remove event listner
    /// </summary>
    void OnDisable()
    {
        _slider.onValueChanged.RemoveListener(delegate { UpdateAttackValueText(); });
    }
    /// <summary>
    /// Set slider reference
    /// </summary>
    private void Start()
    {
        _slider = GetComponent<Slider>();
    }
    /// <summary>
    /// Attack type decided from here when user changes slider value
    /// </summary>
    void Update()
    {

        if(_slider.value <= (_slider.minValue + (difference * .33)))
        {
            _attackText.text = "Light Attack ";
            _attackText.color = Color.green;
            _fillImage.color = Color.green;
            _sliderAttack = SliderAttack.LightAttack;
        }
        else if(_slider.value >= (_slider.minValue + (difference * .34)) && _slider.value <= (_slider.minValue + (difference * .66)))
        {
            _attackText.text = "Medium Attack";
            _attackText.color = Color.yellow;
            _fillImage.color = Color.yellow;
            _sliderAttack = SliderAttack.MediumAttack;
        }
        else if(_slider.value >= (_slider.minValue + (difference * .67)))
        {
            _attackText.text = "Heavy Attack";
            _attackText.color = Color.red;
            _fillImage.color = Color.red;
            _sliderAttack = SliderAttack.HeavyAttack;
        }
    }
    //Not in use
    public void UpdateUI(int value)
    {

    }

    /// <summary>
    ///  Extra speed use decision execution logic- User choose to use extra speed or not and then this function is executed
    /// </summary>
    /// <param name="ans">Answer string - Use extra speed or not to use extra speed</param>
    public void SpeedDecision(string ans)
    {
        ExtraSpeedAmt = ans == "Use" ? MathF.Min(PVPManager.Get().p1Speed * 10f,(_slider.value * 2)) : 0;
        if(ExtraSpeedAmt <= 0)
        {
            SpeedCanceled = true;
            UsedSpeedButton.SetActive(false);
        }
        else
        {
            //  UsedSpeedButton.SetActive(true);
            UsedSpeedTx.text = ExtraSpeedAmt + " Speed \nUsed";
            //  SpeedFill.anchorMax = new Vector2( ExtraSpeedAmt / (_slider.maxValue / 2), 1f);

            SpeedFixed = true;
            _slider.value = ExtraSpeedAmt / 2;
            btn_SliderComplete();
        }

        UseSpeedObj.SetActive(false);
    }

    /// <summary>
    /// Attack button click functionality
    /// </summary>
    public void btn_SliderComplete()
    {
        if(_slider.value > 0)
        {
            PVPManager.manager.HideLowStamina();
            Game.Get().lastAction = action;
            Game.Get().UpdateLastAction(action);

            PVPManager.Get().sliderAttackbuttonClick((int)(_slider.value * 2),Mathf.RoundToInt(((_slider.value * 2) - ExtraSpeedAmt) / 10f),action);
            PVPManager.Get().UpdateRemainingHandHealth((int)(_slider.value * 2) - Mathf.RoundToInt(ExtraSpeedAmt));
            PVPManager.Get().DeductSpeed(MathF.Round(ExtraSpeedAmt / 10f,1));
        }
        else
        {
            PokerButtonManager.instance.Check();
        }

        PVPManager.Get().AttackChoices.SetActive(false);
        PVPManager.Get().speedAttackChoices.SetActive(false);
    }
    /// <summary>
    /// Attack value text update at runtime
    /// </summary>
    public void UpdateAttackValueText()
    {

        float speedVal = PVPManager.Get().p1Speed * 10f;
        if(PVPManager.Get().p1Speed > 0 && !SpeedCanceled && !SpeedFixed)
        {

            if(_slider.value <= speedVal)
            {
                //_slider.value = _slider.value;
                //  _slider.fillRect = SpeedFill;
                //  AttackFill.anchorMax = Vector2.zero;

                int max = Mathf.RoundToInt(MathF.Min(PVPManager.Get().p1Speed * 10f,(_slider.maxValue * 2)));

                ExtraSpeedAmt = Mathf.RoundToInt((_slider.value * 2));
                if(ExtraSpeedAmt > max)
                {
                    ExtraSpeedAmt = max;
                }
                speedTx.text = "Use " + ExtraSpeedAmt + "\nFree Speed?";
                UseSpeedObj.SetActive(true);
            }
            else
            {
                //  _slider.fillRect = AttackFill;
            }
        }
        else
        {
            if(SpeedFixed)
            {
                //  _slider.value = Mathf.Clamp(_slider.value, ExtraSpeedAmt, (_slider.maxValue / 2));
                UsedSpeedButton.SetActive(true);
                UsedSpeedTx.text = ExtraSpeedAmt + " Speed \nUsed";
            }
            else
            {
                // SpeedFill.anchorMax = Vector2.zero;
            }
            // _slider.fillRect = AttackFill;
            UseSpeedObj.SetActive(false);
        }

        if(_slider.value > speedVal && speedVal > 0 && !SpeedCanceled && !SpeedFixed)
        {
            ExtraSpeedAmt = speedVal;
            speedTx.text = "Use " + ExtraSpeedAmt + "\nFree Speed?";
            UseSpeedObj.SetActive(true);
            // SpeedFill.anchorMax = new Vector2(speedVal / (_slider.maxValue / 2), 1f);
        }

        _attackValueText.text = ((int)(_slider.value * 2)).ToString();

        if(_slider.value >= _slider.maxValue && !(speedVal > 0 && !SpeedCanceled))
        {
            Debug.LogError("OTHER PLAYER HEALTH " + PVPManager.Get().P2HealthBar.maxValue);
            if(PVPManager.Get().P2HealthBar.maxValue >= PVPManager.Get().P2HealthBar.maxValue)
            { _attackValueText.text = "All In"; }
        }
    }
}
