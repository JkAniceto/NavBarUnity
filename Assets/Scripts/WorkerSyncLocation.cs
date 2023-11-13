using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.Networking;

public class WorkerSyncLocation : MonoBehaviour
{
    private string updateLocationURL = "http://127.0.0.1:8000/api/updateLongLat";
    public Button syncLocationButton;

    void Start()
    {
        syncLocationButton.onClick.AddListener(SyncLocation);
    }

    // Method to be called when the sync location button is clicked
    public void SyncLocation()
    {
        StartCoroutine(CheckAndStartLocationService());
    }

    IEnumerator CheckAndStartLocationService()
    {
        // Check if location services are enabled
        if (!Input.location.isEnabledByUser)
        {
            // Request location service permission
            Input.location.Start();
            yield return new WaitForSeconds(1); // Wait for a moment
        }

        // Check again if location services are enabled
        if (Input.location.isEnabledByUser)
        {
            // Location services are enabled, proceed with getting the location
            yield return StartCoroutine(GetLocation());
        }
        else
        {
            Debug.LogError("Location services are not enabled");
        }
    }

    IEnumerator GetLocation()
    {
        // Start location service updates with desired accuracy and distance filters.
        Input.location.Start();

        // Wait until service initializes
        int maxWait = 20;
        while (Input.location.status == LocationServiceStatus.Initializing && maxWait > 0)
        {
            yield return new WaitForSeconds(1);
            maxWait--;
        }

        // Service didn't initialize in 20 seconds
        if (maxWait < 1)
        {
            Debug.LogError("Timed out while waiting for location service to initialize");
            yield break;
        }

        // Connection has failed
        if (Input.location.status == LocationServiceStatus.Failed)
        {
            Debug.LogError("Unable to determine device location");
            yield break;
        }
        else
        {
            // Access the device location data
            float latitude = Input.location.lastData.latitude;
            float longitude = Input.location.lastData.longitude;

            Debug.Log("Latitude: " + latitude + ", Longitude: " + longitude);

            // Stop service if there is no need to query location updates continuously
            Input.location.Stop();

            // Convert the location data to a JSON string
            string locationJson = JsonUtility.ToJson(new LocationData(latitude, longitude));

            // Send the location data to the server
            yield return StartCoroutine(UpdateLocation(locationJson));
        }
    }

    IEnumerator UpdateLocation(string locationJson)
    {
        // Send a UnityWebRequest to update the location
        UnityWebRequest request = UnityWebRequest.Post(updateLocationURL, locationJson);
        request.SetRequestHeader("Content-Type", "application/json");

        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.ConnectionError || request.result == UnityWebRequest.Result.ProtocolError)
        {
            Debug.LogError("Error updating location: " + request.error);
        }
        else
        {
            Debug.Log("Location updated successfully!");
        }
    }

    [System.Serializable]
    private class LocationData
    {
        public float latitude;
        public float longitude;

        public LocationData(float lat, float lon)
        {
            latitude = lat;
            longitude = lon;
        }
    }
}
