using System;
using UnityEngine;

public abstract class SingletonMonoBehaviour<T> : MonoBehaviour where T : MonoBehaviour
{
    [SerializeField] private bool dontDestroyOnLoad = false;
    private static T instance;
    public static T Instance
    {
        get
        {
            if (instance == null)
            {
                Type t = typeof(T);

                instance = (T)FindObjectOfType(t);
                if (instance == null)
                {
                    Debug.LogError(t + " をアタッチしているGameObjectはありません");
                }
            }

            return instance;
        }
    }
	public static T I()
	{
		return Instance;
	}

    virtual protected void Awake()
    {
        // 他のGameObjectにアタッチされているか調べる.
        // アタッチされている場合は破棄する.
        if (this != Instance)
        {
            Destroy(this.gameObject);
            return;
        }

        if (dontDestroyOnLoad)
        {
            DontDestroyOnLoad(this.gameObject);
        }
    }
    protected bool IsDontDestroyOnLoad()
    {
        return dontDestroyOnLoad;
    }
}