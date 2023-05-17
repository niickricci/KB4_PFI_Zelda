// NOM: Nicolas Ricci
// DATE: 15/05/23

using SFML.Graphics;
using SFML.Window;
using SFML.System;
using System.Numerics;
using static KB4_PFI_Zelda.Program;
using System.Net;
using System.Runtime.Intrinsics.X86;
using System;
using System.Reflection.Metadata;
using System.Transactions;
using SFML.Audio;
using System.Xml.Linq;
using System.Linq.Expressions;
using System.IO.Enumeration;

namespace KB4_PFI_Zelda;
class Program
{
    public interface IApprochable
    {
        //Contient une propriété Pos (la partie get seulement) qui retourne la position. 
        Vector2f Pos { get; }
        bool EstProche(IApprochable autreObjet);
    }

    public interface IAttaquable
    {
        int SubirAttaque(Personnage attaquant);
    }

    //La classe static Utilitaire
    public static class Utilitaire
    {

        //Méthode Distance
        //La méthode Distance accepte en paramètre deux points de type Vector2f et retourne la distance qui les séparent calculée de la façon suivante : √((p_0.X-p_1.X)^2+(p_0.Y-p_1.Y)^2 )
        public static double Distance(Vector2f point1, Vector2f point2)
        {
            return Math.Sqrt(Math.Pow((point1.X - point2.X), 2) + Math.Pow((point1.Y - point2.Y), 2));
        }

        //Méthode GénérerNombre
        //La méthode GénérerNombre retourne un nombre tiré au hasard entre deux bornes inclusives passées en paramètre.Gérez votre objet Random correctement.
        public static int GenererNombre(int nbr1, int nbr2)
        {
            Random random = new Random();
            int nbGenerer = random.Next(nbr1, nbr2 + 1);

            return nbGenerer;
        }


    }



    //La classe abstraite Personnage
    public abstract class Personnage : Animation, IApprochable, IAttaquable
    {
        private string nom = "";
        private int nbPointsVie;
        private int nbPointsAttaque;
        private int nbPointsDefense;

        public Vector2f Pos
        {
            get { return Position; }
            set { Position = value; }
        }
        public string Nom
        {
            get { return nom; }
            protected set { nom = value; }
        }
        public int NbPointsVie
        {
            get { return nbPointsVie; }
            protected set { nbPointsVie = value; }
        }
        public int NbPointsAttaque
        {
            get { return nbPointsAttaque; }
            protected set { nbPointsAttaque = value; }
        }
        public int NbPointsDefense
        {
            get { return nbPointsDefense; }
            protected set { nbPointsDefense = value; }
        }


        public Personnage(Texture texture, Vector2f pos, string nom, int nbPointsVie, int nbPointsAttaque, int nbPointsDefense) : base(texture, pos)
        {
            Nom = nom;
            NbPointsVie = nbPointsVie;
            NbPointsAttaque = nbPointsAttaque;
            NbPointsDefense = nbPointsDefense;
        }

        public bool EstProche(IApprochable autreObjet)
        {
            const float distanceMax = 10f;
            double distance = Utilitaire.Distance(Pos, autreObjet.Pos);

            //Vrai si X & Y sont plus bas que 10f
            return distance <= distanceMax ? true : false;
        }

        //Méthode abstraite Déplacer
        public abstract bool Deplacer(Carte carte);

        //Méthode Prendre
        public bool Prendre(Item item)
        {
            //-	Vérifier si l’item passé en paramètre est assez proche du personnage pour être pris (voir l’interface IApprochable);
            if (!EstProche(item))
            {
                return false;
            }
            //-Si l’item est assez proche, il est ramassé et les caractéristiques de l’item(GainVie, GainAttaque et GainDefense) sont ajoutées aux caractéristiques correspondantes du personnage;
            NbPointsVie += item.GainPointsVie;
            NbPointsAttaque += item.GainPointsAttaque;
            NbPointsDefense += item.GainPointsDefense;

            //-Un message contenant les nouvelles caractéristiques du personnage ainsi que les propriétés de l’item ramassé est affiché à la console;
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"{item.Nom} rammassé avec succès!");
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine($"Description du {item.Nom}:");
            Console.ResetColor();
            Console.WriteLine(item.ToString());
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine($"Description de {Nom}:");
            Console.ResetColor();
            Console.WriteLine(ToString());
            Console.WriteLine();
            Console.WriteLine();

