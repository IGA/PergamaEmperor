#nullable enable
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using Newtonsoft.Json;
using PergamaApp;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public TMP_Text story;
    public Canvas canvas;
    public GameObject actionButtonPrefab;
    
    private Dictionary<string, object> _state;

    private const string StoryNodesJson = @"
    [
        {
            ""Id"": 1,
            ""StoryText"": ""Soru #1"",
            ""Options"": [
                {""Text"": ""Cevap #1"", ""NextNodeId"": 2, ""HandlePropertyName"": null, ""HandlePropertyValue"": null, ""RequiredKey"": null, ""RequiredValue"": null},
                {""Text"": ""Cevap #2"", ""NextNodeId"": 3, ""HandlePropertyName"": null, ""HandlePropertyValue"": null, ""RequiredKey"": null, ""RequiredValue"": null}
            ]
        },
        {
            ""Id"": 2,
            ""StoryText"": ""Soru #2"",
            ""Options"": [
                {""Text"": ""Cevap #2.1"", ""NextNodeId"": 4, ""HandlePropertyName"": null, ""HandlePropertyValue"": null, ""RequiredKey"": null, ""RequiredValue"": null},
                {""Text"": ""Cevap #2.2"", ""NextNodeId"": 5, ""HandlePropertyName"": null, ""HandlePropertyValue"": null, ""RequiredKey"": null, ""RequiredValue"": null}
            ]
        },
        {
            ""Id"": 3,
            ""StoryText"": ""Soru #3"",
            ""Options"": [
                {""Text"": ""Cevap #3.1"", ""NextNodeId"": 6, ""HandlePropertyName"": null, ""HandlePropertyValue"": null, ""RequiredKey"": null, ""RequiredValue"": null},
                {""Text"": ""Cevap #3.2"", ""NextNodeId"": 7, ""HandlePropertyName"": null, ""HandlePropertyValue"": null, ""RequiredKey"": null, ""RequiredValue"": null}
            ]
        },
        {
            ""Id"": 4,
            ""StoryText"": ""Soru #4"",
            ""Options"": [
                {""Text"": ""Cevap #4.1"", ""NextNodeId"": 1, ""HandlePropertyName"": null, ""HandlePropertyValue"": null, ""RequiredKey"": null, ""RequiredValue"": null},
                {""Text"": ""Cevap #4.2"", ""NextNodeId"": 1, ""HandlePropertyName"": null, ""HandlePropertyValue"": null, ""RequiredKey"": null, ""RequiredValue"": null}
            ]
        },
        {
            ""Id"": 5,
            ""StoryText"": ""Soru #5"",
            ""Options"": [
                {""Text"": ""Tekrar 1"", ""NextNodeId"": 1, ""HandlePropertyName"": null, ""HandlePropertyValue"": null, ""RequiredKey"": null, ""RequiredValue"": null},
                {""Text"": ""Tekrar 2"", ""NextNodeId"": 1, ""HandlePropertyName"": null, ""HandlePropertyValue"": null, ""RequiredKey"": null, ""RequiredValue"": null}
            ]
        },
        {
            ""Id"": 6,
            ""StoryText"": ""Soru #6"",
            ""Options"": [
                {""Text"": ""Tekrar 1"", ""NextNodeId"": 1, ""HandlePropertyName"": null, ""HandlePropertyValue"": null, ""RequiredKey"": null, ""RequiredValue"": null},
                {""Text"": ""Tekrar 2"", ""NextNodeId"": 1, ""HandlePropertyName"": null, ""HandlePropertyValue"": null, ""RequiredKey"": null, ""RequiredValue"": null}
            ]
        },
        {
            ""Id"": 7,
            ""StoryText"": ""Soru #7"",
            ""Options"": [
                {""Text"": ""Tekrar 1"", ""NextNodeId"": 1, ""HandlePropertyName"": null, ""HandlePropertyValue"": null, ""RequiredKey"": null, ""RequiredValue"": null},
                {""Text"": ""Tekrar 2"", ""NextNodeId"": 1, ""HandlePropertyName"": null, ""HandlePropertyValue"": null, ""RequiredKey"": null, ""RequiredValue"": null}
            ]
        }
    ]
    ";

    private StoryBlock[]? _nodes;
    
    void Start()
    {
        _nodes = JsonConvert.DeserializeObject<StoryBlock[]>(StoryNodesJson);
        ShowNode(1);
    }

    void ShowNode(int? id)
    {
        StoryBlock? storyBlock = _nodes?.FirstOrDefault(n => n.Id == id);
        if (storyBlock != null)
        {
            story.text = storyBlock.StoryText;

            var index = 0;
            float canvasWidth = canvas.GetComponent<RectTransform>().sizeDelta.x;
            float canvasHeight = canvas.GetComponent<RectTransform>().sizeDelta.y;
            float totalSpacing = 24 * 2 + 12 * (storyBlock.Options.Length - 1); // Toplam boşluk: Kenar boşlukları + butonlar arası boşluk
            float buttonWidth = (canvasWidth - totalSpacing) / storyBlock.Options.Length; // Her butonun genişliği
            
            foreach (StoryOption storyOption in storyBlock.Options)
            {
                GameObject buttonInstance = Instantiate(actionButtonPrefab, canvas.transform, false);
                RectTransform rectTransform = buttonInstance.GetComponent<RectTransform>();
                
                float posX = (-canvasWidth / 2) + 24 + buttonWidth / 2 + (buttonWidth + 12) * index;
                rectTransform.anchoredPosition = new Vector2(posX, canvasHeight / 2 * -1 + 100 + 24); // X pozisyonunu ayarla
                rectTransform.sizeDelta = new Vector2(buttonWidth, 100); // Buton boyutunu ayarla
                
                Button buttonObj = buttonInstance.GetComponent<Button>();
            
                // TMP Text komponenti ekleyin
                TMP_Text tmpText = buttonObj.GetComponentInChildren<TMP_Text>();
                tmpText.text = storyOption.Text;
            
                // Text'in RectTransform ayarlarını yapılandırma
                RectTransform textRectTransform = tmpText.GetComponent<RectTransform>();
                textRectTransform.sizeDelta = new Vector2(200, 50); // Text boyutunu buton boyutuna eşitle
            
                // Butona bir eylem ekleyin (opsiyonel)
                buttonObj.onClick.AddListener(() => SelectOption(storyOption));
                index++;
            }
            
        }
    }

    bool ShowOption(StoryOption option)
    {
        return option.Required(_state) == true;
    }
    
    void SelectOption(StoryOption option)
    {
        var nextNodeId = option.NextNodeId;
        option.Handle(_state);
        ShowNode(nextNodeId);
    }
}
