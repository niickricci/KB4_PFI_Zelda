////////////////////////////////////////////////////////////

// D�claration de la classe Carte qui repr�sente le monde
// dans lequel �voluent les personnages et les objets
// 
// Joan-S�bastien Morales 
// Cr�ation: 28 avril 2010
// Version 1.0
// Version 1.1 - Bloquer la copie
// Version 2.0 - 9 mai 2014 Adaptation � SFML - Carte d�rive de Sprite
// Version 3.0 - 29 avril 2020 - Traduction en C#
////////////////////////////////////////////////////////////

//--------------------------------------------------------//
// Carte
//--------------------------------------------------------//
using SFML.Graphics;
using SFML.System;

namespace KB4_PFI_Zelda;

class Carte : Sprite
{
    private Image obstruction; // Image d'obstruction
    ////////////////////////////////////////////////////////////
    // Carte
    // Constructeur param�trique
    //
    // Intrants: 
    // Texture: Texture contenant l'image du monde 
    //                  
    // Obstruction: L'image d'obstruction est une version de
    //              l'image du monde o� les pixels
    //              o� les personnages peuvent passer
    //              sont de couleurs noires(RGB 0,0,0)
    ////////////////////////////////////////////////////////////
    public Carte(Texture laTexture, Image obs) : base(laTexture)
    {
        obstruction = obs;
    }
    public uint Hauteur { get => obstruction.Size.Y; }
    public uint Largeur { get => obstruction.Size.X; }
    ////////////////////////////////////////////////////////////
    // Afficher
    // Affiche la carte dans une fen�tre
    //
    // Intrant: Fenetre: Fen�tre dans laquelle la carte doit
    //                    �tre affich�e
    ////////////////////////////////////////////////////////////
    public void Afficher(RenderWindow fenetre)
    {
        fenetre.Draw(this);
    }
    ////////////////////////////////////////////////////////////
    // EstPositionValide
    // Permet d
    // Extrant: Vrai si le personnage peut se trouver � cette
    //          position sur la carte, faux dans le cas 
    //          contraire
    ////////////////////////////////////////////////////////////
    public bool EstPositionValide(Vector2f position)
    {
        if (position.X < 0 || position.X >= obstruction.Size.X ||
            position.Y < 0 || position.Y >= obstruction.Size.Y)
        {
            return false;
            throw new System.Exception("Carte.EstPositionValide: position en dehors de la carte!!");
        }

        return obstruction.GetPixel((uint)position.X, (uint)position.Y) == Color.Black;
    }
}
