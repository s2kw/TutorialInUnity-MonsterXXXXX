using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Singleton<T> : MonoBehaviour where T : MonoBehaviour
{
    private static T instance;

    public static T Instance
    {
        get
        {
            if (instance == null)
            {
                Debug.LogWarning($"{typeof(T).Name} Instanceが生成されていません。");
            }

            return instance;
        }
    } 
    private void Awake()
    {
        if (instance != null)
        {
            Debug.LogWarning($"{typeof(T).Name} Instanceが重複して生成されています。",this);
            Destroy(this);
        }

        instance = this as T;
    }
}
