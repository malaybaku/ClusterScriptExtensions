using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Esprima;
using Esprima.Ast;

namespace Baxter.ClusterScriptExtensions.Editor.ScriptParser
{
    public static class ExtensionFieldParser
    {
        private readonly struct FieldDefineComment
        {
            public readonly Location Location;
            public readonly ExtensionFieldType Type;

            public FieldDefineComment(Location location, ExtensionFieldType type)
            {
                Location = location;
                Type = type;
            }
        }

        private static readonly Dictionary<string, ExtensionFieldType> TypeNames = new()
        {
            ["bool"] = ExtensionFieldType.Bool,
            ["int"] = ExtensionFieldType.Int,
            ["float"] = ExtensionFieldType.Float,
            ["string"] = ExtensionFieldType.String,
            ["vector2"] = ExtensionFieldType.Vector2,
            ["vector3"] = ExtensionFieldType.Vector3,
            ["quaternion"] = ExtensionFieldType.Quaternion,
        };

        private static readonly Regex FieldMetaCommentRegex = new(@"^\s*@field\(([^\)]+)\)");

        // 対象フィールドの検出条件
        // - Scriptのトップレベルで、コメント > 変数定義がこの順で並んでいる
        // - コメントは正規表現で記述できる(強めに制限された)パターンに従っている
        // - 変数定義は "const x = 42;" のような形式になっていることを要求する、つまり以下が必要。
        //   - const値を初期化するステートメントである
        //   - ちゃんとした値が入っている
        public static ScriptExtensionField[] ExtractTargetFields(string code)
        {
            var result = new List<ScriptExtensionField>();
            var parser = new JavaScriptParser(new ParserOptions()
            {
                Comments = true,
            });

            var program = parser.ParseScript(code);
            var statements = program.Body.ToArray();

            // ステートメントの内側に入ってるコメントを弾くことでトップレベルの単独コメントだけを残す
            var comments = program.Comments
                .Where(c =>
                    FieldMetaCommentRegex.IsMatch(c.Value) &&
                    statements.All(statement => !HasOverlap(statement.Location, c.Location)
                    ));
            var fieldDefComments = GetFieldDefinitionComments(comments).ToArray();

            for (var i = 0; i < fieldDefComments.Length; i++)
            {
                var c = fieldDefComments[i];
                var nextStatement = statements.FirstOrDefault(s => s.Location.Start > c.Location.Start);
                // 単一の const値の定義のみが許可される
                if (nextStatement is not VariableDeclaration variableDeclarations ||
                    variableDeclarations.Kind is not VariableDeclarationKind.Const ||
                    variableDeclarations.Declarations.Count != 1
                   )
                {
                    continue;
                }

                // 下記ような順序の定義に対して // $field(int) を無視する
                // // @field(int)
                // // @field(float)
                // const x = 1.2;
                if (i < fieldDefComments.Length - 1 &&
                    fieldDefComments[i + 1].Location.Start < variableDeclarations.Location.Start
                   )
                {
                    continue;
                }

                var decl = variableDeclarations.Declarations[0];
                var fieldName = (decl.Id as Identifier)?.Name;
                // 基本的に起こらないが念のため
                if (string.IsNullOrEmpty(fieldName))
                {
                    continue;
                }

                var field = new ScriptExtensionField()
                {
                    FieldName = fieldName,
                    Type = c.Type,
                    FieldDefinedLocation = new FieldDefinedLocation()
                    {
                        startLine = decl.Location.Start.Line,
                        startColumn = decl.Location.Start.Column,
                        endLine = decl.Location.End.Line,
                        endColumn = decl.Location.End.Column,
                    },
                };

                if (decl.Init is { } initExpr)
                {
                    ApplyInitialValue(field, initExpr);
                }

                result.Add(field);
            }

            return result.ToArray();
        }

        private static List<FieldDefineComment> GetFieldDefinitionComments(IEnumerable<SyntaxComment> comments)
        {
            var result = new List<FieldDefineComment>();
            foreach (var comment in comments)
            {
                if (FieldMetaCommentRegex.Match(comment.Value) is not { Success: true } metaMatch)
                {
                    continue;
                }

                var typeName = metaMatch.Groups[1].Captures[0].Value.ToLower();
                if (TypeNames.TryGetValue(typeName, out var type))
                {
                    result.Add(new FieldDefineComment(comment.Location, type));
                }
            }

            return result;
        }

        private static void ApplyInitialValue(ScriptExtensionField target, Expression expr)
        {
            target.ResetInitialValues();
            switch (target.Type)
            {
                case ExtensionFieldType.Bool:
                    if (LiteralParser.TryParseBool(expr, out var boolInitialValue))
                    {
                        target.BoolInitialValue = boolInitialValue;
                    }
                    break;
                case ExtensionFieldType.Int:
                    if (LiteralParser.TryParseInt(expr, out var intInitialValue))
                    {
                        target.IntInitialValue = intInitialValue;
                    }
                    break;
                case ExtensionFieldType.Float:
                    if (LiteralParser.TryParseFloat(expr, out var floatInitialValue))
                    {
                        target.FloatInitialValue = floatInitialValue;
                    }
                    break;
                case ExtensionFieldType.String:
                    if (LiteralParser.TryParseString(expr, out var stringInitialValue))
                    {
                        target.StringInitialValue = stringInitialValue;
                    }
                    break;
                case ExtensionFieldType.Vector2:
                    if (LiteralParser.TryParseVector2(expr, out var vector2InitialValue))
                    {
                        target.Vector2InitialValue = vector2InitialValue;
                    }
                    break;
                case ExtensionFieldType.Vector3:
                    if (LiteralParser.TryParseVector3(expr, out var vector3InitialValue))
                    {
                        target.Vector3InitialValue = vector3InitialValue;
                    }
                    break;
                case ExtensionFieldType.Quaternion:
                    if (LiteralParser.TryParseQuaternion(expr, out var quaternionInitialValue))
                    {
                        target.QuaternionInitialValue = quaternionInitialValue;
                    }
                    break;
                default:
                    break;
            }
        }

        // 2つのLocationに範囲の重複があるかどうかを判定する
        private static bool HasOverlap(Location x, Location y)
        {
            if (x.Start <= y.Start && x.End >= y.Start) return true;
            if (x.Start <= y.End && x.End >= y.End) return true;

            if (y.Start <= x.Start && y.End >= x.Start) return true;
            if (y.Start <= x.End && y.End >= x.End) return true;

            return false;
        }
    }
}