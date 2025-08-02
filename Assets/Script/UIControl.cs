using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections;

public class UIControl : MonoBehaviour
{
    public TMP_Text angleText;
    public Image backgroundImage;
    private float slideDuration = 0.2f;  //持續時間
    private Vector2 TextOffScreenPosition = new Vector2(1100, 10);
    private Vector2 TextOnScreenPosition = new Vector2(400,-145);
    private Vector2 ImgOffScreenPosition = new Vector2(1310, -110);
    private Vector2 ImgOnScreenPosition = new Vector2(400,-150);

    private RectTransform angleTextRectTransform;
    private RectTransform backgroundImageRectTransform;

    void Start()
    {
        angleTextRectTransform = angleText.GetComponent<RectTransform>();
        backgroundImageRectTransform = backgroundImage.GetComponent<RectTransform>();

        angleTextRectTransform.anchoredPosition = TextOffScreenPosition;
        backgroundImageRectTransform.anchoredPosition = ImgOffScreenPosition;
        backgroundImageRectTransform.sizeDelta = angleTextRectTransform.sizeDelta + new Vector2(20, 20);

        angleText.gameObject.SetActive(false);
        backgroundImage.gameObject.SetActive(false);
    }

    public void OnBallHit(string msg)
    {
        angleText.text = msg;
        StartCoroutine(SlideIn());
    }

    private IEnumerator DelayAction(float delayTime)
    {
        yield return new WaitForSeconds(delayTime);
    }

    private IEnumerator SlideIn()
    {
        angleText.gameObject.SetActive(true);
        backgroundImage.gameObject.SetActive(true);

        float elapsedTime = 0;

        //move
        while (elapsedTime < slideDuration)
        {
            elapsedTime += Time.deltaTime;
            float t = Mathf.Clamp01(elapsedTime / slideDuration);

            angleTextRectTransform.anchoredPosition = Vector2.Lerp(TextOffScreenPosition, TextOnScreenPosition, t);
            backgroundImageRectTransform.anchoredPosition = Vector2.Lerp(ImgOffScreenPosition, ImgOnScreenPosition, t);

            yield return null;
        }
    }

    //隐藏
    public void HideDisplay()
    {
        angleText.gameObject.SetActive(false);
        backgroundImage.gameObject.SetActive(false);
    }
}
