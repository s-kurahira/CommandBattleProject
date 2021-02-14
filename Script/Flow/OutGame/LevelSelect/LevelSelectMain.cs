using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// レベル選択画面クラス
/// </summary>
public class LevelSelectMain : BaseFlowMain
{
    [SerializeField] Button m_UIBtnEasy     = null;
    [SerializeField] Button m_UIBtnNormal   = null;
    [SerializeField] Button m_UIBtnHard     = null;

    // 遷移
    enum State
    {
        start,
        wait,
        select,
        end,
    };
    State m_State = State.start;

    /// <summary>
    /// 難易度ボタンを押下
    /// </summary>
    /// <param name="level">敵の強さ</param>
    public void PushBtnLevelSelect( Global.ENEMY_LEVEL level )
    {
        GameParam.I().SetEnemyLevel( level );

        // 決定SE
        SoundMgr.I().PlaySe("se_btn_push");

        // 初回説明フラグが立ってたら
        // 説明シーンへ
        if( GameParam.I().IsFirstTutorial )
        {
            // バトル選択へ
            GameParam.I().SetFlowId( Global.FLOW_ID.tuto );
        }
        else
        {
            // バトル選択へ
            GameParam.I().SetFlowId( Global.FLOW_ID.battle );
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        if( m_UIBtnEasy == null )
        {
            Debug.Assert( false, "m_UIBtnEasyがありません！");
            return;
        }
        if( m_UIBtnNormal == null )
        {
            Debug.Assert( false, "m_UIBtnNormalがありません！");
            return;
        }
        if( m_UIBtnHard == null )
        {
            Debug.Assert( false, "m_UIBtnHardがありません！");
            return;
        }

        // ボタン押し関数設定
        m_UIBtnEasy.onClick.AddListener( () => PushBtnLevelSelect( Global.ENEMY_LEVEL.easy ) );
        m_UIBtnNormal.onClick.AddListener( () => PushBtnLevelSelect( Global.ENEMY_LEVEL.normal ) );
        m_UIBtnHard.onClick.AddListener( () => PushBtnLevelSelect( Global.ENEMY_LEVEL.hard ) );

        // BGM再生
        SoundMgr.I().PlayBgm("bmg_select");

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

                    m_State = State.wait;
                }
                break;
            case State.wait:
                {
                    if( !FadeMgr.I().IsEndFade() ) break;

                    // ボタン有効
                    //SetDisableAllBtn( true );

                    m_State = State.select;
                }
                break;
            case State.select:
                {
                    // 進行していたら次へ
                    if( GameParam.I().GetFlowId() != Global.FLOW_ID.levelSelect )
                    {
                        // ボタン無効
                        //SetDisableAllBtn( false );

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
