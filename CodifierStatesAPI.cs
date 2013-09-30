using Codifier.AbstractSource;
using Codifier.Helpers;
using Codifier.Token;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Codifier.Values;
using Codifier.KeywordsDictionary;

namespace Codifier.StatesAPI
{
    public static partial class CodifierStatesAPI
    {
        public static CodifierToken error(CodifierAbstractSource abstract_source)
        {
            if (abstract_source == null)
                return null;

            CodifierToken token = new CodifierToken(CodifierTokenType.TT_TOKEN_ERROR,
                   abstract_source.ErrorMessage,
                   abstract_source.LineNumber, abstract_source.LexemeStartPosition, null);
            return token;
        }

        /* current buffer content : LETTER or @LETTER or _ */
        public static CodifierToken idOrKeyword(CodifierAbstractSource abstract_source)
        {
            CodifierToken token = null;
            if (abstract_source == null)
                return token;

            int temp_line_number = abstract_source.LineNumber;

            if (abstract_source.IsEOS)
            {
                if (abstract_source.LexemeBuffer.Length > 1 &&
                !abstract_source.LexemeBuffer.Contains('@')) // \u|\U only
                {
                    abstract_source.ErrorMessage = "Invalid identifier name";
                    goto error_state;
                }
            }

            //(\u|\U) the identifier begin with a unicode character
            if (abstract_source.LexemeBuffer.Length > 1 &&
                !abstract_source.LexemeBuffer.Contains('@'))
            {
                if (!CodifierHelpMeTo.readUnicodeCharacter(abstract_source))
                    goto error_state;
            }


            while (abstract_source.readNextCharacter())
            {
                if (Char.IsLetterOrDigit(abstract_source.CurrentCharacter)
                    || (abstract_source.CurrentCharacter == '_'))
                    continue;
                else if (abstract_source.CurrentCharacter == '\\') // check if it is a unicode character
                {
                    if (abstract_source.IsEOS)
                    {
                        abstract_source.readPreviousCharacter();
                        break;
                    }

                    abstract_source.readNextCharacter();
                    if ((abstract_source.CurrentCharacter != 'u'
                        && abstract_source.CurrentCharacter != 'U'))
                    {
                        abstract_source.readPreviousCharacter();
                        abstract_source.readPreviousCharacter();

                        break;
                    }
                    else
                    {
                        /* read the unicode character*/
                        if (!CodifierHelpMeTo.readUnicodeCharacter(abstract_source))
                            goto error_state;
                    }
                }
                else
                {
                    abstract_source.readPreviousCharacter();
                    break;
                }
            }

            /* Optimization : to avoid search for keyword, we first check for @ prefix */
            if (abstract_source.LexemeBuffer[0] == '@')
                goto accepted_state;
            /* check if it is a keyword */
            else if (CodifierKeywordsDictionary.KeywordsDictionary.ContainsKey(abstract_source.LexemeBuffer)) // keyword
            {
                CodifierTokenType token_type = CodifierKeywordsDictionary.KeywordsDictionary[abstract_source.LexemeBuffer];
                token = new CodifierToken(token_type, abstract_source.LexemeBuffer, temp_line_number,
                    abstract_source.LexemeStartPosition, null);
                return token;
            }
            else /* Normal identifier */
                goto accepted_state;

        accepted_state:
            token = new CodifierToken(CodifierTokenType.TT_TOKEN_ID, abstract_source.LexemeBuffer, temp_line_number,
                abstract_source.LexemeStartPosition, null);
            return token;

        error_state:
            return CodifierStatesAPI.error(abstract_source);
        }
    
        /* State begin with @ character */
        public static CodifierToken at(CodifierAbstractSource abstract_source)
        {
            if (abstract_source.IsEOS)
            {
                abstract_source.ErrorMessage = @"Invalid use of @ character";
                goto error_state;
            }

            abstract_source.readNextCharacter();

            if (abstract_source.CurrentCharacter == '_' ||
            Char.IsLetter(abstract_source.CurrentCharacter))
                return CodifierStatesAPI.idOrKeyword(abstract_source);

            //verbatim string
            else if (abstract_source.CurrentCharacter == '\"')
                return CodifierStatesAPI.str(abstract_source);
            else
            {
                abstract_source.ErrorMessage = @"Invalid use of @ : There is no verbatim string nor identifer after @ character";
                goto error_state;
            }
            
        error_state:
            return CodifierStatesAPI.error(abstract_source);

        }

