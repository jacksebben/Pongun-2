using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DifficultyButton : MonoBehaviour
{   
    private Image image;

    // color constant
    private Color32 unselectedColor = new(255, 255, 255, 255);
    private Color32 selectedColor = new(150, 150, 230, 255);
    
    private void Awake()
    {
        image = GetComponent<Image>();
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    public void Select(bool selected)
    {
        image.color = selected ? selectedColor : unselectedColor;
    }
}
