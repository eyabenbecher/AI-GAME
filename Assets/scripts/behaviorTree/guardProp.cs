using UnityEngine;
using TheKiwiCoder;

[System.Serializable]
public class guardProp : MonoBehaviour { 

    public Transform Player; // La position du player
    public float fovDistance = 100.0f;// Champ de vision: distane
    public float fovAngle = 45.0f;//Champ de vision: angle

    //  --------- pour la poursuite -----------------------
    public float vitesse_poursuite = 2.0f;
    public float vitesseRot_poursuite = 2.0f;
    public float precision_poursuite = 1.0f;
    //-------------- pour la patrouille --------------------
    public float distance_patrouille = 10.0f;
    //[SerializeField] private float attente_patrouille = 3.0f;
    float timing_patrouille = 0.0f;
}