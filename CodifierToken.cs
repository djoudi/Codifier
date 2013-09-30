using Codifier.Context;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Codifier.Token
{
    
    public enum CodifierTokenType
    {
        /*CAUTION: Don't change the order of the tokens : 
           The (findTokenCategory) method try to find the token's category by play with the token's POSITION
         * in the CodifierTokenType enumeration
         */
        TT_TOKEN_LITERAL_UNICODE_SHORT_CHARACTER = 1,// CAUTION : KEEP [=1] UNCHANGED // \uxxxx 4 digits
        TT_TOKEN_LITERAL_UNICODE_LONG_CHARACTER,  // \Uxxxxxxxx 8 digits
        TT_TOKEN_LITERAL_CHARACTER,        // 'a' '\x33' ..
        TT_TOKEN_LITERAL_INTEGER_DECIMAL,
        TT_TOKEN_LITERAL_INTEGER_HEXADECIMAL,
        TT_TOKEN_LITERAL_REAL_WITH_POINT,
        TT_TOKEN_LITERAL_REAL_WITH_EXPONENT,
        TT_TOKEN_LITERAL_REGULAR_STRING,
        TT_TOKEN_LITERAL_VERBATIM_STRING,
        TT_TOKEN_ID,                      // Identifier
        TT_TOKEN_KEYWORD_ABSTRACT,
        TT_TOKEN_KEYWORD_AS,
        TT_TOKEN_KEYWORD_BASE,
        TT_TOKEN_KEYWORD_BOOL,
        TT_TOKEN_KEYWORD_BREAK,
        TT_TOKEN_KEYWORD_BYTE,
        TT_TOKEN_KEYWORD_CASE,
        TT_TOKEN_KEYWORD_CATCH,
        TT_TOKEN_KEYWORD_CHAR,
        TT_TOKEN_KEYWORD_CHECKED,
        TT_TOKEN_KEYWORD_CLASS,
        TT_TOKEN_KEYWORD_CONST,
        TT_TOKEN_KEYWORD_CONTINUE,
        TT_TOKEN_KEYWORD_DECIMAL,
        TT_TOKEN_KEYWORD_DEFAULT,
        TT_TOKEN_KEYWORD_DELEGATE,
        TT_TOKEN_KEYWORD_DO,
        TT_TOKEN_KEYWORD_DOUBLE,
        TT_TOKEN_KEYWORD_ELSE,
        TT_TOKEN_KEYWORD_ENUM,
        TT_TOKEN_KEYWORD_EVENT,
        TT_TOKEN_KEYWORD_EXPLICIT,
        TT_TOKEN_KEYWORD_EXTERN,
        TT_TOKEN_KEYWORD_FALSE,
        TT_TOKEN_KEYWORD_FINALLY,
        TT_TOKEN_KEYWORD_FIXED,
        TT_TOKEN_KEYWORD_FLOAT,
        TT_TOKEN_KEYWORD_FOR,
        TT_TOKEN_KEYWORD_FOREACH,
        TT_TOKEN_KEYWORD_GOTO,
        TT_TOKEN_KEYWORD_IF,
        TT_TOKEN_KEYWORD_IMPLICIT,
        TT_TOKEN_KEYWORD_IN,
        TT_TOKEN_KEYWORD_INT,
        TT_TOKEN_KEYWORD_INTERFACE,
        TT_TOKEN_KEYWORD_INTERNAL,
        TT_TOKEN_KEYWORD_IS,
        TT_TOKEN_KEYWORD_LOCK,
        TT_TOKEN_KEYWORD_LONG,
        TT_TOKEN_KEYWORD_NAMESPACE,
        TT_TOKEN_KEYWORD_NEW,
        TT_TOKEN_KEYWORD_NULL,
        TT_TOKEN_KEYWORD_OBJECT,
        TT_TOKEN_KEYWORD_OPERATOR,
        TT_TOKEN_KEYWORD_OUT,
        TT_TOKEN_KEYWORD_OVERRIDE,
        TT_TOKEN_KEYWORD_PARAMS,
        TT_TOKEN_KEYWORD_PARTIAL,
        TT_TOKEN_KEYWORD_PRIVATE,
        TT_TOKEN_KEYWORD_PROTECTED,
        TT_TOKEN_KEYWORD_PUBLIC,
        TT_TOKEN_KEYWORD_READONLY,
        TT_TOKEN_KEYWORD_REF,
        TT_TOKEN_KEYWORD_RETURN,
        TT_TOKEN_KEYWORD_SBYTE,
        TT_TOKEN_KEYWORD_SEALED,
        TT_TOKEN_KEYWORD_SHORT,
        TT_TOKEN_KEYWORD_SIZEOF,
        TT_TOKEN_KEYWORD_STACKALLOC,
        TT_TOKEN_KEYWORD_STATIC,
        TT_TOKEN_KEYWORD_STRING,
        TT_TOKEN_KEYWORD_STRUCT,
        TT_TOKEN_KEYWORD_SWITCH,
        TT_TOKEN_KEYWORD_THIS,
        TT_TOKEN_KEYWORD_THROW,
        TT_TOKEN_KEYWORD_TRUE,
        TT_TOKEN_KEYWORD_TRY,
        TT_TOKEN_KEYWORD_TYPEOF,
        TT_TOKEN_KEYWORD_UINT,
        TT_TOKEN_KEYWORD_ULONG,
        TT_TOKEN_KEYWORD_UNCHECKED,
        TT_TOKEN_KEYWORD_UNSAFE,
        TT_TOKEN_KEYWORD_USHORT,
        TT_TOKEN_KEYWORD_USING,
        TT_TOKEN_KEYWORD_VIRTUAL,
        TT_TOKEN_KEYWORD_VOLATILE,
        TT_TOKEN_KEYWORD_VOID,
        TT_TOKEN_KEYWORD_WHILE,
        TT_TOKEN_OPERATOR_EQUAL,
        TT_TOKEN_OPERATOR_EQUAL_EQUAL,
        TT_TOKEN_OPERATOR_LESS_THAN,
        TT_TOKEN_OPERATOR_LESS_THAN_OR_EQUAL,
        TT_TOKEN_OPERATOR_SHIFT_LEFT,
        TT_TOKEN_OPERATOR_SHIFT_LEFT_EQUAL,
        TT_TOKEN_OPERATOR_GREATER_THAN,
        TT_TOKEN_OPERATOR_GREATER_THAN_OR_EQUAL,
        TT_TOKEN_OPERATOR_SHIFT_RIGHT,
        TT_TOKEN_OPERATOR_SHIFT_RIGHT_EQUAL,
        TT_TOKEN_OPERATOR_QUESTION_MARK,
        TT_TOKEN_OPERATOR_PLUS,
        TT_TOKEN_OPERATOR_PLUS_PLUS,
        TT_TOKEN_OPERATOR_PLUS_EQUAL,
        TT_TOKEN_OPERATOR_MINUS,
        TT_TOKEN_OPERATOR_MINUS_MINUS,
        TT_TOKEN_OPERATOR_MINUS_EQUAL,
        TT_TOKEN_OPERATOR_MINUS_GREATER_THAN,
        TT_TOKEN_OPERATOR_BITWISE_AND,
        TT_TOKEN_OPERATOR_BITWISE_AND_EQUAL,
        TT_TOKEN_OPERATOR_LOGICAL_AND,
        TT_TOKEN_OPERATOR_BITWISE_OR,
        TT_TOKEN_OPERATOR_BITWISE_OR_EQUAL,
        TT_TOKEN_OPERATOR_LOGICAL_OR,
        TT_TOKEN_OPERATOR_NOT,
        TT_TOKEN_OPERATOR_NOT_EQUAL,
        TT_TOKEN_OPERATOR_MULTIPLY,
        TT_TOKEN_OPERATOR_MULTIPLY_EQUAL,
        TT_TOKEN_OPERATOR_DIVIDE,
        TT_TOKEN_OPERATOR_DIVIDE_EQUAL,
        TT_TOKEN_OPERATOR_PERCENT,
        TT_TOKEN_OPERATOR_PERCENT_EQUAL,
        TT_TOKEN_OPERATOR_CARET, // ^
        TT_TOKEN_OPERATOR_CARET_EQUAL, // ^=
        TT_TOKEN_PUNCTUATOR_DOT,
        TT_TOKEN_PUNCTUATOR_SEMI_COLON,
        TT_TOKEN_PUNCTUATOR_COLON,
        TT_TOKEN_PUNCTUATOR_COMMA,
        TT_TOKEN_PUNCTUATOR_LEFT_BRACKET,
        TT_TOKEN_PUNCTUATOR_RIGHT_BRACKET,
        TT_TOKEN_PUNCTUATOR_LEFT_SQUARE_BRACKET,
        TT_TOKEN_PUNCTUATOR_RIGHT_SQUARE_BRACKET,
        TT_TOKEN_PUNCTUATOR_LEFT_PARNETHESES,
        TT_TOKEN_PUNCTUATOR_RIGHT_PARNETHESES,
        TT_TOKEN_INLINE_COMMENT,
        TT_TOKEN_MULTI_LINES_COMMENT,
        TT_TOKEN_WHITE_SPACE,
        TT_TOKEN_EOS,
        TT_TOKEN_ERROR
    }

    public enum CodifierTokenCategory
    {
        CODIFIER_TOKEN_CATEGORY_ID,
        CODIFIER_TOKEN_CATEGORY_KEYWORD,
        CODIFIER_TOKEN_CATEGORY_LITERAL,
        CODIFIER_TOKEN_CATEGORY_OPERATOR,
        CODIFIER_TOKEN_CATEGORY_PUNCTUATOR,
        CODIFIER_TOKEN_CATEGORY_COMMENT,
        CODIFIER_TOKEN_CATEGORY_WHITE_SPACE,
        CODIFIER_TOKEN_CATEGORY_EOS,
        CODIFIER_TOKEN_CATEGORY_ERROR
    }

    public delegate void TokenApplyActionCallback(CodifierToken token, out object return_value, params object[] parameters);

    public class CodifierToken
    {
        private CodifierTokenType token_type;
        private string token_lexeme;
        private int token_line_number;
        private int token_start_position;
        private CodifierTokenCategory token_category;
        private ParserTokenContext token_context;
        private TokenApplyActionCallback apply_action_callback;

        //public delegate object onTokenDetected(Token token);
        //public delegate object onTokenContextDetected(TokenContext token_context,Token token);

        public TokenApplyActionCallback ApplyActionCallback
        {
            get { return this.apply_action_callback; }
            set { this.apply_action_callback = value; }
        }

        public CodifierTokenType TokenType { get { return this.token_type; } }
        public string TokenLexeme { get { return this.token_lexeme; } set { this.token_lexeme = value; } }
        public int TokenLineNumber { get { return this.token_line_number; } }
        public int TokenStartPosition { get { return this.token_start_position; } }
        public CodifierTokenCategory TokenCategory { get { return this.token_category; }}
        public ParserTokenContext TokenContext { get { return this.token_context; } set { this.token_context = value; } }

        public CodifierToken(CodifierTokenType type, string lexeme = null, int line_number = 0, int start_position = -1, ParserTokenContext context = null)
        {
            this.token_type = type;
            this.token_lexeme = lexeme;
            this.token_line_number = line_number;
            this.token_start_position = start_position;
            this.token_context = context;
            this.findTokenCategory();
        }

        protected void findTokenCategory()
        {
            int token_type_value = (int)this.token_type;
            if (token_type_value >= 1 && token_type_value <= 9) // literal token
                this.token_category = CodifierTokenCategory.CODIFIER_TOKEN_CATEGORY_LITERAL;
            else if (token_type_value == 10) // id token
                this.token_category = CodifierTokenCategory.CODIFIER_TOKEN_CATEGORY_ID;
            else if (token_type_value >= 11 && token_type_value <= 88) // keyword token
                this.token_category = CodifierTokenCategory.CODIFIER_TOKEN_CATEGORY_KEYWORD;
            else if (token_type_value >= 89 && token_type_value <= 122) // keyword token
                this.token_category = CodifierTokenCategory.CODIFIER_TOKEN_CATEGORY_OPERATOR;
            else if (token_type_value >= 123 && token_type_value <= 132) // punctuator token
                this.token_category = CodifierTokenCategory.CODIFIER_TOKEN_CATEGORY_PUNCTUATOR;
            else if (token_type_value >= 133 && token_type_value <= 134) // comment token
                this.token_category = CodifierTokenCategory.CODIFIER_TOKEN_CATEGORY_COMMENT;
            else if (token_type_value == 135) // white_space token
                this.token_category = CodifierTokenCategory.CODIFIER_TOKEN_CATEGORY_WHITE_SPACE;
            else if (token_type_value == 136) // eos: end of source token
                this.token_category = CodifierTokenCategory.CODIFIER_TOKEN_CATEGORY_EOS;
            else if (token_type_value == 137) // error token
                this.token_category = CodifierTokenCategory.CODIFIER_TOKEN_CATEGORY_ERROR;

        }

        public string findTokenCategoryAsString()
        {
            switch (this.token_category)
            {
                case CodifierTokenCategory.CODIFIER_TOKEN_CATEGORY_LITERAL:
                    return "LITERAL";
                case CodifierTokenCategory.CODIFIER_TOKEN_CATEGORY_ID:
                    return "ID";
                case CodifierTokenCategory.CODIFIER_TOKEN_CATEGORY_KEYWORD:
                    return "KEYWORD";
                case CodifierTokenCategory.CODIFIER_TOKEN_CATEGORY_OPERATOR:
                    return "OPERATOR";
                case CodifierTokenCategory.CODIFIER_TOKEN_CATEGORY_PUNCTUATOR:
                    return "PUNCTUATOR";
                case CodifierTokenCategory.CODIFIER_TOKEN_CATEGORY_COMMENT:
                    return "COMMENT";
                case CodifierTokenCategory.CODIFIER_TOKEN_CATEGORY_WHITE_SPACE:
                    return "WHITE_SPACE";
                case CodifierTokenCategory.CODIFIER_TOKEN_CATEGORY_EOS:
                    return "EOS";
                case CodifierTokenCategory.CODIFIER_TOKEN_CATEGORY_ERROR:
                    return "ERROR";
                
                default:
                    return "UNKNOWN";
            }
        }

        public void applyCallBack(out object return_value, params object[] parameters)
        {
            return_value = null;
            if (this.apply_action_callback != null)
                this.apply_action_callback(this, out return_value, parameters);
        }
    }
}
