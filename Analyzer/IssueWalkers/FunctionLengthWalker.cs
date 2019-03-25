using System;
using System.Linq;
using Compiler.Walkers;
using System.Collections.Generic;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.FlowAnalysis;
using Microsoft.CodeAnalysis.Operations;

namespace Compiler.IssueWalkers
{
    // This is the skeleton of what a walker might look like
    internal class FunctionLenWalker : DefaultWalker
    {
        // This is you want to override a node for the AST, go look at the Documentation
        // folder to see which methods you can overrride
        public override void VisitMethodDeclaration(MethodDeclarationSyntax node)
        {
            // If the method does not have a body, ignore it's something like an interface
            if(node.Body == null) {
                return;
            }

            List<int> lines = new List<int>();
            TraverseAllChildren(node, lines);
            if(lines.Count >= 30)
            {
                var issue = new Issue(IssueType.FunctionTooBig, node);
                IssueReporter.Instance.AddIssue(issue);
            }

            Console.WriteLine(node.GetLocation().ToString());

            // When you detect an issue, you can report it by doing this :


            base.VisitMethodDeclaration(node);
        }

        private void TraverseAllChildren(SyntaxNode node, List<int> lines)
        {
            var position = Helper.ExtractPosition(node);
            if(lines.FindIndex(el => el == position.lineNumber) != -1)
            {
                lines.Add(position.lineNumber);
            }
            node.ChildNodesAndTokens().ToList().ForEach(element => {
                //Get the location
                if(element.IsToken)
                {
                    //int line = node.GetText().Lines.GetLineFromPosition(element.AsToken().Span.Start).LineNumber;
                    int line = element.GetLocation().GetLineSpan().StartLinePosition.Line;
                    if(lines.FindIndex(el => el == line) != -1)
                    {
                        lines.Add(line);
                    }
                }
                if(element.IsNode)
                {
                    TraverseAllChildren(element.AsNode(), lines);
                }
            });
        }
        internal override void PreExecute() {
            // You want to call the following line! It will tell the correction system
            // that you want to get a a score for this kind of issue. Without it 
            // you won't be evaluated!
            IssueReporter.Instance.EnableIssueType(IssueType.FunctionTooBig);


            // If you need to execute more code before visiting the AST
        }

        internal override void PostExecute() {
            
            // If you need to execute more code after visiting the AST 

            // For example you stored some nodes during the visit and now you
            // want to do some kind of logic on them and finally report your 
            // issues
        }
    }
}
