using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

public class SaveSystem : MonoBehaviour
{
    public static SaveSystem saveSystem;

    [SerializeField] private GameObject stickerManRef;
    PlayerData data = new PlayerData();

    void Awake()
    {
        // Make sure we only have one instance of the save system and make sure it won't be destroyed between scenes
        if (saveSystem == null)
        {
            DontDestroyOnLoad(gameObject);
            saveSystem = this;
        }
        else if (saveSystem != this)
        {
            Destroy(gameObject);
        }
    }

    public void SaveSegment(SaveSegmentStruct save, int levelIndex, int segmentIndex)
    {
        BinaryFormatter bf = new BinaryFormatter();
        FileStream file = File.Create(Application.persistentDataPath + "/playerInfo.dat");      // According to random source this might need "//playerInfo.dat" instead for android, must be tested
        
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
                    data.segmentSave.Add(new List<SaveSegmentStruct> { emptyStruct });
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

    public void SaveStickers(string name, bool earned)
    {
        BinaryFormatter bf = new BinaryFormatter();
        FileStream file = File.Create(Application.persistentDataPath + "/playerInfo.dat");      // According to random source this might need "//playerInfo.dat" instead for android, must be tested
        
        //Check if specific key exists in the save file, if it does we just save otherwise we create a new one
        if(data.stickersSave.ContainsKey(name))
        {
            data.stickersSave[name] = earned;
        }
        else
        {
            data.stickersSave.Add(name, earned);
        }
        //Serialize the values and save
        bf.Serialize(file, data);
        file.Close();
    }

    public void LoadStickers(Dictionary<string, Sticker> stickersRef)
    {
        if (File.Exists(Application.persistentDataPath + "/playerInfo.dat"))
        {
            BinaryFormatter bf = new BinaryFormatter();
            FileStream file = File.Open(Application.persistentDataPath + "/playerInfo.dat", FileMode.Open);
            data = (PlayerData)bf.Deserialize(file);

            //Get all keys and update the values in the sticker album, basically check if we have earned a sticker
            //and make sure we also have it unlocked again when we start the game anew
            List<string> keyList = new List<string>(stickersRef.Keys);
            for (int i = 0; i < data.stickersSave.Count; i++)
            {
                if (!data.stickersSave.ContainsKey(keyList[i]))
                {
                    data.stickersSave.Add(stickerManRef.GetComponent<StickerManager>().stickers[keyList[i]].Name, false);
                }
                else if(data.stickersSave[keyList[i]] == true)
                {
                    stickerManRef.GetComponent<StickerManager>().stickers[keyList[i]].loaded = true;
                    stickerManRef.GetComponent<StickerManager>().stickers[keyList[i]].Unlocked = data.stickersSave[keyList[i]];
                }
            }

            file.Close();
        }
    }

    //WIP
    public void Load()
    {
        if (File.Exists(Application.persistentDataPath + "/playerInfo.dat"))
        {
            BinaryFormatter bf = new BinaryFormatter();
            FileStream file = File.Open(Application.persistentDataPath + "/playerInfo.dat", FileMode.Open);
            data = (PlayerData)bf.Deserialize(file);
            file.Close();

            //Needs Completed Levels to implement

        }
    }
    
    //Clears the earned stickers, unsure whether we want to use this later or not. 
    //This is currently only used for testing the stickers.
    public void ClearStickers(Dictionary<string, Sticker> stickersRef)
    {
        BinaryFormatter bf = new BinaryFormatter();
        FileStream file = File.Open(Application.persistentDataPath + "/playerInfo.dat", FileMode.Open);
        data = (PlayerData)bf.Deserialize(file);

        List<string> keyList = new List<string>(stickersRef.Keys);
        for (int i = 0; i < stickerManRef.GetComponent<StickerManager>().stickers.Count; i++)
        {
            stickerManRef.GetComponent<StickerManager>().stickers[keyList[i]].loaded = false;
            stickerManRef.GetComponent<StickerManager>().stickers[keyList[i]].Unlocked = false;
        }

        data.stickersSave.Clear();
        file.Close();

        for (int i = 0; i < stickerManRef.GetComponent<StickerManager>().stickers.Count; i++)
        {
            SaveStickers(stickerManRef.GetComponent<StickerManager>().stickers[keyList[i]].Name, false);
        }
    }
}

//WIP, behöver fyllas med exakt vad som måste sparas för varje segment. 
[Serializable]
public struct SaveSegmentStruct
{
    // Ljudfil för sparad inspelning?
    public bool exists;
    public float points;

}

[Serializable]
class PlayerData
{
    //Lista av listor där första listan består av nivåer(kassetter) pch andra listan består av segmenten för denna lista.
    public List<List<SaveSegmentStruct>> segmentSave = new List<List<SaveSegmentStruct>>();

    //Dictionary av strings och bools, sparar namnet på en sticker och sätter boolen till antingen true eller false, baserat på om vi har fått den eller ej.
    public Dictionary<string, bool> stickersSave = new Dictionary<string, bool>();
}