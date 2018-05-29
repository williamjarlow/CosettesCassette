using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class BankManager : Singleton<BankManager> {

	private static bool created = false;

	private AudioManager audioManager;

	private string currentBank;

	[HideInInspector] public List<string> managerBankFiles;
	[HideInInspector] public FMOD.Studio.Bank[] banksToLoad = new FMOD.Studio.Bank[3];
	[HideInInspector] public FMOD.Studio.Bank[] banksToUnload = new FMOD.Studio.Bank[3];

	private FMOD.RESULT result;

	private void Awake()
	{
		if (!created)
		{
			DontDestroyOnLoad(this.gameObject);
			created = true;
		}
			
		audioManager = GameObject.FindWithTag ("AudioManager").GetComponent<AudioManager> ();

		//Load master banks
		FMODUnity.RuntimeManager.LoadBank ("Master Bank.bank", true);
		FMODUnity.RuntimeManager.LoadBank ("Master Bank.strings.bank", true);
	}

	private void Start ()
	{
		FMODUnity.RuntimeManager.LoadBank ("LevelSelect.bank", true);
		currentBank = "LevelSelect.bank";
	}

	public void LoadBanks(string bankToLoad)
	{
		FMODUnity.RuntimeManager.LoadBank (bankToLoad, true);
		currentBank = bankToLoad;
			
		//Waits for loads to finish
		FMODUnity.RuntimeManager.WaitForAllLoads();
		audioManager.systemObj.flushCommands();
	}

	public IEnumerator UnloadBanks(string bankToUnload)
	{
		yield return new WaitForSeconds(2.0f);

		FMODUnity.RuntimeManager.UnloadBank (bankToUnload);
	}

	public void SceneChanged(string levelName)
	{
		StartCoroutine(UnloadBanks(currentBank));

		switch (levelName)
		{
		case "LevelSelect":
			LoadBanks ("LevelSelect.bank");
			break;
		case "Cassette00": 
		case "SongCassette00":
		case "StoryCassette00":
			LoadBanks ("Cassette_00.bank");
			break;
		case "Cassette01":
		case "SongCassette01":
		case "StoryCassette01":
			LoadBanks ("Cassette_01.bank");
			break;
		case "Cassette02":
		case "SongCassette02":
		case "StoryCassette02":
			LoadBanks ("Cassette_02.bank");
			break;
		case "Cassette03":
		case "SongCassette03":
		case "StoryCassette03":
			LoadBanks ("Cassette_03.bank");
			break;
		case "Cassette04":
		case "SongCassette04":
		case "StoryCassette04":
			LoadBanks ("Cassette_04.bank");
			break;
		case "Cassette05":
		case "SongCassette05":
		case "StoryCassette05":
			LoadBanks ("Cassette_05.bank");
			break;
		case "Cassette06":
		case "SongCassette06":
		case "StoryCassette06":
			LoadBanks ("Cassette_06.bank");
			break;
		case "Credits":
			LoadBanks ("Credits.bank");
			break;
		}
	}
}
