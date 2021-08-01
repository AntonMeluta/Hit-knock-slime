using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class BallControl : MonoBehaviour, IDragHandler, IEndDragHandler, IBeginDragHandler {


    float _timeNotDrag = 0;

    Rigidbody _rb;
    Quaternion _startQuat;
    Vector3 _startPos;
    MeshRenderer _meshRenderer;

    GameControl _gameControl;


    Vector3 _startDrag;
    Vector3 _endDrag;
    Vector3 _direction;
    [SerializeField]Transform _targetBeforeDrag;
    Vector3 _targetBfrDragLowerY;
    Coroutine _startMoveCor;
    Coroutine _timeStopwatchCor;

    bool _isDecrementSpeed;
    float _speedCheck;
    float _speed = 3000;

    bool _isMove;
    bool _isStartTimeStopwatch;
    bool _isFly;
    bool _isOnlyHorizontal;

    private void Awake()
    {
        _gameControl = GameObject.FindObjectOfType<GameControl>();

        _meshRenderer = GetComponent<MeshRenderer>();
        _rb = GetComponent<Rigidbody>();
        _startQuat = transform.rotation;
        _startPos = transform.position;

        //_targetBeforeDrag = GameObject.Find("TargetToStartBall").transform.position;
    }

    private void OnEnable()
    {
        _isMove = false;
        _isStartTimeStopwatch = false;
        _isFly = false;
        _isOnlyHorizontal = false;

        

        _rb.velocity = Vector3.zero;
        transform.rotation = _startQuat;
        transform.position = _startPos;
        _rb.constraints = RigidbodyConstraints.FreezePositionY | RigidbodyConstraints.FreezePositionZ 
            | RigidbodyConstraints.FreezeRotationY | RigidbodyConstraints.FreezeRotationX;
        
        

        
        _meshRenderer.material = _gameControl._materialForBalls;
        _startMoveCor = StartCoroutine(STartMoveBalls());

    }



   

    public void OnBeginDrag(PointerEventData eventData)
    {        
        
    }

    public void OnDrag(PointerEventData eventData)
    {
        //PROD
        _speedCheck += eventData.delta.y * 5;
        _speedCheck = Mathf.Clamp(_speedCheck, 4000, 7000);
        //print("_speedCheck = " + _speedCheck);


        //ЕСЛИ МЫ ДВИГАЕМ ПАЛЕЦ ТОЛЬКО ПО ГОРИЗОНТАЛИ
        if (Input.mousePosition.y < Screen.height * 0.2f)
        {
            _isOnlyHorizontal = true;

            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, 1000))
                _targetBfrDragLowerY = new Vector3(hit.point.x, 0, 0);

            
        }
        //Ушли свайпом вверх
        else        
            _isOnlyHorizontal = false;
        


        //Если свайп ушёл наверх в первый раз
        if (!_isStartTimeStopwatch && Input.mousePosition.y > Screen.height * 0.2f)
        {
            //print("ВЫСОТА ВЫШЕ 15");
            _isStartTimeStopwatch = true;
            _timeStopwatchCor = StartCoroutine(TimeStopwatch());
        }
            
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        _isOnlyHorizontal = false;

        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, 1000) && Input.mousePosition.y > Screen.height * 0.2f && !_isFly)
        {

            StopCoroutine(_startMoveCor);
            _rb.velocity = Vector3.zero;
            if (_timeStopwatchCor != null)
                StopCoroutine(_timeStopwatchCor);

            _endDrag = hit.point;

            //_direction = _endDrag - _startDrag;            
            _direction = _endDrag - transform.position;
            _rb.constraints = RigidbodyConstraints.None;
            _isFly = true;
            _rb.AddForce(_direction.normalized * _speedCheck + new Vector3(0, 2500, 0), ForceMode.Force);
            _gameControl.DecrementJobsBalls();
            if (hit.collider.tag == "Banka")
            {
                _rb.AddForce(_direction.normalized * _speedCheck + new Vector3(0, 6000, 0), ForceMode.Force);
                //Invoke("EnableMove", 0.2f);
                EnableMove();
            }
               
        }
    }

    void EnableMove()
    {
        _isMove = true;
    }

    void FixedUpdate ()
    {
        if (!_isMove)
            return;

        //ANTI PROD velocit
        _rb.velocity = _direction.normalized * _speedCheck / 150;

    }

    private void OnCollisionEnter(Collision collision)
    {
        _isMove = false;
    }



    IEnumerator STartMoveBalls()
    {        
        //float _del = 0;

        while (true)
        {
            if (_isOnlyHorizontal)
            {
                _direction = _targetBfrDragLowerY - transform.position;
                _rb.velocity = _direction.normalized * 4;
            }
            else
            {
                _direction = _targetBeforeDrag.position - transform.position;
                _rb.velocity = _direction.normalized;
                
            }
            
            //print("_del = " + _del++);
            yield return new WaitForFixedUpdate();
        }
    }


    //Секундомер drag
    IEnumerator TimeStopwatch()
    {

        _timeNotDrag = 0;
        while (_timeNotDrag < 2)
        {
            _timeNotDrag += Time.deltaTime;
            //ANTI PROD
            _speedCheck -= _timeNotDrag * 60;
            _speedCheck = Mathf.Clamp(_speedCheck, 4000, 7000);
            //print("_speedCheck = " + _speedCheck);
            yield return null;
        }

        //Overly long delay drag
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, 1000) && Input.mousePosition.y > Screen.height * 0.2f)
        {
                      
            StopCoroutine(_startMoveCor);
            _rb.velocity = Vector3.zero;

            _endDrag = hit.point;

            //_direction = _endDrag - _startDrag;
            _direction = _endDrag - transform.position;
            _rb.constraints = RigidbodyConstraints.None;
            _isFly = true;
            _rb.AddForce(_direction.normalized * _speedCheck, ForceMode.Force);
            _gameControl.DecrementJobsBalls();
            if (hit.collider.tag == "Banka")
                //Invoke("EnableMove", 0.2f);
                EnableMove();


        }
    }
    /*
      - реализовать возможность двигать вбок ++++++
      -адекватно летят только если rotation в нулях ++++ fixed
       - сделать минимальное движение в центр ++++
        - ограничить границы +++++
         - написать логику игры (рестарт, продвижение по уровням, триггеры смерти, подчёт упавших банок и шаров) ++++
         - Начать пилить UI. +- 
          - GREAT доделать ++++++
          - Запилить уровни.
           - Заменить текстуры мяча, либо убрать косяченные.
          - Прописать банкам рестарт позиции. +++++
          - GDPR. ++++
           - Ads. +++++
     * */
    
}
