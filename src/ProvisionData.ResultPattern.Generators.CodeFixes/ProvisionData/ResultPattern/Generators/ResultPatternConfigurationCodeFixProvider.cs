// Provision Data Application Framework
// Copyright (C) 2026 Provision Data Systems Inc.
//
// This program is free software: you can redistribute it and/or modify it under the terms of
// the GNU Affero General Public License as published by the Free Software Foundation, either
// version 3 of the License, or (at your option) any later version.
//
// This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY
// without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.
// See the GNU Affero General Public License for more details.
//
// You should have received a copy of the GNU Affero General Public License along with this
// program. If not, see <https://www.gnu.org/licenses/>.

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Collections.Immutable;
using System.Composition;

namespace ProvisionData.ResultPattern.Generators;

[Shared]
[ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(ResultPatternConfigurationCodeFixProvider))]
public sealed class ResultPatternConfigurationCodeFixProvider : CodeFixProvider
{
    public override ImmutableArray<String> FixableDiagnosticIds => [ResultPatternDiagnosticDefinitions.MissingAddResultPatternId];

    public override FixAllProvider GetFixAllProvider()
        => WellKnownFixAllProviders.BatchFixer;

    public override async Task RegisterCodeFixesAsync(CodeFixContext context)
    {
        var root = await context.Document.GetSyntaxRootAsync(context.CancellationToken).ConfigureAwait(false);
        if (root is null)
        {
            return;
        }

        // Heuristic: find "builder.Services" or "services" in Program.cs
        var servicesAccess = root.DescendantNodes()
            .OfType<MemberAccessExpressionSyntax>()
            .FirstOrDefault(ma =>
                ma.Name.Identifier.Text == "Services" &&
                ma.Expression is IdentifierNameSyntax { Identifier.Text: "builder" });

        if (servicesAccess is null)
        {
            return;
        }

        context.RegisterCodeFix(
            CodeAction.Create(
                "Call AddResultPattern()",
                ct => AddCallAsync(context.Document, root, servicesAccess, ct),
                nameof(ResultPatternConfigurationCodeFixProvider)),
            context.Diagnostics);
    }

    private static Task<Document> AddCallAsync(
        Document document,
        SyntaxNode root,
        MemberAccessExpressionSyntax servicesAccess,
        CancellationToken cancellationToken)
    {
        // builder.Services.AddResultPattern();
        var invocation = SyntaxFactory.ExpressionStatement(
            SyntaxFactory.InvocationExpression(
                SyntaxFactory.MemberAccessExpression(
                    SyntaxKind.SimpleMemberAccessExpression,
                    servicesAccess,
                    SyntaxFactory.IdentifierName("AddResultPattern")))
            .WithArgumentList(SyntaxFactory.ArgumentList()));

        var lastStatement = servicesAccess.FirstAncestorOrSelf<StatementSyntax>();
        if (lastStatement is null)
        {
            return Task.FromResult(document);
        }

        var newRoot = root.InsertNodesAfter(lastStatement, [invocation.WithTrailingTrivia(SyntaxFactory.ElasticCarriageReturnLineFeed)]);

        return Task.FromResult(document.WithSyntaxRoot(newRoot));
    }
}
