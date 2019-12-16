using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR.ARFoundation;
using UnityEngine.EventSystems;

[RequireComponent(typeof(ARSessionOrigin))]
public class PlacementController : MonoBehaviour
{
    [SerializeField]
    private GameObject placedPrefab;
    private Vector2 touchPosition = default;
    private GameObject placedObject;

    private Pose hitPose;
    private static List<ARRaycastHit> hits = new List<ARRaycastHit>();
    public GameObject placementIndicator;
    private bool placementPoseIsValid = false;
    
    [SerializeField]
    private Button chairButton, tvButton, lampButton;
    public GameObject PlacedPrefab
    {
        get; private set;
    }

    private ARSessionOrigin aRSessionOrigin;
    // Start is called before the first frame update
    void Awake()
    {
        aRSessionOrigin = GetComponent<ARSessionOrigin>();
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
            placedPrefab = loadedGameObject;
            Debug.Log($"This object {name} successfully loaded.");
        }else
        {
            Debug.Log($"Unable to find this object {name}.");
        }
    }
    bool TryGetTouchPosition(out Vector2 touchPosition)
    {
        if (Input.touchCount > 0)
        {
            touchPosition = Input.GetTouch(0).position;
            return true;
        }
        touchPosition = default;
        return false;
    }

    // Update is called once per frame
    void Update()
    {
        UpdatePlacementPose();
        UpdatePlacementIndicator();
        if (EventSystem.current.IsPointerOverGameObject() || EventSystem.current.currentSelectedGameObject != null)
        {
            if (chairButton != null && tvButton != null && lampButton != null)
            {
                chairButton.onClick.AddListener(() => ChangePrefabSelection("RFAIPP_Office_Armchair_4"));
                tvButton.onClick.AddListener(() => ChangePrefabSelection("RFAIPP_TV"));
                lampButton.onClick.AddListener(() => ChangePrefabSelection("RFAIPP_Lamp"));
            }
            return;
        }
        if (placementPoseIsValid && Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began)
        {
           PlaceObject();
        }

    }

    private void PlaceObject()
    {
        PlacedPrefab = Instantiate(placedPrefab, hitPose.position, hitPose.rotation);
        aRSessionOrigin.MakeContentAppearAt(PlacedPrefab.transform, PlacedPrefab.transform.position, PlacedPrefab.transform.rotation);
    }

    private void UpdatePlacementIndicator()
    {
        if (placementPoseIsValid){
            hitPose = hits[0].pose;
            placementIndicator.SetActive(true);
            placementIndicator.transform.SetPositionAndRotation(hitPose.position, hitPose.rotation);
        }
        else
        {
            placementIndicator.SetActive(false);
        }
    }

    private void UpdatePlacementPose()
    {
        var screenCenter = Camera.current.ViewportToScreenPoint(new Vector3(0.5f, 0.5f));
        aRSessionOrigin.Raycast(screenCenter, hits, UnityEngine.Experimental.XR.TrackableType.Planes);

        placementPoseIsValid = hits.Count > 0;
        if (placementPoseIsValid)
        {
            hitPose = hits[0].pose;
        }
    }
}
