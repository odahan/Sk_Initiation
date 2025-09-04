using Microsoft.SemanticKernel;
using SKPlanningIntro.Plugins.utils;
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


namespace SKPlanningIntro;

class Program
{
    static async Task Main()
    {
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
        builder.AddOpenAIChatCompletion("gpt-3.5-turbo", apiKey!, serviceId: "chat");
        var kernel = builder.Build();

        // charge des plugins
        var assistantPlugin = KernelPluginFactory.CreateFromFunctions(
            "assistant",
            [
                KernelFunctionFactory.CreateFromPrompt(
                    await File.ReadAllTextAsync(Path.Combine(AppContext.BaseDirectory, "Plugins", "assistant", "Salutation.txt")),
                    functionName: "Salutation",
                    description: "Renvoie une salutation."
                ),
                KernelFunctionFactory.CreateFromPrompt(
                    await File.ReadAllTextAsync(Path.Combine(AppContext.BaseDirectory, "Plugins", "assistant", "Signature.txt")),
                    functionName: "Signature",
                    description: "Renvoie une signature."
                )
            ]
        );

        // Enregistrement du plugin assistant
        kernel.Plugins.Add(assistantPlugin);

        // Enregistrement du plugin utils
        kernel.ImportPluginFromObject(new UtilsPlugin(), "utils");

        var consoleDefaultColor = Console.ForegroundColor;

        // Vérification des fonctions chargées
        Console.WriteLine("--- Fonctions disponibles ---");
        var fc = 0;
        foreach (var f in kernel.Plugins.GetFunctionsMetadata())
        {
            Console.WriteLine($"Function #{++fc}, Plugin: {f.PluginName}, Fonction: {f.Name}");
        }

        // charger les fonctions et préparer le message en mode manuel
        var salutation = kernel.Plugins.GetFunction("assistant", "Salutation");
        var date = kernel.Plugins.GetFunction("utils", "DateFr");
        var signature = kernel.Plugins.GetFunction("assistant", "Signature");

        var salutationResult = await kernel.InvokeAsync(salutation);
        var dateResult = await kernel.InvokeAsync(date);
        var signatureResult = await kernel.InvokeAsync(signature);

        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine("Resultat salutation: "+salutationResult);
        Console.ForegroundColor = ConsoleColor.Magenta;
        Console.WriteLine("Resultat date: "+dateResult);
        Console.ForegroundColor = ConsoleColor.DarkYellow;
        Console.WriteLine("Resultat signature: "+signatureResult);
        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.WriteLine("Résultat concaténé : ");
        var manualMessage = $"{salutationResult}\n{dateResult}\n{signatureResult}";
        Console.WriteLine("--- Plan manuel ---");
        Console.WriteLine(manualMessage);
        Console.ForegroundColor = consoleDefaultColor;

        // --- Plan dynamique avec InvokePromptAsync ---
        var executionSettings = new OpenAIPromptExecutionSettings
        {
            FunctionChoiceBehavior = FunctionChoiceBehavior.Required()
        };

        var promptcontent = @"
Rédiges un message comprenant une salutation et une signature et dont le corps du message présente une série de vidéos
sur Semantic Kernel qui vient de paraître (utilise la date du jour pour la date de parution que tu préciseras).
Tu utiliseras les fonctions natives et sémantiques définies pour résoudre salutation, signature et date du jour.";
 // les role, user, system sont optionnels mais recommandés, le format préféré étant de donner ces infos à part en JSON


        var result = await kernel.InvokePromptAsync(
             promptcontent, 
            new(executionSettings)
        );

        string planResult = result.ToString();
        Console.WriteLine("--- Plan dynamique ---");
        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine(planResult);
        Console.ForegroundColor = consoleDefaultColor;
    }
}