using System.Collections;
using System.Collections.Generic;
using Unity.XR.PXR;
using UnityEngine;

public class AvatarControl : MonoBehaviour
{
    public SkinnedMeshRenderer Skin;
    public SkinnedMeshRenderer LeftEyeExample;
    public SkinnedMeshRenderer RightEyeExample;

    private int leftEyeBlinkIndex;
    private float leftEyeOpenness;
    private int leftLookDownIndex;
    private int leftLookUpIndex;
    private int leftLookInIndex;
    private int leftLookOutIndex;

    private int rightEyeBlinkIndex;
    private float rightEyeOpenness;
    private int rightLookDownIndex;
    private int rightLookUpIndex;
    private int rightLookInIndex;
    private int rightLookOutIndex;
    // Start is called before the first frame update
    void Start()
    {
        leftEyeBlinkIndex = Skin.sharedMesh.GetBlendShapeIndex("eyeBlinkLeft");
        /*
        leftLookDownIndex = LeftEyeExample.sharedMesh.GetBlendShapeIndex("eyeLookDownLeft");
        leftLookUpIndex = LeftEyeExample.sharedMesh.GetBlendShapeIndex("eyeLookUpLeft");
        leftLookInIndex = LeftEyeExample.sharedMesh.GetBlendShapeIndex("eyeLookInLeft");
        leftLookOutIndex = LeftEyeExample.sharedMesh.GetBlendShapeIndex("eyeLookOutLeft");
        */

        rightEyeBlinkIndex = Skin.sharedMesh.GetBlendShapeIndex("eyeBlinkRight");
        /*
        rightLookDownIndex = RightEyeExample.sharedMesh.GetBlendShapeIndex("eyeLookDownRight");
        rightLookUpIndex = RightEyeExample.sharedMesh.GetBlendShapeIndex("eyeLookUpRight");
        rightLookInIndex = RightEyeExample.sharedMesh.GetBlendShapeIndex("eyeLookInRight");
        rightLookOutIndex = RightEyeExample.sharedMesh.GetBlendShapeIndex("eyeLookOutRight");
        */
    }

    // Update is called once per frame
    void Update()
    {
        PXR_EyeTracking.GetLeftEyeGazeOpenness(out leftEyeOpenness);
        Skin.SetBlendShapeWeight(leftEyeBlinkIndex, (1f-leftEyeOpenness) * 100);
        PXR_EyeTracking.GetRightEyeGazeOpenness(out rightEyeOpenness);
        Skin.SetBlendShapeWeight(rightEyeBlinkIndex, (1f-rightEyeOpenness) * 100);
    }
}
