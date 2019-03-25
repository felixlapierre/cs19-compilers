using System;
using System.Linq;
using System.Text.RegularExpressions;
using Compiler.Walkers;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.FlowAnalysis;
using Microsoft.CodeAnalysis.Operations;

namespace Compiler.IssueWalkers
{
    // This is the skeleton of what a walker might look like
    internal class CommentedCodeWalker : DefaultWalker
    {
        public CommentedCodeWalker() : base(SyntaxWalkerDepth.Trivia)
        {

        }
        private Regex matchCode = new Regex(@"\);");
        // This is you want to override a node for the AST, go look at the Documentation
        // folder to see which methods you can overrride

        public override void VisitTrivia(SyntaxTrivia trivia)
        {
            // This will be executed for every Trivia
            if(trivia.Kind() == SyntaxKind.SingleLineCommentTrivia)
            {
                var comment = trivia.ToString();
                var matches = matchCode.Matches(comment);
                if(matches.Count > 0)
                {
                    var position = Helper.ExtractPosition(trivia);
                    var issue = new Issue(IssueType.CommentedCode, position.filepath, position.lineNumber);
                    IssueReporter.Instance.AddIssue(issue);
                }
            }

            base.VisitTrivia(trivia);
        }
        internal override void PreExecute() {
            // You want to call the following line! It will tell the correction system
            // that you want to get a a score for this kind of issue. Without it 
            // you won't be evaluated!
            IssueReporter.Instance.EnableIssueType(IssueType.CommentedCode);


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
