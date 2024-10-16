////////////////////////////////////////////////////////////////////////////////////////////////////////
//FileName: SpeedAttackSlider.cs
//FileType: C# Source file
//Description : This script is used to handle Speed attack slider
////////////////////////////////////////////////////////////////////////////////////////////////////////
using UnityEngine;
using UnityEngine.UI;

public class SpeedAttackSlider : MonoBehaviour
{
    public static SpeedAttackSlider instance;                       //Script instance

    public Slider _slider;                                          //Sldier reference
    [SerializeField] private Text _attackText;                      //Attack text object reference
    [SerializeField] private Image _fillImage;                      //Image to reflact fill percentage
    [SerializeField] private Text _attackValueText;                 //Attack value text object  reference

    public SliderAttack _sliderAttack;                              //Attack type - light,medium or heavy
    float difference = 0;                                           //Used to dicide attack type when user changes slider value
    /// <summary>
    /// Set instance
    /// </summary>
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
    }
/// <summary>
/// Setup slider data
/// </summary>
    void OnEnable()
    {
        _slider = PVPManager.Get().speedAttackSlider;
        float speedPercentag = PVPManager.manager.p1Speed * 10f;

        _slider.maxValue = Mathf.Min(PVPManager.manager.P1RemainingHandHealth, PVPManager.manager.P2RemainingHandHealth);
        float perSpeedPoint = _slider.maxValue * 0.1f;
        float max = perSpeedPoint * PVPManager.manager.p1Speed;

        //float max = (int)_slider.maxValue*(speedPercentag/100);
        _slider.maxValue = max;
        difference = _slider.maxValue - _slider.minValue;
        _slider.minValue = 1;
        _slider.value = _slider.minValue;
        _attackValueText.text = _slider.minValue.ToString();
    }
    /// <summary>
    /// Set slider reference object
    /// </summary>
    private void Start()
    {
        _slider = GetComponent<Slider>();
    }

    /// <summary>
    /// Decide the type of current attack with respect to selected slider value
    /// </summary>
    void Update()
    {
        if (_slider.value <= (_slider.minValue + (difference * .33)))
        {
            _attackText.text = "Light Attack ";
            _attackText.color = Color.green;
            _fillImage.color = Color.green;
            _sliderAttack = SliderAttack.LightAttack;
        }
        else if (_slider.value >= (_slider.minValue + (difference * .34)) && _slider.value <= (_slider.minValue + (difference * .66)))
        {
            _attackText.text = "Medium Attack";
            _attackText.color = Color.yellow;
            _fillImage.color = Color.yellow;
            _sliderAttack = SliderAttack.MediumAttack;
        }
        else if (_slider.value >= (_slider.minValue + (difference * .67)))
        {
            _attackText.text = "Heavy Attack";
            _attackText.color = Color.red;
            _fillImage.color = Color.red;
            _sliderAttack = SliderAttack.HeavyAttack;
        }
    }
    /// <summary>
    /// Attack button click
    /// </summary>
    public void btn_SliderComplete()
    {
        PVPManager.manager.isAttackViaSpeedPoints = true;
        //PVPManager.Get().sliderAttackbuttonClick(_slider.value);

        PVPManager.Get().AttackChoices.SetActive(false);
        PVPManager.Get().speedAttackChoices.SetActive(false);
    }
    /// <summary>
    /// Updates attack value text
    /// </summary>
    public void UpdateAttackValueText()
    {
        _attackValueText.text = Mathf.RoundToInt(_slider.value).ToString();
    }
}

