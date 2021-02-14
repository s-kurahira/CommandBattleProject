using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// バトル用HPバーUIクラス
/// </summary>
public class UIHpBar : MonoBehaviour
{
	[SerializeField] private Slider  m_Slider     = null;

    private float m_HpMax         = 0;
    private float m_ViewHp        = 0;
    
    private float   m_TargetViewHp  = 0;
    private float   m_StartViewHp   = 0;
    private bool    m_IsUpdateHp    = false;
    private TimeCnt m_TimeCnt       = new TimeCnt();

    // 更新にかかる秒数
    private const float s_UPDATE_MAX_TIME = 0.5f;
    // HPスライダー最大
    private const float s_SLIDER_VALUE_MAX = 1f;
    // HPスライダー最小
    private const float s_SLIDER_VALUE_MIN = 0;

    /// <summary>
    /// 初期化
    /// </summary>
    /// <param name="hpMax">最大HP</param>
    public void Init( int hpMax )
    {
        m_HpMax     = (float)hpMax;
        m_ViewHp    = m_HpMax;

        m_IsUpdateHp    = false;
        m_TargetViewHp  = m_HpMax;
        m_StartViewHp   = m_HpMax;

        // HPスライダー更新
        m_Slider.value = s_SLIDER_VALUE_MAX;
    }
    /// <summary>
    /// HP変動開始
    /// </summary>
    /// <param name="hp">目標HP</param>
    public void StartHpUpdate( int hp )
    {
        m_StartViewHp   = m_ViewHp;
        m_TargetViewHp  = hp;

        m_IsUpdateHp    = true;
        m_TimeCnt.StartCnt( s_UPDATE_MAX_TIME );
    }
    
    // Update is called once per frame
    void Update()
    {
        if( m_IsUpdateHp )
        {
            m_TimeCnt.Update();

            // HP変動処理
            float addValueMax   = m_TargetViewHp - m_StartViewHp;
            float addValu       = addValueMax * m_TimeCnt.GetDiffRate();
            m_ViewHp = m_StartViewHp + addValu;
            if( m_TargetViewHp > m_StartViewHp )
            {
                if( m_ViewHp >= m_TargetViewHp )
                {
                    m_ViewHp = m_TargetViewHp;
                }
                
            }
            else
            {
                if( m_ViewHp <= m_TargetViewHp )
                {
                    m_ViewHp = m_TargetViewHp;
                }
            }

            // HPスライダー更新
            m_Slider.value = m_ViewHp / m_HpMax;

            if( m_TimeCnt.IsMaxCnt() )
            {
                m_IsUpdateHp = false;
            }
        }
    }
}