        /* State begin with \' character */
        public static CodifierToken singleQuote(CodifierAbstractSource abstract_source)
        {
            /* Eat ' character */
        //    abstract_source.readNextCharacter();
            if (abstract_source.IsEOS)
            {
                abstract_source.ErrorMessage = @"unclosed character value";
                goto error_state;
            }

            int temp_line_number = abstract_source.LineNumber;

            if (!CodifierHelpMeTo.readCharacter(abstract_source))
                goto error_state;

            abstract_source.readNextCharacter(); // must be \'
            
           if (abstract_source.CurrentCharacter == '\'')
            {
                return new CodifierToken(CodifierTokenType.TT_TOKEN_LITERAL_CHARACTER, abstract_source.LexemeBuffer,
                    temp_line_number, abstract_source.LexemeStartPosition, null);
            }
            else
            {
                abstract_source.ErrorMessage = @"Invalid character value";
                goto error_state;
            }

        error_state:
            return CodifierStatesAPI.error(abstract_source);
        }

        /* State begin with \\ character */
        public static CodifierToken backSlash(CodifierAbstractSource abstract_source)
        {
            if (abstract_source.IsEOS) // singl character \
            {
                abstract_source.ErrorMessage = @"Invalid character (\\)";
                goto error_state;
            }

            abstract_source.readNextCharacter();
            if (Char.ToUpper(abstract_source.CurrentCharacter) != 'U')
            {
                abstract_source.ErrorMessage = @"Invalid escape value";
                goto error_state;
            }
            else
                return CodifierStatesAPI.idOrKeyword(abstract_source);

        error_state:
            return CodifierStatesAPI.error(abstract_source);
        }

        /* State begin with any white space character */
        public static CodifierToken whiteSpace(CodifierAbstractSource abstract_source)
        {
            if (abstract_source == null)
                return null;

            int temp_line_number = abstract_source.LineNumber;
            //if (abstract_source.CurrentCharacter == '\n')
            //    temp_line_number--;

            abstract_source.eatWhiteSpaces();

            CodifierToken token = new CodifierToken(CodifierTokenType.TT_TOKEN_WHITE_SPACE,
                    abstract_source.LexemeBuffer, temp_line_number,
                    abstract_source.LexemeStartPosition, null);

            abstract_source.clearLexemBuffer();
            return token;
        }


        /* State begin with digit character */

