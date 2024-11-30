using System;
using System.Collections;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class PlayerDiedWindowController : MonoBehaviour
{
    [SerializeField] private Image background;
    [SerializeField] private Image retryButtonBackground;
    [SerializeField] private Image mainMenuButtonBackground;

    [SerializeField] private TMP_Text youDiedTitle;
    [SerializeField] private TMP_Text youDiedText;
    [SerializeField] private TMP_Text retryButtonText;
    [SerializeField] private TMP_Text mainMenuButtonText;

    [SerializeField] private Button retryButton;
    [SerializeField] private Button mainMenuButton;
    
    private void OnEnable()
    {
        StartCoroutine(FadeInUI());
    }
    
    private IEnumerator FadeInUI()
    {
        retryButton.interactable = false;
        mainMenuButton.interactable = false;

        float fadeDuration = 2.0f;
        float elapsedTime = 0f;
        
        Color backgroundColor = background.color;
        Color retryButtonColor = retryButtonBackground.color;
        Color mainMenuButtonColor = mainMenuButtonBackground.color;
        Color titleColor = youDiedTitle.color;
        Color textColor = youDiedText.color;
        Color retryTextColor = retryButtonText.color;
        Color mainMenuTextColor = mainMenuButtonText.color;
        
        backgroundColor.a = 0;
        retryButtonColor.a = 0;
        mainMenuButtonColor.a = 0;
        titleColor.a = 0;
        textColor.a = 0;
        retryTextColor.a = 0;
        mainMenuTextColor.a = 0;
        
        background.color = backgroundColor;
        retryButtonBackground.color = retryButtonColor;
        mainMenuButtonBackground.color = mainMenuButtonColor;
        youDiedTitle.color = titleColor;
        youDiedText.color = textColor;
        retryButtonText.color = retryTextColor;
        mainMenuButtonText.color = mainMenuTextColor;
        
        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;
            float alpha = Mathf.Clamp01(elapsedTime / fadeDuration);

            backgroundColor.a = alpha;
            retryButtonColor.a = alpha;
            mainMenuButtonColor.a = alpha;
            titleColor.a = alpha;
            textColor.a = alpha;
            retryTextColor.a = alpha;
            mainMenuTextColor.a = alpha;
            
            background.color = backgroundColor;
            retryButtonBackground.color = retryButtonColor;
            mainMenuButtonBackground.color = mainMenuButtonColor;
            youDiedTitle.color = titleColor;
            youDiedText.color = textColor;
            retryButtonText.color = retryTextColor;
            mainMenuButtonText.color = mainMenuTextColor;

            yield return null;
        }
        
        backgroundColor.a = 1;
        retryButtonColor.a = 1;
        mainMenuButtonColor.a = 1;
        titleColor.a = 1;
        textColor.a = 1;
        retryTextColor.a = 1;
        mainMenuTextColor.a = 1;
        
        background.color = backgroundColor;
        retryButtonBackground.color = retryButtonColor;
        mainMenuButtonBackground.color = mainMenuButtonColor;
        youDiedTitle.color = titleColor;
        youDiedText.color = textColor;
        retryButtonText.color = retryTextColor;
        mainMenuButtonText.color = mainMenuTextColor;
        
        retryButton.interactable = true;
        mainMenuButton.interactable = true;
        
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void OnRetryButtonClicked()
    {   
        SceneManager.Instance.ReloadCurrentScene();
    }

    public void OnMainMenuClicked()
    {
        SceneManager.Instance.LoadScene(1);
    }
    
}
