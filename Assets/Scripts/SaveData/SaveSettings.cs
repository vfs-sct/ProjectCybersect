// Copyright (c) 2020 by Yuya Yoshino

using UnityEngine;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

public static class SaveSettings
{
    public static void SaveSetting(OptionsMenu settings)
    {
        BinaryFormatter formatter = new BinaryFormatter();

        string path = Application.persistentDataPath + "/settings.exq";
        FileStream stream = new FileStream(path, FileMode.Create);

        SettingsData data = new SettingsData(settings);

        formatter.Serialize(stream, data);
        stream.Close();

        Debug.Log("Settings Saved.");
    }

    public static SettingsData LoadSetting()
    {
        string path = Application.persistentDataPath + "/settings.exq";
        if(File.Exists(path))
        {
            BinaryFormatter formatter = new BinaryFormatter();
            FileStream stream =  new FileStream(path, FileMode.Open);

            SettingsData data = formatter.Deserialize(stream) as SettingsData;
            stream.Close();

            Debug.Log("Settings Loaded.");
            return data;
        }
        else
        {
            Debug.Log("File not found in " + path);

            return null;
        }
    }
}
