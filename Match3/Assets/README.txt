Je détaille ici l'architecture générale du projet.

Les scènes:
	-Gameroot: Contient le démarrage du jeu. Ne fait rien, sauf lancer les scènes importantes pour le jeu.
	-System: Contient les grands systèmes de jeu : Clocks / StatedMonoSystem (les autres systèmes n'ont que peu d'importance et ne seront pas détaillés)
	-System_GridScene: les systèmes pour le jeu de Match3. Ici seul le script LevelSystem est rééllement important.
	-GridScene: Contient la grille de jeu. Seul le script Grid est important.
	-UI_GridScene: Contient de l'ui. La scène n'a pas d'importance.
	-Player: Contient le script Player.

Mise en place:
	-Les scènes se regroupent par ScenePackage. Un ScenePackage fonctionne de lui-même et répresente une partie du jeu. 
	Il y en a 2 : celui du GameRoot et GameScene.

Architecture:
	-Toutes les entités vraiment importantes dérivent de StatedMono<T>. Un script que j'ai créé pour ce test, qui facilite la gestion des états d'une entité.
	Il y a 3 entités qui dérivent de cette classe: Grid (qui représente la grille de jeu), Player(qui représente le joueur), et Piece (qui répresente une pièce sur la grille).
	-Chaque entité a un nombre x d'états et un enum qui identifie un état, que l'on rajoute via Add(Enum, State). Cela facilite l'accès pour changer d'état via l'enum.
	
	-Les StatedMono<T> ont des fonctions d'entrée, d'update et de sortie d'état pour faciliter leur gestion. 
	le script StatedMonoSystem se chargeant de bien coordonner les entités voulant être mise à jour.

	-La communication inter-état et extra-entité se fait via le FrameDataBuffer<T>. C'est un script qui rend une donnée valide pour une et une seule frame.
	Cela me permet de ne pas dépendre de l'ordre d'exécution du type : A->B donne X comme résultat mais B->A me donne un état différent de l'application.
	A noter qu'ici, il n'est peut-être pas nécessaire d'avoir une si grosse architecture. Mais j'ai voulu prévenir ce cas. 

	-Le script Piece n'a aucune connaissance de la grille sur laquelle il est placé. Grid a conscience de Piece, Player lui a conscience de Grid et de Piece. Je découpe mes entités pour avoir le moins de couplage que possible entre elles.
	Ainsi Grid peut être joué n'importe comment en suivant les règles de game design (elle pourrait être jouée par l'IA).
	Ainsi Piece peut être placé sur n'importe quelle grille.
	Player, lui, sait exactement à quoi il joue et avec quoi. 

	-Il n'y a pas de singleton dans mes applications. J'utilise le design pattern du ServiceProvider/ServiceSubscriber. Toute donnée "static" est récupérée via ServiceProvider.GetService<T>().


GameDesign:
	-J'ai globalement suivi les règles du test. Grid élimine automatiquement les connexions horizontales ou verticales de Piece. L'algorithme part du bas gauche, pour aller vers le haut droit de la grille. 
	-Grid détecte sur une même case la meilleure connexion de Pieces (verticale ou horizontale) et élimine celle-ci. 
	-Player a le droit de faire un peu mieux que Grid, il peut en effet faire des connexions en croix de Pieces. J'ai mis ça en place pour donner au joueur un peu de reflexion et un "truc en +" par rapport à l'algorithme.

	-Player peut drag and drop, ou selectionner 2 cases à interchanger. C'est lui qui ordonne à Grid de faire le changement.
	-Grid reçoit la demande de changement et accepte ou non de la faire. Ainsi interchanger 2 pièces ne créant pas de connexion n'est pas permis par Grid. Mais ça pourrait être le cas sans que Player soit affecté.
	-Grid ensuite élimine si possible les Pieces.

	-La première connexion créée par le joueur en déplaçant une Piece est prise en compte en premier, indépendamment de l'algorithme de la grille. 
	J'ai laissé la possibilité au joueur de choisir ce qui devait être détruit en premier pour qu'il ait une certaine liberté stratégique.

Polish:
	-J'ai intégré tous les sons. Le sons de combo ne se déclenchent que dans le cas où une connexion de plus de 4 Pieces est éliminée.
	-VFX pour l'élimination des Pieces.
	-Timer pour donner un but au joueur.
	-ScoreSystem: Grid transmet les Pieces éliminés au ScoreSystem. Celui-ci ajoute +1 de score pour chaque Pieces éliminés, il ajoute +1 point pour chaque Pieces faisant une connexion de plus de 4 Pieces. 
	Il y a un systeme de combo qui ajoute un multiplicateur à chaque combo.
	Le score s'enregistre sur un fichier via un outil précédemment créé.
	-Popup de fin de jeu avec score + meilleur score.


En espérant que ça vous plaise.
Constantin Benoit.