using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

/// <summary>
/// フェード管理クラス
/// </summary>
public class FadeMgr : SingletonMonoBehaviour<FadeMgr>
{
    [SerializeField] Canvas         m_Canvas        = null;
    [SerializeField] CanvasGroup    m_CanvasGroup   = null;

    private bool    m_IsEndFade     = true;
    private float   m_StartAlpha    = 0;

    private const float s_ALPHA_MIN = 0;
    private const float s_ALPHA_MAX = 1f;

    // 遷移
    private enum STATE
    {
        none,
        fadeIn,
        fadeOut,
    }
    private State<STATE>    m_State     = null;
    private TimeCnt         m_TimeCnt   = new TimeCnt();

    /// <summary>
    /// フェード開始
    /// </summary>
    /// <param name="time">フェード時間</param>
    public void StartFadeIn( float time )
    {
        // Stateが生成されていないと困るためここでも生成チェック
        CreateState();

        // 0秒は瞬時切り替え
        if( time == 0 )
        {
            m_IsEndFade = true;
            SetImageColorA( s_ALPHA_MAX );
            return;
        }
        if( GetImageColorA() >= s_ALPHA_MAX )
        {
            m_IsEndFade = true;
            return;
        }

        // 最前面にていおく
        m_Canvas.sortingOrder = 10000;

        m_IsEndFade     = false;
        m_StartAlpha    = GetImageColorA();

        m_TimeCnt.StartCnt( time );
        m_State.Set( STATE.fadeIn );
    }
    /// <summary>
    /// フェード終了
    /// </summary>
    /// <param name="time">フェード時間</param>
    public void StartFadeOut( float time )
    {
        // Stateが生成されていないと困るためここでも生成チェック
        CreateState();

        // 0秒は瞬時切り替え
        if( time == 0 )
        {
            m_IsEndFade = true;
            SetImageColorA( s_ALPHA_MIN );
            return;
        }
        if( GetImageColorA() <= s_ALPHA_MIN )
        {
            m_IsEndFade = true;
            return;
        }

        // 最前面にていおく
        m_Canvas.sortingOrder = 10000;

        m_IsEndFade     = false;
        m_StartAlpha    = GetImageColorA();

        m_TimeCnt.StartCnt( time );
        m_State.Set( STATE.fadeOut );
    }
    /// <summary>
    /// フェード処理が終了したか
    /// </summary>
    /// <returns>フェード処理終了フラグ</returns>
    public bool IsEndFade()
    {
        return m_IsEndFade;
    }

    /// <summary>
    /// 透過値設定
    /// </summary>
    /// <param name="alpha">透過値</param>
    private void SetImageColorA( float alpha )
    {
        if( alpha < s_ALPHA_MIN )
        {
            alpha = s_ALPHA_MIN;
        }
        if( alpha >= s_ALPHA_MAX )
        {
            alpha = s_ALPHA_MAX;
        }
        m_CanvasGroup.alpha = alpha;
    }
    /// <summary>
    /// 現在の透過値取得
    /// </summary>
    /// <returns>透過値</returns>
    private float GetImageColorA()
    {
        return m_CanvasGroup.alpha;
    }
    /// <summary>
    /// 遷移管理生成
    /// </summary>
    private void CreateState()
    {
        if( m_State != null ) return;

        m_State = new State<STATE>();
        m_State.Add( STATE.none,     null, null );
        m_State.Add( STATE.fadeIn,   null, UpdateState_FadeIn );
        m_State.Add( STATE.fadeOut,  null, UpdateState_FadeOut );
        m_State.Set( STATE.none );
    }

    private void Awake()
    {
        if( m_Canvas == null )
        {
            Debug.Assert( false, "Canvasがありません！");
            return;
        }
        if( m_CanvasGroup == null )
        {
            Debug.Assert( false, "CanvasGroupがありません！");
            return;
        }

        // 遷移生成
        CreateState();

        // 初期は透明
        SetImageColorA( 0 );
    }
    void Update()
    {
        m_TimeCnt.Update();
        if( m_State != null )
        {
            m_State.Update();
        }
    }

    //=======================================
    // 遷移
    //=======================================
    // フェードアップデート
    void UpdateState_FadeIn()
    {
        // 透過設定
        float addMaxAlpha = s_ALPHA_MAX - GetImageColorA();
        float alpha = GetImageColorA() + ( addMaxAlpha * m_TimeCnt.GetDiffRate() );
        if( alpha > s_ALPHA_MAX )
        {
            alpha = s_ALPHA_MAX;
        }
        SetImageColorA( alpha );

        if( m_TimeCnt.IsMaxCnt() )
        {
            m_IsEndFade = true;
            m_State.Set( STATE.none );
        }
    }
    void UpdateState_FadeOut()
    {
        // 透過設定
        float alpha = GetImageColorA() - ( GetImageColorA() * m_TimeCnt.GetDiffRate() );
        if( alpha < s_ALPHA_MIN )
        {
            alpha = s_ALPHA_MIN;
        }
        SetImageColorA( alpha );

        // 雑に最背面
        m_Canvas.sortingOrder = 0;

        if( m_TimeCnt.IsMaxCnt() )
        {
            m_IsEndFade = true;
            m_State.Set( STATE.none );
        }
    }
}
