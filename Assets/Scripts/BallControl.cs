using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

//Внедрить полёт через параболу
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
    [SerializeField]int bottomBorderSpeed = 4000;
    [SerializeField] int upBorderSpeed = 7000;
    float borderBottomTapScreen = 0.2f;
    int valueSmoothSpeed = 150;


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

        _speedCheck += eventData.delta.y * 5;
        _speedCheck = Mathf.Clamp(_speedCheck, bottomBorderSpeed, upBorderSpeed);

        //ЕСЛИ МЫ ДВИГАЕМ ПАЛЕЦ ТОЛЬКО ПО ГОРИЗОНТАЛИ
        if (Input.mousePosition.y < Screen.height * borderBottomTapScreen)
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
        if (!_isStartTimeStopwatch && Input.mousePosition.y > Screen.height * borderBottomTapScreen)
        {
            _isStartTimeStopwatch = true;
            _timeStopwatchCor = StartCoroutine(TimeStopwatch());
        }
            
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        _isOnlyHorizontal = false;
        int multiplierSpeed = 2500;
        int multiplierSpeedToBanka = 6000;

        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, 1000) && Input.mousePosition.y > 
            Screen.height * borderBottomTapScreen && !_isFly)
        {

            StopCoroutine(_startMoveCor);
            _rb.velocity = Vector3.zero;
            if (_timeStopwatchCor != null)
                StopCoroutine(_timeStopwatchCor);

            _endDrag = hit.point;

            _direction = _endDrag - transform.position;
            _rb.constraints = RigidbodyConstraints.None;
            _isFly = true;

            _rb.AddForce(_direction.normalized * _speedCheck + 
                new Vector3(0, multiplierSpeed, 0), ForceMode.Force);

            _gameControl.DecrementJobsBalls();

            if (hit.collider.tag == "Banka")
            {
                _rb.AddForce(_direction.normalized * _speedCheck + 
                    new Vector3(0, multiplierSpeedToBanka, 0), ForceMode.Force);

                _isMove = true;
            }
               
        }
    }


    void FixedUpdate ()
    {
        if (!_isMove)
            return;

        _rb.velocity = _direction.normalized * _speedCheck / valueSmoothSpeed;

    }

    private void OnCollisionEnter(Collision collision)
    {
        _isMove = false;
    }



    IEnumerator STartMoveBalls()
    {

        int increaseVelocity = 4;

        while (true)
        {
            if (_isOnlyHorizontal)
            {
                _direction = _targetBfrDragLowerY - transform.position;
                _rb.velocity = _direction.normalized * increaseVelocity;
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
        int increaseSpeed = 60;

        while (_timeNotDrag < 2)
        {
            _timeNotDrag += Time.deltaTime;
            _speedCheck -= _timeNotDrag * increaseSpeed;
            _speedCheck = Mathf.Clamp(_speedCheck, bottomBorderSpeed, upBorderSpeed);
            yield return null;
        }

        //if Overly long delay drag
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        

        if (Physics.Raycast(ray, out hit, 1000) && Input.mousePosition.y > Screen.height * borderBottomTapScreen)
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
                _isMove = true;


        }
    }
 
    
}
