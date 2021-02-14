using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// バトル用ボタン押し時間表示UIクラス
/// </summary>
public class UITimer : MonoBehaviour
{
	[SerializeField] private Text m_Text = null;

    private bool    m_IsAction      = false;
    private int     m_MaxTimeSec    = 0;
    private TimeCnt m_TimeCnt       = new TimeCnt();

    /// <summary>
    /// タイマー開始
    /// </summary>
    /// <param name="timeSec">タイマー時間</param>
    public void StartTimer( int timeSec )
    {
        m_IsAction = true;
        m_MaxTimeSec = timeSec;
        m_TimeCnt.StartCnt( timeSec );
    }
    /// <summary>
    /// 終了したか
    /// </summary>
    /// <returns>タイマー終了フラグ</returns>
    public bool IsEndTimer()
    {
        return m_TimeCnt.IsMaxCnt();
    }

    // Update is called once per frame
    void Update()
    {
        if( !m_IsAction ) return;

        m_TimeCnt.Update();
        if( m_TimeCnt.IsMaxCnt() )
        {
            m_IsAction = false;
        }

        // 表示数値更新
        int viewTimeSec = m_MaxTimeSec - (int)m_TimeCnt.GetDiffTime();
        if( viewTimeSec < 0 )
        {
            viewTimeSec = 0;
        }
        m_Text.text = viewTimeSec.ToString();
    }
}
