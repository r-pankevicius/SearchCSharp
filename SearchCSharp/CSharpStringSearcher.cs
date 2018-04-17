using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;
using System;
using System.IO;

namespace SearchCSharp
{
	class CSharpStringSearcher : CSharpSyntaxWalker
	{
		int _foundMatches;
		SyntaxTree _tree;
		Func<string, bool> _matchPredicate;

		public CSharpStringSearcher(Func<string, bool> matchPredicate)
		{
			Func<string, bool> matchAny = (s) => true;
			_matchPredicate = matchPredicate ?? matchAny;
		}

		public int SearchInFile(string filePath)
		{
			_foundMatches = 0;
			_tree = ParseFile(filePath);
			if (_tree != null)
			{
				Visit(_tree.GetRoot());
			}

			return _foundMatches;
		}

		public override void VisitConstantPattern(ConstantPatternSyntax node)
		{
			base.VisitConstantPattern(node);
		}

		public override void VisitInterpolatedStringText(InterpolatedStringTextSyntax node)
		{
			TryCapture(node);
			base.VisitInterpolatedStringText(node);
		}

		public override void VisitLiteralExpression(LiteralExpressionSyntax node)
		{
			SyntaxKind kind = node.Kind();
			if (kind == SyntaxKind.StringLiteralExpression)
				TryCapture(node);

			base.VisitLiteralExpression(node);
		}

		#region Implementation

		private void TryCapture(CSharpSyntaxNode node)
		{
			string nodeString = node.ToString();
			if (_matchPredicate(nodeString))
			{
				FileLinePositionSpan pos = _tree.GetLineSpan(node.Span);
				Console.WriteLine($"{pos.Path}({pos.StartLinePosition}): {nodeString}");
				_foundMatches++;
			}
		}

		private static SyntaxTree ParseFile(string path)
		{
			try
			{
				SyntaxTree tree;
				using (var stream = File.OpenRead(path))
				{
					tree = CSharpSyntaxTree.ParseText(SourceText.From(stream), path: path);
				}

				return tree;
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex.Message);
				return null;
			}
		}

		#endregion
	}
}