using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ControleurPersonnage : MonoBehaviour
{
    [Header("Partie du corps")]
    // Références aux parties du corps
    public GameObject tete;
    public GameObject torse;
    // Ajouter les autres parties que vous gérez....

    [Header("Contrôles")]
    // Touches de contrôle
    // Dans un monde idéal, ces touches sont dans une classe dédiée pour faciliter leur gestion
    public KeyCode avancer = KeyCode.W;
    public KeyCode reculer = KeyCode.S;
    public KeyCode droite = KeyCode.D;
    public KeyCode gauche = KeyCode.A;

    public float sensibiliteSouris = 0.5f;
    // Possibilité d'ajouter un booléen pour gérer l'inversion de l'axe Y

    [Header("Caractéristiques")]
    public float vitesse;
    public float vitesseRotation;
    public float amplitudeCou;  // Angle en degré que l'on peut regarder en haut/en bas

    // Référence vers le rigidbody
    private Rigidbody rigidbodyPersonnage;

    private void Start()
    {
        rigidbodyPersonnage = GetComponent<Rigidbody>();
        // On initialise la rotation à 360 pour que le script qui borne la position du cou fonctionne correctement
        // (la valeur 0 est mal gérée apparement, c'est un peu poche de la part de Unity mais bon ¯\_(ツ)_/¯)
        tete.transform.localRotation = Quaternion.Euler(360f, 0f, 0f);
    }

    // On manipule le Rigidbody, donc il est préférable de le faire dans FixedUpdate
    void FixedUpdate()
    {
        // Dans un mode de prod réel, on veut éviter de faire des méthodes Update trop volumineuse
        // Son rôle devrait être de déclencher des traitements à chaque frame
        // Si le traitement est court, on peut le réaliser à cet endroit
        // Voyez-la un peu comme un Main d'un programme
        DeplacerPersonnage();
    }

    void DeplacerPersonnage()
    {
        // Déplacement du personnage
        if(Input.GetKey(avancer))
        {
            // Le vecteur Forward ici prend la direction dans laquelle le personnage regarde vers l'avant
            rigidbodyPersonnage.transform.position += rigidbodyPersonnage.transform.forward * vitesse * Time.deltaTime;
        }
        if (Input.GetKey(reculer))
        {
            rigidbodyPersonnage.transform.position -= rigidbodyPersonnage.transform.forward * vitesse * Time.deltaTime;
        }
        if (Input.GetKey(droite))
        {
            // Le vecteur Forward ici prend la direction dans laquelle le personnage regarde vers l'avant
            rigidbodyPersonnage.transform.position += rigidbodyPersonnage.transform.right * vitesse * Time.deltaTime;
        }
        if (Input.GetKey(gauche))
        {
            rigidbodyPersonnage.transform.position -= rigidbodyPersonnage.transform.right * vitesse * Time.deltaTime;
        }

        // Attention Axis horizontal et vertical traitent des touches W A S D
        // Utile si vous voulez rendre votre jeu portable sur console !
        // Remarquez trois choses : 
            // Inversion X et Y (mouvement de la souris en Y cause une rotation autour de l'axe des X)
            // Inversion du mouvement en Y (*-1) (trouvez pourquoi !)
            // On tourne le personnage en Y (rotation complète), donc forward reste devant
            // Mais on tourne juste la tête de haut en bas (idéalement le "fusil" devrait suivre...)

        // Tête haut / bas
        float rotationX = Input.GetAxis("Mouse Y");
        tete.transform.Rotate(-1 * rotationX * sensibiliteSouris * Time.deltaTime, 0f, 0f);

        float angleTete = tete.transform.localRotation.eulerAngles.x;
        // On positionne la tête dans l'intervalle demandée
        if (angleTete > amplitudeCou && angleTete < 180f)
        {
            tete.transform.localRotation = Quaternion.Euler(amplitudeCou, 0f, 0f);
        }
        // Les angles négatifs sont traités de façon positive par eulerAngles (toujours sur l'intervalle 0 - 360)
        if (angleTete < 360f - amplitudeCou && angleTete > 180f)
        {
            tete.transform.localRotation = Quaternion.Euler(-amplitudeCou, 0f, 0f);
        }

        // Corps gauche / droite
        float rotationY = Input.GetAxis("Mouse X");
        transform.Rotate(0f, rotationY * vitesseRotation * Time.deltaTime, 0f);
    }
}
