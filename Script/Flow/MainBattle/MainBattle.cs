using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

/// <summary>
/// コマンドコウゲキ情報
/// </summary>
[System.Serializable]
public class CommandAttackData {
    public Global.CommandType       type        = Global.CommandType.punch;
	public int                      damageValue = 1;
    public CharaCntrol.ANIM_TYPE    animType    = CharaCntrol.ANIM_TYPE.punch;
}

/// <summary>
/// バトルフローメインクラス
/// </summary>
public class MainBattle : MonoBehaviour
{
    // 共有用
    [SerializeField] private CharaCntrolMgr     m_CharaCntrolMgr    = null;
    [SerializeField] private int                m_PlayerMaxHp       = 100;
    [SerializeField] private int                m_EnemyMaxHp        = 100;
    [SerializeField] private int                m_TimerSecMax       = 5;

    [SerializeField] private List<CommandAttackData>  m_CommandAttackDataList = new List<CommandAttackData>();

    private int             m_PlayerHp      = 100;
    private int             m_EnemyHp       = 100;

    private Global.PlayerId m_WinPlayerId   = Global.PlayerId.player;

    // コマンド入力シーン用
    [SerializeField] GameObject             m_CommandPanel          = null;
    [SerializeField] GameObject             m_ActionPanel           = null;
    [SerializeField] UITimer                m_UITimer               = null;
    [SerializeField] UIStatus               m_UIStatus              = null;
    [SerializeField] UIAttackText           m_UIAttackText          = null;
    [SerializeField] UIFinishText           m_UIFinishText          = null;
    [SerializeField] UIResultText           m_UIResultText          = null;
    [SerializeField] Button                 m_UIAttackBtnP          = null;
    [SerializeField] Button                 m_UIAttackBtnK          = null;
    [SerializeField] UITargetPickUp         m_UITargetPickUp        = null;
    [SerializeField] List<UITargetCommand>  m_UITargetCommandList   = new List<UITargetCommand>();
    [SerializeField] List<UIPushResult>     m_UIPushResultList      = new List<UIPushResult>();

    private int         m_CommandInputCnt       = 0;
    private int         m_CharaAttackCnt        = 0;
    private List<bool>  m_IsInputSuccessList    = null;

    private const int   s_ENEMY_PUSH_SUCCESS_RATE = 50; // 敵の入力成功率

    // 遷移
    private enum STATE
    {
        none,

        start,          // 開始
        playerAttack,   // プレイヤー攻撃
        enemyAttack,    // 敵コウゲキ
        result,         // 結果
        end,            // 終了
    };
    private State<STATE>    m_State     = new State<STATE>();
    private TimeCnt         m_TimeCnt   = new TimeCnt();

    /// <summary>
    /// コマンドコウゲキ情報を返す
    /// </summary>
    /// <param name="type">コマンド種類</param>
    /// <returns>コマンドコウゲキ情報</returns>
    private CommandAttackData GetCommandAttackData( Global.CommandType type )
    {
        foreach( CommandAttackData data in m_CommandAttackDataList.ToArray() )
        {
            if( type == data.type )
            {
                return data;
            }
        }

        return null;
    }

    /// <summary>
    /// コウゲキ ボタン押し
    /// </summary>
    /// <param name="type">コマンド種類</param>
    public void PushAttackBtn( Global.CommandType type )
    {
        // 現在の指定コマンド
        Global.CommandType  targetCommandType = m_UITargetCommandList[ m_CommandInputCnt ].GetCommandType();

        // 入力成否を決定
        bool IsPushSuccess = false;
        if( targetCommandType == type )
        {
            IsPushSuccess = true;
        }
        m_IsInputSuccessList.Add( IsPushSuccess );

        // 成否SE
        SoundMgr.I().PlaySe( IsPushSuccess ? "se_push_succsess" : "se_push_failure" );

        // ボタン押し結果表示
        m_UIPushResultList[ m_CommandInputCnt ].StartView( IsPushSuccess, 0.5f );

        // 次のコマンドへ
        m_CommandInputCnt++;
        if( m_CommandInputCnt < m_UITargetCommandList.Count )
        {
            Vector2 pickUpPos = m_UITargetCommandList[ m_CommandInputCnt ].GetPos();
            m_UITargetPickUp.StartView(pickUpPos);
        }
    }

