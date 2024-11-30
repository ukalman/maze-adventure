using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class StartGameSequence : MonoBehaviour
{
    [Header("UI Components")]
    [SerializeField] private GameObject firstText;
    [SerializeField] private GameObject secondText;
    [SerializeField] private GameObject thirdText;

    [SerializeField] private Button continueButton;
    [SerializeField] private TMP_Text continueButtonText;
    [SerializeField] private Image continueButtonImage;

    [Header("Audio")]
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip clickSound;

    private void Start()
    {
        SetOpacityToZero(firstText.GetComponent<TMP_Text>());
        SetOpacityToZero(secondText.GetComponent<TMP_Text>());
        SetOpacityToZero(thirdText.GetComponent<TMP_Text>());
        SetOpacityToZero(continueButtonText);
        SetOpacityToZero(continueButtonImage);

        firstText.SetActive(true);
        secondText.SetActive(false);
        thirdText.SetActive(false);

        continueButton.interactable = false;
        
        StartCoroutine(FadeInSequence(firstText.GetComponent<TMP_Text>(), continueButtonText, continueButtonImage));
    }

    private void SetOpacityToZero(Graphic graphic)
    {
        Color color = graphic.color;
        color.a = 0f;
        graphic.color = color;
    }

    public void OnContinueButtonClicked()
    {
        audioSource.PlayOneShot(clickSound);
        
        if (firstText.activeSelf)
        {
            StartCoroutine(SwitchToNextText(firstText, secondText));
        }
        else if (secondText.activeSelf)
        {
            StartCoroutine(SwitchToNextText(secondText, thirdText));
        }
        else if (thirdText.activeSelf)
        {
            StartCoroutine(LoadNextScene());
        }
    }

    private IEnumerator FadeInSequence(TMP_Text text, TMP_Text buttonText, Image buttonImage)
    {
        // Fade in the text and button components
        StartCoroutine(FadeInText(text, 1.0f));
        StartCoroutine(FadeInText(buttonText, 1.0f));
        StartCoroutine(FadeInImage(buttonImage, 1.0f));
        yield return new WaitForSeconds(1.0f);

        // Make the button interactable after fading in
        continueButton.interactable = true;
    }

    private IEnumerator SwitchToNextText(GameObject currentText, GameObject nextText)
    {
        yield return StartCoroutine(FadeOutText(currentText.GetComponent<TMP_Text>(), 1.0f));
        currentText.SetActive(false);


        nextText.SetActive(true);
        yield return StartCoroutine(FadeInSequence(nextText.GetComponent<TMP_Text>(), continueButtonText, continueButtonImage));
    }

    private IEnumerator FadeInText(TMP_Text text, float duration)
    {
        Color color = text.color;
        float startAlpha = color.a;
        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            color.a = Mathf.Lerp(startAlpha, 1f, elapsedTime / duration);
            text.color = color;
            yield return null;
        }

        color.a = 1f;
        text.color = color;
    }

    private IEnumerator FadeOutText(TMP_Text text, float duration)
    {
        Color color = text.color;
        float startAlpha = color.a;
        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            color.a = Mathf.Lerp(startAlpha, 0f, elapsedTime / duration);
            text.color = color;
            yield return null;
        }

        color.a = 0f;
        text.color = color;
    }

    private IEnumerator FadeInImage(Image image, float duration)
    {
        Color color = image.color;
        float startAlpha = color.a;
        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            color.a = Mathf.Lerp(startAlpha, 1f, elapsedTime / duration);
            image.color = color;
            yield return null;
        }

        color.a = 1f;
        image.color = color;
    }

    private IEnumerator LoadNextScene()
    {
        StartCoroutine(FadeOutText(thirdText.GetComponent<TMP_Text>(), 1.0f));
        StartCoroutine(FadeOutText(continueButtonText, 1.0f));
        StartCoroutine(FadeOutImage(continueButtonImage, 1.0f));
        yield return new WaitForSeconds(2.0f);

        UnityEngine.SceneManagement.SceneManager.LoadScene(1);
    }

    private IEnumerator FadeOutImage(Image image, float duration)
    {
        Color color = image.color;
        float startAlpha = color.a;
        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            color.a = Mathf.Lerp(startAlpha, 0f, elapsedTime / duration);
            image.color = color;
            yield return null;
        }

        color.a = 0f;
        image.color = color;
    }
}
