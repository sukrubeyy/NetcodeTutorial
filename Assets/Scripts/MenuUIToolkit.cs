using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UIElements;

public class MenuUIToolkit : MonoBehaviour
{
    public UIDocument _document;
    public StyleSheet _menuStyle;

    private void Start()
    {
        var root = _document.rootVisualElement;
        root.styleSheets.Add(_menuStyle);

        var buttonWrapper = Create("ButtonWrapper");
        var HostButton = Create<Button>("CustomButton","border-box");
        HostButton.text = "Host";
        HostButton.clicked += ((() =>
        {
            NetworkManager.Singleton.StartHost();
        }));

        var ClientButton = Create<Button>("CustomButton","border-box");
        ClientButton.text = "Client";
        ClientButton.clicked += ((() =>
        {
            NetworkManager.Singleton.StartClient();
        }));
        
        
        var ServerButton = Create<Button>("CustomButton","border-box");
        ServerButton.text = "Server";
        ServerButton.clicked += ((() =>
        {
            NetworkManager.Singleton.StartServer();
        }));
        
        
        var QuitButton = Create<Button>("CustomButton","border-box");
        QuitButton.text = "Quit";
        QuitButton.clicked += ((() =>
        {
            Application.Quit();
        }));
        
        buttonWrapper.Add(HostButton);
        buttonWrapper.Add(ClientButton);
        buttonWrapper.Add(ServerButton);
        buttonWrapper.Add(QuitButton);
        root.Add(buttonWrapper);
        
    }

    private VisualElement Create(params string[] classNames)
    {
        return Create<VisualElement>(classNames);
    }

    private T Create<T>(params string[] classNames) where T : VisualElement, new()
    {
        var ele = new T();
        foreach (var name in classNames)
        {
            ele.AddToClassList(name);
        }

        return ele;
    }
    private void OnGUI()
    {
        if (GUILayout.Button("Host"))
        {
            NetworkManager.Singleton.StartHost();
        }

        if (GUILayout.Button("Client"))
        {
            NetworkManager.Singleton.StartClient();
        }

        if (GUILayout.Button("Server"))
        {
            NetworkManager.Singleton.StartServer();
        }
    }
}
