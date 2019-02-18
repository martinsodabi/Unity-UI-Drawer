using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MartinsOdabi
{
    /// <summary>
    /// The UIDrawer class, any game object
    /// with this component must be a child
    /// of a game object with a
    /// UIDrawerController component.
    /// <para>See the documentation for more deatails</para>
    /// </summary>
    [RequireComponent(typeof(RectTransform))]
    public class UIDrawer : MonoBehaviour
    {
        private RectTransform rectTransform;

        [Header("UI DRAWER SETTINGS")]

        [Tooltip(
        "The side of the drawer according to"
        + " its respective hide position,"
        + " where it will be dragged from"
        )]
        [SerializeField]
        public UIDrawerSide drawerSide;

        [Tooltip(
        "The position of the Drawer when"
        + " it is NOT VISIBLE on the screen."
        + " Note that this is either on the"
        + " x or the y axis depending on"
        + " the appropriate drawer side."
        )]
        public Vector2 hidePosition;

        [Tooltip(
        "The position of the Drawer when"
        + " it BECOMES VISIBLE on the screen."
        + " Note that this is either on the"
        + " x or the y axis depending on"
        + " the appropriate drawer side."
        )]
        public Vector2 showPosition;

        [Tooltip(
        "How much to drag before the Drawer starts"
        + " Animating OUT of the screen. This is a"
        + " percentage of the total size of the UIDrawer"
        )]
        [SerializeField]
        [Range(0.0f, 1.0f)]
        private float hideRangePercent;

        [Tooltip(
        "How much to drag before the Drawer starts"
        + " Animating IN to the screen. This is a"
        + " percentage of the total size of the UIDrawer"
        )]
        [SerializeField]
        [Range(0.0f, 1.0f)]
        private float showRangePercent;

        //Is the drawer visible?
        [HideInInspector]
        public bool isVisible;

        //Is the drawer moving?
        //[HideInInspector]
        private bool isAnimating;

        [Tooltip("The speed of the drawer's Movement Animation")]
        public float speed;

        //The UI Drawer Controller that
        //this drawer is a child of.
        private UIDrawerController drawerController;

        private void Awake()
        {
            rectTransform = GetComponent<RectTransform>();
            drawerController = GetComponentInParent<UIDrawerController>();

            rectTransform.anchoredPosition = hidePosition;
        }

        public void MoveDrawer(Vector2 _delta)
        {
            //Prevents Drag while the UI drawer is still...
            //animating to destination position.
            if (isAnimating)
            {
                return;
            }

            //rect.anchoredPosition = Vector2.SmoothDamp(rect.anchoredPosition, _delta, ref currentVelocityRef, Time.deltaTime);
            rectTransform.anchoredPosition += _delta;
            ClampDrawer();
        }

        //Clamps the drawer to not move more than...    
        //show position and below hide position.
        private void ClampDrawer()
        {
            switch (drawerSide)
            {
                case UIDrawerSide.LEFT:
                    ClampLeftDrawer();
                    break;
                case UIDrawerSide.RIGHT:
                    ClampRightDrawer();
                    break;
                case UIDrawerSide.TOP:
                    ClampTopDrawer();
                    break;
                case UIDrawerSide.BOTTOM:
                    ClampBottomDrawer();
                    break;
            }
        }

        private void ClampLeftDrawer()
        {
            if (rectTransform.anchoredPosition.x >= showPosition.x)
            {
                rectTransform.anchoredPosition = showPosition;
            }
            else if (rectTransform.anchoredPosition.x <= hidePosition.x)
            {
                rectTransform.anchoredPosition = hidePosition;
            }
        }

        private void ClampRightDrawer()
        {
            if (rectTransform.anchoredPosition.x <= showPosition.x)
            {
                rectTransform.anchoredPosition = showPosition;
            }
            else if (rectTransform.anchoredPosition.x >= hidePosition.x)
            {
                rectTransform.anchoredPosition = hidePosition;
            }
        }

        private void ClampTopDrawer()
        {
            if (rectTransform.anchoredPosition.y <= showPosition.y)
            {
                rectTransform.anchoredPosition = showPosition;
            }
            else if (rectTransform.anchoredPosition.y >= hidePosition.y)
            {
                rectTransform.anchoredPosition = hidePosition;
            }
        }

        private void ClampBottomDrawer()
        {
            if (rectTransform.anchoredPosition.y >= showPosition.y)
            {
                rectTransform.anchoredPosition = showPosition;
            }
            else if (rectTransform.anchoredPosition.y <= hidePosition.y)
            {
                rectTransform.anchoredPosition = hidePosition;
            }
        }

        /// <summary>
        /// Check if the drawer should be shown
        /// or hidden depending on how much of
        /// the drawer is visible on the screen.
        /// </summary>
        public void CheckVisibility()
        {
            Vector2 _showRange = rectTransform.sizeDelta.Abs() * Mathf.Abs(showRangePercent);
            Vector2 _hideRange = rectTransform.sizeDelta.Abs() * Mathf.Abs(hideRangePercent);
            Vector2 _currentPosition = rectTransform.anchoredPosition.Abs();

            if (!isVisible)
            {
                if (drawerSide == UIDrawerSide.LEFT || drawerSide == UIDrawerSide.RIGHT)
                {
                    if (Mathf.Abs(hidePosition.Abs().x - _currentPosition.x) >= _showRange.x)
                    {
                        Show();
                    }
                    else
                    {
                        Hide();
                    }
                }
                else
                {
                    if (Mathf.Abs(hidePosition.Abs().y - _currentPosition.y) >= _showRange.y)
                    {
                        Show();
                    }
                    else
                    {
                        Hide();
                    }
                }

                return;
            }

            if (drawerSide == UIDrawerSide.LEFT || drawerSide == UIDrawerSide.RIGHT)
            {
                if (Mathf.Abs(showPosition.Abs().x - _currentPosition.x) >= _hideRange.x)
                {
                    Hide();
                }
                else
                {
                    Show();
                }
            }
            else
            {
                if (Mathf.Abs(showPosition.Abs().y - _currentPosition.y) >= _hideRange.y)
                {
                    Hide();
                }
                else
                {
                    Show();
                }
            }
        }

        /// <summary>
        /// Show the UI Drawer
        /// </summary>
        public void Show()
        {
            //TODO Setup Animation to ShowPosition
            StopCoroutine("AnimateUIDrawer");
            StartCoroutine(AnimateUIDrawer(showPosition, true));
        }

        /// <summary>
        /// Hide the UI Drawer
        /// </summary>
        public void Hide()
        {
            //TODO Setup Animation to HidePosition
            StopCoroutine("AnimateUIDrawer");
            StartCoroutine(AnimateUIDrawer(hidePosition, false));
        }

        private void BeforeAnimationInit()
        {
            isAnimating = true;
        }

        private void AfterAnimationInit(bool _toShowOrHide)
        {
            drawerController.SetUIDrawerVisibility(_toShowOrHide);
            drawerController.currentDrawer = _toShowOrHide ? this : null;
            drawerController.uIDrawerSide = _toShowOrHide ? drawerSide : UIDrawerSide.NEUTRAL;

            isAnimating = false;
            isVisible = _toShowOrHide;
        }

        public IEnumerator AnimateUIDrawer(Vector2 _targetPosition, bool _toShowOrHide)
        {
            BeforeAnimationInit();

            Vector2 start = rectTransform.anchoredPosition;
            //total time this has been running
            float runningTime = 0;
            //the longest it would take to get to the destination at this speed
            float totalRunningTime = Vector2.Distance(start, _targetPosition) / (speed * 100);
            //for the length of time it takes to get to the end position
            while (runningTime < totalRunningTime)
            {
                //keep track of the time each frame
                runningTime += Time.deltaTime;
                //lerp between start and end, based on the current amount of time that has passed
                // and the total amount of time it would take to get there at this speed.
                rectTransform.anchoredPosition = Vector2.Lerp(start, _targetPosition, runningTime / totalRunningTime);
                yield return 0;
            }

            rectTransform.anchoredPosition = _targetPosition;

            AfterAnimationInit(_toShowOrHide);
        }
    }

    ///<summary>
    ///The side of the UIDrawer NEUTRAL is default.
    ///</summary>
    public enum UIDrawerSide
    {
        NEUTRAL,
        LEFT,
        RIGHT,
        TOP,
        BOTTOM
    }
}