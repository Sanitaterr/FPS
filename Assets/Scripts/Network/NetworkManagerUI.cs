using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;
using Unity.Netcode.Transports.UTP;
using Unity.Netcode.Transports.UNET;

public class NetworkManagerUI : MonoBehaviour
{
    [SerializeField]
    private Button hostBtn;
    [SerializeField]
    private Button serverBtn;
    [SerializeField]
    private Button clientBtn;

    [SerializeField]
    private Button room1;
    [SerializeField]
    private Button room2;

    // Start is called before the first frame update
    void Start()
    {
        var args = System.Environment.GetCommandLineArgs(); // ���ص�ǰ��������������в���
        
        for (int i = 0; i < args.Length; i++)
        {
            if (args[i] == "-port")
            {
                int port = int.Parse(args[i + 1]);
                var transport = GetComponent<UNetTransport>();
                transport.ConnectPort = transport.ServerListenPort = port;
            }
        }

        for (int i = 0; i < args.Length; i++)
        {
            if (args[i] == "-lauch-as-server")
            {
                NetworkManager.Singleton.StartServer();
                DestroyAllButtons();
            }
        }

        room1.onClick.AddListener(() =>
        {
            var transport = GetComponent<UNetTransport>();
            transport.ConnectPort = transport.ServerListenPort = 7777;
            NetworkManager.Singleton.StartClient();
            DestroyAllButtons();
        });

        room2.onClick.AddListener(() =>
        {
            var transport = GetComponent<UNetTransport>();
            transport.ConnectPort = transport.ServerListenPort = 7778;
            NetworkManager.Singleton.StartClient();
            DestroyAllButtons();
        });

        hostBtn.onClick.AddListener(() =>
        {
            NetworkManager.Singleton.StartHost(); // �����hostBtn��ťʱ�Զ�����StartHost����
            DestroyAllButtons();
        });
        serverBtn.onClick.AddListener(() =>
        {
            NetworkManager.Singleton.StartServer(); // �����hostBtn��ťʱ�Զ�����StartServer����
            DestroyAllButtons();
        });
        clientBtn.onClick.AddListener(() =>
        {
            NetworkManager.Singleton.StartClient(); // �����hostBtn��ťʱ�Զ�����StartClient����
            DestroyAllButtons();
        });
    }

    private void DestroyAllButtons()
    {
        Destroy(hostBtn.gameObject);
        Destroy(serverBtn.gameObject);
        Destroy(clientBtn.gameObject);
        Destroy(room1.gameObject);
        Destroy(room2.gameObject);
    }
}