    private void Awake()
    {
        if( m_CharaCntrolMgr == null )
        {
            Debug.Assert( false, "CharaCntrolMgrがありません！");
            return;
        }
        if( m_CommandAttackDataList == null )
        {
            Debug.Assert( false, "CommandAttackDataListがありません！");
            return;
        }
        if( m_CommandPanel == null )
        {
            Debug.Assert( false, "CommandPanelがありません！");
            return;
        }
        if( m_ActionPanel == null )
        {
            Debug.Assert( false, "ActionPanelがありません！");
            return;
        }
        if( m_UITimer == null )
        {
            Debug.Assert( false, "UITimerがありません！");
            return;
        }
        if( m_UIStatus == null )
        {
            Debug.Assert( false, "UIStatusがありません！");
            return;
        }
        if( m_UIAttackText == null )
        {
            Debug.Assert( false, "UIAttackTextがありません！");
            return;
        }
        if( m_UIFinishText == null )
        {
            Debug.Assert( false, "UIFinishTextがありません！");
            return;
        }
        if( m_UIResultText == null )
        {
            Debug.Assert( false, "UIResultTextがありません！");
            return;
        }
        if( m_UIAttackBtnP == null )
        {
            Debug.Assert( false, "UIAttackBtnPがありません！");
            return;
        }
        if( m_UIAttackBtnK == null )
        {
            Debug.Assert( false, "UIAttackBtnKがありません！");
            return;
        }
        if( m_UITargetCommandList == null )
        {
            Debug.Assert( false, "UITargetCommandListがありません！");
            return;
        }
        if( m_UIPushResultList == null )
        {
            Debug.Assert( false, "UIPushResultListがありません！");
            return;
        }

        // ボタン押し関数設定
        m_UIAttackBtnP.onClick.AddListener( () => PushAttackBtn( Global.CommandType.punch ) );
        m_UIAttackBtnK.onClick.AddListener( () => PushAttackBtn( Global.CommandType.kick ) );

        // 遷移設定
        m_State.Add( STATE.none,            null,                           null );
        m_State.Add( STATE.start,           UpdateState_StartInit,          UpdateState_Start );
        m_State.Add( STATE.playerAttack,    UpdateState_PlayerAttackInit,   UpdateState_PlayerAttack );
        m_State.Add( STATE.enemyAttack,     UpdateState_EnemyAttackInit,    UpdateState_EnemyAttack );
        m_State.Add( STATE.result,          UpdateState_ResultInit,         UpdateState_Result );
        m_State.Add( STATE.end,             UpdateState_EndInit,            UpdateState_End );
        m_State.Set( STATE.none );
    }

    private void Start()
    {
        // HP設定
        m_PlayerHp  = m_PlayerMaxHp;
        m_EnemyHp   = m_EnemyMaxHp;

        m_UIStatus.InitHpBar( Global.PlayerId.player, m_PlayerHp );
        m_UIStatus.InitHpBar( Global.PlayerId.enemy, m_EnemyHp );

        // BGM再生
        SoundMgr.I().PlayBgm("bmg_battle");

        m_State.Set( STATE.start );
    }

    private void Update()
    {
        m_TimeCnt.Update();
        m_State.Update();
    }

    //=======================================
    // 遷移
    //=======================================
    // start 開始
    private enum SUB_STATE_START
    {
        start,     // 開始
        end,       // 終了
    };
    private void UpdateState_StartInit()
    {
        m_State.SetSubState( (int)SUB_STATE_START.start );
    }
    private void UpdateState_Start()
    {
        switch( m_State.GetSubState() )
        {
            case (int)SUB_STATE_START.start:
                {
                    // フェードアウト
                    FadeMgr.I().StartFadeOut( 0.5f );

                    m_State.SetSubState( (int)SUB_STATE_START.end );
                }
                break;
            case (int)SUB_STATE_START.end:
                {
                    if( !FadeMgr.I().IsEndFade() ) break;

                    m_State.Set( STATE.playerAttack );
                }
                break;
        }
    }

