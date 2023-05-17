////////////////////////////////////////////////////////////
// Animation.h
// 
// Déclaration de la classe Animation qui représente
// et permet de manipuler une animation
//
// Joan-Sébastien Morales 
// Création: 28 avril 2010
// Version 1.0
// Version 1.1 - Bloquer la copie
// Version 2.0 - CAnimation - Dérive de Sprite - 5 mai 2014
// Version 3.0 - Traduction en C# - 29 avril 2020
////////////////////////////////////////////////////////////

//--------------------------------------------------------//
// Animation
//--------------------------------------------------------//
using SFML.Graphics;
using SFML.System;

namespace KB4_PFI_Zelda;

class Animation : Sprite
{
    // Nombre d'images dans l'animation
    private int nbImages;
    // Vitesse de l'animation 
    private int vitesse;
    // Numéro de l'image
    private int compteur = 0;
    // Hauteur en pixels d'une image
    private int imageHauteur;
    // Largeur en pixels d'une image
    private int imageLargeur;
    // Position précédente
    private Vector2f anciennePosition;

    ////////////////////////////////////////////////////////////
    // Animation
    // Constructeur
    //
    // Intrants : Texture, position, NbImages par direction et vitesse
    // La texture doit contenir autant de directions que d'images 
    // Exemple: 4x4
    //
    ////////////////////////////////////////////////////////////
    public Animation(Texture texture, Vector2f pos, int images = 4, int vit = 3) : base(texture)
    {
        nbImages = images;
        vitesse = vit;

        Position = pos; // Position héritée de Sprite
        imageLargeur = (int)texture.Size.X / nbImages;
        imageHauteur = (int)texture.Size.Y / nbImages;

        Origin = new Vector2f(imageLargeur / 2, imageHauteur); // Les pieds
    }
    ////////////////////////////////////////////////////////////
    // Afficher
    // Méthode qui affiche l'animation dans
    // la fenêtre. 
    // Intrant: Fenetre dans laquelle il faut afficher  
    //          l'animation
    //
    ////////////////////////////////////////////////////////////
    public void Afficher(RenderWindow fenetre)
    {
        // Si la position est différente, changer d'image
        if (!anciennePosition.Equals(Position))
        {
            compteur++;
            anciennePosition = Position; // référence? - non struct 
        }

        // Calcul du rectangle source
        TextureRect = new IntRect(imageLargeur * (compteur / vitesse % nbImages),
            imageHauteur * (int)Dir,
            imageLargeur,
            imageHauteur);

        // Affichage
        fenetre.Draw(this);
    }
    ////////////////////////////////////////////////////////////
    // Direction courante de l'amimation
    ////////////////////////////////////////////////////////////
    public enum Direction // Pour clarifier le code
    {
        Bas, Gauche, Droite, Haut // cet ordre est important car il est identique à l'ordre des images dans le spriteset
    };
    public Direction Dir { get; protected set; } = Direction.Droite;
}
