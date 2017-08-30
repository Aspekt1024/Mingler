using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;
using UnityEngine.Serialization;

#if UNITY_EDITOR
using UnityEditor;
#endif

public static class ProfileSerializer {

    public static void Save(Profile profile)
    {
        string path = "/Resources/Profiles/" + profile.Name + ".json";
        string json = JsonUtility.ToJson(profile);
        File.WriteAllText(Application.dataPath + path, json);
#if UNITY_EDITOR
        AssetDatabase.ImportAsset("Assets" + path);
#endif
        Debug.Log(path + " saved!");
    }

    public static Profile Load(string name)
    {
        TextAsset json = Resources.Load<TextAsset>("Profiles/" + name);
        string jsonString = json.text;
        return JsonUtility.FromJson<Profile>(jsonString);
    }
}

[Serializable]
public class Profile
{
    public string Name;
    public string Age;
    public string Tagline;
    public Sprite Photo;
    public Vector2 PhotoSize;
    public Vector3 PhotoScale;
    public Vector2 PhotoAnchor;
    public Vector3 PhotoPosition;
}
