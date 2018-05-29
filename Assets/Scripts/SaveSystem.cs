using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

public class SaveSystem : Singleton<SaveSystem>
{
    public static SaveSystem saveSystem;
    private int sceneIndexes;
    private FileStream file;

    [SerializeField] private GameObject stickerManRef;
    PlayerData data = new PlayerData();

	[HideInInspector] public bool masterBanksLoaded = false;

    void Awake()
    {
        //Make sure the save file is copied when starting the application.
        if (File.Exists(Application.persistentDataPath + Path.DirectorySeparatorChar + "playerInfo.dat"))
        {
            BinaryFormatter bf = new BinaryFormatter();
            file = File.Open(Application.persistentDataPath + Path.DirectorySeparatorChar + "playerInfo.dat", FileMode.OpenOrCreate);
            data = (PlayerData)bf.Deserialize(file);
            file.Close();
        }

        //Make object dont destroy on load, singleton requirement.
        DontDestroyOnLoad(gameObject);
        saveSystem = this;

        //Lock and unlocks levels in the preset order if this is the first time application is started.
        if (!data.levelLockSave.ContainsKey(0))
        {
            sceneIndexes = SceneManager.sceneCountInBuildSettings;
            BinaryFormatter bf = new BinaryFormatter();
            file = File.Open(Application.persistentDataPath + Path.DirectorySeparatorChar + "playerInfo.dat", FileMode.OpenOrCreate);

            //StartScreen, LevelSelect and Tutorial Level unlocked from start.
            data.levelLockSave.Add(0, true);
            data.levelLockSave.Add(1, true);
            data.levelLockSave.Add(2, true);

            for (int i = 3; i < sceneIndexes; i++)
            {
                data.levelLockSave.Add(i, false);
            }

            //Credits unlocked from start.
            data.levelLockSave[23] = true;

            bf.Serialize(file, data);
            file.Close();

        }
    }

    private void Update()
    {
        //Reset key if needed.
        if (Input.GetKeyDown(KeyCode.R))
        {
            ClearSegments();
            ClearUnlocks();
        }
    }

    //Save individual segments.
    public void SaveSegment(SaveSegmentStruct save, int levelIndex, int segmentIndex)
    {
        BinaryFormatter bf = new BinaryFormatter();
        file = File.Open(Application.persistentDataPath + Path.DirectorySeparatorChar + "playerInfo.dat", FileMode.OpenOrCreate);      // According to random source this might need "//playerInfo.dat" instead for android, must be tested
        
        //Check if the specific level and segment is a part of the save file, if its not we create a slot for it
    CreateSaveState:
        if(data.segmentSave.Count < levelIndex + 1)
        {
            SaveSegmentStruct emptyStruct = new SaveSegmentStruct();
            data.segmentSave.Add(new List<SaveSegmentStruct> { emptyStruct });
            goto CreateSaveState;
        }
        else
        {
            if (data.segmentSave[levelIndex].Count < segmentIndex + 1)
            {
                SaveSegmentStruct emptyStruct = new SaveSegmentStruct();
                data.segmentSave[levelIndex].Add(emptyStruct);
                goto CreateSaveState;
            }
            else
            {
                if (data.segmentSave[levelIndex][segmentIndex].exists == false)
                {
                    SaveSegmentStruct emptyStruct = new SaveSegmentStruct();
                    emptyStruct.exists = true;
                    data.segmentSave[levelIndex][segmentIndex] = emptyStruct;
                    goto CreateSaveState;
                }
                else
                {
                    goto Continue;
                }
            }
        }
        //Add segment save to the save file and serialize it.
    Continue:
        data.segmentSave[levelIndex][segmentIndex] = save;
        bf.Serialize(file, data);
        file.Close();
    }

    //Save individual stickers.
    public void SaveStickers(string name, bool earned)
    {
        BinaryFormatter bf = new BinaryFormatter();
        file = File.Open(Application.persistentDataPath + Path.DirectorySeparatorChar + "playerInfo.dat", FileMode.OpenOrCreate);      // According to random source this might need "//playerInfo.dat" instead for android, must be tested

        //Check if specific key exists in the save file, if it does we just save otherwise we create a new one
        if (data.stickersSave.ContainsKey(name))
        {
            data.stickersSave[name] = earned;
        }
        else
        {
            data.stickersSave.Add(name, earned);
        }
        //Serialize the values and save.
        bf.Serialize(file, data);
        file.Close();
    }

