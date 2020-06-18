
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

public static class SaveSystem
{
    static string starsFilename = "/playerdata.stars";

    public static void SavePlayer(List<PlayerData> data)
    {
        BinaryFormatter formatter = new BinaryFormatter();
        string path = Application.dataPath + starsFilename;
        FileStream stream = new FileStream(path, FileMode.Create);

        formatter.Serialize(stream, data);
        stream.Close();

    }


    public static List<PlayerData> LoadPlayer()
    {
        string path = Application.dataPath + starsFilename;
        if (File.Exists(path))
        {

            BinaryFormatter formatter = new BinaryFormatter();
            FileStream stream = new FileStream(path, FileMode.Open);

            List<PlayerData> data = formatter.Deserialize(stream) as List<PlayerData>;
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
        string path = Application.dataPath + starsFilename;
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

