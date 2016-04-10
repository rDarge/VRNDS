// Gaze Input Module by Peter Koch <peterept@gmail.com>
using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections.Generic;

// To use:
// 1. Drag onto your EventSystem game object.
// 2. Disable any other Input Modules (eg: StandaloneInputModule & TouchInputModule) as they will fight over selections.
// 3. Make sure your Canvas is in world space and has a GraphicRaycaster (should by default).
// 4. If you have multiple cameras then make sure to drag your VR (center eye) camera into the canvas.
public class GazeInputModule : PointerInputModule
{
    public enum Mode { Click = 0, Gaze };
    public Mode mode;

    [Header("Click Settings")]
    public string ClickInputName = "Submit";
    public string AltClickInputName = "Button A";
    [Header("Gaze Settings")]
    public float GazeTimeInSeconds = 2f;

    public RaycastResult CurrentRaycast;

    private PointerEventData pointerEventData;
    private GameObject currentLookAtHandler;
    private float currentLookAtHandlerClickTime;
    private GazeInputButtonScript currentLookAtHandlerGazeScript;

    public override void Process()
    {
        HandleLook();
        HandleSelection();
    }

    void HandleLook()
    {
        if (pointerEventData == null)
        {
            pointerEventData = new PointerEventData(eventSystem);
        }
        // fake a pointer always being at the center of the screen
        pointerEventData.position = new Vector2(Screen.width / 2, Screen.height / 2);
        pointerEventData.delta = Vector2.zero;
        List<RaycastResult> raycastResults = new List<RaycastResult>();
        eventSystem.RaycastAll(pointerEventData, raycastResults);
        CurrentRaycast = pointerEventData.pointerCurrentRaycast = FindFirstRaycast(raycastResults);
        ProcessMove(pointerEventData);
    }

    void HandleSelection()
    {
        if (pointerEventData.pointerEnter != null)
        {
            // if the ui receiver has changed, reset the gaze delay timer
            GameObject handler = ExecuteEvents.GetEventHandler<IPointerClickHandler>(pointerEventData.pointerEnter);
            if (currentLookAtHandler != handler)
            {
                currentLookAtHandler = handler;
                try {
                    currentLookAtHandlerGazeScript = (GazeInputButtonScript)handler.GetComponent(typeof(GazeInputButtonScript));
                } catch (System.NullReferenceException e) {
                    currentLookAtHandlerGazeScript = null;
                }
                
                if(currentLookAtHandlerGazeScript) {
                    currentLookAtHandlerClickTime = Time.realtimeSinceStartup + currentLookAtHandlerGazeScript.waitTimeInSeconds;
                }
            }

            // if we have a handler and it's time to click, do it now
            if (currentLookAtHandler != null &&
                ((mode == Mode.Gaze || currentLookAtHandlerGazeScript) && Time.realtimeSinceStartup > currentLookAtHandlerClickTime) ||
                (mode == Mode.Click && (Input.GetButtonDown(ClickInputName) || Input.GetButtonDown(AltClickInputName) || Input.GetMouseButton(0))))
            {
                ExecuteEvents.ExecuteHierarchy(currentLookAtHandler, pointerEventData, ExecuteEvents.pointerClickHandler);
                currentLookAtHandlerClickTime = float.MaxValue;
            }
        }
        else
        {
            currentLookAtHandler = null;
        }
    }


}