using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

public class SaveSystem : MonoBehaviour
{
    public static SaveSystem saveSystem;
    
    //public bool temp = true;
    //public bool temp1 = true;

    void Awake()
    {
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

    // Used for testing, if working properly please remove me
    /*void Update()
    {
        PlayerData data = new PlayerData();

        if (temp == true)
        {
            SaveSegmentStruct A = new SaveSegmentStruct();
            A.points = 1;

            SaveSegmentStruct B = new SaveSegmentStruct();
            B.points = 1;
            SaveSegmentStruct F = new SaveSegmentStruct();
            F.points = 1;
            SaveSegmentStruct G = new SaveSegmentStruct();
            G.points = 1;

            data.segmentSave.Add(new List<SaveSegmentStruct> { A, B, F, G });

            SaveSegmentStruct C = new SaveSegmentStruct();
            C.points = 1;

            SaveSegmentStruct D = new SaveSegmentStruct();
            D.points = 1;

            SaveSegmentStruct E = new SaveSegmentStruct();
            E.points = 1;

            data.segmentSave.Add(new List<SaveSegmentStruct> { C, D, E });
            
            data.segmentSave[0][1] = A;

            temp = false;
        }

        if(temp1 == true)
        {
            print(data.segmentSave.Count);
            print(data.segmentSave[0].Count);
            print(data.segmentSave[1].Count);

            temp1 = false;
        }
    }*/

    public void Save(SaveSegmentStruct save, int levelIndex, int segmentIndex)
    {
        BinaryFormatter bf = new BinaryFormatter();
        FileStream file = File.Create(Application.persistentDataPath + "/playerInfo.dat");      // According to random source this might need "//playerInfo.dat" instead for android, must be tested

        PlayerData data = new PlayerData();

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
    Continue:
        data.segmentSave[levelIndex][segmentIndex] = save;

        bf.Serialize(file, data);
        file.Close();
    }

    public void Load()
    {
        if (File.Exists(Application.persistentDataPath + "/playerInfo.dat"))
        {
            BinaryFormatter bf = new BinaryFormatter();
            FileStream file = File.Open(Application.persistentDataPath + "/playerInfo.dat", FileMode.Open);
            PlayerData data = (PlayerData)bf.Deserialize(file);
            file.Close();

            //Needs Completed Levels to implement

        }
    }
}

//WIP, behöver fyllas med exakt vad som måste sparas för varje segment. 
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
}