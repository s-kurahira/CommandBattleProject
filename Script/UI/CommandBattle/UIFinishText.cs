﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// バトル用終了UIクラス
/// </summary>
public class UIFinishText : MonoBehaviour
{
	[SerializeField] private Image      m_Image     = null;
	[SerializeField] private UIFade     m_UIFade    = null;

    // 表示状態
    private enum VIEW_STATE
    {
        none,
        start,
        end,
    };
    private VIEW_STATE m_ViewState = VIEW_STATE.none;

    /// <summary>
    /// 表示開始
    /// </summary>
    /// <param name="fadeTimeSec">フェード時間</param>
    public void StartView( float fadeTimeSec )
    {
        this.gameObject.SetActive( true );

        // フェードして表示
        m_UIFade.StartFadeIn( fadeTimeSec );

        m_ViewState = VIEW_STATE.start;
    }
    /// <summary>
    /// 表示終了
    /// </summary>
    /// <param name="fadeTimeSec">フェード時間</param>
    public void EndView( float fadeTimeSec )
    {
        // フェードアウト
        m_UIFade.StartFadeOut( fadeTimeSec );

        m_ViewState = VIEW_STATE.end;
    }

    private void Awake()
    {
       if( m_Image == null )
        {
            Debug.Assert( false, "Imageがありません！");
            return;
        } 
       if( m_UIFade == null )
        {
            Debug.Assert( false, "UIFadeがありません！");
            return;
        }
    }
    void Update()
    {
        if( m_ViewState == VIEW_STATE.end )
        {
            // フェードアウト終了していたら活動停止
            if( m_UIFade.IsEndFade() )
            {
                m_ViewState = VIEW_STATE.none;
                this.gameObject.SetActive( false );
            }
        }
    }
}
