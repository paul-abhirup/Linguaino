using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Networking;
using System.Collections;



public class ChatManager : MonoBehaviour
{
    public TMP_InputField inputField;
    public Button sendButton;

    public GameObject chatMessagePrefab; // Drag the prefab to this field in the Inspector
    public Transform content;            // Reference to the Content object


    private string apiUrl = "https://hono-barista.abhiruppaul1249.workers.dev/chat";

    void Start()
    {
        sendButton.onClick.AddListener(OnSendButtonClick);
    }

    void OnSendButtonClick()
    {
        string playerMessage = inputField.text;
        if (!string.IsNullOrEmpty(playerMessage))
        {
            AddChatMessage("Player: " + playerMessage);
            StartCoroutine(SendMessageToAPI(playerMessage));
            inputField.text = ""; // Clear input field
        }
    }

    IEnumerator SendMessageToAPI(string message)
    {
        WWWForm form = new WWWForm();
        form.AddField("message", message);

        using (UnityWebRequest request = UnityWebRequest.Post(apiUrl, form))
        {
            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.ConnectionError || request.result == UnityWebRequest.Result.ProtocolError)
            {
                Debug.LogError("Error: " + request.error);
            }
            else
            {
                string response = request.downloadHandler.text;
                AddChatMessage("Barista: " + response);
            }
        }
    }

    void AddChatMessage(string message)
    {
        GameObject chatMessageObject = Instantiate(chatMessagePrefab, content);
        TextMeshProUGUI messageText = chatMessageObject.GetComponent<TextMeshProUGUI>();
        messageText.text = message;
    }
}