using System.Text.RegularExpressions;
using Microsoft.SemanticKernel;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

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


// --- 1. Lecture de la clé API via ApiKeyStore (fourni dans les exemples précédents)
var apiKey = ApiKeyStore.Load();
if (string.IsNullOrWhiteSpace(apiKey))
{
    Console.Write("Veuillez entrer votre clé OpenAI : ");
    apiKey = Console.ReadLine();
    ApiKeyStore.Save(apiKey!);
}

// --- 2. Configuration de la télémétrie simplifiée (console uniquement)
var telemetry = new SimpleTelemetryPipeline();

// Logger de base
var services = new ServiceCollection();
services.AddLogging(builder => builder.AddConsole());
var serviceProvider = services.BuildServiceProvider();
var loggerFactory = serviceProvider.GetRequiredService<ILoggerFactory>();

// Création du Kernel avec instrumentation
var builder = Kernel.CreateBuilder();
builder.Services.AddSingleton<ILoggerFactory>(loggerFactory);
builder.AddOpenAIChatCompletion("gpt-3.5-turbo-1106", apiKey!, serviceId: "chat");
var kernel = builder.Build();

// --- 3. Ajout d'un filtre Semantic Kernel (IFunctionInvocationFilter)
kernel.FunctionInvocationFilters.Add(new SimpleSecurityFilter());
// --- La télémtrie doit être ajoutée en tant que filtre les events sont deprecated
kernel.FunctionInvocationFilters.Add(new TelemetryFunctionInvocationFilter(telemetry));

// --- 4. Assistant simple : résumé de texte
var template = """
Résume le texte suivant de manière concise :
{{$texte}}
""";

var prompt = kernel.CreateFunctionFromPrompt(
    template,
    functionName: "ResumerTexte",
    description: "Résumé de texte"
);

// --- 5. Lecture de l'entrée utilisateur
Console.WriteLine("Entrez un texte à résumer :");
var input = Console.ReadLine() ?? "";

// --- 6. Validation manuelle classique (avant SK)
if (input.Contains("efface", StringComparison.OrdinalIgnoreCase) ||
    Regex.IsMatch(input, @"\b(secret|confidentiel)\b", RegexOptions.IgnoreCase))
{
    Console.WriteLine("❌ Entrée refusée pour raison de sécurité.");
    return;
}

// --- 7. Exécution
var arguments = new KernelArguments { ["texte"] = input };
var result = await kernel.InvokeAsync(prompt, arguments);

// --- 8. Résultat
Console.WriteLine("\n✅ Résumé généré :\n" + result.GetValue<string>());

// --- 9. Affichage des événements
Console.WriteLine("\n📊 Événements collectés :");
foreach (var evt in telemetry.GetEvents())
{
    Console.WriteLine(evt);
}

// === CLASSES UTILISÉES ===

public class SimpleTelemetryPipeline
{
    private readonly List<string> _events = new();
    public void Publish(string evt)
    {
        _events.Add(evt);
        Console.WriteLine(evt);
    }
    public IReadOnlyList<string> GetEvents() => _events;
}

public class SimpleSecurityFilter : IFunctionInvocationFilter
{
    public async Task OnFunctionInvocationAsync(FunctionInvocationContext context, Func<FunctionInvocationContext, Task> next)
    {
        // Use the indexer to access the argument instead of .Get()
        var texte = context.Arguments.ContainsKey("texte") ? context.Arguments["texte"]?.ToString() ?? "" : "";

        if (texte.Contains("pirater", StringComparison.OrdinalIgnoreCase))
        {
            Console.WriteLine("🚫 Contenu bloqué par le filtre SK (mot interdit).");
            return;
        }

        await next(context); // Pass context as required by the interface
    }
}

// Add this class to the file (after SimpleSecurityFilter)
public class TelemetryFunctionInvocationFilter(SimpleTelemetryPipeline telemetry) : IFunctionInvocationFilter
{
    public async Task OnFunctionInvocationAsync(FunctionInvocationContext context, Func<FunctionInvocationContext, Task> next)
    {
        await next(context);
        telemetry.Publish($"[SK] Fonction '{context.Function.Name}' exécutée");
    }
}
