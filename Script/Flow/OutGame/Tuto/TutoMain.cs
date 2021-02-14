using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 初回プレイ用説明画面クラス
/// </summary>
public class TutoMain : BaseFlowMain
{
    [SerializeField] GameObject m_UITuto1     = null;
    [SerializeField] GameObject m_UITuto2     = null;

    // 遷移
    enum State
    {
        start,
        tuto1,
        tuto2,
        end,
    };
    private State   m_State     = State.start;
    private TimeCnt m_TimeCnt   = new TimeCnt();

    // Start is called before the first frame update
    void Start()
    {
        if( m_UITuto1 == null )
        {
            Debug.Assert( false, "UITuto1がありません！");
            return;
        }
        if( m_UITuto2 == null )
        {
            Debug.Assert( false, "UITuto2がありません！");
            return;
        }

        m_State = State.start;
    }
    // Update is called once per frame
    void Update()
    {
        m_TimeCnt.Update();
        switch( m_State )
        {
            case State.start:
                {
                    // フェードアウト
                    FadeMgr.I().StartFadeOut( 0.5f );

                    m_State = State.tuto1;
                }
                break;
            case State.tuto1:
                {
                    if( !FadeMgr.I().IsEndFade() ) break;

                    // クリックorタッチで進行
                    if( Input.GetMouseButtonDown(0) || Input.touchCount > 0 )
                    {
                        // 決定SE
                        SoundMgr.I().PlaySe("se_btn_push");

                        // 説明1を閉じて説明2を表示
                        m_UITuto1.SetActive( false );
                        m_UITuto2.SetActive( true );

                        m_TimeCnt.StartCnt( 0.2f );
                        m_State = State.tuto2;
                    }
                }
                break;
            case State.tuto2:
                {
                    if( !m_TimeCnt.IsMaxCnt() ) break;

                    // クリックorタッチで進行
                    if( Input.GetMouseButtonDown(0) || Input.touchCount > 0 )
                    {
                        // 決定SE
                        SoundMgr.I().PlaySe("se_btn_push");

                        // レベル選択へ
                        GameParam.I().SetFlowId( Global.FLOW_ID.battle );
                        
                        // フェードイン
                        FadeMgr.I().StartFadeIn( 0.5f );
                        
                        m_State = State.end;
                    }
                }
                break;
            case State.end:
                {
                    if( !FadeMgr.I().IsEndFade() ) break;

                    // 2回目以降は表示しないからフラグを下す
                    GameParam.I().IsFirstTutorial = false;

                     m_IsEnd = true;
                }
                break;
        }
    }
}
