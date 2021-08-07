using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BankaController : MonoBehaviour {

    Quaternion _startQuat;
    Vector3 _startPos;
    Rigidbody _rb;
        
    private void Awake()
    {
        _rb = GetComponent<Rigidbody>();

        _startQuat = transform.rotation;
        _startPos = transform.position;
    }

    private void OnEnable()
    {       
        _rb.velocity = Vector3.zero;
        _rb.freezeRotation = true;
        Invoke("FReezeNot", 0.5f);

        transform.rotation = _startQuat;
        transform.position = _startPos;
    }


    void FReezeNot()
    {
        _rb.freezeRotation = false;
    }

      
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.GetComponent<BallControl>())
        {
            //_rb.useGravity = true;
            _rb.freezeRotation = false;
            
        }
        
    }

}
