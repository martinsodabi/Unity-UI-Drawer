using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace MartinsOdabi
{
    /// <summary>
    /// The UIDrawerController class, any game object
    /// with this component should have at least
    /// a child game object with a UIDrawer component.
    /// <para>See the documentation for more deatails</para>
    /// </summary>
    public class UIDrawerController : MonoBehaviour
    {
        [Header("UI DRAWER CONTROLLER SETTINGS")]

        [SerializeField]
        private List<UIDrawer> drawers = new List<UIDrawer>();

        [HideInInspector]
        public UIDrawer currentDrawer;

        [HideInInspector]
        public UIDrawerSide uIDrawerSide;

        [Tooltip(
        "The input area from the left or right"
        + " side of the screen before dragging"
        + " can begin. Should only be a maximum"
        + " of 50% of the screen from both sides."
        )]
        [SerializeField]
        [Range(0.0f, 0.5f)]
        private float inputAreaPercentageX;

        [Tooltip(
        "The input area from the top or bottom"
        + " side of the screen before dragging"
        + " can begin. Should only be a maximum"
        + " of 50% of the screen from both sides."
        )]
        [SerializeField]
        [Range(0.0f, 0.5f)]
        private float inputAreaPercentageY;

        private bool isDragging;
        private bool canDrag;
        private bool aDrawerIsVisible;

        private Vector2 currentInputPosition;
        private Vector2 lastInputPosition;
        private Vector2 deltaPosition;

        private void Awake()
        {
            SetUIDrawerVisibility(false);
        }

        private void Update()
        {
            //No need to check for Input if no UIDrawers exist.
            if (drawers == null || drawers.Count < 1)
            {
                return;
            }

            CheckInitialInputPosition();

            if(Input.GetMouseButtonUp(0))
            {
                canDrag = false;
                isDragging = false;
                ResetInputPositions();
                uIDrawerSide = UIDrawerSide.NEUTRAL;
            }

            //Can't act on a null UIDrawer.
            if (currentDrawer == null)
            {
                return;
            }

            if (canDrag)
            {
                DragDrawer();
            }

            if (Input.GetMouseButtonUp(0))
            {
                if (!isDragging && aDrawerIsVisible)
                {
                    currentDrawer.Hide();
                }

                currentDrawer.CheckVisibility();
            }

            if (Input.GetMouseButton(0) && aDrawerIsVisible)
            {
                DragDrawer();
            }
        }

        //Check the input position to know if a drag can begin
        private void CheckInitialInputPosition()
        {
            //Prevents checking for initial input if...
            //a drawer is visible.
            if (aDrawerIsVisible)
            {
                return;
            }

            //Prevents Multiple Input
            if (isDragging)
            {
                return;
            }

            if (Input.GetMouseButtonDown(0))
            {
                //Check Left Side of the Screen
                if (Input.mousePosition.x < (inputAreaPercentageX * Screen.width))
                {
                    ProccessInitialInputPosition(UIDrawerSide.LEFT);
                }
                //Check Right Side of the Screen
                else if (Input.mousePosition.x > Screen.width - (inputAreaPercentageX * Screen.width))
                {
                    ProccessInitialInputPosition(UIDrawerSide.RIGHT);
                }
                //Check Top Side of the Screen
                else if (Input.mousePosition.y > Screen.height - (inputAreaPercentageY * Screen.height))
                {
                    ProccessInitialInputPosition(UIDrawerSide.TOP);
                }
                //Check Bottom Side of the Screen
                else if (Input.mousePosition.y < (inputAreaPercentageY * Screen.height))
                {
                    ProccessInitialInputPosition(UIDrawerSide.BOTTOM);
                }
            }
        }

        private void ProccessInitialInputPosition(UIDrawerSide _uIDrawerSide)
        {
            canDrag = true;
            uIDrawerSide = _uIDrawerSide;
            GetCurrentDrawer();
        }

        private void GetCurrentDrawer()
        {
            if (drawers == null || drawers.Count < 1)
            {
                return;
            }

            foreach (var item in drawers)
            {
                if (item != null && item.drawerSide == uIDrawerSide)
                {
                    currentDrawer = item;
                    break;
                }
            }
        }

        private void DragDrawer()
        {

            currentInputPosition = Input.mousePosition;

            if (uIDrawerSide == UIDrawerSide.LEFT || uIDrawerSide == UIDrawerSide.RIGHT)
            {
                //Debug.Log("Current Input: " + currentInputPosition + " Last Input " + lastInputPosition);
                deltaPosition.x = currentInputPosition.x - lastInputPosition.x;

            }
            else if (uIDrawerSide == UIDrawerSide.TOP || uIDrawerSide == UIDrawerSide.BOTTOM)
            {
                //Debug.Log("Current Input: " + currentInputPosition + " Last Input " + lastInputPosition);
                deltaPosition.y = currentInputPosition.y - lastInputPosition.y;
            }

            //Prevents calling MoveUiDrawer if not dragging.
            if (lastInputPosition != Vector2.zero)
            {
                if (Vector2.Distance(deltaPosition.Abs(), currentInputPosition - lastInputPosition) > 0.1f)
                {
                    isDragging = true;
                }

                currentDrawer.MoveDrawer(deltaPosition);
            }

            lastInputPosition = currentInputPosition;
        }

        private void ResetInputPositions()
        {
            currentInputPosition = new Vector2();
            lastInputPosition = new Vector2();
            deltaPosition = new Vector2();
        }

        public void SetUIDrawerVisibility(bool _isVisible)
        {
            aDrawerIsVisible = _isVisible;
        }

        private bool CheckIfDrawerExist(int _drawerIndex)
        {
            if (drawers == null || drawers.Count < 1)
            {
                return false;
            }

            if(drawers.Count < (_drawerIndex + 1))
            {
                return false;
            }

            if (drawers[_drawerIndex] == null)
            {
                return false;
            }

            return true;
        }

        ///<summary>Call this function to TOGGLE UIDrawer.
        ///<para>Takes an integer value as the index of the 
        ///UIDrawer in the drawers list assigned in the inspector.</para>
        ///</summary>
        public void ToggleUIDrawerAction(int _drawerIndex)
        {
            if (CheckIfDrawerExist(_drawerIndex) == false)
            {
                return;
            }

            if (drawers[_drawerIndex].isVisible == false)
            {
                ShowUIDrawerAction(_drawerIndex);
            }
            else
            {
                HideUIDrawerAction(_drawerIndex);
            }
        }

        ///<summary>Call this function to SHOW UIDrawer.
        ///<para>Takes an integer value as the index of the 
        ///UIDrawer in the drawers list assigned in the inspector.</para>
        ///</summary>
        public void ShowUIDrawerAction(int _drawerIndex)
        {
            if (CheckIfDrawerExist(_drawerIndex) == false)
            {
                return;
            }

            //Prevents calling show if user is...
            //currenly dragging a side drawer
            if (isDragging)
            {
                return;
            }

            //Hide All UI drawers in the scene before...
            //showing the current UI drawer.
            HideUIDrawers();

            drawers[_drawerIndex].Show();
        }

        ///<summary>Call this function to HIDE UIDrawer.
        ///<para>Takes an integer value as the index of the 
        ///UIDrawer in the drawers list assigned in the inspector.</para>
        ///</summary>
        public void HideUIDrawerAction(int _drawerIndex)
        {
            if (CheckIfDrawerExist(_drawerIndex) == false)
            {
                return;
            }

            //Prevents calling hide if user is...
            //currenly dragging a side drawer
            if (isDragging)
            {
                return;
            }

            drawers[_drawerIndex].Hide();
        }

        ///<summary>
        ///Hides all UIDrawers that are shown.
        ///</summary>
        private void HideUIDrawers()
        {
            if (drawers == null || drawers.Count < 1)
            {
                return;
            }

            foreach (var item in drawers)
            {
                if (item != null && item.isVisible)
                {
                    item.Hide();
                }
            }
        }

        //public void OnPointerClick(PointerEventData eventData)
        //{
        //    if (eventData == null)
        //    {
        //        return;
        //    }

        //    if (eventData.pointerEnter.gameObject != this.gameObject)
        //    {
        //        return;
        //    }

        //    if (currentDrawer != null)
        //    {
        //        currentDrawer.Hide();
        //    }

        //}
    }
}