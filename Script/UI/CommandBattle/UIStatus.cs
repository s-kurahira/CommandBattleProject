using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// バトル用ステータス表示UIクラス
/// </summary>
public class UIStatus : MonoBehaviour
{
	[SerializeField] private UIHpBar  m_HpBarPlayer     = null;
	[SerializeField] private UIHpBar  m_HpBarEnemy      = null;

    /// <summary>
    /// HP表示初期化
    /// </summary>
    /// <param name="playerId">プレイヤー指定</param>
    /// <param name="hpMax">最大HP</param>
    public void InitHpBar( Global.PlayerId playerId, int hpMax )
    {
        if( m_HpBarPlayer == null ||
            m_HpBarEnemy == null )
        {
            return;
        }

        if( playerId == Global.PlayerId.player )
        {
            m_HpBarPlayer.Init( hpMax );
        }
        else
        {
            m_HpBarEnemy.Init( hpMax );
        }
    }
    /// <summary>
    /// HP変動開始
    /// </summary>
    /// <param name="playerId">プレイヤー指定</param>
    /// <param name="hp">目標HP</param>
    public void StartHpUpdate( Global.PlayerId playerId, int hp )
    {
        if( m_HpBarPlayer == null ||
            m_HpBarEnemy == null )
        {
            return;
        }

        if( playerId == Global.PlayerId.player )
        {
            m_HpBarPlayer.StartHpUpdate( hp );
        }
        else
        {
            m_HpBarEnemy.StartHpUpdate( hp );
        }
    }

    private void Awake()
    {
        if( m_HpBarPlayer == null )
        {
            Debug.Assert(false, "playerのhpBarがありません！");
            return;
        }
        if( m_HpBarEnemy == null )
        {
            Debug.Assert(false, "enemyのhpBarがありません！");
            return;
        }
    }
}