    //Function to load stickers, used when restarting the application.
    public void LoadStickers(Dictionary<string, Sticker> stickersRef)
    {
        if (File.Exists(Application.persistentDataPath + Path.DirectorySeparatorChar + "playerInfo.dat"))
        {
            BinaryFormatter bf = new BinaryFormatter();
            file = File.Open(Application.persistentDataPath + Path.DirectorySeparatorChar + "playerInfo.dat", FileMode.OpenOrCreate);
            data = (PlayerData)bf.Deserialize(file);

            //Get all keys and update the values in the sticker album, basically check if we have earned a sticker
            //and make sure we also have it unlocked again when we start the game anew.
            List<string> keyList = new List<string>(stickersRef.Keys);
            for (int i = 0; i < data.stickersSave.Count; i++)
            {
                if (!data.stickersSave.ContainsKey(keyList[i]))
                {
                    data.stickersSave.Add(stickerManRef.GetComponent<StickerManager>().stickerDictionary[keyList[i]].Name, false);
                }
                else if(data.stickersSave[keyList[i]] == true)
                {
                    stickerManRef.GetComponent<StickerManager>().stickerDictionary[keyList[i]].loaded = true;
                    stickerManRef.GetComponent<StickerManager>().stickerDictionary[keyList[i]].Unlocked = data.stickersSave[keyList[i]];
                }
            }

            file.Close();
        }
    }

    //Function to load segments, used when starting a level.
    public SaveSegmentStruct LoadSegment(SaveSegmentStruct load, int loadLevelID, int loadSegmentID)
    {
        if (File.Exists(Application.persistentDataPath + Path.DirectorySeparatorChar + "playerInfo.dat"))
        {
            BinaryFormatter bf = new BinaryFormatter();
            file = File.Open(Application.persistentDataPath + Path.DirectorySeparatorChar + "playerInfo.dat", FileMode.OpenOrCreate);
            data = (PlayerData)bf.Deserialize(file);

        CreateSaveStateLoad:
            if (data.segmentSave.Count < loadLevelID + 1)
            {
                SaveSegmentStruct emptyStruct = new SaveSegmentStruct();
                data.segmentSave.Add(new List<SaveSegmentStruct> { emptyStruct });
                goto CreateSaveStateLoad;
            }
            else
            {
                if (data.segmentSave[loadLevelID].Count < loadSegmentID + 1)
                {
                    SaveSegmentStruct emptyStruct = new SaveSegmentStruct();
                    data.segmentSave[loadLevelID].Add(emptyStruct);
                    goto CreateSaveStateLoad;
                }
                else
                {
                    if (data.segmentSave[loadLevelID][loadSegmentID].exists == false)
                    {
                        SaveSegmentStruct emptyStruct = new SaveSegmentStruct();
                        emptyStruct.exists = true;
                        data.segmentSave[loadLevelID][loadSegmentID] = emptyStruct;
                        goto CreateSaveStateLoad;
                    }
                    else
                    {
                        goto ContinueLoad;
                    }
                }
            }
        //Add segment save to the save file and serialize it.
        ContinueLoad:
            load.points = data.segmentSave[loadLevelID][loadSegmentID].points;
            file.Close();
            
        }
        return load;
    }
    
    //Clears the earned stickers, unsure whether we want to use this later or not. 
    //This is currently only used for testing the stickers.
    public void ClearStickers(Dictionary<string, Sticker> stickersRef)
    {
        if (File.Exists(Application.persistentDataPath + Path.DirectorySeparatorChar + "playerInfo.dat"))
        {
            BinaryFormatter bf = new BinaryFormatter();
            file = File.Open(Application.persistentDataPath + Path.DirectorySeparatorChar + "playerInfo.dat", FileMode.OpenOrCreate);
            data = (PlayerData)bf.Deserialize(file);

            List<string> keyList = new List<string>(stickersRef.Keys);
            for (int i = 0; i < stickerManRef.GetComponent<StickerManager>().stickerDictionary.Count; i++)
            {
                stickerManRef.GetComponent<StickerManager>().stickerDictionary[keyList[i]].loaded = false;
                stickerManRef.GetComponent<StickerManager>().stickerDictionary[keyList[i]].Unlocked = false;
            }

            data.stickersSave.Clear();
            file.Close();

            for (int i = 0; i < stickerManRef.GetComponent<StickerManager>().stickerDictionary.Count; i++)
            {
                SaveStickers(stickerManRef.GetComponent<StickerManager>().stickerDictionary[keyList[i]].Name, false);
            }
        }
    }

    //Clears all saved segment data, unsure whether we want to use this later or not. 
    //This is currently only used for testing the stickers.
    public void ClearSegments()
    {
        if (File.Exists(Application.persistentDataPath + Path.DirectorySeparatorChar + "playerInfo.dat"))
        {
            BinaryFormatter bf = new BinaryFormatter();
            file = File.Open(Application.persistentDataPath + Path.DirectorySeparatorChar + "playerInfo.dat", FileMode.OpenOrCreate);

            SaveSegmentStruct emptyStruct = new SaveSegmentStruct();
            emptyStruct.points = 0;
            for (int i = 0; i < data.segmentSave.Count; i++)
            {
                for (int j = 0; j < data.segmentSave[i].Count; j++)
                {
                    data.segmentSave[i][j] = emptyStruct;
                }
            }

            bf.Serialize(file, data);
            file.Close();
        }

    }

