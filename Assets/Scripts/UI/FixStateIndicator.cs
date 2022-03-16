using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;
using Mirror;

public class FixStateIndicator : MonoBehaviour
{
    public Fixable fixableObject;
    [SerializeField] private float m_offset;
    private Camera cam;

    private void Start()
    {
        fixableObject.OnObjectFix += Hide;
        fixableObject.OnObjectBreak += Show;
        Hide();
    }

    private void Show() => gameObject.SetActive(true);

    private void Hide() => gameObject.SetActive(false);

    private void LateUpdate()
    {
        transform.position = cam.WorldToScreenPoint(fixableObject.transform.position + Vector3.up * m_offset);
    }

    private void OnEnable()
    {
        if (cam == null)
        {
            cam = Player.Instance.Cam;
        }
    }

    private void OnDisable()
    {
    }
}