        public static CodifierToken number(CodifierAbstractSource abstract_source)
        {
            if (abstract_source == null)
                return null;

            int temp_line_number = abstract_source.LineNumber;

            if (abstract_source.LexemeBuffer.StartsWith("."))
                goto after_dot_state;

            if (abstract_source.IsEOS)
                goto accept_digit;

            abstract_source.readNextCharacter();
            if (abstract_source.CurrentCharacter == '.')
                goto after_dot_state;
            if (!Char.IsDigit(abstract_source.CurrentCharacter) &&
                Char.ToUpper(abstract_source.CurrentCharacter) != 'X')
            {
                abstract_source.readPreviousCharacter();
                goto accept_digit;
            }

            /* begin check for hexadecimal */
            
            if (abstract_source.LexemeBuffer.Equals("0x") ||
                abstract_source.LexemeBuffer.Equals("0X"))
            {
                if (!CodifierHelpMeTo.readHexadecimalCharacter(abstract_source))
                    goto error_state;
                else
                {
                    /* read L U l u .. */
                    CodifierHelpMeTo.readIntegerTypeSuffixCharacter(abstract_source);

                    return new CodifierToken(CodifierTokenType.TT_TOKEN_LITERAL_INTEGER_HEXADECIMAL, abstract_source.LexemeBuffer,
                        temp_line_number, abstract_source.LexemeStartPosition, null);
                }
            }

            /* end check for hexadecimal */

            if (abstract_source.IsEOS && abstract_source.CurrentCharacter != '.')
                goto accept_digit;

            /* read digits */
            while (abstract_source.readNextCharacter())
            {
                if (!Char.IsDigit(abstract_source.CurrentCharacter))
                    break;
            }

            if (abstract_source.IsEOS && Char.IsDigit(abstract_source.CurrentCharacter))
                goto accept_digit;

            /* check for dot [REAL NUMBER]*/
         
            if (abstract_source.CurrentCharacter == '.')
            {
            
                abstract_source.readNextCharacter();
                if (!Char.IsDigit(abstract_source.CurrentCharacter)) // There is no digit after the dot
                {
                    abstract_source.ErrorMessage = "Incomplete real number";
                    goto error_state;
                }
                else
                    goto after_dot_state;
            }
            else if (Char.ToUpper(abstract_source.CurrentCharacter) == 'E')
                goto exponent_state;
            else
            {
                abstract_source.readPreviousCharacter();
                goto accept_digit;
            }

        after_dot_state:
            CodifierHelpMeTo.readDecimalCharacter(abstract_source);
            if (abstract_source.IsEOS)
                goto accept_real;

            /* check for e|E character */
            abstract_source.readNextCharacter();

            if (Char.ToUpper(abstract_source.CurrentCharacter) == 'E')
                goto exponent_state;

            if (!CodifierValues.RealTypeSuffixValues.Contains(abstract_source.CurrentCharacter.ToString()))
                abstract_source.readPreviousCharacter();

        accept_real:
            return new CodifierToken(CodifierTokenType.TT_TOKEN_LITERAL_REAL_WITH_POINT, abstract_source.LexemeBuffer,
                   temp_line_number, abstract_source.LexemeStartPosition, null);
        // after dot
        exponent_state:

            if (!CodifierHelpMeTo.readRealExponentPart(abstract_source))
                goto error_state;
            else
            {
                /*check for any suffix d D f F m M*/
                CodifierHelpMeTo.readRealTypeSuffixCharacter(abstract_source);

                return new CodifierToken(CodifierTokenType.TT_TOKEN_LITERAL_REAL_WITH_EXPONENT, abstract_source.LexemeBuffer,
                    temp_line_number, abstract_source.LexemeStartPosition, null);
            }

        accept_digit:
            return new CodifierToken(CodifierTokenType.TT_TOKEN_LITERAL_INTEGER_DECIMAL, abstract_source.LexemeBuffer,
                   temp_line_number, abstract_source.LexemeStartPosition, null);

        error_state:
            return CodifierStatesAPI.error(abstract_source);
        }


        //////////////////////////////////////////////////////////////////////////////////////
        /* String State */
        //////////////////////////////////////////////////////////////////////////////////////

        public static CodifierToken str(CodifierAbstractSource abstract_source)
        {
            if (abstract_source == null)
                return null;
            CodifierTokenType token_type;
            int temp_line_number = abstract_source.LineNumber;

            if (CodifierHelpMeTo.readString(abstract_source))
            {
                if (abstract_source.LexemeBuffer.StartsWith("@"))
                    token_type = CodifierTokenType.TT_TOKEN_LITERAL_VERBATIM_STRING;
                else
                    token_type = CodifierTokenType.TT_TOKEN_LITERAL_REGULAR_STRING;
            }
            else
                token_type = CodifierTokenType.TT_TOKEN_ERROR;

            if(token_type == CodifierTokenType.TT_TOKEN_ERROR)
                return CodifierStatesAPI.error(abstract_source);

            return new CodifierToken(token_type, abstract_source.LexemeBuffer, temp_line_number,
                  abstract_source.LexemeBufferCurrentPosition, null);
        }


        //////////////////////////////////////////////////////////////////////////////////////
        /* Operators States */
        //////////////////////////////////////////////////////////////////////////////////////

        /* State begin with = character */
        public static CodifierToken equal(CodifierAbstractSource abstract_source)
        {
            if (abstract_source == null)
                return null;

            int temp_line_number = abstract_source.LineNumber;

            if (abstract_source.IsEOS)
                goto accept_equal;

            abstract_source.readNextCharacter();

            if (abstract_source.CurrentCharacter == '=')
                return new CodifierToken(CodifierTokenType.TT_TOKEN_OPERATOR_EQUAL_EQUAL, abstract_source.LexemeBuffer,
                    temp_line_number, abstract_source.LexemeStartPosition, null);
            else
                abstract_source.readPreviousCharacter();

        accept_equal:
            return new CodifierToken(CodifierTokenType.TT_TOKEN_OPERATOR_EQUAL, abstract_source.LexemeBuffer,
                   temp_line_number, abstract_source.LexemeStartPosition, null);

        }

