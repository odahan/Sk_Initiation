using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Connectors.OpenAI;
using Microsoft.SemanticKernel.ChatCompletion; // Needed for FunctionCallBehavior
using System;
using SKNativePlugins.Plugins;

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


#region license
var apiKey = ApiKeyStore.Load();

if (string.IsNullOrWhiteSpace(apiKey))
{
    Console.Write("Veuillez entrer votre clé OpenAI : ");
    apiKey = Console.ReadLine();
    ApiKeyStore.Save(apiKey!);
}
#endregion


var builder = Kernel.CreateBuilder();
builder.AddOpenAIChatCompletion("gpt-4-turbo", apiKey!);
var kernel = builder.Build();

// Plugin C# natif
var outils = new OutilsTexte();
var plugin = kernel.ImportPluginFromObject(outils, "texte");

#region Compter les mots
// Appel à la fonction "compterMots"
var result = await kernel.InvokeAsync(plugin["compterMots"], new()
{
    ["texte"] = "Ceci est une fonction native très utile."
});
Console.WriteLine($"Nombre de mots : {result.GetValue<int>()}");

//return;
#endregion

#region autres fonctions
// Appel à la fonction "tronquer"
var tronque = await kernel.InvokeAsync(plugin["tronquer"], new()
{
    ["texte"] = "Ceci est une longue phrase qui doit être tronquée intelligemment.",
    ["longueur"] = 30
});
Console.WriteLine($"Texte tronqué : {tronque.GetValue<string>()}");

// Appel à la fonction "inverser"
var inversed = await kernel.InvokeAsync(plugin["inverser"], new()
{
    ["texte"] = "Bonjour le monde"
});
Console.WriteLine($"Texte inversé : {inversed.GetValue<string>()}");

//return;
#endregion

#region appel via prompt IA
// Appel via prompt IA → C# natif
var prompt = """
             Raccourcis ce texte à environ 40 caractères :
             "Le comité d’organisation s’est réuni mardi pour valider le budget prévisionnel du second trimestre."
             """;


PromptExecutionSettings settings = new() { FunctionChoiceBehavior = FunctionChoiceBehavior.Auto() };

var resultPrompt = await kernel.InvokePromptAsync(prompt, new(settings));
Console.WriteLine("\n[Prompt IA → fonction C#]");
Console.WriteLine(resultPrompt.GetValue<string>());
#endregion