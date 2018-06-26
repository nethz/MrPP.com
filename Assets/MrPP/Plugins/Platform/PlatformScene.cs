using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace MrPP { 
    public class PlatformScene : MonoBehaviour {
        private void Awake()
        {
            bool isLoaded = false;
            Platform.Type type = Platform.Instance.type;
            for (int i = 0; i < SceneManager.sceneCount; ++i)
            {
                Scene scene = SceneManager.GetSceneAt(i);
                Debug.Log(scene.name);
                Debug.Log(scene.isLoaded);
                if (scene.name == type.ToString())
                {
                    isLoaded = true;
                }

            }
            if (!isLoaded)
            {
                SceneManager.LoadScene(type.ToString(), LoadSceneMode.Additive);
            }


        }
    }
}