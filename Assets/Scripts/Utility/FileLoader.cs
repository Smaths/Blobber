using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Networking;

namespace Utility
{
    public class FileLoader : MonoBehaviour
    {
        public UnityEvent<List<string>> OnFileLoaded;

        public void LoadFileFromAssets(string fileName)
        {
            StartCoroutine(LoadFileRoutine(fileName));
        }

        private IEnumerator LoadFileRoutine(string fileName)
        {
            string filePath = "file://" + Application.dataPath + fileName;

            if (Application.platform == RuntimePlatform.WebGLPlayer)
                filePath = "file://" + Application.dataPath + "/StreamingAssets/" + fileName;

            UnityWebRequest request = UnityWebRequest.Get(filePath);
            yield return request.SendWebRequest();

            if (request.result != UnityWebRequest.Result.ConnectionError && request.result != UnityWebRequest.Result.ProtocolError)
            {
                string content = request.downloadHandler.text;
                Debug.Log("File contents: " + content);

                List<string> foo = ParseCSV('\n', content);
                OnFileLoaded?.Invoke(foo);
            }
            else
            {
                Debug.LogError("Error loading file: " + request.error);
            }

            request.Dispose();
        }

        private static List<string> ParseCSV(char separator, string csvText)
        {
            string[] lines = csvText.Split(separator);
            return lines.Select(line => line.Trim()).ToList();
        }
    }
}