    //Clears the unlocked dictionary.
    public void ClearUnlocks()
    {
        if (File.Exists(Application.persistentDataPath + Path.DirectorySeparatorChar + "playerInfo.dat"))
        {
            BinaryFormatter bf = new BinaryFormatter();
            file = File.Open(Application.persistentDataPath + Path.DirectorySeparatorChar + "playerInfo.dat", FileMode.OpenOrCreate);

            data.levelLockSave.Clear();
            sceneIndexes = SceneManager.sceneCountInBuildSettings;

            //StartScreen, LevelSelect and Tutorial Level unlocked from start.
            data.levelLockSave.Add(0, true);
            data.levelLockSave.Add(1, true);
            data.levelLockSave.Add(2, true);
            for (int i = 3; i < sceneIndexes; i++)
            {
                data.levelLockSave.Add(i, false);
            }

            //Credits unlocked from start.
            data.levelLockSave[23] = true;

            bf.Serialize(file, data);
            file.Close();
        }

    }

    //Unlock specific scene index, currently it unlocks levels and at the same time it unlocks the audiolog and music scene
    //for the completed level aswell. 
    public void UnlockLevel(int buildIndex)
    {
        BinaryFormatter bf = new BinaryFormatter();
        file = File.Open(Application.persistentDataPath + Path.DirectorySeparatorChar + "playerInfo.dat", FileMode.OpenOrCreate);      // According to random source this might need "//playerInfo.dat" instead for android, must be tested

        if (buildIndex < 8)
        {
            data.levelLockSave[buildIndex + 1] = true;
        }
        data.levelLockSave[buildIndex + 7] = true;
        data.levelLockSave[buildIndex + 14] = true;

        //Serialize the values and save
        bf.Serialize(file, data);
        file.Close();
    }

    //Function to get all unlocks.
    public Dictionary<int, bool> GetUnlocks()
    {
        Dictionary<int, bool> unlocks = new Dictionary<int, bool>();

        if (File.Exists(Application.persistentDataPath + Path.DirectorySeparatorChar + "playerInfo.dat"))
        {
            BinaryFormatter bf = new BinaryFormatter();
            file = File.Open(Application.persistentDataPath + Path.DirectorySeparatorChar + "playerInfo.dat", FileMode.OpenOrCreate);
            data = (PlayerData)bf.Deserialize(file);

            unlocks = data.levelLockSave;

            file.Close();
            return unlocks;
        }
        return unlocks;
    }

    //Clear all data
    public void ClearData()
    {
        ClearSegments();
        ClearUnlocks();

        if (File.Exists(Application.persistentDataPath + Path.DirectorySeparatorChar + "playerInfo.dat"))
        {
            BinaryFormatter bf = new BinaryFormatter();
            file = File.Open(Application.persistentDataPath + Path.DirectorySeparatorChar + "playerInfo.dat", FileMode.OpenOrCreate);
            data = (PlayerData)bf.Deserialize(file);

            List<string> keyList = new List<string>(stickerManRef.GetComponent<StickerManager>().stickerDictionary.Keys);
            for (int i = 0; i < stickerManRef.GetComponent<StickerManager>().stickerDictionary.Count; i++)
            {
                stickerManRef.GetComponent<StickerManager>().stickerDictionary[keyList[i]].loaded = false;
                stickerManRef.GetComponent<StickerManager>().stickerDictionary[keyList[i]].Unlocked = false;
            }

            data.stickersSave.Clear();
            file.Close();

            for (int i = 0; i < stickerManRef.GetComponent<StickerManager>().stickerDictionary.Count; i++)
            {
                SaveStickers(stickerManRef.GetComponent<StickerManager>().stickerDictionary[keyList[i]].Name, false);
            }
        }
    }

}

//Struct for the needed variables for a segment.
[Serializable]
public struct SaveSegmentStruct
{
    public bool exists;
    public float points;
}

//All data we save into the file.
[Serializable]
class PlayerData
{
    //Lista av listor där första listan består av nivåer(kassetter) pch andra listan består av segmenten för denna lista.
    public List<List<SaveSegmentStruct>> segmentSave = new List<List<SaveSegmentStruct>>();

    //Dictionary av strings och bools, sparar namnet på en sticker och sätter boolen till antingen true eller false, baserat på om vi har fått den eller ej.
    public Dictionary<string, bool> stickersSave = new Dictionary<string, bool>();

    //Dictionary av int och bools, sparar indexet på en scen och sätter boolen till antingen true eller false, baserat på om vi har låst upp den eller ej.
    public Dictionary<int, bool> levelLockSave = new Dictionary<int, bool>();
}