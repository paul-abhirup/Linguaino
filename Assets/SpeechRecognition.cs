using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

public class SpeechManager : MonoBehaviour
{
    public string edenApiKey = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJ1c2VyX2lkIjoiZTNjZjA2MzEtNzhkMC00OThjLWFkNWUtNWIzZTMwNjFhYzIzIiwidHlwZSI6ImFwaV90b2tlbiJ9.u9SJChHHV5ZRN2iAmO-K6ip7wr8yYcrvx6h-KaZYf7o";
    private AudioClip audioClip;
    private string edenApiUrl = "https://api.edenai.run/v2/audio/speech_to_text";
    private string backendApiUrl = "https://hono-barista.abhiruppaul1249.workers.dev/chat"; // Your backend endpoint

    void Start()
    {
        // This method could be triggered by a button or key press
        StartRecording();
    }

    void StartRecording()
    {
        // Start recording for 5 seconds with 16k sample rate
        audioClip = Microphone.Start(null, false, 5, 16000);
        Debug.Log("Recording started...");
    }

    void StopRecordingAndSend()
    {
        // Stop recording and send to Eden AI
        if (Microphone.IsRecording(null))
        {
            Microphone.End(null);
            StartCoroutine(SendAudioToEden());
        }
    }

    IEnumerator SendAudioToEden()
    {
        if (audioClip == null)
        {
            Debug.LogError("No audio recorded");
            yield break;
        }

        // Convert AudioClip to WAV byte array
        byte[] audioData = WavUtility.FromAudioClip(audioClip);
        if (audioData == null)
        {
            Debug.LogError("Failed to convert audio to byte array");
            yield break;
        }

        // Create form to send audio data
        WWWForm form = new WWWForm();
        form.AddBinaryData("files", audioData, "speech.wav", "audio/wav");
        form.AddField("providers", "google");

        UnityWebRequest request = UnityWebRequest.Post(edenApiUrl, form);
        request.SetRequestHeader("Authorization", $"Bearer {edenApiKey}");

        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.ConnectionError || request.result == UnityWebRequest.Result.ProtocolError)
        {
            Debug.LogError($"Error: {request.error}");
        }
        else
        {
            string recognizedSpeech = request.downloadHandler.text;
            Debug.Log("Recognized Speech: " + recognizedSpeech);
            HandleUserInput(recognizedSpeech);
        }
    }

    void HandleUserInput(string userSpeech)
    {
        // Send the recognized text to your backend for processing
        Debug.Log("User input received: " + userSpeech);
        StartCoroutine(SendTextToBackend(userSpeech));
    }

    IEnumerator SendTextToBackend(string userSpeech)
    {
        WWWForm form = new WWWForm();
        form.AddField("message", userSpeech);

        UnityWebRequest request = UnityWebRequest.Post(backendApiUrl, form);

        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.ConnectionError || request.result == UnityWebRequest.Result.ProtocolError)
        {
            Debug.LogError($"Backend Error: {request.error}");
        }
        else
        {
            string baristaResponse = request.downloadHandler.text;
            Debug.Log("Barista Response: " + baristaResponse);
            SpeakBaristaResponse(baristaResponse);
        }
    }

    void SpeakBaristaResponse(string responseText)
    {
        Debug.Log("Barista speaks: " + responseText);
        StartCoroutine(TextToSpeech(responseText));
    }

    IEnumerator TextToSpeech(string responseText)
    {
        WWWForm form = new WWWForm();
        form.AddField("text", responseText);
        form.AddField("providers", "google"); // You can choose other providers as well

        UnityWebRequest request = UnityWebRequest.Post("https://api.edenai.run/v2/audio/text_to_speech", form);
        request.SetRequestHeader("Authorization", $"Bearer {edenApiKey}");

        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.ConnectionError || request.result == UnityWebRequest.Result.ProtocolError)
        {
            Debug.LogError($"TTS Error: {request.error}");
        }
        else
        {
            // Download the synthesized audio and play it
            AudioClip clip = DownloadHandlerAudioClip.GetContent(request);
            PlayResponseAudio(clip);
        }
    }

    void PlayResponseAudio(AudioClip clip)
    {
        if (clip == null)
        {
            Debug.LogError("TTS AudioClip is null");
            return;
        }

        AudioSource audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }

        audioSource.clip = clip;
        audioSource.Play();
    }

    void Update()
    {
        // Example trigger: Press the space bar to stop recording
        if (Input.GetKeyDown(KeyCode.Space))
        {
            StopRecordingAndSend();
        }
    }
}