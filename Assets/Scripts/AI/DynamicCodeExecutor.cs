#if UNITY_EDITOR
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis;
using System.Reflection;
using System.Linq;
using UnityEngine;
using System.IO;
using System;

public static class DynamicCodeExecutor
{
    public static void Execute(string markdown)
    {
        if (!markdown.Contains("```")) return;
        
        var startIndex = markdown.IndexOf("```cs", StringComparison.Ordinal) + 5;
        var endIndex = markdown.IndexOf("```", startIndex, StringComparison.Ordinal);
        var code = markdown.Substring(startIndex, endIndex - startIndex).Trim();
        
        string fullCode = @"
using System;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEditor;

public class HoenkyPlanky : MonoBehaviour
{
    public static void Execute()
    {
        " + code + @"
    }
}";
        
        CompileAndRun(fullCode);
    }

    private static void CompileAndRun(string code)
    {
        var syntaxTree = CSharpSyntaxTree.ParseText(code);
        var assemblyName = Path.GetRandomFileName();
        var references = AppDomain.CurrentDomain.GetAssemblies()
            .Where(a => !a.IsDynamic && !string.IsNullOrEmpty(a.Location))
            .Select(a => MetadataReference.CreateFromFile(a.Location))
            .Cast<MetadataReference>();
        
        var compilation = CSharpCompilation.Create(assemblyName,
            new[] { syntaxTree },
            references: references,
            options: new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary));

        using var ms = new MemoryStream();
        var result = compilation.Emit(ms);

        if (result.Success)
        {
            try
            {
                ms.Seek(0, SeekOrigin.Begin);
                var assembly = Assembly.Load(ms.ToArray());

                var type = assembly.GetType("HoenkyPlanky");
                var method = type.GetMethod("Execute", BindingFlags.Public | BindingFlags.Static);

                method.Invoke(null, null);
            }
            catch
            {
                Debug.Log("smth went wrong bru");
            }
        }
        else
        {
            var failures = result.Diagnostics.Where(diagnostic =>
                diagnostic.IsWarningAsError ||
                diagnostic.Severity == DiagnosticSeverity.Error);

            foreach (var diagnostic in failures)
            {
                Debug.LogError($"{diagnostic.Id}: {diagnostic.GetMessage()}");
            }
        }
    }
}
#endif