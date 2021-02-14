using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// キャラ操作管理クラス
/// </summary>
public class CharaCntrolMgr : MonoBehaviour
{
    [SerializeField] private List<CharaCntrol> m_CharaCntrolList = new List<CharaCntrol>();

    /// <summary>
    /// 指定キャラにアニメーション再生
    /// </summary>
    /// <param name="player">指定プレイヤー</param>
    /// <param name="animType">アニメーション種類</param>
    public void PlayAnim( Global.PlayerId player, CharaCntrol.ANIM_TYPE animType )
    {
        foreach( CharaCntrol cnt in m_CharaCntrolList.ToArray() )
        {
            if( cnt.GetPlayerId() != player ) continue;

            cnt.PlayAnim( animType );
            return;
        }

        // ここにきてたらエラー
        Debug.Assert( false, "指定キャラはいません！");
    }
}
