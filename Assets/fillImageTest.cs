using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class fillImageTest : MonoBehaviour
{
    private Image m_fillImage;
    private float m_timeElasped;

    public HUD hud;

    private void Awake()
    {
        m_fillImage = GetComponent<Image>();
    }

    // Start is called before the first frame update
    private void Start()
    {
    }

    // Update is called once per frame
    private void Update()
    {
        m_timeElasped += Time.deltaTime;
        hud.SetStressValue(Mathf.Clamp(m_timeElasped, 0, 100));
    }
}