using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RuntimeInitializer
{
    // どのシーンでも必要なシステムオブジェクトを生成
    [RuntimeInitializeOnLoadMethod( RuntimeInitializeLoadType.BeforeSceneLoad )]
    private static void InitializeBeforeSceneLoad()
    {
        // ゲーム中に常に存在するオブジェクトを読み込み、およびシーンの変更時にも破棄されないようにする。
        var soundMgr = GameObject.Instantiate( Resources.Load( "System/SoundMgr" ) );
        GameObject.DontDestroyOnLoad( soundMgr );
        var gameParam = GameObject.Instantiate( Resources.Load( "System/GameParam" ) );
        GameObject.DontDestroyOnLoad( gameParam );
        var fadeMgr = GameObject.Instantiate( Resources.Load( "System/FadeMgr" ) );
        GameObject.DontDestroyOnLoad( fadeMgr );
    }
}
