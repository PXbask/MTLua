using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LoadingUI : MonoBehaviour
{
    [SerializeField] Image progressValue;
    [SerializeField] Text progressText;
    [SerializeField] GameObject progressBar;
    [SerializeField] Text progressDesc;
    float Max;
    public void InitProgress(float max,string desc)
    {
        Max = max;
        progressBar.SetActive(true);
        progressDesc.gameObject.SetActive(true);
        progressDesc.text = desc;
        progressValue.fillAmount = max > 0 ? 0 : 100;
        progressText.gameObject.SetActive(max > 0);
    }
    public void UpdateProgress(float progress)
    {
        progressValue.fillAmount = progress / Max;
        progressText.text = string.Format("{0:0}%", progressValue.fillAmount * 100);
    }
}
