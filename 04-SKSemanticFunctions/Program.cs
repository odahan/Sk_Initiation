using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Connectors.OpenAI;

/*
   ──────────────────────────────────────────────────────────────────────────
   Conditions d’utilisation – Code de démonstration (Initiation Semantic Kernel)
   ──────────────────────────────────────────────────────────────────────────

   © 2025 Olivier Dahan & E-Naxos – Tous droits réservés.

   1) Objet et périmètre
      Ce code est fourni « tel quel », sans aucune garantie, à des fins
      exclusivement pédagogiques, en accompagnement de la série
      « Initiation à Semantic Kernel » publiée sur :
      https://www.youtube.com/@e-naxosConsulting

   2) Propriété intellectuelle
      La propriété du code et des documents associés appartient à
      Olivier Dahan & E-Naxos. 
      Toute utilisation hors du cadre pédagogique personnel, toute
      intégration dans un produit/service, tout usage professionnel
      ou commercial, toute mise en production, toute modification,
      adaptation, publication ou redistribution, en tout ou partie,
      sont interdits sans autorisation écrite préalable de l’auteur.

   3) Licence applicable aux documents fournis (code + supports)
      L’ensemble des documents fournis est régi par la licence :
      Creative Commons Attribution – NonCommercial – NoDerivatives 4.0 International
      (CC BY-NC-ND 4.0)

      Texte officiel : https://creativecommons.org/licenses/by-nc-nd/4.0/
      Effet pratique (résumé non contractuel) :
        • Vous pouvez télécharger et partager le contenu tel quel,
          avec attribution, sans usage commercial, et sans modification.
        • Aucune création d’œuvre dérivée n’est autorisée.
        • Toute autre utilisation ou publication nécessite l’accord écrit
          préalable de l’éditeur (Olivier Dahan & E-Naxos).

   4) Exclusion de garantie et limitation de responsabilité
      CE CONTENU EST FOURNI « EN L’ÉTAT », SANS AUCUNE GARANTIE EXPRESSE
      OU IMPLICITE, Y COMPRIS, SANS S’Y LIMITER, LES GARANTIES DE QUALITÉ
      MARCHANDE, D’ADÉQUATION À UN USAGE PARTICULIER ET D’ABSENCE
      DE CONTREFAÇON. EN AUCUN CAS L’AUTEUR/L’ÉDITEUR NE SAURAIT ÊTRE
      TENU RESPONSABLE DE DOMMAGES DIRECTS OU INDIRECTS, SPÉCIAUX,
      ACCESSOIRES OU CONSÉCUTIFS, PERTES DE DONNÉES OU D’EXPLOITATION,
      DÉCOULANT DE L’UTILISATION OU DE L’IMPOSSIBILITÉ D’UTILISER CE CODE,
      MÊME SI LA POSSIBILITÉ DE TELS DOMMAGES A ÉTÉ SIGNALÉE.

   5) Tolérance d’usage
      Autorisé : consultation, exécution locale et étude à titre d’exemple,
      à des fins personnelles d’apprentissage, strictement dans le cadre
      de la série précitée.
      Interdit : tout autre usage (notamment professionnel/commercial),
      toute redistribution ou hébergement public (dépôts Git, gists,
      packages, sites, etc.) sans accord écrit préalable.

   ──────────────────────────────────────────────────────────────────────────
*/


// Chargement ou saisie de la clé API OpenAI
#region license
var apiKey = ApiKeyStore.Load();

if (string.IsNullOrWhiteSpace(apiKey))
{
    Console.Write("Veuillez entrer votre clé OpenAI : ");
    apiKey = Console.ReadLine();
    ApiKeyStore.Save(apiKey!);
}
#endregion

// Initialisation du kernel Semantic Kernel avec le modèle OpenAI
var builder = Kernel.CreateBuilder();
builder.AddOpenAIChatCompletion("gpt-3.5-turbo-1106", apiKey!);
var kernel = builder.Build();

// Chargement du prompt pour la fonction de résumé
var resumePrompt = File.ReadAllText("./prompts/Resumeur.txt"); 

// Création de la fonction sémantique pour le résumé avec paramètres de génération
var resumeur = KernelFunctionFactory.CreateFromPrompt(
    resumePrompt,
    executionSettings: new OpenAIPromptExecutionSettings
    {
        Temperature = 0.7, // audace globale (déviation par rapport au plus probable)
        TopP = 0.8 // nucleus sampling - contrôle la diversité des réponses générées 
    });

// Texte d'entrée à résumer
var input = @"Le projet Semantic Kernel, développé par Microsoft, est une boîte à outils open-source conçue pour aider les développeurs .NET à exploiter pleinement les capacités des modèles d’intelligence artificielle générative tels que ceux d’OpenAI ou Azure OpenAI. Il propose une approche modulaire permettant d’orchestrer des fonctions sémantiques (basées sur des prompts) et des fonctions natives (méthodes C# classiques), en les combinant dans des pipelines intelligents. Grâce à ses abstractions claires, il devient possible d’automatiser des tâches complexes à partir d’instructions en langage naturel.
En plus de cela, Semantic Kernel permet d'intégrer une mémoire sémantique persistante, en stockant des informations vectorielles dans des moteurs comme Qdrant ou Redis. Cela permet aux applications de “se souvenir” de conversations, documents ou éléments importants sur la durée, et de les réutiliser intelligemment.
Ce projet s’inscrit dans la stratégie plus large de Microsoft visant à rendre l’IA générative accessible, reproductible et intégrable dans des logiciels métiers existants, sans nécessiter une expertise approfondie en machine learning. Il favorise un développement agile et une intégration locale ou cloud, tout en respectant les pratiques de développement modernes de l’écosystème .NET.";

// Exécution de la fonction de résumé et récupération du résultat
var résuméResult = await kernel.InvokeAsync(resumeur, new() { ["input"] = input });
var résumé = résuméResult.GetValue<string>();

// Sauvegarde de la couleur de la console
var fcolor = Console.ForegroundColor;

// Affichage du résumé généré en vert
Console.WriteLine("Résumé : ");
Console.ForegroundColor = ConsoleColor.Green;
Console.WriteLine(résumé);
Console.ForegroundColor = fcolor;

Console.WriteLine("1ere fonction exécutée");
Console.ReadLine(); // Pause pour lire le résumé

// Chargement du prompt pour la génération d'email
var mailPrompt = File.ReadAllText("./prompts/MailGenerator.txt");

// Création de la fonction sémantique pour générer un email personnalisé
var mailFunction = KernelFunctionFactory.CreateFromPrompt(mailPrompt,
    executionSettings: new OpenAIPromptExecutionSettings
    {
        Temperature = 0.7,
        TopP = 0.8
    });

// Définition des variables de contexte pour l'email
var contextVars = new KernelArguments
{
    ["resume"] = résumé,
    ["client"] = "Mme Durand",
    ["produit"] = "la licence Premium du logiciel Delta"
};

// Exécution de la fonction de génération d'email et récupération du résultat
var mailResult = await kernel.InvokeAsync(mailFunction, contextVars);
var mail = mailResult.GetValue<string>();

// Affichage de l'email généré en vert
Console.WriteLine("\nMail généré :\n" );
Console.ForegroundColor = ConsoleColor.Green;
Console.WriteLine(mail);
Console.ForegroundColor = fcolor;