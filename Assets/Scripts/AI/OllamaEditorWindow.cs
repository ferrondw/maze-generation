#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

public class OllamaEditorWindow : EditorWindow
{
    private string userMessage = ""; 
    private Vector2 scrollPos; 
    private string chatHistory = ""; 

    
    [MenuItem("Tools/Ollama")]
    public static void ShowWindow()
    {
        GetWindow<OllamaEditorWindow>("Ollama");
        
        Ollama.SetSystemPrompt(@"You are an AI code assistant in the Unity Engine. When asked to generate code, format it inside markdown with C# syntax like this:
```cs

```
For example, if the user says 'Create 10 cubes in random locations', the output should be:
```cs
for(int i = 0; i < 10; i++) {
    GameObject.CreatePrimitive(PrimitiveType.Cube).transform.position = new Vector3(UnityEngine.Random.Range(-10, 10), 0, UnityEngine.Random.Range(-10, 10));
}
```
Your responses must always be in a format that can be executed directly by the Unity environment.
Also keep in mind that all the code you write will be put inside of an Execute method, so you cannot use 'using' tags and such inside your code.
These are the namespaces that are included in the code:
using System;
using System.Linq;
using System.Reflection;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEditor;
So do not use any other namespaces.
Also keep in mind some of this code may be executed in Edit mode, so make sure code also works there (Think of Destroy not working in edit mode, but DestroyImmediate does)
With that, do NOT give any explanation to the code since it will NOT be shown to the user, JUST write the code inside of ```cs markdown");
    }
    
    private void OnGUI()
    {
        scrollPos = EditorGUILayout.BeginScrollView(scrollPos, GUILayout.Height(300));
        GUILayout.Label(chatHistory, EditorStyles.wordWrappedLabel);
        EditorGUILayout.EndScrollView();
        
        userMessage = EditorGUILayout.TextField("Your Message:", userMessage);
        
        if (GUILayout.Button("Send"))
        {
            if (!string.IsNullOrEmpty(userMessage))
            {
                SendMessageToOllama(userMessage);
                userMessage = ""; 
            }
        }
        
        if (GUILayout.Button("Reset Chat"))
        {
            Ollama.Reset();
            chatHistory = "Chat history cleared.\n";
        }
    }
    
    private async void SendMessageToOllama(string message)
    {
        chatHistory += $"\nUser: {message}";
        
        await Ollama.Generate(message);
        var lastMessage = Ollama.LastMessage();
        
        if (!string.IsNullOrEmpty(lastMessage))
        {
            chatHistory += $"\nAI: {lastMessage}";
            DynamicCodeExecutor.Execute(lastMessage);
        }
        
        Repaint();
    }
}
#endif