        /* State begin with < character */
        public static CodifierToken lessThan(CodifierAbstractSource abstract_source)
        {
            if (abstract_source == null)
                return null;

            int temp_line_number = abstract_source.LineNumber;

            if (abstract_source.IsEOS)
                goto accept_lt;

            abstract_source.readNextCharacter();

            if (abstract_source.CurrentCharacter != '=' &&
               abstract_source.CurrentCharacter != '<')
            {
                abstract_source.readPreviousCharacter();
                goto accept_lt;
            }

            if (abstract_source.CurrentCharacter == '=')
                return new CodifierToken(CodifierTokenType.TT_TOKEN_OPERATOR_LESS_THAN_OR_EQUAL, abstract_source.LexemeBuffer,
                    temp_line_number, abstract_source.LexemeStartPosition, null);
            else // <<
            {
                if (abstract_source.IsEOS)
                    goto accept_shift_left_state;

                abstract_source.readNextCharacter();

                if(abstract_source.CurrentCharacter == '=')
                    return new CodifierToken(CodifierTokenType.TT_TOKEN_OPERATOR_SHIFT_LEFT_EQUAL, abstract_source.LexemeBuffer,
                    temp_line_number, abstract_source.LexemeStartPosition, null);

                abstract_source.readPreviousCharacter();

                goto accept_shift_left_state;
            }

        accept_lt:
            return new CodifierToken(CodifierTokenType.TT_TOKEN_OPERATOR_LESS_THAN, abstract_source.LexemeBuffer,
                   temp_line_number, abstract_source.LexemeStartPosition, null);
            
       accept_shift_left_state:
            return new CodifierToken(CodifierTokenType.TT_TOKEN_OPERATOR_SHIFT_LEFT, abstract_source.LexemeBuffer,
               temp_line_number, abstract_source.LexemeStartPosition, null);
        }

        /* State begin with > character */
        public static CodifierToken greaterThan(CodifierAbstractSource abstract_source)
        {
            if (abstract_source == null)
                return null;

            int temp_line_number = abstract_source.LineNumber;

            if (abstract_source.IsEOS)
                goto accept_gt;

            abstract_source.readNextCharacter();

            if (abstract_source.CurrentCharacter != '=' &&
               abstract_source.CurrentCharacter != '>')
            {
                abstract_source.readPreviousCharacter();
                goto accept_gt;
            }

            if (abstract_source.CurrentCharacter == '=')
                return new CodifierToken(CodifierTokenType.TT_TOKEN_OPERATOR_GREATER_THAN_OR_EQUAL, abstract_source.LexemeBuffer,
                    temp_line_number, abstract_source.LexemeStartPosition, null);
            else // <<
            {
                if (abstract_source.IsEOS)
                    goto accept_shift_right_state;

                abstract_source.readNextCharacter();

                if (abstract_source.CurrentCharacter == '=')
                    return new CodifierToken(CodifierTokenType.TT_TOKEN_OPERATOR_SHIFT_RIGHT_EQUAL, abstract_source.LexemeBuffer,
                    temp_line_number, abstract_source.LexemeStartPosition, null);

                abstract_source.readPreviousCharacter();

                goto accept_shift_right_state;
            }

        accept_gt:
            return new CodifierToken(CodifierTokenType.TT_TOKEN_OPERATOR_GREATER_THAN, abstract_source.LexemeBuffer,
                   temp_line_number, abstract_source.LexemeStartPosition, null);

        accept_shift_right_state:
            return new CodifierToken(CodifierTokenType.TT_TOKEN_OPERATOR_SHIFT_RIGHT, abstract_source.LexemeBuffer,
               temp_line_number, abstract_source.LexemeStartPosition, null);
        }


