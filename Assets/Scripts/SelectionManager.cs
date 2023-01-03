using StarterAssets;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using Unity.VisualScripting;
using UnityEditor;
using UnityEditor.Presets;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.Profiling;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using static UnityEditor.Rendering.InspectorCurveEditor;

public class SelectionManager : MonoBehaviour
{
    public FirstPersonController fps;
    Renderer[] allChildRenderers;
    private bool firstObjectwithTag = true;
    public GameObject hittedfirst = null;

    private string SelectableTag = "Selectable";
    private string SelectableLightTag = "SelectableLight";
    private string SelectableMeshTag = "SelectableMesh";

    [SerializeField]
    public GameObject firstObj, secondObj;

    int versMesh = 1;

    private bool iPress = false;

    private GameObject hitObject = null;

    [SerializeField]
    private float emissiveIntensity;

    public UI_CanvasScript USpace_Panel;
    public UI_Script UI_List;

    private Color emissiveColor = Color.yellow; //je choisi la couleur de mon highlight

    public Renderer selectionRenderer;

    private void Start()
    {
    }



    // Update is called once per frame
    private void Update()
    {

        Ray ray = new Ray(Camera.main.transform.position, Camera.main.transform.forward);
        RaycastHit hit;

        //Debug.DrawRay(ray.origin, ray.direction, Color.red);

        if (Physics.Raycast(ray, out hit))
        {
            Transform selection = hit.transform; //on stock le transfome de l'objet touché
            
            if (selection.CompareTag(SelectableTag) || selection.CompareTag(SelectableLightTag) || selection.CompareTag(SelectableMeshTag)) //si l'objet touché est quelque chose sur lequel on peut agir
            {
                if(iPress == false)
                { 
                USpace_Panel.OpenPressIMenu(true);
                }


                if (firstObjectwithTag == true)     // et si c'est le premier objet avec lequel on peut interagir
                                                    //(au cas ou deux objets interactifs sont collés)
                {
                hittedfirst = selection.gameObject;
                    firstObjectwithTag = false;
                    allChildRenderers = hittedfirst.GetComponentsInChildren<Renderer>();
                    if (allChildRenderers == null)
                    { 

                        selectionRenderer = hittedfirst.GetComponent<Renderer>();//ici je recupere la partie "render"
                                                                             //de mon objet que je viens de toucher
                        selectionRenderer.material.EnableKeyword("_EMISSION");// j'active l'emission et je change la couleur
                        selectionRenderer.material.SetColor("_EmissionColor", emissiveColor * emissiveIntensity);
                    }
                    else
                    { 
                    for (int i = 0; i < allChildRenderers.Length; i++)
                    {
                        allChildRenderers[i].material.EnableKeyword("_EMISSION");// j'active l'emission et je change la couleur
                        allChildRenderers[i].material.SetColor("_EmissionColor", emissiveColor * emissiveIntensity);
                    }
                    }
                }
                else if (firstObjectwithTag == false) //si ce n'est pas le premier objet interactif que je croise
                {
                    selectionRenderer = hittedfirst.GetComponent<Renderer>();//j'eteint le premier
                    selectionRenderer.material.DisableKeyword("_EMISSION");
                    hittedfirst = selection.gameObject;//je recupere le nouveau objet a allumer
                    selectionRenderer = hittedfirst.GetComponent<Renderer>();//j'allume le deuxieme
                    selectionRenderer.material.EnableKeyword("_EMISSION");
                    selectionRenderer.material.SetColor("_EmissionColor", emissiveColor * emissiveIntensity);

                }
            }
            else // si l'objet touché n'a pas de tag
            {
                USpace_Panel.OpenPressIMenu(false);
                USpace_Panel.OpenListOption(false);

                if (hittedfirst != null) //si mon objet touché est différents de null alors je dois l'eteindre
                {
                    allChildRenderers = hittedfirst.GetComponentsInChildren<Renderer>();
                    if (allChildRenderers == null)
                    {

                        selectionRenderer = hittedfirst.GetComponent<Renderer>();
                        selectionRenderer.material.DisableKeyword("_EMISSION");
                    }
                    else
                    {
                        for (int i = 0; i < allChildRenderers.Length; i++)
                        {
                            allChildRenderers[i].material.DisableKeyword("_EMISSION");// j'active l'emission et je change la couleur
                        }
                    }
                    selectionRenderer = hittedfirst.GetComponent<Renderer>();
                selectionRenderer.material.DisableKeyword("_EMISSION");
                firstObjectwithTag = true;//je repasse toutes mes validations en etat neutre 
                hittedfirst = null;
                iPress = false;
                }
            }
        }

        if (Input.GetKeyDown(KeyCode.I) && hittedfirst != null && !iPress) // si la touche i est pressée
        {
            if (hittedfirst.tag == SelectableTag) // je compare le tag, si c'est un tag "objet" alors :
            {
                iPress = true;
                //j'ouvre les menus et desactive le fps pour pouvoir acceder au menus tout en laissant mon raycast au bon  endroit
                USpace_Panel._text.text = "Press I again to close";

                USpace_Panel.OpenListOption(true);
                fps.GetComponent<FirstPersonController>().enabled = false;
                Cursor.lockState = CursorLockMode.Confined;
                Cursor.visible = true;


            }
            if (hittedfirst.tag == SelectableLightTag)//je compare le tag, si c'est un tag light alors :
            {
                Light Light = hittedfirst.gameObject.GetComponentInChildren<Light>(); // je recupere dans les enfants l'objet lumiere

                if (Light.enabled) // petite logique pour eteindre et allumer
                    Light.enabled = false;
                else
                    Light.enabled = true;


            }
            if (hittedfirst.tag == SelectableMeshTag)//je compare le tag, si c'est un tag Mesh alors :
            {
                versMesh = 1;

                if (versMesh == 1)
                {
                    GameObject newObject;
                newObject = Instantiate(secondObj) as GameObject;
                newObject.transform.position = hittedfirst.transform.position;
                newObject.transform.rotation = hittedfirst.transform.rotation;
                newObject.transform.parent = hittedfirst.transform.parent;
                    newObject.tag = SelectableMeshTag;
                    versMesh = 2;
                    DestroyImmediate(hittedfirst);
                    hittedfirst = newObject;
                }
                else if (versMesh == 2)
                {
                    GameObject newObject;
                    newObject = Instantiate(firstObj) as GameObject;
                    newObject.transform.position = hittedfirst.transform.position;
                    newObject.transform.rotation = hittedfirst.transform.rotation;
                    newObject.transform.parent = hittedfirst.transform.parent;
                    newObject.tag = SelectableMeshTag;
                    versMesh = 1;
                    DestroyImmediate(hittedfirst);
                    hittedfirst = newObject;
                }


            }
        }
        else if (Input.GetKeyDown(KeyCode.I) && hittedfirst != null && iPress) // si la touche I est réappuyer alors je ferme les menus, réactive le fps
        {
            USpace_Panel.OpenPressIMenu(true);
            USpace_Panel.OpenListOption(false);
            fps.GetComponent<FirstPersonController>().enabled = true;
            //Cursor.lockState = CursorLockMode.Confined;
            Cursor.visible = false;
            iPress = false;
            USpace_Panel._text.text = "Press I key to interact";
        }

    }
}
