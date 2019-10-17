using Irony.Parsing;
using System;
using XLParser;

namespace Cells.Common.Spreadsheet.Expression
{
    public abstract class Visitor<T>
    {
        protected virtual T VisitFormula(ParseTreeNode node)
        {
            return Visit(node.ChildNodes[0]);
        }

        protected virtual T VisitConstant(ParseTreeNode node)
        {
            return Visit(node.ChildNodes[0]);
        }

        protected virtual T VisitNumber(ParseTreeNode node)
        {
            return Visit(node.ChildNodes[0]);
        }

        protected virtual T VisitReference(ParseTreeNode node)
        {
            return Visit(node.ChildNodes[0]);
        }

        protected virtual T VisitNamedRange(ParseTreeNode node)
        {
            return Visit(node.ChildNodes[0]);
        }

        protected virtual T VisitArgument(ParseTreeNode node)
        {
            return Visit(node.ChildNodes[0]);
        }

        protected virtual T VisitCell(ParseTreeNode node)
        {
            return Visit(node.ChildNodes[0]);
        }

        protected abstract T VisitFunctionCall(ParseTreeNode node);

        protected abstract T VisitReferenceFunctionCall(ParseTreeNode node);

        protected abstract T VisitArguments(ParseTreeNode node);

        protected abstract T VisitNumberToken(ParseTreeNode node);

        protected abstract T VisitNameToken(ParseTreeNode node);

        protected abstract T VisitCellToken(ParseTreeNode node);

        protected virtual T Visit(ParseTreeNode node)
        {
            switch (node.Term)
            {
                case NonTerminal nonTerminal when nonTerminal.Name == nameof(ExcelFormulaGrammar.Formula): return VisitFormula(node);
                case NonTerminal nonTerminal when nonTerminal.Name == nameof(ExcelFormulaGrammar.FunctionCall): return VisitFunctionCall(node);
                case NonTerminal nonTerminal when nonTerminal.Name == nameof(ExcelFormulaGrammar.Constant): return VisitConstant(node);
                case NonTerminal nonTerminal when nonTerminal.Name == nameof(ExcelFormulaGrammar.Number): return VisitNumber(node);
                case NonTerminal nonTerminal when nonTerminal.Name == nameof(ExcelFormulaGrammar.Reference): return VisitReference(node);
                case NonTerminal nonTerminal when nonTerminal.Name == nameof(ExcelFormulaGrammar.ReferenceFunctionCall): return VisitReferenceFunctionCall(node);
                case NonTerminal nonTerminal when nonTerminal.Name == nameof(ExcelFormulaGrammar.NamedRange): return VisitNamedRange(node);
                case NonTerminal nonTerminal when nonTerminal.Name == nameof(ExcelFormulaGrammar.Arguments): return VisitArguments(node);
                case NonTerminal nonTerminal when nonTerminal.Name == nameof(ExcelFormulaGrammar.Argument): return VisitArgument(node);
                case NonTerminal nonTerminal when nonTerminal.Name == nameof(ExcelFormulaGrammar.Cell): return VisitCell(node);
                case Terminal terminal when terminal.Name == nameof(ExcelFormulaGrammar.NumberToken): return VisitNumberToken(node);
                case Terminal terminal when terminal.Name == nameof(ExcelFormulaGrammar.NameToken): return VisitNameToken(node);
                case Terminal terminal when terminal.Name == nameof(ExcelFormulaGrammar.CellToken): return VisitCellToken(node);
                default: throw new ArgumentException("node");
            }
        }
    }
}
