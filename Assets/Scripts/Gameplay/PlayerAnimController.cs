using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimController : MonoBehaviour
{
    [SerializeField] private Player _player;

    [SerializeField] private GameObject _rig;
    [SerializeField] private GameObject _head;
    [SerializeField] private GameObject _eyebrows;
    [SerializeField] private GameObject _teeth;

    [SerializeField] private Animator _animator;

    [SerializeField] private PlayerCustomization.COLOUR _colour;

    void Start()
    {
        // Can set inactive if its not the right choice
        if(_colour != _player.colour)
        {
            gameObject.SetActive(false);
            return;
        }

        // If local player remove hair and shit
        if (_player == Player.Instance)
        {
            var toHide = new GameObject[] { _head, _eyebrows, _teeth, _rig };

            _head.SetActive(false);
            _eyebrows.SetActive(false);
            _teeth.SetActive(false);
            foreach (var obj in toHide)
            {
                // Only cast shadows from them
                foreach (var renderer in obj.GetComponentsInChildren<Renderer>())
                {
                    renderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.ShadowsOnly;
                }
                foreach (var renderer in obj.GetComponents<Renderer>())
                {
                    renderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.ShadowsOnly;
                }
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        transform.localRotation = Quaternion.Euler(0, _player.yRotation, 0);

        var walking = _player.movement != Vector3.zero;

        _animator.SetBool("IsWalking", walking);

        if (walking && _player.sprint)
        {
            _animator.SetBool("IsRunning", true);
        }

        /*
        if (_player.jump)
        {
            _animator.SetTrigger("Jump");
        }
        */
    }
}
