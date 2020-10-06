using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_GridControl : MonoBehaviour
{
    [Range(5, 50)]
    public int minSize = 10;

    [Range(5, 50)]
    public int maxSize = 50;



    [SerializeField]
    private Button buildButton;

    [SerializeField]
    private InputField xSizeInputField;

    [SerializeField]
    private InputField ySizeInputField;

    private void Start()
    {
        OnBuildButtonClick();
    }

    /// <summary>
    /// Построить сетку
    /// </summary>
    public void OnBuildButtonClick()
    {
        if (xSizeInputField && ySizeInputField)
        {
            if (int.TryParse(xSizeInputField.text, out int x) && int.TryParse(ySizeInputField.text, out int y))
            {
                Vector2Int size = new Vector2Int(Mathf.Clamp(x, minSize, maxSize), Mathf.Clamp(y, minSize, maxSize));
                NavGrid.Singleton.Build(size);
            }
        }
        else
        {
            Debug.LogError("Missing InputField size references");
        }
    }

    /// <summary>
    /// Проверка на ввод размера сетки
    /// </summary>
    public void CheckSize()
    {
        if (int.TryParse(xSizeInputField.text, out int value)) xSizeInputField.text = Mathf.Clamp(value, minSize, maxSize).ToString();
        else xSizeInputField.text = minSize.ToString();
        if (int.TryParse(ySizeInputField.text, out value)) ySizeInputField.text = Mathf.Clamp(value, minSize, maxSize).ToString();
        else ySizeInputField.text = minSize.ToString();
    }
}
