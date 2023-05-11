using System;
using System.Collections;
using System.Net;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class IPSetter : MonoBehaviour
{
    [SerializeField] private int nextSceneIndex;
    [SerializeField] private TMP_Text displayText;
    
    private bool blocked = false;

    private string entry = "192.168.178.52:8569";

    private const string playerPrefsIpIdentifier = "IP";
    private const string playerPrefsPortIdentifier = "Port";

    
    private void Start()
    {
        if (PlayerPrefs.HasKey(playerPrefsIpIdentifier))
        {
            entry = PlayerPrefs.GetString(playerPrefsIpIdentifier) + ":" +
                     PlayerPrefs.GetString(playerPrefsPortIdentifier);
            displayText.text = entry;
        }
    }
    
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            LoadNextScene();
        }
    }
    
    
    public void SetValue(string s)
    {
        if (!blocked)
        {
            entry = entry + s;
            displayText.text = entry;
            StartCoroutine(InputBlockRoutine());
        }
    }

    public void SetIP()
    {
        IPAddress ipAdress;

        if (IPAddress.TryParse(entry, out ipAdress))
        {
            PlayerPrefs.SetString(playerPrefsIpIdentifier,ipAdress.ToString());
            LoadNextScene();
            return;
        }
            
        for (int i = 0; i < entry.Length; i++)
        {
            if (entry[i].Equals(':'))
            {
                string s = "";
                for (int j = 0; j < i; j++)
                {
                    s = s + entry[j];
                }
                if(IPAddress.TryParse(s, out ipAdress)) PlayerPrefs.SetString(playerPrefsIpIdentifier, s);

                string t = "";
                for (int j = i+1; j < entry.Length; j++)
                {
                    t = t + entry[j];
                }

                int l = int.Parse(t);
                if (l > 65535) return;
                PlayerPrefs.SetString(playerPrefsPortIdentifier, t);
                break;
            }
        }
        LoadNextScene();
    } 
    
    public void EnterValue(string s)
    {
        if (!blocked)
        {
            entry = entry + s;
            displayText.text = entry;
            StartCoroutine(InputBlockRoutine());
        }
    }
    
    public void RemoveValue()
    {
        if(!String.IsNullOrEmpty(entry)) entry = entry.Remove(entry.Length - 1, 1);
        
        displayText.text = entry;
    }

    private IEnumerator InputBlockRoutine()
    {
        blocked = true;
        yield return new WaitForSeconds(0.4f);
        blocked = false;
    }


    private void LoadNextScene()
    {
        SceneManager.LoadScene(nextSceneIndex);
    }
}