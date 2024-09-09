using Esprima;
using Esprima.Ast;
using UnityEngine;

namespace Baxter.Internal.ClusterScriptExtensions.Editor
{
    public static class DebugLogUtils
    {
        private static void LogCommentAndTokenAndStatements(string code)
        {
            var parser = new JavaScriptParser(new ParserOptions()
            {
                Comments = true,
                Tokens = true,
            });
            var program = parser.ParseScript(code);
            Debug.Log("Logging program itself...");
            Debug.Log(program.ToString());

            if (program.Comments != null)
            {
                Debug.Log("Comment...");
                foreach (var comment in program.Comments)
                {
                    Debug.Log($"Comment: loc={comment.Location.Start}:{comment.Location.End}, value={comment.Value}");
                }

                Debug.Log("Comment logged.");
            }

            if (program.Tokens != null)
            {
                Debug.Log("Tokens...");
                foreach (var token in program.Tokens)
                {
                    Debug.Log(
                        $"token: loc={token.Location.Start}:{token.Location.End}, type={token.Type}, value={token.Value}");
                }

                Debug.Log("Tokens logged.");
            }

            Debug.Log("Statement: Start");
            foreach (var statement in program.Body.AsSpan())
            {
                Debug.Log("Statement:");
                foreach (var node in statement.ChildNodes)
                {
                    Debug.Log($"node: loc={node.Location.Start}:{node.Location.End},type={node.Type}");
                    var foo = node as VariableDeclaration;
                }

                Debug.Log("Statement logged.");
            }

            Debug.Log("Statement: End");
        }

        private static void WriteDownNode(Node node, string indent = "")
        {
            Debug.Log($"{indent}node[{node.Type}]: {node}");
            foreach (var child in node.ChildNodes)
            {
                WriteDownNode(child, indent + "  ");
            }
        }
    }
}
