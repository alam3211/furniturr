using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR.ARFoundation;
using UnityEngine.Experimental.XR;
using UnityEngine.EventSystems;
using System;

public class PlacementScript : MonoBehaviour
{
    // Start is called before the first frame update
    public GameObject objectToPlace;
    public GameObject placementIndicator;

    public GameObject spawnedObject {get; private set;}
    private ARSessionOrigin arOrigin;
    private Pose placementPose;
    private bool placementPoseIsValid = false;

    public Button chairButton, tvButton, lampButton;
    private GameObject ObjectToPlace{
        get;set;
    }

    void Start()
    {
        arOrigin = FindObjectOfType<ARSessionOrigin>();
    }

    void Awake(){

        if (chairButton != null && tvButton != null && lampButton != null)
        {
            chairButton.onClick.AddListener(() => ChangePrefabSelection("RFAIPP_Office_Armchair_4"));
            tvButton.onClick.AddListener(() => ChangePrefabSelection("RFAIPP_TV"));
            lampButton.onClick.AddListener(() => ChangePrefabSelection("RFAIPP_Lamp"));
        }
    }

    private void ChangePrefabSelection(string name)
    {
        GameObject loadedGameObject = (GameObject)Resources.Load($"Furniture/Prefabs/{name}");
        if (loadedGameObject != null)
        {
            ObjectToPlace = loadedGameObject;
            Debug.Log($"This object {name} successfully loaded.");
        }else
        {
            Debug.Log($"Unable to find this object {name}.");
        }
    }
    // Update is called once per frame
    void Update()
    {
        if (EventSystem.current.IsPointerOverGameObject() || EventSystem.current.currentSelectedGameObject != null)
        {
            if (chairButton != null && tvButton != null && lampButton != null)
            {
                chairButton.onClick.AddListener(() => ChangePrefabSelection("RFAIPP_Office_Armchair_4"));
                tvButton.onClick.AddListener(() => ChangePrefabSelection("RFAIPP_TV"));
                lampButton.onClick.AddListener(() => ChangePrefabSelection("RFAIPP_Lamp"));
            }
        }

        UpdatePlacementPose();
        UpdatePlacementIndicator();

        if (placementPoseIsValid && Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began)
        {
            PlaceObject();
        }
    }

    private void PlaceObject()
    {
        spawnedObject = Instantiate(objectToPlace, placementPose.position, placementPose.rotation);
        arOrigin.MakeContentAppearAt(spawnedObject.transform, spawnedObject.transform.position, spawnedObject.transform.rotation);
    }

    private void UpdatePlacementIndicator()
    {
        if (placementPoseIsValid){
            placementIndicator.SetActive(true);
            placementIndicator.transform.SetPositionAndRotation(placementPose.position, placementPose.rotation);
        }
        else
        {
            placementIndicator.SetActive(false);
        }
    }

    private void UpdatePlacementPose()
    {
        var screenCenter = Camera.current.ViewportToScreenPoint(new Vector3(0.5f, 0.5f));
        var hits = new List<ARRaycastHit>();
        arOrigin.Raycast(screenCenter, hits, TrackableType.Planes);

        placementPoseIsValid = hits.Count > 0;
        if (placementPoseIsValid)
        {
            placementPose = hits[0].pose;
        }
    }
}
