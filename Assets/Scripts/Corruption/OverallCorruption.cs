using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class CorruptionInformation {
	[Range(0, 100)]
	public float maxDistortion;
	[HideInInspector]
	public float currentDistortion;
	[Range(0, 100)]
	public int clearThreshold;
	[Header("ID of segment in 'Game Manager'")]
	public int segmentID;
}

public class OverallCorruption : MonoBehaviour {
	[SerializeField]
	[Range(0, 100)]
	float overallDistortionMax;

	// The list of beats
	public List<Duration> segments;

	[HideInInspector]
	// The list of beats converted to milliseconds
	public List<Duration> durations;

	[Range(1, 200)]
	public int bpm;
	const int beatsToMsConversion = 15000; //60000 = 1 minute in milliseconds, divided by four to make sixteenth notes.

	[HideInInspector]
	public int bpmInMs;

    GameManager gameManager;
    private Button ejectButton;

    [Tooltip("Percent of corruption that has to be cleared before corruption is considered solved.")]
    [SerializeField] [Range(0, 100)] int corruptionClearThreshold;

	[HideInInspector] public float overallCorruption;
	[HideInInspector] public float overallDistortion;

	AudioDistortion audioDistortion;
	[HideInInspector] public List<CorruptionBaseClass> corruptions = new List<CorruptionBaseClass>();

	// ** Corrupted Area** //
	[SerializeField]
	[Tooltip("Corrupted area prefab")]
	private GameObject corruptedArea;
	private List<GameObject> corruptedAreaList = new List<GameObject>();

    // Added for special case of LiveTutorial popup
    private LiveTutorial liveTutorial;

    void Awake () {

        bpmInMs = ConvertBpmToMs(bpm);
		for (int i = 0; i < segments.Count; i++)
		{
			durations.Add(new Duration());
			durations[i].start = segments[i].start * bpmInMs;
			durations[i].stop = segments[i].stop * bpmInMs;
		}
	}

	void Start()
	{
        //// Special case for LiveTutorial
        if (SceneManager.GetActiveScene().name == "Cassette00Tutorial")
            liveTutorial = GameObject.FindGameObjectWithTag("LiveTutorial").GetComponent<LiveTutorial>();
        ////

        gameManager = GameObject.FindWithTag("GameManager").GetComponent<GameManager>();
        audioDistortion = gameManager.audioDistortion;
        ejectButton = gameManager.ejectButton;

		corruptions.AddRange(GetComponentsInChildren<CorruptionBaseClass>());

		Debug.Assert(corruptedArea != null, "Attach the corrupted area prefab to 'Overall Corruption'");

            for (int i = 0; i < segments.Count; i++)
            {
                // Instantiate the corrupted area prefab according to the corrupted area specifications
                RectTransform timelineSlider = gameManager.timelineSlider.GetComponent<RectTransform>();
                GameObject instantiatedObject = Instantiate(corruptedArea, timelineSlider);
                instantiatedObject.transform.SetAsFirstSibling();
                corruptedAreaList.Add(instantiatedObject);
                instantiatedObject.GetComponent<CorruptionVisuals>().SetCorruptionPosition(durations[i].start, durations[i].stop);
            }
            UpdateCorruptionAmount();
            UpdateDistortionAmount();

	}

	void Update () {
		if (Input.GetKeyDown(KeyCode.C))
		{
			Debug.Log("Overall corruption: " + overallCorruption  + "%");
			Debug.Log("Overall distortion: " + overallDistortion + "%");
		}

        if (Input.GetKeyDown(KeyCode.L))
        {
            print("Current time: " + gameManager.audioManager.GetTimeLinePosition());
            print("Current Beat in 16th notes: " + gameManager.audioManager.GetTimeLinePosition() / bpmInMs);
            print("Current Beat: " + gameManager.audioManager.GetTimeLinePosition() / (bpmInMs * 4));
        }
		// Set the current segment to cleared
		if(Input.GetKeyDown(KeyCode.X))
		{
			corruptions[gameManager.currentSegmentIndex].corruptionClearedPercent = 100;
			UpdateCorruptionAmount();
		}

		// Win the game
		if(Input.GetKeyDown(KeyCode.Z))
		{
			//bool levelCleared = true;
			foreach (CorruptionBaseClass corruption in corruptions)
			{
				corruption.corruptionClearedPercent = 100;
				corruption.cleared = true;
			}

			UpdateCorruptionAmount();
		}
	}

