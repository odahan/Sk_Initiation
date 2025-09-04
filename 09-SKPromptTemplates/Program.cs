using System;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;

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


// Chargement de la clé API
var apiKey = ApiKeyStore.Load();

if (string.IsNullOrWhiteSpace(apiKey))
{
    Console.Write("Veuillez entrer votre clé OpenAI : ");
    apiKey = Console.ReadLine();
    ApiKeyStore.Save(apiKey!);
}

// Construction du kernel avec le modèle chat GPT
var builder = Kernel.CreateBuilder();
builder.AddOpenAIChatCompletion("gpt-3.5-turbo-1106", apiKey!, serviceId: "chat");
var kernel = builder.Build();

// Activation du moteur de templating officiel (Handlebars)
var promptTemplateFactory = new KernelPromptTemplateFactory();

// 🔐 SECTION : protection du prompt avec contenu structuré

// Ici on simule un cas où on veut insérer une balise HTML ou XML (par exemple dans une variable "nom")
// Le filtre de sécurité de Semantic Kernel bloquerait ce contenu par défaut
// On lève cette protection volontairement avec AllowDangerouslySetContent = true
// Ce paramètre doit être utilisé uniquement si on maîtrise le contenu injecté

var mailTemplate = promptTemplateFactory.Create(new PromptTemplateConfig
{
    Template = @"Rédige un email à notre client {{$nom}} en réponse à son mail ({{$mailOriginal}}) envoyé à notre société et dont l'objet est {{$sujet}}.
                 Précise que pour toute correspondance future, il peut utiliser le numéro de ticket suivant : {{$numeroTicket}}.
                 Utilise un ton formel et évite toute opinion personnelle.",
    InputVariables =
    [
        new InputVariable
        {
            Name = "nom",
            Description = "Nom du destinataire",
            IsRequired = true,
            AllowDangerouslySetContent = true //  Autorise l'insertion de contenu HTML/XML dans la variable
        },
        new InputVariable
        {
            Name = "sujet",
            Description = "Sujet du mail",
            IsRequired = true
            // ici, pas besoin de AllowDangerouslySetContent : le contenu est simple
        },
        new InputVariable
        {
            Name = "numeroTicket",
            Description = "Numéro du ticket du suivi de la demande",
            IsRequired = true,
            Default = "N/A" // valeur par défaut si non fourni
        },
        new InputVariable
        {
            Name = "mailOriginal",
            Description = "mail original émanant du client",
            IsRequired = true,
            Default = "N/A"
        }
    ]
});

// Préparation des variables utilisateur (le nom pourrait contenir des balises HTML)
var arguments = new KernelArguments
{
    // Le nom contient ici un élément structuré (ex: balise <b>), normalement filtré
    ["nom"] = "<b>M. Morel</b>", // ce contenu structuré est accepté grâce à AllowDangerouslySetContent, extrait par IA du mail client
    ["sujet"] = "demande de support technique", // extrait de l'objet du mail ou calculé par une fonction sémantique
    ["numeroTicket"] = "T2025/12345", // extrait d'un CRM ou d'un système de ticketing
    ["mailOriginal"] = "Bonjour, j'ai un problème avec mon ordinateur d'écran bleu lorsque je lance votre logiciel SuperScreen." // mail original
};

// Rendu du prompt structuré
var renderedPrompt = await mailTemplate.RenderAsync(kernel, arguments);
Console.WriteLine("\n--- Prompt rendu ---\n" + renderedPrompt);

// Envoi via le modèle GPT (chat)
var chat = kernel.GetRequiredService<IChatCompletionService>();
var chatHistory = new ChatHistory(renderedPrompt);

var response = await chat.GetChatMessageContentAsync(chatHistory);
Console.WriteLine("\n--- Mail généré ---\n" + response.Content);
