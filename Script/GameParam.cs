using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ゲーム進行形情報管理クラス
/// </summary>
public class GameParam : SingletonMonoBehaviour<GameParam>
{
    Global.FLOW_ID      m_FlowId        = Global.FLOW_ID.none;
    Global.FLOW_ID      m_PrevFlowId    = Global.FLOW_ID.none;
    Global.ENEMY_LEVEL  m_EnemyLevel    = Global.ENEMY_LEVEL.easy;

    // 初回説明用
	public bool IsFirstTutorial { get; set; }

    /// <summary>
    /// 初期化
    /// </summary>
    public void Init()
    {
        m_FlowId        = Global.FLOW_ID.none;
        m_PrevFlowId    = Global.FLOW_ID.none;
        m_EnemyLevel    = Global.ENEMY_LEVEL.easy;

        // SE音量設定
        SoundMgr.I().volume.seValue     = Global.s_DefaultSEVolume;
        SoundMgr.I().volume.bgmValue    = Global.s_DefaultBGMVolume;

        // 最初は説明を行う
        IsFirstTutorial = true;
    }

    /// <summary>
    /// フロー設定
    /// </summary>
    /// <param name="flowId">フローID</param>
    public void SetFlowId( Global.FLOW_ID flowId )
    {
        m_PrevFlowId    = m_FlowId;
        m_FlowId        = flowId;
    }
    /// <summary>
    /// 現在のフローを返す
    /// </summary>
    /// <returns>フローID</returns>
    public Global.FLOW_ID GetFlowId()
    {
        return m_FlowId;
    }
    /// <summary>
    /// 現在の前回のフローを返す
    /// </summary>
    /// <returns>前回のフローID</returns>
    public Global.FLOW_ID GetPrevFlowId()
    {
        return m_PrevFlowId;
    }

    /// <summary>
    /// 敵の強さを設定
    /// </summary>
    /// <param name="level">敵の強さ</param>
    public void SetEnemyLevel( Global.ENEMY_LEVEL level )
    {
        m_EnemyLevel = level;
    }
    /// <summary>
    /// 現在の敵の強さを返す
    /// </summary>
    /// <returns>現在の敵の強さ</returns>
    public Global.ENEMY_LEVEL GetEnemyLevel()
    {
        return m_EnemyLevel;
    }
    /// <summary>
    /// 敵のボタン押し成功率を返す
    /// </summary>
    /// <returns>敵のボタン押し成功率</returns>
    public int GetEnemyPushSuccessRate100()
    {
        return Global.s_ENEMY_LEVEL_PUSH_SUCCSESS_RATE_100[ (int)m_EnemyLevel ];
    }


    override protected void Awake()
    {
        Debug.Assert( Global.s_ENEMY_LEVEL_PUSH_SUCCSESS_RATE_100.Length == (int)Global.ENEMY_LEVEL.num, "レベルに応じたボタン押し成功率の定義数エラー");

        Init();
	}
}
