using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// キャラクタ操作クラス
/// </summary>
public class CharaCntrol : MonoBehaviour
{
    [SerializeField] Global.PlayerId    m_PlayerId = Global.PlayerId.player;
    [SerializeField] Animator           m_Animator = null;

    // アニメーション種類
    public enum ANIM_TYPE
    {
        idle,
        punch,
        kick,
        damage,
        damageBig,
        defense,
        win,
        lose,

        num,
    };
    // アニメーションSTATE名
    public static readonly string[] s_STR_ANIM_STATE_NAME =
    {
        "Idle",
        "Attack_P",
        "Attack_K",
        "Damage",
        "Damage_Big",
        "Defense",
        "Win",
        "Lose",
    };

    private void Awake()
    {
       Debug.Assert(s_STR_ANIM_STATE_NAME.Length >= (int)ANIM_TYPE.num, "アニメーションSTATE名定義エラー");
    }

    /// <summary>
    /// プレイヤーIDを返す
    /// </summary>
    /// <returns>プレイヤーID</returns>
    public Global.PlayerId GetPlayerId()
    {
        return m_PlayerId;
    }

    /// <summary>
    /// アニメーション再生
    /// </summary>
    /// <param name="type">アニメーション種類</param>
    public void PlayAnim( ANIM_TYPE type )
    {
        if( m_Animator == null )
        {
            Debug.Assert( false, "Animatorがありません！");
            return;
        }
        m_Animator.Play( s_STR_ANIM_STATE_NAME[(int)type], 0, 0 );
    }
}
