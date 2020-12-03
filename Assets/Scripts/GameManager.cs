using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using System.IO;

public class GameManager : MonoBehaviour {
    Texture myTexture;
    [SerializeField]
    Image image;
    object WebPullLock = new object();

    string uriString;

    public void SetURI(string uri) {
        uriString = uri;
    }
    public void LoadTexture() {
        lock (WebPullLock) {
            StartCoroutine(GetTexture(uriString));
        }
    }

    IEnumerator GetTexture(string uri) {
        bool isValid = false;
        try {
            isValid = new Uri(uri).IsWellFormedOriginalString();
            if (!isValid && Path.IsPathRooted(uri)) {
                uri = Path.GetFullPath(uri);
                isValid = true;
            }
        }
        catch (Exception e) {
            Debug.Log(e.Message);
        }
        if (isValid) {
            UnityWebRequest request = UnityWebRequestTexture.GetTexture(uri);
            yield return request.SendWebRequest();
            try {
                if (request.isNetworkError || request.isHttpError) {
                    Debug.Log("Error getting: " + uri);
                    Debug.Log(request.error);
                    string path = Application.streamingAssetsPath + "/404-error.png";
                    Debug.Log(path);
                    StartCoroutine(GetTexture(path));
                }
                else {
                    myTexture = ((DownloadHandlerTexture)request.downloadHandler).texture;
                    image.sprite = Sprite.Create((Texture2D)myTexture, new Rect(Vector2.zero, new Vector2(myTexture.width, myTexture.height)), Vector2.zero);
                }
                
            }
            catch (Exception e) {
                GetTexture(Application.streamingAssetsPath + "/404-error.png");
                Debug.Log(e.Message);
            }
        } 
    }
}

