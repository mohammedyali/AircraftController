using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class AircraftHUD : MonoBehaviour
{
    public AircratController aircrat;
    public Text Speed;
    public Text Altitude;
    public Transform Cursor;
    public Transform AimPoint;
    private Camera camera;

    private void Start()
    {
        camera = Camera.main;
    }

    // Update is called once per frame
    void Update()
    {
        Cursor.position = camera.WorldToScreenPoint(AimPoint.transform.position);
        Speed.text = aircrat.LocalVelocity.z * 5.76f + " KPH";
        Altitude.text = aircrat.transform.position.y + " M";
    }
}
