using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class OptionsMenuNew : MonoBehaviour {

	// toggle buttons
	public GameObject fullscreentext;
	public GameObject shadowofftext;
	public GameObject shadowofftextLINE;
	public GameObject shadowlowtext;
	public GameObject shadowlowtextLINE;
	public GameObject shadowhightext;
	public GameObject shadowhightextLINE;
	public GameObject showhudtext;
	public GameObject tooltipstext;
	public GameObject difficultynormaltext;
	public GameObject difficultynormaltextLINE;
	public GameObject difficultyhardcoretext;
	public GameObject difficultyhardcoretextLINE;
	public GameObject cameraeffectstext;
	public GameObject invertmousetext;
	public GameObject vsynctext;
	public GameObject motionblurtext;
	public GameObject ambientocclusiontext;
	public GameObject texturelowtext;
	public GameObject texturelowtextLINE;
	public GameObject texturemedtext;
	public GameObject texturemedtextLINE;
	public GameObject texturehightext;
	public GameObject texturehightextLINE;
	public GameObject aaofftext;
	public GameObject aaofftextLINE;
	public GameObject aa2xtext;
	public GameObject aa2xtextLINE;
	public GameObject aa4xtext;
	public GameObject aa4xtextLINE;
	public GameObject aa8xtext;
	public GameObject aa8xtextLINE;

	// sliders
	public GameObject musicSlider;
	public GameObject sensitivityXSlider;
	public GameObject sensitivityYSlider;
	public GameObject mouseSmoothSlider;

	private float sliderValue = 0.0f;
	private float sliderValueXSensitivity = 0.0f;
	private float sliderValueYSensitivity = 0.0f;
	private float sliderValueSmoothing = 0.0f;

	public void  Start (){
		// check difficulty
		if(PlayerPrefs.GetInt("NormalDifficulty") == 1){
			difficultynormaltextLINE.gameObject.SetActive(true);
			difficultyhardcoretextLINE.gameObject.SetActive(false);
		}
		else
		{
			difficultyhardcoretextLINE.gameObject.SetActive(true);
			difficultynormaltextLINE.gameObject.SetActive(false);
		}

		// check slider values
		musicSlider.GetComponent<Slider>().value = PlayerPrefs.GetFloat("MusicVolume");
		sensitivityXSlider.GetComponent<Slider>().value = PlayerPrefs.GetFloat("XSensitivity");
		sensitivityYSlider.GetComponent<Slider>().value = PlayerPrefs.GetFloat("YSensitivity");
		mouseSmoothSlider.GetComponent<Slider>().value = PlayerPrefs.GetFloat("MouseSmoothing");

		// check full screen
		if(Screen.fullScreen == true){
			fullscreentext.GetComponent<Text>().text = "on";
		}
		else if(Screen.fullScreen == false){
			fullscreentext.GetComponent<Text>().text = "off";
		}

		// check hud value
		if(PlayerPrefs.GetInt("ShowHUD")==0){
			showhudtext.GetComponent<Text>().text = "off";
		}
		else{
			showhudtext.GetComponent<Text>().text = "on";
		}

		// check tool tip value
		if(PlayerPrefs.GetInt("ToolTips")==0){
			tooltipstext.GetComponent<Text>().text = "off";
		}
		else{
			tooltipstext.GetComponent<Text>().text = "on";
		}

		// check shadow distance/enabled
		if(PlayerPrefs.GetInt("Shadows") == 0){
			QualitySettings.shadowCascades = 0;
			QualitySettings.shadowDistance = 0;
			shadowofftext.GetComponent<Text>().text = "OFF";
			shadowlowtext.GetComponent<Text>().text = "low";
			shadowhightext.GetComponent<Text>().text = "high";
			shadowofftextLINE.gameObject.SetActive(true);
			shadowlowtextLINE.gameObject.SetActive(false);
			shadowhightextLINE.gameObject.SetActive(false);
		}
		else if(PlayerPrefs.GetInt("Shadows") == 1){
			QualitySettings.shadowCascades = 2;
			QualitySettings.shadowDistance = 75;
			shadowofftext.GetComponent<Text>().text = "off";
			shadowlowtext.GetComponent<Text>().text = "LOW";
			shadowhightext.GetComponent<Text>().text = "high";
			shadowofftextLINE.gameObject.SetActive(false);
			shadowlowtextLINE.gameObject.SetActive(true);
			shadowhightextLINE.gameObject.SetActive(false);
		}
		else if(PlayerPrefs.GetInt("Shadows") == 2){
			QualitySettings.shadowCascades = 4;
			QualitySettings.shadowDistance = 500;
			shadowofftext.GetComponent<Text>().text = "off";
			shadowlowtext.GetComponent<Text>().text = "low";
			shadowhightext.GetComponent<Text>().text = "HIGH";
			shadowofftextLINE.gameObject.SetActive(false);
			shadowlowtextLINE.gameObject.SetActive(false);
			shadowhightextLINE.gameObject.SetActive(true);
		}

		// check vsync
		if(QualitySettings.vSyncCount == 0){
			vsynctext.GetComponent<Text>().text = "off";
		}
		else if(QualitySettings.vSyncCount == 1){
			vsynctext.GetComponent<Text>().text = "on";
		}

		// check mouse inverse
		if(PlayerPrefs.GetInt("Inverted")==0){
			invertmousetext.GetComponent<Text>().text = "off";
		}
		else if(PlayerPrefs.GetInt("Inverted")==1){
			invertmousetext.GetComponent<Text>().text = "on";
		}

		// check motion blur
		if(PlayerPrefs.GetInt("MotionBlur")==0){
			motionblurtext.GetComponent<Text>().text = "off";
		}
		else if(PlayerPrefs.GetInt("MotionBlur")==1){
			motionblurtext.GetComponent<Text>().text = "on";
		}

		// check ambient occlusion
		if(PlayerPrefs.GetInt("AmbientOcclusion")==0){
			ambientocclusiontext.GetComponent<Text>().text = "off";
		}
		else if(PlayerPrefs.GetInt("AmbientOcclusion")==1){
			ambientocclusiontext.GetComponent<Text>().text = "on";
		}

		// check texture quality
		if(PlayerPrefs.GetInt("Textures") == 0){
			QualitySettings.masterTextureLimit = 2;
			texturelowtext.GetComponent<Text>().text = "LOW";
			texturemedtext.GetComponent<Text>().text = "med";
			texturehightext.GetComponent<Text>().text = "high";
			texturelowtextLINE.gameObject.SetActive(true);
			texturemedtextLINE.gameObject.SetActive(false);
			texturehightextLINE.gameObject.SetActive(false);
		}
		else if(PlayerPrefs.GetInt("Textures") == 1){
			QualitySettings.masterTextureLimit = 1;
			texturelowtext.GetComponent<Text>().text = "low";
			texturemedtext.GetComponent<Text>().text = "MED";
			texturehightext.GetComponent<Text>().text = "high";
			texturelowtextLINE.gameObject.SetActive(false);
			texturemedtextLINE.gameObject.SetActive(true);
			texturehightextLINE.gameObject.SetActive(false);
		}
		else if(PlayerPrefs.GetInt("Textures") == 2){
			QualitySettings.masterTextureLimit = 0;
			texturelowtext.GetComponent<Text>().text = "low";
			texturemedtext.GetComponent<Text>().text = "med";
			texturehightext.GetComponent<Text>().text = "HIGH";
			texturelowtextLINE.gameObject.SetActive(false);
			texturemedtextLINE.gameObject.SetActive(false);
			texturehightextLINE.gameObject.SetActive(true);
		}
	}

	public void  Update (){
		sliderValue = musicSlider.GetComponent<Slider>().value;
		sliderValueXSensitivity = sensitivityXSlider.GetComponent<Slider>().value;
		sliderValueYSensitivity = sensitivityYSlider.GetComponent<Slider>().value;
		sliderValueSmoothing = mouseSmoothSlider.GetComponent<Slider>().value;
	}

	public void  FullScreen (){
		Screen.fullScreen = !Screen.fullScreen;

		if(Screen.fullScreen == true){
			fullscreentext.GetComponent<Text>().text = "on";
		}
		else if(Screen.fullScreen == false){
			fullscreentext.GetComponent<Text>().text = "off";
		}
	}

	public void MusicSlider (){
		PlayerPrefs.SetFloat("MusicVolume", sliderValue);
	}

	public void  SensitivityXSlider (){
		PlayerPrefs.SetFloat("XSensitivity", sliderValueXSensitivity);
	}

	public void  SensitivityYSlider (){
		PlayerPrefs.SetFloat("YSensitivity", sliderValueYSensitivity);
	}

	public void  SensitivitySmoothing (){
		PlayerPrefs.SetFloat("MouseSmoothing", sliderValueSmoothing);
		Debug.Log(PlayerPrefs.GetFloat("MouseSmoothing"));
	}

	// the playerprefs variable that is checked to enable hud while in game
	public void  ShowHUD (){
		if(PlayerPrefs.GetInt("ShowHUD")==0){
			PlayerPrefs.SetInt("ShowHUD",1);
			showhudtext.GetComponent<Text>().text = "on";
		}
		else if(PlayerPrefs.GetInt("ShowHUD")==1){
			PlayerPrefs.SetInt("ShowHUD",0);
			showhudtext.GetComponent<Text>().text = "off";
		}
	}

	// show tool tips like: 'How to Play' control pop ups
	public void  ToolTips (){
		if(PlayerPrefs.GetInt("ToolTips")==0){
			PlayerPrefs.SetInt("ToolTips",1);
			tooltipstext.GetComponent<Text>().text = "on";
		}
		else if(PlayerPrefs.GetInt("ToolTips")==1){
			PlayerPrefs.SetInt("ToolTips",0);
			tooltipstext.GetComponent<Text>().text = "off";
		}
	}

	public void  NormalDifficulty (){
		//difficultynormaltext.GetComponent<Text>().text = "NORMAL";
		//difficultyhardcoretext.GetComponent<Text>().text = "hardcore";
		difficultyhardcoretextLINE.gameObject.SetActive(false);
		difficultynormaltextLINE.gameObject.SetActive(true);
		PlayerPrefs.SetInt("NormalDifficulty",1);
		PlayerPrefs.SetInt("HardCoreDifficulty",0);
	}

	public void  HardcoreDifficulty (){
		//difficultynormaltext.GetComponent<Text>().text = "normal";
		//difficultyhardcoretext.GetComponent<Text>().text = "HARDCORE";
		difficultyhardcoretextLINE.gameObject.SetActive(true);
		difficultynormaltextLINE.gameObject.SetActive(false);
		PlayerPrefs.SetInt("NormalDifficulty",0);
		PlayerPrefs.SetInt("HardCoreDifficulty",1);
	}

	public void  ShadowsOff (){
		PlayerPrefs.SetInt("Shadows",0);
		QualitySettings.shadowCascades = 0;
		QualitySettings.shadowDistance = 0;
		shadowofftext.GetComponent<Text>().text = "OFF";
		shadowlowtext.GetComponent<Text>().text = "low";
		shadowhightext.GetComponent<Text>().text = "high";
		shadowofftextLINE.gameObject.SetActive(true);
		shadowlowtextLINE.gameObject.SetActive(false);
		shadowhightextLINE.gameObject.SetActive(false);
	}

	public void  ShadowsLow (){
		PlayerPrefs.SetInt("Shadows",1);
		QualitySettings.shadowCascades = 2;
		QualitySettings.shadowDistance = 75;
		shadowofftext.GetComponent<Text>().text = "off";
		shadowlowtext.GetComponent<Text>().text = "LOW";
		shadowhightext.GetComponent<Text>().text = "high";
		shadowofftextLINE.gameObject.SetActive(false);
		shadowlowtextLINE.gameObject.SetActive(true);
		shadowhightextLINE.gameObject.SetActive(false);
	}

	public void  ShadowsHigh (){
		PlayerPrefs.SetInt("Shadows",2);
		QualitySettings.shadowCascades = 4;
		QualitySettings.shadowDistance = 500;
		shadowofftext.GetComponent<Text>().text = "off";
		shadowlowtext.GetComponent<Text>().text = "low";
		shadowhightext.GetComponent<Text>().text = "HIGH";
		shadowofftextLINE.gameObject.SetActive(false);
		shadowlowtextLINE.gameObject.SetActive(false);
		shadowhightextLINE.gameObject.SetActive(true);
	}

	public void  vsync (){
		if(QualitySettings.vSyncCount == 0){
			QualitySettings.vSyncCount = 1;
			vsynctext.GetComponent<Text>().text = "on";
		}
		else if(QualitySettings.vSyncCount == 1){
			QualitySettings.vSyncCount = 0;
			vsynctext.GetComponent<Text>().text = "off";
		}
	}

	public void  InvertMouse (){
		if(PlayerPrefs.GetInt("Inverted")==0){
			PlayerPrefs.SetInt("Inverted",1);
			invertmousetext.GetComponent<Text>().text = "on";
		}
		else if(PlayerPrefs.GetInt("Inverted")==1){
			PlayerPrefs.SetInt("Inverted",0);
			invertmousetext.GetComponent<Text>().text = "off";
		}
	}

	public void  MotionBlur (){
		if(PlayerPrefs.GetInt("MotionBlur")==0){
			PlayerPrefs.SetInt("MotionBlur",1);
			motionblurtext.GetComponent<Text>().text = "on";
		}
		else if(PlayerPrefs.GetInt("MotionBlur")==1){
			PlayerPrefs.SetInt("MotionBlur",0);
			motionblurtext.GetComponent<Text>().text = "off";
		}
	}

	public void  AmbientOcclusion (){
		if(PlayerPrefs.GetInt("AmbientOcclusion")==0){
			PlayerPrefs.SetInt("AmbientOcclusion",1);
			ambientocclusiontext.GetComponent<Text>().text = "on";
		}
		else if(PlayerPrefs.GetInt("AmbientOcclusion")==1){
			PlayerPrefs.SetInt("AmbientOcclusion",0);
			ambientocclusiontext.GetComponent<Text>().text = "off";
		}
	}

	public void  CameraEffects (){
		if(PlayerPrefs.GetInt("CameraEffects")==0){
			PlayerPrefs.SetInt("CameraEffects",1);
			cameraeffectstext.GetComponent<Text>().text = "on";
		}
		else if(PlayerPrefs.GetInt("CameraEffects")==1){
			PlayerPrefs.SetInt("CameraEffects",0);
			cameraeffectstext.GetComponent<Text>().text = "off";
		}
	}

	public void  TexturesLow (){
		PlayerPrefs.SetInt("Textures",0);
		QualitySettings.masterTextureLimit = 2;
		texturelowtext.GetComponent<Text>().text = "LOW";
		texturemedtext.GetComponent<Text>().text = "med";
		texturehightext.GetComponent<Text>().text = "high";
		texturelowtextLINE.gameObject.SetActive(true);
		texturemedtextLINE.gameObject.SetActive(false);
		texturehightextLINE.gameObject.SetActive(false);
	}

	public void  TexturesMed (){
		PlayerPrefs.SetInt("Textures",1);
		QualitySettings.masterTextureLimit = 1;
		texturelowtext.GetComponent<Text>().text = "low";
		texturemedtext.GetComponent<Text>().text = "MED";
		texturehightext.GetComponent<Text>().text = "high";
		texturelowtextLINE.gameObject.SetActive(false);
		texturemedtextLINE.gameObject.SetActive(true);
		texturehightextLINE.gameObject.SetActive(false);
	}

	public void  TexturesHigh (){
		PlayerPrefs.SetInt("Textures",2);
		QualitySettings.masterTextureLimit = 0;
		texturelowtext.GetComponent<Text>().text = "low";
		texturemedtext.GetComponent<Text>().text = "med";
		texturehightext.GetComponent<Text>().text = "HIGH";
		texturelowtextLINE.gameObject.SetActive(false);
		texturemedtextLINE.gameObject.SetActive(false);
		texturehightextLINE.gameObject.SetActive(true);
	}
}