	public void UpdateCorruptionAmount()
	{
		overallCorruption = 0;
		for (int i = 0; i < corruptions.Count; i++)
		{
            overallCorruption += (100 - corruptions[i].corruptionClearedPercent) / corruptions.Count;

            // Set the alpha value according to corruption percentage of the segment
            corruptedAreaList[corruptions[i].segmentID].GetComponent<CorruptionVisuals>().SetAlpha(corruptions[i].corruptionClearedPercent);

            if (corruptions[i].corruptionClearedPercent >= corruptions[i].clearThreshold && !corruptions[i].cleared)
			{
                corruptions[i].cleared = true;
                //corruptedAreaList[i].GetComponent<CorruptionVisuals>().RestoreOriginalColor();
                corruptedAreaList[corruptions[i].segmentID].GetComponent<CorruptionVisuals>().RestoreOriginalColor();
            }
			else
				corruptions[i].cleared = false;
		}

        if(gameManager.LevelPerfected == false && overallCorruption == 0)
        {
            if (gameManager.stickerManageRef.EarnSticker(gameManager.stickerForPerfect.Name))
            {
                if (!gameManager.LevelCleared)
                {
                    gameManager.LevelCleared = true;
                    gameManager.stageClearVFX.CallVFXWith2StickersEarned(gameManager.stickerForGood.Sprite, gameManager.stickerForPerfect.Sprite);
                    gameManager.stickerManageRef.EarnSticker(gameManager.stickerForGood.Name);

                    // Activate the eject button when the level is cleared
                    ejectButton.interactable = true;
                    Debug.Log("Set ejectButton to interactable");

                    //// Special case for LiveTutorial
                    if (liveTutorial != null)
                    liveTutorial.ForceOpenLiveTutorial("The Cassette is now repaired enough for you to access the B-side. Just press the Eject Button!", gameManager.stageClearVFX.timeToShowGood + gameManager.stageClearVFX.timeToShowNew + +gameManager.stageClearVFX.timeToShowNew);
                    ////
                }
                else
                    gameManager.stageClearVFX.CallVFXWithStickerEarned(segmentEffects.perfect, gameManager.stickerForPerfect.Sprite);
                gameManager.audioManager.PlayWinSound(1);
                gameManager.LevelPerfected = true;
            }
        }
		else if(gameManager.stickerForGood.EarnSticker() && overallCorruption <= 100-corruptionClearThreshold) //If player hasn't won already
        {
            gameManager.stageClearVFX.CallVFXWithStickerEarned(segmentEffects.good, gameManager.stickerForGood.Sprite);
            gameManager.audioManager.PlayWinSound(0);
            gameManager.LevelCleared = true;

            // Activate the eject button when the level is cleared
            ejectButton.interactable = true;

            //// Special case for LiveTutorial
            if (liveTutorial != null)
            liveTutorial.ForceOpenLiveTutorial("The Cassette is now repaired enough for you to access the B-side. Just press the Eject Button!", gameManager.stageClearVFX.timeToShowGood + gameManager.stageClearVFX.timeToShowNew);
            ////
        }
	} 

	public void UpdateDistortionAmount()
	{
		overallDistortion = 0;
		foreach (CorruptionBaseClass corruption in corruptions)
		{
			overallDistortion += corruption.innerDistortion; 
		}
		overallDistortion += overallCorruption * overallDistortionMax / 100;
		audioDistortion.SetDistortion(overallDistortion);
	}

	public float GetOverallCorruptionAmount()
	{
		return overallCorruption;
	}

	int ConvertBpmToMs(int bpm)
	{
		return Mathf.RoundToInt(beatsToMsConversion / bpm);
	}
}