using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SpeedAttackSlider : MonoBehaviour
{
    public static SpeedAttackSlider instance;

    public Slider _slider;
    [SerializeField] private Text _attackText;
    [SerializeField] private Image _fillImage;
    [SerializeField] private Text _attackValueText;

    public SliderAttack _sliderAttack;
    float difference = 0;
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
    }
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
    private void Start()
    {
        _slider = GetComponent<Slider>();
    }

    // Update is called once per frame
    void Update()
    {

        //if (_slider.value<=(_slider.maxValue*.33))
        //{
        //    _attackText.text = "Light Attack ";
        //    _attackText.color = Color.green;
        //    _fillImage.color = Color.green;
        //    _sliderAttack = SliderAttack.LightAttack;
        //}
        //else if (_slider.value>= (_slider.maxValue * .34) && _slider.value <= (_slider.maxValue * .66))
        //{
        //    _attackText.text = "Medium Attack";
        //    _attackText.color = Color.yellow;
        //    _fillImage.color = Color.yellow;
        //    _sliderAttack = SliderAttack.MediumAttack;
        //}
        //else if (_slider.value>= (_slider.maxValue * .67))
        //{
        //    _attackText.text = "Heavy Attack";
        //    _attackText.color = Color.red;
        //    _fillImage.color = Color.red;
        //    _sliderAttack = SliderAttack.HeavyAttack;
        //}
        //else
        //{
        //    _attackText.text = "nun";
        //    _sliderAttack = SliderAttack.nun;

        //}      


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
        //else
        //{
        //    _attackText.text = "nun";
        //    _sliderAttack = SliderAttack.nun;

        //}
    }

    public void btn_SliderComplete()
    {
        PVPManager.manager.isAttackViaSpeedPoints = true;
        //PVPManager.Get().sliderAttackbuttonClick(_slider.value);

        PVPManager.Get().AttackChoices.SetActive(false);
        PVPManager.Get().speedAttackChoices.SetActive(false);
    }
    public void UpdateAttackValueText()
    {
        _attackValueText.text = Mathf.RoundToInt(_slider.value).ToString();

        if (_slider.value == _slider.maxValue)
        {
            // _attackValueText.text = "All In";
        }
    }
}

