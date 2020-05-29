// Copyright (c) 2020 by Yuya Yoshino

using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class GammaController : MonoBehaviour
{
    [SerializeField] private Volume _volume = null;
    [SerializeField] private OptionsMenu _optionsMenu = null;

    private LiftGammaGain LGB;

    private void Awake()
    {
        LGB = _volume.GetComponent<LiftGammaGain>();
        LGB.gamma.value = new Vector4(1,1,1,_optionsMenu.GammaLevel);
    }

    public void ChangeGamma(float gammaValue)
    {
        LGB.gamma.value = new Vector4(1,1,1,gammaValue);
    }
}
