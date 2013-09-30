using Codifier.Indent;
using Codifier.Parser;
using Codifier.Token;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Codifier.Context
{

    /* In which block the current token exists */
    public enum ParserContextType
    {
        T_CONTEXT_ROOT,
        T_CONTEXT_NAMESPACE,
        T_CONTEXT_CLASS,
        T_CONTEXT_INTERFACE,
        T_CONTEXT_STRUCT,
        T_CONTEXT_ENUM,
        T_CONTEXT_METHOD,
        T_CONTEXT_OPERATOR_METHOD,
        T_CONTEXT_PROPERTY,
        T_CONTEXT_IF,
        T_CONTEXT_ELSE_IF,
        T_CONTEXT_ELSE,
        T_CONTEXT_SWITCH,
        T_CONTEXT_SWITCH_CASE,
        T_CONTEXT_SWITCH_DEFAULT,
        T_CONTEXT_FOR,
        T_CONTEXT_FOREACH,
        T_CONTEXT_WHILE,
        T_CONTEXT_DO_WHILE,
        T_CONTEXT_TRY,
        T_CONTEXT_CATCH,
        T_CONTEXT_FINALLY,
        T_CONTEXT_UNSAFE,
        T_CONTEXT_LOCK,
        T_CONTEXT_UNCHEKED,
        T_CONTEXT_BLOCK_CONTEXT, /*any general block { .. }*/
        T_CONTEXT_DONE
    }
    

    public class ParserTokenContext
    {
        private ParserContextType context_type;
        public ParserContextType ContextType { get { return this.context_type; } set { this.context_type = value; } }
        //public TokenContextStatus token_context_status;
        private string context_name;
        public string ContextName { get { return this.context_name; } set { this.context_name = value; } }

        private string context_lexeme;
        public string ContextLexeme { get { return this.context_lexeme; } set { this.context_lexeme = value; } }

        private int context_start_line_number;
        public int ContextStartLineNumber { get { return this.context_start_line_number;}
            set { this.context_start_line_number = value; }
        }

        private int context_end_line_number;
        public int ContextEndLineNumber { get { return this.context_end_line_number; }
            set { this.context_end_line_number = value; }
        }

        private int context_start_position;
        public int ContextStartPosition
        {
            get { return this.context_start_position; }
            set { this.context_start_position = value; }
        }

        private int context_level; /* 0 for file context (root context) */
        public int ContextLevel { get { return this.context_level; }
            set { this.context_level = value; }
        }

        private ParserTokenContext parent_context;
        public ParserTokenContext ParentContext
        {
            get { return this.parent_context; }
            set { this.parent_context = value; }
        }

        private List<ParserTokenContext> children_contexts;
        public List<ParserTokenContext> ChildrenContexts;

        /*some context end with new line : like if without brackets */
        private bool is_context_end_with_new_line;
        public bool IsContextEndWithNewLine
        {
            get { return this.is_context_end_with_new_line; }
            set { this.is_context_end_with_new_line = value; }
        }

        public ParserTokenContext(ParserContextType context_type,int context_level, string context_name = null,
            string context_lexeme = null,ParserTokenContext parent_context = null)
        {
            this.context_type = context_type;
            this.context_name = context_name;
            this.context_lexeme = context_lexeme;
            this.parent_context = parent_context;

            this.context_level = context_level;
            this.context_start_line_number = 1;
            this.context_end_line_number = 1;
            this.context_start_position = 0;

            this.children_contexts = new List<ParserTokenContext>();

            if (parent_context != null)
                parent_context.addContext(this);
        }

        public int addContext(ParserTokenContext context)
        {
            int position = this.children_contexts.Count;
            context.parent_context = this;
            this.children_contexts.Add(context);
            return position;
        }

        public ParserTokenContext getContextByIndex(int index)
        {
            if (index >= this.children_contexts.Count)
                return null;

            return this.children_contexts[index];
        }

        public void removeChildrenContexts()
        {
            this.children_contexts.Clear();
        }


    }
    
    public class ContextParser
    {

        private Stack<ParserTokenContext> context_stack;
        public Stack<ParserTokenContext> ContextStack { get { return this.context_stack; } }

        private CodifierIndent indent;
        public CodifierIndent Indent { get { return this.indent; } }

        private bool error_flag;
        public bool ErrorFlag { get { return this.error_flag; } }

        private string error_message;
        public string ErrorMessage { get { return this.error_message; } 
            set { this.error_message = value;
        this.error_flag = true;
            }
        }
 
       // private int sub_context_start_line_number;
        private Codifier tokenizer;
        public Codifier Tokenizer { get { return this.tokenizer; } }

        private ParserTokenContext root_context;
        public ParserTokenContext RootContext { get { return this.root_context; } }

        public ContextParser(Codifier tokenizer)
        {
            this.tokenizer = tokenizer;
            this.error_flag = false;
            this.error_message = null;
         //   this.sub_context_start_line_number = 1;

            this.context_stack = new Stack<ParserTokenContext>();
            this.root_context = new ParserTokenContext
                (ParserContextType.T_CONTEXT_ROOT, 0, "ROOT", null);

            this.context_stack.Push(this.root_context);

            this.indent = new CodifierIndent();
        }      
       
       
        public CodifierToken nextToken()
        {
            CodifierToken token = null;
            
            while ((token = this.tokenizer.nextToken()).TokenType == CodifierTokenType.TT_TOKEN_WHITE_SPACE)
                this.indent.getTokenIndent(token);

            this.indent.getTokenIndent(token);

            if (token.TokenType == CodifierTokenType.TT_TOKEN_EOS && this.indent.ErrorFlag)
                this.ErrorMessage = this.indent.ErrorMessage;
            else if (token.TokenType == CodifierTokenType.TT_TOKEN_PUNCTUATOR_RIGHT_BRACKET)
                token.TokenContext = this.popContext(token);
            else
            {
                /*Check for unclosed context*/
                if (this.context_stack.Count > 0)
                {
                    ParserTokenContext tmp_ctx = this.context_stack.Peek();

                    if (token.TokenType == CodifierTokenType.TT_TOKEN_EOS &&
                        tmp_ctx.ContextType != ParserContextType.T_CONTEXT_ROOT)
                    {
                        this.parserError(token, string.Format("Unclosed {0}", tmp_ctx.ContextLexeme));
                    }
                }
                this.pushContext(token);
            }

            return token;
        }

        public bool pushContext(CodifierToken token)
        {
            switch (token.TokenType)
            {
                case CodifierTokenType.TT_TOKEN_KEYWORD_NAMESPACE:
                    {
                        token.TokenContext = this.context_stack.Peek();
                        return NamespaceContextParser.parse(token, this);
                    }
            }

            return false;
        }

        public ParserTokenContext popContext(CodifierToken token)
        {
            ParserTokenContext ctx = null;
            if (this.context_stack.Count > 0)
            {
                ctx = this.context_stack.Pop();
                ctx.ContextEndLineNumber = token.TokenLineNumber;
                ctx.IsContextEndWithNewLine = false;
            }

            return ctx;
        }

        public bool isContextToken(CodifierToken token)
        {
            if(token == null)
                return false;

            if (token.TokenType == CodifierTokenType.TT_TOKEN_KEYWORD_NAMESPACE ||
               token.TokenType == CodifierTokenType.TT_TOKEN_KEYWORD_CLASS ||
               token.TokenType == CodifierTokenType.TT_TOKEN_KEYWORD_INTERFACE ||
               token.TokenType == CodifierTokenType.TT_TOKEN_KEYWORD_STRUCT ||
               token.TokenType == CodifierTokenType.TT_TOKEN_KEYWORD_ENUM ||
               token.TokenType == CodifierTokenType.TT_TOKEN_KEYWORD_OPERATOR ||
               token.TokenType == CodifierTokenType.TT_TOKEN_KEYWORD_IF ||
               token.TokenType == CodifierTokenType.TT_TOKEN_KEYWORD_ELSE ||
               token.TokenType == CodifierTokenType.TT_TOKEN_KEYWORD_SWITCH ||
               token.TokenType == CodifierTokenType.TT_TOKEN_KEYWORD_CASE ||
               token.TokenType == CodifierTokenType.TT_TOKEN_KEYWORD_DEFAULT ||
               token.TokenType == CodifierTokenType.TT_TOKEN_KEYWORD_FOR ||
               token.TokenType == CodifierTokenType.TT_TOKEN_KEYWORD_FOREACH ||
               token.TokenType == CodifierTokenType.TT_TOKEN_KEYWORD_WHILE ||
               token.TokenType == CodifierTokenType.TT_TOKEN_KEYWORD_DO ||
               token.TokenType == CodifierTokenType.TT_TOKEN_KEYWORD_TRY ||
               token.TokenType == CodifierTokenType.TT_TOKEN_KEYWORD_CATCH ||
               token.TokenType == CodifierTokenType.TT_TOKEN_KEYWORD_FINALLY ||
               token.TokenType == CodifierTokenType.TT_TOKEN_KEYWORD_UNSAFE ||
               token.TokenType == CodifierTokenType.TT_TOKEN_KEYWORD_LOCK ||
               token.TokenType == CodifierTokenType.TT_TOKEN_KEYWORD_UNCHECKED ||
               token.TokenType == CodifierTokenType.TT_TOKEN_PUNCTUATOR_LEFT_BRACKET)
                return true;

            return false;
        }

     
        public void parserError(CodifierToken token,string error_message)
        {
            if (token != null)
            {
                this.ErrorMessage = string.Format("Parser Error : {0} at line : {1}, position {2}", error_message, token.TokenLineNumber,
                    token.TokenStartPosition);
            }
            else
            {
                this.ErrorMessage = error_message;
            }
        }
        public bool classContextDetector() { return true; }

    }

    //HoldContext(SuspendedContext| WaitingContext): Context detected but hasn't started yet (HAS STATUS -- CREATED -- COMPLETED -- WAIT_FOR_DATA --..)
    //ContextDetector : detects the token context after the comming of the token from the tokenizer.

    // ContextBuffer to save all information before create the context
    // Create Symbol Table to avoid duplication

    // ContextList (pop from ContextStack and add it to the list)

    //TokensHistory : Keep number of previous tokens
    // public int WelcomeToMe { ...}
    //public static int welcomeToMe(int c, char b){
    //}




    // CurrentContext : save the information of the comming context


}