            //-La méthode retourne false si l’item a été ramassé et true dans le cas contraire;
            //-Un item qui a été ramassé ne sera plus affiché sur la carte(false = n’existe plus!);
            return true;


        }

        //Méthode Attaquer
        public bool Attaquer(Personnage ennemie)
        {

            //Vérifier si l’ennemi est assez proche pour être attaqué(voir l’interface IApprochable);
            if (EstProche(ennemie))
            {

                //-	Lancé un dé à 20 faces (un nombre aléatoire entre 1 et 20) pour déterminer si l’attaque aura lieu;
                int lancerDe = Utilitaire.GenererNombre(1, 20);

                if (lancerDe >= ennemie.NbPointsDefense)
                {
                    // L'attaque a lieu
                    int degats = NbPointsAttaque - ennemie.nbPointsDefense;
                    ennemie.SubirAttaque(this);
                    bool estVivant = ennemie.NbPointsVie > 0;

                    // Afficher les détails de l'attaque et les caractéristiques finales de l'attaquant et de l'ennemi
                    Console.ForegroundColor = ConsoleColor.Cyan;
                    Console.WriteLine($"Attaque enclenché:");
                    Console.ResetColor();
                    Console.WriteLine($"L'attaquant {Nom} a lancé un dé de {lancerDe} pour attaquer l'ennemi {ennemie.Nom}.");
                    Console.WriteLine($"L'attaque réussit, infligeant {degats} points de dégâts à l'ennemi {ennemie.Nom}.");
                    Console.WriteLine($"Les caractéristiques finales de l'attaquant {Nom} sont: points de vie = {NbPointsVie}, points d'attaque = {NbPointsAttaque}, points de défense = {NbPointsDefense}.");
                    Console.WriteLine($"Les caractéristiques finales de l'ennemi {ennemie.Nom} sont : points de vie = {ennemie.NbPointsVie}, points d'attaque = {ennemie.NbPointsAttaque}, points de défense = {ennemie.NbPointsDefense}.");
                    Console.WriteLine();

                    if (!estVivant)
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine($"L'ennemi {ennemie.Nom} est mort!");
                        Console.ResetColor();
                        Console.WriteLine();
                        Console.WriteLine();
                        return false;
                    }
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine($"L'ennemi {ennemie.Nom} est encore vivant!");
                    Console.ResetColor();
                    Console.WriteLine();
                    Console.WriteLine();
                    return true;

                }
                else
                {
                    // L'attaque echoue
                    Console.WriteLine($"L'attaquant {Nom} a lancé un dé de {lancerDe} pour attaquer l'ennemi {ennemie.Nom}.");
                    Console.WriteLine("L'attaque a échoué.");
                    Console.WriteLine();
                    Console.WriteLine();

                    return true;
                }
            }
            return true;
        }

        //Méthode abstraite SubirAttaque
        public abstract int SubirAttaque(Personnage attaquant);

        //Méthode ToString
        public override string ToString()
        {
            return $"Nom : {Nom}\nPoints de vie : {NbPointsVie}\nPoints de défense : {NbPointsDefense}\nPoints d'attaque : {NbPointsAttaque}\nPosition : {Pos}";
        }
    }


    //La classe Heros
    public class Heros : Personnage
    {
        //Statistiques de départ const
        private const int nbPointsVieDepart = 10;
        private const int nbPointsAttaqueDepart = 2;
        private const int nbPointsDefenseDepart = 10;
        public Heros(Texture texture, Vector2f pos, string nom) : base(texture, pos, nom, nbPointsVieDepart, nbPointsAttaqueDepart, nbPointsDefenseDepart) { }

        public override bool Deplacer(Carte carte)
        {
            Vector2f positionTemp = Pos;

            //Haut
            if (Keyboard.IsKeyPressed(Keyboard.Key.Up) || (Keyboard.IsKeyPressed(Keyboard.Key.W)))
            {
                positionTemp = new Vector2f(Position.X, Position.Y - 3);
                Dir = Direction.Haut;
            }


            //Bas
            if (Keyboard.IsKeyPressed(Keyboard.Key.Down) || (Keyboard.IsKeyPressed(Keyboard.Key.S)))
            {
                positionTemp = new Vector2f(Position.X, Position.Y + 3);
                Dir = Direction.Bas;
            }


            //Gauche
            if (Keyboard.IsKeyPressed(Keyboard.Key.Left) || (Keyboard.IsKeyPressed(Keyboard.Key.A)))
            {
                positionTemp = new Vector2f(Position.X - 3, Position.Y);
                Dir = Direction.Gauche;
            }


            //Droite
            if (Keyboard.IsKeyPressed(Keyboard.Key.Right) || (Keyboard.IsKeyPressed(Keyboard.Key.D)))
            {
                positionTemp = new Vector2f(Position.X + 3, Position.Y);
                Dir = Direction.Droite;
            }


            //Verifier deplacement
            if (carte.EstPositionValide(positionTemp))
            {
                Pos = positionTemp;
            }

            //Quitter?
            if (Keyboard.IsKeyPressed(Keyboard.Key.Escape))
            {
                return true;
            }
            return false;
        }


        //Méthode SubirAttaque
        public override int SubirAttaque(Personnage attaquant)
        {
            //Dans le cas du héros, le nombre de points de dommages est déterminé au hasard en tirant un nombre entre 1
            //et la capacité d’attaque de l’attaquant (propriété A). 
            int nbPointDommageSubit = Utilitaire.GenererNombre(1, attaquant.NbPointsAttaque);


            //Les points de dommage sont soustraits du nombre de points de vie (V) de l’objet Héros.
            NbPointsVie -= nbPointDommageSubit;


            //Par contre, si l’attaque est perpétrée par un monstre de type Squelette, la défense du héros est également diminuée de la moitié des points de dommage reçus,
            //car les griffes acérées du Squelette endommagent l’armure du Héros.
            if (attaquant is Squelette)
            {
                int nbPointDommageDef = nbPointDommageSubit / 2;
                NbPointsDefense -= nbPointDommageDef;
            }

            //La méthode retourne les points de dommage reçus.
            return nbPointDommageSubit;
        }
    }


    //La classe abstraite Monstre
    public abstract class Monstre : Personnage
    {
        //propriété Vitesse 
        public int Vitesse { get; protected set; }

        protected Monstre(Texture texture, Vector2f pos, string nom, int nbPointsVie, int nbPointsAttaque, int nbPointsDefense, int vitesse) : base(texture, pos, nom, nbPointsVie, nbPointsAttaque, nbPointsDefense)
        {
            Vitesse = vitesse;
        }

        //Méthode Déplacer Monstre
        public override bool Deplacer(Carte carte)
        {
            Vector2f positionTemp;
            Vector2f dernierePos = Pos;
            bool positionValide;

            //Haut
            if (Dir == Direction.Haut)
            {
                positionTemp = new Vector2f(Position.X, Position.Y - Vitesse);
                positionValide = carte.EstPositionValide(positionTemp);

                if (positionValide && positionTemp != dernierePos)
                {
                    Pos = positionTemp;
                    dernierePos = positionTemp;
                }

                if (!positionValide)
                {
                    NouvellePositionAleatoire();
                }
            }
            //Bas
            else if (Dir == Direction.Bas)
            {
                positionTemp = new Vector2f(Position.X, Position.Y + Vitesse);
                positionValide = carte.EstPositionValide(positionTemp);

                if (positionValide && positionTemp != dernierePos)
                {
                    Pos = positionTemp;
                    dernierePos = positionTemp;
                }

                if (!positionValide)
                {
                    NouvellePositionAleatoire();
                }
            }
            //Gauche
            else if (Dir == Direction.Gauche)
            {
                positionTemp = new Vector2f(Position.X - Vitesse, Position.Y);
                positionValide = carte.EstPositionValide(positionTemp);

                if (positionValide && positionTemp != dernierePos)
                {
                    Pos = positionTemp;
                    dernierePos = positionTemp;
                }

                if (!positionValide)
                {
                    NouvellePositionAleatoire();
                }
            }
            //Droit
            else if (Dir == Direction.Droite)
            {
                positionTemp = new Vector2f(Position.X + Vitesse, Position.Y);
                positionValide = carte.EstPositionValide(positionTemp);

                if (positionValide && positionTemp != dernierePos)
                {
                    Pos = positionTemp;
                    dernierePos = positionTemp;
                }

                if (!positionValide)
                {
                    NouvellePositionAleatoire();
                }
            }
            return false;
        }

        private void NouvellePositionAleatoire()
        {

            int directionAleatoire = Utilitaire.GenererNombre(0, 3);

            switch (directionAleatoire)
            {
                case 0: //haut
                    Dir = Direction.Haut;
                    break;
                case 1: //bas
                    Dir = Direction.Bas;
                    break;
                case 2: //gauche
                    Dir = Direction.Gauche;
                    break;
                case 3: //droite
                    Dir = Direction.Droite;
                    break;
            }
        }
    }

    //La classe Zombie
    public class Zombie : Monstre
    {
        //Statistiques de départ
        private const string nom = "Zombie";
        private const int nbPointsVieDepart = 80;
        private const int nbPointsAttaqueDepart = 12;
        private const int nbPointsDefenseDepart = 5;
        private const int vitesseDepart = 2;

        public Zombie(Texture texture, Vector2f pos) : base(texture, pos, nom, nbPointsVieDepart, nbPointsAttaqueDepart, nbPointsDefenseDepart, vitesseDepart) { }

        //Méthode SubirAttaque
        public override int SubirAttaque(Personnage attaquant)
        {

            //Dans le cas d’un monstre de type Zombie, le nombre de points de dommage est égal au nombre de points d’attaque de l’attaquant 
            int nbPointDommageSubit = attaquant.NbPointsAttaque;

            //Les points de dommage sont soustraits du nombre de points de vie (V) de l’objet Héros.
            NbPointsVie -= nbPointDommageSubit;

            //Cependant, toutes les fois qu’il est attaqué, la défense d’un monstre zombie augmente d’un nombre de points déterminé au hasard
            //en tirant un nombre entre 1 et la capacité d’attaque de l’attaquant (propriété A). Ce gain en défense est ajouté aux points de défense du
            int augmentationDef = Utilitaire.GenererNombre(1, attaquant.NbPointsAttaque);
            NbPointsDefense += augmentationDef;

            //La méthode retourne les points de dommage reçus.
            return nbPointDommageSubit;
        }
    }


    //La classe Dino
    public class Dino : Monstre
    {
        //Statistiques de départ
        private const string nom = "Dino";
        private const int nbPointsVieDepart = 150;
        private const int nbPointsAttaqueDepart = 10;
        private const int nbPointsDefenseDepart = 15;
        private const int vitesseDepart = 3;

        public Dino(Texture texture, Vector2f pos) : base(texture, pos, nom, nbPointsVieDepart, nbPointsAttaqueDepart, nbPointsDefenseDepart, vitesseDepart) { }

        //Méthode SubirAttaque
        public override int SubirAttaque(Personnage attaquant)
        {

            //Dans le cas d’un monstre de type Dino, le nombre de points de dommages est déterminé au hasard en tirant un nombre entre 1 et
            //la capacité d’attaque de l’attaquant (propriété A). 
            int nbPointDommageSubit = Utilitaire.GenererNombre(1, attaquant.NbPointsAttaque);

            //Les points de dommage sont soustraits du nombre de points de vie (V) du monstre.
            NbPointsVie -= nbPointDommageSubit;

            //Par contre, toutes les fois qu’il est attaqué, la défense d’un monstre Dino diminue du même nombre de points de dommage infligé à sa vie.
            NbPointsDefense -= nbPointDommageSubit;

            //La méthode retourne les points de dommage infligés au monstre.
            return nbPointDommageSubit;
        }
    }

    //La classe Squelette
    public class Squelette : Monstre
    {
        //Statistiques de départ
        private const string nom = "Squelette";
        private const int nbPointsVieDepart = 20;
        private const int nbPointsAttaqueDepart = 8;
        private const int nbPointsDefenseDepart = 15;
        private const int vitesseDepart = 4;

        public Squelette(Texture texture, Vector2f pos) : base(texture, pos, nom, nbPointsVieDepart, nbPointsAttaqueDepart, nbPointsDefenseDepart, vitesseDepart) { }

        //Méthode SubirAttaque
        public override int SubirAttaque(Personnage attaquant)
        {

            //Dans le cas d’un monstre de type Squelette, le dommage infligé par le héros est fatal
            int nbPointDommageSubit = NbPointsVie;

            //Les points de dommage sont soustraits du nombre de points de vie (V) du monstre.
            NbPointsVie -= nbPointDommageSubit;

            //La méthode retourne les points de dommage infligés au monstre.
            return nbPointDommageSubit;
        }
    }

    //La classe Item
    public class Item : Sprite, IApprochable
    {
        public string Nom { get; private set; }
        public int GainPointsVie { get; private set; }
        public int GainPointsAttaque { get; private set; }
        public int GainPointsDefense { get; private set; }
        public Vector2f Pos
        {
            get { return Position; }
            private set { Position = value; }
        }

        public Item(Texture texture, Vector2f pos, string nom, int gainPointsVie, int gainPointsAttaque, int gainPointsDefense) : base(texture)
        {
            Nom = nom;
            GainPointsVie = gainPointsVie;
            GainPointsAttaque = gainPointsAttaque;
            GainPointsDefense = gainPointsDefense;
            Pos = pos;
            Origin = new Vector2f(texture.Size.X / 2f, texture.Size.Y / 2f);
        }

        public bool EstProche(IApprochable autreObjet)
        {
            const float distanceMax = 10f;
            double distance = Utilitaire.Distance(Pos, autreObjet.Pos);

            //Vrai si X & Y sont plus bas que 10f
            return distance <= distanceMax ? true : false;
        }

        //Méthode Afficher\
        public void Afficher(RenderWindow fenetre)
        {
            fenetre.Draw(this);
        }

        //Méthode ToString
        public override string ToString()
        {
            return $"Nom : {Nom}\nGain en points de vie : {GainPointsVie}\nGain en points de défense : {GainPointsDefense}\nGain en Points d'attaque : {GainPointsAttaque}\n Position : {Position}";
        }

    }

    //La classe Item Type Pain
    public class Pain : Item
    {
        //Statistiques de départ
        private const string nom = "Pain";
        private const int gainPointsVieDepart = 50;
        private const int gainPointsAttaqueDepart = 10;
        private const int gainPointsDefenseDepart = 15;

        public Pain(Texture texture, Vector2f pos) : base(texture, pos, nom, gainPointsVieDepart, gainPointsAttaqueDepart, gainPointsDefenseDepart)
        {
            Origin = new Vector2f(texture.Size.X / 2f, texture.Size.Y / 2f);
        }
    }
    //La classe Item Type Glaive
    public class Glaive : Item
    {
        //Statistiques de départ
        private const string nom = "Glaive";
        private const int gainPointsVieDepart = 50;
        private const int gainPointsAttaqueDepart = 10;
        private const int gainPointsDefenseDepart = 15;

        public Glaive(Texture texture, Vector2f pos) : base(texture, pos, nom, gainPointsVieDepart, gainPointsAttaqueDepart, gainPointsDefenseDepart)
        {
            Origin = new Vector2f(texture.Size.X / 2f, texture.Size.Y / 2f);
        }
    }

    const int Largeur = 640;
    const int Hauteur = 480;
    const int PosiInitHérosX = 848;
    const int PosiInitHérosY = 1989;
    const int NbMonstres = 100;
    const int NbItems = 100;

    enum SorteDeMonstres { Dino, Squelette, Zombie, MAX_SORTE };


    static void Main(string[] args)
    {
        bool bQuitter = false; // Vrai s'il faut continuer la partie
        bool bLink = true;    // Vrai si le héros est vivant


        //*****Font*****
        Font fontRetroGaming = new Font("Fonts/retro_gaming.ttf");

        //*****Menu********
        //background
        Texture menuTexture = new Texture("Images/zelda_menu.png");
        Sprite menuSprite = new Sprite(menuTexture);
        menuSprite.Position = new Vector2f(0, 0);
        //Titre
        Text titreMenu = new Text("Zelda | PFI KB4", fontRetroGaming, 50);
        titreMenu.Color = Color.Green;
        titreMenu.Position = new Vector2f(90, 50);
        //Credit
        Text creditMenu = new Text("Nicolas Ricci - Hiver 2023", fontRetroGaming, 15);
        creditMenu.Color = Color.Green;
        creditMenu.Position = new Vector2f(210, 400);
        //Jouer
        Text jouerMenu = new Text("Jouer", fontRetroGaming, 40);
        jouerMenu.Position = new Vector2f(100, 225);
        //Quitter
        Text quitterMenu = new Text("Quitter", fontRetroGaming, 40);
        quitterMenu.Position = new Vector2f(100, 275);


        //*****Music*****
        Music musicMenu = new Music("Sounds/zelda_menu_music.wav");
        Music musicJeu = new Music("Sounds/zelda_theme_song.wav");
        Music musicPrendreItem = new Music("Sounds/zelda_item_catch.wav");
        Music musicHeroMort = new Music("Sounds/zelda_death_sound.wav");
        Music musicMonstreMort = new Music("Sounds/zelda_kill_sound.wav");
        //Parametre Music
        musicMenu.Loop = true;
        musicMenu.Volume = 40;

        musicJeu.Loop = true;
        musicJeu.Volume = 50;

        musicPrendreItem.Loop = false;
        musicPrendreItem.Volume = 35;

        musicHeroMort.Loop = false;
        musicHeroMort.Volume = 50;

        musicMonstreMort.Loop = false;
        musicMonstreMort.Volume = 80;

        //Design ASCII "Zelda" pour console:
        string titre = @"
                                    /@
                     __        __   /\/
                    /==\      /  \_/\/   
                  /======\    \/\__ \__
                /==/\  /\==\    /\_|__ \
             /==/    ||    \=\ / / / /_/
           /=/    /\ || /\   \=\/ /     
        /===/   /   \||/   \   \===\
      /===/   /_________________ \===\
   /====/   / |                /  \====\
 /====/   /   |  _________    /  \   \===\    THE LEGEND OF 
 /==/   /     | /   /  \ / / /  __________\_____      ______       ___
|===| /       |/   /____/ / /   \   _____ |\   /      \   _ \      \  \
 \==\             /\   / / /     | |  /= \| | |        | | \ \     / _ \
 \===\__    \    /  \ / / /   /  | | /===/  | |        | |  \ \   / / \ \
   \==\ \    \\ /____/   /_\ //  | |_____/| | |        | |   | | / /___\ \
   \===\ \   \\\\\\\/   /////// /|  _____ | | |        | |   | | |  ___  |
     \==\/     \\\\/ / //////   \| |/==/ \| | |        | |   | | | /   \ |
     \==\     _ \\/ / /////    _ | |==/     | |        | |  / /  | |   | |
       \==\  / \ / / ///      /|\| |_____/| | |_____/| | |_/ /   | |   | |
       \==\ /   / / /________/ |/_________|/_________|/_____/   /___\ /___\
         \==\  /               | /==/
         \=\  /________________|/=/       KB4 PFI - Nicolas Ricci
           \==\     _____     /==/              Hiver 2023
          / \===\   \   /   /===/
         / / /\===\  \_/  /===/
        / / /   \====\ /====/
       / / /      \===|===/
       |/_/         \===/
                      =                                                       ";


        //Position initial Heros
        Vector2f posInitHero;
        posInitHero.X = PosiInitHérosX;
        posInitHero.Y = PosiInitHérosY;


        // Création de la fenêtre
        VideoMode mode = new VideoMode(Largeur, Hauteur);
        RenderWindow fenetre = new RenderWindow(mode, "PFI Zelda - Hiver 2023 | Nicolas Ricci");
        fenetre.Closed += (s, a) => fenetre.Close(); // Ferme la fenêtre sur l'évènement Closed



        // Création de la carte du monde
        Texture mondeTexture = new Texture("images/lemonde.bmp");
        Image masqueTexture = new Image("images/MasqueDuMonde.bmp");
        Carte monde = new Carte(mondeTexture, masqueTexture);
        Vector2i cs = (Vector2i)mondeTexture.Size; // dimensions de la carte du monde



        Texture linkText = new Texture("images/link.png");
        // Compléter la création du personnage héros "link"

        // Début de la création du héros
        Heros link = new Heros(linkText, posInitHero, "Link");
        Vector2i ls = (Vector2i)linkText.Size;
        // Fin de la création du héros


        // Déclarer une liste de Monstres appelée "monstres"
        List<Monstre> monstres = new List<Monstre>(NbMonstres);

        // Chargement des textures des monstres
        Texture dinoText = new Texture("images/dino.png");
        Texture skelText = new Texture("images/skel.png");
        Texture zombieText = new Texture("images/zombie.png");
        // Dimension des textures des monstres, ça pourra vous servir
        Vector2i ds = (Vector2i)dinoText.Size;
        Vector2i ss = (Vector2i)skelText.Size;
        Vector2i zs = (Vector2i)zombieText.Size;
        // Compléter la création des monstres
        // Début de la création des monstres
        for (int i = 0; i < monstres.Capacity; i++) // Boucle pour créer les monstres
        {

            // Positionner les monstres au hasard sur la carte
            // dont les dimensions sont monde.Hauteur par monde.Largeur.
            //Vector2f positionMonstre = new Vector2f(Utilitaire.GenererNombre(0, (int)monde.Largeur), Utilitaire.GenererNombre(0, (int)monde.Hauteur));

            // Vérifier que le monstre est dans une position valide de la carte avant de le placer.
            Vector2f positionMonstre;
            do
            {
                positionMonstre = new Vector2f(Utilitaire.GenererNombre(0, (int)monde.Largeur), Utilitaire.GenererNombre(0, (int)monde.Hauteur));
            }
            while (!monde.EstPositionValide(positionMonstre));


            // Générer au hasard soit des Dino, des Zombie ou des Squelette
            int typeMonstre = Utilitaire.GenererNombre(1, 3);

            Monstre monstre;
            if (typeMonstre == 1)
            {
                monstre = new Dino(dinoText, positionMonstre);
            }
            else if (typeMonstre == 2)
            {
                monstre = new Zombie(zombieText, positionMonstre);
            }
            else if (typeMonstre == 3)
            {
                monstre = new Squelette(skelText, positionMonstre);
            }
            else
            {
                throw new Exception("Erreur: type de monstre invalide.");
            }

            monstres.Add(monstre); //Ajout du monstre a la liste
        }
        // Fin de la création de la création des monstres



        // Déclarer une liste d'Items appelé "items"
        List<Item> items = new List<Item>(NbItems);

        // Chargement des textures des items
        Texture painTexture = new Texture("images/pain.png");
        Texture glaiveTexture = new Texture("images/epee.png");
        // Compléter la création des items (pains et épées)
        Vector2i ps = (Vector2i)painTexture.Size;
        Vector2i gs = (Vector2i)glaiveTexture.Size;
        // Générer au hasard soit des « pain » ou des « épée »
        // Vérifier que l'item est dans une position valide de la carte avant de le placer.
        // Début de la création des items

        for (int i = 0; i < items.Capacity; i++) // Boucle pour créer les items
        {

            // Positionner les items au hasard sur la carte
            // dont les dimensions sont monde.Hauteur par monde.Largeur.
            //Vector2f positionItems = new Vector2f(Utilitaire.GenererNombre(0, (int)monde.Largeur), Utilitaire.GenererNombre(0, (int)monde.Hauteur));

            Vector2f positionItems;
            do
            {
                positionItems = new Vector2f(Utilitaire.GenererNombre(0, (int)monde.Largeur), Utilitaire.GenererNombre(0, (int)monde.Hauteur));
            }
            while (!monde.EstPositionValide(positionItems));


            // Générer au hasard soit des « pain » ou des « épée »
            int typeItem = Utilitaire.GenererNombre(1, 2);

            Item item;
            if (typeItem == 1)
            {
                item = new Pain(painTexture, positionItems);
            }
            else if (typeItem == 2)
            {
                item = new Glaive(glaiveTexture, positionItems);
            }
            else
            {
                throw new Exception("Erreur: type d'item invalide.");
            }

            items.Add(item); //Ajout du monstre a la liste
        }


        // Fin de la création de la création des items
        //Jouer theme (Music)
        musicMenu.Play();

        RectangleShape textStatsBackground = new RectangleShape();

        Text textStats = new Text($"{link.Nom}\nVie: {link.NbPointsVie}\nAttaque: {link.NbPointsAttaque}\nDefense: {link.NbPointsDefense}\nPosition X: {link.Pos.X}\nPosition Y: {link.Pos.Y}", fontRetroGaming, 10);
        textStats.FillColor = Color.White;
        textStatsBackground.Size = new Vector2f(105, 80);
        textStatsBackground.FillColor = new Color(0, 0, 0, 120);

        Console.WriteLine(titre); //Afficher titre Console

        do
        {
            //Afficher menu:
            Console.Title = $"PFI Zelda | Nicolas Ricci - Hiver 2023";
            fenetre.Draw(menuSprite);
            fenetre.Draw(titreMenu);
            fenetre.Draw(jouerMenu);
            fenetre.Draw(quitterMenu);
            fenetre.Draw(creditMenu);

            if (jouerMenu.GetGlobalBounds().Contains(Mouse.GetPosition(fenetre).X, Mouse.GetPosition(fenetre).Y))  //Si le curseur est sur jouerMenu:
            {
                jouerMenu.Color = Color.Yellow; //jouerMenu devient jaune

                if (Mouse.IsButtonPressed(Mouse.Button.Left)) // Si jouerMenu est clique: Debut du jeu -->
                {
                    musicMenu.Stop();
                    musicJeu.Play();
                    // Boucle principale du jeu
                    do
                    {
                        // Déplacer les personnages (héros, puis monstres)


                        // Début
                        bQuitter = link.Deplacer(monde);

                        try
                        {
                            foreach (Monstre monstre in monstres)
                            {
                                monstre.Deplacer(monde);
                            }
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine(ex.Message);
                        }
                        // Fin 


                        // Essayer de prendre les items (le héros essaie de prendre tous les items du monde)
                        // Début 
                        for (int i = 0; i < items.Count; i++)
                        {
                            bool itemPris = link.Prendre(items[i]);
                            if (itemPris)
                            {
                                musicPrendreItem.Stop();
                                musicPrendreItem.Play();
                                items.Remove(items[i]);
                            }
                        }
                        // Fin 


                        // Essayer d'attaquer (le héros essaie d'attaquer tous les monstres du monde, puis si le monstre
                        // n'a pas été tué, il tente d'attaquer le héros en retour)
                        // Début 
                        for (int i = 0; i < monstres.Count; i++)
                        {
                            bool etatMonstre = link.Attaquer(monstres[i]);
                            if (!etatMonstre)
                            {
                                musicMonstreMort.Stop();
                                musicMonstreMort.Play();
                                monstres.Remove(monstres[i]);
                            }
                            bool etatHeros = monstres[i].Attaquer(link);
                            if (!etatHeros)
                            {
                                musicJeu.Stop();
                                musicHeroMort.Play();
                                Console.WriteLine("Vous etes mort!");
                                bLink = false;
                            }
                        }
                        // Fin 


                        // Affichage du monde
                        View Vue = fenetre.GetView();
                        //>>>>>>>>>>>> DÉCOMMENTER LA LIGNE CI - DESSOUS QUAND LE HÉROS "link' SERA INSTANCIÉ.


                        Vue.Center = link.Pos;


                        //<<<<<<<<<<<<<<<<<<<<<<
                        fenetre.SetView(Vue);
                        monde.Afficher(fenetre);


                        // Afficher les monstres, les items et le héros
                        // Début
                        foreach (Monstre monstre in monstres)
                        {
                            monstre.Afficher(fenetre);
                        }

                        foreach (Item item in items)
                        {
                            item.Afficher(fenetre);
                        }

                        link.Afficher(fenetre);
                        // Fin 


                        //Afficher Stats:
                        //Debut
                        textStats.Position = new Vector2f(link.Pos.X - 320, link.Pos.Y + 160);
                        textStats.DisplayedString = $"{link.Nom}\nVie: {link.NbPointsVie}\nAttaque: {link.NbPointsAttaque}\nDefense: {link.NbPointsDefense}\nPosition X: {link.Pos.X}\nPosition Y: {link.Pos.Y}";
                        Console.Title = $"PFI Zelda 2023 - Nicolas Ricci    |    Hero: {link.Nom} | Vie: {link.NbPointsVie} | Attaque: {link.NbPointsAttaque} | Defense: {link.NbPointsDefense} | X: {link.Pos.X} | Y: {link.Pos.Y}";

                        textStatsBackground.Position = textStats.Position;
                        fenetre.Draw(textStatsBackground);
                        fenetre.Draw(textStats);
                        //Fin


                        // Rafraichir la fenêtre
                        fenetre.Display();
                        // Attendre 30 millisecondes - à ajuster selon la vitesse voulue	
                        Thread.Sleep(30);
                        // Traiter les évènements de la fenêtre
                        fenetre.DispatchEvents();


                    } while (!bQuitter && bLink && fenetre.IsOpen);
                    musicJeu.Stop();

                    if (!bLink || bQuitter)
                    {
                        fenetre.Close();
                        Console.WriteLine("Faites Enter pour quitter");
                        Console.ReadLine();
                    }
                }
            }
            else //Si le curseur est pas sur jouerMenu:
            {
                jouerMenu.Color = Color.White; //jouerMenu reste blanc
            }

            if (quitterMenu.GetGlobalBounds().Contains(Mouse.GetPosition(fenetre).X, Mouse.GetPosition(fenetre).Y))   //Si le curseur est sur quitterMenu:
            {
                quitterMenu.Color = Color.Yellow;  //quitterMenu reste blanc

                if (Mouse.IsButtonPressed(Mouse.Button.Left)) //Si quitterMenu est clique: Fermer fenetre -->
                {
                    musicMenu.Stop();
                    fenetre.Close();
                    Console.WriteLine("Faites Enter pour quitter");
                    Console.ReadLine();
                }
            }
            else //Si le curseur est pas sur quitterMenu:
            {
                quitterMenu.Color = Color.White; //quitteerMenu reste blanc
            }

            // Rafraichir la fenêtre
            fenetre.Display();
            // Attendre 30 millisecondes - à ajuster selon la vitesse voulue	
            Thread.Sleep(30);
            // Traiter les évènements de la fenêtre
            fenetre.DispatchEvents();
        }
        while (fenetre.IsOpen);
    }
}