        /* State begin with + character */
        public static CodifierToken plus(CodifierAbstractSource abstract_source)
        {
            if (abstract_source == null)
                return null;

            int temp_line_number = abstract_source.LineNumber;

            if (abstract_source.IsEOS)
                goto accept_plus;

            abstract_source.readNextCharacter();

            if (abstract_source.CurrentCharacter != '+' &&
               abstract_source.CurrentCharacter != '=')
            {
                abstract_source.readPreviousCharacter();
                goto accept_plus;
            }

            if (abstract_source.CurrentCharacter == '=') //+=
                return new CodifierToken(CodifierTokenType.TT_TOKEN_OPERATOR_PLUS_EQUAL, abstract_source.LexemeBuffer,
                    temp_line_number, abstract_source.LexemeStartPosition, null);
            else // ++
                return new CodifierToken(CodifierTokenType.TT_TOKEN_OPERATOR_PLUS_PLUS, abstract_source.LexemeBuffer,
                   temp_line_number, abstract_source.LexemeStartPosition, null);

        accept_plus:
            return new CodifierToken(CodifierTokenType.TT_TOKEN_OPERATOR_PLUS, abstract_source.LexemeBuffer,
                   temp_line_number, abstract_source.LexemeStartPosition, null);

        }

        /* State begin with - character */
        public static CodifierToken minus(CodifierAbstractSource abstract_source)
        {
            if (abstract_source == null)
                return null;

            int temp_line_number = abstract_source.LineNumber;

            if (abstract_source.IsEOS)
                goto accept_minus;

            abstract_source.readNextCharacter();

            if (abstract_source.CurrentCharacter != '-' &&
               abstract_source.CurrentCharacter != '=' &&
               abstract_source.CurrentCharacter != '>')
            {
                abstract_source.readPreviousCharacter();
                goto accept_minus;
            }

            if (abstract_source.CurrentCharacter == '=') //-=
                return new CodifierToken(CodifierTokenType.TT_TOKEN_OPERATOR_MINUS_EQUAL, abstract_source.LexemeBuffer,
                    temp_line_number, abstract_source.LexemeStartPosition, null);
            else if (abstract_source.CurrentCharacter == '-') // --
                return new CodifierToken(CodifierTokenType.TT_TOKEN_OPERATOR_MINUS_MINUS, abstract_source.LexemeBuffer,
                   temp_line_number, abstract_source.LexemeStartPosition, null);
            else // ->
                return new CodifierToken(CodifierTokenType.TT_TOKEN_OPERATOR_MINUS_GREATER_THAN, abstract_source.LexemeBuffer,
                   temp_line_number, abstract_source.LexemeStartPosition, null);


        accept_minus:
            return new CodifierToken(CodifierTokenType.TT_TOKEN_OPERATOR_MINUS, abstract_source.LexemeBuffer,
                   temp_line_number, abstract_source.LexemeStartPosition, null);

        }


        /* State begin with & character */
        public static CodifierToken and(CodifierAbstractSource abstract_source)
        {
            if (abstract_source == null)
                return null;

            int temp_line_number = abstract_source.LineNumber;

            if (abstract_source.IsEOS)
                goto accept_bitwise_and;

            abstract_source.readNextCharacter();

            if (abstract_source.CurrentCharacter != '&' &&
                abstract_source.CurrentCharacter != '=')
            {
                abstract_source.readPreviousCharacter();
                goto accept_bitwise_and;
            }

            if(abstract_source.CurrentCharacter == '&')     //&&
                return new CodifierToken(CodifierTokenType.TT_TOKEN_OPERATOR_LOGICAL_AND, abstract_source.LexemeBuffer,
                   temp_line_number, abstract_source.LexemeStartPosition, null);
            else // &=
                return new CodifierToken(CodifierTokenType.TT_TOKEN_OPERATOR_BITWISE_AND_EQUAL, abstract_source.LexemeBuffer,
                   temp_line_number, abstract_source.LexemeStartPosition, null);


        accept_bitwise_and:
            return new CodifierToken(CodifierTokenType.TT_TOKEN_OPERATOR_BITWISE_AND, abstract_source.LexemeBuffer,
                   temp_line_number, abstract_source.LexemeStartPosition, null);

        }

