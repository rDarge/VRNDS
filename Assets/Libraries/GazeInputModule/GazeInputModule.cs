// Gaze Input Module by Peter Koch <peterept@gmail.com>
using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections.Generic;

// To use:
// 1. Drag onto your EventSystem game object.
// 2. Disable any other Input Modules (eg: StandaloneInputModule & TouchInputModule) as they will fight over selections.
// 3. Make sure your Canvas is in world space and has a GraphicRaycaster (should by default).
// 4. If you have multiple cameras then make sure to drag your VR (center eye) camera into the canvas.
public class GazeInputModule : PointerInputModule {
    public enum Mode { Click = 0, Gaze };
    public Mode mode;

    [Header("Click Settings")]
    public string ClickInputName = "Submit";
    public string AltClickInputName = "Button A";
    [Header("Gaze Settings")]
    public float GazeTimeInSeconds = 2f;
    [Header("Cursor (Optional)")]
    public bool useCursor = true;
    public bool deselectOnLookAway = false;
    public bool clickAgainWithoutLookingAway = false;
    public float waitThisLongBeforeClickingAgain = 1;
    public RectTransform cursor;
    public List<GameObject> ignoreForCursor;

    public RaycastResult CurrentRaycast;

    private PointerEventData pointerEventData;
    private GameObject currentHandler;
    private float lastClickTime;
    private GazeInputButtonScript lastGazeScript;
    private GazeInputButtonScript currentGazeScript;
    private int tick;
    private int tickSkip = 5;

    private int cursorRotation = 0;

    public override void Process() {
        if (pointerEventData == null) {
            pointerEventData = new PointerEventData(eventSystem);
        }

        //Update the cursor always
        UpdateCursor(pointerEventData);

        //Only toggle events every X ticks
        if (++tick % tickSkip == 0) {
            HandleLook();
            HandleSelection();
        }

    }

    void HandleLook() {
        // fake a pointer always being at the center of the screen
        pointerEventData.position = new Vector2(Screen.width / 2, Screen.height / 2);
        pointerEventData.delta = Vector2.zero;

        //Raycast to find any gaze-able objects
        List<RaycastResult> raycastResults = new List<RaycastResult>();
        eventSystem.RaycastAll(pointerEventData, raycastResults);
        CurrentRaycast = pointerEventData.pointerCurrentRaycast = FindFirstRaycast(raycastResults);
        ProcessMove(pointerEventData);
    }

    void HandleSelection() {
        if (pointerEventData.pointerEnter != null) {
            // if the ui receiver has changed, reset the gaze delay timer
            GameObject handler = ExecuteEvents.GetEventHandler<IPointerClickHandler>(pointerEventData.pointerEnter);
            if (currentHandler != handler) {
                currentHandler = handler;
                try {

                    currentGazeScript = (GazeInputButtonScript)handler.GetComponent(typeof(GazeInputButtonScript));
                } catch (System.NullReferenceException e) {
                    currentGazeScript = null;
                    cursorRotation = 0;

                    if(deselectOnLookAway && lastGazeScript != null) {
                        lastGazeScript.deselect();
                    }
                }

                if (currentGazeScript) {
                    lastClickTime = Time.realtimeSinceStartup + currentGazeScript.waitTimeInSeconds;
                }
            }

            // if we have a handler and it's time to click, do it now
            if (currentHandler != null &&
                ((mode == Mode.Gaze || currentGazeScript) && Time.realtimeSinceStartup > lastClickTime) ||
                (mode == Mode.Click && (Input.GetButtonDown(ClickInputName) || Input.GetButtonDown(AltClickInputName) || Input.GetMouseButton(0)))) {

                ExecuteEvents.ExecuteHierarchy(currentHandler, pointerEventData, ExecuteEvents.pointerClickHandler);

                if(clickAgainWithoutLookingAway) {
                    lastClickTime = Time.realtimeSinceStartup + waitThisLongBeforeClickingAgain;
                } else {
                    lastClickTime = float.MaxValue;
                }
                

                //Handle "Selection" operations
                if (lastGazeScript != currentGazeScript && currentGazeScript != null) {

                    currentGazeScript.select();

                    if (lastGazeScript != null) {
                        lastGazeScript.deselect();
                    }

                    lastGazeScript = currentGazeScript;
                }
                cursorRotation = 0;

            } else if (currentHandler != null && lastGazeScript != currentGazeScript) {
                cursorRotation += 10;
            }
        } else {
            currentHandler = null;
        }
    }

    // update the cursor location and whether it is enabled
    // this code is based on Unity's DragMe.cs code provided in the UI drag and drop example
    private void UpdateCursor(PointerEventData lookData) {
        if (cursor != null && lookData != null) {
            if (useCursor) {
                if (lookData.pointerEnter != null && !ignoreForCursor.Contains(lookData.pointerEnter)) {
                    RectTransform draggingPlane = lookData.pointerEnter.GetComponent<RectTransform>();
                    Vector3 globalLookPos;
                    if (RectTransformUtility.ScreenPointToWorldPointInRectangle(draggingPlane, lookData.position, lookData.enterEventCamera, out globalLookPos)) {
                        cursor.gameObject.SetActive(true);
                        cursor.position = globalLookPos;
                        cursor.rotation = draggingPlane.rotation;
                        cursor.Rotate(new Vector3(0, 0, 1), cursorRotation);
                    } else {
                        cursor.gameObject.SetActive(false);
                    }
                } else {
                    cursor.gameObject.SetActive(false);
                }
            } else {
                cursor.gameObject.SetActive(false);
            }
        }
    }



}