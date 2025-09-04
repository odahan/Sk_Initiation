README.TXT
================================================================================
Pack « Initiation à Semantic Kernel » – README
================================================================================

IMPORTANT — DROITS D’UTILISATION
Ce pack est régi par la licence indiquée dans le fichier « LICENCE.TXT ». 
Merci de le lire avant toute utilisation. Toute utilisation implique l’acceptation 
de ses termes.

--------------------------------------------------------------------------------
IDENTITÉ & CONTACTS
- Auteur : Olivier Dahan © 2025
- Contact : odahan@e-naxos.com
- Publication : E-Naxos sarl – chaîne YouTube https://www.youtube.com/@e-naxosConsulting
- Site E-Naxos : www.e-naxos.com
- Blog « Dot.Blog » : www.e-naxos.com/blog
- Film complet (11 épisodes) : https://youtu.be/X_cuKRb0gZA

--------------------------------------------------------------------------------
PRÉSENTATION DU CONTENU DU PACK
Ce pack accompagne la série vidéo « Initiation à Semantic Kernel » (11 épisodes, FR).
Il contient :
- /Sources           → Code source de démonstration (C#) par épisode ou thème
- LICENCE.TXT        → Licence d'utilisation
- README.TXT         → Le présent fichier

Remarque : l’arborescence exacte peut légèrement varier selon les mises à jour.
Dans les dernières versions du pack les Quizz, les fiches mémo et les exercices
corrigés ne sont plus fournis. Ils feront l'objet de publications à part sur le
blog "Dot.Blog" www.e-naxos.com/blog

--------------------------------------------------------------------------------
CONTEXTE DE LA SÉRIE
L’initiation couvre les fondamentaux pour intégrer Semantic Kernel (SK) dans des
applications .NET/C#. Elle aborde : installation, fonctions sémantiques, plugins
C#, structuration de projet, sécurisation (filtres, télémétrie) et planification
(orchestration) — afin de passer rapidement de « zéro » à un assistant/agent IA
opérationnel, avec du code réaliste et des bonnes pratiques.
Langue : **français**. Version ciblée : **SK 1.61 et 1.62+**.

--------------------------------------------------------------------------------
COMMENT UTILISER LE MATÉRIEL
1) Pré-requis
   - Visual Studio 2022 (édition Pro utilisée lors du tournage, à jour en août 2025)
   - SDK .NET compatible avec les projets du dossier /Sources
   - Clé API OpenAI valide (obligatoire pour exécuter les parties IA)
   - Connexion Internet pour les appels au modèle (si vous utilisez OpenAI/équivalent)

2) Ouverture et exécution rapide
   - Ouvrez la solution (.sln) dans Visual Studio 2022.
   - Restaurez les packages NuGet (VS le propose automatiquement).
   - Sélectionnez le projet de démo voulu comme « Projet de démarrage ».
   - Préparez la clé API OpenAI (voir §3).
   - Lancez (F5/Ctrl+F5). Les démos sont conçues pour fonctionner directement
     après chargement, sur l’environnement indiqué ci-dessus.

3) Configuration de la clé OpenAI
   - Utilisez une **clé API OpenAI valide**.
   - La clé peut être lue via :
       • un petit utilitaire/stockage local fourni (ex. classe « ApiKeyStore »),
   - Lors de la première exécution du premier exemple que vous ferez tourner, la
     clé d'API vous sera demandée à la Console. Entrez votre clé et validez.
     Il ne sera ensuite plus nécessaire de la saisir dans les autres projets.
   - Référez-vous aux commentaires dans le code et aux épisodes (notamment l’épisode 2)
     pour la méthode exacte utilisée.
   - APRES UTILISATION DES EXEMPLES DE CODE FOURNIS, PENSEZ à utiliser la fonction 
     d'effacement de "ApiKeyStore" en créant un projet console pour ce faire. Cela
     supprimera le stockage disque de la clé. ENSUITE SUPPRIMEZ LA CLE DANS LA 
     CONSOLE OPENAI AFIN QU'ELLE NE PUISSE PAS ËTRE REUTILISEE A VOTRE INSU.
     Si vous avez besoin plus tard de refaire fonctionner le code exemple,
     créez une nouvelle clé et saisissez-la comme indiqué plus haut.

4) Exercices & Quiz
   - Les dossiers /exercices et /quiz contiennent des énoncés et leurs corrigés
     pour valider la compréhension. Les corrigés sont fournis **à titre d’exemple**.
     Dans la dernière version du pack ces fichiers ne sont plus fournis et feront
     l'objet de publications à part sur le Dot.Blog www.e-naxos.com/blog

5) Limitations
   - Le code est fourni « tel quel », sans garantie, à des fins pédagogiques (voir LICENSE.TXT).
   - Les configurations, API et versions pouvant évoluer, adaptez au besoin.
   - En cas de doute consultez la documentation officielle

--------------------------------------------------------------------------------
COMPATIBILITÉ & VERSIONS
- Environnement de référence : Visual Studio 2022 (Pro), à jour **août 2025**.
- Semantic Kernel : **1.61** et **1.62+**.
- En cas de breaking changes ultérieurs, reportez-vous aux notes des épisodes
  et mettez à jour vos packages NuGet en conséquence.

--------------------------------------------------------------------------------
SÉCURITÉ, COÛTS & BONNES PRATIQUES (RAPPEL)
- Protégez votre clé API (ne la committez jamais).
- Surveillez l’usage (tokens/coûts) si vous exécutez de nombreux prompts.
- Les exemples intègrent des validations/filtres simples à titre d’illustration ;
  durcissez-les avant tout usage **hors** du cadre pédagogique.
- Les journaux et la télémétrie (si activés) peuvent contenir des données sensibles :
  configurez-les prudemment.

--------------------------------------------------------------------------------
DROITS & LICENCE
Les droits d’utilisation, limitations, exclusions de garantie et mentions légales
sont précisés dans **LICENSE.TXT** (licence Creative Commons BY-NC-ND 4.0).
En résumé non contractuel :
- usage **pédagogique et personnel** uniquement ;
- pas d’usage **professionnel/commercial** ;
- pas de modification ni d’œuvre dérivée ;
- attribution requise lors de tout partage autorisé ;
- toute autre utilisation nécessite une **autorisation écrite préalable**.

--------------------------------------------------------------------------------
ASSISTANCE
- Pour les questions liées au contenu : voir les commentaires des vidéos sur la
  chaîne YouTube ci-dessus.
- Pour des usages dépassant le cadre de la licence : contacter l’auteur.

Merci d’utiliser ce pack et bon apprentissage !
