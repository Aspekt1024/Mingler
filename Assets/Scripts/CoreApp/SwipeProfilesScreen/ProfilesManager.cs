using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProfilesManager : MonoBehaviour {

    private static ProfilesManager manager;
    public static ProfilesManager Instance
    {
        get { return manager; }
    }

    private List<SwipableProfile> profiles = new List<SwipableProfile>();
    private int numProfiles;
    private int profilesSwiped;
    public RectTransform panelCenter;

    private void Awake()
    {
        if (manager != null)
        {
            Debug.LogError("Detected more than one Profiles Manager in the scene!");
            enabled = false;
        }
        else
        {
            manager = FindObjectOfType<ProfilesManager>();
            if (manager == null)
            {
                Debug.LogError("You need to add a ProfilesManager to the scene!");
            }
        }
    }

    
    public void StartAgain()
    {
        profilesSwiped = 0;
        Debug.Log("starting again");
    }

    public void ProfileSwiped()
    {
        profilesSwiped++;
        if (profilesSwiped == numProfiles)
        {
            StartAgain();
        }
    }

    private void Start()
    {
        var profileList = Resources.LoadAll("Profiles");
        numProfiles = profileList.Length;

        foreach(Object profileObj in profileList)
        {
            Profile loadedProfile = ProfileSerializer.Load(profileObj.name);
            SwipableProfile newProfile = Instantiate(Resources.Load<GameObject>("Prefabs/ProfilePanel")).GetComponent<SwipableProfile>();
            newProfile.transform.SetParent(transform);
            newProfile.transform.localScale = Vector3.one;
            newProfile.transform.position = new Vector3(0f, -6f, 0);
            newProfile.GetComponent<RectTransform>().anchoredPosition = panelCenter.anchoredPosition;

            newProfile.SetProfile(loadedProfile);
            profiles.Add(newProfile);
        }
    }

}
