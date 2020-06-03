
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

public static class SaveSystem
{

    public static void SavePlayer(Dictionary<string, PlayerData> data)
    {
        BinaryFormatter formatter = new BinaryFormatter();
        string path = Application.dataPath + "/database.stars";
        FileStream stream = new FileStream(path, FileMode.Create);

        formatter.Serialize(stream, data);
        stream.Close();

    }


    public static Dictionary<string, PlayerData> LoadPlayer()
    {
        string path = Application.dataPath + "/database.stars";
        if (File.Exists(path))
        {

            BinaryFormatter formatter = new BinaryFormatter();
            FileStream stream = new FileStream(path, FileMode.Open);

            Dictionary<string, PlayerData> data = formatter.Deserialize(stream) as Dictionary<string, PlayerData>;
            stream.Close();

            return data;
        }
        else
        {

            Debug.LogError("Save file not found in" + path);
            return null;

        }
    }

    public static bool PlayerDataExists()
    {
        string path = Application.dataPath + "/database.stars";
        if (File.Exists(path))
        {
            return true;
        }
        else
        {
            return false;
        }
    }
    //ID
    public static void SavePlayerDataID(string data)
    {
        BinaryFormatter formatter = new BinaryFormatter();
        string path = Application.dataPath + "/id.stars";
        FileStream stream = new FileStream(path, FileMode.Create);

        formatter.Serialize(stream, data);
        stream.Close();

    }


    public static string LoadPlayerDataID()
    {
        string path = Application.dataPath + "/id.stars";
        if (File.Exists(path))
        {

            BinaryFormatter formatter = new BinaryFormatter();
            FileStream stream = new FileStream(path, FileMode.Open);

            string data = formatter.Deserialize(stream) as string;
            stream.Close();

            return data;
        }
        else
        {

            Debug.LogError("Save file not found in" + path);
            return null;

        }
    }

    public static bool PlayerDataIDExists()
    {
        string path = Application.dataPath + "/id.stars";
        if (File.Exists(path))
        {
            return true;
        }
        else
        {
            return false;
        }
    }
}

