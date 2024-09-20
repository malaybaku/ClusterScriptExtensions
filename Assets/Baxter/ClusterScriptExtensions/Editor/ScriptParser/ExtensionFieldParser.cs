using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Baxter.ClusterScriptExtensions.Editor.Localization;
using Esprima;
using Esprima.Ast;
using UnityEngine;

namespace Baxter.ClusterScriptExtensions.Editor.ScriptParser
{
    public static class ExtensionFieldParser
    {
        private static readonly Regex FieldMetaCommentRegex = new(@"^\s*@field\(([^\)]+)\)");
        
        // FieldMetaCommentの検出後に追加で調べに行くattrっぽいやつ
        private static readonly Regex RangeRegex = new(@"@range\(([^\)]+)\)");
        private const string TextAreaAttrLiteral = "@textArea";

        private readonly struct FieldDefineComment
        {
            public readonly Location Location;
            public readonly ExtensionFieldType Type;

            public readonly bool HasRange;
            public readonly float RangeMin;
            public readonly float RangeMax;

            public readonly bool UseTextArea;

            FieldDefineComment(Location location, ExtensionFieldType type,
                bool hasRange, float rangeMin, float rangeMax,
                bool useTextArea
                )
            {
                Location = location;
                Type = type;

                HasRange = hasRange;
                RangeMin = rangeMin;
                RangeMax = rangeMax;

                UseTextArea = useTextArea;
            }

            public static FieldDefineComment Create(Location location, ExtensionFieldType type) => new(
                location, type,
                false, 0f, 0f,
                false
            );

            public FieldDefineComment WithRange(float min, float max) => new(
                Location, Type,
                true, min, max,
                UseTextArea
            );

            public FieldDefineComment WithUseTextArea() => new(
                Location, Type,
                HasRange, RangeMin, RangeMax,
                true
            );
        }

        private static readonly Dictionary<string, ExtensionFieldType> TypeNames = new()
        {
            ["auto"] = ExtensionFieldType.Unknown,
            ["bool"] = ExtensionFieldType.Bool,
            ["int"] = ExtensionFieldType.Int,
            ["float"] = ExtensionFieldType.Float,
            ["string"] = ExtensionFieldType.String,
            ["vector2"] = ExtensionFieldType.Vector2,
            ["vector3"] = ExtensionFieldType.Vector3,
            ["quaternion"] = ExtensionFieldType.Quaternion,
            ["audioclip"] = ExtensionFieldType.AudioClip,
            ["humanoidanimation"] = ExtensionFieldType.HumanoidAnimation,
            ["worlditem"] = ExtensionFieldType.WorldItem,
            ["worlditemtemplate"] = ExtensionFieldType.WorldItemTemplate,
            ["material"] = ExtensionFieldType.Material,
        };

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

                // 下記のような順序の定義に対して // $field(int) を無視する
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

                var fieldType = c.Type;
                
                // 式全体から型推定をすべき場合はそうする & それが失敗した場合は無効なフィールドとする
                if (c.Type is ExtensionFieldType.Unknown && 
                    decl.Init is { } initNode)
                {
                    var estimateSuccess = ScriptNodeParser.TryEstimateExtensionFieldType(initNode, out var estimatedType);
                    if (estimateSuccess)
                    {
                        fieldType = estimatedType;
                    }
                    else
                    {
                        Debug.LogWarning(Texts.Get(Keys.ScriptExtensionField_Warning_FailedToEstimateType, fieldName));
                        continue;
                    }
                } 
                
                var field = new ScriptExtensionField()
                {
                    FieldName = fieldName,
                    Type = fieldType,
                    FieldDefinedLocation = new FieldDefinedLocation()
                    {
                        startLine = decl.Location.Start.Line,
                        startColumn = decl.Location.Start.Column,
                        endLine = decl.Location.End.Line,
                        endColumn = decl.Location.End.Column,
                    },
                    HasRange = c.HasRange,
                    RangeMin = c.RangeMin,
                    RangeMax = c.RangeMax,
                    UseTextArea = c.UseTextArea,
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
                if (!TypeNames.TryGetValue(typeName, out var type))
                {
                    continue;
                }

                var definition = FieldDefineComment.Create(comment.Location, type);
                definition = ApplyAdditionalAttributes(definition, comment);

                result.Add(definition);
            }

            return result;
        }

        private static void ApplyInitialValue(ScriptExtensionField target, Expression expr)
        {
            //NOTE: アセット参照の型に対しては何もしないでOK (既定値は常にnullにしてよい)
            target.ResetInitialValues();
            switch (target.Type)
            {
                case ExtensionFieldType.Bool:
                    if (ScriptNodeParser.TryParseBool(expr, out var boolInitialValue))
                    {
                        target.BoolInitialValue = boolInitialValue;
                    }
                    break;
                case ExtensionFieldType.Int:
                    if (ScriptNodeParser.TryParseInt(expr, out var intInitialValue))
                    {
                        target.IntInitialValue = intInitialValue;
                    }
                    break;
                case ExtensionFieldType.Float:
                    if (ScriptNodeParser.TryParseFloat(expr, out var floatInitialValue))
                    {
                        target.FloatInitialValue = floatInitialValue;
                    }
                    break;
                case ExtensionFieldType.String:
                    if (ScriptNodeParser.TryParseString(expr, out var stringInitialValue))
                    {
                        target.StringInitialValue = stringInitialValue;
                    }
                    break;
                case ExtensionFieldType.Vector2:
                    if (ScriptNodeParser.TryParseVector2(expr, out var vector2InitialValue))
                    {
                        target.Vector2InitialValue = vector2InitialValue;
                    }
                    break;
                case ExtensionFieldType.Vector3:
                    if (ScriptNodeParser.TryParseVector3(expr, out var vector3InitialValue))
                    {
                        target.Vector3InitialValue = vector3InitialValue;
                    }
                    break;
                case ExtensionFieldType.Quaternion:
                    if (ScriptNodeParser.TryParseQuaternion(expr, out var quaternionInitialValue))
                    {
                        target.QuaternionInitialValue = quaternionInitialValue;
                    }
                    break;
                default:
                    break;
            }
        }

        // @field以外で追加で定義した値があるか確認してパースする。
        // 現時点ではRegexで収まるくらいの処理しかしていないが、必要ならコメントの内容自体をEsprimaで読みだすことを検討してもOK
        private static FieldDefineComment ApplyAdditionalAttributes(FieldDefineComment source, SyntaxComment comment)
        {
            var result = source;

            if (RangeRegex.Match(comment.Value) is { Success: true } rangeMatch)
            {
                var rangeContents = rangeMatch.Groups[1].Captures[0].Value.Split(',');
                if (rangeContents.Length == 2 &&
                    float.TryParse(rangeContents[0], out var min) &&
                    float.TryParse(rangeContents[1], out var max) && 
                    min < max)
                {
                    result = result.WithRange(min, max);
                }
            }
                
            if (comment.Value.Contains(TextAreaAttrLiteral))
            {
                result = result.WithUseTextArea();
            }

            return result;
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