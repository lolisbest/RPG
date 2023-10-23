using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RPG.Common;
using UnityEngine.Networking;
using System;

public class HttpCommunicate : MonoBehaviour
{
    public bool IsUsing { get; private set; }

    public string Response { get; private set; }

    void Awake()
    {
        IsUsing = false;
        Response = string.Empty;
    }
    public bool TryGet(string url, Action<string> callback)
    {
        if (IsUsing) return false;

        StartCoroutine(HttpGet(url, callback));
        return true;
    }

    public bool TryGet(string url)
    {
        if (IsUsing) return false;

        StartCoroutine(HttpGet(url));
        return true;
    }

    private IEnumerator HttpGet(string url, Action<string> callback = null)
    {
        IsUsing = true;
        ToggleProgressIndicator(true);

        using (UnityWebRequest request = UnityWebRequest.Get(url))
        {
            yield return request.SendWebRequest();

            ResultType result = ProcessResult(request, out string Response);
            switch (result)
            {
                case ResultType.RequestSuccess:
                    callback?.Invoke(Response);
                    break;
                case ResultType.RequestInProgress:
                    //Debug.Log("RequestInProgress");
                    break;
                //case ResultType.RequestConnectionError:
                //case ResultType.RequestProtocolError:
                //case ResultType.RequestDataProcessingError:
                default:
                    GameManager.Instance.OpenServerCommunicationErrorWindow(result.ToString());
                    //Debug.Log("default");
                    break;
            }
        }

        ToggleProgressIndicator(false);
        IsUsing = false;
    }

    public bool TryPost(string url, string data)
    {
        if (IsUsing) return false;

        Debug.Log("TryPost " + data);
        StartCoroutine(HttpPost(url, data));
        return true;
    }

    private IEnumerator HttpPost(string url, string data)
    {
        IsUsing = true;
        //ToggleProgressIndicator(true);

        using (UnityWebRequest request = UnityWebRequest.Post(url, data))
        {
            yield return request.SendWebRequest();

            ResultType result = ProcessResult(request, out string Response);
            switch (result)
            {
                case ResultType.RequestSuccess:
                    break;
                case ResultType.RequestInProgress:
                    break;
                default:
                    Debug.Log("Post Fail");
                    break;
            }
        }

        //ToggleProgressIndicator(false);
        IsUsing = false;
    }


    private void ToggleProgressIndicator(bool active)
    {
        GameManager.Instance.ToggleProgressIndicator(active);
    }

    protected virtual ResultType ProcessResult(UnityWebRequest req, out string result)
    {
        switch (req.result)
        {
            case UnityWebRequest.Result.InProgress:
                result = req.downloadHandler.text;
                return ResultType.RequestInProgress;

            case UnityWebRequest.Result.Success:
                result = req.downloadHandler.text;
                return ResultType.RequestSuccess;

            case UnityWebRequest.Result.ConnectionError:
                result = req.downloadHandler.text;
                return ResultType.RequestConnectionError;

            case UnityWebRequest.Result.ProtocolError:
                result = req.downloadHandler.text;
                return ResultType.RequestProtocolError;

            case UnityWebRequest.Result.DataProcessingError:
                result = req.downloadHandler.text;
                return ResultType.RequestDataProcessingError;

            default:
                result = req.downloadHandler.text;
                return ResultType.RequestUnknownCase;
        }
    }

}
