Je d�taille ici l'architecture g�n�rale du projet.

Les sc�nes:
	-Gameroot: Contient le d�marrage du jeu. Ne fait rien, sauf lancer les sc�nes importantes pour le jeu.
	-System: Contient les grands syst�mes de jeu : Clocks / StatedMonoSystem (les autres syst�mes n'ont que peu d'importance et ne seront pas d�taill�s)
	-System_GridScene: les syst�mes pour le jeu de Match3. Ici seul le script LevelSystem est r��llement important.
	-GridScene: Contient la grille de jeu. Seul le script Grid est important.
	-UI_GridScene: Contient de l'ui. La sc�ne n'a pas d'importance.
	-Player: Contient le script Player.

Mise en place:
	-Les sc�nes se regroupent par ScenePackage. Un ScenePackage fonctionne de lui-m�me et r�presente une partie du jeu. 
	Il y en a 2 : celui du GameRoot et GameScene.

Architecture:
	-Toutes les entit�s vraiment importantes d�rivent de StatedMono<T>. Un script que j'ai cr�� pour ce test, qui facilite la gestion des �tats d'une entit�.
	Il y a 3 entit�s qui d�rivent de cette classe: Grid (qui repr�sente la grille de jeu), Player(qui repr�sente le joueur), et Piece (qui r�presente une pi�ce sur la grille).
	-Chaque entit� a un nombre x d'�tats et un enum qui identifie un �tat, que l'on rajoute via Add(Enum, State). Cela facilite l'acc�s pour changer d'�tat via l'enum.
	
	-Les StatedMono<T> ont des fonctions d'entr�e, d'update et de sortie d'�tat pour faciliter leur gestion. 
	le script StatedMonoSystem se chargeant de bien coordonner les entit�s voulant �tre mise � jour.

	-La communication inter-�tat et extra-entit� se fait via le FrameDataBuffer<T>. C'est un script qui rend une donn�e valide pour une et une seule frame.
	Cela me permet de ne pas d�pendre de l'ordre d'ex�cution du type : A->B donne X comme r�sultat mais B->A me donne un �tat diff�rent de l'application.
	A noter qu'ici, il n'est peut-�tre pas n�cessaire d'avoir une si grosse architecture. Mais j'ai voulu pr�venir ce cas. 

	-Le script Piece n'a aucune connaissance de la grille sur laquelle il est plac�. Grid a conscience de Piece, Player lui a conscience de Grid et de Piece. Je d�coupe mes entit�s pour avoir le moins de couplage que possible entre elles.
	Ainsi Grid peut �tre jou� n'importe comment en suivant les r�gles de game design (elle pourrait �tre jou�e par l'IA).
	Ainsi Piece peut �tre plac� sur n'importe quelle grille.
	Player, lui, sait exactement � quoi il joue et avec quoi. 

	-Il n'y a pas de singleton dans mes applications. J'utilise le design pattern du ServiceProvider/ServiceSubscriber. Toute donn�e "static" est r�cup�r�e via ServiceProvider.GetService<T>().


GameDesign:
	-J'ai globalement suivi les r�gles du test. Grid �limine automatiquement les connexions horizontales ou verticales de Piece. L'algorithme part du bas gauche, pour aller vers le haut droit de la grille. 
	-Grid d�tecte sur une m�me case la meilleure connexion de Pieces (verticale ou horizontale) et �limine celle-ci. 
	-Player a le droit de faire un peu mieux que Grid, il peut en effet faire des connexions en croix de Pieces. J'ai mis �a en place pour donner au joueur un peu de reflexion et un "truc en +" par rapport � l'algorithme.

	-Player peut drag and drop, ou selectionner 2 cases � interchanger. C'est lui qui ordonne � Grid de faire le changement.
	-Grid re�oit la demande de changement et accepte ou non de la faire. Ainsi interchanger 2 pi�ces ne cr�ant pas de connexion n'est pas permis par Grid. Mais �a pourrait �tre le cas sans que Player soit affect�.
	-Grid ensuite �limine si possible les Pieces.

	-La premi�re connexion cr��e par le joueur en d�pla�ant une Piece est prise en compte en premier, ind�pendamment de l'algorithme de la grille. 
	J'ai laiss� la possibilit� au joueur de choisir ce qui devait �tre d�truit en premier pour qu'il ait une certaine libert� strat�gique.

Polish:
	-J'ai int�gr� tous les sons. Le sons de combo ne se d�clenchent que dans le cas o� une connexion de plus de 4 Pieces est �limin�e.
	-VFX pour l'�limination des Pieces.
	-Timer pour donner un but au joueur.
	-ScoreSystem: Grid transmet les Pieces �limin�s au ScoreSystem. Celui-ci ajoute +1 de score pour chaque Pieces �limin�s, il ajoute +1 point pour chaque Pieces faisant une connexion de plus de 4 Pieces. 
	Il y a un systeme de combo qui ajoute un multiplicateur � chaque combo.
	Le score s'enregistre sur un fichier via un outil pr�c�demment cr��.
	-Popup de fin de jeu avec score + meilleur score.


En esp�rant que �a vous plaise.
Constantin Benoit.