using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;

public class UI_CanvasScript : MonoBehaviour
{
    // Start is called before the first frame update

    [SerializeField]
    GameObject PressSpace;

    public Text _text;

    [SerializeField]
    GameObject ShowMenu;
    void Start()
    {
        PressSpace.SetActive(false);
        ShowMenu.SetActive(false);
    }
    public void OpenPressIMenu(bool IsTrue)
    {
        PressSpace.SetActive(IsTrue);
    }

    public void OpenListOption(bool IsTrue)
    {
        ShowMenu.SetActive(IsTrue);
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
