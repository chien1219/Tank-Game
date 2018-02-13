using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WaveSlider : MonoBehaviour {

    public static WaveSlider instance;

    public Slider mSlider;
    public Text mText;

    public int waveValue;

    public void Start()
    {
        //Adds a listener to the main slider and invokes a method when the value changes.
        instance = this;
        waveValue = (int)mSlider.value;
        mText.text = mSlider.value.ToString();
        mSlider.onValueChanged.AddListener(delegate { ValueChangeCheck(); });
    }

    // Invoked when the value of the slider changes.
    public void ValueChangeCheck()
    {
        waveValue = (int)mSlider.value;
        mText.text = mSlider.value.ToString();
    }
}
