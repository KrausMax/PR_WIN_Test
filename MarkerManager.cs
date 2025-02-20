using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR;
using TMPro;
using Varjo.XR;

public class MarkerManager : MonoBehaviour
{
    private List<VarjoMarker> markers;
    private List<long> markerIds;
    private List<long> absentIds;
    private Dictionary<long, MarkerVisualizer> markerVisualizers;
    private List<long> whiteListIds

    public VarjoMarker marker;

    public Transform xrRig;
    public GameObject markerPrefab;

    public bool markersEnabled = true;
    private bool _markersEnabled;

    public long markerTimeout = 3000;
    private long _markerTimeout;

    private Transform markerTransform;

    public DisplayInfo displayInfo;

    void Start()
    {
        markers = new List<VarjoMarker>();
        markerIds = new List<long>();
        absentIds = new List<long>();
        markerVisualizers = new Dictionary<long, MarkerVisualizer>();
        marker = new VarjoMarker();
        InvokeRepeating("CheckForRightTriggerInput", 1.0f, 0.4f);
    }

    void Update()
    {
        if (markersEnabled != _markersEnabled)
        {
            markersEnabled = VarjoMarkers.EnableVarjoMarkers(markersEnabled);
            _markersEnabled = markersEnabled;
        }

        if (VarjoMarkers.IsVarjoMarkersEnabled())
        {
            markers.Clear();
            markerIds.Clear();
            int foundMarkers = VarjoMarkers.GetVarjoMarkers(out markers);
            if (markers.Count > 0)
            {
                foreach (var marker in markers)
                {
                    markerIds.Add(marker.id);
                    if (markerVisualizers.ContainsKey(marker.id))
                    {
                        UpdateMarkerVisualizer(marker);
                    }
                    else
                    {
                        CreateMarkerVisualizer(marker);
                        VarjoMarkers.SetVarjoMarkerTimeout(marker.id, markerTimeout);
                    }
                }

                if (markerTimeout != _markerTimeout)
                {
                    SetMarkerTimeOuts();
                    _markerTimeout = markerTimeout;
                }
            }

            VarjoMarkers.GetRemovedVarjoMarkerIds(out absentIds);

            foreach (var id in absentIds)
            {
                if (markerVisualizers.ContainsKey(id))
                {
                    Destroy(markerVisualizers[id].gameObject);
                    markerVisualizers.Remove(id);
                }
                markerIds.Remove(id);
            }
            absentIds.Clear();
        }

        if (markerIds.Count == 0 && markerVisualizers.Count > 0)
        {
            var ids = markerVisualizers.Keys.ToArray();
            foreach (var id in ids)
            {
                Destroy(markerVisualizers[id].gameObject);
                markerVisualizers.Remove(id);
            }
        }
    }

    void CreateMarkerVisualizer(VarjoMarker marker)
    {
        switch (marker.id)
        {
            case 100:
            case 101:
                if (whiteListIds.Contains(marker.id))
                {
                    SensorScannedEvent(marker.id);
                }
                break;

            case 200:
            case 201:
                ParcelScannedEvent(marker.id);
                break;

            case 350:
            case 351:
                if (whiteListIds.Contains(marker.id))
                {
                    ShelfScannedEvent(marker.id);
                }
                break;

            default:
                break;
        }
    }

    void UpdateMarkerVisualizer(VarjoMarker marker)
    {
        markerVisualizers[marker.id].SetMarkerData(marker);
    }

    void SetMarkerTimeOuts()
    {
        for (var i = 0; i < markerIds.Count; i++)
        {
            VarjoMarkers.SetVarjoMarkerTimeout(markerIds[i], markerTimeout);
        }
    }

    void CheckForRightTriggerInput()
    {
        // Get the right-hand controller device
        var rightHandDevice = InputDevices.GetDeviceAtXRNode(XRNode.RightHand);

        // Check if the trigger button is pressed
        if (rightHandDevice.TryGetFeatureValue(CommonUsages.triggerButton, out bool triggerPressed) && triggerPressed)
        {
            OnEnable();
        }
        else{
            OnDisable();
        }
    }

    void SensorScannedEvent(long markerId)
    {
        switch (markerId)
        {
            case 100:
                whiteListIds.Add(350);
                displayInfo.ShowInfo("Place in Shelf 350!");
                break;

            case 101:
                whiteListIds.Add(351);
                displayInfo.ShowInfo("Place in Shelf 351!");
                break;

            default:
                break;
        }
    }

    void ParcelScannedEvent(long markerId)
    {
        switch (markerId)
        {
            case 200:
                whiteListIds.Add(100);
                displayInfo.ShowInfo("Install Sensor 100!");
                break;

            case 201:
                whiteListIds.Add(101);
                displayInfo.ShowInfo("Install Sensor 101!");
                break;

            default:
                break;
        }
    }

    void ShelfScannedEvent(long markerId)
    {
        switch (markerId)
        {
            case 350:
                GameObject go = Instantiate(markerPrefab);
                markerTransform = go.transform;
                go.name = marker.id.ToString();
                markerTransform.SetParent(xrRig);
                MarkerVisualizer visualizer = go.GetComponent<MarkerVisualizer>();
                markerVisualizers.Add(marker.id, visualizer);
                visualizer.SetMarkerData(marker);
                displayInfo.ShowInfo("Update Inventory!");
                whiteListIds.Clear();
                break;

            case 351:
                GameObject go = Instantiate(markerPrefab);
                markerTransform = go.transform;
                go.name = marker.id.ToString();
                markerTransform.SetParent(xrRig);
                MarkerVisualizer visualizer = go.GetComponent<MarkerVisualizer>();
                markerVisualizers.Add(marker.id, visualizer);
                visualizer.SetMarkerData(marker);
                displayInfo.ShowInfo("Update Inventory!");
                whiteListIds.Clear();
                break;

            default:
                break;
        }
    }

    private void OnEnable()
    {
        // Enable Varjo Marker tracking.
        VarjoMarkers.EnableVarjoMarkers(true);
    }

    private void OnDisable()
    {
        // Disable Varjo Marker tracking.
        VarjoMarkers.EnableVarjoMarkers(false);
    }
}

