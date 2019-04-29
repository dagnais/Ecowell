/*
Titulo: "Ecowell"
Hecho en el año:2018 
-----
Title: "Ecowell"
Made in the year: 2018
*/
using UnityEngine;
using AnimalsDetails;
using UnityEngine.UI;
using System.Collections.Generic;

public class MainController : MonoBehaviour {
    #region UI
    public bool isLerp;
    public Vector3 endMarker;

    private float lerpCount;

    [Header("Icons")]
    public Sprite activeSearchMarker;
    public Sprite normalMarker, farmerMarker,
        alertMarker, transMarker, outCorralMarker;

    [Header("UI Elements")]
    public GameObject[] menuButtons;
    public GameObject mapObject, panelSearch, contentViewMap, searchBtn, cancelSearchBtn;
    public InputField latObj, lonObj, searchField;
    public Button zoomInBtn, zoomOutBtn;
    Vector3 contentOriginPos;
    string searchText;

    #endregion
    [Header("Others")]
    int zoomLevel=5;
    GoogleMap googleMap;
    public GoogleMapLocation country;

    public Text pathDB;
    [HideInInspector]
    public Animals animalsDB;

    public List<MarkerController> mk;
    int mkI;

    void Start() {

        if (mapObject != null)
            googleMap = mapObject.GetComponent<GoogleMap>();
        else
            Debug.LogError("Mapa no asignado");


        animalsDB = new Animals();
        if (animalsDB == null)
            Debug.LogError("Variable vacia");

        if (pathDB == null)
            Debug.LogError("pathDB no esta asignada con un objeto de tipo UI Text");
        pathDB.text = "Path: " + animalsDB.pathAnimals;

        AutosearchCountry();

        contentOriginPos = contentViewMap.transform.localPosition;
    }

    void Update() {
        if (isLerp)
            LerpContent();
    }

    void StartLerp()
    {
        lerpCount = 0;
    }

    void LerpContent()
    {
        contentViewMap.transform.localPosition = Vector3.Lerp(contentViewMap.transform.localPosition,
            endMarker, 0.01f);
        Debug.Log(endMarker);
        if (lerpCount < 2f)
        {
            lerpCount += 0.01f;
        }
        else
        {
            isLerp = false;
        } 
    }

    public void SearchLocationCountry()
    {
        panelSearch.SetActive(false);
        mapObject.SetActive(true);
        googleMap.centerLocation.latitude = double.Parse(latObj.text);
        googleMap.centerLocation.longitude = double.Parse(lonObj.text);
        GetAmmountMarkers();
        googleMap.Refresh();
        country = googleMap.centerLocation;
    }

    void AutosearchCountry()
    {
        string[] st = new string[2];
        st = animalsDB.SearchInDBItem(0, animalsDB.dbAnimals, animalsDB.fieldsAnimal);
        mapObject.SetActive(true);
        googleMap.centerLocation = new GoogleMapLocation(string.Empty, double.Parse(st[0]), double.Parse(st[1]));
        googleMap.Refresh();
    }
    void GetAmmountMarkers()
    {
        string[] st = new string[2];
        googleMap.markers= new GoogleMapMarker[animalsDB.GetAmount()];
        for (int i = 0; i < googleMap.markers.Length; i++)
        {
            googleMap.markers[i] = new GoogleMapMarker(GoogleMapMarker.GoogleMapMarkerSize.Tiny, GoogleMapColor.white, "_", new GoogleMapLocation[1],GoogleMapMarker.State.none);
            googleMap.markers[i].locations[0] = new GoogleMapLocation("",-34.61092d, -68.3559d);
            st = animalsDB.SearchInDBItem(i, animalsDB.dbAnimals, animalsDB.fieldsAnimal);
            googleMap.markers[i].locations[0].latitude = double.Parse(st[0]);
            googleMap.markers[i].locations[0].longitude = double.Parse(st[1]);
        }
    }

    public void SearchByID()
    {
        for (int i = 0; i < mk.Count; i++)
        {
            if (mk[i].id == searchField.text)
            {
                mk[i].gameObject.transform.GetChild(0).gameObject.SetActive(true);//efecto
                endMarker = mk[i].transform.localPosition;

                Vector3 dif = contentOriginPos - contentViewMap.transform.localPosition;
                Debug.Log(dif);
                endMarker = endMarker - dif;

                endMarker = contentViewMap.transform.localPosition - endMarker;
                StartLerp();
                isLerp = true;
            }
        }

        cancelSearchBtn.SetActive(true);
        searchBtn.SetActive(false);
        searchText = searchField.text;
        searchField.text = string.Empty;
        searchField.interactable = false;
    }
    public void GetGlobalPosition(Transform pos)
    {
        Debug.Log(pos.position + "  /  local: " + pos.localPosition);
    }

    public void ExitSearch()
    {
        string[] st = new string[2];
        st = animalsDB.SearchItem(animalsDB.dbAnimals, searchText, animalsDB.fieldsAnimal);
        MarkerController[] mk = FindObjectsOfType<MarkerController>();
        for (int i = 0; i < mk.Length; i++)
        {
            if (mk[i].id == searchText)
            {
                mk[i].gameObject.transform.GetChild(0).gameObject.SetActive(false);
            }
        }
        searchText = string.Empty;
        cancelSearchBtn.SetActive(false);
        searchBtn.SetActive(true);

        endMarker = contentOriginPos;
        StartLerp();
        isLerp = true;

        searchField.text = string.Empty;
        searchField.interactable = true;
    }

    public void ShowHideMenuButtons()
    {
        for (int i = 0; i < menuButtons.Length; i++)
        {
            menuButtons[i].SetActive(!menuButtons[i].activeInHierarchy);
        }
    }

    public void ShowText(string value)
    {
        menuButtons[0].GetComponent<Text>().text = value;
    }

    public void HideText()
    {
        menuButtons[0].GetComponent<Text>().text = string.Empty;
    }

    public void Zoom(bool zoomIn)
    {
        if (zoomIn)
        {
            if (zoomLevel < 12)
            {
                zoomLevel += 1;
                contentViewMap.transform.localScale *= 1.1f;
                contentViewMap.transform.localPosition *= 1.1f;
            } 
        }
        else
        {
            if (zoomLevel > 1)
            {
                zoomLevel -= 1;
                contentViewMap.transform.localScale *= 0.9f;
                contentViewMap.transform.localPosition *= 0.9f;
            }
        }

        if (zoomLevel >= 11)
            zoomInBtn.interactable = false;

        else
            zoomInBtn.interactable = true;

        if (zoomLevel <= 1)
            zoomOutBtn.interactable = false;
        else
            zoomOutBtn.interactable = true;
    }
}
