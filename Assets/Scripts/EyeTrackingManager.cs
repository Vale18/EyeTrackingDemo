using UnityEngine;
using Unity.XR.PXR;
using UnityEngine.XR;
using static TMPro.SpriteAssetUtilities.TexturePacker_JsonArray;

[RequireComponent( typeof( LineRenderer))]
public class EyeTrackingManager : MonoBehaviour
{
    public GameObject EyeCoordinates;
    public GameObject Models;
    public Transform Greenpoint;

    //---------------Line visual------------------
    private Vector3 combineEyeGazeVector;
    private Vector3 combineEyeGazePoint;
    private LineRenderer lineRenderer;
    private Matrix4x4 matrix;

    private RaycastHit hitinfo;
    
    private Transform selectObj;

    private bool wasPressed;
    void Start()
    {
        //Get Line renderer
        if (!lineRenderer)
        {
            lineRenderer = transform.GetComponent<LineRenderer>();
        }
        InitializeLine();

        combineEyeGazeVector = Vector3.zero;
        combineEyeGazePoint = Vector3.zero;
    }

    /// <summary>
    /// Initialize the line
    /// </summary>
    void InitializeLine()
    {
        lineRenderer.startWidth = 0.002f;
        lineRenderer.endWidth = 0.002f;
    }

    void Update()
    {

        PXR_EyeTracking.GetHeadPosMatrix(out matrix);

        PXR_EyeTracking.GetCombineEyeGazeVector(out combineEyeGazeVector);
        PXR_EyeTracking.GetCombineEyeGazePoint(out combineEyeGazePoint);


        var RealOriginOffset = matrix.MultiplyPoint(combineEyeGazePoint);
        var DirectionOffset = matrix.MultiplyVector(combineEyeGazeVector);


        lineRenderer.SetPosition(0, RealOriginOffset); 
        lineRenderer.SetPosition(1, DirectionOffset * 20); 
        GazeTargetControl(RealOriginOffset, DirectionOffset);
        bool triggerIsDone;
        if (InputDevices.GetDeviceAtXRNode(XRNode.RightHand).TryGetFeatureValue(CommonUsages.triggerButton, out triggerIsDone) && triggerIsDone)
        {
            if(!wasPressed)
                ToggleCoordinates();
            wasPressed = true;
        }
        else
        {
            wasPressed = false;
        }
    }

    void GazeTargetControl(Vector3 origin,Vector3 vector)
    {
        Ray ray = new Ray(origin,vector);
        if (Physics.Raycast(ray, out hitinfo))
        {
            if (hitinfo.collider.transform.name.Equals("zbx"))
            {
                Greenpoint.gameObject.SetActive(true);
                Greenpoint.position= hitinfo.point;
            }

            if (selectObj != null && selectObj != hitinfo.transform)
            {
                if(selectObj.GetComponent<ETObject>()!=null)
                    selectObj.GetComponent<ETObject>().UnFocused();
                selectObj = null;
            }
            else if (selectObj == null)
            {
                selectObj = hitinfo.transform;
                if (selectObj.GetComponent<ETObject>() != null)
                    selectObj.GetComponent<ETObject>().IsFocused();
            }

        }
        else
        {
            if (selectObj != null)
            {
               if (selectObj.GetComponent<ETObject>() != null)
                    selectObj.GetComponent<ETObject>().UnFocused();
                selectObj = null;
            }
            Greenpoint.gameObject.SetActive(false);
        }    
    }

    public void ToggleCoordinates()
    {
        EyeCoordinates.SetActive(!EyeCoordinates.activeSelf);
        Models.SetActive(!Models.activeSelf);
    }
}
