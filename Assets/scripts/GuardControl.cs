//#define DebugPlayerVisionTest

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem.Composites;
using UnityEngine.AI;
using UnityEngine.UIElements;

public class GuardControl : MonoBehaviour
{
    //-----------POUR LA FSM-------------------------
    enum Etats { Patrouiller, Chercher, Poursuivre };
    Etats etatActuel = Etats.Patrouiller;

    // --------Le Garde ----------------------
    public Transform Player; // La position du player
    [SerializeField] private float fovDistance = 20.0f;// Champ de vision: distane
    [SerializeField] private float fovAngle = 45.0f;//Champ de vision: angle

    //  --------- pour la poursuite -----------------------
    public float vitesse_poursuite = 2.0f;
    public float vitesseRot_poursuite = 2.0f;
    public float precision_poursuite = 5.0f;
    //-------------- pour la patrouille --------------------
    public float distance_patrouille = 10.0f;
    [SerializeField] private float attente_patrouille = 3.0f;
    float timing_patrouille = 0.0f;
    //------------------------------------------------------

    //
    Vector3 dernierEmplacementVu; //le dernier emplacement ou la player e été vu
    //-------------------------------------



    void Start()
    {
        timing_patrouille = attente_patrouille;
        dernierEmplacementVu = this.transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        Etats etatTemporaire = etatActuel;


#if DebugPlayerVisionTest
        if (ICanSee(Player))
        {
            Debug.Log("Player localisé à " + Player.position);
        }
        else
        {
            Debug.Log("RAS...");
        }
#endif
        if (ICanSee(Player))
        {   
            etatActuel = Etats.Poursuivre; //je le vois donc je le poursuit
            dernierEmplacementVu = Player.position;// je le vois donc j'enregistre sa position
        }
        else
        {
            if (etatActuel == Etats.Poursuivre)
            {
                etatActuel = Etats.Chercher;
            }
        }

        switch (etatActuel)
        {
            case Etats.Patrouiller:
                Patrouiller();
                break;
            case Etats.Chercher:
                Chercher();
                break;
            case Etats.Poursuivre:
                Poursuivre(Player);
                break;
        }
       
        if (etatTemporaire != etatActuel)
            Debug.Log("Etat du garde:" + etatActuel);
        DrawFieldOfView();

    }
    bool ICanSee(Transform player)
    {
        Vector3 direction = Player.position - this.transform.position;
        float angle = Vector3.Angle(direction, this.transform.forward);
        RaycastHit hit;
 

        if (
            Physics.Raycast(this.transform.position, direction, out hit) && // je peux lancer rayon vers le Player ?
            hit.collider.gameObject.tag == "Player" &&// la collision est avec le player ?
            direction.magnitude < fovDistance &&// Le player est assez proche pour etre vu ?
            angle < fovAngle // Le palyer est dans mon champ de vision ?

            )
        {

            return true;// je vois le Player !

        }


        return false; // je le vois pas 
        
    }
    void DrawFieldOfView()
    {
        Vector3 origin = transform.position;
        float halfFOV = fovAngle / 2.0f;
        Quaternion leftRayRotation = Quaternion.AngleAxis(-halfFOV, Vector3.up);
        Quaternion rightRayRotation = Quaternion.AngleAxis(halfFOV, Vector3.up);
        Vector3 leftRayDirection = leftRayRotation * transform.forward;
        Vector3 rightRayDirection = rightRayRotation * transform.forward;

        Debug.DrawRay(origin, leftRayDirection * fovDistance, Color.green);
        Debug.DrawRay(origin, rightRayDirection * fovDistance, Color.green);

        Debug.DrawLine(origin, origin + leftRayDirection * fovDistance, Color.green);
        Debug.DrawLine(origin, origin + rightRayDirection * fovDistance, Color.green);

        
        float currentAngle = -halfFOV;
        float step = fovAngle / 10.0f;
        for (int i = 0; i < 10; i++)
        {
            Quaternion rotation = Quaternion.AngleAxis(currentAngle, Vector3.up);
            Vector3 direction = rotation * transform.forward;
            Debug.DrawRay(origin, direction * fovDistance, Color.green);
            currentAngle += step;
        }
    }

    void Poursuivre(Transform Player)
    {
        this.GetComponent<UnityEngine.AI.NavMeshAgent>().isStopped = true;
        this.GetComponent<UnityEngine.AI.NavMeshAgent>().ResetPath();
        Vector3 direction = Player.position - this.transform.position;
        this.transform.rotation = Quaternion.Slerp(this.transform.rotation, Quaternion.
         LookRotation(direction), Time.deltaTime * this.vitesseRot_poursuite);

        if (direction.magnitude > this.precision_poursuite)
        {
            this.transform.Translate(0, 0, Time.deltaTime * this.vitesse_poursuite);
            //ici chisir le bon emplacement sur votre map !
        }

    }

    void Chercher()
    {
        //le garde doit aller au dernier endroit ou le player a été vu et 
        //il doit patrouiller à cet endroit
        if (transform.position == dernierEmplacementVu)
        {
            etatActuel = Etats.Patrouiller;
        }
        else
        {
            this.GetComponent<UnityEngine.AI.NavMeshAgent>().SetDestination(dernierEmplacementVu);
            Debug.Log("Etat du Garde: " + etatActuel + " point " + dernierEmplacementVu);
        }

    }

    void Patrouiller()
    {
        timing_patrouille += Time.deltaTime;
        if (timing_patrouille > attente_patrouille)
        {
            timing_patrouille = 0.0f;
            Vector3 pointDePatrouille = dernierEmplacementVu;

            //on genere un emplacement aleatoir à partir du dernier emplacement Vu
            float alleatoir = Random.Range(-distance_patrouille, distance_patrouille);
            pointDePatrouille += new Vector3(alleatoir, 0, alleatoir);

            //et maintenant on y va..
            this.GetComponent<UnityEngine.AI.NavMeshAgent>().SetDestination(pointDePatrouille);
        }
    }




}