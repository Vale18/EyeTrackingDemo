using UnityEngine;
using Unity.XR.PXR;
using UnityEngine.XR;
using TMPro;

public class EyeTrackingManager : MonoBehaviour
{
    public GameObject EyeCoordinates;
    public GameObject Models;
    public Transform Greenpoint;
    public GameObject SpotLight;
    public TMP_Text GazeOffsetText;

    private Vector3 combineEyeGazeVector;
    private Vector3 combineEyeGazeOriginOffset;
    private Vector3 combineEyeGazeOrigin;
    private Matrix4x4 matrix;

    private Vector3 combineEyeGazeVectorInWorldSpace;
    private Vector3 combineEyeGazeOriginInWorldSpace;

    private Vector2 primary2DAxis;

    private RaycastHit hitinfo;
    
    private Transform selectObj;

    private bool wasPressed;
    void Start()
    {
        combineEyeGazeOriginOffset = Vector3.zero;
        combineEyeGazeVector = Vector3.zero;
        combineEyeGazeOrigin = Vector3.zero;
    }

    void Update()
    {

        if (InputDevices.GetDeviceAtXRNode(XRNode.RightHand).TryGetFeatureValue(CommonUsages.primary2DAxis, out primary2DAxis))
        {

            combineEyeGazeOriginOffset.x += primary2DAxis.x*0.001f;
            combineEyeGazeOriginOffset.y += primary2DAxis.y*0.001f;

        }
        GazeOffsetText.text = combineEyeGazeOriginOffset.ToString("F3");
        PXR_EyeTracking.GetHeadPosMatrix(out matrix);

        PXR_EyeTracking.GetCombineEyeGazeVector(out combineEyeGazeVector);
        PXR_EyeTracking.GetCombineEyeGazePoint(out combineEyeGazeOrigin);


        combineEyeGazeOrigin += combineEyeGazeOriginOffset;
        combineEyeGazeOriginInWorldSpace = matrix.MultiplyPoint(combineEyeGazeOrigin);
        combineEyeGazeVectorInWorldSpace = matrix.MultiplyVector(combineEyeGazeVector);

        SpotLight.transform.position = combineEyeGazeOriginInWorldSpace;
        SpotLight.transform.rotation = Quaternion.LookRotation(combineEyeGazeVectorInWorldSpace, Vector3.up);

        GazeTargetControl(combineEyeGazeOriginInWorldSpace, combineEyeGazeVectorInWorldSpace);
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
