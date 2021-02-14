using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 時間経過管理クラス
/// </summary>
public class TimeCnt
{
    private float m_DiffTimeSec     = 0;
    private float m_MaxTimeSec      = 0;

    // 最大経過進捗
    public const float s_TIME_CNT_RATE_MAX = 1f;

    /// <summary>
    /// カウント開始
    /// </summary>
    /// <param name="maxTimeSec">カウント時間</param>
    public void StartCnt( float maxTimeSec )
    {
        m_MaxTimeSec   = maxTimeSec;
        m_DiffTimeSec  = 0;
    }
    /// <summary>
    /// 残り時間を返す
    /// </summary>
    /// <returns>残り時間</returns>
    public float GetRemainingTime()
    {
        float remainingTime = m_MaxTimeSec - GetDiffTime();
        if (remainingTime < 0)
        {
            remainingTime = 0;
        }
        return remainingTime;
    }
    /// <summary>
    /// 現在の経過時間を返す
    /// </summary>
    /// <returns>経過時間</returns>
    public float GetDiffTime()
    {
        return m_DiffTimeSec;
    }
    /// <summary>
    /// 経過進捗を返す
    /// </summary>
    /// <returns>経過進捗</returns>
    public float GetDiffRate()
    {
        var rate = GetDiffTime() / m_MaxTimeSec;
        if( rate >= s_TIME_CNT_RATE_MAX )
        {
            rate = s_TIME_CNT_RATE_MAX;
        }
        return rate;
    }
    /// <summary>
    /// カウントが最大までいったか
    /// </summary>
    /// <returns>最大カウント経過フラグ</returns>
    public bool IsMaxCnt()
    {
        return ( GetDiffTime() >= m_MaxTimeSec ) ? true : false;
    }

    /// <summary>
	/// 指定秒数後の経過進捗を返す
    /// </summary>
    /// <param name="second">指定秒数</param>
    /// <returns>指定秒数後の経過進捗</returns>
	public float GetDiffRate_Second( float second )
	{
		var rate = ( m_DiffTimeSec + second ) / m_MaxTimeSec;
		if( rate >= s_TIME_CNT_RATE_MAX )
		{
			rate = s_TIME_CNT_RATE_MAX;
		}
		return rate;
	}

    /// <summary>
    /// 更新
    /// </summary>
    public void Update()
    {
        m_DiffTimeSec += Time.deltaTime;
    }
}