        /* State begin with | character */
        public static CodifierToken or(CodifierAbstractSource abstract_source)
        {
            if (abstract_source == null)
                return null;

            int temp_line_number = abstract_source.LineNumber;

            if (abstract_source.IsEOS)
                goto accept_bitwise_or;

            abstract_source.readNextCharacter();

            if (abstract_source.CurrentCharacter != '|' &&
                abstract_source.CurrentCharacter != '=')
            {
                abstract_source.readPreviousCharacter();
                goto accept_bitwise_or;
            }

            if (abstract_source.CurrentCharacter == '|')     //||
                return new CodifierToken(CodifierTokenType.TT_TOKEN_OPERATOR_LOGICAL_OR, abstract_source.LexemeBuffer,
                   temp_line_number, abstract_source.LexemeStartPosition, null);
            else // |=
                return new CodifierToken(CodifierTokenType.TT_TOKEN_OPERATOR_BITWISE_OR_EQUAL, abstract_source.LexemeBuffer,
                   temp_line_number, abstract_source.LexemeStartPosition, null);


        accept_bitwise_or:
            return new CodifierToken(CodifierTokenType.TT_TOKEN_OPERATOR_BITWISE_OR, abstract_source.LexemeBuffer,
                   temp_line_number, abstract_source.LexemeStartPosition, null);

        }

        /* State begin with ! character */
        public static CodifierToken not(CodifierAbstractSource abstract_source)
        {
            if (abstract_source == null)
                return null;

            int temp_line_number = abstract_source.LineNumber;

            if (abstract_source.IsEOS)
                goto accept_logical_not;

            abstract_source.readNextCharacter();

            if (abstract_source.CurrentCharacter != '=')
            {
                abstract_source.readPreviousCharacter();
                goto accept_logical_not;
            }

            //!=
            return new CodifierToken(CodifierTokenType.TT_TOKEN_OPERATOR_NOT_EQUAL, abstract_source.LexemeBuffer,
                   temp_line_number, abstract_source.LexemeStartPosition, null);

        accept_logical_not:
            return new CodifierToken(CodifierTokenType.TT_TOKEN_OPERATOR_NOT, abstract_source.LexemeBuffer,
                   temp_line_number, abstract_source.LexemeStartPosition, null);

        }

        /* State begin with * character */
        public static CodifierToken multiply(CodifierAbstractSource abstract_source)
        {
            if (abstract_source == null)
                return null;

            int temp_line_number = abstract_source.LineNumber;

            if (abstract_source.IsEOS)
                goto accept_multiply;

            abstract_source.readNextCharacter();

            if (abstract_source.CurrentCharacter != '=')
            {
                abstract_source.readPreviousCharacter();
                goto accept_multiply;
            }

            //*=
            return new CodifierToken(CodifierTokenType.TT_TOKEN_OPERATOR_MINUS_EQUAL, abstract_source.LexemeBuffer,
                   temp_line_number, abstract_source.LexemeStartPosition, null);

        accept_multiply:
            return new CodifierToken(CodifierTokenType.TT_TOKEN_OPERATOR_MULTIPLY, abstract_source.LexemeBuffer,
                   temp_line_number, abstract_source.LexemeStartPosition, null);

        }

