using System.Collections;
using System.Collections.Generic;
using Assets.Scripts.Tracking;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Utils;

namespace Tracking
{
    public class MyInput : Singleton<MyInput>
    {
        public float cursorSpeed = 10.0f;
        public float cursorSize = 0.6f;

        public float cursorRotateSpeed = 0.5f;
        private readonly float _buttonHoldTime = 2f;
        private float _buttonTimer;
        private float _cameraIdleTimer;
        private bool _camTracking = true;
        private Image _circularTimer;
        private Canvas _cursorCanvas;
        private readonly float _cursorFadeTime = 1f;
        private EventSystem _eventSystem;

        private readonly float _idleTime = 1f;
        private bool _isMouse;
        private GameObject _lastButton;

        private float _mouseIdleTimer;

        private Vector3 _oldMousePosition;
        private GraphicRaycaster _raycaster;

        //private 

        private Tracker _tracker;
        private LayerMask _ui;

        private float _x;
        private float _y;

        public float X => CursorImage.transform.position.x;

        public float Y => CursorImage.transform.position.y;

        public bool IsTracked { get; private set; }

        public bool IsTrackedCursor => CursorImage.gameObject.activeSelf;

        public Image CursorImage { get; private set; }

        public override void Awake()
        {
            base.Awake();
            var cc = Instantiate(Resources.Load("CursorCanvas")) as GameObject;
            cc.transform.SetParent(transform);
            _cursorCanvas = cc.GetComponent<Canvas>();

            var cursor = Instantiate(Resources.Load("CursorCircle")) as GameObject;
            cursor.transform.localScale = new Vector3(cursorSize, cursorSize, cursorSize);
            cursor.transform.SetParent(cc.transform);
            CursorImage = cursor.GetComponent<Image>();
            var cTimer = Instantiate(Resources.Load("CircularTimer")) as GameObject;
            cTimer.transform.position = cursor.transform.position;
            cTimer.transform.SetParent(cursor.transform);
            _circularTimer = cTimer.GetComponent<Image>();
            _ui = LayerMask.GetMask("UI");
        }

        public void Update()
        {
            IsTracked = false;
            if (TryTrackMouse())
            {
                _x = Input.mousePosition.x;
                _y = Input.mousePosition.y;
                IsTracked = true;
                _isMouse = true;
            }
            else if (_camTracking)
            {
                if (_tracker.isTracked)
                {
                    var trackingPos = EmguCamera.Instance.CameraToScreen(_tracker.trackingPosition, _cursorCanvas);
                    _x = trackingPos.x;
                    _y = trackingPos.y;
                    IsTracked = true;
                }
                else
                {
                    _cameraIdleTimer += Time.deltaTime;
                    //Debug.Log("not tracked:" + _cameraIdleTimer);
                    if (_cameraIdleTimer > _idleTime) IsTracked = false;
                }

                _isMouse = false;
            }

            if (!IsTracked)
            {
                StartCoroutine(FadeCursor());
                return;
            }

            if (_isMouse)
            {
                CursorImage.transform.position = new Vector3(_x, _y);
                CursorImage.gameObject.SetActive(true);
            }
            else
            {
                if (_cameraIdleTimer > _idleTime)
                    CursorImage.transform.position = new Vector3(_x, _y);
                else
                    MoveCursor();
                CursorImage.gameObject.SetActive(true);
                _cameraIdleTimer = 0;
                CheckButtons();
            }
        }

        private void OnEnable()
        {
            if (_cursorCanvas != null) _cursorCanvas.sortingOrder = 5;
            _cursorCanvas.gameObject.SetActive(true);
        }

        private void OnDisable()
        {
            _cursorCanvas.gameObject.SetActive(false);
        }

        public void CamTrackingOn()
        {
            _camTracking = true;
        }

        public void CamTrackingOff()
        {
            _camTracking = false;
        }

        public void Init()
        {
            _tracker = Tracker.Instance;
            _raycaster = FindFirstObjectByType<GraphicRaycaster>();
            _eventSystem = FindFirstObjectByType<EventSystem>();
        }

        public bool TryTrackMouse()
        {
            var mousePos = Input.mousePosition;
            if (mousePos == _oldMousePosition)
            {
                _mouseIdleTimer += Time.deltaTime;
                if (_mouseIdleTimer > _idleTime) return false;
            }
            else
            {
                _oldMousePosition = mousePos;
                _mouseIdleTimer = 0;
            }

            return true;
        }

        private IEnumerator FadeCursor()
        {
            var alp = 1f;
            float alpTime = 0;
            while (alp > 0)
            {
                alpTime += Time.deltaTime;
                alp = 1 - alpTime / _cursorFadeTime;
                CursorImage.color = new Color(CursorImage.color.r, CursorImage.color.g, CursorImage.color.b, alp);
                if (IsTracked) break;
                yield return null;
            }

            if (!IsTracked)
            {
                CursorImage.gameObject.SetActive(false);
                CursorImage.color = new Color(CursorImage.color.r, CursorImage.color.g, CursorImage.color.b, 1);
            }
            else
            {
                CursorImage.gameObject.SetActive(true);
                CursorImage.color = new Color(CursorImage.color.r, CursorImage.color.g, CursorImage.color.b, 1);
            }
        }

        private void CheckButtons()
        {
            //Set up the new Pointer Event
            var m_PointerEventData = new PointerEventData(_eventSystem);
            //Set the Pointer Event Position to that of the mouse position
            m_PointerEventData.position =
                new Vector2(CursorImage.transform.position.x, CursorImage.transform.position.y);

            //Create a list of Raycast Results
            var results = new List<RaycastResult>();

            //Raycast using the Graphics Raycaster and mouse click position
            _raycaster.Raycast(m_PointerEventData, results);

            //For every result returned, output the name of the GameObject on the Canvas hit by the Ray
            foreach (var result in results)
            {
                var button = result.gameObject.GetComponent<Button>();
                if (button != null)
                {
                    var scaleCoof = (CursorImage.transform as RectTransform).rect.width +
                                    (button.transform as RectTransform).rect.height;
                    UnityEngine.Debug.Log("scalecoof - " + scaleCoof);
                    //_circularTimer.transform.localScale = new Vector3
                    //{
                    //    x = scaleCoof / 600,
                    //    y = scaleCoof / 600,
                    //    z = scaleCoof / 600
                    //};                    
                    //_circularTimer.transform.position = button.transform.position;
                    _circularTimer.gameObject.SetActive(true);
                    if (_lastButton == button.gameObject)
                    {
                        _buttonTimer += Time.deltaTime;
                        if (_buttonTimer >= _buttonHoldTime)
                        {
                            _circularTimer.gameObject.SetActive(false);
                            button.onClick.Invoke();
                        }
                    }
                    else
                    {
                        _buttonTimer = 0;
                    }

                    _circularTimer.fillAmount = _buttonTimer / _buttonHoldTime;
                    _lastButton = button.gameObject;
                    return;
                }
            }

            _circularTimer.gameObject.SetActive(false);
            _lastButton = null;
            _buttonTimer = 0;
        }

        private void MoveCursor()
        {
            if (IsTracked)
            {
                var newPos = new Vector2(_x, _y);
                var distance = Vector2.Distance(CursorImage.transform.position, newPos);
                CursorImage.transform.position = Vector3.MoveTowards(CursorImage.transform.position, newPos,
                    cursorSpeed * Time.deltaTime * distance);
            }
        }
    }
}