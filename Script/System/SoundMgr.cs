using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// サウンド音量情報
/// </summary>
[System.Serializable]
public class SoundVolume
{
    public float    bgmValue    = Global.s_DefaultBGMVolume;
    public float    seValue     = Global.s_DefaultSEVolume;
    public bool     mute        = false;

    public void Reset()
    {
        bgmValue    = Global.s_DefaultBGMVolume;
        seValue     = Global.s_DefaultSEVolume;
        mute = false;
    }
}

/// <summary>
/// サウンド管理クラス
/// </summary>
public class SoundMgr : SingletonMonoBehaviour<SoundMgr>
{
    public SoundVolume volume = new SoundVolume();

    private AudioClip[] m_SeClips;
    private AudioClip[] m_BgmClips;

    private Dictionary<string, int> m_SeIndexes     = new Dictionary<string, int>();
    private Dictionary<string, int> m_BgmIndexes    = new Dictionary<string, int>();

    const int cNumChannel = 16;
    private AudioSource m_BgmSource = null;
    private AudioSource[] m_SeSources = new AudioSource[cNumChannel];

    override protected void Awake()
    {
        // 他のGameObjectにアタッチされているか調べる.
        // アタッチされている場合は破棄する.
        if( this != Instance )
        {
            Destroy(this);
            return;
        }
        if( IsDontDestroyOnLoad() )
        {
            DontDestroyOnLoad(this.gameObject);
        }

        m_BgmSource = gameObject.AddComponent<AudioSource>();
        m_BgmSource.loop = true;

        for( int i = 0; i < m_SeSources.Length; i++ )
        {
            m_SeSources[i] = gameObject.AddComponent<AudioSource>();
        }

        m_SeClips   = Resources.LoadAll<AudioClip>("Sound/SE");
        m_BgmClips  = Resources.LoadAll<AudioClip>("Sound/BGM");

        for( int i = 0; i < m_SeClips.Length; ++i )
        {
            m_SeIndexes[m_SeClips[i].name] = i;
        }

        for( int i = 0; i < m_BgmClips.Length; ++i )
        {
            m_BgmIndexes[m_BgmClips[i].name] = i;
        }
    }
    void Update()
    {
        m_BgmSource.mute = volume.mute;
        foreach( var source in m_SeSources )
        {
            source.mute = volume.mute;
        }

        m_BgmSource.volume = volume.bgmValue;
        foreach( var source in m_SeSources )
        {
            source.volume = volume.seValue;
        }
    }

    /// <summary>
    /// 再生SEのidxを取得
    /// </summary>
    private int playSeImpl( int index )
    {
        if( 0 > index || m_SeClips.Length <= index )
        {
            return -1;
        }

        for( int idx_i = 0; idx_i < cNumChannel; idx_i++ )
        {
            if( false == m_SeSources[idx_i].isPlaying )
            {
                m_SeSources[idx_i].clip = m_SeClips[index];
                m_SeSources[idx_i].Play();
                return idx_i;
            }
        }

        return -1;
    }

    /// <summary>
    /// SE名からindexを返す
    /// </summary>
    /// <param name="name">SE名</param>
    /// <returns>SEのindex</returns>
    public int GetSeIndex( string name )
    {
        return m_SeIndexes[name];
    }

    /// <summary>
    /// BGM名からindexを返す
    /// </summary>
    /// <param name="name">BGM名</param>
    /// <returns>BGMのindex</returns>
    public int GetBgmIndex( string name )
    {
        return m_BgmIndexes[name];
    }

    /// <summary>
    /// BGM再生
    /// </summary>
    /// <param name="name">BGM名</param>
    public void PlayBgm( string name )
    {
        int index = m_BgmIndexes[name];
        PlayBgm(index);
    }

    /// <summary>
    /// BGM再生
    /// <param name="index">BGMのindex</param>
    public void PlayBgm( int index )
    {
        if( 0 > index || m_BgmClips.Length <= index )
        {
            return;
        }

        if( m_BgmSource.clip == m_BgmClips[index] )
        {
            return;
        }

        m_BgmSource.Stop();
        m_BgmSource.clip = m_BgmClips[index];
        m_BgmSource.Play();
    }

    /// <summary>
    /// BGM停止
    /// </summary>
    public void StopBgm()
    {
        m_BgmSource.Stop();
        m_BgmSource.clip = null;
    }

    /// <summary>
    /// 指定SE再生
    /// </summary>
    /// <param name="name">SE名</param>
    public int PlaySe( string name )
    {
        return PlaySe(GetSeIndex(name));
    }
    /// <summary>
    /// 指定SE再生
    /// </summary>
    /// <param name="index">SEのindex</param>
    public int PlaySe( int index )
    {
        return playSeImpl(index);
    }

    /// <summary>
    /// 指定SE停止
    /// </summary>
    /// <param name="playIdx">再生中SEのidx</param>
    public void StopSe( int playIdx )
    {
        if( playIdx < 0 || playIdx > cNumChannel )
        {
            return;
        }

        m_SeSources[playIdx].Stop();
        m_SeSources[playIdx].clip = null;
    }

    /// <summary>
    /// 全SE停止
    /// </summary>
    public void StopAllSe()
    {
        foreach( AudioSource source in m_SeSources )
        {
            source.Stop();
            source.clip = null;
        }
    }
}