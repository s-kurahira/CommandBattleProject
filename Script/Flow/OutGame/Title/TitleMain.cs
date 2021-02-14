using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// タイトル画面クラス
/// </summary>
public class TitleMain : BaseFlowMain
{
    // 遷移
    enum State
    {
        start,
        select,
        end,
    };
    State m_State = State.select;

    // Start is called before the first frame update
    void Start()
    {
        // BGM再生
        SoundMgr.I().PlayBgm("bgm_title");

        m_State = State.start;
    }
    // Update is called once per frame
    void Update()
    {
        switch( m_State )
        {
            case State.start:
                {
                    // フェードアウト
                    FadeMgr.I().StartFadeOut( 0.5f );

                    m_State = State.select;
                }
                break;
            case State.select:
                {
                    if( !FadeMgr.I().IsEndFade() ) break;

                    // クリックorタッチで進行
                    if( Input.GetMouseButtonDown(0) || Input.touchCount > 0 )
                    {
                        // 決定SE
                        SoundMgr.I().PlaySe("se_btn_push");

                        // レベル選択へ
                        GameParam.I().SetFlowId( Global.FLOW_ID.levelSelect );
                        
                        // フェードイン
                        FadeMgr.I().StartFadeIn( 0.5f );
                        
                        m_State = State.end;
                    }
                }
                break;
            case State.end:
                {
                    if( !FadeMgr.I().IsEndFade() ) break;

                     m_IsEnd = true;
                }
                break;
        }
    }
}
