using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BankaController : MonoBehaviour {

    Quaternion _startQuat;
    Vector3 _startPos;
    Rigidbody _rb;
        
    //List<Rigidbody> _allRb;

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
        //_rb.useGravity = false;
        Invoke("FReezeNot", 0.5f);

        transform.rotation = _startQuat;
        transform.position = _startPos;

    }

    private void Update()
    {
       /* print("speed = " + _rb.velocity.magnitude);
        if (!_rb.useGravity && _rb.velocity.magnitude > 0)
        {
            _rb.useGravity = true;
        }*/
    }

    void FReezeNot()
    {
        _rb.freezeRotation = false;
    }

    private void Start()
    {
        /*_allBanks = GameObject.FindObjectsOfType<BankaController>();fix

        _allRb = new Rigidbody[_allBanks.Length];
        for (int i = 0; i < _allBanks.Length; i++)
        {
            _allRb[i] = _allBanks[i].GetComponent<Rigidbody>();
        }*/
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
