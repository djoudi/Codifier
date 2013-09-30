using Codifier.Context;
using Codifier.Token;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Codifier.Parser
{
    public class NamespaceContextParser : ParserTokenContext
    {
        private string namespace_id;
        public string NamespaceId { get { return this.namespace_id; } set { this.namespace_id = value; } }

        public NamespaceContextParser(string namespace_id, int context_level, string context_name, ParserTokenContext parent_context) :
            base(ParserContextType.T_CONTEXT_NAMESPACE, context_level, context_name, "namespace", parent_context)
        {
            this.namespace_id = namespace_id;
        }

        public static bool parse(CodifierToken token, ContextParser parser)
        {
            ParserTokenContext parent_ctx = parser.ContextStack.Peek();
            if (parent_ctx == null)
                return false;

            CodifierToken current_token = parser.nextToken();
            NamespaceContextParser namespace_ctx = null;

            /* To help to detect the name of the namespace with (dot(s))*/

            bool previous_token_was_id = false;
            string namespace_name_buffer = "";

            while (current_token.TokenType != CodifierTokenType.TT_TOKEN_EOS &&
                current_token.TokenType != CodifierTokenType.TT_TOKEN_ERROR)
            {
                if (current_token.TokenType == CodifierTokenType.TT_TOKEN_PUNCTUATOR_LEFT_BRACKET)
                {
                    if (!previous_token_was_id)
                        break; // invalid namespace

                    namespace_ctx = new NamespaceContextParser(namespace_name_buffer,parent_ctx.ContextLevel + 1,
                     "namespace", parent_ctx);

                    namespace_ctx.ContextStartLineNumber = token.TokenLineNumber;
                    namespace_ctx.ContextStartPosition = token.TokenStartPosition;

                    /*make name space the current parent context*/
                    current_token.TokenContext = parent_ctx; // for left bracket
                    parser.ContextStack.Push(namespace_ctx);
                    System.Console.WriteLine(namespace_name_buffer);
                    return true;
                }
                else
                {
                    if (current_token.TokenType == CodifierTokenType.TT_TOKEN_ID)
                    {
                        namespace_name_buffer += current_token.TokenLexeme;
                        previous_token_was_id = true;
                    }
                    else if (current_token.TokenType == CodifierTokenType.TT_TOKEN_PUNCTUATOR_DOT)
                    {
                        if (!previous_token_was_id)
                            break; // invalid namespace

                        previous_token_was_id = false;
                        namespace_name_buffer += current_token.TokenLexeme;
                    }
                    else if (current_token.TokenType == CodifierTokenType.TT_TOKEN_PUNCTUATOR_SEMI_COLON) /*namespace without body*/
                    {
                        if (!previous_token_was_id)
                        {
                            parser.parserError(current_token, "Invalid namespace name");
                            return false;
                        }

                        current_token.TokenContext = parent_ctx;
                        return true;
                    }
                    else
                    {
                        if (parser.isContextToken(current_token))
                        {
                            parser.parserError(current_token, string.Format("Invalid use of '{0}' keyword", current_token.TokenLexeme));
                            return false;
                        }
                        else
                            break; // error

                    }
                }

                current_token.TokenContext = parent_ctx;
                current_token = parser.nextToken();
            }

            if (current_token.TokenType == CodifierTokenType.TT_TOKEN_ERROR)
            {
                parser.ErrorMessage = current_token.TokenLexeme;
                return false;
            }

            parser.parserError(token, "Invalid namespace");
            return false;
        }
    }
}
