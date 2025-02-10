using UnityEngine;
using ZXing;
using ZXing.QrCode;
using UnityEngine.XR;

public class QRCodeScanner : MonoBehaviour
{
    private WebCamTexture webcamTexture;
    private BarcodeReader barcodeReader;
    private bool isScanning = false;

    // Reference to the DisplayInfo script
    public DisplayInfo displayInfo;

    void Start()
    {
        // Initialize the webcam texture
        webcamTexture = new WebCamTexture();
        GetComponent<Renderer>().material.mainTexture = webcamTexture;
        webcamTexture.Play();

        // Initialize the barcode reader
        barcodeReader = new BarcodeReader();
    }

    void Update()
    {
        // Check for controller button press (e.g., trigger button)
        if (CheckForControllerInput())
        {
            StartScanning();
        }

        // Only scan when isScanning is true
        if (isScanning && webcamTexture.isPlaying && webcamTexture.didUpdateThisFrame)
        {
            try
            {
                // Decode the QR/barcode
                var result = barcodeReader.Decode(webcamTexture.GetPixels32(), webcamTexture.width, webcamTexture.height);
                if (result != null)
                {
                    Debug.Log("Scanned: " + result.Text);
                    // Call the ShowInfo method to display the result
                    displayInfo.ShowInfo(result.Text);
                    StopScanning();
                }
            }
            catch (System.Exception ex)
            {
                Debug.LogError(ex.Message);
                StopScanning();
            }
        }
    }

    void StartScanning()
    {
        isScanning = true;
        Debug.Log("Started scanning...");
    }

    void StopScanning()
    {
        isScanning = false;
        Debug.Log("Stopped scanning.");
    }

    bool CheckForControllerInput()
    {
        // Get the right-hand controller device
        var rightHandDevice = InputDevices.GetDeviceAtXRNode(XRNode.RightHand);

        // Check if the trigger button is pressed
        if (rightHandDevice.TryGetFeatureValue(CommonUsages.triggerButton, out bool triggerPressed) && triggerPressed)
        {
            return true;
        }

        return false;
    }
}