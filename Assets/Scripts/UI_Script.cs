using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Unity.VisualScripting;
using JetBrains.Annotations;

public static class ButtonExtension
{
    //ici ce n'est pas tres clair ... il est question de delegate j'en conclu que quand on click sur un bouton
    //on envoie "des trucs" au truc qui sont abonnées   a ce delegate...
    public static void AddEventListener<T> (this Button button, T param,Action<T> OnClick)
    {
        button.onClick.AddListener (delegate()
        {
            OnClick (param); 
        });
    }
}


public class UI_Script : MonoBehaviour
{
    //ici un petit tour de pass pass pour convertir mes images en sprite.
    public int ThisIsClicked;

    [SerializeField]
    public SelectionManager ManagerScript;
    public static Sprite TextureToSprite(Texture2D texture) => Sprite.Create(texture, new Rect(0f, 0f, texture.width, texture.height), new Vector2(0.5f, 0.5f), 50f, 0, SpriteMeshType.FullRect);
    List<int> _list = new List<int>();
    [System.Serializable] //declaration de la struct de choix
    public struct GameObjText
    {
        //public int IndexI;
        public string Name;
        public string Description;
        public Texture2D ImageToGo;
    }

    [SerializeField] // on affiche et on remplis le struct dans l'editeur
    GameObjText[] AllObjT;

    int selectedItem = 1;
    // Start is called before the first frame update
    void Start()
    {
        GameObject buttontemplate = transform.GetChild(0).gameObject;
        GameObject g;
        // on populate la liste
        int N = AllObjT.Length;

        for (int i = 0; i < N; i++) // on populate la liste avec des boutons
        {
            g = Instantiate(buttontemplate, transform);
            Sprite spriteToGo = TextureToSprite(AllObjT[i].ImageToGo);
            g.transform.GetChild(0).GetComponent<Text>().text = AllObjT[i].Name;
            g.transform.GetChild(1).GetComponent<Text>().text = AllObjT[i].Description;
            g.transform.GetChild(2).GetComponent<Image>().sprite = spriteToGo;

            int v = g.GetInstanceID();
            _list.Add(v);
            //ici on ajoute pour chaque bouton un eventListener ?
            g.GetComponent<Button>().AddEventListener(i, ItemClicked);
        }
        Destroy(buttontemplate);// on detruit le bouon qui nous a servi de model
    }


    public void ItemClicked(int itemIndex) // ici je comprend pas d'ou vient le item index ...
    {

        GameObject CurrentO = ManagerScript.hittedfirst;
        CurrentO.GetComponent<Renderer>().material.mainTexture = AllObjT[itemIndex].ImageToGo;
    }
}
