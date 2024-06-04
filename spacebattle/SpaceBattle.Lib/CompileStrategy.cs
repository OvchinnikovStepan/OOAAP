namespace SpaceBattle.Lib;
using System.Reflection;
using Hwdtech;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

public class CompileStrategy : IStrategy
{
    public object Run(params object[] args)
    {
        var codeString = (string)args[0];
        var type = (Type)args[1];

        var compilation = CSharpCompilation.Create(type.ToString() + "Adapter")
            .AddReferences(IoC.Resolve<IEnumerable<MetadataReference>>("Compile.References"))
            .WithOptions(new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary))
            .AddSyntaxTrees(CSharpSyntaxTree.ParseText(codeString));

        Assembly assembly;

        using (var ms = new MemoryStream())
        {
            var result = compilation.Emit(ms);
            ms.Seek(0, SeekOrigin.Begin);
            assembly = Assembly.Load(ms.ToArray());
        }

        return assembly;
    }
}
