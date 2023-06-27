using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public interface IClient_UnitProperty
{
    HealthMagicPointShowUI HealthMagicPointShowUI { get; }
    public void SetShowUI(HealthMagicPointShowUI ui);
}
public interface IStartAfterNetwork 
{
    public void StartAfterNetwork();
}

public class HealthMagicPointShowUI : MonoBehaviour
{
    // Start is called before the first frame update
    //float healthPer=0;
    //float magicPer=0;
    Scrollbar healthScollbar;
    Scrollbar magicScollbar;
    Image healthHandle;
    Coroutine healthCoroutine;
    Coroutine magicCoroutine;
    Color showColor = default;
    float animationTakeTime = 0.5f;
    void Awake()
    {
        foreach (var v in GetComponentsInChildren<Scrollbar>())
        {
            if (v.gameObject.name.StartsWith("Health"))
                healthScollbar = v;
            else if (v.gameObject.name.StartsWith("Magic"))
                magicScollbar = v;
            if (healthScollbar != null && magicScollbar != null)
                break;
        }
        if (healthScollbar != null)
            healthHandle = healthScollbar.targetGraphic.GetComponent<Image>();
    }
    public void SetHandleColor(Color color)
    {
        if (color == this.showColor) return;
        this.showColor = color;
        gameObject.SetActive(true);
        StartCoroutine(WaitForHealthHandle());
    }
    public void SetHealthPer(float per, bool needAnimation = true)
    {
        //Debug.Log("血量更新+" + per+"|"+healthScollbar.size);

        if (healthScollbar == null || healthScollbar.gameObject.activeSelf == false) return;
        if (per == healthScollbar.size) return;
        //Debug.Log("血量更新+"+per);

        if (per < 0)
            healthScollbar.gameObject.SetActive(false);
        else
        {
            healthScollbar.gameObject.SetActive(true);
            //this.gameObject.SetActive(true);
            if (healthCoroutine != null)
                StopCoroutine(healthCoroutine);
            healthCoroutine = null;
            if (needAnimation)
                healthCoroutine = StartCoroutine(PointUIChnageSlerp(healthScollbar, per, animationTakeTime));
            else
                healthScollbar.size = per;
        }
    }
    public void SetMagicPer(float per, bool needAnimation = true)
    {
        if (magicScollbar == null || magicScollbar.gameObject.activeSelf == false) return;
        if (per == magicScollbar.size) return;

        if (per < 0)
            magicScollbar.gameObject.SetActive(false);
        else
        {
            //this.gameObject.SetActive(true);
            magicScollbar.gameObject.SetActive(true);
            if (magicCoroutine != null)
                StopCoroutine(magicCoroutine);
            magicCoroutine = null;
            if (needAnimation)
                magicCoroutine = StartCoroutine(PointUIChnageSlerp(magicScollbar, per, animationTakeTime));
            else
                magicScollbar.size = per;
        }
    }
    //private void OnDisable()
    //{
    //    StopAllCoroutines();
    //}
    IEnumerator PointUIChnageSlerp(Scrollbar scrollbar, float per, float takeTime)
    {
        if (scrollbar == null) yield break;
        float timer = 0;
        while (timer < takeTime)
        {
            timer += Time.deltaTime;
            scrollbar.size = Mathf.Lerp(scrollbar.size, per, takeTime * Time.deltaTime);
        }
    }
    IEnumerator WaitForHealthHandle()
    {
        float timer = 0;
        WaitUntil waitUntil = new WaitUntil(() => { timer += Time.deltaTime; return healthHandle != null || timer > 10; });
        yield return waitUntil;
        if (healthHandle != null)
            healthHandle.color = showColor;
    }

}

