using System.Text.Json;
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


namespace SKPluginComplet;

public static class SemanticPluginLoader
{
    public static IDictionary<string, KernelFunction> LoadSemanticPlugin(
        Kernel kernel,
        string directoryPath,
        out string pluginName)
    {
        pluginName = new DirectoryInfo(directoryPath).Name;

        // Charger les paramètres du plugin s'ils existent
        var settings = new OpenAIPromptExecutionSettings
        {
            Temperature = 0.7,
            TopP = 0.9,
            MaxTokens = 256
        };

        var configPath = Path.Combine(directoryPath, "config.json");
        if (File.Exists(configPath))
        {
            using var stream = File.OpenRead(configPath);
            using var doc = JsonDocument.Parse(stream);
            if (doc.RootElement.TryGetProperty("chat", out var chatSettings))
            {
                if (chatSettings.TryGetProperty("temperature", out var t)) settings.Temperature = t.GetDouble();
                if (chatSettings.TryGetProperty("top_p", out var p)) settings.TopP = p.GetDouble();
                if (chatSettings.TryGetProperty("max_tokens", out var m)) settings.MaxTokens = m.GetInt32();
            }

            if (doc.RootElement.TryGetProperty("name_for_model", out var n))
            {
                pluginName = n.GetString() ?? pluginName;
            }
        }

        var functions = new Dictionary<string, KernelFunction>(StringComparer.OrdinalIgnoreCase);

        foreach (var file in Directory.GetFiles(directoryPath, "*.skprompt.txt"))
        {
            var functionName = Path.GetFileName(file)
                .Replace(".skprompt.txt", "", StringComparison.OrdinalIgnoreCase);
            var prompt = File.ReadAllText(file);

            var kernelFunction = kernel.CreateFunctionFromPrompt(prompt, settings);

            functions[functionName] = kernelFunction;
        }

        return functions;
    }
}