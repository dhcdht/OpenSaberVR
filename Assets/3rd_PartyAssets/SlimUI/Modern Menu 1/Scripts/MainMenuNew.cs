using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System;

public class MainMenuNew : MonoBehaviour {

	Animator CameraObject;

	[Header("Loaded Scene")]
	[Tooltip("The name of the scene in the build settings that will load")]
	public string sceneName = ""; 

	[Header("Panels")]
	[Tooltip("The UI Panel that holds the CONTROLS window tab")]
	public GameObject PanelControls;
	[Tooltip("The UI Panel that holds the VIDEO window tab")]
	public GameObject PanelVideo;
	[Tooltip("The UI Panel that holds the GAME window tab")]
	public GameObject PanelGame;
	[Tooltip("The UI Panel that holds the KEY BINDINGS window tab")]
	public GameObject PanelKeyBindings;
	[Tooltip("The UI Sub-Panel under KEY BINDINGS for MOVEMENT")]
	public GameObject PanelMovement;
	[Tooltip("The UI Sub-Panel under KEY BINDINGS for COMBAT")]
	public GameObject PanelCombat;
	[Tooltip("The UI Sub-Panel under KEY BINDINGS for GENERAL")]
	public GameObject PanelGeneral;
	[Tooltip("The UI Pop-Up when 'EXIT' is clicked")]
	public GameObject PanelareYouSure;

	[Header("SFX")]
	[Tooltip("The GameObject holding the Audio Source component for the HOVER SOUND")]
	public GameObject hoverSound;
	[Tooltip("The GameObject holding the Audio Source component for the AUDIO SLIDER")]
	public GameObject sliderSound;
	[Tooltip("The GameObject holding the Audio Source component for the SWOOSH SOUND when switching to the Settings Screen")]
	public GameObject swooshSound;

	// campaign button sub menu
	[Header("PLAY Sub-Buttons")]
	[Tooltip("Continue Button GameObject Pop Up")]
	public GameObject continueBtn;
	[Tooltip("New Game Button GameObject Pop Up")]
	public GameObject newGameBtn;
	[Tooltip("Load Game Button GameObject Pop Up")]
	public GameObject loadGameBtn;

	// highlights
	[Header("Highlight Effects")]
	[Tooltip("Highlight Image for when GAME Tab is selected in Settings")]
	public GameObject lineGame;
	[Tooltip("Highlight Image for when VIDEO Tab is selected in Settings")]
	public GameObject lineVideo;
	[Tooltip("Highlight Image for when CONTROLS Tab is selected in Settings")]
	public GameObject lineControls;
	[Tooltip("Highlight Image for when KEY BINDINGS Tab is selected in Settings")]
	public GameObject lineKeyBindings;
	[Tooltip("Highlight Image for when MOVEMENT Sub-Tab is selected in KEY BINDINGS")]
	public GameObject lineMovement;
	[Tooltip("Highlight Image for when COMBAT Sub-Tab is selected in KEY BINDINGS")]
	public GameObject lineCombat;
	[Tooltip("Highlight Image for when GENERAL Sub-Tab is selected in KEY BINDINGS")]
	public GameObject lineGeneral;

    void Start(){
		//CameraObject = transform.GetComponent<Animator>();
	}

	public void  PlayCampaign (){
		PanelareYouSure.gameObject.SetActive(false);
		continueBtn.gameObject.SetActive(true);
		newGameBtn.gameObject.SetActive(true);
		loadGameBtn.gameObject.SetActive(true);
	}

    public void NewGame(){
		if(sceneName != ""){
			SceneManager.LoadScene(sceneName, LoadSceneMode.Single);
		}
	}

	public void  DisablePlayCampaign (){
		continueBtn.gameObject.SetActive(false);
		newGameBtn.gameObject.SetActive(false);
		loadGameBtn.gameObject.SetActive(false);
	}

	public void  Position2 (){
		DisablePlayCampaign();
		CameraObject.SetFloat("Animate",1);
	}

	public void  Position1 (){
		CameraObject.SetFloat("Animate",0);
	}

	public void  GamePanel (){
		PanelControls.gameObject.SetActive(false);
		PanelVideo.gameObject.SetActive(false);
		PanelGame.gameObject.SetActive(true);
		PanelKeyBindings.gameObject.SetActive(false);

		lineGame.gameObject.SetActive(true);
		lineControls.gameObject.SetActive(false);
		lineVideo.gameObject.SetActive(false);
		lineKeyBindings.gameObject.SetActive(false);
	}

	public void  VideoPanel (){
		PanelControls.gameObject.SetActive(false);
		PanelVideo.gameObject.SetActive(true);
		PanelGame.gameObject.SetActive(false);
		PanelKeyBindings.gameObject.SetActive(false);

		lineGame.gameObject.SetActive(false);
		lineControls.gameObject.SetActive(false);
		lineVideo.gameObject.SetActive(true);
		lineKeyBindings.gameObject.SetActive(false);
	}

	public void  ControlsPanel (){
		PanelControls.gameObject.SetActive(true);
		PanelVideo.gameObject.SetActive(false);
		PanelGame.gameObject.SetActive(false);
		PanelKeyBindings.gameObject.SetActive(false);

		lineGame.gameObject.SetActive(false);
		lineControls.gameObject.SetActive(true);
		lineVideo.gameObject.SetActive(false);
		lineKeyBindings.gameObject.SetActive(false);
	}

	public void  KeyBindingsPanel (){
		PanelControls.gameObject.SetActive(false);
		PanelVideo.gameObject.SetActive(false);
		PanelGame.gameObject.SetActive(false);
		PanelKeyBindings.gameObject.SetActive(true);

		lineGame.gameObject.SetActive(false);
		lineControls.gameObject.SetActive(false);
		lineVideo.gameObject.SetActive(true);
		lineKeyBindings.gameObject.SetActive(true);
	}

	public void  MovementPanel (){
		PanelMovement.gameObject.SetActive(true);
		PanelCombat.gameObject.SetActive(false);
		PanelGeneral.gameObject.SetActive(false);

		lineMovement.gameObject.SetActive(true);
		lineCombat.gameObject.SetActive(false);
		lineGeneral.gameObject.SetActive(false);
	}

	public void CombatPanel (){
		PanelMovement.gameObject.SetActive(false);
		PanelCombat.gameObject.SetActive(true);
		PanelGeneral.gameObject.SetActive(false);

		lineMovement.gameObject.SetActive(false);
		lineCombat.gameObject.SetActive(true);
		lineGeneral.gameObject.SetActive(false);
	}

	public void GeneralPanel (){
		PanelMovement.gameObject.SetActive(false);
		PanelCombat.gameObject.SetActive(false);
		PanelGeneral.gameObject.SetActive(true);

		lineMovement.gameObject.SetActive(false);
		lineCombat.gameObject.SetActive(false);
		lineGeneral.gameObject.SetActive(true);
	}

	public void PlayHover (){
		hoverSound.GetComponent<AudioSource>().Play();
	}

	public void PlaySFXHover (){
		sliderSound.GetComponent<AudioSource>().Play();
	}

	public void PlaySwoosh (){
		swooshSound.GetComponent<AudioSource>().Play();
	}

	// Are You Sure - Quit Panel Pop Up
	public void  AreYouSure (){
		PanelareYouSure.gameObject.SetActive(true);
		DisablePlayCampaign();
	}

	public void  No (){
		PanelareYouSure.gameObject.SetActive(false);
	}

	public void  Yes (){
		Application.Quit();
	}
}