    // playerAttack プレイヤー攻撃
    private enum SUB_STATE_PLAYER_ATTACK
    {
        start,      // 開始
        attackUIEnd,// コウゲキ開始UI終了
        inputReady, // 入力中準備
        input,      // 入力中
        wait,       // 待機
        charaAttack,// キャラ攻撃
        end,        // 終了
    };
    private void UpdateState_PlayerAttackInit()
    {
        m_State.SetSubState( (int)SUB_STATE_PLAYER_ATTACK.start );
    }
    private void UpdateState_PlayerAttack()
    {
        switch( m_State.GetSubState() )
        {
            case (int)SUB_STATE_PLAYER_ATTACK.start:
                {
                    // 開始UI表示
                    m_UIAttackText.StartView( Global.PlayerId.player, 1f );

                    // 開始SE
                    SoundMgr.I().PlaySe("se_attack_start");
                    
                    m_TimeCnt.StartCnt( 2 );
                    m_State.SetSubState( (int)SUB_STATE_PLAYER_ATTACK.attackUIEnd );
                }
                break;
            case (int)SUB_STATE_PLAYER_ATTACK.attackUIEnd:
                {
                    if( !m_TimeCnt.IsMaxCnt() ) break;

                    // 開始UI停止
                    m_UIAttackText.EndView( 1f );

                    m_TimeCnt.StartCnt( 1f );
                    m_State.SetSubState( (int)SUB_STATE_PLAYER_ATTACK.inputReady );
                }
                break;
            case (int)SUB_STATE_PLAYER_ATTACK.inputReady:
                {
                    if( !m_TimeCnt.IsMaxCnt() ) break;

                    // UI表示
                    m_CommandPanel.SetActive( true );
                    m_ActionPanel.SetActive( true );

                    // 今回のコマンドをランダムで設定
                    System.Random randm = new System.Random();
                    foreach( UITargetCommand uITargetCommand in m_UITargetCommandList.ToArray() )
                    {
                        int randIdx = randm.Next( 0, (int)Global.CommandType.num );
                        Global.CommandType commandType = (Global.CommandType)randIdx;
                        uITargetCommand.SetCommandType( commandType );
                    }

                    // ボタン押し結果UIを念のため非表示
                    foreach( UIPushResult uIPushResult in m_UIPushResultList.ToArray() )
                    {
                        uIPushResult.EndView( 0 );
                    }

                    // 最初のターゲットを設定
                    m_CommandInputCnt = 0;
                    Vector2 pickUpPos = m_UITargetCommandList[ m_CommandInputCnt ].GetPos();
                    m_UITargetPickUp.StartView( pickUpPos );

                    // 入力成否リスト初期化
                    m_IsInputSuccessList = null;
                    m_IsInputSuccessList = new List<bool>();

                    // ボタン押し開始
                    m_UIAttackBtnP.enabled = true;
                    m_UIAttackBtnK.enabled = true;

                    // タイマー開始
                    m_UITimer.StartTimer( m_TimerSecMax );
                    
                    m_State.SetSubState( (int)SUB_STATE_PLAYER_ATTACK.input );
                }
                break;
            case (int)SUB_STATE_PLAYER_ATTACK.input:
                {
                    // 全部入力済みor時間切れで終了
                    if( m_CommandInputCnt >= m_UITargetCommandList.Count ||
                        m_UITimer.IsEndTimer() )
                    {
                        // ボタン入力無効
                        m_UIAttackBtnP.enabled = false;
                        m_UIAttackBtnK.enabled = false;

                        m_TimeCnt.StartCnt( 0.6f );
                        m_State.SetSubState( (int)SUB_STATE_PLAYER_ATTACK.wait );
                    }
                }
                break;
            case (int)SUB_STATE_PLAYER_ATTACK.wait:
                {
                    if( !m_TimeCnt.IsMaxCnt() ) break;

                    // UI非表示
                    m_CommandPanel.SetActive( false );
                    m_ActionPanel.SetActive( false );
                    
                    // 入力ナシならキャラ攻撃終了
                    if( m_IsInputSuccessList.Count == 0 )
                    {
                        m_State.SetSubState( (int)SUB_STATE_PLAYER_ATTACK.end );
                        return; // ここで抜ける
                    }

                    m_CharaAttackCnt = 0;
                    
                    m_TimeCnt.StartCnt( 0.5f );
                    m_State.SetSubState( (int)SUB_STATE_PLAYER_ATTACK.charaAttack );
                }
                break;
            case (int)SUB_STATE_PLAYER_ATTACK.charaAttack:
                {
                    if( !m_TimeCnt.IsMaxCnt() ) break;

                    Global.CommandType  commandType         = m_UITargetCommandList[ m_CharaAttackCnt ].GetCommandType();
                    CommandAttackData   commandAttackData   = GetCommandAttackData( commandType );

                    // キャラの攻撃モーションを再生
                    m_CharaCntrolMgr.PlayAnim( Global.PlayerId.player, commandAttackData.animType );
                    // コウゲキボイス
                    if( commandType == Global.CommandType.punch )
                    {
                        SoundMgr.I().PlaySe("V0005");

                        // パンチSE
                        SoundMgr.I().PlaySe("se_punch");
                    }
                    else
                    {
                        SoundMgr.I().PlaySe("V0006");

                        // キックSE
                        SoundMgr.I().PlaySe("se_kick");
                    }

                    // 入力の成否に応じてコウゲキが成功
                    bool IsSuccess = false;
                    if( m_IsInputSuccessList[ m_CharaAttackCnt ] )
                    {
                        // 敵にダメージ
                        m_EnemyHp -= commandAttackData.damageValue;
                        if( m_EnemyHp < 0 )
                        {
                            m_EnemyHp = 0;
                        }
                        m_UIStatus.StartHpUpdate( Global.PlayerId.enemy, m_EnemyHp );

                        // ダメージモーション
                        // 体力が0なら死亡モーション
                        if( m_EnemyHp <= 0 )
                        {
                            m_CharaCntrolMgr.PlayAnim( Global.PlayerId.enemy, CharaCntrol.ANIM_TYPE.damageBig );
                            // やられボイス
                            SoundMgr.I().PlaySe( "V1018" );
                        }
                        else
                        {
                            m_CharaCntrolMgr.PlayAnim( Global.PlayerId.enemy, CharaCntrol.ANIM_TYPE.damage ); 
                            // ダメージボイス
                            SoundMgr.I().PlaySe( "V1016" );
                        }
                    }
                    else
                    {
                        // 防御されてダメージ無し
                        // 防御モーション
                        m_CharaCntrolMgr.PlayAnim( Global.PlayerId.enemy, CharaCntrol.ANIM_TYPE.defense ); 
                    }

                    m_CharaAttackCnt++;

                    m_TimeCnt.StartCnt( 0.9f );
                    
                    // 敵の体力が0 or 入力コマンド数を超えたら終了
                    if( m_EnemyHp <= 0 ||
                        m_CharaAttackCnt >= m_IsInputSuccessList.Count )
                    {
                        m_State.SetSubState( (int)SUB_STATE_PLAYER_ATTACK.end );
                    }
                }
                break;
            case (int)SUB_STATE_PLAYER_ATTACK.end:
                {
                    if( !m_TimeCnt.IsMaxCnt() ) break;

                    // 敵の体力が0なら結果へ
                    if( m_EnemyHp <= 0 )
                    {
                        m_WinPlayerId = Global.PlayerId.player;

                        m_State.Set( STATE.result );
                    }
                    // 生きてたら敵のコウゲキへ
                    else
                    {
                        m_State.Set( STATE.enemyAttack );
                    }
                }
                break;
        }
    }

