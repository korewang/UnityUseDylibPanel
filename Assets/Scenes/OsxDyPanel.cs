using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

namespace OsxDYForUnityPanel
{
    public class OsxDyPanel : MonoBehaviour
{
    [SerializeField] private Button _button;

    [SerializeField] private RawImage _rawImage;
    // Start is called before the first frame update
    void Start()
    {
        _button.onClick.AddListener(OnSelectorFile);
    }

    private void OnSelectorFile()
    {
        string[] paths = OpenFileDialog("open File", "", "jpg,png,jpeg", false);
        Debug.Log(paths.Length);
        if (paths.Length > 0 && paths[0] != String.Empty) { 
            StartCoroutine(LoadImage(paths[0]));
        }
    }

    IEnumerator LoadImage(string path)
    {
        UnityWebRequest unityWebRequest = new UnityWebRequest(new System.Uri(path).AbsoluteUri);
        DownloadHandlerTexture Textures = new DownloadHandlerTexture(true);
        unityWebRequest.downloadHandler = Textures;
        yield return unityWebRequest.SendWebRequest();
        if (unityWebRequest.result != UnityWebRequest.Result.ConnectionError)
        {
            _rawImage.texture = Textures.texture;
        }
    }
    
    [DllImport("OsxBundleForUnityPanel")]
    private static extern IntPtr OsxOpenFilesDialog(string title, string directory, string extension, bool multFile);
    
        
    public string[] OpenFileDialog(string title, string directory, string extensions, bool multFile) {
        var extension = string.IsNullOrEmpty(extensions) ? null : new [] { new ExtensionFilter("", extensions) };
        var paths = Marshal.PtrToStringAnsi(OsxOpenFilesDialog(
            title,
            directory,
            GetFileExtensionTypeList(extension),
            multFile));
        return paths.Split((char)28);
    }
    
    // Update is called once per frame
    void Update()
    {
        
    }
    
    
    public static string GetFileExtensionTypeList(ExtensionFilter[] extensions) {
        if (extensions == null) {
            return "";
        }

        var filterString = "";
        foreach (var filter in extensions) {
            filterString += filter.Name + ";";

            foreach (var ext in filter.Extensions) {
                filterString += ext + ",";
            }

            filterString = filterString.Remove(filterString.Length - 1);
            filterString += "|";
        }
        filterString = filterString.Remove(filterString.Length - 1);
            
        return filterString;
    }
    public struct ExtensionFilter {
        public string Name;
        public string[] Extensions;

        public ExtensionFilter(string filterName, params string[] filterExtensions) {
            Name = filterName;
            Extensions = filterExtensions;
        }
    }
}
}


