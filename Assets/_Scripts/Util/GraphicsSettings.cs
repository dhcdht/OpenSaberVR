using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;

public static class GraphicsSettings
{
    public static UnityEvent BloomChanged = new UnityEvent();
    public static UnityEvent ColorGradingChanged = new UnityEvent();
    public static UnityEvent PostProcessingChanged = new UnityEvent();

    static bool GetBool(string name) {
        return PlayerPrefs.GetInt(name, 0) == 1;
    }

    public static bool IsBloomEnabled
    {
        get
        {
            return GetBool(PrefConstants.UseBloom);
        }
    }

    public static bool IsColorGradingEnabled
    {
        get
        {
            return GetBool(PrefConstants.UseColorGrading);
        }
    }

    public static bool IsPostProcessingEnabled
    {
        get
        {
            return GetBool(PrefConstants.UsePostProcessing);
        }
    }

    static void TryChangeBool(string name, bool value, UnityEvent e) {
        if (GetBool(name) != value) {
            PlayerPrefs.SetInt(name, value? 1 : 0);
            e.Invoke();
        }
    }

    public static void SetBloom(bool enabled) {
        TryChangeBool(PrefConstants.UseBloom, enabled, BloomChanged);
    }

    public static void SetColorGrading(bool enabled) {
        TryChangeBool(PrefConstants.UseColorGrading, enabled, ColorGradingChanged);
    }

    public static void SetPostProcessing(bool enabled) {
        TryChangeBool(PrefConstants.UsePostProcessing, enabled, PostProcessingChanged);
    }
}