    // enemyAttack 敵攻撃
    private enum SUB_STATE_ENEMY_ATTACK
    {
        start,      // 開始
        attackUIEnd,// コウゲキ開始UI終了
        ready,      // 準備
        charaAttack,// キャラ攻撃
        end,        // 終了
    };
    private void UpdateState_EnemyAttackInit()
    {
        m_State.SetSubState( (int)SUB_STATE_ENEMY_ATTACK.start );
    }
    private void UpdateState_EnemyAttack()
    {
        switch( m_State.GetSubState() )
        {
            case (int)SUB_STATE_ENEMY_ATTACK.start:
                {
                    // 開始UI表示
                    m_UIAttackText.StartView( Global.PlayerId.enemy, 1f );

                    // 開始SE
                    SoundMgr.I().PlaySe("se_attack_start");
                    
                    m_TimeCnt.StartCnt( 2 );
                    m_State.SetSubState( (int)SUB_STATE_ENEMY_ATTACK.attackUIEnd );
                }
                break;
            case (int)SUB_STATE_ENEMY_ATTACK.attackUIEnd:
                {
                    if( !m_TimeCnt.IsMaxCnt() ) break;

                    // 開始UI停止
                    m_UIAttackText.EndView( 1f );

                    m_TimeCnt.StartCnt( 1f );
                    m_State.SetSubState( (int)SUB_STATE_ENEMY_ATTACK.ready );
                }
                break;
            case (int)SUB_STATE_ENEMY_ATTACK.ready:
                {
                    if( !m_TimeCnt.IsMaxCnt() ) break;

                    System.Random randm = new System.Random();

                    // 今回のコマンドをランダムで設定
                    foreach( UITargetCommand uITargetCommand in m_UITargetCommandList.ToArray() )
                    {
                        int randIdx = randm.Next( 0, (int)Global.CommandType.num );
                        Global.CommandType commandType = (Global.CommandType)randIdx;
                        uITargetCommand.SetCommandType( commandType );
                    }

                    // 敵の入力成功数を決定
                    m_IsInputSuccessList = null;
                    m_IsInputSuccessList = new List<bool>();
                    foreach( UITargetCommand uITargetCommand in m_UITargetCommandList.ToArray() )
                    {
                        bool IsSuccess = false;
                        // ランダムで成功
                        int randValue = randm.Next( 0, Global.s_PUSH_SUCCSESS_RATE_MAX );
                        if( randValue <= GameParam.I().GetEnemyPushSuccessRate100() )
                        {
                            IsSuccess = true;
                        }
                        m_IsInputSuccessList.Add( IsSuccess );
                    }

                    m_CharaAttackCnt = 0;
                    
                    m_TimeCnt.StartCnt( 0.1f );
                    m_State.SetSubState( (int)SUB_STATE_ENEMY_ATTACK.charaAttack );
                }
                break;
            case (int)SUB_STATE_ENEMY_ATTACK.charaAttack:
                {
                    if( !m_TimeCnt.IsMaxCnt() ) break;

                    Global.CommandType  commandType         = m_UITargetCommandList[ m_CharaAttackCnt ].GetCommandType();
                    CommandAttackData   commandAttackData   = GetCommandAttackData( commandType );

                    // コウゲキボイス
                    if( commandType == Global.CommandType.punch )
                    {
                        SoundMgr.I().PlaySe("V1005");

                        // パンチSE
                        SoundMgr.I().PlaySe("se_punch");
                    }
                    else
                    {
                        SoundMgr.I().PlaySe("V1006");

                        // キックSE
                        SoundMgr.I().PlaySe("se_kick");
                    }

                    // キャラの攻撃モーションを再生
                    m_CharaCntrolMgr.PlayAnim( Global.PlayerId.enemy, commandAttackData.animType );

                    // 入力の成否に応じてコウゲキが成功
                    bool IsSuccess = false;
                    if( m_IsInputSuccessList[ m_CharaAttackCnt ] )
                    {
                        // プレイヤーにダメージ
                        m_PlayerHp -= commandAttackData.damageValue;
                        if( m_PlayerHp < 0 )
                        {
                            m_PlayerHp = 0;
                        }
                        m_UIStatus.StartHpUpdate( Global.PlayerId.player, m_PlayerHp );

                        // ダメージモーション
                        // 体力が0なら死亡モーション
                        if( m_PlayerHp <= 0 )
                        {
                            m_CharaCntrolMgr.PlayAnim( Global.PlayerId.player, CharaCntrol.ANIM_TYPE.damageBig );
                            // やられボイス
                            SoundMgr.I().PlaySe( "V0018" );
                        }
                        else
                        {
                            m_CharaCntrolMgr.PlayAnim( Global.PlayerId.player, CharaCntrol.ANIM_TYPE.damage ); 
                            // ダメージボイス
                            SoundMgr.I().PlaySe( "V0016" );
                        }
                    }
                    else
                    {
                        // 防御されてダメージ無し
                        // 防御モーション
                        m_CharaCntrolMgr.PlayAnim( Global.PlayerId.player, CharaCntrol.ANIM_TYPE.defense ); 
                    }

                    m_CharaAttackCnt++;

                    m_TimeCnt.StartCnt( 0.9f );
                    
                    // 敵の体力が0 or 入力コマンド数を超えたら終了
                    if( m_PlayerHp <= 0 ||
                        m_CharaAttackCnt >= m_IsInputSuccessList.Count )
                    {
                        m_State.SetSubState( (int)SUB_STATE_ENEMY_ATTACK.end );
                    }
                }
                break;
            case (int)SUB_STATE_ENEMY_ATTACK.end:
                {
                    if( !m_TimeCnt.IsMaxCnt() ) break;

                    // 敵の体力が0なら結果へ
                    if( m_PlayerHp <= 0 )
                    {
                        m_WinPlayerId = Global.PlayerId.enemy;

                        m_State.Set( STATE.result );
                    }
                    // 生きてたらプレイヤーのコウゲキへ
                    else
                    {
                        m_State.Set( STATE.playerAttack );
                    }
                }
                break;
        }
    }

