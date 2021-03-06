﻿using Mond.Compiler.Expressions;
using Mond.Compiler.Expressions.Statements;

namespace Mond.Compiler.Parselets.Statements
{
    class YieldParselet : IStatementParselet, IPrefixParselet
    {
        public Expression Parse(Parser parser, Token token, out bool trailingSemicolon)
        {
            trailingSemicolon = true;

            if (parser.MatchAndTake(TokenType.Break))
                return new YieldBreakExpression(token);

            return Parse(parser, token);
        }

        public Expression Parse(Parser parser, Token token)
        {
            // check next token to see if we could have a value
            var missingValue = parser.Match(TokenType.Semicolon) ||
                               parser.Match(TokenType.Comma) ||
                               parser.Match(TokenType.Dot) ||
                               parser.Match(TokenType.RightParen) ||
                               parser.Match(TokenType.RightBrace) ||
                               parser.Match(TokenType.RightSquare) ||
                               parser.Match(TokenType.Pipeline);

            var value = missingValue ? new UndefinedExpression(token) : parser.ParseExpession();
            return new YieldExpression(token, value);
        }
    }
}
