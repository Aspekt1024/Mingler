using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.UI;
using System.IO;
using System.Linq;

[CustomEditor(typeof(SwipableProfile))]
public class ProfileCreatorInspector : Editor {
    
    private SwipableProfile profile;
    private bool saveButtonPressed;
    private bool loadButtonPressed;
    private int profileIndex;

    public override void OnInspectorGUI()
    {
        profile = (SwipableProfile)target;

        profile.NameText = (Text)EditorGUILayout.ObjectField("Name Text", profile.NameText, typeof(Text), true);
        profile.AgeText = (Text)EditorGUILayout.ObjectField("Age Text", profile.AgeText, typeof(Text), true);
        profile.TaglineText = (Text)EditorGUILayout.ObjectField("Tagline Text", profile.TaglineText, typeof(Text), true);
        profile.Photo = (Image)EditorGUILayout.ObjectField("Photo", profile.Photo, typeof(Image), true);
        profile.YesOverlay = (CanvasGroup)EditorGUILayout.ObjectField("Yes Overlay", profile.YesOverlay, typeof(CanvasGroup), true);
        profile.NoOverlay = (CanvasGroup)EditorGUILayout.ObjectField("No Overlay", profile.NoOverlay, typeof(CanvasGroup), true);

        EditorGUILayout.Space();

        profile.Photo.rectTransform.sizeDelta = EditorGUILayout.Vector2Field("Photo Size", profile.Photo.rectTransform.sizeDelta);
        profile.Photo.rectTransform.anchoredPosition = EditorGUILayout.Vector2Field("Photo Anchor", profile.Photo.rectTransform.anchoredPosition);
        profile.Photo.transform.position = EditorGUILayout.Vector3Field("Photo Position", profile.Photo.transform.position);
        profile.Photo.transform.localScale = EditorGUILayout.Vector3Field("Photo Scale", profile.Photo.transform.localScale);

        EditorGUILayout.Space();
        
        profile.NameText.text = EditorGUILayout.TextField("Name", profile.NameText.text);
        profile.AgeText.text = EditorGUILayout.TextField("Age", profile.AgeText.text);
        profile.TaglineText.text = EditorGUILayout.TextField("Tagline", profile.TaglineText.text);

        EditorGUILayout.Space();
        EditorGUILayout.Space();

        SetLoadSection();

        EditorGUILayout.Space();
        EditorGUILayout.Space();

        if (saveButtonPressed || GUILayout.Button("Save"))
        {
            saveButtonPressed = true;
            EditorGUILayout.LabelField("Are you sure you want to save " + profile.NameText.text + "?");
            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("Yes"))
            {
                saveButtonPressed = false;
                Save();
            }
            if (GUILayout.Button("Cancel"))
            {
                saveButtonPressed = false;
            }
            EditorGUILayout.EndHorizontal();
        }
        GUI.changed = true;
    }

    private void SetLoadSection()
    {
        string selectedProfile = GetProfileName();

        if (loadButtonPressed || GUILayout.Button("Load"))
        {
            loadButtonPressed = true;
            EditorGUILayout.LabelField("Are you sure you want to load " + selectedProfile + "?");
            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("Yes"))
            {
                loadButtonPressed = false;
                Load(selectedProfile);
            }
            if (GUILayout.Button("Cancel"))
            {
                loadButtonPressed = false;
            }
            EditorGUILayout.EndHorizontal();
        }
    }

    private string GetProfileName()
    {
        var info = new DirectoryInfo(Application.dataPath + "/Resources/Profiles");
        var profileNames = from file in info.GetFiles()
                       where file.Name.EndsWith(".json")
                       select Path.GetFileNameWithoutExtension(file.Name);

        string[] profiles = profileNames.ToArray();

        profileIndex = EditorGUILayout.Popup("Profile to load:", profileIndex, profiles);
        return profiles[profileIndex];
    }

    private void Load(string profileName)
    {
        Profile profileValues = ProfileSerializer.Load(profileName);
        profile.SetProfile(profileValues);
    }

    private void Save()
    {
        Profile profileValues = profile.GetProfile();
        ProfileSerializer.Save(profileValues);
    }
}