    // result 結果
    private enum SUB_STATE_RESULT
    {
        start,      // 開始
        finishUIOut,// 結果UI非表示
        winView,    // 勝利表示
        end,        // 終了
    };
    private void UpdateState_ResultInit()
    {
        m_State.SetSubState( (int)SUB_STATE_RESULT.start );
    }
    private void UpdateState_Result()
    {
        switch( m_State.GetSubState() )
        {
            case (int)SUB_STATE_RESULT.start:
                {
                    // 終了表示
                    m_UIFinishText.StartView( 1f );

                    // 終了SE
                    SoundMgr.I().PlaySe("se_finish");
                    
                    m_TimeCnt.StartCnt( 2 );
                    m_State.SetSubState( (int)SUB_STATE_RESULT.finishUIOut );
                }
                break;
            case (int)SUB_STATE_RESULT.finishUIOut:
                {
                    if( !m_TimeCnt.IsMaxCnt() ) break;

                    // 終了表示停止
                    m_UIFinishText.EndView( 1f );

                    m_TimeCnt.StartCnt( 1 );
                    m_State.SetSubState( (int)SUB_STATE_RESULT.winView );
                }
                break;
            case (int)SUB_STATE_RESULT.winView:
                {
                    if( !m_TimeCnt.IsMaxCnt() ) break;

                    // 勝利表示
                    m_UIResultText.StartView( m_WinPlayerId, 1f );

                    // 各キャラに勝利/敗北ポーズ
                    CharaCntrol.ANIM_TYPE playerAnim = CharaCntrol.ANIM_TYPE.lose;
                    if( m_WinPlayerId == Global.PlayerId.player )
                    {
                        playerAnim = CharaCntrol.ANIM_TYPE.win;
                        // 勝利ボイス
                        SoundMgr.I().PlaySe( "V0025" );

                        // 勝利SE
                        SoundMgr.I().PlaySe("se_win");
                    }
                    else
                    {
                        // 敗北ボイス
                        SoundMgr.I().PlaySe( "V0019" );

                        // 敗北SE
                        SoundMgr.I().PlaySe("se_lose");
                    }
                    m_CharaCntrolMgr.PlayAnim( Global.PlayerId.player, playerAnim );

                    CharaCntrol.ANIM_TYPE enemyAnim = CharaCntrol.ANIM_TYPE.lose;
                    if( m_WinPlayerId == Global.PlayerId.enemy )
                    {
                        enemyAnim = CharaCntrol.ANIM_TYPE.win;
                        // 勝利ボイス
                        SoundMgr.I().PlaySe( "V1025" );
                    }
                    else
                    {
                        // 敗北ボイス
                        SoundMgr.I().PlaySe( "V1019" );
                    }
                    m_CharaCntrolMgr.PlayAnim( Global.PlayerId.enemy, enemyAnim );

                    m_TimeCnt.StartCnt( 4 );
                    m_State.SetSubState( (int)SUB_STATE_RESULT.end );
                }
                break;
            case (int)SUB_STATE_RESULT.end:
                {
                    if( !m_TimeCnt.IsMaxCnt() ) break;
                    
                    m_State.Set( STATE.end );
                }
                break;
        }
    }

    // end 終了
    private enum SUB_STATE_END
    {
        start,     // 移動開始
        end,       // 終了
    };
    private void UpdateState_EndInit()
    {
        m_State.SetSubState( (int)SUB_STATE_END.start );
    }
    private void UpdateState_End()
    {
        switch( m_State.GetSubState() )
        {
            case (int)SUB_STATE_END.start:
                {
                    // フェードイン
                    FadeMgr.I().StartFadeIn( 0.5f );
                    
                    m_State.SetSubState( (int)SUB_STATE_END.end );
                }
                break;
            case (int)SUB_STATE_END.end:
                {
                    if( !FadeMgr.I().IsEndFade() ) break;
                    
                    // シーン移行
                    GameParam.I().SetFlowId( Global.FLOW_ID.title );
                    SceneManager.LoadScene( "OutGame" );
                }
                break;
        }
    }
}