        /* State begin with / character */
        public static CodifierToken divide(CodifierAbstractSource abstract_source)
        {
            if (abstract_source == null)
                return null;

            int temp_line_number = abstract_source.LineNumber;

            if (abstract_source.IsEOS)
                goto accept_divide;

            
            abstract_source.readNextCharacter();

            if (abstract_source.CurrentCharacter != '=' &&
                abstract_source.CurrentCharacter != '/' &&
                abstract_source.CurrentCharacter != '*')
            {
                abstract_source.readPreviousCharacter();
                goto accept_divide;
            }

            if (abstract_source.CurrentCharacter == '=')
                return new CodifierToken(CodifierTokenType.TT_TOKEN_OPERATOR_DIVIDE_EQUAL, abstract_source.LexemeBuffer,
                   temp_line_number, abstract_source.LexemeStartPosition, null);
            else if (abstract_source.CurrentCharacter == '/') // inline comment (//)
            {
               
                abstract_source.readNextCharacter();
                while (abstract_source.CurrentCharacter != '\n' && (!abstract_source.IsEOS))
                {
                    abstract_source.readNextCharacter();
                }

                if (abstract_source.CurrentCharacter == '\n')
                {
                    abstract_source.bufferGoBackOneCharacter();
                    abstract_source.addToBuffer("\0");
                }

                return new CodifierToken(CodifierTokenType.TT_TOKEN_INLINE_COMMENT, abstract_source.LexemeBuffer,
                   temp_line_number, abstract_source.LexemeStartPosition, null);

            }
            else /* multi-lines comment */
            {
                /* to keep the line number of the start of the comment */
                //int temp_line_number = abstract_source.LineNumber;

                abstract_source.readNextCharacter();
                while (!abstract_source.IsEOS)
                {
                    if (abstract_source.CurrentCharacter == '*')
                    {
                        abstract_source.readNextCharacter();
                        if (abstract_source.CurrentCharacter == '/')
                            return new CodifierToken(CodifierTokenType.TT_TOKEN_MULTI_LINES_COMMENT, abstract_source.LexemeBuffer,
                                temp_line_number, abstract_source.LexemeStartPosition, null);
                        else
                            abstract_source.readPreviousCharacter();
                    }

                    abstract_source.readNextCharacter();
                }

                abstract_source.ErrorMessage = "Invalid multi-lines comment";
                return CodifierStatesAPI.error(abstract_source);
            }


        accept_divide:
            return new CodifierToken(CodifierTokenType.TT_TOKEN_OPERATOR_DIVIDE, abstract_source.LexemeBuffer,
                   temp_line_number, abstract_source.LexemeStartPosition, null);

        }

        /* State begin with % character */
        public static CodifierToken percent(CodifierAbstractSource abstract_source)
        {
            if (abstract_source == null)
                return null;

            int temp_line_number = abstract_source.LineNumber;

            if (abstract_source.IsEOS)
                goto accept_percent;

            abstract_source.readNextCharacter();

            if (abstract_source.CurrentCharacter != '=')
            {
                abstract_source.readPreviousCharacter();
                goto accept_percent;
            }

            // %=
            return new CodifierToken(CodifierTokenType.TT_TOKEN_OPERATOR_PERCENT_EQUAL, abstract_source.LexemeBuffer,
                   temp_line_number, abstract_source.LexemeStartPosition, null);

        accept_percent:
            return new CodifierToken(CodifierTokenType.TT_TOKEN_OPERATOR_PERCENT, abstract_source.LexemeBuffer,
                   temp_line_number, abstract_source.LexemeStartPosition, null);

        }

        /* State begin with ^ character */
        public static CodifierToken caret(CodifierAbstractSource abstract_source)
        {
            if (abstract_source == null)
                return null;

            int temp_line_number = abstract_source.LineNumber;

            if (abstract_source.IsEOS)
                goto accept_caret;

            abstract_source.readNextCharacter();

            if (abstract_source.CurrentCharacter != '=')
            {
                abstract_source.readPreviousCharacter();
                goto accept_caret;
            }

            // ^=
            return new CodifierToken(CodifierTokenType.TT_TOKEN_OPERATOR_CARET_EQUAL, abstract_source.LexemeBuffer,
                   temp_line_number, abstract_source.LexemeStartPosition, null);

        accept_caret:
            return new CodifierToken(CodifierTokenType.TT_TOKEN_OPERATOR_CARET, abstract_source.LexemeBuffer,
                   temp_line_number, abstract_source.LexemeStartPosition, null);

        }

        /* State begin with . character */
        public static CodifierToken dot(CodifierAbstractSource abstract_source)
        {
            if (abstract_source == null)
                return null;

            int temp_line_number = abstract_source.LineNumber;

            if (abstract_source.IsEOS)
                goto accept_dot;

            abstract_source.readNextCharacter();

            if (!Char.IsDigit(abstract_source.CurrentCharacter))
            {
                abstract_source.readPreviousCharacter();
                goto accept_dot;
            }

           // abstract_source.readPreviousCharacter();
            return CodifierStatesAPI.number(abstract_source);

        accept_dot:
            return new CodifierToken(CodifierTokenType.TT_TOKEN_PUNCTUATOR_DOT, abstract_source.LexemeBuffer,
                   temp_line_number, abstract_source.LexemeStartPosition, null);

        }
    }
}
