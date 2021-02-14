using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 状態遷移管理クラス
/// </summary>
public class State<T>
{
	class StateAction
	{
		public T m_Tag;
		public Action m_Init;
		public Action m_Update;
		public Action m_Exit;

		public StateAction(T tag, Action init, Action update, Action exit)
		{
			m_Tag = tag;
			m_Init = init;
			m_Update = update;
			m_Exit = exit;
		}
	}

	Dictionary<T, StateAction> m_StateDic;
	StateAction m_Now;
	T m_PrevTag;
	int m_SubStateNow;
	int m_SubStatePrev;

    /// <summary>
    /// 現在の遷移
    /// </summary>
	public T CurrentState { get { return m_Now.m_Tag; } }

    /// <summary>
    /// 移行前の遷移
    /// </summary>
	public T PrevState
	{
		get
		{
			return m_PrevTag;
		}
	}

    /// <summary>
    /// 現在の遷移を返す
    /// </summary>
    /// <returns>現在の遷移</returns>
	public State()
	{
		m_StateDic = new Dictionary<T, StateAction>();
	}

    /// <summary>
    /// 遷移情報を追加
    /// </summary>
    /// <param name="tag">遷移</param>
    /// <param name="init">初期化関数</param>
    /// <param name="update">更新関数</param>
    /// <param name="exit">終了関数</param>
    public void Add( T tag, Action init, Action update, Action exit = null )
    {
		if( !m_StateDic.ContainsKey( tag ) )
		{
			m_StateDic.Add( tag, new StateAction( tag, init, update, exit ) );
		}
    }

    /// <summary>
    /// 遷移を設定
    /// </summary>
	public void Set( T tag, int subState = 0 )
	{
		if( m_Now != null )
		{
			if( m_Now.m_Exit != null )
			{
				m_Now.m_Exit();
			}
			m_PrevTag = m_Now.m_Tag;
			m_Now = null;
		}
		m_Now = m_StateDic[tag];

		if( m_Now != null )
		{
			if( m_Now.m_Init != null )
			{
				m_Now.m_Init();
			}
		}

        m_SubStatePrev  = m_SubStateNow;
        m_SubStateNow = subState;
    }

    /// <summary>
    /// 現在のサブ遷移を返す
    /// </summary>
    public int GetSubState()
    {
        return m_SubStateNow;
    }
    /// <summary>
    /// 移行前のサブ遷移を返す
    /// </summary>
    /// <returns>移行前のサブ遷移</returns>
    public int GetPrevSubState()
    {
        return m_SubStatePrev;
    }
    /// <summary>
    /// サブ遷移を設定
    /// </summary>
    /// <param name="subState">サブ遷移</param>
    public void SetSubState( int subState )
    {
        m_SubStatePrev = m_SubStateNow;
        m_SubStateNow = subState;
    }

    /// <summary>
    /// 更新
    /// </summary>
    public void Update()
    {
        if( m_Now == null )
		{
			return;
		}

		if( m_Now.m_Update != null )
		{
			m_Now.m_Update();
		}
    }
}

