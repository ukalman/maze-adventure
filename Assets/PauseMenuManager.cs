using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseMenuManager : MonoBehaviour
{

    [SerializeField] private GameObject PauseMenu;
    [SerializeField] private GameObject HowToPlayMenu;
    [SerializeField] private GameObject SettingsMenu;
    [SerializeField] private GameObject AudioPanel;
    [SerializeField] private GameObject ControlsPanel;
    [SerializeField] private GameObject MissionBriefingPanel;
    [SerializeField] private GameObject HintsPanel;
    
    // Start is called before the first frame update
    void Start()
    {
        PauseMenu.SetActive(false);
        HowToPlayMenu.SetActive(false);
        DeactivateHowToPlayPanels();
        SettingsMenu.SetActive(false);
        DeactivateSettingsPanels();
        EventManager.Instance.OnGamePaused += OnGamePaused;
        EventManager.Instance.OnGameContinued += OnGameContinued;
    }

    private void OnDestroy()
    {
        EventManager.Instance.OnGamePaused -= OnGamePaused;
        EventManager.Instance.OnGameContinued -= OnGameContinued;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnGamePaused()
    {
        PauseMenu.SetActive(true);
    }

    private void OnGameContinued()
    {
        DeactivateSettingsPanels();
        DeactivateHowToPlayPanels();
        HowToPlayMenu.SetActive(false);
        SettingsMenu.SetActive(false);
        PauseMenu.SetActive(false);

    }

    public void OnResumeClicked()
    {
        EventManager.Instance.InvokeOnGameContinued();
    }

    public void OnHowToPlayClicked()
    {
        PauseMenu.SetActive(false);
        DeactivateHowToPlayPanels();
        HowToPlayMenu.SetActive(true);
    }

    public void OnSettingsClicked()
    {
        PauseMenu.SetActive(false);
        DeactivateSettingsPanels();
        SettingsMenu.SetActive(true);
    }

    public void OnMainMenuClicked()
    {
        Time.timeScale = 1.0f;
        SceneManager.Instance.LoadScene(0);
    }

    /* How To Play Menu  */

    public void DeactivateHowToPlayPanels()
    {
        ControlsPanel.SetActive(false);
        MissionBriefingPanel.SetActive(false);
        HintsPanel.SetActive(false);
    }
    
    public void OnControlsClicked()
    {
        DeactivateHowToPlayPanels();
        ControlsPanel.SetActive(true);
    }

    public void OnMissionBriefingClicked()
    {
        DeactivateHowToPlayPanels();
        MissionBriefingPanel.SetActive(true);
    }

    public void OnHintsClicked()
    {
        DeactivateHowToPlayPanels();
        HintsPanel.SetActive(true);
    }


    
    /* Settings Menu */

    public void DeactivateSettingsPanels()
    {
        AudioPanel.SetActive(false);
        // More might come later
    }

    public void OnAudioClicked()
    {
        DeactivateSettingsPanels();
        AudioPanel.SetActive(true);
    }
    
    
    public void OnBackClicked()
    {
        DeactivateHowToPlayPanels();
        DeactivateSettingsPanels();
        HowToPlayMenu.SetActive(false);
        SettingsMenu.SetActive(false);
        PauseMenu.SetActive(true);        
    }
    
    
    
    
    
}
